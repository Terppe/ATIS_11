using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public class Tbl60SubtribusViewModel : BaseListViewModel<Models.Tbl60Subtribus>
{
    public Tbl93CommentViewModel CommentViewModel { get; }
    public Tbl90ReferenceViewModel ReferenceViewModel { get; }
    public Tbl63InfratribusViewModel InfratribusViewModel { get; }

    private readonly IDialogService _dialog;

    public Tbl60SubtribusViewModel(
        IDataService data,
        IDialogService dialog,
        Tbl93CommentViewModel commentViewModel,
        Tbl90ReferenceViewModel referenceViewModel,
        Tbl63InfratribusViewModel infratribusViewModel
    ) : base(data)
    {
        _dialog = dialog;
        CommentViewModel = commentViewModel;
        ReferenceViewModel = referenceViewModel;
        InfratribusViewModel = infratribusViewModel;

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
    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------

    public int? CurrentTribusId { get; private set; }
    public int? CurrentSubtribusId { get; private set; }

    public async Task LoadForTribusAsync(int tribusId)
    {
        //Debug.WriteLine($"[TribusVM] LoadForTribusAsync({tribusId})");

        //CurrentTribusId = tribusId;
        //CurrentSubtribusId = null; // ← Modus zurücksetzen

        //Items = new ObservableCollection<Models.Tbl60Subtribus>(await _data.GetSubtribusForTribusAsync(tribusId));

        //DataSetCount = Items.Count;

        //IsSubtribusEditVisible = false;
        //SelectItemSilently(null);
    }

    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentFilter))
            return Enumerable.Empty<Models.Tbl60Subtribus>();

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
    private bool _isSubtribusEditVisible;
    public bool IsSubtribusEditVisible
    {
        get => _isSubtribusEditVisible;
        set { _isSubtribusEditVisible = value; OnPropertyChanged(); }
    }

    // ---------------------------------------------------------
    // Selection -> load comments, refernces, infratribusses for FK
    // ---------------------------------------------------------

    public override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem == null)
        {
            await ReferenceViewModel.LoadForSubtribusAsync(0);
            await CommentViewModel.LoadForSubtribusAsync(0);
            await InfratribusViewModel.LoadForSubtribusAsync(0);
            return;
        }

        await CommentViewModel.LoadForSubtribusAsync(SelectedItem.SubtribusId);
        await ReferenceViewModel.LoadForSubtribusAsync(SelectedItem.SubtribusId);
        await InfratribusViewModel.LoadForSubtribusAsync(SelectedItem.SubtribusId);
    }

    private Models.Tbl93Comment? _selectedComment;

    public Models.Tbl93Comment? SelectedComment
    {
        get => _selectedComment;
        set => SetProperty(ref _selectedComment, value);
    }
    private Models.Tbl90Reference? _selectedReference;

    public Models.Tbl90Reference? SelectedReference
    {
        get => _selectedReference;
        set => SetProperty(ref _selectedReference, value);
    }
    private Models.Tbl63Infratribus? _selectedInfratribus;

    public Models.Tbl63Infratribus? SelectedInfratribus
    {
        get => _selectedInfratribus;
        set => SetProperty(ref _selectedInfratribus, value);
    }

    // ---------------------------------------------------------
    // New
    // ---------------------------------------------------------
    protected override Models.Tbl60Subtribus CreateNewItem()
        => new()
        {
            TribusId = TribusList.First().TribusId,   // ← gültiger Wert
            SubtribusId = 0
        };


    protected override void OnAfterNew(Models.Tbl60Subtribus item)
    {
        // 2. Detailbereich öffnen
        IsDetailVisible = true;
    }

    protected override bool IsUnsavedNewItem(Models.Tbl60Subtribus item)
        => item.SubtribusId == 0;

    // ---------------------------------------------------------
    // Confirm dialogs from base hooks
    // ---------------------------------------------------------
    protected override Task<bool> ConfirmSaveAsync(Models.Tbl60Subtribus item)
        => _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl60Subtribus item)
        => _dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    // ---------------------------------------------------------
    // Save/Delete
    // ---------------------------------------------------------
    protected override async Task OnBeforeReloadAfterSaveAsync(Models.Tbl60Subtribus item)
    {
        // Wenn SearchBox leer ist → Filter automatisch setzen
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            SearchText = item.SubtribusName;
            CurrentFilter = item.SubtribusName;
            OnPropertyChanged(nameof(CurrentFilter));
        }
    }

    protected override async Task<Models.Tbl60Subtribus?> SaveItemAsync(Models.Tbl60Subtribus item)
    {
        if (await _data.SubtribusExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null; // IMPORTANT: base soll nicht reload+select auf "unsaved"
        }

        var saved = await _data.UpsertSubtribusAsync(item);
        return await _data.GetSubtribusByIdAsync(saved.SubtribusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl60Subtribus item)
    {
        // Neu/unsaved → verwerfen
        if (item.SubtribusId == 0)
        {
            SelectedItem = null;
            return false;
        }

        // FK-Prüfungen
        if (await _data.SubtribusHasInfratribusAsync(item.SubtribusId) ||
            await _data.SubtribusHasReferenceAsync(item.SubtribusId) ||
            await _data.SubtribusHasCommentAsync(item.SubtribusId))
        {
            await _dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        // Löschen
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

        IEnumerable<Models.Tbl60Subtribus> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.SubtribusId)
                                   : Items.OrderByDescending(x => x.SubtribusId),

            "Extra1" => _sortAscending ? Items.OrderBy(x => x.SubtribusName)
                                       : Items.OrderByDescending(x => x.SubtribusName),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl60Subtribus>(sorted);

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
