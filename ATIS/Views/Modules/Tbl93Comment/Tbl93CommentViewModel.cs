using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentViewModel : BaseListViewModel<Models.Tbl93Comment>
{
    private readonly IDialogService _dialog;

    public Tbl93CommentViewModel(IDataService data, IDialogService dialog)
        : base(data)
    {
        _dialog = dialog;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        OpenEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OpenEdit);
        CloseEditCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(CloseEdit);

        // New muss async prüfen (Subtribus ausgewählt?)
        NewCommand = new AsyncRelayCommand(NewAsync);
    }

    // ---------------------------------------------------------
    // Commands (zusätzlich zur Base)
    // ---------------------------------------------------------
    public ICommand SortCommand { get; }
    public ICommand OpenEditCommand { get; }
    public ICommand CloseEditCommand { get; }

    // ---------------------------------------------------------
    // FK: aktueller Subtribus
    // ---------------------------------------------------------
    public int? CurrentSubtribusId { get; private set; }
    public int? CurrentInfratribusId { get; private set; }
    public int? CurrentGenusId { get; private set; }

    public async Task LoadForSubtribusAsync(int subtribusId)
    {
        if (subtribusId <= 0)
        {
            Items = new ObservableCollection<Models.Tbl93Comment>();
            SelectedItem = null;
            IsCommentEditVisible = false;
            return;
        }

        CurrentSubtribusId = subtribusId;
        CurrentInfratribusId = null;

        Items = new ObservableCollection<Models.Tbl93Comment>(
            await _data.GetCommentsForSubtribusAsync(subtribusId));

        SelectedItem = null;
        IsCommentEditVisible = false;
    }

    //public async Task LoadForSubtribusAsync(int subtribusId)
    //{
    //    CurrentSubtribusId = subtribusId;
    //    CurrentInfratribusId = null;

    //    Items = new ObservableCollection<Models.Tbl93Comment>(
    //        await _data.GetCommentsForSubtribusAsync(subtribusId));

    //    SelectedItem = null;
    //    IsCommentEditVisible = false;
    //}
    public async Task LoadForInfratribusAsync(int infratribusId)
    {
        // Modus setzen
        CurrentInfratribusId = infratribusId;
        CurrentSubtribusId = null;

        // Laden
        Items = new ObservableCollection<Models.Tbl93Comment>(
            await _data.GetCommentsForInfratribusAsync(infratribusId)
        );

        DataSetCount = Items.Count;

        // Auswahl zurücksetzen
        SelectItemSilently(null);
        SelectedItem = null;

        // Editor schließen
        IsCommentEditVisible = false;

        // Detailbereich schließen, wenn keine Items
        IsDetailVisible = false;
    }
    public async Task LoadForGenusAsync(int genusId)
    {
        // Modus setzen
        CurrentGenusId = genusId;
        CurrentInfratribusId = null;

        // Laden
        Items = new ObservableCollection<Models.Tbl93Comment>(
            await _data.GetCommentsForGenusAsync(genusId)
        );

        DataSetCount = Items.Count;

        // Auswahl zurücksetzen
        SelectItemSilently(null);
        SelectedItem = null;

        // Editor schließen
        IsCommentEditVisible = false;

        // Detailbereich schließen, wenn keine Items
        IsDetailVisible = false;
    }

    //public async Task LoadForInfratribusAsync(int infratribusId)
    //{
    //    CurrentInfratribusId = infratribusId;
    //    CurrentSubtribusId = null; // ← Modus zurücksetzen

    //    Items = new ObservableCollection<Models.Tbl93Comment>(
    //        await _data.GetCommentsForInfratribusAsync(infratribusId)
    //    );

    //    DataSetCount = Items.Count;

    //    IsCommentEditVisible = false;
    //    SelectItemSilently(null);
    //}

    // FK-Module werden nicht über Base-LoadItems geladen
    protected override Task<IEnumerable<Models.Tbl93Comment>> LoadItemsAsync()
        => Task.FromResult<IEnumerable<Models.Tbl93Comment>>(Enumerable.Empty<Models.Tbl93Comment>());

    protected override Models.Tbl93Comment CreateNewItem()
    {
        if (CurrentSubtribusId != null)
            return new Models.Tbl93Comment { SubtribusId = CurrentSubtribusId.Value };

        if (CurrentInfratribusId != null)
            return new Models.Tbl93Comment { InfratribusId = CurrentInfratribusId.Value };

        throw new InvalidOperationException("No parent context set.");
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl93Comment item)
        => _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl93Comment item)
        => _dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override async Task<Models.Tbl93Comment?> SaveItemAsync(Models.Tbl93Comment item)
    {
        // FK korrekt setzen
        if (CurrentSubtribusId != null)
        {
            item.SubtribusId = CurrentSubtribusId.Value;
            item.InfratribusId = null;
        }

        if (CurrentInfratribusId != null)
        {
            item.InfratribusId = CurrentInfratribusId.Value;
            item.SubtribusId = null;
        }

        // Duplicate check
        if (await _data.CommentExistsAsync(item))
        {
            await _dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await _data.UpsertCommentAsync(item);
        return await _data.GetCommentByIdAsync(saved.CommentId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl93Comment item)
    {
        // Neu/unsaved => verwerfen
        if (item.CommentId == 0)
        {
            IsCommentEditVisible = false;
            SelectItemSilently(null);
            return false;
        }

        await _data.DeleteCommentAsync(item.CommentId);
        return true;
    }

    protected override void OnAfterNew(Models.Tbl93Comment item)
        => IsCommentEditVisible = true;

    protected override bool IsUnsavedNewItem(Models.Tbl93Comment item)
        => item.CommentId == 0;

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
    // Edit visibility
    // ---------------------------------------------------------
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set { _isDetailVisible = value; OnPropertyChanged(); }
    }
    private bool _isDetailVisible;

    private bool _isCommentEditVisible;
    public bool IsCommentEditVisible
    {
        get => _isCommentEditVisible;
        set { _isCommentEditVisible = value; OnPropertyChanged(); }
    }

    private void OpenEdit()
    {
        if (SelectedItem != null)
            IsCommentEditVisible = true;
    }

    private void CloseEdit()
    {
        IsCommentEditVisible = false;
        SelectedItem = null;
    }
    public bool IsInfratribusEditVisible
    {
        get => _isInfratribusEditVisible;
        set { _isInfratribusEditVisible = value; OnPropertyChanged(); }
    }
    private bool _isInfratribusEditVisible;

    public bool IsReferenceEditVisible
    {
        get => _isReferenceEditVisible;
        set { _isReferenceEditVisible = value; OnPropertyChanged(); }
    }
    private bool _isReferenceEditVisible;

    // ---------------------------------------------------------
    // Reload (FK)
    // ---------------------------------------------------------
    public override async Task ReloadAndSelectAsync(Models.Tbl93Comment? saved, bool autoSelect)
    {
        IsLoading = true;
        try
        {
            var old = SelectedItem;

            IsCommentEditVisible = false;

            // 1. Items laden je nach Parent-Modus
            if (CurrentSubtribusId != null)
            {
                Items = new ObservableCollection<Models.Tbl93Comment>(
                    await _data.GetCommentsForSubtribusAsync(CurrentSubtribusId.Value)
                );
            }
            else if (CurrentInfratribusId != null)
            {
                Items = new ObservableCollection<Models.Tbl93Comment>(
                    await _data.GetCommentsForInfratribusAsync(CurrentInfratribusId.Value)
                );
            }
            else
            {
                Items = new ObservableCollection<Models.Tbl93Comment>();
            }

            DataSetCount = Items.Count;

            SelectItemSilently(null);

            if (!autoSelect)
                return;

            // 2. Auswahl wiederherstellen
            if (saved != null)
                SelectedItem = Items.FirstOrDefault(x => x.CommentId == saved.CommentId);
            else if (old != null && old.CommentId != 0)
                SelectedItem = Items.FirstOrDefault(x => x.CommentId == old.CommentId);
            else
                SelectedItem = Items.FirstOrDefault();
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ---------------------------------------------------------
    // Sort
    // ---------------------------------------------------------
    private bool _sortAscending = true;
    private string _currentSortColumn = "";

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

        IEnumerable<Models.Tbl93Comment> sorted = column switch
        {
            "Id" => _sortAscending ? Items.OrderBy(x => x.CommentId)
                                   : Items.OrderByDescending(x => x.CommentId),

            "Info" => _sortAscending ? Items.OrderBy(x => x.Info)
                                     : Items.OrderByDescending(x => x.Info),

            _ => Items
        };

        Items = new ObservableCollection<Models.Tbl93Comment>(sorted);
    }
}
