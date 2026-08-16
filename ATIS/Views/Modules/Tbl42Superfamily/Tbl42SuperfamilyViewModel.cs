using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl45Family;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl42SuperfamilyViewModel Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl42Superfamily;

public class Tbl42SuperfamilyViewModel : BaseBossListViewModel<Models.Tbl42Superfamily>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl42Superfamily>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel SuperfamilyCommentViewModel { get; }
    public Tbl90ReferenceViewModel SuperfamilyReferenceViewModel { get; }
    public Tbl45FamilyViewModel FamilyViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl39Infraordo> InfraordoList { get; private set; } = new();
    public Dictionary<int, string?> InfraordoLookup { get; private set; } = new();

    public async Task LoadInfraordoLookupAsync()
    {
        InfraordoList = await _lookupService.GetInfraordoListAsync();
        InfraordoLookup = await _lookupService.GetInfraordoLookupAsync();
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   

    public Tbl42SuperfamilyViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SuperfamilyCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SuperfamilyReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        FamilyViewModel = new Tbl45FamilyViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SuperfamilyId")) ? "Id" : loc.GetLocalizedString("HeaderSort_SuperfamilyId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_SuperfamilyName")) ? "Name" : loc.GetLocalizedString("HeaderSort_SuperfamilyName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------               

    private int _selectedDetailPivotIndex;
    public int SelectedDetailPivotIndex
    {
        get => _selectedDetailPivotIndex;
        set
        {
            if (SetProperty(ref _selectedDetailPivotIndex, value))
            {
                UpdateButtonStates();
            }
        }
    }

    private void UpdateButtonStates()
    {
        // Pivot 0 = Boss-Modus
        IsInParentMode = SelectedDetailPivotIndex != 0;

        if (SelectedDetailPivotIndex == 2)
            _ = SuperfamilyReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = FamilyViewModel.FamilyReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsFamilyParentSelected => FamilyViewModel.SelectedItem != null;
    public bool IsDetailVisible => SelectedItem != null;
    public bool IsEditMode => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadInfraordoLookupAsync();

        Items = new ObservableCollection<Models.Tbl42Superfamily>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------

    public async Task LoadForInfraordoAsync(int infraordoId)
        => await LoadInternalAsync(infraordoId, Data.GetSuperfamilyForInfraordoAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl42Superfamily>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl42Superfamily>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl42Superfamily>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSuperfamilyWithInfraordoAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl42Superfamily>();
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
            SelectedDetailPivotIndex = 0;
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
        await SuperfamilyCommentViewModel.LoadForSuperfamilyAsync(entityId);
        await SuperfamilyReferenceViewModel.LoadForSuperfamilyAsync(entityId);

        await FamilyViewModel.LoadSuperfamilyLookupAsync();
        await FamilyViewModel.LoadForSuperfamilyAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        FamilyViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SuperfamilyCommentViewModel.Items?.Clear();
        SuperfamilyReferenceViewModel.Items?.Clear();
        FamilyViewModel.Items?.Clear();

        if (IsBossMode)
        {
            SuperfamilyCommentViewModel.SelectedItem = null;
            SuperfamilyReferenceViewModel.SelectedItem = null;
            FamilyViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl42Superfamily item, string column)
        => column switch
        {
            "Id" => item.SuperfamilyId,
            "Name" => item.SuperfamilyName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl42Superfamily CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl42Superfamily
        {
            InfraordoId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl42Superfamily?> SaveItemAsync(Models.Tbl42Superfamily item)
    {
        if (string.IsNullOrWhiteSpace(item.SuperfamilyName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSuperfamilyAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl42Superfamily item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SuperfamilyName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl42Superfamily item)
    {
        if (item.SuperfamilyId == 0)
            return false;

        if (await Data.SuperfamilyHasFamilyAsync(item.SuperfamilyId) ||
            await Data.SuperfamilyHasReferenceAsync(item.SuperfamilyId) ||
            await Data.SuperfamilyHasCommentAsync(item.SuperfamilyId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSuperfamilyAsync(item.SuperfamilyId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl42Superfamily item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl42Superfamily item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl42Superfamily item)
        => item.SuperfamilyId;

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

        SelectedDetailPivotIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }



}
