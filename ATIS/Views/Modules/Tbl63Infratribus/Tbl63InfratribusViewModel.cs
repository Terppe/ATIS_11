using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusViewModel : BaseBossListViewModel<Models.Tbl63Infratribus>
{
    private readonly LookupService _lookupService;
    private int _currentParentId;

    // ---------------------------------------------------------
    // CHILD MODULES (Infratribus hat Comments + References)
    // ---------------------------------------------------------
    public Tbl93CommentViewModel CommentViewModel { get; }
    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
    public Tbl66GenusViewModel GenusViewModel { get; }
    // ---------------------------------------------------------
    // CONSTRUCTOR
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

        CommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookupService, user);
        ReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookupService, user);
        GenusViewModel = new Tbl66GenusViewModel(factory, dialog, coordinator, lookupService, user);

        IsInfratribusDetailVisible = false;
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
    // ============================================
    // SORTING HEADERS
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
    // ---------------------------------------------------------
    // LOOKUP: TRIBUS
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; private set; } = new();
    public Dictionary<int, string?> SubtribusLookup { get; private set; } = new();

    private bool _isLoadingSubtribus;

    private async Task LoadSubtribusAsync()
    {
        if (_isLoadingSubtribus)
            return;

        _isLoadingSubtribus = true;

        try
        {
            SubtribusList = await _lookupService.GetSubtribusListAsync();
            SubtribusLookup = await _lookupService.GetSubtribusLookupAsync();
        }
        finally
        {
            _isLoadingSubtribus = false;
        }
    }

    // ---------------------------------------------------------
    // LOAD PARENT (Subtribus → Infratribus)
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        _currentParentId = parentId;

        SelectedItem = null;
        IsInfratribusDetailVisible = false;

        var items = await Data.GetInfratribusForSubtribusAsync(parentId);
        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
    }
    // ---------------------------------------------------------
    // LOAD PARENT (Tribus → Subtribus)
    // ---------------------------------------------------------
    //public override async Task LoadParentAsync(int parentId)
    //{
    //    _currentSubtribusId = parentId;

    //    await LoadSubtribusAsync();

    //    var items = await Data.GetInfratribusForSubtribusAsync(parentId);
    //    Items = new ObservableCollection<Models.Tbl63Infratribus>(items);

    //    SelectedItem = null;
    //    IsSubtribusDetailVisible = false;
    //}

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return Enumerable.Empty<Models.Tbl63Infratribus>();

        return await Data.SearchInfratribusWithSubtribusAsync(SearchText);
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
    }

    // ---------------------------------------------------------
    // CHILDREN (Infratribus → Comments/References)
    // ---------------------------------------------------------
    //protected override async Task LoadChildrenAsync(int entityId)
    //{
    //    await CommentViewModel.LoadParentAsync(entityId);
    //    await ReferenceViewModel.LoadParentAsync(entityId);
    //    await GenusViewModel.LoadParentAsync(entityId);
    //}
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await CommentViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
        await ReferenceViewModel.LoadParentAsync<Models.Tbl66Genus>(entityId);
        await GenusViewModel.LoadParentAsync(entityId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        CommentViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        GenusViewModel.Items?.Clear();
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
        => new Models.Tbl63Infratribus
        {
            SubtribusId = _currentParentId
        };

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
        return await Data.GetInfratribusByIdAsync(saved.InfratribusId);
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl63Infratribus item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            SearchText = item.InfratribusName;

        return Task.CompletedTask;
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

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl63Infratribus item)
        => item.InfratribusId;
}


//public class Tbl63InfratribusViewModel
//    : BaseBossListViewModel<Models.Tbl63Infratribus>
//{
//    private readonly LookupService _lookupService;
//    private int? _currentSubtribusId;

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
//        var lookup = new LookupService(factory, user);

//        // Child‑Module = Boss‑VMs
//        GenusViewModel = new Tbl66GenusViewModel(factory, dialog, coordinator, lookup, user);
//        CommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookup, user);
//        ReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookup, user);

//        IsInfratribusDetailVisible = false;
//        IsEditVisible = false;

//        // Sortier‑Header
//        //SortHeader1 = "Id";
//        //SortHeader2 = "Name";
//        //SortHeader3 = "Subtribus";
//        //SortHeader4 = "Valid";

//        NewCommand = new AsyncRelayCommand(NewAsync);
//    }

//    // ---------------------------------------------------------
//    // CHILD BOSS‑VIEWMODELS
//    // ---------------------------------------------------------
//    public Tbl66GenusViewModel GenusViewModel { get; }
//    public Tbl93CommentViewModel CommentViewModel { get; }
//    public Tbl90ReferenceViewModel ReferenceViewModel { get; }

//    // ---------------------------------------------------------
//    // UI‑STATE
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
//    private bool _isInfratribusDetailVisible;
//    public bool IsInfratribusDetailVisible
//    {
//        get => _isInfratribusDetailVisible;
//        set => SetProperty(ref _isInfratribusDetailVisible, value);
//    }
//    public string? CurrentFilter { get; private set; }
//    public bool SuppressFilter { get; set; }

//    public async Task ApplyFilterAsync(string? filter)
//    {
//        if (SuppressFilter)
//            return;

//        SearchText = filter;          // ⭐ WICHTIG
//        CurrentFilter = filter;

//        OnPropertyChanged(nameof(SearchText));
//        OnPropertyChanged(nameof(CurrentFilter));

//        await LoadAsync();

//        SelectedItem = null;
//    }


