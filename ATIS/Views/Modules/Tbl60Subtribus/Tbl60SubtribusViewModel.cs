using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl60Subtribus;

/// <summary>
/// Subtribus ist gleichzeitig:
/// - CHILD von Tribus
/// - PARENT für Infratribusses, Comments, References
/// </summary>
public class Tbl60SubtribusViewModel : BaseBossListViewModel<Models.Tbl60Subtribus>
{
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl60SubtribusViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        Tbl63InfratribusViewModel infratribusViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;

        // Dictionary mit Children initialisieren
        ChildViewModels["Infratribus"] = infratribusViewModel;
        ChildViewModels["Reference"] = referenceViewModel;
        ChildViewModels["Comment"] = commentViewModel;

        //NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(New);
        //SaveCommand = new AsyncRelayCommand(SaveAsync, () => SelectedItem != null);
        //DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedItem != null);
        //RefreshCommand = new AsyncRelayCommand(ReloadAsync);

    }
    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    public Tbl63InfratribusViewModel InfratribusViewModel
    {
        get => ChildViewModels.TryGetValue("Infratribus", out var vm)
            ? (Tbl63InfratribusViewModel)vm
            : null!;
        set => ChildViewModels["Infratribus"] = value;
    }
    public Tbl90ReferenceViewModel ReferenceViewModel
    {
        get => ChildViewModels.TryGetValue("Reference", out var vm)
            ? (Tbl90ReferenceViewModel)vm
            : null!;
        set => ChildViewModels["Reference"] = value;
    }
    public Tbl93CommentViewModel CommentViewModel
    {
        get => ChildViewModels.TryGetValue("Comment", out var vm)
            ? (Tbl93CommentViewModel)vm
            : null!;
        set => ChildViewModels["Comment"] = value;
    }

    // ============================================
    // HIERARCHIE-KONFIGURATION
    // ============================================
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Subtribus;
    protected override HierarchyLevel ParentLevel => HierarchyLevel.Tribus;

    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ============================================
    // TRIBUS-DATEN LADEN (LOOKUP)
    // ============================================
    private ObservableCollection<Models.Tbl57Tribus> _tribusList = new();
    public ObservableCollection<Models.Tbl57Tribus> TribusList
    {
        get => _tribusList;
        set => SetProperty(ref _tribusList, value);
    }

    public Dictionary<int, string?> TribusLookup { get; set; } = new();

    private bool _isLoadingTribus = false;

    public async Task LoadTribusAsync()
    {
        // Verhindere gleichzeitiges Laden
        if (_isLoadingTribus)
            return;

        _isLoadingTribus = true;

        try
        {
            TribusList = await _lookupService.GetTribusListAsync();
            TribusLookup = await _lookupService.GetTribusLookupAsync();
        }
        finally
        {
            _isLoadingTribus = false;
        }
    }

    public async Task LoadAsync()
    {
        await LoadTribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);

        IsDetailVisible = false;
        SelectedItem = null;
    }

    // ============================================
    // LOAD ITEMS
    // ============================================
    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl60Subtribus>();

        return await _data.SearchSubtribusWithTribusAsync(CurrentFilter);
    }

    protected override async Task LoadForParentAsync(int parentId)
    {
        var items = await _data.GetSubtribusForTribusAsync(parentId);
        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
    }

    public async Task LoadForTribusAsync(int tribusId)
    {
        if (tribusId <= 0) return;

        var items = await _data.GetSubtribusForTribusAsync(tribusId);
        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
    }

    // ============================================
    // FILTER
    // ============================================
    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadTribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
    }
    public new bool IsEditVisible
    {
        get => _isEditVisible;
        set => SetProperty(ref _isEditVisible, value);
    }
    private bool _isEditVisible;

    // ============================================
    // NEW / CREATE
    // ============================================
    public override void New()
    {
        var newItem = new Models.Tbl60Subtribus
        {
            Valid = true
        };

        Items.Add(newItem);
        SelectedItem = newItem;

        IsDetailVisible = true;
        SelectedTabIndex = 0;
    }

    protected override Models.Tbl60Subtribus CreateNewItem()
        => new()
        {
            TribusId = _coordinator.GetSelectedId(ParentLevel) ?? 0,
            SubtribusId = 0
        };

    // ============================================
    // LOAD CHILDREN
    // ============================================
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await InfratribusViewModel.LoadForSubtribusAsync(entityId);
        await CommentViewModel.LoadForSubtribusAsync(entityId);
        await ReferenceViewModel.LoadForSubtribusAsync(entityId);

        await ReferenceViewModel.LoadReferenceExpertsAsync();
        await ReferenceViewModel.LoadReferenceSourcesAsync();
        await ReferenceViewModel.LoadReferenceAuthorsAsync();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        InfratribusViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        await Task.CompletedTask;
    }

    // ============================================
    // SAVE
    // ============================================
    protected override string? GetFilterForSavedItem(Models.Tbl60Subtribus saved)
        => saved.SubtribusName;

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl60Subtribus item)
    {
        return await _dialog.ShowConfirmAsync("Save_Confirm_Title", "Save_Confirm_Message");
    }

    public override async Task SaveAsync()
    {
        if (SelectedItem == null)
            return;

        var item = SelectedItem;

        // 1) Abfrage
        if (!await ConfirmSaveAsync(item))
            return;

        // 2) Validierung
        if (!await ValidateBeforeSaveAsync(item))
            return;

        // 3) Speichern
        var saved = await SaveItemAsync(item);
        if (saved == null)
            return;

        // 4) Filter setzen (damit neuer Datensatz sichtbar wird)
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = saved.SubtribusName;
            CurrentFilter = saved.SubtribusName;
        }

        // 5) Reload + neuen Datensatz auswählen
        await ReloadAndSelectAsync(saved, autoSelect: true);
    }

    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
    {
        if (string.IsNullOrWhiteSpace(item.SubtribusName))
        {
            await _dialog.ShowErrorAsync(
                "Error_EmptyName",
                "Subtribus-Name darf nicht leer sein."
            );
            return null;
        }

        if (await _data.SubtribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertSubtribusAsync(item);
        return await _data.GetSubtribusByIdAsync(saved.SubtribusId);
    }

    protected override async Task OnBeforeReloadAfterSaveAsync(Models.Tbl60Subtribus item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = item.SubtribusName;
            CurrentFilter = item.SubtribusName;
        }
    }
    // ============================================
    // DELETE
    // ============================================

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl60Subtribus item)
    {
        return await _dialog.ShowConfirmAsync("Delete_Confirm_Title", "Delete_Confirm_Message");
    }

    public override async Task DeleteAsync()
    {
        if (SelectedItem == null)
            return;

        var toDelete = SelectedItem;

        // 1) In der DB löschen
        await _data.DeleteSubtribusAsync(toDelete.SubtribusId);

        // 2) Aus der Liste entfernen
        Items.Remove(toDelete);

        // 3) Neues SelectedItem bestimmen
        if (Items.Count > 0)
        {
            SelectedItem = Items[0];   // Fokus auf ersten Datensatz
            IsDetailVisible = true;
            SelectedTabIndex = 0;
        }
        else
        {
            SelectedItem = null;
            IsDetailVisible = false;   // EditControl ausblenden
        }
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
    {
        if (item.SubtribusId == 0)
            return false;

        if (await _data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
            await _data.SubtribusHasReferenceAsync(item.SubtribusId) ||
            await _data.SubtribusHasCommentAsync(item.SubtribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteSubtribusAsync(item.SubtribusId);
        return true;
    }
    protected  async Task OnAfterSaveAsync(Models.Tbl60Subtribus saved)
    {
        // 1) Filter setzen, damit der neue Datensatz sichtbar wird
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = saved.SubtribusName;
            CurrentFilter = saved.SubtribusName;
        }

        // 2) Liste neu laden und neuen Datensatz auswählen
        await ReloadAndSelectAsync(saved, autoSelect: true);
    }

    // ============================================
    // DETAIL VISIBILITY & CONTEXT
    // ============================================
    private Models.Tbl60Subtribus? _selectedSubtribus;
    public Models.Tbl60Subtribus? SelectedSubtribus
    {
        get => _selectedSubtribus;
        set
        {
            if (SetProperty(ref _selectedSubtribus, value))
            {
                UpdateChildContexts(value);
            }
        }
    }

    private void UpdateChildContexts(Models.Tbl60Subtribus? selectedSubtribus)
    {
        if (selectedSubtribus == null)
        {
            CommentViewModel.CurrentSubtribusId = null;
            ReferenceViewModel.CurrentSubtribusId = null;
            InfratribusViewModel.CurrentSubtribusId = null;
            return;
        }

        int id = selectedSubtribus.SubtribusId;
        CommentViewModel.CurrentSubtribusId = id;
        ReferenceViewModel.CurrentSubtribusId = id;
        InfratribusViewModel.CurrentSubtribusId = id;
    }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                SetParentContextsForTab(value);
            }
        }
    }

    private int _selectedInnerTabIndex;
    public int SelectedInnerTabIndex
    {
        get => _selectedInnerTabIndex;
        set
        {
            if (SetProperty(ref _selectedInnerTabIndex, value))
            {
                SetParentContextsForTab(value);
            }
        }
    }

    private void SetParentContextsForTab(int tabIndex)
    {
        if (SelectedSubtribus == null) return;

        int id = SelectedSubtribus.SubtribusId;

        if (tabIndex == 1) // Comments Subtribus
        {
            CommentViewModel.CurrentSubtribusId = id;
            CommentViewModel.CurrentTribusId = null;
        }

        if (tabIndex == 2) // References Subtribus
        {
            ReferenceViewModel.CurrentSubtribusId = id;
            ReferenceViewModel.CurrentTribusId = null;
        }
    }

    public void ResetBelow()
    {
        InfratribusViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
    }

    // ============================================
    // EDIT VISIBILITY
    // ============================================
    private bool _isSubtribusEditVisible;
    public bool IsSubtribusEditVisible
    {
        get => _isSubtribusEditVisible;
        set => SetProperty(ref _isSubtribusEditVisible, value);
    }


    // ============================================
    // SEARCH & DETAIL VISIBILITY
    // ============================================
    private string _searchText = string.Empty;
    public string? SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    // ============================================
    // SORTING
    // ============================================
    private string _idHeaderText = "ID";
    public string IdHeaderText
    {
        get => _idHeaderText;
        set => SetProperty(ref _idHeaderText, value);
    }

    private string _nameHeaderText = "Name";
    public string NameHeaderText
    {
        get => _nameHeaderText;
        set => SetProperty(ref _nameHeaderText, value);
    }

    protected override IEnumerable<Models.Tbl60Subtribus> SortItems(
        IEnumerable<Models.Tbl60Subtribus> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.SubtribusId)
                : items.OrderByDescending(x => x.SubtribusId),
            "Name" => ascending
                ? items.OrderBy(x => x.SubtribusName)
                : items.OrderByDescending(x => x.SubtribusName),
            _ => items
        };
    }

    protected override void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "ID";
        NameHeaderText = "Name";
    }
}


