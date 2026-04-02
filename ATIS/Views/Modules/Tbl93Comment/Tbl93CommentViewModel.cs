using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Modules.Tbl63Infratribus;

namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentViewModel : ObservableObject
{
    private readonly IDataService _dataService;
    public Tbl63InfratribusViewModel Parent { get; set; }

    public Tbl93CommentViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    // ---------------------------------------------------------
    // GRID WIDTHS (neutral)
    // // ---------------------------------------------------------
    public GridLength IdWidth { get; } = new GridLength(80);
    public GridLength InfoWidth { get; } = new GridLength(1, GridUnitType.Star);
    // ---------------------------------------------------------
    // DATA
    // ---------------------------------------------------------
    private ObservableCollection<Models.Tbl93Comment> _comments = new();
    public ObservableCollection<Models.Tbl93Comment> Comments
    {
        get => _comments;
        set => SetProperty(ref _comments, value);
    }

    private Models.Tbl93Comment _selectedComment;
    public Models.Tbl93Comment SelectedComment
    {
        get => _selectedComment;
        set
        {
            if (SetProperty(ref _selectedComment, value))
            {
                Parent?.SaveCommentCommand.NotifyCanExecuteChanged();
                Parent?.DeleteCommentCommand.NotifyCanExecuteChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // LOAD
    // ---------------------------------------------------------
    public async Task LoadCommentsAsync(int infratribusId)
    {
        var list = await _dataService.GetCommentsForInfratribusAsync(infratribusId);

        Comments = new ObservableCollection<Models.Tbl93Comment>(list);
        SelectedComment = Comments.FirstOrDefault();
    }

    public void Reset()
    {
        Comments.Clear();
        SelectedComment = null;
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        set => SetProperty(ref _dataSetCount, value);
    }

}
