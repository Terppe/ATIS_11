using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl27Infraclass;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;


//    Tbl24SubclassViewModel Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl24Subclass;

public class Tbl24SubclassViewModel : BaseBossListViewModel<Models.Tbl24Subclass>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl24Subclass>>>? _activeLoader;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel SubclassCommentViewModel { get; }
    public Tbl90ReferenceViewModel SubclassReferenceViewModel { get; }
    public Tbl27InfraclassViewModel InfraclassViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl21Class> ClassList { get; private set; } = new();
    public Dictionary<int, string?> ClassLookup { get; private set; } = new();

    public async Task LoadClassLookupAsync()
    {
        ClassList = await _lookupService.GetClassListAsync();
        ClassLookup = await _lookupService.GetClassLookupAsync();
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   

    public Tbl24SubclassViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SubclassCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SubclassReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        InfraclassViewModel = new Tbl27InfraclassViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Subclass ID");
        RegisterHeader("Name", "Subclass Name");
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
            _ = SubclassReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = InfraclassViewModel.InfraclassReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsInfraclassParentSelected => InfraclassViewModel.SelectedItem != null;
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

        await LoadClassLookupAsync();

        Items = new ObservableCollection<Models.Tbl24Subclass>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------

    public async Task LoadForClassAsync(int classeId)
        => await LoadInternalAsync(classeId, Data.GetSubclassForClassAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl24Subclass>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl24Subclass>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl24Subclass>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSubclassWithClassAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl24Subclass>();
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
        await SubclassCommentViewModel.LoadForSubclassAsync(entityId);
        await SubclassReferenceViewModel.LoadForSubclassAsync(entityId);

        await InfraclassViewModel.LoadSubclassLookupAsync();
        await InfraclassViewModel.LoadForSubclassAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        InfraclassViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SubclassCommentViewModel.Items?.Clear();
        SubclassReferenceViewModel.Items?.Clear();
        InfraclassViewModel.Items?.Clear();

        if (IsBossMode)
        {
            SubclassCommentViewModel.SelectedItem = null;
            SubclassReferenceViewModel.SelectedItem = null;
            InfraclassViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl24Subclass item, string column)
        => column switch
        {
            "Id" => item.SubclassId,
            "Name" => item.SubclassName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl24Subclass CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl24Subclass
        {
            ClassId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl24Subclass?> SaveItemAsync(Models.Tbl24Subclass item)
    {
        if (string.IsNullOrWhiteSpace(item.SubclassName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSubclassAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl24Subclass item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.SubclassName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl24Subclass item)
    {
        if (item.SubclassId == 0)
            return false;

        if (await Data.SubclassHasInfraclassAsync(item.SubclassId) ||
            await Data.SubclassHasReferenceAsync(item.SubclassId) ||
            await Data.SubclassHasCommentAsync(item.SubclassId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSubclassAsync(item.SubclassId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl24Subclass item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl24Subclass item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl24Subclass item)
        => item.SubclassId;

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
