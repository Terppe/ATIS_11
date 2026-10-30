using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl48Subfamily;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl45FamilyViewModel Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl45Family;

public class Tbl45FamilyViewModel : BaseBossListViewModel<Models.Tbl45Family>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl45Family>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel FamilyCommentViewModel { get; }
    public Tbl90ReferenceViewModel FamilyReferenceViewModel { get; }
    public Tbl48SubfamilyViewModel SubfamilyViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl42Superfamily> SuperfamilyList { get; private set; } = new();
    public Dictionary<int, string?> SuperfamilyLookup { get; private set; } = new();

    public async Task LoadSuperfamilyLookupAsync()
    {
        var list = await _lookupService.GetSuperfamilyListAsync();
        var lookup = await _lookupService.GetSuperfamilyLookupAsync();

        SuperfamilyList.Clear();
        foreach (var item in list)
        {
            SuperfamilyList.Add(item);
        }

        SuperfamilyLookup = lookup;
        OnPropertyChanged(nameof(SuperfamilyLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl45FamilyViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        FamilyCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        FamilyReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SubfamilyViewModel = new Tbl48SubfamilyViewModel(factory, dialog, coordinator, lookupService, user);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_FamilyId")) ? "Id" : loc.GetLocalizedString("HeaderSort_FamilyId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_FamilyName")) ? "Name" : loc.GetLocalizedString("HeaderSort_FamilyName"));
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
            _ = FamilyReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = SubfamilyViewModel.SubfamilyReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsSubfamilyParentSelected => SubfamilyViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadSuperfamilyLookupAsync();

        Items = new ObservableCollection<Models.Tbl45Family>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSuperfamilyAsync(int superfamilyId)
        => await LoadInternalAsync(superfamilyId, Data.GetFamilyForSuperfamilyAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl45Family>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl45Family>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl45Family>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchFamilyWithSuperfamilyAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl45Family>();
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
        await FamilyCommentViewModel.LoadForFamilyAsync(entityId);
        await FamilyReferenceViewModel.LoadForFamilyAsync(entityId);

        await SubfamilyViewModel.LoadFamilyLookupAsync();
        await SubfamilyViewModel.LoadForFamilyAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        SubfamilyViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        FamilyCommentViewModel.Items?.Clear();
        FamilyReferenceViewModel.Items?.Clear();
        SubfamilyViewModel.Items?.Clear();

        if (IsBossMode)
        {
            FamilyCommentViewModel.SelectedItem = null;
            FamilyReferenceViewModel.SelectedItem = null;
            SubfamilyViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl45Family item, string column)
        => column switch
        {
            "Id" => item.FamilyId,
            "Name" => item.FamilyName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl45Family CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl45Family
        {
            SuperfamilyId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl45Family?> SaveItemAsync(Models.Tbl45Family item)
    {
        if (string.IsNullOrWhiteSpace(item.FamilyName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertFamilyAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl45Family item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.FamilyName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl45Family item)
    {
        if (item.FamilyId == 0)
            return false;

        if (await Data.FamilyHasSubfamilyAsync(item.FamilyId) ||
            await Data.FamilyHasReferenceAsync(item.FamilyId) ||
            await Data.FamilyHasCommentAsync(item.FamilyId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteFamilyAsync(item.FamilyId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl45Family item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl45Family item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl45Family item)
        => item.FamilyId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
}