//public class Tbl60SubtribusViewModel : BaseBossListViewModel<Models.Tbl60Subtribus>
//{
//    private readonly IDataService _data;
//    public ICommand SortCommand { get; }
//    public ICommand OpenEditCommand { get; }
//    public ICommand CloseEditCommand { get; }

//    // ============================================
//    // CONSTRUCTOR
//    // ============================================
//    public Tbl60SubtribusViewModel(
//        IDataService data,
//        IDialogService dialog,
//        Tbl63InfratribusViewModel infratribusViewModel,
//        Tbl90ReferenceViewModel referenceViewModel,
//        Tbl93CommentViewModel commentViewModel,
//        HierarchyCoordinator coordinator
//    ) : base(data, dialog, coordinator)
//    {
//        _data = data;

//        // Dictionary mit Children initialisieren
//        ChildViewModels["Infratribus"] = infratribusViewModel;
//        ChildViewModels["Reference"] = referenceViewModel;
//        ChildViewModels["Comment"] = commentViewModel;
//    }

//    // ============================================
//    // HIERARCHIE-KONFIGURATION
//    // ============================================
//    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Subtribus;
//    protected override HierarchyLevel ParentLevel => HierarchyLevel.Tribus;

//    // ============================================
//    // CHILD VIEWMODELS
//    // ============================================
//    public Tbl63InfratribusViewModel InfratribusViewModel
//    {
//        get => ChildViewModels.TryGetValue("Infratribus", out var vm)
//            ? (Tbl63InfratribusViewModel)vm
//            : null!;
//        set => ChildViewModels["Infratribus"] = value;
//    }

