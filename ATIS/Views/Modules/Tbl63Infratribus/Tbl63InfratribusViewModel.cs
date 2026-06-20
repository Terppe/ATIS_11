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

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusViewModel : BaseBossListViewModel<Models.Tbl63Infratribus>
{
    private  readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private Action<Models.Tbl63Infratribus>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl63Infratribus>>>? _loadByParent;
    private int _currentParentId;

    public Tbl93CommentViewModel InfratribusCommentViewModel { get; }
    public Tbl90ReferenceViewModel InfratribusReferenceViewModel { get; }
    public Tbl66GenusViewModel GenusViewModel { get; }

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

        IsInfratribusDetailVisible = false;
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }
    // ---------------------------------------------------------
    // UI STATE
    // ---------------------------------------------------------

    private bool _isInfratribusDetailVisible;
    public bool IsInfratribusDetailVisible
    {
        get => _isInfratribusDetailVisible;
        set => SetProperty(ref _isInfratribusDetailVisible, value);
    }
    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;
        await ReloadAsync();
        SelectedItem = null;
    }
    // ---------------------------------------------------------
    // LOOKUP: SUBTRIBUS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; private set; } = new();
    public Dictionary<int, string?> SubtribusLookup { get; private set; } = new();

    public async Task LoadSubtribusLookupAsync()
    {
        SubtribusList = await _lookupService.GetSubtribusListAsync();
        SubtribusLookup = await _lookupService.GetSubtribusLookupAsync();
    }

    // ---------------------------------------------------------
    // PARENT SYSTEM
    // ---------------------------------------------------------
    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl60Subtribus))
        {
            _assignParentFk = i => i.SubtribusId = parentId;
            _loadByParent = Data.GetInfratribusForSubtribusAsync;
        }
    }

    // ---------------------------------------------------------
    // LOAD PARENT (GENERIC)
    // ---------------------------------------------------------

    public async Task LoadParentAsync<TParent>(int parentId)
    {
        _currentParentId = parentId;

        ResolveParent<TParent>(parentId);

        var items = _loadByParent != null
            ? await _loadByParent(parentId)
            : Enumerable.Empty<Models.Tbl63Infratribus>();

        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD PARENT 
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        if (_loadByParent == null)
            return;

        var items = await _loadByParent(parentId);
        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);

        DataSetCount = Items.Count;

        // ❗ KEIN Auto-Select
        SelectedItem = null;
        IsInfratribusDetailVisible = false;

        // ⭐ Child-VMs Parent laden, aber NICHT das EditControl öffnen
        await InfratribusCommentViewModel.LoadParentAsync(parentId);
        await InfratribusReferenceViewModel.LoadParentAsync(parentId);
    }

    // ---------------------------------------------------------
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _loadByParent = null;
        _currentParentId = 0;

        await LoadSubtribusLookupAsync();

        Items = new ObservableCollection<Models.Tbl63Infratribus>();
        SelectedItem = null;
        IsInfratribusDetailVisible = false;

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // LOAD ITEMS
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
    {
        if (_loadByParent != null && _currentParentId > 0)
            return await _loadByParent(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchInfratribusWithSubtribusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl63Infratribus>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        IsInfratribusDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
            await LoadChildrenAsync(SelectedItem.InfratribusId);
        else
            await ClearChildCollectionsAsync();

        await base.OnSelectedItemChangedAsync();
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        // Infratribus Comments + References
        await InfratribusCommentViewModel.LoadParentAsync<Models.Tbl63Infratribus>(entityId);
        await InfratribusCommentViewModel.LoadParentAsync(entityId);

        await InfratribusReferenceViewModel.LoadParentAsync<Models.Tbl63Infratribus>(entityId);
        await InfratribusReferenceViewModel.LoadParentAsync(entityId);

        // ⭐ GenusVM vollständig initialisieren
        await GenusViewModel.LoadAsync();
        await GenusViewModel.LoadInfratribusLookupAsync();

        // Genus Liste + UI
        await GenusViewModel.LoadParentAsync<Models.Tbl63Infratribus>(entityId);
        await GenusViewModel.LoadParentAsync(entityId);

    }

    protected override Task ClearChildCollectionsAsync()
    {
        InfratribusCommentViewModel.Items?.Clear();
        InfratribusCommentViewModel.SelectedItem = null;

        InfratribusReferenceViewModel.Items?.Clear();
        InfratribusReferenceViewModel.SelectedItem = null;

        GenusViewModel.Items?.Clear();
        GenusViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsInfratribusDetailVisible = true;
        await Task.Delay(50);
    }

    protected override Models.Tbl63Infratribus CreateNewItem()
    {
        var i = new Models.Tbl63Infratribus();
        _assignParentFk?.Invoke(i);
        return i;
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

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
    {
        if (item.InfratribusId == 0)
            return false;

        if (await Data.InfratribusHasReferenceAsync(item.InfratribusId) ||
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
    // Sorting (in-memory)
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    private string _idHeaderText = "InfratribusID";
    public string IdHeaderText
    {
        get => _idHeaderText;
        set { _idHeaderText = value; OnPropertyChanged(); }
    }

    private string _nameHeaderText = "Infratribus Name";
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

        IEnumerable<Models.Tbl63Infratribus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.InfratribusId)
                                   : Items.OrderByDescending(x => x.InfratribusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.InfratribusName)
                                       : Items.OrderByDescending(x => x.InfratribusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl63Infratribus>(sorted);

        // Selection bewusst auf null setzen (wie vorher)
        SelectedItem = null;

        UpdateHeaderTexts();
    }

    private void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "Infratribus ID";
        NameHeaderText = "Infratribus Name";

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


//public class Tbl63InfratribusViewModel : BaseBossListViewModel<Models.Tbl63Infratribus>
//{
//    private readonly LookupService _lookupService;
//    private int _currentParentId;

//    // ---------------------------------------------------------
//    // CHILD MODULES (Infratribus hat Comments + References)
//    // ---------------------------------------------------------
//    public Tbl93CommentViewModel CommentViewModel { get; }
//    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
//    public Tbl66GenusViewModel GenusViewModel { get; }
//    // ---------------------------------------------------------
//    // CONSTRUCTOR
//    // ---------------------------------------------------------
//    public Tbl63InfratribusViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookupService,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        _lookupService = lookupService;

//        CommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
//        ReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
//        GenusViewModel = new Tbl66GenusViewModel(factory, dialog, coordinator, lookupService, user);

//        IsInfratribusDetailVisible = false;
//        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
//    }

//    // ---------------------------------------------------------
//    // Commands
//    // ---------------------------------------------------------
//    public ICommand SortCommand { get; }

//    // ---------------------------------------------------------
//    // UI STATE
//    // ---------------------------------------------------------
//    private bool _isInfratribusDetailVisible;
//    public bool IsInfratribusDetailVisible
//    {
//        get => _isInfratribusDetailVisible;
//        set => SetProperty(ref _isInfratribusDetailVisible, value);
//    }
//    // ---------------------------------------------------------
//    // LOOKUP: TRIBUS
//    // ---------------------------------------------------------
//    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; private set; } = new();
//    public Dictionary<int, string?> SubtribusLookup { get; private set; } = new();

//    private bool _isLoadingSubtribus;

//    private async Task LoadSubtribusAsync()
//    {
//        if (_isLoadingSubtribus)
//            return;

//        _isLoadingSubtribus = true;

//        try
//        {
//            SubtribusList = await _lookupService.GetSubtribusListAsync();
//            SubtribusLookup = await _lookupService.GetSubtribusLookupAsync();
//        }
//        finally
//        {
//            _isLoadingSubtribus = false;
//        }
//    }

//    // ---------------------------------------------------------
//    // LOAD PARENT (Subtribus → Infratribus)
//    // ---------------------------------------------------------
//    public override async Task LoadParentAsync(int parentId)
//    {
//        _currentParentId = parentId;

//        SelectedItem = null;
//        IsInfratribusDetailVisible = false;

//        var items = await Data.GetInfratribusForSubtribusAsync(parentId);
//        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
//    }
//    // ---------------------------------------------------------
//    // LOAD PARENT (Tribus → Subtribus)
//    // ---------------------------------------------------------
//    //public override async Task LoadParentAsync(int parentId)
//    //{
//    //    _currentSubtribusId = parentId;

//    //    await LoadSubtribusAsync();

//    //    var items = await Data.GetInfratribusForSubtribusAsync(parentId);
//    //    Items = new ObservableCollection<Models.Tbl63Infratribus>(items);

//    //    SelectedItem = null;
//    //    IsSubtribusDetailVisible = false;
//    //}

//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
//    {
//        if (string.IsNullOrWhiteSpace(SearchText))
//            return Enumerable.Empty<Models.Tbl63Infratribus>();

//        return await Data.SearchInfratribusWithSubtribusAsync(SearchText);
//    }

//    // ---------------------------------------------------------
//    // SELECTED ITEM
//    // ---------------------------------------------------------
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        IsInfratribusDetailVisible = SelectedItem != null;

//        if (SelectedItem != null)
//            await LoadChildrenAsync(SelectedItem.InfratribusId);
//        else
//            await ClearChildCollectionsAsync();
//    }

//    // ---------------------------------------------------------
//    // CHILDREN (Infratribus → Comments/References)
//    // ---------------------------------------------------------
//    //protected override async Task LoadChildrenAsync(int entityId)
//    //{
//    //    await CommentViewModel.LoadParentAsync(entityId);
//    //    await ReferenceViewModel.LoadParentAsync(entityId);
//    //    await GenusViewModel.LoadParentAsync(entityId);
//    //}
//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        await CommentViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
//        await ReferenceViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
//        await GenusViewModel.LoadParentAsync(entityId);
//    }

//    protected override Task ClearChildCollectionsAsync()
//    {
//        CommentViewModel.Items?.Clear();
//        ReferenceViewModel.Items?.Clear();
//        GenusViewModel.Items?.Clear();
//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // NEW
//    // ---------------------------------------------------------
//    protected override async Task NewAsync()
//    {
//        SelectedItem = CreateNewItem();
//        IsInfratribusDetailVisible = true;

//        await Task.Delay(50);
//    }

//    protected override Models.Tbl63Infratribus CreateNewItem()
//        => new Models.Tbl63Infratribus
//        {
//            SubtribusId = _currentParentId
//        };

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.Tbl63Infratribus?> SaveItemAsync(Models.Tbl63Infratribus item)
//    {
//        if (string.IsNullOrWhiteSpace(item.InfratribusName))
//        {
//            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null;
//        }

//        var saved = await Data.UpsertInfratribusAsync(item);
//        return await Data.GetInfratribusByIdAsync(saved.InfratribusId);
//    }

//    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl63Infratribus item)
//    {
//        if (string.IsNullOrWhiteSpace(SearchText))
//            SearchText = item.InfratribusName;

//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
//    {
//        if (item.InfratribusId == 0)
//            return false;

//        if (await Data.InfratribusHasReferenceAsync(item.InfratribusId) ||
//            await Data.InfratribusHasCommentAsync(item.InfratribusId))
//        {
//            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
//            return false;
//        }

//        await Data.DeleteInfratribusAsync(item.InfratribusId);
//        return true;
//    }

//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl63Infratribus item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl63Infratribus item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

//    // ---------------------------------------------------------
//    // ID
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl63Infratribus item)
//        => item.InfratribusId;

//    // ---------------------------------------------------------
//    // Sorting (in-memory)
//    // ---------------------------------------------------------
//    private bool _sortAscending = true;
//    private string _currentSortColumn = "";

//    private string _idHeaderText = "ID";
//    public string IdHeaderText
//    {
//        get => _idHeaderText;
//        set { _idHeaderText = value; OnPropertyChanged(); }
//    }

//    private string _nameHeaderText = "Name";
//    public string NameHeaderText
//    {
//        get => _nameHeaderText;
//        set { _nameHeaderText = value; OnPropertyChanged(); }
//    }

//    public void SortBy(string column)
//    {
//        if (Items.Count == 0)
//            return;

//        if (_currentSortColumn == column)
//            _sortAscending = !_sortAscending;
//        else
//        {
//            _currentSortColumn = column;
//            _sortAscending = true;
//        }

//        IEnumerable<Models.Tbl63Infratribus> sorted = column switch
//        {
//            "Id" => _sortAscending ? Items.OrderBy(x => x.InfratribusId)
//                                   : Items.OrderByDescending(x => x.InfratribusId),

//            "Extra1" => _sortAscending ? Items.OrderBy(x => x.InfratribusName)
//                                       : Items.OrderByDescending(x => x.InfratribusName),

//            _ => Items
//        };

//        Items = new ObservableCollection<Models.Tbl63Infratribus>(sorted);

//        // Selection bewusst auf null setzen (wie vorher)
//        SelectedItem = null;

//        UpdateHeaderTexts();
//    }

//    private void UpdateHeaderTexts()
//    {
//        const string asc = " ▲";
//        const string desc = " ▼";

//        IdHeaderText = "ID";
//        NameHeaderText = "Name";

//        var suffix = _sortAscending ? asc : desc;

//        switch (_currentSortColumn)
//        {
//            case "Id":
//                IdHeaderText += suffix;
//                break;
//            case "Extra1":
//                NameHeaderText += suffix;
//                break;
//        }
//    }

//}



