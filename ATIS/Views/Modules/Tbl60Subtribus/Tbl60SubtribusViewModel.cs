using System.Collections.ObjectModel;
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
    private readonly LookupService _lookupService;
    private int? _currentTribusId;

    // ---------------------------------------------------------
    // CHILD MODULES
    // ---------------------------------------------------------
    public Tbl93CommentViewModel CommentViewModel { get; }
    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
    public Tbl63InfratribusViewModel InfratribusViewModel { get; }

    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl60SubtribusViewModel(
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
        InfratribusViewModel = new Tbl63InfratribusViewModel(factory, dialog, coordinator, lookupService, user);

        IsSubtribusDetailVisible = false;
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
    public ObservableCollection<Models.Tbl57Tribus> TribusList { get; private set; } = new();
    public Dictionary<int, string?> TribusLookup { get; private set; } = new();

    private bool _isLoadingTribus;

    private async Task LoadTribusAsync()
    {
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

    // ---------------------------------------------------------
    // LOAD PARENT (Tribus → Subtribus)
    // ---------------------------------------------------------
    public override async Task LoadParentAsync(int parentId)
    {
        _currentTribusId = parentId;

        await LoadTribusAsync();

        var items = await Data.GetSubtribusForTribusAsync(parentId);
        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);

        SelectedItem = null;
        IsSubtribusDetailVisible = false;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return Enumerable.Empty<Models.Tbl60Subtribus>();

        return await Data.SearchSubtribusWithTribusAsync(SearchText);
    }
    // ---------------------------------------------------------
    // INITIAL LOAD (für Page_Loaded)
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        // Tribus-Lookups laden
        await LoadTribusAsync();

        // Liste laden und ersten Eintrag auswählen
        await ReloadAndSelectAsync(0, x => x.SubtribusId);

        IsSubtribusDetailVisible = false;
    }
    // ---------------------------------------------------------
    // FILTER (für AutoSuggestBox)
    // ---------------------------------------------------------
    public async Task ApplyFilterAsync(string? filter)
    {
        SearchText = filter;      // Boss-Mechanik: löst ReloadAsync() aus
        await ReloadAsync();      // explizit, damit UI sofort reagiert

        SelectedItem = null;
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
    }

    // ---------------------------------------------------------
    // CHILDREN (Subtribus → Comments/References/Infratribus)
    // ---------------------------------------------------------
    //protected override async Task LoadChildrenAsync(int entityId)
    //{
    //    await CommentViewModel.LoadParentAsync(entityId);
    //    await ReferenceViewModel.LoadParentAsync(entityId);
    //    await InfratribusViewModel.LoadParentAsync(entityId);
    //}
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await CommentViewModel.LoadParentAsync<Models.Tbl60Subtribus>(entityId);
        await ReferenceViewModel.LoadParentAsync<Models.Tbl60Subtribus>(entityId);
        await InfratribusViewModel.LoadParentAsync(entityId);
    }

    protected override Task ClearChildCollectionsAsync()
    {
        CommentViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        InfratribusViewModel.Items?.Clear();
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
        => new Models.Tbl60Subtribus
        {
            TribusId = _currentTribusId ?? 0
        };

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

        var saved = await Data.UpsertSubtribusAsync(item);
        return await Data.GetSubtribusByIdAsync(saved.SubtribusId);
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl60Subtribus item)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = item.SubtribusName;
        }

        return Task.CompletedTask;
    }

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

    // ---------------------------------------------------------
    // ID
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl60Subtribus item)
        => item.SubtribusId;
}


//public class Tbl60SubtribusViewModel : BaseBossListViewModel<Models.Tbl60Subtribus>
//{
//    private readonly LookupService _lookupService;
//    private int? _currentTribusId;

//    // ---------------------------------------------------------
//    // CONSTRUCTOR
//    // ---------------------------------------------------------
//    public Tbl60SubtribusViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookupService,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {
//        _lookupService = lookupService;
//        var lookup = new LookupService(factory, user);

//        CommentViewModel = new Tbl93CommentViewModel(factory, dialog, coordinator, lookup, user);
//        ReferenceViewModel = new Tbl90ReferenceViewModel(factory, dialog, coordinator, lookup, user);
//        InfratribusViewModel = new Tbl63InfratribusViewModel(factory, dialog, coordinator, lookup, user);

//        IsSubtribusDetailVisible = false;
//        IsEditVisible = false;

//        NewCommand = new AsyncRelayCommand(NewAsync);
//    }
//    // ---------------------------------------------------------
//    // UI-STATE
//    // ---------------------------------------------------------
//    public Tbl93CommentViewModel CommentViewModel { get; }
//    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
//    public Tbl63InfratribusViewModel InfratribusViewModel { get; }

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
//    private bool _isSubtribusDetailVisible;
//    public bool IsSubtribusDetailVisible
//    {
//        get => _isSubtribusDetailVisible;
//        set => SetProperty(ref _isSubtribusDetailVisible, value);
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

//    // ============================================
//    // Parent laden (Tribus → Subtribus) 
//    // ============================================
//    public override async Task LoadParentAsync(int parentId)
//    {
//        _currentTribusId = parentId;

