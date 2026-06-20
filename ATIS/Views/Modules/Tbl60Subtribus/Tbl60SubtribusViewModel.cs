using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public class Tbl60SubtribusViewModel : BaseBossListViewModel<Models.Tbl60Subtribus>
{
    private  readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    // Parent-Mechanik
    private Action<Models.Tbl60Subtribus>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl60Subtribus>>>? _loadByParent;
    private int _currentParentId;

    // Child-VMs
    public Tbl93CommentViewModel SubtribusCommentViewModel { get; }
    public Tbl90ReferenceViewModel SubtribusReferenceViewModel { get; }
    public Tbl63InfratribusViewModel InfratribusViewModel { get; }

    public Tbl60SubtribusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SubtribusCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SubtribusReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        InfratribusViewModel = new Tbl63InfratribusViewModel(factory, dialog, coordinator, lookupService, user);

        IsSubtribusDetailVisible = false;
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }

    // ---------------------------------------------------------
    // UI STATE
    // ---------------------------------------------------------

    private bool _isSubtribusDetailVisible;
    public bool IsSubtribusDetailVisible
    {
        get => _isSubtribusDetailVisible;
        set => SetProperty(ref _isSubtribusDetailVisible, value);
    }
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
        // Nur Pivot 0 = Boss-Modus
        IsInParentMode = SelectedDetailPivotIndex != 0;

        // ⭐ Lookups laden, wenn Reference-Pivot aktiv ist
        if (SelectedDetailPivotIndex == 2) // Subtribus-References
        {
            _ = SubtribusReferenceViewModel.LoadReferenceLookupsAsync();
        }

        if (SelectedDetailPivotIndex == 5) // Infratribus-References
        {
            _ = InfratribusViewModel.InfratribusReferenceViewModel.LoadReferenceLookupsAsync();
        }

        UpdateRefreshState();

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    //private void UpdateButtonStates()
    //{
    //    // Pivot 0 = Subtribus Edit (Boss)
    //    // Pivot 1 = Subtribus Comments (Parent)
    //    // Pivot 2 = Subtribus References (Parent)
    //    // Pivot 3 = Infratribus Edit (Parent)
    //    // Pivot 4 = Infratribus Comments (Parent)
    //    // Pivot 5 = Infratribus References (Parent)

    //    // ⭐ Nur Pivot 0 ist Boss-Modus
    //    IsInParentMode = SelectedDetailPivotIndex != 0;

    //    // ⭐ Refresh-Status aktualisieren (Items leer? Parent aktiv?)
    //    UpdateRefreshState();

    //    // ⭐ Commands aktualisieren
    //    NewCommand.NotifyCanExecuteChanged();
    //    RefreshCommand.NotifyCanExecuteChanged();
    //}

    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;
        await ReloadAsync();
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // LOOKUP: TRIBUS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl57Tribus> TribusList { get; private set; } = new();
    public Dictionary<int, string?> TribusLookup { get; private set; } = new();

    //private bool _isLoadingTribus;

    //private async Task LoadTribusAsync()
    //{
    //    if (_isLoadingTribus)
    //        return;

    //    _isLoadingTribus = true;

    //    try
    //    {
    //        TribusList = await _lookupService.GetTribusListAsync();
    //        TribusLookup = await _lookupService.GetTribusLookupAsync();
    //    }
    //    finally
    //    {
    //        _isLoadingTribus = false;
    //    }
    //}

    public async Task LoadTribusLookupAsync()
    {
        TribusList = await _lookupService.GetTribusListAsync();
        TribusLookup = await _lookupService.GetTribusLookupAsync();
    }

    // ---------------------------------------------------------
    // PARENT SYSTEM
    // ---------------------------------------------------------
    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl57Tribus))
        {
            _assignParentFk = s => s.TribusId = parentId;
            _loadByParent = Data.GetSubtribusForTribusAsync;
        }
    }

    // ---------------------------------------------------------
    // LOAD PARENT 
    // ---------------------------------------------------------

    public async Task LoadParentAsync<TParent>(int parentId)
    {
        _currentParentId = parentId;

        ResolveParent<TParent>(parentId);

        var items = _loadByParent != null
            ? await _loadByParent(parentId)
            : Enumerable.Empty<Models.Tbl60Subtribus>();

        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);

        DataSetCount = Items.Count;
    }

    public override async Task LoadParentAsync(int parentId)
    {
        if (_loadByParent == null)
            return;

        var items = await _loadByParent(parentId);
        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);

        DataSetCount = Items.Count;

        // ❗ KEIN Auto-Select
        SelectedItem = null;
        IsSubtribusDetailVisible = false;

        // ⭐ Child-VMs Parent laden, aber NICHT das EditControl öffnen
        await SubtribusCommentViewModel.LoadParentAsync(parentId);
        await SubtribusReferenceViewModel.LoadParentAsync(parentId);
    }
    // ---------------------------------------------------------
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------

    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _loadByParent = null;
        _currentParentId = 0;

        await LoadTribusLookupAsync();

        Items = new ObservableCollection<Models.Tbl60Subtribus>();
        SelectedItem = null;
        IsSubtribusDetailVisible = false;

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        if (_loadByParent != null && _currentParentId > 0)
            return await _loadByParent(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSubtribusWithTribusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl60Subtribus>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        IsSubtribusDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
            await LoadChildrenAsync(SelectedItem.SubtribusId);
        else
            await ClearChildCollectionsAsync();

        await base.OnSelectedItemChangedAsync();
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        // Subtribus Comments + References
        await SubtribusCommentViewModel.LoadParentAsync<Models.Tbl60Subtribus>(entityId);
        await SubtribusCommentViewModel.LoadParentAsync(entityId);

        await SubtribusReferenceViewModel.LoadParentAsync<Models.Tbl60Subtribus>(entityId);
        await SubtribusReferenceViewModel.LoadParentAsync(entityId);

        // ⭐ InfratribusVM vollständig initialisieren
        await InfratribusViewModel.LoadAsync();
        await InfratribusViewModel.LoadSubtribusLookupAsync();

        // Infratribus Liste + UI
        await InfratribusViewModel.LoadParentAsync<Models.Tbl60Subtribus>(entityId);
        await InfratribusViewModel.LoadParentAsync(entityId);
    }
    protected override Task ClearChildCollectionsAsync()
    {
        SubtribusCommentViewModel.Items?.Clear();
        SubtribusCommentViewModel.SelectedItem = null;

        SubtribusReferenceViewModel.Items?.Clear();
        SubtribusReferenceViewModel.SelectedItem = null;

        InfratribusViewModel.Items?.Clear();
        InfratribusViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsSubtribusDetailVisible = true;
        await Task.Delay(50);
    }

    protected override Models.Tbl60Subtribus CreateNewItem()
    {
        var s = new Models.Tbl60Subtribus();
        _assignParentFk?.Invoke(s);
        return s;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
    {
        if (string.IsNullOrWhiteSpace(item.SubtribusName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        // speichern
        var saved = await Data.UpsertSubtribusAsync(item);

        // ⭐ Parent-Liste neu laden
        if (_loadByParent != null && _currentParentId > 0)
        {
            var list = await _loadByParent(_currentParentId);
            Items = new ObservableCollection<Models.Tbl60Subtribus>(list);
            DataSetCount = Items.Count;

            // ⭐ neuen Datensatz auswählen
            SelectedItem = Items.FirstOrDefault(x => x.SubtribusId == saved.SubtribusId);

            // ⭐ SearchBox setzen
            SearchText = saved.SubtribusName;
        }
        else
        {
            // Boss-Modus (keine Parent-Filterung)
            await ReloadAsync();

            SelectedItem = Items.FirstOrDefault(x => x.SubtribusId == saved.SubtribusId);
            SearchText = saved.SubtribusName;
        }

        return saved;
    }

    //protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
    //{
    //    if (string.IsNullOrWhiteSpace(item.SubtribusName))
    //    {
    //        await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
    //        return null;
    //    }

    //    var saved = await Data.UpsertSubtribusAsync(item);
    //    return saved;
    //}

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
    {
        if (item.SubtribusId == 0)
            return false;

        if (await Data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
            await Data.SubtribusHasReferenceAsync(item.SubtribusId) ||
            await Data.SubtribusHasCommentAsync(item.SubtribusId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSubtribusAsync(item.SubtribusId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl60Subtribus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl60Subtribus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl60Subtribus item)
        => item.SubtribusId;

    // ---------------------------------------------------------
    // Sorting (in-memory)
    // ---------------------------------------------------------

    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    private string _idHeaderText = "Subtribus ID";
    public string IdHeaderText
    {
        get => _idHeaderText;
        set { _idHeaderText = value; OnPropertyChanged(); }
    }

    private string _nameHeaderText = "Subtribus Name";
    public string NameHeaderText
    {
        get => _nameHeaderText;
        set { _nameHeaderText = value; OnPropertyChanged(); }
    }

    public void SortBy(string column)
    {
        if (Items.Count == 0)
            return;

        if (_currentSortColumn == column)
            _sortAscending = !_sortAscending;
        else
        {
            _currentSortColumn = column;
            _sortAscending = true;
        }

        IEnumerable<Models.Tbl60Subtribus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.SubtribusId)
                                   : Items.OrderByDescending(x => x.SubtribusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.SubtribusName)
                                       : Items.OrderByDescending(x => x.SubtribusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl60Subtribus>(sorted);

        // Selection bewusst auf null setzen (wie vorher)
        SelectedItem = null;

        UpdateHeaderTexts();
    }

    private void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "Subtribus ID";
        NameHeaderText = "Subtribus Name";

        var suffix = _sortAscending ? asc : desc;

        switch (_currentSortColumn)
        {
            case "Id":
                IdHeaderText += suffix;
                break;
            case "Extra1":
                NameHeaderText += suffix;
                break;
        }
    }

}