//    // ---------------------------------------------------------
//    // LOAD PARENT (Subtribus)
//    // ---------------------------------------------------------
//    public override async Task LoadParentAsync(int parentId)
//    {
//        _currentSubtribusId = parentId;

//        await LoadSubtribusLookupsAsync();   // ⭐ WICHTIG

//        SelectedItem = null;
//        IsInfratribusDetailVisible = false;

//        var items = await Data.GetInfratribusForSubtribusAsync(parentId);
//        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);

//    }
//    // ============================================
//    // SUBTRIBUS LOOKUP
//    // ============================================
//    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; } = new();

//    public async Task LoadSubtribusLookupsAsync()
//    {
//        SubtribusList.Clear();

//        var subtribusses = await _lookupService.GetSubtribusListAsync();

//        foreach (var a in subtribusses) SubtribusList.Add(a);
//    }


//    public Dictionary<int, string?> SubtribusLookup { get; set; } = new();

//    private bool _isLoadingSubtribus = false;

//    public async Task LoadSubtribusAsync()
//    {
//        if (_isLoadingSubtribus)
//            return;

//        _isLoadingSubtribus = true;

//        try
//        {
//        //    SubtribusList = await _lookupService.GetSubtribusListAsync();
//            SubtribusLookup = await _lookupService.GetSubtribusLookupAsync();
//        }
//        finally
//        {
//            _isLoadingSubtribus = false;
//        }
//    }
//    public async Task LoadAsync()
//    {
//        await LoadSubtribusAsync();
//        await ReloadAndSelectAsync(0, x => x.InfratribusId);
//        IsInfratribusDetailVisible = false;
//    }
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        IsInfratribusDetailVisible = SelectedItem != null;

//        if (SelectedItem != null)
//            await LoadChildrenAsync(SelectedItem.InfratribusId);
//        else
//            await ClearChildCollectionsAsync();
//    }
//    // ---------------------------------------------------------
//    // LOAD CHILDREN (Infratribus → Genus, Comments, References)
//    // ---------------------------------------------------------
//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        await GenusViewModel.LoadParentAsync(entityId);
//        await CommentViewModel.LoadParentAsync(entityId);
//        await ReferenceViewModel.LoadParentAsync(entityId);
//    }

//    // ---------------------------------------------------------
//    // CLEAR CHILDREN
//    // ---------------------------------------------------------
//    protected override Task ClearChildCollectionsAsync()
//    {
//        GenusViewModel.Items?.Clear();
//        CommentViewModel.Items?.Clear();
//        ReferenceViewModel.Items?.Clear();

//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
//    {
//        if (_currentSubtribusId is null)
//            return Enumerable.Empty<Models.Tbl63Infratribus>();

//        return await Data.GetInfratribusForSubtribusAsync(_currentSubtribusId.Value);
//    }

//    // ---------------------------------------------------------
//    // CREATE NEW
//    // ---------------------------------------------------------
//    protected override async Task NewAsync()
//    {
//        var item = CreateNewItem();
//        SelectedItem = item;

//        // UI binden lassen
//        OnPropertyChanged(nameof(SelectedItem));

//        // Validierung aktivieren
//        item.ValidateInfratribusName();

//        // WICHTIG: UI Zeit geben, den roten Rahmen zu rendern
//        await Task.Delay(50);
//    }

//    protected override Models.Tbl63Infratribus CreateNewItem()
//        => new Models.Tbl63Infratribus
//        {
//            SubtribusId = _currentSubtribusId ?? 0,
//            Valid = true
//        };


//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.Tbl63Infratribus?> SaveItemAsync(Models.Tbl63Infratribus item)
//    {
//        if (string.IsNullOrWhiteSpace(item.InfratribusName))
//        {
//            await Dialog.ShowErrorAsync("Name fehlt", "Bitte einen gültigen Infratribus‑Namen eingeben.");
//            return null;
//        }

//        var saved = await Data.UpsertInfratribusAsync(item);
//        return await Data.GetInfratribusByIdAsync(saved.InfratribusId);
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
//    {
//        if (item.InfratribusId == 0)
//            return false;

//        if (await Data.InfratribusHasGenusAsync(item.InfratribusId) ||
//            await Data.InfratribusHasReferenceAsync(item.InfratribusId) ||
//            await Data.InfratribusHasCommentAsync(item.InfratribusId))
//        {
//            await Dialog.ShowErrorAsync(
//                "Löschen nicht möglich",
//                "Dieser Infratribus hat abhängige Daten."
//            );
//            return false;
//        }

//        await Data.DeleteInfratribusAsync(item.InfratribusId);
//        return true;
//    }

//    // ---------------------------------------------------------
//    // CONFIRM SAVE / DELETE
//    // ---------------------------------------------------------
//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl63Infratribus item)
//        => Dialog.ShowConfirmAsync("Speichern?", "Möchten Sie diesen Infratribus speichern?");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl63Infratribus item)
//        => Dialog.ShowConfirmAsync("Löschen?", "Möchten Sie diesen Infratribus wirklich löschen?");

//    // ---------------------------------------------------------
//    // ID SELECTOR
//    // ---------------------------------------------------------
//    protected override int GetId(Models.Tbl63Infratribus model)
//        => model.InfratribusId;
//    // ============================================
//    // SORTING
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
//    public IEnumerable<Models.Tbl60Subtribus> SortItems(
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

//    public void UpdateHeaderTexts()
//    {
//        const string asc = " ▲";
//        const string desc = " ▼";

//        IdHeaderText = "ID";
//        NameHeaderText = "Name";
//    }
//}