//    public Tbl90ReferenceViewModel ReferenceViewModel
//    {
//        get => ChildViewModels.TryGetValue("Reference", out var vm)
//            ? (Tbl90ReferenceViewModel)vm
//            : null!;
//        set => ChildViewModels["Reference"] = value;
//    }

//    public Tbl93CommentViewModel CommentViewModel
//    {
//        get => ChildViewModels.TryGetValue("Comment", out var vm)
//            ? (Tbl93CommentViewModel)vm
//            : null!;
//        set => ChildViewModels["Comment"] = value;
//    }

//    // ---------------------------------------------------------
//    // Tribus lookup
//    // ---------------------------------------------------------
//    public ObservableCollection<Models.Tbl57Tribus> TribusList { get; } = new();

//    public Dictionary<int, string?> TribusLookup { get; } = new();

//    public string? GetTribusName(int id)
//        => TribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

//    public async Task LoadAsync()
//    {
//        await LoadTribusAsync();
//        await ReloadAndSelectAsync(saved: default, autoSelect: true);
//    }
//    public async Task LoadTribusAsync()
//    {
//        TribusList.Clear();
//        TribusLookup.Clear();

//        var list = await _data.GetAllTribusAsync();

//        foreach (var item in list)
//        {
//            TribusList.Add(item);
//            TribusLookup[item.TribusId] = item.TribusName;
//        }
//    }
//    public async Task LoadForTribusAsync(int tribusId)
//    {
//        if (tribusId <= 0) return;

