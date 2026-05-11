using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusViewModel : BaseListViewModel<Models.Tbl63Infratribus>
{
    private readonly IDialogService _dialog;

    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
    public Tbl93CommentViewModel CommentViewModel { get; }
    public Tbl66GenusViewModel GenusViewModel { get; }

    public Tbl63InfratribusViewModel(
        IDataService data,
        IDialogService dialog,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl93CommentViewModel commentViewModel,
        Tbl66GenusViewModel genusViewModel
    ) : base(data)
    {
        _dialog = dialog;
        ReferenceViewModel = referenceViewModel;
        CommentViewModel = commentViewModel;
        GenusViewModel = genusViewModel;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        OpenEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OpenEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);

        NewCommand = new AsyncRelayCommand(NewAsync);

    }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ---------------------------------------------------------
    // Tribus lookup
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; } = new();

    public Dictionary<int, string?> SubtribusLookup { get; } = new();

    public string? GetSubtribusName(int id)
        => SubtribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

    public async Task LoadSubtribusAsync()
    {
        SubtribusList.Clear();
        SubtribusLookup.Clear();

        var list = await _data.GetAllSubtribusAsync();

        foreach (var item in list)
        {
            SubtribusList.Add(item);
            SubtribusLookup[item.SubtribusId] = item.SubtribusName;
        }
    }
    // ---------------------------------------------------------
    // Load entrypoint for Page
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        await LoadSubtribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
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

    // ---------------------------------------------------------
    // Edit visibility
    // ---------------------------------------------------------

    private bool _isInfratribusEditVisible;
    public bool IsInfratribusEditVisible
    {
        get => _isInfratribusEditVisible;
        set { _isInfratribusEditVisible = value; OnPropertyChanged(); }
    }
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
    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------

    public int? CurrentSubtribusId { get; private set; }
    public int? CurrentInfratribusId { get; private set; }

    public async Task LoadForSubtribusAsync(int subtribusId)
    {
        Debug.WriteLine($"[InfratribusVM] LoadForSubtribusAsync({subtribusId})");

        CurrentSubtribusId = subtribusId;
        CurrentInfratribusId = null; // ← Modus zurücksetzen

        Items = new ObservableCollection<Models.Tbl63Infratribus>(await _data.GetInfratribusForSubtribusAsync(subtribusId));

        DataSetCount = Items.Count;

        IsInfratribusEditVisible = false;
        SelectItemSilently(null);
    }

    // ---------------------------------------------------------
    // Load entrypoint
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl63Infratribus>();

        return await _data.SearchInfratribusAsync(CurrentFilter);
    }

    // ---------------------------------------------------------
    // Selection → load children
    // ---------------------------------------------------------
    public override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
        {
            await ReferenceViewModel.LoadForInfratribusAsync(0);
            await CommentViewModel.LoadForInfratribusAsync(0);
            await GenusViewModel.LoadForInfratribusAsync(0);
            return;
        }

        await ReferenceViewModel.LoadForInfratribusAsync(SelectedItem.InfratribusId);
        await CommentViewModel.LoadForInfratribusAsync(SelectedItem.InfratribusId);
        await GenusViewModel.LoadForInfratribusAsync(SelectedItem.InfratribusId);
    }

    private string _searchText = string.Empty;
    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }
    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }


    // ---------------------------------------------------------
    // New
    // ---------------------------------------------------------
    protected override Models.Tbl63Infratribus CreateNewItem()
        => new()
        {
            InfratribusId = 0,
            SubtribusId = CurrentSubtribusId,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now
        };

    protected override void OnAfterNew(Models.Tbl63Infratribus item)
        => IsInfratribusEditVisible = true;

    protected override bool IsUnsavedNewItem(Models.Tbl63Infratribus item)
        => item.InfratribusId == 0;

    private async Task NewAsync()
    {
        if (CurrentSubtribusId <= 0)
        {
            await _dialog.ShowErrorAsync("Hinweis", "Bitte zuerst einen Subtribus auswählen.");
            return;
        }

        base.New();
    }

    // ---------------------------------------------------------
    // Save
    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    // Delete
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl63Infratribus item)
    {
        // Neu/unsaved → verwerfen
        if (item.InfratribusId == 0)
        {
            SelectedItem = null;
            return false;
        }

        // FK-Prüfungen
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

        // Löschen
        await _data.DeleteInfratribusAsync(item.InfratribusId);
        return true;
    }
    //public override async Task ReloadAndSelectAsync(Models.Tbl63Infratribus? saved, bool autoSelect)
    //{
    //    IsLoading = true;
    //    try
    //    {
    //        var old = SelectedItem;

    //        IsInfratribusEditVisible = false;

    //        Items = new ObservableCollection<Models.Tbl63Infratribus>(
    //            await _data.GetInfratribusForSubtribusAsync(CurrentSubtribusId)
    //        );
    //        Debug.WriteLine($"[InfratribusVM] Items.Count = {Items.Count}");

    //        DataSetCount = Items.Count;

    //        SelectItemSilently(null);

    //        if (!autoSelect)
    //            return;

    //        if (saved != null)
    //            SelectedItem = Items.FirstOrDefault(x => x.InfratribusId == saved.InfratribusId);
    //        else if (old != null && old.InfratribusId != 0)
    //            SelectedItem = Items.FirstOrDefault(x => x.InfratribusId == old.InfratribusId);
    //        else
    //            SelectedItem = Items.FirstOrDefault();
    //    }
    //    finally
    //    {
    //        IsLoading = false;
    //    }
    //}
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

        IEnumerable<Models.Tbl63Infratribus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.InfratribusId)
                                   : Items.OrderByDescending(x => x.InfratribusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.InfratribusName)
                                       : Items.OrderByDescending(x => x.InfratribusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl63Infratribus>(sorted);

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
