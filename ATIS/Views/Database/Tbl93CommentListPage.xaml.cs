
using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  Tbl93CommentListPage.xaml.cs Skriptdatum:  06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl93CommentListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl93CommentViewModel ViewModel { get; }
    private Tbl93Comment SingleComment { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl93CommentListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl93CommentViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]         

    #region [Comment Methods]

    private async void GetCommentsByInfoOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl93CommentsList?.Clear();

            ViewModel.CommentItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl93CommentsList?.Clear();

                CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
                CommentDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingComment = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl93CommentsList ??= [];
                ViewModel.Tbl93CommentsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl93Comments
                .Where(e => e.CommentId == id)]
                : [.. Context.Tbl93Comments
                .Where(e => e.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];

            });
            CommentDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;

            if (ViewModel.Tbl93CommentsList is { Count: 0 })
            {
                ViewModel.Tbl93CommentsList?.Clear();

                AppBarButtonDeleteComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelComment.Visibility = Visibility.Visible;
                CommentDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl93CommentListPage));
            }
            else
            {
                CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
                ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void CommentDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await CommentDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task CommentDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.CommentSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddComment()
    {
        ViewModel.IsInEditComment = true;
        ViewModel.IsNewComment = true;

        await ViewModel.AddCommentAsync();

        if (ViewModel.Tbl93CommentsList != null)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
        }

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;

        InfoComment.PlaceholderText = StringsRes.EnterText;
    }

    private void CommentSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        CommentSearchBox.Visibility = Visibility.Visible;
        CommentSearchBox.Focus(FocusState.Programmatic);
        CommentSearchBox.PlaceholderText = StringsRes.SearchInfoOrId;
    }

    private void CommentDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl93CommentsList.Sort);
    }

    private void AddNewCommentCanceled(object sender, EventArgs e) => CommentDetailExpander.IsExpanded = false;

    private async void ViewCommentHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewCommentHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewCommentHyperlink()
    {
        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;

        if (CommentDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleComment = App.SingleComment(ViewModel.CommentSelected.CommentId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsCommentAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewComment)
        {
            CommentDetailExpander.IsExpanded = false;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelCommentEdits();
        }
    }

    private void CancelCommentEdits()
    {
        var x = (DataGrid)FindName("CommentDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];
            SingleComment = App.SingleComment(ViewModel.CommentSelected.CommentId);
            CommentSingles();
        }
        else
        {
            CommentDetailExpander.IsExpanded = false;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
    }

    private void CommentSingles()
    {
        ViewModel.CommentSelected.Valid = SingleComment.Valid;
        ViewModel.CommentSelected.ValidYear = SingleComment.ValidYear;
        ViewModel.CommentSelected.Info = SingleComment.Info;
        ViewModel.CommentSelected.Memo = SingleComment.Memo;
        ViewModel.CommentSelected.RegnumId = SingleComment.RegnumId;
        ViewModel.CommentSelected.PhylumId = SingleComment.PhylumId;
        ViewModel.CommentSelected.DivisionId = SingleComment.DivisionId;
        ViewModel.CommentSelected.SubphylumId = SingleComment.SubphylumId;
        ViewModel.CommentSelected.SubdivisionId = SingleComment.SubdivisionId;
        ViewModel.CommentSelected.SuperclassId = SingleComment.SuperclassId;
        ViewModel.CommentSelected.ClassId = SingleComment.ClassId;
        ViewModel.CommentSelected.SubclassId = SingleComment.SubclassId;
        ViewModel.CommentSelected.InfraclassId = SingleComment.InfraclassId;
        ViewModel.CommentSelected.LegioId = SingleComment.LegioId;
        ViewModel.CommentSelected.OrdoId = SingleComment.OrdoId;
        ViewModel.CommentSelected.SubordoId = SingleComment.SubordoId;
        ViewModel.CommentSelected.InfraordoId = SingleComment.InfraordoId;
        ViewModel.CommentSelected.SuperfamilyId = SingleComment.SuperfamilyId;
        ViewModel.CommentSelected.FamilyId = SingleComment.FamilyId;
        ViewModel.CommentSelected.SubfamilyId = SingleComment.SubfamilyId;
        ViewModel.CommentSelected.InfrafamilyId = SingleComment.InfrafamilyId;
        ViewModel.CommentSelected.SupertribusId = SingleComment.SupertribusId;
        ViewModel.CommentSelected.TribusId = SingleComment.TribusId;
        ViewModel.CommentSelected.SubtribusId = SingleComment.SubtribusId;
        ViewModel.CommentSelected.InfratribusId = SingleComment.InfratribusId;
        ViewModel.CommentSelected.GenusId = SingleComment.GenusId;
        ViewModel.CommentSelected.SpeciesId = SingleComment.SpeciesId;
    }
    private async Task RevertChangesCommentAsync()
    {
        ViewModel.IsInEditComment = false;
        if (ViewModel.IsModifiedComment)
        {
            await RefreshCommentAsync();
            ViewModel.IsModifiedComment = false;
        }
    }
    private Task RefreshCommentAsync()
    {
        if (CommentDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleComment = App.SingleComment(ViewModel.CommentSelected.CommentId);
            CommentSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteComment()
    {
        var deleteDialog = new DeleteDialog
        {
            PrimaryButtonText = StringsRes.Delete,
            SecondaryButtonText = StringsRes.DoNotDelete,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Delete,
            XamlRoot = Content.XamlRoot
        };
        await deleteDialog.ShowAsync();
        var res = deleteDialog.Result;

        switch (res)
        {
            case DeleteDialogResult.Delete:
                await ViewModel.DeleteCommentAsync(ViewModel.CommentSelected);
                await Refresh();
                CommentDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesCommentAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveComment();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveComment()
    {
        var saveDialog = new SaveChangesDialog
        {
            PrimaryButtonText = StringsRes.Save,
            SecondaryButtonText = StringsRes.DoNotSave,
            CloseButtonText = StringsRes.Cancel,
            Title = StringsRes.Save,
            XamlRoot = Content.XamlRoot
        };
        await saveDialog.ShowAsync();
        var res = saveDialog.Result;

        switch (res)
        {
            case SaveChangesDialogResult.Save:
                if (await BeforeSaveComment())
                {
                    return;
                }
                await ViewModel.SaveCommentAsync(ViewModel.CommentSelected);
                await BehindSaveComment(ViewModel.CommentSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedComment = true;
                await RevertChangesCommentAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveComment()
    {
        _search = CommentSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .Where(a => a.Info == ViewModel.CommentSelected.Info &&
                        a.RegnumId == ViewModel.CommentSelected.RegnumId &&
                        a.PhylumId == ViewModel.CommentSelected.PhylumId &&
                        a.DivisionId == ViewModel.CommentSelected.DivisionId &&
                        a.SubphylumId == ViewModel.CommentSelected.SubphylumId &&
                        a.SubdivisionId == ViewModel.CommentSelected.SubdivisionId &&
                        a.SuperclassId == ViewModel.CommentSelected.SuperclassId &&
                        a.ClassId == ViewModel.CommentSelected.ClassId &&
                        a.SubclassId == ViewModel.CommentSelected.SubclassId &&
                        a.InfraclassId == ViewModel.CommentSelected.InfraclassId &&
                        a.LegioId == ViewModel.CommentSelected.LegioId &&
                        a.OrdoId == ViewModel.CommentSelected.OrdoId &&
                        a.SubordoId == ViewModel.CommentSelected.SubordoId &&
                        a.InfraordoId == ViewModel.CommentSelected.InfraordoId &&
                        a.SuperfamilyId == ViewModel.CommentSelected.SuperfamilyId &&
                        a.FamilyId == ViewModel.CommentSelected.FamilyId &&
                        a.SubfamilyId == ViewModel.CommentSelected.SubfamilyId &&
                        a.InfrafamilyId == ViewModel.CommentSelected.InfrafamilyId &&
                        a.SupertribusId == ViewModel.CommentSelected.SupertribusId &&
                        a.TribusId == ViewModel.CommentSelected.TribusId &&
                        a.SubtribusId == ViewModel.CommentSelected.SubtribusId &&
                        a.InfratribusId == ViewModel.CommentSelected.InfratribusId &&
                        a.GenusId == ViewModel.CommentSelected.GenusId &&
                        a.SpeciesId == ViewModel.CommentSelected.SpeciesId)];
        });
        if (ViewModel.IsNewComment)
        {
            if (ViewModel.Tbl93CommentsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedComment = true;
        return false;
    }

    private async Task BehindSaveComment(Tbl93Comment viewModelCommentSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            if (CommentSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl93CommentsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl93Comments
                    .Where(a => a.CommentId == id)]
                    : [.. Context.Tbl93Comments
                    .Where(a => _search != null && a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            }
        });

        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(a => a.CommentId == viewModelCommentSelected.CommentId)];
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
            await Refresh();
            CommentDataGrid.SelectedIndex = 0;
        }
        else
        {
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
            var index = ViewModel.CommentItems.ToList().FindIndex(item => item.CommentId == viewModelCommentSelected.CommentId);
            CommentDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;
    }

    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Comment Methods]        

    private void BackDetailExpanders()
    {
        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    #region [Refresh Methods]
    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Refresh();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task Refresh()
    {
        ViewModel.IsLoading = true;

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (string.IsNullOrEmpty(_search))
        {
            //DetailPanelComment.Visibility = Visibility.Visible;
            //CommentDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("CommentDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl93CommentsList!.Count)
            {
                ViewModel.CommentItems.Clear();
                foreach (var item in ViewModel.Tbl93CommentsList)
                {
                    ViewModel.CommentItems.Add(item);
                }

                ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];
            }
            CommentDataGrid.SelectedIndex = iNdx;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl93CommentsList ??= [];
                ViewModel.Tbl93CommentsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl93Comments
                .Where(a => a.CommentId == id)]
                : [.. Context.Tbl93Comments
                    .Where(a => a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            });
            if (ViewModel.Tbl93CommentsList is { Count: 0 })
            {
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;

            if (ViewModel.IsDeletingComment)
            {
                if (iNdx == 0 && ViewModel.Tbl93CommentsList.Count == 0)
                {
                    ViewModel.Tbl93CommentsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];
            }
        }
        CommentDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingComment = false;
    }

    #endregion [Refresh Methods] 


}

