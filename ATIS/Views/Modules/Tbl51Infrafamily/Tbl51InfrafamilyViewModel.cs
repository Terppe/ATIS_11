using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl54Supertribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;


//    Tbl51InfrafamilyViewModel Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public class Tbl51InfrafamilyViewModel : BaseBossListViewModel<Models.Tbl51Infrafamily>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl51Infrafamily>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    public Tbl93CommentViewModel InfrafamilyCommentViewModel { get; }
    public Tbl90ReferenceViewModel InfrafamilyReferenceViewModel { get; }
    public Tbl54SupertribusViewModel SupertribusViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl48Subfamily> SubfamilyList { get; private set; } = new();
    public Dictionary<int, string?> SubfamilyLookup { get; private set; } = new();

    public async Task LoadSubfamilyLookupAsync()
    {
        var list = await _lookupService.GetSubfamilyListAsync();
        var lookup = await _lookupService.GetSubfamilyLookupAsync();

        SubfamilyList.Clear();
        foreach (var item in list)
        {
            SubfamilyList.Add(item);
        }

        SubfamilyLookup = lookup;
        OnPropertyChanged(nameof(SubfamilyLookup));
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl51InfrafamilyViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        InfrafamilyCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        InfrafamilyReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SupertribusViewModel = new Tbl54SupertribusViewModel(factory, dialog, coordinator, lookupService, user);

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
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_InfrafamilyId")) ? "Id" : loc.GetLocalizedString("HeaderSort_InfrafamilyId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_InfrafamilyName")) ? "Name" : loc.GetLocalizedString("HeaderSort_InfrafamilyName"));
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
            _ = InfrafamilyReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailTabIndex == 5)
            _ = SupertribusViewModel.SupertribusReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsSupertribusParentSelected => SupertribusViewModel.SelectedItem != null;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        await LoadSubfamilyLookupAsync();

        Items = new ObservableCollection<Models.Tbl51Infrafamily>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSubfamilyAsync(int subfamilyId)
        => await LoadInternalAsync(subfamilyId, Data.GetInfrafamilyForSubfamilyAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl51Infrafamily>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl51Infrafamily>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl51Infrafamily>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchInfrafamilyWithSubfamilyAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl51Infrafamily>();
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
        await InfrafamilyCommentViewModel.LoadForInfrafamilyAsync(entityId);
        await InfrafamilyReferenceViewModel.LoadForInfrafamilyAsync(entityId);

        await SupertribusViewModel.LoadInfrafamilyLookupAsync();
        await SupertribusViewModel.LoadForInfrafamilyAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        SupertribusViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        InfrafamilyCommentViewModel.Items?.Clear();
        InfrafamilyReferenceViewModel.Items?.Clear();
        SupertribusViewModel.Items?.Clear();

        if (IsBossMode)
        {
            InfrafamilyCommentViewModel.SelectedItem = null;
            InfrafamilyReferenceViewModel.SelectedItem = null;
            SupertribusViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl51Infrafamily item, string column)
        => column switch
        {
            "Id" => item.InfrafamilyId,
            "Name" => item.InfrafamilyName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl51Infrafamily CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl51Infrafamily
        {
            SubfamilyId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl51Infrafamily?> SaveItemAsync(Models.Tbl51Infrafamily item)
    {
        if (string.IsNullOrWhiteSpace(item.InfrafamilyName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertInfrafamilyAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl51Infrafamily item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.InfrafamilyName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl51Infrafamily item)
    {
        if (item.InfrafamilyId == 0)
            return false;

        if (await Data.InfrafamilyHasSupertribusAsync(item.InfrafamilyId) ||
            await Data.InfrafamilyHasReferenceAsync(item.InfrafamilyId) ||
            await Data.InfrafamilyHasCommentAsync(item.InfrafamilyId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteInfrafamilyAsync(item.InfrafamilyId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl51Infrafamily item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl51Infrafamily item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl51Infrafamily item)
        => item.InfrafamilyId;

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
