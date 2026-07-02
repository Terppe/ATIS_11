using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using ATIS.Core.Taxonomy;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl75Species;

public class Tbl75SpeciesViewModel : BaseHierarchyBossViewModel<Models.Tbl75Species>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }

    private int _currentParentId;
    private Func<int, Task<IEnumerable<Models.Tbl75Species>>>? _activeLoader;

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------

    public Tbl93CommentViewModel SpeciesCommentViewModel { get; }
    public Tbl90ReferenceViewModel SpeciesReferenceViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public ObservableCollection<Models.Tbl66Genus> GenusList { get; private set; } = new();

    public async Task LoadGenusLookupAsync()
    {
        GenusList = await _lookupService.GetGenusListAsync();
    }

    // ---------------------------------------------------------
    // CTOR
    // ---------------------------------------------------------

    public Tbl75SpeciesViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;

        SpeciesCommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        SpeciesReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        //  BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Species ID");
        RegisterHeader("Name", "Species Name");
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
            _ = SpeciesReferenceViewModel.LoadReferenceLookupsAsync();

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

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------

    public override async Task LoadLookupsAsync()
    {
        await LoadGenusLookupAsync();
    }

    // ---------------------------------------------------------
    // BOSS LOAD
    // ---------------------------------------------------------

    public override async Task LoadAsync()
    {
        _currentParentId = 0;
        _activeLoader = null;

        await base.LoadAsync();

        SelectedItem = null;
        IsDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD FOR GENUS (Parent)
    // ---------------------------------------------------------

    public async Task LoadForGenusAsync(int genusId)
        => await LoadInternalAsync(genusId, Data.GetSpeciesForGenusAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (universell)
    // ---------------------------------------------------------

    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl75Species>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl75Species>(items);

        DataSetCount = Items.Count;

        if (Items.Any())
        {
            SelectedItem = Items.First();
            IsDetailVisible = true;
        }
        else
        {
            SelectedItem = null;
            IsDetailVisible = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------

    protected override async Task<IEnumerable<Models.Tbl75Species>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchSpeciesAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl75Species>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------

    protected override async Task OnSelectedItemChangedAsync()
    {
        IsDetailVisible = SelectedItem != null;

        if (SelectedItem != null)
            await LoadChildrenAsync(SelectedItem.SpeciesId);
        else
            await ClearChildCollectionsAsync();
    }

    // ---------------------------------------------------------
    // CHILDREN (nur Comments + References)
    // ---------------------------------------------------------

    protected override async Task LoadChildrenAsync(int speciesId)
    {
        await SpeciesCommentViewModel.LoadForSpeciesAsync(speciesId);
        await SpeciesReferenceViewModel.LoadForSpeciesAsync(speciesId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        SpeciesCommentViewModel.Items?.Clear();
        SpeciesReferenceViewModel.Items?.Clear();

        SpeciesCommentViewModel.SelectedItem = null;
        SpeciesReferenceViewModel.SelectedItem = null;

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------

    protected override Models.Tbl75Species CreateNewItem()
        => new() { GenusId = _currentParentId };

    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsDetailVisible = true;
        await Task.Delay(50);
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    protected override async Task<Models.Tbl75Species?> SaveItemAsync(Models.Tbl75Species item)
    {
        if (string.IsNullOrWhiteSpace(item.SpeciesName))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertSpeciesAsync(item);

        if (_activeLoader != null)
        {
            var list = await _activeLoader(_currentParentId);
            Items = new ObservableCollection<Models.Tbl75Species>(list);
        }

        SelectedItem = Items.FirstOrDefault(x => x.SpeciesId == saved.SpeciesId);
        SearchText = saved.SpeciesName;

        return saved;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------

    protected override async Task<bool> DeleteItemAsync(Models.Tbl75Species item)
    {
        if (item.SpeciesId == 0)
            return false;

        if (await Data.SpeciesHasReferenceAsync(item.SpeciesId) ||
            await Data.SpeciesHasCommentAsync(item.SpeciesId))
        {
            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        await Data.DeleteSpeciesAsync(item.SpeciesId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl75Species item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl75Species item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl75Species item)
        => item.SpeciesId;


    // ---------------------------------------------------------
    // Sorting (in-memory)
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl75Species item, string column)
    {
        return column switch
        {
            "Id" => item.SpeciesId,
            "Name" => item.SpeciesName ?? "",
            _ => ""
        };
    }

    //private bool _sortAscending = true;
    //private string _currentSortColumn = "";

    //private string _idHeaderText = "Species ID";

    //public string IdHeaderText
    //{
    //    get => _idHeaderText;
    //    set
    //    {
    //        _idHeaderText = value;
    //        OnPropertyChanged();
    //    }
    //}

    //private string _nameHeaderText = "Species Name";

    //public string NameHeaderText
    //{
    //    get => _nameHeaderText;
    //    set
    //    {
    //        _nameHeaderText = value;
    //        OnPropertyChanged();
    //    }
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

    //    IEnumerable<Models.Tbl75Species> sorted = column switch
    //    {
    //        "Id" => _sortAscending
    //            ? Items.OrderBy(x => x.SpeciesId)
    //            : Items.OrderByDescending(x => x.SpeciesId),

    //        "Extra1" => _sortAscending
    //            ? Items.OrderBy(x => x.SpeciesName)
    //            : Items.OrderByDescending(x => x.SpeciesName),

    //        _ => Items
    //    };

    //    Items = new ObservableCollection<Models.Tbl75Species>(sorted);

    //    // Selection bewusst auf null setzen (wie vorher)
    //    SelectedItem = null;

    //    UpdateHeaderTexts();
    //}

    //private void UpdateHeaderTexts()
    //{
    //    const string asc = " ▲";
    //    const string desc = " ▼";

    //    IdHeaderText = "Species ID";
    //    NameHeaderText = "Species Name";

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

}


//public class Tbl75SpeciesViewModel : BaseBossListViewModel<Models.Tbl75Species>
//{
//    private readonly IDataService _data;

//    // ============================================
//    // HIERARCHIE-KONFIGURATION
//    // ============================================
//  //  protected override HierarchyLevel CurrentLevel => HierarchyLevel.Species;
//  //protected override HierarchyLevel CurrentLevel { get; }
//  //protected override HierarchyLevel ParentLevel => HierarchyLevel.Genus;
//  public ICommand SortCommand { get; }
//    public ICommand OpenEditCommand { get; }
//    public ICommand CloseEditCommand { get; }

//    // ============================================
//    // CONSTRUCTOR
//    // ============================================
//    public Tbl75SpeciesViewModel(
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
//    public Tbl78NameViewModel NameViewModel { get; }
//    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
//    public Tbl93CommentViewModel CommentViewModel { get; }

//    // ---------------------------------------------------------
//    // Lookup (Genus ComboBox)
//    // ---------------------------------------------------------
//    public ObservableCollection<Models.Tbl66Genus> GenusList { get; } = new();
//    public Dictionary<int, string?> GenusLookup { get; } = new();

//    public string? GetGenusName(int id)
//        => GenusLookup.TryGetValue(id, out var name) ? name : string.Empty;

//    public async Task LoadGenusAsync()
//    {
//        GenusList.Clear();
//        GenusLookup.Clear();

//        var list = await _data.GetAllGenusAsync();
//        foreach (var item in list)
//        {
//            GenusList.Add(item);
//            GenusLookup[item.GenusId] = item.GenusName;
//        }
//    }
//    // ---------------------------------------------------------
//    // Edit visibility
//    // ---------------------------------------------------------

//    private void OpenEdit()
//    {
//        if (SelectedItem != null)
//            IsSpeciesEditVisible = true;
//    }

//    private void CloseEdit()
//    {
//        IsSpeciesEditVisible = false;
//        SelectedItem = null;
//    }

//    // ---------------------------------------------------------
//    // Load entrypoint for Page
//    // ---------------------------------------------------------
//    public async Task LoadAsync()
//    {
//        await LoadGenusAsync();

//       // await ReloadAndSelectAsync(saved: default, autoSelect: true);
//    }

//    public  async Task ApplyFilterAsync(string? filter)
//    {
//        //if (SuppressFilter)
//        //    return;

//        //CurrentFilter = filter;
//        //OnPropertyChanged(nameof(CurrentFilter));

//        await LoadGenusAsync();
//      //  await ReloadAndSelectAsync(saved: default, autoSelect: false);
//    }

//    // ---------------------------------------------------------
//    // FK Navigation 
//    // ---------------------------------------------------------
//    public int? CurrentSpeciesId { get; set; }
//    public int? CurrentGenusId { get; set; }

//    public void ResetBelow()
//    {
//        // nichts unter Species
//    }

//    // ---------------------------------------------------------
//    // LoadForXAsync – neue Pipeline
//    // ---------------------------------------------------------
//    //public Task LoadForGenusAsync(int id)
//    //    => LoadForAsync(id, _data.GetSpeciesForGenusAsync, ResetBelow);

//    //public Task LoadForSpeciesAsync(int id)
//    //    => LoadForAsync(id, _data.GetSpeciesAsync, ResetBelow);

//    // ---------------------------------------------------------
//    // Selection -> load Comments, References via Navigator
//    // ---------------------------------------------------------
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        if (SelectedItem == null)
//        {
//            await TaxonomyNavigator.NavigateToAsync("Comments", 0);
//            await TaxonomyNavigator.NavigateToAsync("References", 0);
//            return;
//        }

//        int id = SelectedItem.SpeciesId;

//        await TaxonomyNavigator.NavigateToAsync("Comments", id);
//        await TaxonomyNavigator.NavigateToAsync("References", id);
//    }

//    //protected override void OnEditClosed()
//    //{
//    //    IsSpeciesEditVisible = false;
//    //}
//    // ---------------------------------------------------------
//    // Base overrides
//    // ---------------------------------------------------------

//    protected override async Task<IEnumerable<Models.Tbl75Species>> LoadItemsAsync()
//    {
//        if (string.IsNullOrWhiteSpace(default))
//            return Enumerable.Empty<Models.Tbl75Species>();

////        return await _data.SearchSpeciesAsync(CurrentFilter);
//        return Enumerable.Empty<Models.Tbl75Species>();
//    }


//    protected override int GetId(Models.Tbl75Species model)
//        => model.SpeciesId;

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

//    private bool _isSpeciesEditVisible;
//    public bool IsSpeciesEditVisible
//    {
//        get => _isSpeciesEditVisible;
//        set => SetProperty(ref _isSpeciesEditVisible, value);
//    }

//    private bool _isCommentEditVisible;
//    public bool IsCommentEditVisible
//    {
//        get => _isCommentEditVisible;
//        set => SetProperty(ref _isCommentEditVisible, value);
//    }

//    private bool _isReferenceEditVisible;
//    public bool IsReferenceEditVisible
//    {
//        get => _isReferenceEditVisible;
//        set => SetProperty(ref _isReferenceEditVisible, value);
//    }
//    private bool _isGenusEditVisible;
//    public bool IsGenusEditVisible
//    {
//        get => _isGenusEditVisible;
//        set => SetProperty(ref _isGenusEditVisible, value);
//    }

//    private Models.Tbl66Genus? _selectedGenus;
//    public Models.Tbl66Genus? SelectedGenus 
//    {
//        get => _selectedGenus;
//        set => SetProperty(ref _selectedGenus, value);
//    }

//    protected override async Task NewAsync()
//    {
//        if (CurrentGenusId <= 0)
//        {
//          //  await _dialog.ShowErrorAsync("Hinweis", "Bitte zuerst einen Genus auswählen.");
//            return;
//        }

//        // Neuen Datensatz erzeugen
//        var item = CreateNewItem();

//        // Liste initialisieren falls nötig
//        Items ??= new ObservableCollection<Models.Tbl75Species>();

//        // Hinzufügen
//        Items.Add(item);

//        // Auswahl setzen
//        SelectedItem = item;

//        // 🔥 WICHTIG: FK-VMs laden + Edit öffnen
//        await OnSelectedItemChangedAsync();

//        // Edit öffnen
//        IsSpeciesEditVisible = true;
//    }

//    // ---------------------------------------------------------
//    // Confirm dialogs from base hooks
//    // ---------------------------------------------------------

//    protected override Models.Tbl75Species CreateNewItem()
//        => new()
//        {
//            GenusId = CurrentGenusId ?? 0,
//            SpeciesId = 0,
//        };

//    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl75Species item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<Models.Tbl75Species?> SaveItemAsync(Models.Tbl75Species item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl75Species item)
//    {
//        throw new NotImplementedException();
//    }

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl75Species item)
//    {
//        throw new NotImplementedException();
//    }



//    //protected override async Task LoadForParentAsync(int parentId)
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //protected override async Task ClearChildCollectionsAsync()
//    //{
//    //    throw new NotImplementedException();
//    //}

//    //protected override async Task LoadChildrenAsync(int entityId)
//    //{
//    //    throw new NotImplementedException();
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

//    //protected override IEnumerable<Models.Tbl75Species> SortItems(
//    //    IEnumerable<Models.Tbl75Species> items, string column, bool ascending)
//    //{
//    //    return column switch
//    //    {
//    //        "Id" => ascending
//    //            ? items.OrderBy(x => x.SpeciesId)
//    //            : items.OrderByDescending(x => x.SpeciesId),
//    //        "Name" => ascending
//    //            ? items.OrderBy(x => x.SpeciesName)
//    //            : items.OrderByDescending(x => x.SpeciesName),
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
