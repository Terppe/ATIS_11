using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl60Subtribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl57Tribus;


public class Tbl57TribusViewModel : BaseBossListViewModel<Models.Tbl57Tribus>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl57Tribus>>>? _activeLoader;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel TribusCommentViewModel { get; }
    public Tbl90ReferenceViewModel TribusReferenceViewModel { get; }
    public Tbl60SubtribusViewModel SubtribusViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public Dictionary<int, string?> SupertribusLookup { get; private set; } = new();

    public ObservableCollection<Models.Tbl54Supertribus> SupertribusList { get; private set; } = new();

    public async Task LoadSupertribusLookupAsync()
    {
        SupertribusList = await _lookupService.GetSupertribusListAsync();
        SupertribusLookup = await _lookupService.GetSupertribusLookupAsync();
    }

    // ---------------------------------------------------------
    // CTOR
    // ---------------------------------------------------------

    public Tbl57TribusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        TribusCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        TribusReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        SubtribusViewModel = new Tbl60SubtribusViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        //  BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Tribus ID");
        RegisterHeader("Name", "Tribus Name");
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
        IsInParentMode = SelectedDetailPivotIndex != 0;

        if (SelectedDetailPivotIndex == 2)
            _ = TribusReferenceViewModel.LoadReferenceLookupsAsync();

        if (SelectedDetailPivotIndex == 5)
            _ = SubtribusViewModel.SubtribusReferenceViewModel.LoadReferenceLookupsAsync();

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;
        await ReloadAsync();
        SelectedItem = null;
    }

    //// ---------------------------------------------------------
    //// LOOKUPS
    //// ---------------------------------------------------------

    //public override async Task LoadLookupsAsync()
    //{
    //    await LoadSupertribusLookupAsync();
    //}

    // ---------------------------------------------------------
    // BOSS LOAD
    // ---------------------------------------------------------

    public async Task LoadAsync()
    {
        _currentParentId = 0;
        _activeLoader = null;

        await LoadSupertribusLookupAsync();

        SelectedItem = null;
      //  IsDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD FOR Supertribus (Parent)
    // ---------------------------------------------------------

    public async Task LoadForSupertribusAsync(int supertribusId)
        => await LoadInternalAsync(supertribusId, Data.GetTribusForSupertribusAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (universell)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl57Tribus>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl57Tribus>(items);

        DataSetCount = Items.Count;

        if (Items.Any())
        {
            SelectedItem = Items.First();
          //  IsDetailVisible = true;
        }
        else
        {
            SelectedItem = null;
          //  IsDetailVisible = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl57Tribus>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchTribusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl57Tribus>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------

    protected override async Task OnSelectedItemChangedAsync()
    {
        //  IsDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
            await LoadChildrenAsync(SelectedItem.TribusId);
        else
            await ClearChildCollectionsAsync();
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------

    protected override async Task LoadChildrenAsync(int tribusId)
    {
        await TribusCommentViewModel.LoadForTribusAsync(tribusId);
        await TribusReferenceViewModel.LoadForTribusAsync(tribusId);

        await SubtribusViewModel.LoadTribusLookupAsync();
        await SubtribusViewModel.LoadForTribusAsync(tribusId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        TribusCommentViewModel.Items?.Clear();
        TribusReferenceViewModel.Items?.Clear();
        SubtribusViewModel.Items?.Clear();

        TribusCommentViewModel.SelectedItem = null;
        TribusReferenceViewModel.SelectedItem = null;
        SubtribusViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Models.Tbl57Tribus CreateNewItem()
        => new() { SupertribusId = _currentParentId };

    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        //  IsDetailVisible = true;
        await Task.Delay(50);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl57Tribus?> SaveItemAsync(Models.Tbl57Tribus item)
    {
        if (string.IsNullOrWhiteSpace(item.TribusName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertTribusAsync(item);

        if (_activeLoader != null)
        {
            var list = await _activeLoader(_currentParentId);
            Items = new ObservableCollection<Models.Tbl57Tribus>(list);
        }

        SelectedItem = Items.FirstOrDefault(x => x.TribusId == saved.TribusId);
        SearchText = saved.TribusName;

        return saved;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl57Tribus item)
    {
        if (item.TribusId == 0)
            return false;

        if (await Data.TribusHasSubtribusAsync(item.TribusId) ||
            await Data.TribusHasReferenceAsync(item.TribusId) ||
            await Data.TribusHasCommentAsync(item.TribusId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteTribusAsync(item.TribusId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl57Tribus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl57Tribus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl57Tribus item)
        => item.TribusId;

    // ---------------------------------------------------------
    // Sorting (in-memory)
    // ---------------------------------------------------------

    //private bool _sortAscending = true;
    //private string _currentSortColumn = "";

    //private string _idHeaderText = "Tribus ID";
    //public string IdHeaderText
    //{
    //    get => _idHeaderText;
    //    set { _idHeaderText = value; OnPropertyChanged(); }
    //}

    //private string _nameHeaderText = "Tribus Name";
    //public string NameHeaderText
    //{
    //    get => _nameHeaderText;
    //    set { _nameHeaderText = value; OnPropertyChanged(); }
    //}

    //public void SortBy(string column)
    //{
    //    if (Items.Count == 0)
    //        return;

    //    if (_currentSortColumn == column)
    //        _sortAscending = !_sortAscending;
    //    else
    //    {
    //        _currentSortColumn = column;
    //        _sortAscending = true;
    //    }

    //    IEnumerable<Models.Tbl57Tribus> sorted = column switch
    //    {
    //        "Id" => _sortAscending ? Items.OrderBy(x => x.TribusId)
    //                               : Items.OrderByDescending(x => x.TribusId),

    //        "Extra1" => _sortAscending ? Items.OrderBy(x => x.TribusName)
    //                                   : Items.OrderByDescending(x => x.TribusName),

    //        _ => Items
    //    };

    //    Items = new ObservableCollection<Models.Tbl57Tribus>(sorted);

    //    // Selection bewusst auf null setzen (wie vorher)
    //    SelectedItem = null;

    //    UpdateHeaderTexts();
    //}

    //private void UpdateHeaderTexts()
    //{
    //    const string asc = " ▲";
    //    const string desc = " ▼";

    //    IdHeaderText = "Tribus ID";
    //    NameHeaderText = "Tribus Name";

    //    var suffix = _sortAscending ? asc : desc;

    //    switch (_currentSortColumn)
    //    {
    //        case "Id":
    //            IdHeaderText += suffix;
    //            break;
    //        case "Extra1":
    //            NameHeaderText += suffix;
    //            break;
    //    }
    //}

    protected override object GetSortKey(Models.Tbl57Tribus item, string column)
    {
        return column switch
        {
            "Id" => item.TribusId,
            "Extra1" => item.TribusName ?? "",
            _ => ""
        };
    }
}

//public class Tbl57TribusViewModel : BaseBossListViewModel<Models.Tbl57Tribus>
//{
//    private readonly IDataService _data;
//    private readonly IDialogService _dialog;

//    private readonly LookupService _lookupService;

//    // ============================================
//    // CONSTRUCTOR
//    // ============================================
//    public Tbl57TribusViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookup,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        //Child1ViewModel = new TblYYListViewModel(factory, user, dialog);
//        //Child2ViewModel = new TblZZListViewModel(factory, user, dialog);

//        //IsDetailVisible = false;
//        //IsEditVisible = false;
//    }

//    // ============================================
//    // CHILD VIEWMODELS
//    // ============================================
//    //public Tbl60SubtribusViewModel SubtribusViewModel
//    //{
//    //    get => ChildViewModels.TryGetValue("Subtribus", out var vm)
//    //        ? (Tbl60SubtribusViewModel)vm
//    //        : null!;
//    //    set => ChildViewModels["Subtribus"] = value;
//    //}
//    //public Tbl90ReferenceViewModel ReferenceViewModel
//    //{
//    //    get => ChildViewModels.TryGetValue("Reference", out var vm)
//    //        ? (Tbl90ReferenceViewModel)vm
//    //        : null!;
//    //    set => ChildViewModels["Reference"] = value;
//    //}

//    //public Tbl93CommentViewModel CommentViewModel
//    //{
//    //    get => ChildViewModels.TryGetValue("Comment", out var vm)
//    //        ? (Tbl93CommentViewModel)vm
//    //        : null!;
//    //    set => ChildViewModels["Comment"] = value;
//    //}
//    // ============================================
//    // HIERARCHIE-KONFIGURATION
//    // ============================================
//    //protected override HierarchyLevel CurrentLevel => HierarchyLevel.Tribus;
//    //protected override HierarchyLevel ParentLevel => HierarchyLevel.Supertribus;
//    public ICommand SortCommand { get; }
//    public ICommand OpenEditCommand { get; }
//    public ICommand CloseEditCommand { get; }

//    public async Task LoadForSupertribusAsync(int supertribusId)
//    {
//        if (supertribusId <= 0) return;

//        var items = await _data.GetTribusForSupertribusAsync(supertribusId);
//     //   Items = new ObservableCollection<Models.Tbl57Tribus>(items);
//    }

//    // ---------------------------------------------------------
//    // Supertribus lookup
//    // ---------------------------------------------------------
//    public ObservableCollection<Models.Tbl54Supertribus> SupertribusList { get; } = new();

//    public Dictionary<int, string?> SupertribusLookup { get; } = new();

//    public string? GetSupertribusName(int id)
//        => SupertribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

//    public async Task LoadSupertribusAsync()
//    {
//        SupertribusList.Clear();
//        SupertribusLookup.Clear();

//        var list = await _data.GetAllSupertribusAsync();

//        foreach (var item in list)
//        {
//            SupertribusList.Add(item);
//            SupertribusLookup[item.SupertribusId] = item.SupertribusName;
//        }
//    }
//    //public async Task LoadAsync()
//    //{
//    //    await LoadSupertribusAsync();
//    //    await ReloadAndSelectAsync(saved: default, autoSelect: true);
//    //}

//    protected override async Task NewAsync()
//    {
//        throw new NotImplementedException();
//    }

//    protected override Models.Tbl57Tribus CreateNewItem()
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<IEnumerable<Models.Tbl57Tribus>> LoadItemsAsync()
//    {
//        if (string.IsNullOrWhiteSpace(default))
//            return Enumerable.Empty<Models.Tbl57Tribus>();

//        //if (string.IsNullOrWhiteSpace(CurrentFilter))
//        //{
//        //    // Lade alle Items, wenn kein Filter gesetzt ist
//        //    return await _data.GetAllTribusAsync();
//        //}

//        // Filtere nach dem Text in CurrentFilter
//        return await _data.SearchTribusAsync(default);
//    }

//    protected override async Task<Models.Tbl57Tribus?> SaveItemAsync(Models.Tbl57Tribus item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl57Tribus item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl57Tribus item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl57Tribus item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override int GetId(Models.Tbl57Tribus model)
//    {
//        throw new NotImplementedException();
//    }


//    public override async Task LoadParentAsync(int parentId)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task ClearChildCollectionsAsync()
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        throw new NotImplementedException();
//    }


//    public  async Task ApplyFilterAsync(string? filter)
//    {

//        await LoadSupertribusAsync();
//       // await ReloadAndSelectAsync(saved: default, autoSelect: false);
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
//    private Models.Tbl57Tribus? _selectedTribus;
//    public Models.Tbl57Tribus? SelectedTribus
//    {
//        get => _selectedTribus;
//        set
//        {
//            if (SetProperty(ref _selectedTribus, value))
//            {
//                if (value == null)
//                {
//                    //CommentViewModel.CurrentTribusId = null;
//                    //ReferenceViewModel.CurrentTribusId = null;
//                    return;
//                }

//                int id = value.TribusId;

//                // Parent für Comments
//      //          CommentViewModel.CurrentTribusId = id;
//                //CommentViewModel.CurrentSubtribusId = null;

//                //// Parent für References
//                //ReferenceViewModel.CurrentTribusId = id;
//                //ReferenceViewModel.CurrentSubtribusId = null;

//                // Parent für Subtribus
//       //         SubtribusViewModel.CurrenTribusId = id;
//            }
//        }
//    }

//    private int _selectedTabIndex;
//    public int SelectedTabIndex
//    {
//        get => _selectedTabIndex;
//        set
//        {
//            if (SetProperty(ref _selectedTabIndex, value) && SelectedTribus != null)
//            {
//                int id = SelectedTribus.TribusId;

//                if (value == 1) // Comments Tribus
//                {
//                    //CommentViewModel.CurrentTribusId = id;
//                    //CommentViewModel.CurrentSupertribusId = null;
//                }

//                if (value == 2) // References Tribus
//                {
//                    //ReferenceViewModel.CurrentTribusId = id;
//                    //ReferenceViewModel.CurrentSupertribusId = null;
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
//            if (SetProperty(ref _selectedInnerTabIndex, value) && SelectedTribus != null)
//            {
//                int id = SelectedTribus.TribusId;

//                if (value == 1) // Comments Tribus
//                {
//                    //CommentViewModel.CurrentTribusId = id;
//                    //CommentViewModel.CurrentSupertribusId = null;
//                }

//                if (value == 2) // References Tribus
//                {
//                    //ReferenceViewModel.CurrentTribusId = id;
//                    //ReferenceViewModel.CurrentSupertribusId = null;
//                }
//            }
//        }
//    }


//    // ============================================
//    // TEMPLATE IMPLEMENTATIONS
//    // ============================================


//    //protected override async Task LoadForParentAsync(int supertribusId)
//    //{
//    //    var items = await _data.GetTribusForSupertribusAsync(supertribusId);
//    //    Items = new ObservableCollection<Models.Tbl57Tribus>(items);
//    //}



//    //protected override async Task ClearChildCollectionsAsync()
//    //{
//    //    SubtribusViewModel.Items?.Clear();
//    //    await Task.CompletedTask;
//    //}

//    //protected override async Task LoadChildrenAsync(int entityId)
//    //{
//    //    await SubtribusViewModel.LoadForTribusAsync(entityId);
//    //}


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


//    //protected override IEnumerable<Models.Tbl57Tribus> SortItems(
//    //    IEnumerable<Models.Tbl57Tribus> items, string column, bool ascending)
//    //{
//    //    return column switch
//    //    {
//    //        "Id" => ascending
//    //            ? items.OrderBy(x => x.TribusId)
//    //            : items.OrderByDescending(x => x.TribusId),
//    //        "Name" => ascending
//    //            ? items.OrderBy(x => x.TribusName)
//    //            : items.OrderByDescending(x => x.TribusName),
//    //        _ => items
//    //    };
//    //}

//    //protected override void UpdateHeaderTexts()
//    //{
//    //    const string asc = " ▲";
//    //    const string desc = " ▼";

//    //    IdHeaderText = "ID";
//    //    NameHeaderText = "Name";

//    //    // Optional: Sorting-Pfeile hinzufügen wenn nötig
//    //}
//}