//        var items = await _data.GetSubtribusForTribusAsync(tribusId);
//        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
//    }

//    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
//    {
//        if (string.IsNullOrWhiteSpace(CurrentFilter))
//            return Enumerable.Empty<Models.Tbl60Subtribus>();

//        //if (string.IsNullOrWhiteSpace(CurrentFilter))
//        //{
//        //    // Lade alle Items, wenn kein Filter gesetzt ist
//        //    return await _data.GetAllSubtribusAsync();
//        //}

//        // Filtere nach dem Text in CurrentFilter
//        return await _data.SearchSubtribusAsync(CurrentFilter);
//    }
//    public override async Task ApplyFilterAsync(string? filter)
//    {
//        if (SuppressFilter)
//            return;

//        CurrentFilter = filter;
//        OnPropertyChanged(nameof(CurrentFilter));

//        await LoadTribusAsync();
//        await ReloadAndSelectAsync(saved: default, autoSelect: false);
//    }

//    // ---------------------------------------------------------
//    // Edit visibility
//    // ---------------------------------------------------------
//    private void OpenEdit()
//    {
//        if (SelectedItem != null)
//            IsSubtribusEditVisible = true;
//    }

//    private void CloseEdit()
//    {
//        IsSubtribusEditVisible = false;
//        SelectedItem = null;
//    }

//    // ============================================
//    // DETAIL VISIBILITY
//    // ============================================
//    private string _searchText = string.Empty;
//    public string SearchText
//    {
//        get => _searchText;
//        set => SetProperty(ref _searchText, value);
//    }

//    private bool _isDetailVisible;
//    public bool IsDetailVisible
//    {
//        get => _isDetailVisible;
//        set => SetProperty(ref _isDetailVisible, value);
//    }
//    private Models.Tbl60Subtribus? _selectedSubtribus;
//    public Models.Tbl60Subtribus? SelectedSubtribus
//    {
//        get => _selectedSubtribus;
//        set
//        {
//            if (SetProperty(ref _selectedSubtribus, value))
//            {
//                if (value == null)
//                {
//                    CommentViewModel.CurrentSubtribusId = null;
//                    ReferenceViewModel.CurrentSubtribusId = null;
//                    return;
//                }

//                int id = value.SubtribusId;

//                // Parent für Comments
//                CommentViewModel.CurrentSubtribusId = id;
//                CommentViewModel.CurrentInfratribusId = null;

//                // Parent für References
//                ReferenceViewModel.CurrentSubtribusId = id;
//                ReferenceViewModel.CurrentInfratribusId = null;

//                // Parent für Infratribus
//                InfratribusViewModel.CurrentSubtribusId = id;
//            }
//        }
//    }

//    private int _selectedTabIndex;
//    public int SelectedTabIndex
//    {
//        get => _selectedTabIndex;
//        set
//        {
//            if (SetProperty(ref _selectedTabIndex, value) && SelectedSubtribus != null)
//            {
//                int id = SelectedSubtribus.SubtribusId;

//                if (value == 1) // Comments Subtribus
//                {
//                    CommentViewModel.CurrentSubtribusId = id;
//                    CommentViewModel.CurrentTribusId = null;
//                }

//                if (value == 2) // References Subtribus
//                {
//                    ReferenceViewModel.CurrentSubtribusId = id;
//                    ReferenceViewModel.CurrentTribusId = null;
//                }
//            }
//        }
//    }

