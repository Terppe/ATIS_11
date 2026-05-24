using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl57Tribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl54Supertribus;

public class Tbl54SupertribusViewModel : BaseBossListViewModel<Models.Tbl54Supertribus>
{
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl54SupertribusViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        Tbl57TribusViewModel tribusViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;

        // Dictionary mit Children initialisieren
        ChildViewModels["Tribus"] = tribusViewModel;
        ChildViewModels["Reference"] = referenceViewModel;
        ChildViewModels["Comment"] = commentViewModel;
    }
    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    public Tbl57TribusViewModel TribusViewModel
    {
        get => ChildViewModels.TryGetValue("Tribus", out var vm)
            ? (Tbl57TribusViewModel)vm
            : null!;
        set => ChildViewModels["Tribus"] = value;
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
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Supertribus;
    protected override HierarchyLevel ParentLevel { get; }
    //   protected override HierarchyLevel ParentLevel => HierarchyLevel.Unknown; // Kein Parent für Supertribus


    // ============================================
    // TEMPLATE IMPLEMENTATIONS
    // ============================================

    protected override async Task<IEnumerable<Models.Tbl54Supertribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl54Supertribus>();

        return Enumerable.Empty<Models.Tbl54Supertribus>();
    }

    protected override Models.Tbl54Supertribus CreateNewItem()
        => new()
        {
            SupertribusId = 0 // Keine Parent-ID, da Supertribus die Top-Level-Ebene ist
        };

    protected override async Task<Models.Tbl54Supertribus?> SaveItemAsync(Models.Tbl54Supertribus item)
    {
        if (await _data.SupertribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertSupertribusAsync(item);
        return await _data.GetSupertribusByIdAsync(saved.SupertribusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl54Supertribus item)
    {
        if (item.SupertribusId == 0)
            return false;

        if (await _data.SupertribusHasTribusAsync(item.SupertribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteSupertribusAsync(item.SupertribusId);
        return true;
    }

    protected override async Task LoadForParentAsync(int parentId)
    {
        throw new NotImplementedException();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        TribusViewModel.Items?.Clear();
        await Task.CompletedTask;
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        await TribusViewModel.LoadForSupertribusAsync(entityId);
    }

    // ============================================
    // DETAIL VISIBILITY
    // ============================================
    private string _searchText = string.Empty;
    public string SearchText
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


    protected override IEnumerable<Models.Tbl54Supertribus> SortItems(
        IEnumerable<Models.Tbl54Supertribus> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.SupertribusId)
                : items.OrderByDescending(x => x.SupertribusId),
            "Name" => ascending
                ? items.OrderBy(x => x.SupertribusName)
                : items.OrderByDescending(x => x.SupertribusName),
            _ => items
        };
    }

    protected override void UpdateHeaderTexts()
    {
        const string asc = " ▲";
        const string desc = " ▼";

        IdHeaderText = "ID";
        NameHeaderText = "Name";

        // Optional: Sorting-Pfeile hinzufügen wenn nötig
    }
}