//        var items = await Data.GetSubtribusForTribusAsync(parentId);
//        Items = new ObservableCollection<Models.Tbl60Subtribus>(items);
//    }

//    // ============================================
//    // TRIBUS LOOKUP
//    // ============================================
//    private ObservableCollection<Models.Tbl57Tribus> _tribusList = new();
//    public ObservableCollection<Models.Tbl57Tribus> TribusList
//    {
//        get => _tribusList;
//        set => SetProperty(ref _tribusList, value);
//    }

//    public Dictionary<int, string?> TribusLookup { get; set; } = new();

//    private bool _isLoadingTribus = false;

//    public async Task LoadTribusAsync()
//    {
//        if (_isLoadingTribus)
//            return;

//        _isLoadingTribus = true;

//        try
//        {
//            TribusList = await _lookupService.GetTribusListAsync();
//            TribusLookup = await _lookupService.GetTribusLookupAsync();
//        }
//        finally
//        {
//            _isLoadingTribus = false;
//        }
//    }
//    public async Task LoadAsync()
//    {
//        await LoadTribusAsync();
//        await ReloadAndSelectAsync(0, x => x.SubtribusId);
//        IsSubtribusDetailVisible = false;
//    }

//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        IsSubtribusDetailVisible = SelectedItem != null;

//        if (SelectedItem != null)
//            await LoadChildrenAsync(SelectedItem.SubtribusId);
//        else
//            await ClearChildCollectionsAsync();
//    }

//    // ---------------- Children laden (Subtribus → Child-Tabs) ----------------
//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        await CommentViewModel.LoadParentAsync(entityId);
//        await ReferenceViewModel.LoadParentAsync(entityId);
//        await InfratribusViewModel.LoadParentAsync(entityId);
//    }

//    protected override Task ClearChildCollectionsAsync()
//    {
//        CommentViewModel.Items?.Clear();
//        ReferenceViewModel.Items?.Clear();
//        InfratribusViewModel.Items?.Clear();

//        return Task.CompletedTask;
//    }

//    // ---------------- CRUD ----------------
//    protected override async Task NewAsync()
//    {
//        var item = CreateNewItem();
//        SelectedItem = item;

//        // UI binden lassen
//        OnPropertyChanged(nameof(SelectedItem));

//        // Validierung aktivieren
//        item.ValidateSubtribusName();

//        // WICHTIG: UI Zeit geben, den roten Rahmen zu rendern
//        await Task.Delay(50);
//    }

//    protected override Models.Tbl60Subtribus CreateNewItem()
//        => new Models.Tbl60Subtribus
//        {
//            TribusId = _currentTribusId ?? 0
//        };

//    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
//    {
//        if (string.IsNullOrWhiteSpace(CurrentFilter))
//            return Enumerable.Empty<Models.Tbl60Subtribus>();

//        return await Data.SearchSubtribusWithTribusAsync(CurrentFilter);
//    }

//    protected override int GetId(Models.Tbl60Subtribus item)
//    {
//        return item?.SubtribusId ?? 0;
//    }

//    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
//    {
//        if (string.IsNullOrWhiteSpace(item.SubtribusName))
//        {
//            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");

//            return default;
//        }

//        var saved = await Data.UpsertSubtribusAsync(item);
//        return await Data.GetSubtribusByIdAsync(saved.SubtribusId);
//    }
//    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl60Subtribus item)
//    {
//        // Nur setzen, wenn der Benutzer NICHT gesucht hat
//        if (string.IsNullOrWhiteSpace(SearchText))
//        {
//            SearchText = item.SubtribusName;
//            CurrentFilter = item.SubtribusName;
//        }

//        return Task.CompletedTask;
//    }

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
//    {
//        if (item.SubtribusId == 0)
//            return false;

//        // Prüfen, ob abhängige Daten existieren
//        if (await Data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
//            await Data.SubtribusHasReferenceAsync(item.SubtribusId) ||
//            await Data.SubtribusHasCommentAsync(item.SubtribusId))
//        {
//            await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
//            return false;
//        }

//        // Löschen
//        await Data.DeleteSubtribusAsync(item.SubtribusId);
//        return true;
//    }

//    //alternativ
//    //protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
//    //{
//    //    if (item.SubtribusId == 0)
//    //        return false;

//    //    if (await Data.SubtribusHasChildrenAsync(item.SubtribusId))
//    //    {
//    //        await Dialog.ShowErrorAsync(
//    //            "Error_DeleteNotAllowed_Title",
//    //            "Error_DeleteNotAllowed_Message"
//    //        );
//    //        return false;
//    //    }

//    //    await Data.DeleteSubtribusAsync(item.SubtribusId);
//    //    return true;
//    //}

//    protected override Task<bool> ConfirmSaveAsync(Models.Tbl60Subtribus item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl60Subtribus item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

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


//    protected IEnumerable<Models.Tbl60Subtribus> SortItems(
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

//    protected void UpdateHeaderTexts()
//    {
//        const string asc = " ▲";
//        const string desc = " ▼";

//        IdHeaderText = "ID";
//        NameHeaderText = "Name";

//        // Optional: Sorting-Pfeile hinzufügen wenn nötig
//    }

//}
