using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl33Ordo;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl30LegioViewModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl30Legio;

public class Tbl30LegioViewModel : BaseBossListViewModel<Models.Tbl30Legio>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl30Legio>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel LegioCommentViewModel { get; }
    public Tbl90ReferenceViewModel LegioReferenceViewModel { get; }
    public Tbl33OrdoViewModel OrdoViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl27Infraclass> InfraclassList { get; private set; } = new();
    public Dictionary<int, string?> InfraclassLookup { get; private set; } = new();

    public async Task LoadInfraclassLookupAsync()
    {
        var list = await _lookupService.GetInfraclassListAsync();
        var lookup = await _lookupService.GetInfraclassLookupAsync();

        InfraclassList.Clear();
        foreach (var item in list)
        {
            InfraclassList.Add(item);
        }

        InfraclassLookup = lookup;
        OnPropertyChanged(nameof(InfraclassLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl30LegioViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        LegioCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        LegioReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        OrdoViewModel = new Tbl33OrdoViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();
    }

    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_LegioId")) ? "Id" : loc.GetLocalizedString("HeaderSort_LegioId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_LegioName")) ? "Name" : loc.GetLocalizedString("HeaderSort_LegioName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------               
    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
    {
        SelectedDetailTabIndex = index;
    });

    protected override void OnSelectedDetailTabIndexChanged()
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

        if (SelectedDetailTabIndex == 2)
            _ = LegioReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = OrdoViewModel.OrdoReferenceViewModel.LoadReferenceLookupsAsync();

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    public async Task ApplyFilterAsync(string? filter)
    {
        if (IsBossMode)
        {
            SearchText = filter;
            await ReloadAsync();
        }
    }

    public bool IsOrdoParentSelected => OrdoViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadInfraclassLookupAsync();

        Items = new ObservableCollection<Models.Tbl30Legio>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForInfraclassAsync(int infraclassId)
        => await LoadInternalAsync(infraclassId, Data.GetLegioForInfraclassAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl30Legio>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl30Legio>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl30Legio>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchLegioWithInfraclassAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl30Legio>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        // 1. BACK → SelectedItem == null
        if (SelectedItem == null)
        {
            OnPropertyChanged(nameof(IsDetailVisible));
            await ClearChildCollectionsAsync();
            SelectedDetailTabIndex = 0;
            return;
        }

        // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
        if (IsInParentMode)
        {
            await LoadChildrenAsync(GetId(SelectedItem));
            return;
        }

        // 3. Boss-Modus → volle Detail-Logik
        OnPropertyChanged(nameof(IsDetailVisible));
        await LoadChildrenAsync(GetId(SelectedItem));
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await LegioCommentViewModel.LoadForLegioAsync(entityId);
        await LegioReferenceViewModel.LoadForLegioAsync(entityId);

        await OrdoViewModel.LoadLegioLookupAsync();
        await OrdoViewModel.LoadForLegioAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        OrdoViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        LegioCommentViewModel.Items?.Clear();
        LegioReferenceViewModel.Items?.Clear();
        OrdoViewModel.Items?.Clear();

        if (IsBossMode)
        {
            LegioCommentViewModel.SelectedItem = null;
            LegioReferenceViewModel.SelectedItem = null;
            OrdoViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl30Legio item, string column)
        => column switch
        {
            "Id" => item.LegioId,
            "Name" => item.LegioName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl30Legio CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl30Legio
        {
            InfraclassId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl30Legio?> SaveItemAsync(Models.Tbl30Legio item)
    {
        if (string.IsNullOrWhiteSpace(item.LegioName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertLegioAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl30Legio item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.LegioName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl30Legio item)
    {
        if (item.LegioId == 0)
            return false;

        if (await Data.LegioHasOrdoAsync(item.LegioId) ||
            await Data.LegioHasReferenceAsync(item.LegioId) ||
            await Data.LegioHasCommentAsync(item.LegioId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteLegioAsync(item.LegioId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl30Legio item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl30Legio item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl30Legio item)
        => item.LegioId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        // ⭐ Back MUSS SelectedItem löschen
        SelectedItem = null;

        // ⭐ Detail schließen
        OnPropertyChanged(nameof(IsDetailVisible));

        // Parent-Modus bleibt aktiv
        IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
