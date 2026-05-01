using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl93Comment;

public class Tbl60SubtribusViewModel : BaseListViewModel<Tbl60Subtribus>
{
    public Tbl93CommentViewModel CommentViewModel { get; }

    private readonly IDialogService _dialog;

    public Tbl60SubtribusViewModel(
        IDataService data,
        IDialogService dialog,
        Tbl93CommentViewModel commentViewModel
    ) : base(data)
    {
        _dialog = dialog;
        CommentViewModel = commentViewModel;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
    }

    // ---------------------------------------------------------
    // Commands
    // ---------------------------------------------------------
    public ICommand SortCommand { get; }

    // ---------------------------------------------------------
    // Tribus lookup
    // ---------------------------------------------------------
    public ObservableCollection<Tbl57Tribus> TribusList { get; } = new();

    public Dictionary<int, string?> TribusLookup { get; } = new();

    public string? GetTribusName(int id)
        => TribusLookup.TryGetValue(id, out var name) ? name : string.Empty;

    public async Task LoadTribusAsync()
    {
        TribusList.Clear();
        TribusLookup.Clear();

        var list = await _data.GetAllTribusAsync();

        foreach (var item in list)
        {
            TribusList.Add(item);
            TribusLookup[item.TribusId] = item.TribusName;
        }
    }

    // ---------------------------------------------------------
    // Load entrypoint for Page
    // ---------------------------------------------------------
    public async Task LoadAsync()
    {
        await LoadTribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: true);
    }

    public override async Task ApplyFilterAsync(string? filter)
    {
        if (SuppressFilter)
            return;

        CurrentFilter = filter;
        OnPropertyChanged(nameof(CurrentFilter));

        await LoadTribusAsync();
        await ReloadAndSelectAsync(saved: default, autoSelect: false);
    }

    protected override async Task<IEnumerable<Tbl60Subtribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Tbl60Subtribus>();

        return await _data.SearchSubtribusAsync(CurrentFilter);
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
    // Selection -> load comments for FK
    // ---------------------------------------------------------
    public override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
            return;

        await CommentViewModel.LoadForSubtribusAsync(SelectedItem.SubtribusId);
    }

    private Tbl93Comment? _selectedComment;

    public Tbl93Comment? SelectedComment
    {
        get => _selectedComment;
        set => SetProperty(ref _selectedComment, value);
    }

    // ---------------------------------------------------------
    // New
    // ---------------------------------------------------------
    protected override Tbl60Subtribus CreateNewItem()
        => new()
        {
            TribusId = TribusList.First().TribusId,   // ← gültiger Wert
            SubtribusId = 0
        };

    protected override void OnAfterNew(Tbl60Subtribus item)
    {
        // 2. Detailbereich öffnen
        IsDetailVisible = true;
    }

    protected override bool IsUnsavedNewItem(Tbl60Subtribus item)
        => item.SubtribusId == 0;

    // ---------------------------------------------------------
    // Confirm dialogs from base hooks
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Tbl60Subtribus item)
        => _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Tbl60Subtribus item)
        => _dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    // ---------------------------------------------------------
    // Save/Delete
    // ---------------------------------------------------------
    protected override async Task OnBeforeReloadAfterSaveAsync(Tbl60Subtribus item)
    {
        // Wenn SearchBox leer ist → Filter automatisch setzen
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = item.SubtribusName;
            CurrentFilter = item.SubtribusName;
            OnPropertyChanged(nameof(CurrentFilter));
        }
    }

    protected override async Task<Tbl60Subtribus?> SaveItemAsync(Tbl60Subtribus item)
    {
        if (await _data.SubtribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null; // IMPORTANT: base soll nicht reload+select auf "unsaved"
        }

        var saved = await _data.UpsertSubtribusAsync(item);
        return await _data.GetSubtribusByIdAsync(saved.SubtribusId);
    }

    protected override async Task<bool> DeleteItemAsync(Tbl60Subtribus item)
    {
        // Unsaved -> verwerfen, kein reload
        if (item.SubtribusId == 0)
        {
            SelectedItem = default;
            return false;
        }

        if (await _data.HasInfratribusAsync(item.SubtribusId))
        {
            await _dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
            return false;
        }

        if (await _data.HasCommentAsync(item.SubtribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Subtribus kann nicht gelöscht werden, weil Comments vorhanden sind."
            );
            return false;
        }

        await _data.DeleteSubtribusAsync(item.SubtribusId);
        return true;
    }

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

        IEnumerable<Tbl60Subtribus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.SubtribusId)
                                   : Items.OrderByDescending(x => x.SubtribusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.SubtribusName)
                                       : Items.OrderByDescending(x => x.SubtribusName),

            _ => Items
        };

        Items = new ObservableCollection<Tbl60Subtribus>(sorted);

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
