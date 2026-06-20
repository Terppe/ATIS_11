using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl66Genus;

public class Tbl66GenusViewModel : BaseBossListViewModel<Models.Tbl66Genus>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private Action<Models.Tbl63Infratribus>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl63Infratribus>>>? _loadByParent;
    private int _currentParentId;
    public Tbl75SpeciesListViewModel SpeciesViewModel { get; }
    public Tbl93CommentListViewModel CommentViewModel { get; }
    public Tbl90ReferenceListViewModel ReferenceViewModel { get; }

    public Tbl66GenusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SpeciesViewModel = new Tbl75SpeciesListViewModel(factory, user, dialog);
        CommentViewModel = new Tbl93CommentListViewModel(factory, user, dialog);
        ReferenceViewModel = new Tbl90ReferenceListViewModel(factory, user, dialog);

        IsGenusDetailVisible = false;
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }
    // ---------------------------------------------------------
    // CHILD VIEWMODELS (für Genus)
    // ---------------------------------------------------------

    // ---------------------------------------------------------
    // UI-STATE
    // ---------------------------------------------------------
    private bool _isGenusDetailVisible;
    public bool IsGenusDetailVisible
    {
        get => _isGenusDetailVisible;
        set => SetProperty(ref _isGenusDetailVisible, value);
    }
    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;
        await ReloadAsync();
        SelectedItem = null;
    }

    // ---------------------------------------------------------
    // LOOKUP: INFRATRIBUS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl63Infratribus> InfratribusList { get; private set; } = new();
    public Dictionary<int, string?> InfratribusLookup { get; private set; } = new();

    //private bool _isLoadingInfratribus;

    //private async Task LoadInfratribusAsync()
    //{
    //    if (_isLoadingInfratribus)
    //        return;

    //    _isLoadingInfratribus = true;

    //    try
    //    {
    //        InfratribusList = await _lookupService.GetInfratribusListAsync();
    //        InfratribusLookup = await _lookupService.GetInfratribusLookupAsync();
    //    }
    //    finally
    //    {
    //        _isLoadingInfratribus = false;
    //    }
    //}
    public async Task LoadInfratribusLookupAsync()
    {
        InfratribusList = await _lookupService.GetInfratribusListAsync();
        InfratribusLookup = await _lookupService.GetInfratribusLookupAsync();
    }
    // ---------------------------------------------------------
    // PARENT SYSTEM
    // ---------------------------------------------------------
    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl63Infratribus))
        {
            _assignParentFk = s => s.InfratribusId = parentId;
           // _loadByParent = Data.GetGenusForInfratribusAsync;
        }
    }

    // ---------------------------------------------------------
    // LOAD PARENT 
    // ---------------------------------------------------------

    public async Task LoadParentAsync<TParent>(int parentId)
    {
        _currentParentId = parentId;

        ResolveParent<TParent>(parentId);

        //var items = _loadByParent != null
        //    ? await _loadByParent(parentId)
        //    : Enumerable.Empty<Models.Tbl66Genus>();

     //   Items = new ObservableCollection<Models.Tbl66Genus>(items);
     //   DataSetCount = Items.Count;

        // FIX:
        //SelectedItem = Items.FirstOrDefault();
        //IsInfratribusDetailVisible = SelectedItem != null;
    }

    public override async Task LoadParentAsync(int parentId)
    {
        if (_loadByParent == null)
            return;

        var items = await _loadByParent(parentId);
     //   Items = new ObservableCollection<Models.Tbl66Genus>(items);

        DataSetCount = Items.Count;

        // ⭐ WICHTIG: ???
        SelectedItem = Items.FirstOrDefault();
        IsGenusDetailVisible = SelectedItem != null;
    }



    //public override async Task LoadParentAsync(int parentId)
    //{
    //    _currentInfratribusId = parentId;

    //    var items = await Data.GetGenusForInfratribusAsync(parentId);
    //    Items = new ObservableCollection<Models.Tbl66Genus>(items);
    //}

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        //await CommentViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
        //await ReferenceViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);

        // ⭐ NEU: Lookups für Genus laden
        //await SpeciesViewModel.LoadGenusLookupAsync();
        //await SpeciesViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        CommentViewModel.Items?.Clear();
        CommentViewModel.SelectedItem = null;

        ReferenceViewModel.Items?.Clear();
        ReferenceViewModel.SelectedItem = null;

        SpeciesViewModel.Items?.Clear();
        SpeciesViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }
    // ---------------------------------------------------------
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        // Boss-Start: Parent-Kontext löschen
        _loadByParent = null;
        _currentParentId = 0;

        // Falls du später noch Lookups brauchst, hier wäre der Platz dafür
        await LoadInfratribusLookupAsync();

        Items = new ObservableCollection<Models.Tbl66Genus>();
        SelectedItem = null;
        IsGenusDetailVisible = false;

        // optional: Suchmodus aktivieren
        await ReloadAsync();
    }

    // ---------------------------------------------------------
    // CRUD
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl66Genus CreateNewItem()
        => new Models.Tbl66Genus
        {
//            InfratribusId = _currentParentId ?? 0
            
        };

    protected override async Task<IEnumerable<Models.Tbl66Genus>> LoadItemsAsync()
    {
        //if (_currentParentId is null)
        //    return Enumerable.Empty<Models.Tbl66Genus>();
      //  return await Data.GetGenusForInfratribusAsync(_currentParentId.Value);

        return null;
    }

    protected override async Task<Models.Tbl66Genus?> SaveItemAsync(Models.Tbl66Genus item)
    {
        if (string.IsNullOrWhiteSpace(item.GenusName))
        {
            await Dialog.ShowErrorAsync("Name fehlt", "Bitte einen gültigen Genus-Namen eingeben.");
            return default;
        }

        var saved = await Data.UpsertGenusAsync(item);
        return await Data.GetGenusByIdAsync(saved.GenusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    {
        if (item.GenusId == 0)
            return false; // Unsaved

        if (await Data.GenusHasSpeciesAsync(item.GenusId) ||
            await Data.GenusHasReferenceAsync(item.GenusId) ||
            await Data.GenusHasCommentAsync(item.GenusId))
        {
            await Dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await Data.DeleteGenusAsync(item.GenusId);
        return true;
    }

    //protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    //{
    //    if (item.GenusId == 0)
    //        return false;

    //    if (await Data.GenusHasChildrenAsync(item.GenusId))
    //    {
    //        await Dialog.ShowErrorAsync(
    //            "Löschen nicht möglich",
    //            "Dieses Genus hat abhängige Daten."
    //        );
    //        return false;
    //    }

    //    await Data.DeleteGenusAsync(item.GenusId);
    //    return true;
    //}

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl66Genus item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl66Genus item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    // ---------------------------------------------------------
    // LOAD PARENT (Infratribus → Genus)
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl66Genus model)
        => model.GenusId;

    // ---------------------------------------------------------
    // Sorting (in-memory)
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

    private string _idHeaderText = "ID";
    public string IdHeaderText
    {
        get => _idHeaderText;
        set { _idHeaderText = value; OnPropertyChanged(); }
    }

    private string _nameHeaderText = "Name";
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

        IEnumerable<Models.Tbl66Genus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.GenusId)
                                   : Items.OrderByDescending(x => x.GenusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.GenusName)
                                       : Items.OrderByDescending(x => x.GenusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl66Genus>(sorted);

        // Selection bewusst auf null setzen (wie vorher)
        SelectedItem = null;

        UpdateHeaderTexts();
    }

    private void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "ID";
        NameHeaderText = "Name";

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
