using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl90Reference;


public class Tbl90ReferenceViewModel : BaseBossListViewModel<Models.Tbl90Reference>
{
    private readonly LookupService _lookupService;

    // dynamische Parent-Zuweisung
    private Action<Models.Tbl90Reference>? _assignParentFk;
    private Func<int, Task<IEnumerable<Models.Tbl90Reference>>>? _loadByParent;
        private int _currentParentId;

    public Tbl90ReferenceViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        _lookupService = lookupService;
        IsReferenceDetailVisible = false;
    }

    private bool _isReferenceDetailVisible;
    public bool IsReferenceDetailVisible
    {
        get => _isReferenceDetailVisible;
        set => SetProperty(ref _isReferenceDetailVisible, value);
    }

    //public override async Task LoadParentAsync(int parentId)
    //{
    //    Debug.WriteLine("LOAD PARENT REFERENCES");

    //    _currentParentId = parentId;

    //    // WICHTIG: Parent-FK setzen
    //    _assignParentFk = r => r.SubtribusId = parentId;

    //    var items = await Data.GetReferencesForSubtribusAsync(parentId);
    //    Items = new ObservableCollection<Models.Tbl90Reference>(items);

    //    DataSetCount = Items.Count;

    //    SelectedItem = null;
    //    IsReferenceDetailVisible = false;
    //}
    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; } = new();
    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; } = new();

    public async Task LoadParentAsync<TParent>(int parentId)
    {
        Debug.WriteLine($"LOAD PARENT REFERENCES for {typeof(TParent).Name}, Id={parentId}");

        // 1. Parent-FK setzen
        ResolveParent<TParent>(parentId);

        // 2. Lookups laden
        await LoadReferenceLookupsAsync();

        // 3. Items laden
        var items = _loadByParent != null
            ? await _loadByParent(parentId)
            : Enumerable.Empty<Models.Tbl90Reference>();

        Items = new ObservableCollection<Models.Tbl90Reference>(items);
        DataSetCount = Items.Count;

        SelectedItem = null;
        IsReferenceDetailVisible = false;
    }

    private void ResolveParent<TParent>(int parentId)
    {
        if (typeof(TParent) == typeof(Models.Tbl75Species))
        {
            _assignParentFk = r => r.SpeciesId = parentId;
            _loadByParent = Data.GetReferencesForSpeciesAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl66Genus))
        {
            _assignParentFk = r => r.GenusId = parentId;
            _loadByParent = Data.GetReferencesForGenusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl63Infratribus))
        {
            _assignParentFk = r => r.InfratribusId = parentId;
            _loadByParent = Data.GetReferencesForInfratribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl60Subtribus))
        {
            _assignParentFk = r => r.SubtribusId = parentId;
            _loadByParent = Data.GetReferencesForSubtribusAsync;
        }
        else if (typeof(TParent) == typeof(Models.Tbl57Tribus))
        {
            _assignParentFk = r => r.TribusId = parentId;
            _loadByParent = Data.GetReferencesForTribusAsync;
        }

        // … und so weiter für alle 20 Ebenen …
    }

    private async Task LoadReferenceLookupsAsync()
    {
        RefAuthorsList.Clear();
        RefExpertsList.Clear();
        RefSourcesList.Clear();

        var authors = await _lookupService.GetRefAuthorListAsync();
        var experts = await _lookupService.GetRefExpertListAsync();
        var sources = await _lookupService.GetRefSourceListAsync();

        foreach (var a in authors) RefAuthorsList.Add(a);
        foreach (var e in experts) RefExpertsList.Add(e);
        foreach (var s in sources) RefSourcesList.Add(s);
    }


    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return Enumerable.Empty<Models.Tbl90Reference>();

        return await Data.SearchReferenceAsync(SearchText);
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override Task OnSelectedItemChangedAsync()
    {
        IsReferenceDetailVisible = SelectedItem != null;
        return Task.CompletedTask;
    }

    protected override Task LoadChildrenAsync(int entityId)
        => Task.CompletedTask;

    protected override Task ClearChildCollectionsAsync()
        => Task.CompletedTask;

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        SelectedItem = CreateNewItem();
        IsReferenceDetailVisible = true;
        await Task.Delay(50);
    }

    protected override Models.Tbl90Reference CreateNewItem()
    {
        var r = new Models.Tbl90Reference
        {
            Valid = true,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now
        };

        _assignParentFk?.Invoke(r);
        return r;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        if (!item.IsValid())
            return null;

        item.Updater = Environment.UserName;
        item.UpdaterDate = DateTime.Now;

        return await Data.UpsertReferenceAsync(item);
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
    {
        await Data.DeleteReferenceAsync(item.ReferenceId);
        return true;
    }

    // ---------------------------------------------------------
    // CONFIRM
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
        => Dialog.ShowConfirmAsync("Speichern?", "Referenz speichern?");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
        => Dialog.ShowConfirmAsync("Löschen?", "Referenz löschen?");

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl90Reference item)
        => item.ReferenceId;
}

