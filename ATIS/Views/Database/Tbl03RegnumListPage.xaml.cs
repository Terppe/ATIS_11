
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

//  Tbl03RegnumListPage.xaml.cs Skriptdatum:  30.12.2024  12:32           

namespace ATIS.Views.Database;

public sealed partial class Tbl03RegnumListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl03RegnumViewModel ViewModel { get; }
    private Tbl03Regnum SingleRegnum { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl06Phylum SinglePhylum { get; set; }
    private Tbl09Division SingleDivision { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl03RegnumListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl03RegnumViewModel>();
            InitializeComponent();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Regnum Methods]

    private async void GetRegnumsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl03RegnumsList?.Clear();

            ViewModel.RegnumItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl03RegnumsList?.Clear();

                RegnumDataGrid.ItemsSource = ViewModel.Tbl03RegnumsList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl06PhylumsList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                PhylumsExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingRegnum = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl03RegnumsList ??= [];
                ViewModel.Tbl03RegnumsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl03Regnums
                .Where(a => a.RegnumId == id)]
                : [.. Context.Tbl03Regnums
                .Where(a => _search != null && a.RegnumName.StartsWith(_search))
                .OrderBy(a => a.RegnumName)
                .ThenBy(a => a.Subregnum)];
            });

            RegnumDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            PhylumsExpander.Visibility = Visibility.Collapsed;
            PhylumDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.RegnumDataSetCount = ViewModel.Tbl03RegnumsList.Count;

            if (ViewModel.Tbl03RegnumsList is { Count: 0 })
            {
                ViewModel.Tbl03RegnumsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl06PhylumsList?.Clear();

                AppBarButtonDeleteRegnum.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelRegnum.Visibility = Visibility.Visible;
                RegnumDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                PhylumsExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl03RegnumListPage));
            }
            else
            {
                RegnumDataGrid.ItemsSource = ViewModel.Tbl03RegnumsList;
                ViewModel.RegnumSelected = ViewModel.Tbl03RegnumsList[0];
            }
            ButtonsVisible();
            ButtonsIconVisible();
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void RegnumDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await RegnumDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task RegnumDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.RegnumSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.RegnumSelected != null)
        {
            //----------------------------------Reference--------------------------------      
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.RegnumId == ViewModel.RegnumSelected.RegnumId)];
            });
            ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;
            ViewModel.RefreshReferenceItems();

            if (ViewModel.Tbl90ReferencesList.Count == 0)
            {
                ReferencesExpander.Visibility = Visibility.Collapsed;
                ReferenceDataGrid.ItemsSource = null;
            }
            else
            {
                ReferencesExpander.Visibility = Visibility.Visible;
                ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            }

            //----------------------------------Comment--------------------------------
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl93CommentsList ??= [];
                ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .AsNoTracking()
            .Where(com => com.RegnumId == ViewModel.RegnumSelected.RegnumId)];
            });
            ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;
            ViewModel.RefreshCommentItems();

            if (ViewModel.Tbl93CommentsList.Count == 0)
            {
                CommentsExpander.Visibility = Visibility.Collapsed;
                CommentDataGrid.ItemsSource = null;
            }
            else
            {
                CommentsExpander.Visibility = Visibility.Visible;
                CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
            }
            //--------------------------Phylum-----------------------------   
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl06PhylumsList ??= [];
                ViewModel.Tbl06PhylumsList = [.. Context.Tbl06Phylums
            .AsNoTracking()
            .Where(x => x.RegnumId == ViewModel.RegnumSelected.RegnumId)];
            });
            ViewModel.PhylumDataSetCount = ViewModel.Tbl06PhylumsList.Count;
            ViewModel.RefreshPhylumItems();

            if (ViewModel.Tbl06PhylumsList.Count == 0)
            {
                PhylumsExpander.Visibility = Visibility.Collapsed;
                PhylumDataGrid.ItemsSource = null;
            }
            else
            {
                PhylumsExpander.Visibility = Visibility.Visible;
                PhylumDataGrid.ItemsSource = ViewModel.Tbl06PhylumsList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddRegnum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddRegnum();
            RegnumName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(RegnumName);
            };
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddRegnum()
    {
        ViewModel.IsInEditRegnum = true;
        ViewModel.IsNewRegnum = true;

        await ViewModel.AddRegnumAsync();

        if (ViewModel.Tbl03RegnumsList != null)
        {
            ViewModel.RegnumSelected = ViewModel.Tbl03RegnumsList[0];
        }

        RegnumDetailExpander.IsExpanded = true;
        RegnumDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        RegnumName.PlaceholderText = StringsRes.EnterText;
    }

    private void RegnumSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        RegnumSearchBox.Visibility = Visibility.Visible;
        RegnumSearchBox.Focus(FocusState.Programmatic);
        RegnumSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void RegnumDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl03RegnumsList.Sort);
    }

    private void AddNewRegnumCanceled(object sender, EventArgs e) => RegnumDetailExpander.IsExpanded = false;

    private async void ViewRegnumHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewRegnumHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewRegnumHyperlink()
    {
        ViewModel.IsInEditRegnum = false;
        ViewModel.IsNewRegnum = false;
        RegnumDetailExpander.IsExpanded = true;
        RegnumDetailExpander.Visibility = Visibility.Visible;

        if (RegnumDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleRegnum = App.SingleRegnum(ViewModel.RegnumSelected.RegnumId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsRegnumAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewRegnum)
        {
            RegnumDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelRegnumEdits();
            RegnumDetailExpander.Visibility = Visibility.Collapsed;
        }
    }

    private void CancelRegnumEdits()
    {
        var x = (DataGrid)FindName("RegnumDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.RegnumSelected = ViewModel.Tbl03RegnumsList[iNdx];
            SingleRegnum = App.SingleRegnum(ViewModel.RegnumSelected.RegnumId);
            RegnumSingles();
        }
        else
        {
            RegnumDetailExpander.IsExpanded = false;
            RegnumDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditRegnum = false;
        ViewModel.IsNewRegnum = false;
    }

    private void RegnumSingles()
    {
        ViewModel.RegnumSelected.RegnumName = SingleRegnum.RegnumName;
        ViewModel.RegnumSelected.Subregnum = SingleRegnum.Subregnum;
        ViewModel.RegnumSelected.Valid = SingleRegnum.Valid;
        ViewModel.RegnumSelected.ValidYear = SingleRegnum.ValidYear;
        ViewModel.RegnumSelected.Author = SingleRegnum.Author;
        ViewModel.RegnumSelected.AuthorYear = SingleRegnum.AuthorYear;
        ViewModel.RegnumSelected.Info = SingleRegnum.Info;
        ViewModel.RegnumSelected.Synonym = SingleRegnum.Synonym;
        ViewModel.RegnumSelected.EngName = SingleRegnum.EngName;
        ViewModel.RegnumSelected.GerName = SingleRegnum.GerName;
        ViewModel.RegnumSelected.FraName = SingleRegnum.FraName;
        ViewModel.RegnumSelected.PorName = SingleRegnum.PorName;
        ViewModel.RegnumSelected.Memo = SingleRegnum.Memo;
    }
    private async Task RevertChangesRegnumAsync()
    {
        ViewModel.IsInEditRegnum = false;
        if (ViewModel.IsModifiedRegnum)
        {
            await RefreshRegnumAsync();
            ViewModel.IsModifiedRegnum = false;
        }
    }
    private Task RefreshRegnumAsync()
    {
        if (RegnumDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.RegnumSelected.RegnumId == 0)
            {
                return Task.CompletedTask;
            }
            SingleRegnum = App.SingleRegnum(ViewModel.RegnumSelected.RegnumId);
            RegnumSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteRegnum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteRegnum();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteRegnum()
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
                await ViewModel.DeleteRegnumAsync(ViewModel.RegnumSelected);
                await Refresh();
                RegnumDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesRegnumAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveRegnum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveRegnum();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveRegnum()
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
                if (await BeforeSaveRegnum())
                {
                    return;
                }
                await ViewModel.SaveRegnumAsync(ViewModel.RegnumSelected);
                await BehindSaveRegnum(ViewModel.RegnumSelected);
                await Refresh();
                ViewModel.IsNewRegnum = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedRegnum = true;
                await RevertChangesRegnumAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveRegnum()
    {
        _search = RegnumSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl03RegnumsList ??= [];
            ViewModel.Tbl03RegnumsList = [.. Context.Tbl03Regnums
                .Where(a => a.RegnumName == ViewModel.RegnumSelected.RegnumName &&
                        a.Subregnum == ViewModel.RegnumSelected.Subregnum)];
        });
        if (ViewModel.IsNewRegnum)
        {
            if (ViewModel.Tbl03RegnumsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedRegnum = true;
        return false;
    }

    private async Task BehindSaveRegnum(Tbl03Regnum viewModelRegnumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl03RegnumsList ??= [];
            if (RegnumSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl03RegnumsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl03Regnums
                    .Where(a => a.RegnumId == id)]
                    : [.. Context.Tbl03Regnums
                    .Where(a => _search != null && a.RegnumName.StartsWith(_search))
                    .OrderBy(a => a.RegnumName)];
            }
        });

        ViewModel.RefreshRegnumItems();

        if (ViewModel.Tbl03RegnumsList.Count == 0)
        {
            ViewModel.Tbl03RegnumsList = [.. Context.Tbl03Regnums
                .Where(a => a.RegnumId == viewModelRegnumSelected.RegnumId)];
            RegnumDataGrid.ItemsSource = ViewModel.Tbl03RegnumsList;
            await Refresh();
            RegnumDataGrid.SelectedIndex = 0;
        }
        else
        {
            RegnumDataGrid.ItemsSource = ViewModel.Tbl03RegnumsList;
            var index = ViewModel.RegnumItems.ToList().FindIndex(item => item.RegnumId == viewModelRegnumSelected.RegnumId);
            RegnumDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.RegnumDataSetCount = ViewModel.Tbl03RegnumsList.Count;
    }

    private void RegnumName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackRegnum_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Regnum Methods]                    

    #region [Reference Methods]

    private void AddNewReferenceCanceled(object sender, EventArgs e) => ReferencesExpander.IsExpanded = false;

    private void DataGrid_Reference_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90ReferencesList.Sort);
    }

    private async void ViewReferenceHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewReferenceHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewReferenceHyperlink()
    {
        ViewModel.IsInEditReference = false;
        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReference = false;
        ViewModel.IsNewReferenceExpert = false;
        ViewModel.IsNewReferenceSource = false;
        ViewModel.IsNewReferenceAuthor = false;

        if (ViewModel.ReferenceSelected != null)
        {
            if (ViewModel.ReferenceSelected.RefExpertId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceExpertDetailExpander.IsExpanded = true;
                ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 0;
                TabViewItemReferenceExpert.Visibility = Visibility.Visible;
                TabViewItemReferenceSource.IsEnabled = false;
                TabViewItemReferenceAuthor.IsEnabled = false;
                return Task.CompletedTask;
            }
            if (ViewModel.ReferenceSelected.RefSourceId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceSourceDetailExpander.IsExpanded = true;
                ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 1;
                TabViewItemReferenceSource.Visibility = Visibility.Visible;
                TabViewItemReferenceExpert.IsEnabled = false;
                TabViewItemReferenceAuthor.IsEnabled = false;
                return Task.CompletedTask;
            }
            if (ViewModel.ReferenceSelected.RefAuthorId != null)
            {
                ReferenceDetailExpander.IsExpanded = true;
                ReferenceDetailExpander.Visibility = Visibility.Visible;
                ReferenceAuthorDetailExpander.IsExpanded = true;
                ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
                TabViewReference.SelectedIndex = 2;
                TabViewItemReferenceAuthor.Visibility = Visibility.Visible;
                TabViewItemReferenceExpert.IsEnabled = false;
                TabViewItemReferenceSource.IsEnabled = false;
            }
        }

        if (ReferenceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
        }
        else
        {
            if (ViewModel.ReferenceSelected != null)
            {
                SingleReference = App.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
            }
        }

        return Task.CompletedTask;
    }

    private void ReferenceSingles()
    {
        ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
    }
    private async Task RevertChangesReferenceAsync()
    {
        ViewModel.IsInEditReference = false;
        if (ViewModel.IsModifiedReference)
        {
            await RefreshReferenceAsync();
            ViewModel.IsModifiedReference = false;
        }
    }

    private Task RefreshReferenceAsync()
    {
        if (ReferenceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
        }
        else
        {
            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId, ViewModel.RegnumSelected.RegnumId);
            ReferenceSingles();
        }
        return Task.CompletedTask;
    }

    private async void DeleteReference_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteReference();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteReference()
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
                await ViewModel.DeleteReferenceAsync(ViewModel.ReferenceSelected);
                await Refresh();
                ReferenceDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesReferenceAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveReference_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveReference();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveReference()
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
                if (await BeforeSaveReference())
                {
                    return;
                }
                await ViewModel.SaveReferenceAsync(ViewModel.ReferenceSelected);
                await BehindSaveReference(ViewModel.ReferenceSelected);
                await Refresh();
                ViewModel.IsNewReference = false;
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedReference = true;
                await RevertChangesReferenceAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveReference()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(a => a.Info == ViewModel.ReferenceSelected.Info &&
                        a.RegnumId == ViewModel.ReferenceSelected.RegnumId &&
                        a.RefExpertId == ViewModel.ReferenceSelected.RefExpertId &&
                        a.RefSourceId == ViewModel.ReferenceSelected.RefSourceId &&
                        a.RefAuthorId == ViewModel.ReferenceSelected.RefAuthorId)];
        });
        if (ViewModel.IsNewReference)
        {
            if (ViewModel.Tbl90ReferencesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedReference = true;
        return false;
    }

    private async Task BehindSaveReference(Tbl90Reference viewModelReferenceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(refe => refe.RegnumId == viewModelReferenceSelected.RegnumId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.RegnumId == viewModelReferenceSelected.RegnumId)];
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            await Refresh();
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceDataGrid.ItemsSource = null;
        }
        else
        {
            ReferencesExpander.Visibility = Visibility.Visible;
            ReferenceDataGrid.ItemsSource = ViewModel.Tbl90ReferencesList;
            ReferenceDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;
    }

    private void BackReference_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Reference Methods]   

    #region [Reference Expert Methods]

    private async void AddReferenceExpertButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceExpert();
            InfoReferenceExpert.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfoReferenceExpert);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceExpert()
    {
        ViewModel.IsInEditReferenceExpert = true;
        ViewModel.IsNewReferenceExpert = true;

        ReferenceExpertCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceExpertAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 0;
        TabViewItemReferenceExpert.Visibility = Visibility.Visible;
        TabViewItemReferenceSource.IsEnabled = false;
        TabViewItemReferenceAuthor.IsEnabled = false;
    }

    private void CancelEditsReferenceExpertAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceExpert)
        {
            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelReferenceExpertEdits();
        }
    }

    private void CancelReferenceExpertEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.RegnumSelected.RegnumId);
            ReferenceExpertSingles();
        }
        else
        {
            ReferenceExpertDetailExpander.IsExpanded = false;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsNewReferenceExpert = false;
    }

    private void ReferenceExpertSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        //ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        //ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceExpertAsync()
    {
        ViewModel.IsInEditReferenceExpert = false;
        if (ViewModel.IsModifiedReferenceExpert)
        {
            await RefreshReferenceExpertAsync();
            ViewModel.IsModifiedReferenceExpert = false;
        }
    }
    private async Task RefreshReferenceExpertAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.RegnumSelected.RegnumId);

            ReferenceExpertSingles();
        });
    }
    #endregion [Reference Expert Methods]

    #region [Reference Source Methods]

    private async void AddReferenceSourceButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceSource();
            InfoReferenceSource.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfoReferenceSource);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceSource()
    {
        ViewModel.IsInEditReferenceSource = true;
        ViewModel.IsNewReferenceSource = true;

        ReferenceSourceCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceSourceAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 1;
        TabViewItemReferenceSource.Visibility = Visibility.Visible;
        TabViewItemReferenceExpert.IsEnabled = false;
        TabViewItemReferenceAuthor.IsEnabled = false;
    }

    private void CancelEditsReferenceSourceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceSource)
        {
            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelReferenceSourceEdits();
        }
    }
    private void CancelReferenceSourceEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.RegnumSelected.RegnumId);
            ReferenceSourceSingles();
        }
        else
        {
            ReferenceSourceDetailExpander.IsExpanded = false;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsNewReferenceSource = false;
    }

    private void ReferenceSourceSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        // ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        //ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceSourceAsync()
    {
        ViewModel.IsInEditReferenceSource = false;
        if (ViewModel.IsModifiedReferenceSource)
        {
            await RefreshReferenceSourceAsync();
            ViewModel.IsModifiedReferenceSource = false;
        }
    }
    private async Task RefreshReferenceSourceAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.RegnumSelected.RegnumId);

            ReferenceSourceSingles();
        });
    }

    #endregion [Reference Source Methods]

    #region [Reference Author Methods]

    private async void AddReferenceAuthorButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddReferenceAuthor();
            InfoReferenceAuthor.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfoReferenceAuthor);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceAuthor()
    {
        ViewModel.IsInEditReferenceAuthor = true;
        ViewModel.IsNewReferenceAuthor = true;

        ReferenceAuthorCombo.PlaceholderText = StringsRes.SelectPlease;

        await ViewModel.AddReferenceAuthorAsync();

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[0];

        ReferenceDetailExpander.IsExpanded = true;
        ReferenceDetailExpander.Visibility = Visibility.Visible;
        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
        TabViewReference.SelectedIndex = 2;
        TabViewItemReferenceAuthor.Visibility = Visibility.Visible;
        TabViewItemReferenceExpert.IsEnabled = false;
        TabViewItemReferenceSource.IsEnabled = false;
    }

    private void CancelEditsReferenceAuthorAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceAuthor)
        {
            ReferenceDetailExpander.IsExpanded = false;
            ReferenceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelReferenceAuthorEdits();
        }
    }

    private void CancelReferenceAuthorEdits()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.RegnumSelected.RegnumId);
            ReferenceAuthorSingles();
        }
        else
        {
            ReferenceAuthorDetailExpander.IsExpanded = false;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReferenceAuthor = false;
    }

    private void ReferenceAuthorSingles()
    {
        ViewModel.ReferenceSelected.Valid = SingleReference.Valid;
        ViewModel.ReferenceSelected.ValidYear = SingleReference.ValidYear;
        ViewModel.ReferenceSelected.Info = SingleReference.Info;
        ViewModel.ReferenceSelected.Memo = SingleReference.Memo;
        // ViewModel.ReferenceSelected.RefExpertId = SingleReference.RefExpertId;
        // ViewModel.ReferenceSelected.RefSourceId = SingleReference.RefSourceId;
        ViewModel.ReferenceSelected.RefAuthorId = SingleReference.RefAuthorId;
    }
    public async Task RevertChangesReferenceAuthorAsync()
    {
        ViewModel.IsInEditReferenceAuthor = false;
        if (ViewModel.IsModifiedReferenceAuthor)
        {
            await RefreshReferenceAuthorAsync();
            ViewModel.IsModifiedReferenceAuthor = false;
        }
    }
    private async Task RefreshReferenceAuthorAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            SingleReference = App.SingleReferenceRegnum(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.RegnumSelected.RegnumId);

            ReferenceAuthorSingles();
        });
    }

    #endregion [Reference Author Methods]   

    #region [Comment Methods]
    private async void AddComment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddComment();
            InfoComment.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfoComment);
            };
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

        await ViewModel.AddComment();

        if (ViewModel.Tbl93CommentsList != null)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
        }

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
    }

    private void DataGrid_Comment_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl93CommentsList.Sort);
    }

    private void AddNewCommentCanceled(object sender, EventArgs e) => CommentsExpander.IsExpanded = false;

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
            SingleComment = App.SingleCommentRegnum(ViewModel.CommentSelected.CommentId, ViewModel.RegnumSelected.RegnumId);
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

            SingleComment = App.SingleCommentRegnum(ViewModel.CommentSelected.CommentId, ViewModel.RegnumSelected.RegnumId);
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

    private async Task RefreshCommentAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {

            if (CommentDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();   //zeile markieren
            }
            else
            {
                SingleComment = App.SingleCommentRegnum(ViewModel.CommentSelected.CommentId, ViewModel.RegnumSelected.RegnumId);
                CommentSingles();
            }
        });
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
                ViewModel.IsNewComment = false;
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
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
            .Where(a => a.Info == ViewModel.CommentSelected.Info &&
                        a.Memo == ViewModel.CommentSelected.Memo &&
                        a.RegnumId == ViewModel.CommentSelected.RegnumId)];
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
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.RegnumId == viewModelCommentSelected.RegnumId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.RegnumId == viewModelCommentSelected.RegnumId)];
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
            await Refresh();
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDataGrid.ItemsSource = null;
        }
        else
        {
            CommentsExpander.Visibility = Visibility.Visible;
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
            CommentDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;
    }

    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Comment Methods]  

    #region [Phylum Methods]


    private void DataGrid_Phylum_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl06PhylumsList.Sort);
    }

    private async void ViewPhylumHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewPhylumHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewPhylumHyperlink()
    {
        ViewModel.IsInEditPhylum = false;
        ViewModel.IsNewPhylum = false;
        PhylumDetailExpander.IsExpanded = true;
        PhylumDetailExpander.Visibility = Visibility.Visible;

        if (PhylumDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SinglePhylum = App.SinglePhylumRegnum(ViewModel.PhylumSelected.PhylumId,
                    ViewModel.RegnumSelected.RegnumId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsPhylumAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewPhylum)
        {
            PhylumDetailExpander.IsExpanded = false;
            PhylumDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelPhylumEdits();
        }
    }

    private void CancelPhylumEdits()
    {
        var x = (DataGrid)FindName("PhylumDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.PhylumSelected = ViewModel.Tbl06PhylumsList[iNdx];

            SinglePhylum = App.SinglePhylumRegnum(ViewModel.PhylumSelected.PhylumId, ViewModel.RegnumSelected.RegnumId);
            PhylumSingles();
        }
        else
        {
            PhylumDetailExpander.IsExpanded = false;
            PhylumDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditPhylum = false;
        ViewModel.IsNewPhylum = false;
    }

    private void PhylumSingles()
    {
        ViewModel.PhylumSelected.PhylumName = SinglePhylum.PhylumName;
        ViewModel.PhylumSelected.RegnumId = SinglePhylum.RegnumId;
        ViewModel.PhylumSelected.Valid = SinglePhylum.Valid;
        ViewModel.PhylumSelected.ValidYear = SinglePhylum.ValidYear;
        ViewModel.PhylumSelected.Author = SinglePhylum.Author;
        ViewModel.PhylumSelected.AuthorYear = SinglePhylum.AuthorYear;
        ViewModel.PhylumSelected.Info = SinglePhylum.Info;
        ViewModel.PhylumSelected.Synonym = SinglePhylum.Synonym;
        ViewModel.PhylumSelected.EngName = SinglePhylum.EngName;
        ViewModel.PhylumSelected.GerName = SinglePhylum.GerName;
        ViewModel.PhylumSelected.FraName = SinglePhylum.FraName;
        ViewModel.PhylumSelected.PorName = SinglePhylum.PorName;
        ViewModel.PhylumSelected.Memo = SinglePhylum.Memo;
    }

    private async Task RevertChangesPhylumAsync()
    {
        ViewModel.IsInEditPhylum = false;
        if (ViewModel.IsModifiedPhylum)
        {
            await RefreshPhylumAsync();
            ViewModel.IsModifiedPhylum = false;
        }
    }
    private async Task RefreshPhylumAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (PhylumDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SinglePhylum = App.SinglePhylumRegnum(ViewModel.PhylumSelected.PhylumId, ViewModel.RegnumSelected.RegnumId);
                PhylumSingles();
            }
        });
    }

    private async void SavePhylum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SavePhylum();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SavePhylum()
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
                if (await BeforeSavePhylum())
                {
                    return;
                }
                await ViewModel.SavePhylumAsync(ViewModel.PhylumSelected);
                await BehindSavePhylum(ViewModel.PhylumSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedPhylum = true;
                await RevertChangesPhylumAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSavePhylum()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl06PhylumsList ??= [];
            ViewModel.Tbl06PhylumsList = [.. Context.Tbl06Phylums
            .Where(a => a.PhylumName == ViewModel.PhylumSelected.PhylumName &&
                        a.RegnumId == ViewModel.PhylumSelected.RegnumId)];
        });
        if (ViewModel.IsNewPhylum)
        {
            if (ViewModel.Tbl06PhylumsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedPhylum = true;
        return false;
    }

    private async Task BehindSavePhylum(Tbl06Phylum viewModelPhylumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl06PhylumsList ??= [];
            ViewModel.Tbl06PhylumsList = [.. Context.Tbl06Phylums
                .Where(com => com.RegnumId == viewModelPhylumSelected.RegnumId)];
        });
        ViewModel.RefreshPhylumItems();

        if (ViewModel.Tbl06PhylumsList.Count == 0)
        {
            PhylumsExpander.Visibility = Visibility.Collapsed;
            PhylumDataGrid.ItemsSource = null;
        }
        else
        {
            PhylumsExpander.Visibility = Visibility.Visible;
            PhylumDataGrid.ItemsSource = ViewModel.Tbl06PhylumsList;
            PhylumDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.PhylumDataSetCount = ViewModel.Tbl06PhylumsList.Count;
    }

    private void PhylumName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackPhylum_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Phylum Methods] 

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
            DetailPanelRegnum.Visibility = Visibility.Visible;
            RegnumDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("RegnumDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl03RegnumsList!.Count)
            {
                ViewModel.RegnumItems.Clear();
                foreach (var item in ViewModel.Tbl03RegnumsList)
                {
                    ViewModel.RegnumItems.Add(item);
                }

                ViewModel.RegnumSelected = ViewModel.Tbl03RegnumsList[iNdx];
            }
            RegnumDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl03RegnumsList.Count == 0)
                {
                    ViewModel.Tbl03RegnumsList ??= [];
                    ViewModel.Tbl03RegnumsList = [.. Context.Tbl03Regnums
                            .Where(a => a.RegnumId == ViewModel.RegnumSelected.RegnumId)];
                }
                else
                {
                    ViewModel.Tbl03RegnumsList ??= [];
                    ViewModel.Tbl03RegnumsList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl03Regnums
                           .Where(a => a.RegnumId == id)]
                       : [.. Context.Tbl03Regnums
                           .Where(a => _search != null && a.RegnumName.StartsWith(_search))
                         .OrderBy(a => a.RegnumName)
                .ThenBy(a => a.Subregnum)];
                }
            });

            RegnumDataGrid.ItemsSource = ViewModel.Tbl03RegnumsList;

            if (ViewModel.IsDeletingRegnum)
            {
                if (iNdx == 0 && ViewModel.Tbl03RegnumsList.Count == 0)
                {
                    ViewModel.Tbl03RegnumsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.RegnumSelected = ViewModel.Tbl03RegnumsList[iNdx];
            }
            if (ViewModel.Tbl03RegnumsList is { Count: 0 })
            {
                ViewModel.Tbl03RegnumsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl06PhylumsList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        RegnumDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingRegnum = false;
    }

    private void BackDetailExpanders()
    {
        RegnumDetailExpander.IsExpanded = false;
        RegnumDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        PhylumDetailExpander.IsExpanded = false;
        PhylumDetailExpander.Visibility = Visibility.Collapsed;

    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteRegnum.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteRegnum.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteRegnum.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteRegnum.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