//    private int _selectedInnerTabIndex;
//    public int SelectedInnerTabIndex
//    {
//        get => _selectedInnerTabIndex;
//        set
//        {
//            if (SetProperty(ref _selectedInnerTabIndex, value) && SelectedSubtribus != null)
//            {
//                int id = SelectedSubtribus.SubtribusId;

//                if (value == 1) // Comments Subtribus
//                {
//                    CommentViewModel.CurrentSubtribusId = id;
//                    CommentViewModel.CurrentTribusId = null;
//                }

//                if (value == 2) // References Subtribus
//                {
//                    ReferenceViewModel.CurrentSubtribusId = id;
//                    ReferenceViewModel.CurrentTribusId = null;
//                }
//            }
//        }
//    }

//    // ============================================
//    // SORTING HEADERS
//    // ============================================
//    private bool _isSubtribusEditVisible;
//    public bool IsSubtribusEditVisible
//    {
//        get => _isSubtribusEditVisible;
//        set { _isSubtribusEditVisible = value; OnPropertyChanged(); }
//    }

//    public int CurrenTribusId { get; set; }

//    // ============================================
//    // TEMPLATE IMPLEMENTATIONS
//    // ============================================


//    protected override Models.Tbl60Subtribus CreateNewItem()
//        => new()
//        {
//            TribusId = _coordinator.GetSelectedId(ParentLevel) ?? 0,
//            SubtribusId = 0
//        };

//    protected override async Task LoadForParentAsync(int tribusId)
//    {
//        var items = await _data.GetSubtribusForTribusAsync(tribusId);
//        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
//    }

//    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
//    {
//        if (await _data.SubtribusExistsAsync(item))
//        {
//            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null;
//        }

//        var saved = await _data.UpsertSubtribusAsync(item);
//        return await _data.GetSubtribusByIdAsync(saved.SubtribusId);
//    }

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
//    {
//        if (item.SubtribusId == 0)
//            return false; // Unsaved

//        if (await _data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
//            await _data.SubtribusHasReferenceAsync(item.SubtribusId) ||
//            await _data.SubtribusHasCommentAsync(item.SubtribusId))
//        {
//            await _dialog.ShowErrorAsync(
//                "Error_DeleteNotAllowed_Title",
//                "Error_DeleteNotAllowed_Message"
//            );
//            return false;
//        }

//        await _data.DeleteSubtribusAsync(item.SubtribusId);
//        return true;
//    }

//    protected override async Task ClearChildCollectionsAsync()
//    {
//        InfratribusViewModel.Items?.Clear();
//        CommentViewModel.Items?.Clear();
//        ReferenceViewModel.Items?.Clear();
//        await Task.CompletedTask;
//    }
//    public void ResetBelow()
//    {
//        InfratribusViewModel.Items?.Clear();
//        ReferenceViewModel.Items?.Clear();
//        CommentViewModel.Items?.Clear();
//    }
//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        await InfratribusViewModel.LoadForSubtribusAsync(entityId);
//        await CommentViewModel.LoadForSubtribusAsync(entityId);
//        await ReferenceViewModel.LoadForSubtribusAsync(entityId);

//        await ReferenceViewModel.LoadReferenceExpertsAsync();
//        await ReferenceViewModel.LoadReferenceSourcesAsync();
//        await ReferenceViewModel.LoadReferenceAuthorsAsync();
//    }

//    // ============================================
//    // SORTING HEADERS
//    // ============================================
//    private string _idHeaderText = "ID";
//    public string IdHeaderText
//    {
//        get => _idHeaderText;
//        set => SetProperty(ref _idHeaderText, value);
//    }

//    private string _nameHeaderText = "Name";
//    public string NameHeaderText
//    {
//        get => _nameHeaderText;
//        set => SetProperty(ref _nameHeaderText, value);
//    }


//    protected override IEnumerable<Models.Tbl60Subtribus> SortItems(
//        IEnumerable<Models.Tbl60Subtribus> items, string column, bool ascending)
//    {
//        return column switch
//        {
//            "Id" => ascending
//                ? items.OrderBy(x => x.SubtribusId)
//                : items.OrderByDescending(x => x.SubtribusId),
//            "Name" => ascending
//                ? items.OrderBy(x => x.SubtribusName)
//                : items.OrderByDescending(x => x.SubtribusName),
//            _ => items
//        };
//    }

//    protected override void UpdateHeaderTexts()
//    {
//        const string asc = " ▲";
//        const string desc = " ▼";

//        IdHeaderText = "ID";
//        NameHeaderText = "Name";

//        // Optional: Sorting-Pfeile hinzufügen wenn nötig
//    }
//}
