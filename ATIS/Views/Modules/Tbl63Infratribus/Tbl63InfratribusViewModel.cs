using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusViewModel
    : BaseBossSortableViewModel<Models.Tbl63Infratribus>
{
    private readonly IDataService _data;
    private readonly IUserService _user;

    public IAsyncRelayCommand SaveCommentCommand { get; }
    public IAsyncRelayCommand DeleteCommentCommand { get; }
    public IRelayCommand NewCommentCommand { get; }

    // ---------------------------------------------------------
    // FK-MODULE
    // ---------------------------------------------------------
    public Tbl93CommentViewModel CommentViewModel { get; }

    public Tbl63InfratribusViewModel(IDataService data, IUserService user)
    {
        _data = data;
        _user = user;

        CommentViewModel = new Tbl93CommentViewModel(data);
        CommentViewModel.Parent = this;

        SaveCommentCommand = new AsyncRelayCommand(
            SaveCommentAsync,
            () => CommentViewModel.SelectedComment != null
        );
        DeleteCommentCommand = new AsyncRelayCommand(
            DeleteCommentAsync,
            () => CommentViewModel.SelectedComment != null
        );
        NewCommentCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(
            NewComment,
            () => SelectedItem != null
        );

    }
    // ---------------------------------------------------------
    #region [  GridLength ]
    // ---------------------------------------------------------

    public GridLength IdWidth { get; } = new GridLength(120);     // ID
    public GridLength Extra1Width { get; } = new GridLength(250); // Name
    public GridLength Extra2Width { get; } = new GridLength(250); // SubtribusID später SubtribusName
    public GridLength Extra3Width { get; } = new GridLength(80);  // Valid
    public GridLength Extra4Width { get; } = new GridLength(100); // ValidYear
    public GridLength Extra5Width { get; } = new GridLength(150); // Author
    public GridLength Extra6Width { get; } = new GridLength(100); // AuthorYear
    public GridLength Extra7Width { get; } = new GridLength(0);   // leer
    public GridLength Extra8Width { get; } = new GridLength(0);   // leer
    #endregion [  GridLength ]

    // ---------------------------------------------------------
    // SUBTRIBUS LOOKUP
    // ---------------------------------------------------------
    public ObservableCollection<Models.Tbl60Subtribus> SubtribusList { get; }
        = new ObservableCollection<Models.Tbl60Subtribus>();

    public Dictionary<int, string> SubtribusLookup { get; }
        = new Dictionary<int, string>();

    public string GetSubtribusName(int id)
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
    // INITIAL LOAD
    // ---------------------------------------------------------
    public  async Task LoadAsync()
    {
        await base.LoadAsync();

        CommentViewModel.Reset();

        await LoadSubtribusAsync();

    }

    //----------------------------------------------------------
    // SEARCH (serverseitig)
    //----------------------------------------------------------
    public async Task ApplyFilterAsync(string search)
    {
        _currentFilter = search ?? "";

        if (string.IsNullOrWhiteSpace(_currentFilter))
        {
            Items.Clear();
   //         DataSetCount = 0;
            return;
        }

        var result = await _data.SearchInfratribusAsync(_currentFilter);

        Items.Clear();
        foreach (var item in result)
            Items.Add(item);

    //    DataSetCount = Items.Count;
    }
    //public async Task ApplyFilterAsync(string search)
    //{
    //    var result = await _dataService.SearchInfratribusAsync(search);

    //    Items = new ObservableCollection<Models.Tbl63Infratribus>(result);
    //    OnPropertyChanged(nameof(Items));

    //    // Wichtig:
    //    SelectItem(null);
    //}

    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Models.Tbl63Infratribus CreateNewDomain()
    {
        return new Models.Tbl63Infratribus
        {
            Valid = false,
            ValidYear = DateTime.Now.Year.ToString(),
            SubtribusId = 0
        };
    }
    private void NewComment()
    {
        Debug.WriteLine("NewComment");
        if (SelectedItem == null)
            return;

        var newComment = new Models.Tbl93Comment
        {
            InfratribusId = SelectedItem.InfratribusId,
            Valid = true,
            ValidYear = DateTime.Now.Year.ToString()
        };

        CommentViewModel.Comments.Add(newComment);
        CommentViewModel.SelectedComment = newComment;
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override Task<Models.Tbl63Infratribus> SaveDomainAsync(Models.Tbl63Infratribus domain)
    {
        return _data.UpsertInfratribusAsync(domain);
      //  RestoreRowFocusToSelected();
    }

    private async Task SaveCommentAsync()
    {
        if (CommentViewModel.SelectedComment == null)
            return;

        await _data.UpsertCommentAsync(CommentViewModel.SelectedComment);
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task DeleteDomainAsync(Models.Tbl63Infratribus domain)
    {
        if (await _data.HasGenusAsync(domain.InfratribusId))
        {
            await ShowErrorAsync("Löschen nicht möglich. Es existieren abhängige Genus-Datensätze.");
            return;
        }

        await _data.DeleteCommentsAsync(domain.InfratribusId);
        await _data.DeleteReferencesAsync(domain.InfratribusId);
        await _data.DeleteInfratribusAsync(domain.InfratribusId);
    }

    private Task ShowErrorAsync(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
        return Task.CompletedTask;
    }
    private async Task DeleteCommentAsync()
    {
        if (CommentViewModel.SelectedComment == null)
            return;

        var comment = CommentViewModel.SelectedComment;

        await _data.DeleteCommentAsync(comment.CommentId);

        CommentViewModel.Comments.Remove(comment);
        CommentViewModel.SelectedComment =
            CommentViewModel.Comments.FirstOrDefault();
    }

    // ---------------------------------------------------------
    // LOAD (für ReloadAndSelectAsync)
    // ---------------------------------------------------------
    protected override Task<IEnumerable<Models.Tbl63Infratribus>> LoadItemsAsync()
        => _data.GetAllInfratribusAsync();

    protected override Task<IEnumerable<Models.Tbl63Infratribus>> SearchAsync(string filter)
        => _data.SearchInfratribusAsync(filter);

    // ---------------------------------------------------------
    // SELECTION CHANGED
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        if (SelectedItem != null)
            await CommentViewModel.LoadCommentsAsync(SelectedItem.InfratribusId);
        else
            CommentViewModel.Reset();


        SaveCommentCommand.NotifyCanExecuteChanged();
        DeleteCommentCommand.NotifyCanExecuteChanged();
        NewCommentCommand.NotifyCanExecuteChanged();

    }

    // ---------------------------------------------------------
    // ID COMPARISON
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl63Infratribus domain)
        => domain.InfratribusId;

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override IEnumerable<Models.Tbl63Infratribus> SortItems(string column, bool ascending)
    {
        return column switch
        {
            "Id" => ascending ? Items.OrderBy(x => x.InfratribusId) : Items.OrderByDescending(x => x.InfratribusId),
            "Extra1" => ascending ? Items.OrderBy(x => x.InfratribusName) : Items.OrderByDescending(x => x.InfratribusName),
            "Extra2" => ascending ? Items.OrderBy(x => x.SubtribusId) : Items.OrderByDescending(x => x.SubtribusId),
            "Extra3" => ascending ? Items.OrderBy(x => x.Valid) : Items.OrderByDescending(x => x.Valid),
            "Extra4" => ascending ? Items.OrderBy(x => x.ValidYear) : Items.OrderByDescending(x => x.ValidYear),
            "Extra5" => ascending ? Items.OrderBy(x => x.Author) : Items.OrderByDescending(x => x.Author),
            "Extra6" => ascending ? Items.OrderBy(x => x.AuthorYear) : Items.OrderByDescending(x => x.AuthorYear),
            _ => Items
        };
    }


//// ---------------------------------------------------------
//// SORT
//// ---------------------------------------------------------
//private bool _sortAscending = true;
//    private string _currentSortColumn = "";

//    public void SortBy(string column)
//    {
//        if (Items == null || Items.Count == 0)
//            return;

//        // Sortierrichtung bestimmen
//        if (_currentSortColumn == column)
//            _sortAscending = !_sortAscending;
//        else
//        {
//            _currentSortColumn = column;
//            _sortAscending = true;
//        }

//        // Sortierung anwenden
//        IEnumerable<Models.Tbl63Infratribus> sorted = column switch
//        {
//            "Id" => _sortAscending
//                                ? Items.OrderBy(x => x.InfratribusId)
//                                : Items.OrderByDescending(x => x.InfratribusId),

//            "Extra1" => _sortAscending
//                                ? Items.OrderBy(x => x.InfratribusName)
//                                : Items.OrderByDescending(x => x.InfratribusName),

//            _ => Items
//        };

//        // Neue sortierte Liste setzen
//        Items = new ObservableCollection<Models.Tbl63Infratribus>(sorted);

//        // Auswahl zurücksetzen
//        SelectItem(null);

//        // Header aktualisieren (Pfeile)
//        //UpdateHeaderTexts();
//    }

    //private void UpdateHeaderTexts()
    //{
    //    string asc = " ▲";
    //    string desc = " ▼";

    //    // Grundtexte
    //    IdHeaderText = "ID";
    //    NameHeaderText = "Name";

    //    // Pfeil bestimmen
    //    string suffix = _sortAscending ? asc : desc;

    //    // Pfeil an die richtige Spalte hängen
    //    switch (_currentSortColumn)
    //    {
    //        case "Id":
    //            IdHeaderText += suffix;
    //            break;

    //        case "Name":
    //            NameHeaderText += suffix;
    //            break;

    //    }

    //    // UI aktualisieren
    //    OnPropertyChanged(nameof(IdHeaderText));
    //    OnPropertyChanged(nameof(NameHeaderText));
    //}

}
