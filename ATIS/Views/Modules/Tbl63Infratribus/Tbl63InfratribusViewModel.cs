using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;


//    Tbl63InfratribusViewModel Skriptdatum:  30.12.2024  10:32    

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusViewModel : BaseBossListViewModel<Models.Tbl63Infratribus>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl63Infratribus>>>? _activeLoader;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel InfratribusCommentViewModel { get; }
    public Tbl90ReferenceViewModel InfratribusReferenceViewModel { get; }
    public Tbl66GenusViewModel GenusViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; private set; } = new();
    public Dictionary<int, string?> SubtribusLookup { get; private set; } = new();

    public async Task LoadSubtribusLookupAsync()
    {
        SubtribusList = await _lookupService.GetSubtribusListAsync();
        SubtribusLookup = await _lookupService.GetSubtribusLookupAsync();
    }

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   

    public Tbl63InfratribusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        InfratribusCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        InfratribusReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        GenusViewModel = new Tbl66GenusViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Infratribus ID");
        RegisterHeader("Name", "Infratribus Name");
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
            _ = InfratribusReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = GenusViewModel.GenusReferenceViewModel.LoadReferenceLookupsAsync();

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

    public bool IsGenusParentSelected => GenusViewModel.SelectedItem != null;
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

        await LoadSubtribusLookupAsync();

        Items = new ObservableCollection<Models.Tbl63Infratribus>();

        OnPropertyChanged(nameof(HasItems));

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------

    public async Task LoadForSubtribusAsync(int subtribusId)
        => await LoadInternalAsync(subtribusId, Data.GetInfratribusForSubtribusAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl63Infratribus>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchInfratribusWithSubtribusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl63Infratribus>();
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
        await InfratribusCommentViewModel.LoadForInfratribusAsync(entityId);
        await InfratribusReferenceViewModel.LoadForInfratribusAsync(entityId);

        await GenusViewModel.LoadInfratribusLookupAsync();
        await GenusViewModel.LoadForInfratribusAsync(entityId);


        // ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        GenusViewModel.IsInParentMode = true;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        InfratribusCommentViewModel.Items?.Clear();
        InfratribusReferenceViewModel.Items?.Clear();
        GenusViewModel.Items?.Clear();

        if (IsBossMode)
        {
            InfratribusCommentViewModel.SelectedItem = null;
            InfratribusReferenceViewModel.SelectedItem = null;
            GenusViewModel.SelectedItem = null;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------

    protected override object GetSortKey(Models.Tbl63Infratribus item, string column)
        => column switch
        {
            "Id" => item.InfratribusId,
            "Name" => item.InfratribusName ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl63Infratribus CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl63Infratribus
        {
            SubtribusId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl63Infratribus?> SaveItemAsync(Models.Tbl63Infratribus item)
    {
        if (string.IsNullOrWhiteSpace(item.InfratribusName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertInfratribusAsync(item);
        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl63Infratribus item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.InfratribusName;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
    {
        if (item.InfratribusId == 0)
            return false;

        if (await Data.InfratribusHasGenusAsync(item.InfratribusId) ||
            await Data.InfratribusHasReferenceAsync(item.InfratribusId) ||
            await Data.InfratribusHasCommentAsync(item.InfratribusId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteInfratribusAsync(item.InfratribusId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl63Infratribus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl63Infratribus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl63Infratribus item)
        => item.InfratribusId;

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
