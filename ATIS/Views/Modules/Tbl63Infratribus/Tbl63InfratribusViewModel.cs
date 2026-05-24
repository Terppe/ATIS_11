using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl63Infratribus;

/// <summary>
/// Infratribus ist gleichzeitig:
/// - CHILD von Subtribus
/// - PARENT für Genus, Comments, References
/// </summary>
public class Tbl63InfratribusViewModel : BaseBossListViewModel<Models.Tbl63Infratribus>
{
    private readonly IDataService _data;
    private readonly LookupService _lookupService;

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl63InfratribusViewModel(
        IDataService data,
        IDialogService dialog,
        LookupService lookupService,
        Tbl66GenusViewModel genusViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
        _lookupService = lookupService;  // ← Speichern

        // Dictionary mit Children initialisieren
        ChildViewModels["Genus"] = genusViewModel;
        ChildViewModels["Reference"] = referenceViewModel;
        ChildViewModels["Comment"] = commentViewModel;
    }

    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    public Tbl66GenusViewModel GenusViewModel
    {
        get => ChildViewModels.TryGetValue("Genus", out var vm)
            ? (Tbl66GenusViewModel)vm
            : null!;
        set => ChildViewModels["Genus"] = value;
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
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Infratribus;
    protected override HierarchyLevel ParentLevel => HierarchyLevel.Subtribus;

    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ============================================
    // SUBTRIBUS-DATEN LADEN
    // ============================================
    public async Task LoadAsync()
    {
        await LoadSubtribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    private ObservableCollection<Models.Tbl60Subtribus> _subtribusList = new();
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList
    {
        get => _subtribusList;
        set => SetProperty(ref _subtribusList, value);
    }
    public Dictionary<int, string?> SubtribusLookup { get; set; } = new();
    private bool _isLoadingSubtribus = false;

    public async Task LoadSubtribusAsync()
    {
        // Verhindere gleichzeitiges Laden
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
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await GenusViewModel.LoadForInfratribusAsync(entityId);
        await CommentViewModel.LoadForInfratribusAsync(entityId);
        await ReferenceViewModel.LoadForInfratribusAsync(entityId);

        await ReferenceViewModel.LoadReferenceExpertsAsync();
        await ReferenceViewModel.LoadReferenceSourcesAsync();
        await ReferenceViewModel.LoadReferenceAuthorsAsync();
    }

    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl63Infratribus>();

        // Filtere nach dem Text in CurrentFilter
        return await _data.SearchInfratribusWithSubtribusAsync(CurrentFilter);
    }

    public async Task LoadForSubtribusAsync(int subtribusId)
    {
        if (subtribusId <= 0) return;

        var items = await _data.GetInfratribusForSubtribusAsync(subtribusId);
        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
    }

    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadSubtribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
    }
    public override void New()
    {
        // Erstelle leeren neuen Datensatz
        var item = CreateNewItem();

        // Falls kein ItemsSource vorhanden, initialisiere sie
        Items ??= new ObservableCollection<Models.Tbl63Infratribus>();
        Items.Add(item);

        // Setzte das neu erstellte Item als ausgewählt
        SelectedItem = item;

        // Zeige das EditControl für dieses neue Item
        IsEditVisible = true;
    }
    protected override Models.Tbl63Infratribus CreateNewItem()
        => new()
        {
            SubtribusId = _coordinator.GetSelectedId(ParentLevel) ?? 0,
            InfratribusId = 0
        };

    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------
    public int? CurrentSubtribusId { get; set; }
    // ---------------------------------------------------------
    // Edit visibility
    // ---------------------------------------------------------
    private void OpenEdit()
    {
        if (SelectedItem != null)
            IsInfratribusEditVisible = true;
    }

    private void CloseEdit()
    {
        IsInfratribusEditVisible = false;
        SelectedItem = null;
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

    private bool _isInfratribusEditVisible;
    public bool IsInfratribusEditVisible
    {
        get => _isInfratribusEditVisible;
        set { _isInfratribusEditVisible = value; OnPropertyChanged(); }
    }

    private Models.Tbl63Infratribus? _selectedInfratribus;
    public Models.Tbl63Infratribus? SelectedInfratribus
    {
        get => _selectedInfratribus;
        set
        {
            if (SetProperty(ref _selectedInfratribus, value))
            {
                if (value == null)
                {
                    CommentViewModel.CurrentInfratribusId = null;
                    ReferenceViewModel.CurrentInfratribusId = null;
                    return;
                }

                int id = value.InfratribusId;

                // Parent für Comments
                CommentViewModel.CurrentInfratribusId = id;
                CommentViewModel.CurrentGenusId = null;

                // Parent für References
                ReferenceViewModel.CurrentInfratribusId = id;
                ReferenceViewModel.CurrentGenusId = null;

                // Parent für Infratribus
                GenusViewModel.CurrentInfratribusId = id;
            }
        }
    }
    public void ResetBelow()
    {
        GenusViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
    }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value) && SelectedInfratribus != null)
            {
                int id = SelectedInfratribus.InfratribusId;

                if (value == 1) // Comments Infratribus
                {
                    CommentViewModel.CurrentInfratribusId = id;
                    CommentViewModel.CurrentSubtribusId = null;
                }

                if (value == 2) // References Infratribus
                {
                    ReferenceViewModel.CurrentInfratribusId = id;
                    ReferenceViewModel.CurrentSubtribusId = null;
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
            if (SetProperty(ref _selectedInnerTabIndex, value) && SelectedInfratribus != null)
            {
                int id = SelectedInfratribus.InfratribusId;

                if (value == 1) // Comments Infratribus
                {
                    CommentViewModel.CurrentInfratribusId = id;
                    CommentViewModel.CurrentSubtribusId = null;
                }

                if (value == 2) // References Infratribus
                {
                    ReferenceViewModel.CurrentInfratribusId = id;
                    ReferenceViewModel.CurrentSubtribusId = null;
                }
            }
        }
    }


    // ============================================
    // TEMPLATE IMPLEMENTATIONS
    // ============================================


    protected override async Task LoadForParentAsync(int subtribusId)
    {
        var items = await _data.GetInfratribusForSubtribusAsync(subtribusId);
        Items = new ObservableCollection<Models.Tbl63Infratribus>(items);
    }

    protected override async Task<Models.Tbl63Infratribus?> SaveItemAsync(Models.Tbl63Infratribus item)
    {
        if (await _data.InfratribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertInfratribusAsync(item);
        return await _data.GetInfratribusByIdAsync(saved.InfratribusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
    {
        if (item.InfratribusId == 0)
            return false; // Unsaved

        if (await _data.InfratribusHasGenusAsync(item.InfratribusId) ||
            await _data.InfratribusHasReferenceAsync(item.InfratribusId) ||
            await _data.InfratribusHasCommentAsync(item.InfratribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteInfratribusAsync(item.InfratribusId);
        return true;
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        GenusViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();
        await Task.CompletedTask;
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


    protected override IEnumerable<Models.Tbl63Infratribus> SortItems(
        IEnumerable<Models.Tbl63Infratribus> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.InfratribusId)
                : items.OrderByDescending(x => x.InfratribusId),
            "Name" => ascending
                ? items.OrderBy(x => x.InfratribusName)
                : items.OrderByDescending(x => x.InfratribusName),
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
