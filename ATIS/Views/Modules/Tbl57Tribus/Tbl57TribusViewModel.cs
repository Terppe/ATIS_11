using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl60Subtribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl57Tribus;

/// <summary>
/// Tribus ist CHAIN:
/// - CHILD von Supertribus
/// - PARENT für Subtribus
/// </summary>
public class Tbl57TribusViewModel : BaseBossListViewModel<Models.Tbl57Tribus>
{
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl57TribusViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        Tbl60SubtribusViewModel subtribusViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel, 
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;

        // Dictionary mit Children initialisieren
        ChildViewModels["Subtribus"] = subtribusViewModel;
        ChildViewModels["Reference"] = referenceViewModel;
        ChildViewModels["Comment"] = commentViewModel;
    }
    public void ResetBelow()
    {
        SubtribusViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
    }

    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    public Tbl60SubtribusViewModel SubtribusViewModel
    {
        get => ChildViewModels.TryGetValue("Subtribus", out var vm)
            ? (Tbl60SubtribusViewModel)vm
            : null!;
        set => ChildViewModels["Subtribus"] = value;
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
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Tribus;
    protected override HierarchyLevel ParentLevel => HierarchyLevel.Supertribus;
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    public async Task LoadForSupertribusAsync(int supertribusId)
    {
        if (supertribusId <= 0) return;

        var items = await _data.GetTribusForSupertribusAsync(supertribusId);
        Items = new ObservableCollection<Models.Tbl57Tribus>(items);
    }

    // ---------------------------------------------------------
    // Supertribus lookup
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl54Supertribus> SupertribusList { get; } = new();

    public Dictionary<int, string?> SupertribusLookup { get; } = new();

    public string? GetSupertribusName(int id)
        => SupertribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

    public async Task LoadSupertribusAsync()
    {
        SupertribusList.Clear();
        SupertribusLookup.Clear();

        var list = await _data.GetAllSupertribusAsync();

        foreach (var item in list)
        {
            SupertribusList.Add(item);
            SupertribusLookup[item.SupertribusId] = item.SupertribusName;
        }
    }
    public async Task LoadAsync()
    {
        await LoadSupertribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    protected override async Task<IEnumerable<Models.Tbl57Tribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl57Tribus>();

        //if (string.IsNullOrWhiteSpace(CurrentFilter))
        //{
        //    // Lade alle Items, wenn kein Filter gesetzt ist
        //    return await _data.GetAllTribusAsync();
        //}

        // Filtere nach dem Text in CurrentFilter
        return await _data.SearchTribusAsync(CurrentFilter);
    }
    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadSupertribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
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
    private Models.Tbl57Tribus? _selectedTribus;
    public Models.Tbl57Tribus? SelectedTribus
    {
        get => _selectedTribus;
        set
        {
            if (SetProperty(ref _selectedTribus, value))
            {
                if (value == null)
                {
                    CommentViewModel.CurrentTribusId = null;
                    ReferenceViewModel.CurrentTribusId = null;
                    return;
                }

                int id = value.TribusId;

                // Parent für Comments
                CommentViewModel.CurrentTribusId = id;
                CommentViewModel.CurrentSubtribusId = null;

                // Parent für References
                ReferenceViewModel.CurrentTribusId = id;
                ReferenceViewModel.CurrentSubtribusId = null;

                // Parent für Subtribus
       //         SubtribusViewModel.CurrenTribusId = id;
            }
        }
    }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value) && SelectedTribus != null)
            {
                int id = SelectedTribus.TribusId;

                if (value == 1) // Comments Tribus
                {
                    CommentViewModel.CurrentTribusId = id;
                    CommentViewModel.CurrentSupertribusId = null;
                }

                if (value == 2) // References Tribus
                {
                    ReferenceViewModel.CurrentTribusId = id;
                    ReferenceViewModel.CurrentSupertribusId = null;
                }
            }
        }
    }

    private int _selectedInnerTabIndex;
    public int SelectedInnerTabIndex
    {
        get => _selectedInnerTabIndex;
        set
        {
            if (SetProperty(ref _selectedInnerTabIndex, value) && SelectedTribus != null)
            {
                int id = SelectedTribus.TribusId;

                if (value == 1) // Comments Tribus
                {
                    CommentViewModel.CurrentTribusId = id;
                    CommentViewModel.CurrentSupertribusId = null;
                }

                if (value == 2) // References Tribus
                {
                    ReferenceViewModel.CurrentTribusId = id;
                    ReferenceViewModel.CurrentSupertribusId = null;
                }
            }
        }
    }


    // ============================================
    // TEMPLATE IMPLEMENTATIONS
    // ============================================

    protected override Models.Tbl57Tribus CreateNewItem()
        => new()
        {
            SupertribusId = _coordinator.GetSelectedId(ParentLevel) ?? 0,
            TribusId = 0
        };

    protected override async Task LoadForParentAsync(int supertribusId)
    {
        var items = await _data.GetTribusForSupertribusAsync(supertribusId);
        Items = new ObservableCollection<Models.Tbl57Tribus>(items);
    }

    protected override async Task<Models.Tbl57Tribus?> SaveItemAsync(Models.Tbl57Tribus item)
    {
        if (await _data.TribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertTribusAsync(item);
        return await _data.GetTribusByIdAsync(saved.TribusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl57Tribus item)
    {
        if (item.TribusId == 0)
            return false; // Unsaved

        if (await _data.TribusHasSubtribusAsync(item.TribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteTribusAsync(item.TribusId);
        return true;
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        SubtribusViewModel.Items?.Clear();
        await Task.CompletedTask;
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        await SubtribusViewModel.LoadForTribusAsync(entityId);
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


    protected override IEnumerable<Models.Tbl57Tribus> SortItems(
        IEnumerable<Models.Tbl57Tribus> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.TribusId)
                : items.OrderByDescending(x => x.TribusId),
            "Name" => ascending
                ? items.OrderBy(x => x.TribusName)
                : items.OrderByDescending(x => x.TribusName),
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
