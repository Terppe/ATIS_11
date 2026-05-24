using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl66Genus;

/// <summary>
/// Genus ViewModel,
/// - CHILD von Infratribus
/// </summary>
public class Tbl66GenusViewModel : BaseBossListViewModel<Models.Tbl66Genus>
{
    private readonly IDataService _data;

    // ============================================
    // HIERARCHIE-KONFIGURATION
    // ============================================
    protected override HierarchyLevel CurrentLevel => HierarchyLevel.Genus;
    protected override HierarchyLevel ParentLevel => HierarchyLevel.Infratribus;
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl66GenusViewModel(
        IDataService data,
        IDialogService dialog,
        Tbl75SpeciesViewModel speciesViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel,
        HierarchyCoordinator coordinator
    ) : base(data, dialog, coordinator)
    {
        _data = data;
    }
    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    public Tbl75SpeciesViewModel SpeciesViewModel
    {
        get => ChildViewModels.TryGetValue("Species", out var vm)
            ? (Tbl75SpeciesViewModel)vm
            : null!;
        set => ChildViewModels["Species"] = value;
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
    // ---------------------------------------------------------
    // Infratribus lookup
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl63Infratribus> InfratribusList { get; } = new();

    public Dictionary<int, string?> InfratribusLookup { get; } = new();

    public string? GetInfratribusName(int id)
        => InfratribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

    public async Task LoadInfratribusAsync()
    {
        InfratribusList.Clear();
        InfratribusLookup.Clear();

        var list = await _data.GetAllInfratribusAsync();

        foreach (var item in list)
        {
            InfratribusList.Add(item);
            InfratribusLookup[item.InfratribusId] = item.InfratribusName;
        }
    }

    public async Task LoadAsync()
    {
        await LoadInfratribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    protected override async Task<IEnumerable<Models.Tbl66Genus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl66Genus>();

        //if (string.IsNullOrWhiteSpace(CurrentFilter))
        //{
        //    // Lade alle Items, wenn kein Filter gesetzt ist
        //    return await _data.GetAllInfratribusAsync();
        //}

        // Filtere nach dem Text in CurrentFilter
        return await _data.SearchGenusAsync(CurrentFilter);
    }
    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadInfratribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
    }

    public async Task LoadForInfratribusAsync(int infratribusId)
    {
        if (infratribusId <= 0) return;

        var items = await _data.GetGenusForInfratribusAsync(infratribusId);
        Items = new ObservableCollection<Models.Tbl66Genus>(items);
    }
    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------
    private Models.Tbl66Genus? _selectedGenus;
    public Models.Tbl66Genus? SelectedGenus
    {
        get => _selectedGenus;
        set
        {
            if (SetProperty(ref _selectedGenus, value))
            {
                if (value == null)
                {
                    CommentViewModel.CurrentSubtribusId = null;
                    ReferenceViewModel.CurrentSubtribusId = null;
                    return;
                }

                int id = value.GenusId;

                // Parent für Comments
                CommentViewModel.CurrentGenusId = id;
       //         CommentViewModel.CurrentSpeciesId = null;

                // Parent für References
                ReferenceViewModel.CurrentGenusId = id;
       //         ReferenceViewModel.CurrentSpeciesId = null;

                // Parent für Genus
                SpeciesViewModel.CurrentGenusId = id;
            }
        }
    }

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value) && SelectedGenus != null)
            {
                int id = SelectedGenus.GenusId;

                if (value == 1) // Comments Genus
                {
                    CommentViewModel.CurrentGenusId = id;
             //       CommentViewModel.CurrentSpeciesId = null;
                }

                if (value == 2) // References Genus 
                {
                    ReferenceViewModel.CurrentGenusId = id;
          //          ReferenceViewModel.CurrentSpeciesId = null;
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
            if (SetProperty(ref _selectedInnerTabIndex, value) && SelectedGenus != null)
            {
                int id = SelectedGenus.GenusId;

                if (value == 1) // Comments Genus
                {
                    CommentViewModel.CurrentGenusId = id;
          //          CommentViewModel.CurrentSpeciesId = null;
                }

                if (value == 2) // References Genus
                {
                    ReferenceViewModel.CurrentGenusId = id;
            //        ReferenceViewModel.CurrentSpeciesId = null;
                }
            }
        }
    }
    public int? CurrentInfratribusId { get; set; }
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
    private bool _isGenusEditVisible;
    public bool IsGenusEditVisible
    {
        get => _isGenusEditVisible;
        set { _isGenusEditVisible = value; OnPropertyChanged(); }
    }

    // ============================================
    // TEMPLATE IMPLEMENTATIONS
    // ============================================


    protected override Models.Tbl66Genus CreateNewItem()
        => new()
        {
            InfratribusId = _coordinator.GetSelectedId(ParentLevel) ?? 0,
            GenusId = 0
        };

    protected override async Task LoadForParentAsync(int infratribusId)
    {
        var items = await _data.GetGenusForInfratribusAsync(infratribusId);
        Items = new ObservableCollection<Models.Tbl66Genus>(items);
    }

    protected override async Task<Models.Tbl66Genus?> SaveItemAsync(Models.Tbl66Genus item)
    {
        if (await _data.GenusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertGenusAsync(item);
        return await _data.GetGenusByIdAsync(saved.GenusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    {
        if (item.GenusId == 0)
            return false;

        if (await _data.GenusHasReferenceAsync(item.GenusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await _data.DeleteGenusAsync(item.GenusId);
        return true;
    }
    public void ResetBelow()
    {
        //SpeciesViewModel.Items?.Clear();
        //ReferenceViewModel.Items?.Clear();
        //CommentViewModel.Items?.Clear();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        await Task.CompletedTask;
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        // Aktuell keine weitere Child-Ebene für Genus
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


    protected override IEnumerable<Models.Tbl66Genus> SortItems(
        IEnumerable<Models.Tbl66Genus> items, string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending
                ? items.OrderBy(x => x.GenusId)
                : items.OrderByDescending(x => x.GenusId),
            "Name" => ascending
                ? items.OrderBy(x => x.GenusName)
                : items.OrderByDescending(x => x.GenusName),
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