//public class Tbl90ReferenceViewModel
//    : BaseBossListViewModel<Models.Tbl90Reference>
//{
//    private readonly LookupService _lookupService;
//    private int _currentParentId;
//    public ICommand NewCommand { get; }
//    public ICommand SaveCommand { get; }
//    public ICommand DeleteCommand { get; }
//    public ICommand RefreshCommand { get; }
//    public ICommand CloseEditCommand { get; }
//    // ---------------------------------------------------------
//    // CONSTRUCTOR
//    // ---------------------------------------------------------
//    public Tbl90ReferenceViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookupService,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        _lookupService = lookupService;
//        var lookup = new LookupService(factory, user);

//        IsReferenceDetailVisible = false;
//        IsEditVisible = false;

//        // Sortier‑Header
//        //SortHeader1 = "Id";
//        //SortHeader2 = "Writer";
//        //SortHeader3 = "Date";
//        //SortHeader4 = "Valid";

//     //   NewCommand = new AsyncRelayCommand(NewAsync);
//        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(NewItem);
//        SaveCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await SaveItemAsync(), CanSave);
//        DeleteCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await DeleteItemAsync(), CanDelete);
//        RefreshCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(async () => await RefreshAsync());
//        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);
//    }
//    // ============================================
//    // REFAUTHOR, REFEXPERT, REFSOURCE LOOKUP
//    // ============================================
//    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; } = new();
//    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; } = new();
//    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; } = new();

//    // ---------------------------------------------------------
//    // VISIBILITY FLAGS
//    // ---------------------------------------------------------
//    private bool _isDetailVisible;
//    public bool IsDetailVisible
//    {
//        get => _isDetailVisible;
//        set => SetProperty(ref _isDetailVisible, value);
//    }

//    private bool _isEditVisible;
//    public bool IsEditVisible
//    {
//        get => _isEditVisible;
//        set => SetProperty(ref _isEditVisible, value);
//    }
//    private bool _isReferenceDetailVisible;
//    public bool IsReferenceDetailVisible
//    {
//        get => _isReferenceDetailVisible;
//        set => SetProperty(ref _isReferenceDetailVisible, value);
//    }

//    public async Task LoadReferenceLookupsAsync()
//    {
//        RefAuthorsList.Clear();
//        RefExpertsList.Clear();
//        RefSourcesList.Clear();

//        var authors = await _lookupService.GetRefAuthorListAsync();
//        var experts = await _lookupService.GetRefExpertListAsync();
//        var sources = await _lookupService.GetRefSourceListAsync();

//        foreach (var a in authors) RefAuthorsList.Add(a);
//        foreach (var e in experts) RefExpertsList.Add(e);
//        foreach (var s in sources) RefSourcesList.Add(s);
//    }

//    // ---------------------------------------------------------
//    // CREATE NEW
//    // ---------------------------------------------------------
//    private void NewItem()
//    {
//        SelectedItem = new Models.Tbl90Reference()
//        {
//            SubtribusId = _currentParentId
//        };

//        IsReferenceDetailVisible = true;
//    }

