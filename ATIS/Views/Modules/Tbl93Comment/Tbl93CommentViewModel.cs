using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
{
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl93CommentViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;
        InitCommands();
    }
    // ============================================
    // INIT
    // ============================================

    protected void InitCommands()
    {
        NewCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(New);

        //SaveCommand = new AsyncRelayCommand(async () =>
        //    {
        //        if (!await ConfirmSaveAsync(SelectedItem))
        //            return;

        //        await SaveAsync();
        //    },
        //    () => SelectedItem != null);

        //DeleteCommand = new AsyncRelayCommand(async () =>
        //    {
        //        if (!await ConfirmDeleteAsync(SelectedItem))
        //            return;

        //        await DeleteAsync();
        //    },
        //    () => SelectedItem != null);

        RefreshCommand = new AsyncRelayCommand(ReloadAsync);

        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() =>
        {
            SelectedItem = null;          // Editor schließen
            IsCommentEditVisible = false; // optional
        });
    }

    // ============================================
    // HIERARCHIE-KONFIGURATION
    // ============================================
    // Comments sind LEAF-Level (haben keine Children)
    // Sie können unter Infratribus oder Subtribus existieren
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Comment;

    protected override HierarchyLevel ParentLevel
    {
        get
        {
            // Nutze den höchsten/aktuellsten Parent-Level
            if (CurrentInfratribusId.HasValue)
                return HierarchyLevel.Infratribus;
            if (CurrentSubtribusId.HasValue)
                return HierarchyLevel.Subtribus;
            if (CurrentGenusId.HasValue)
                return HierarchyLevel.Genus;

            return HierarchyLevel.Infratribus; // Default Fallback
        }
    }
    public ICommand CloseEditCommand { get; private set; }
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }

    // ============================================
    // PARENT-IDs (Kontexte für verschiedene Level)
    // ============================================
    public int? CurrentSupertribusId { get; set; }
    public int? CurrentTribusId { get; set; }
    public int? CurrentSubtribusId { get; set; }
    public int? CurrentInfratribusId { get; set; }
    public int? CurrentGenusId { get; set; }
    public int? CurrentSpeciesId { get; set; }

    // ============================================
    // LOAD ITEMS
    // ============================================
    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
    {
        if (CurrentSubtribusId == null)
            return Enumerable.Empty<Models.Tbl93Comment>();

        return await _data.GetCommentsForSubtribusAsync(CurrentSubtribusId.Value);
    }

    //protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
    //{
    //    if (string.IsNullOrWhiteSpace(CurrentFilter))
    //        return Enumerable.Empty<Models.Tbl93Comment>();

    //    return await _data.SearchCommentAsync(CurrentFilter);
    //}

    protected override Task LoadForParentAsync(int parentId)
    {
        // Comments haben keinen generischen Parent
        return Task.CompletedTask;
    }

    public async Task LoadForSubtribusAsync(int subtribusId)
    {
        CurrentSubtribusId = subtribusId;

        var items = await _data.GetCommentsForSubtribusAsync(subtribusId);
        Items = new ObservableCollection<Models.Tbl93Comment>(items);

        // SelectedItem NICHT anfassen
        IsCommentEditVisible = false;
    }

    public async Task LoadForInfratribusAsync(int infratribusId)
    {
        CurrentInfratribusId = infratribusId;
        CurrentSubtribusId = null;

        Items = new ObservableCollection<Models.Tbl93Comment>(await _data.GetCommentsForInfratribusAsync(infratribusId));

        IsCommentEditVisible = false;
    }


    // ============================================
    // NEW / CREATE
    // ============================================
    public override void New()
    {
        // Comments können an verschiedenen Kontexten erstellt werden
        //if (CurrentInfratribusId == null && CurrentSubtribusId == null)
        //{
        //    _ = _dialog.ShowErrorAsync(
        //        "Error_NoParent_Title",
        //        "Bitte wählen Sie zuerst ein Infratribus oder Subtribus."
        //    );
        //    return;
        //}

        var item = CreateNewItem();

        Items ??= new ObservableCollection<Models.Tbl93Comment>();
        Items.Add(item);

        // Direkt nach dem Setzen von Items
        if (Items.Any(c => c is null))
        {
            throw new InvalidOperationException("Items enthält null-Einträge.");
        }

        SelectedItem = item;   // aber im richtigen ViewModel!
        IsCommentEditVisible = true;
    }

    protected override Models.Tbl93Comment CreateNewItem()
    {
        // Priorität: Infratribus > Subtribus
        if (CurrentInfratribusId.HasValue && CurrentInfratribusId > 0)
        {
            return new Models.Tbl93Comment
            {
                InfratribusId = CurrentInfratribusId.Value,
                CommentId = 0
            };
        }

        if (CurrentSubtribusId.HasValue && CurrentSubtribusId > 0)
        {
            return new Models.Tbl93Comment
            {
                SubtribusId = CurrentSubtribusId.Value,
                CommentId = 0
            };
        }

        throw new InvalidOperationException("No parent context set (Infratribus or Subtribus).");
    }

    // ============================================
    // SAVE
    // ============================================
    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
    {
        return await _dialog.ShowConfirmAsync("Save_Confirm_Title", "Save_Confirm_Message");
    }

    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
    {
        //if (string.IsNullOrWhiteSpace(item.Info))
        //{
        //    await _dialog.ShowErrorAsync("Dialog_Error_Empty_Title", "Dialog_Error_Empty_Message");
        //    return null;
        //}

        //if (await _data.CommentExistsAsync(item))
        //{
        //    await _dialog.ShowErrorAsync("Dialog_Error_RecordExists_Title", "Dialog_Error_RecordExists_Message");
        //    return null;
        //}

        var saved = await _data.UpsertCommentAsync(item);
        var reloaded = await _data.GetCommentByIdAsync(saved.CommentId);

        await ReloadAndSelectAsync(reloaded);

        return reloaded;
    }

    // ============================================
    // DELETE
    // ============================================
    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
    {
        return await _dialog.ShowConfirmAsync("Delete_Confirm_Title", "Delete_Confirm_Message");
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
    {
        if (item.CommentId == 0)
            return false;

        await _data.DeleteCommentAsync(item.CommentId);
        return true;
    }

    // ============================================
    // CHILDREN (Comments haben keine Children)
    // ============================================
    protected override async Task ClearChildCollectionsAsync()
    {
        await Task.CompletedTask; // Comments sind Leaf-Level
    }
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await Task.CompletedTask; // Comments sind Leaf-Level
    }
    public override Task OnSelectedItemChangedAsync()
    {
        // Comments haben keine Children → nichts löschen, nichts laden
        IsCommentEditVisible = SelectedItem != null;
        return Task.CompletedTask;
    }
    protected void OnHierarchyLevelChanged(object? sender, HierarchyLevelChangedArgs args)
    {
        // Comments reagieren NICHT auf generische Hierarchie-Events
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

    private string _nameHeaderText = "Comment";
    public string NameHeaderText
    {
        get => _nameHeaderText;
        set => SetProperty(ref _nameHeaderText, value);
    }

    protected override IEnumerable<Models.Tbl93Comment> SortItems(
        IEnumerable<Models.Tbl93Comment> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.CommentId)
                : items.OrderByDescending(x => x.CommentId),
            "Name" => ascending
                ? items.OrderBy(x => x.Info)
                : items.OrderByDescending(x => x.Info),
            _ => items
        };
    }

    protected override void UpdateHeaderTexts()
    {
        IdHeaderText = "ID";
        NameHeaderText = "Comment";
    }

    // ============================================
    // EDIT VISIBILITY
    // ============================================
    private bool _isCommentEditVisible;
    public bool IsCommentEditVisible
    {
        get => _isCommentEditVisible;
        set => SetProperty(ref _isCommentEditVisible, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    private string _searchText = string.Empty;


    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
}

//public class Tbl93CommentViewModel : BaseBossListViewModel<Models.Tbl93Comment>
//{
//    private readonly IDataService _data;

//    // Parent-IDs (werden zur Erstellung eines neuen Kommentars benötigt)
//    public int? CurrentSupertribusId { get; set; }
//    public int? CurrentTribusId { get; set; }
//    public int? CurrentSubtribusId { get; set; }
//    public int? CurrentInfratribusId { get; set; }
//    public int? CurrentGenusId { get; set; }
//    public int? CurrentSpeciesId { get; set; }
//    public ICommand SortCommand { get; }
//    public ICommand OpenEditCommand { get; }
//    public ICommand CloseEditCommand { get; }

//    // ======================================
//    // KONSTRUKTOR
//    // ======================================
//    public Tbl93CommentViewModel(
//        IDataService data,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator
//    ) : base(data, dialog, coordinator)
//    {
//        _data = data;

//        // Subscribing zu Änderungen im Hierarchiekoordinator
//        _coordinator.LevelChanged += OnHierarchyLevelChanged;
//    }
//    // ======================================
//    // WIRD VON SUBTRIBUS AUFGERUFEN
//    // ======================================
//    public async Task LoadForSubtribusAsync(int subtribusId)
//    {
//        if (subtribusId <= 0) return;

//        // Lade alle Kommentare für ein spezifisches Subtribus
//        var items = await _data.GetCommentsForSubtribusAsync(subtribusId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);
//    }

//    // ======================================
//    // WIRD VON INFRATRIBUS AUFGERUFEN
//    // ======================================
//    public async Task LoadForInfratribusAsync(int infratribusId)
//    {
//        if (infratribusId <= 0) return;

//        // Lade alle Kommentare für ein spezifisches Infratribus
//        var items = await _data.GetCommentsForInfratribusAsync(infratribusId);
//        Items = new ObservableCollection<Models.Tbl93Comment>(items);
//    }

//    // ======================================
//    // HANDLING HIERARCHIE-ÄNDERUNGEN
//    // ======================================
//    protected override HierarchyLevel CurrentLevel { get; }
//    protected override HierarchyLevel ParentLevel { get; }

//    private void OnHierarchyLevelChanged(object? sender, HierarchyLevelChangedArgs e)
//    {
//        // Wenn die Ebene Subtribus ist, aktualisiere `CurrentSubtribusId`.
//        if (e.Level == HierarchyLevel.Subtribus)
//        {
//            CurrentSubtribusId = e.SelectedId;
//        }

//        // Wenn die Ebene Infratribus ist, aktualisiere `CurrentInfratribusId`.
//        if (e.Level == HierarchyLevel.Infratribus)
//        {
//            CurrentInfratribusId = e.SelectedId;
//        }
//    }
//    protected override async Task ClearChildCollectionsAsync()
//    {
//        // Keine Kindersammlungen zum Löschen im CommentViewModel
//        await Task.CompletedTask;
//    }
//    protected override async Task LoadForParentAsync(int parentId)
//    {
//        throw new NotImplementedException();
//    }


//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        throw new NotImplementedException();
//    }

//    protected override IEnumerable<Models.Tbl93Comment> SortItems(IEnumerable<Models.Tbl93Comment> items, string column, bool ascending)
//    {
//        return column switch
//        {
//            "Id" => ascending
//                ? items.OrderBy(x => x.CommentId)
//                : items.OrderByDescending(x => x.CommentId),
//            "Name" => ascending
//                ? items.OrderBy(x => x.Info) // Falls ein "CommentText" existiert
//                : items.OrderByDescending(x => x.Info),
//            _ => items
//        };
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

//    protected override void UpdateHeaderTexts()
//    {
//        const string asc = " ▲";
//        const string desc = " ▼";

//        IdHeaderText = "Comment ID";
//        NameHeaderText = "Comment Text";

//        //if (CurrentSortColumn == "Id")
//        //{
//        //    IdHeaderText += CurrentSortAscending ? asc : desc;
//        //}
//        //else if (CurrentSortColumn == "Name") // Falls du andere Spalten unterstützt
//        //{
//        //    NameHeaderText += CurrentSortAscending ? asc : desc;
//        //}
//    }
//    // ======================================
//    // NEUES ITEM ERSTELLEN
//    // ======================================
//    protected override Models.Tbl93Comment CreateNewItem()
//    {
//        if (CurrentInfratribusId != null)
//        {
//            return new Models.Tbl93Comment
//            {
//                InfratribusId = CurrentInfratribusId.Value
//            };
//        }

//        if (CurrentSubtribusId != null)
//        {
//            return new Models.Tbl93Comment
//            {
//                SubtribusId = CurrentSubtribusId.Value
//            };
//        }

//        throw new InvalidOperationException("No parent context set.");
//    }

//    // ======================================
//    // FILTERING / LADE-OPTIONEN
//    // ======================================
//    protected override async Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
//    {
//        if (CurrentInfratribusId != null)
//        {
//            // Lade Kommentare für aktuelles Infratribus-Item
//            return await _data.GetCommentsForInfratribusAsync(CurrentInfratribusId.Value);
//        }

//        if (CurrentSubtribusId != null)
//        {
//            // Lade Kommentare für aktuelles Subtribus-Item
//            return await _data.GetCommentsForSubtribusAsync(CurrentSubtribusId.Value);
//        }

//        return Enumerable.Empty<Models.Tbl93Comment>();
//    }

//    // ======================================
//    // ENTWÜRFE SPEICHERN
//    // ======================================
//    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
//    {
//        if (await _data.CommentExistsAsync(item))
//        {
//            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null;
//        }

//        var saved = await _data.UpsertCommentAsync(item);
//        return await _data.GetCommentByIdAsync(saved.CommentId);
//    }

//    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
//    {
//        throw new NotImplementedException();
//    }
//}