//    protected override async Task NewAsync()
//    {
//        var item = CreateNewItem();
//        SelectedItem = item;

//        // UI binden lassen
//        OnPropertyChanged(nameof(SelectedItem));

//        // Validierung aktivieren
//       //    item.ValidateReferenceInfo();

//        // WICHTIG: UI Zeit geben, den roten Rahmen zu rendern
//        await Task.Delay(50);
//    }
//    protected override Models.Tbl90Reference CreateNewItem()
//        => new Models.Tbl90Reference
//        {
//            SubtribusId = _currentParentId   
//        };

//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
//    {
//        if (_currentParentId is null)
//            return Enumerable.Empty<Models.Tbl90Reference>();

//        return await Data.GetReferencesForSubtribusAsync(_currentParentId.Value);
//    }

//    // ---------------------------------------------------------
//    // LOAD PARENT (Boss)
//    // ---------------------------------------------------------
//    public override async Task LoadParentAsync(int parentId)
//    {
//        _currentParentId = parentId;

//        await LoadReferenceLookupsAsync();   // ⭐ WICHTIG

//        SelectedItem = null;
//        IsReferenceDetailVisible = false;

//        var items = await Data.GetReferencesForSubtribusAsync(parentId);
//        Items = new ObservableCollection<Models.Tbl90Reference>(items);

//    }

//    // ---------------------------------------------------------
//    // LOAD CHILDREN (Boss)
//    // ---------------------------------------------------------
//    protected override Task LoadChildrenAsync(int entityId)
//    {
//        // References haben KEINE Children
//        IsReferenceDetailVisible = true;
//        return Task.CompletedTask;
//    }
//    protected override Task OnSelectedItemChangedAsync()
//    {
//        IsReferenceDetailVisible = SelectedItem != null;
//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // CLEAR CHILDREN (Boss)
//    // ---------------------------------------------------------
//    protected override Task ClearChildCollectionsAsync()       
//    {
//        IsReferenceDetailVisible = false;
//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override bool CanSave()
//    {
//        if (SelectedItem == null)
//            return false;

//        return SelectedItem.IsValid();
//    }
//    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
//    {
//        if (!item.IsValid())
//            return null;

//        item.Updater = Environment.UserName;
//        item.UpdaterDate = DateTime.Now;

//        return await Data.UpsertReferenceAsync(item);
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override bool CanDelete()
//    {
//        if (SelectedItem == null)
//            return false;

//        return SelectedItem.ReferenceId != 0;
//    }
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
//    {
//        await Data.DeleteReferenceAsync(item.ReferenceId);
//        return true;
//    }
//    private async Task RefreshAsync()
//    {
//        await LoadParentAsync(_currentParentId);
//    }
//    private void CloseEdit()
//    {
//        IsReferenceDetailVisible = false;
//    }

//    // ---------------------------------------------------------
//    // CONFIRM SAVE / DELETE
//    // ---------------------------------------------------------
//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl90Reference item)
//        => Dialog.ShowConfirmAsync("Speichern?", "Möchten Sie diese Referenz speichern?");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl90Reference item)
//        => Dialog.ShowConfirmAsync("Löschen?", "Möchten Sie diese Referenz wirklich löschen?");

//    // ---------------------------------------------------------
//    // ID SELECTOR
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl90Reference model)
//        => model.ReferenceId;

//    // ---------------------------------------------------------
//    // SORTING
//    // ---------------------------------------------------------
//    //public override void Sort(int columnIndex)
//    //{
//    //    switch (columnIndex)
//    //    {
//    //        case 0: Items = new ObservableCollection<Models.Tbl90Reference>(Items.OrderBy(i => i.ReferenceId)); break;
//    //        case 1: Items = new ObservableCollection<Models.Tbl90Reference>(Items.OrderBy(i => i.Writer)); break;
//    //        case 2: Items = new ObservableCollection<Models.Tbl90Reference>(Items.OrderBy(i => i.WriterDate)); break;
//    //        case 3: Items = new ObservableCollection<Models.Tbl90Reference>(Items.OrderBy(i => i.Valid)); break;
//    //    }
//    //}
//}



