
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

//  Tbl48SubfamilyListPage.xaml.cs Skriptdatum:  15.06.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl48SubfamilyListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl48SubfamilyViewModel ViewModel { get; }
    private Tbl48Subfamily SingleSubfamily { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl51Infrafamily SingleInfrafamily { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl48SubfamilyListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl48SubfamilyViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Subfamily Methods]

    private async void GetSubfamiliesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl48SubfamiliesList?.Clear();

            ViewModel.SubfamilyItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl48SubfamiliesList?.Clear();

                SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl51InfrafamiliesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfrafamiliesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSubfamily = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl48SubfamiliesList ??= [];
                ViewModel.Tbl48SubfamiliesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl48Subfamilies
                .Where(a => a.SubfamilyId == id)]
                : [.. Context.Tbl48Subfamilies
                .Where(a => _search != null && a.SubfamilyName.StartsWith(_search))
                .OrderBy(a => a.SubfamilyName)];
            });

            SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            InfrafamiliesExpander.Visibility = Visibility.Collapsed;
            InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SubfamilyDataSetCount = ViewModel.Tbl48SubfamiliesList.Count;

            if (ViewModel.Tbl48SubfamiliesList is { Count: 0 })
            {
                ViewModel.Tbl48SubfamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl51InfrafamiliesList?.Clear();

                AppBarButtonDeleteSubfamily.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddInfrafamily.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSubfamily.Visibility = Visibility.Visible;
                SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfrafamiliesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl48SubfamilyListPage));
            }
            else
            {
                SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
                ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void SubfamilyDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SubfamilyDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SubfamilyDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SubfamilySelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        //----------------------------------Reference--------------------------------       
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
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
            .Where(com => com.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
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
        //--------------------------Infrafamily-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl51InfrafamiliesList ??= [];
            ViewModel.Tbl51InfrafamiliesList = [.. Context.Tbl51Infrafamilies
            .AsNoTracking()
            .Where(com => com.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
        });
        ViewModel.InfrafamilyDataSetCount = ViewModel.Tbl51InfrafamiliesList.Count;
        ViewModel.RefreshInfrafamilyItems();

        if (ViewModel.Tbl51InfrafamiliesList.Count == 0)
        {
            InfrafamiliesExpander.Visibility = Visibility.Collapsed;
            InfrafamilyDataGrid.ItemsSource = null;
        }
        else
        {
            InfrafamiliesExpander.Visibility = Visibility.Visible;
            InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSubfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubfamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSubfamily()
    {
        SubfamilyName.Loaded += (s, e) =>
        {
            SubfamilyName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSubfamily = true;
        ViewModel.IsNewSubfamily = true;

        await ViewModel.AddSubfamilyAsync();

        if (ViewModel.Tbl48SubfamiliesList != null)
        {
            ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[0];
        }

        SubfamilyDetailExpander.IsExpanded = true;
        SubfamilyDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewSubfamily = false;
        SubfamilyName.PlaceholderText = StringsRes.EnterText;
    }

    private void SubfamilySearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SubfamilySearchBox.Visibility = Visibility.Visible;
        SubfamilySearchBox.Focus(FocusState.Programmatic);
        SubfamilySearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SubfamilyDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl48SubfamiliesList.Sort);
    }

    private void AddNewSubfamilyCanceled(object sender, EventArgs e) => SubfamilyDetailExpander.IsExpanded = false;

    private async void ViewSubfamilyHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubfamilyHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubfamilyHyperlink()
    {
        ViewModel.IsInEditSubfamily = false;
        ViewModel.IsNewSubfamily = false;
        SubfamilyDetailExpander.IsExpanded = true;
        SubfamilyDetailExpander.Visibility = Visibility.Visible;

        if (SubfamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubfamily = App.SingleSubfamily(ViewModel.SubfamilySelected.SubfamilyId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSubfamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubfamily)
        {
            SubfamilyDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelSubfamilyEdits();
        }
    }

    private void CancelSubfamilyEdits()
    {
        var x = (DataGrid)FindName("SubfamilyDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditSubfamily = false;
            ViewModel.IsNewSubfamily = false;
            SubfamilyDetailExpander.IsExpanded = false;
            SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[iNdx];

        SingleSubfamily = App.SingleSubfamily(ViewModel.SubfamilySelected.SubfamilyId);
        SubfamilySingles();

        ViewModel.IsInEditSubfamily = false;
        ViewModel.IsNewSubfamily = false;
        SubfamilyDetailExpander.IsExpanded = true;
        SubfamilyDetailExpander.Visibility = Visibility.Visible;
    }

    private void SubfamilySingles()
    {
        ViewModel.SubfamilySelected.SubfamilyName = SingleSubfamily.SubfamilyName;
        ViewModel.SubfamilySelected.FamilyId = SingleSubfamily.FamilyId;
        ViewModel.SubfamilySelected.Valid = SingleSubfamily.Valid;
        ViewModel.SubfamilySelected.ValidYear = SingleSubfamily.ValidYear;
        ViewModel.SubfamilySelected.Author = SingleSubfamily.Author;
        ViewModel.SubfamilySelected.AuthorYear = SingleSubfamily.AuthorYear;
        ViewModel.SubfamilySelected.Info = SingleSubfamily.Info;
        ViewModel.SubfamilySelected.Synonym = SingleSubfamily.Synonym;
        ViewModel.SubfamilySelected.EngName = SingleSubfamily.EngName;
        ViewModel.SubfamilySelected.GerName = SingleSubfamily.GerName;
        ViewModel.SubfamilySelected.FraName = SingleSubfamily.FraName;
        ViewModel.SubfamilySelected.PorName = SingleSubfamily.PorName;
        ViewModel.SubfamilySelected.Memo = SingleSubfamily.Memo;
    }
    private async Task RevertChangesSubfamilyAsync()
    {
        ViewModel.IsInEditSubfamily = false;
        if (ViewModel.IsModifiedSubfamily)
        {
            await RefreshSubfamilyAsync();
            ViewModel.IsModifiedSubfamily = false;
        }
    }
    private Task RefreshSubfamilyAsync()
    {
        if (SubfamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubfamily = App.SingleSubfamily(ViewModel.SubfamilySelected.SubfamilyId);
            SubfamilySingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSubfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSubfamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSubfamily()
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
                await ViewModel.DeleteSubfamilyAsync(ViewModel.SubfamilySelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSubfamilyAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSubfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubfamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubfamily()
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
                if (await BeforeSaveSubfamily())
                {
                    return;
                }
                await ViewModel.SaveSubfamilyAsync(ViewModel.SubfamilySelected);
                await BehindSaveSubfamily();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubfamily = true;
                await RevertChangesSubfamilyAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubfamily()
    {
        _search = SubfamilySearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl48SubfamiliesList ??= [];
            ViewModel.Tbl48SubfamiliesList = [.. Context.Tbl48Subfamilies
                .Where(a => a.SubfamilyName == ViewModel.SubfamilySelected.SubfamilyName &&
                        a.FamilyId == ViewModel.SubfamilySelected.FamilyId)];
        });
        if (ViewModel.IsNewSubfamily)
        {
            if (ViewModel.Tbl48SubfamiliesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubfamily = true;
        return false;
    }

    private async Task BehindSaveSubfamily()
    {
        var x = (DataGrid)FindName("SubfamilyDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl48SubfamiliesList ??= [];
            ViewModel.Tbl48SubfamiliesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl48Subfamilies
                    .Where(a => a.SubfamilyId == id)]
                : [.. Context.Tbl48Subfamilies
                    .Where(a => _search != null && a.SubfamilyName.StartsWith(_search))
                    .OrderBy(a => a.SubfamilyName)];
        });

        ViewModel.RefreshSubfamilyItems();

        if (ViewModel.Tbl48SubfamiliesList.Count == 0)
        {
            SubfamilyDataGrid.ItemsSource = null;
        }
        else
        {
            SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
        }
        SubfamilyDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void SubfamilyName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubfamily_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subfamily Methods]                    

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
            SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);
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
                await BehindSaveReference();
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
                        a.SubfamilyId == ViewModel.ReferenceSelected.SubfamilyId &&
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

    private async Task BehindSaveReference()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(refe => refe.FamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
        });
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
        ReferenceDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceExpert()
    {
        InfoReferenceExpert.Loaded += (s, e) =>
        {
            InfoReferenceExpert.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceExpertAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceExpert)
        {
            ReferenceExpertDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceExpert = false;
            ViewModel.IsNewReferenceExpert = false;
            ReferenceExpertDetailExpander.IsExpanded = false;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

        ReferenceExpertSingles();

        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsNewReferenceExpert = false;
        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceSource()
    {
        InfoReferenceSource.Loaded += (s, e) =>
        {
            InfoReferenceSource.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceSourceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceSource)
        {
            ReferenceSourceDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceSource = false;
            ViewModel.IsNewReferenceSource = false;
            ReferenceSourceDetailExpander.IsExpanded = false;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

        ReferenceSourceSingles();

        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsNewReferenceSource = false;
        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceAuthor()
    {
        InfoReferenceAuthor.Loaded += (s, e) =>
        {
            InfoReferenceAuthor.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceAuthorAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceAuthor)
        {
            ReferenceAuthorDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceAuthor = false;
            ViewModel.IsNewReferenceAuthor = false;
            ReferenceAuthorDetailExpander.IsExpanded = false;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

        ReferenceAuthorSingles();

        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReferenceAuthor = false;
        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSubfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubfamilySelected.SubfamilyId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddComment()
    {
        InfoComment.Loaded += (s, e) =>
        {
            InfoComment.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditComment = true;
        ViewModel.IsNewComment = true;

        await ViewModel.AddComment();

        if (ViewModel.Tbl93CommentsList != null)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
        }

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewComment = false;
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
            SingleComment = App.SingleCommentSubfamily(ViewModel.CommentSelected.CommentId, ViewModel.SubfamilySelected.SubfamilyId);
        }
        return Task.CompletedTask;
    }

    private void CancelEditsCommentAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewComment)
        {
            CommentDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditComment = false;
            ViewModel.IsNewComment = false;
            CommentDetailExpander.IsExpanded = false;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];

        SingleComment = App.SingleCommentSubfamily(ViewModel.CommentSelected.CommentId, ViewModel.SubfamilySelected.SubfamilyId);
        CommentSingles();

        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
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
                SingleComment = App.SingleCommentSubfamily(ViewModel.CommentSelected.CommentId, ViewModel.SubfamilySelected.SubfamilyId);
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
                await BehindSaveComment();
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
                        a.SubfamilyId == ViewModel.CommentSelected.SubfamilyId)];
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

    private async Task BehindSaveComment()
    {
        var x = (DataGrid)FindName("CommentDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .AsNoTracking()
                .Where(com => com.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
        });
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
        CommentDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Comment Methods]  

    #region [Infrafamily Methods]

    private async void AddInfrafamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddInfrafamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddInfrafamily()
    {
        InfrafamilyName.Loaded += (s, e) =>
        {
            InfrafamilyName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditInfrafamily = true;
        ViewModel.IsNewInfrafamily = true;

        await ViewModel.AddInfrafamilyAsync();

        if (ViewModel.Tbl51InfrafamiliesList != null)
        {
            ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[0];
        }

        InfrafamilyDetailExpander.IsExpanded = true;
        InfrafamilyDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewInfrafamily = false;
    }

    private void DataGrid_Infrafamily_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl51InfrafamiliesList.Sort);
    }

    private void AddNewInfrafamilyCanceled(object sender, EventArgs e) => InfrafamiliesExpander.IsExpanded = false;

    private async void ViewInfrafamilyHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewInfrafamilyHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewInfrafamilyHyperlink()
    {
        ViewModel.IsInEditInfrafamily = false;
        ViewModel.IsNewInfrafamily = false;
        InfrafamilyDetailExpander.IsExpanded = true;
        InfrafamilyDetailExpander.Visibility = Visibility.Visible;

        if (InfrafamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleInfrafamily = App.SingleInfrafamilySubfamily(ViewModel.InfrafamilySelected.InfrafamilyId,
                    ViewModel.SubfamilySelected.SubfamilyId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsInfrafamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewInfrafamily)
        {
            InfrafamilyDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelInfrafamilyEdits();
        }
    }

    private void CancelInfrafamilyEdits()
    {
        var x = (DataGrid)FindName("InfrafamilyDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditInfrafamily = false;
            ViewModel.IsNewInfrafamily = false;
            InfrafamilyDetailExpander.IsExpanded = false;
            InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[iNdx];

        SingleInfrafamily = App.SingleInfrafamilySubfamily(ViewModel.InfrafamilySelected.InfrafamilyId, ViewModel.SubfamilySelected.SubfamilyId);
        InfrafamilySingles();

        ViewModel.IsInEditInfrafamily = false;
        ViewModel.IsNewInfrafamily = false;
        InfrafamilyDetailExpander.IsExpanded = true;
        InfrafamilyDetailExpander.Visibility = Visibility.Visible;
    }

    private void InfrafamilySingles()
    {
        ViewModel.InfrafamilySelected.InfrafamilyName = SingleInfrafamily.InfrafamilyName;
        ViewModel.InfrafamilySelected.SubfamilyId = SingleInfrafamily.SubfamilyId;
        ViewModel.InfrafamilySelected.Valid = SingleInfrafamily.Valid;
        ViewModel.InfrafamilySelected.ValidYear = SingleInfrafamily.ValidYear;
        ViewModel.InfrafamilySelected.Author = SingleInfrafamily.Author;
        ViewModel.InfrafamilySelected.AuthorYear = SingleInfrafamily.AuthorYear;
        ViewModel.InfrafamilySelected.Info = SingleInfrafamily.Info;
        ViewModel.InfrafamilySelected.Synonym = SingleInfrafamily.Synonym;
        ViewModel.InfrafamilySelected.EngName = SingleInfrafamily.EngName;
        ViewModel.InfrafamilySelected.GerName = SingleInfrafamily.GerName;
        ViewModel.InfrafamilySelected.FraName = SingleInfrafamily.FraName;
        ViewModel.InfrafamilySelected.PorName = SingleInfrafamily.PorName;
        ViewModel.InfrafamilySelected.Memo = SingleInfrafamily.Memo;
    }

    private async Task RevertChangesInfrafamilyAsync()
    {
        ViewModel.IsInEditInfrafamily = false;
        if (ViewModel.IsModifiedInfrafamily)
        {
            await RefreshInfrafamilyAsync();
            ViewModel.IsModifiedInfrafamily = false;
        }
    }
    private async Task RefreshInfrafamilyAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (InfrafamilyDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleInfrafamily = App.SingleInfrafamilySubfamily(ViewModel.InfrafamilySelected.InfrafamilyId, ViewModel.SubfamilySelected.SubfamilyId);
                InfrafamilySingles();
            }
        });
    }

    private async void SaveInfrafamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveInfrafamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveInfrafamily()
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
                if (await BeforeSaveInfrafamily())
                {
                    return;
                }
                await ViewModel.SaveInfrafamilyAsync(ViewModel.InfrafamilySelected);
                await BehindSaveInfrafamily();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfrafamily = true;
                await RevertChangesInfrafamilyAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfrafamily()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl51InfrafamiliesList ??= [];
            ViewModel.Tbl51InfrafamiliesList = [.. Context.Tbl51Infrafamilies
            .Where(a => a.InfrafamilyName == ViewModel.InfrafamilySelected.InfrafamilyName &&
                        a.SubfamilyId == ViewModel.InfrafamilySelected.SubfamilyId)];
        });
        if (ViewModel.IsNewInfrafamily)
        {
            if (ViewModel.Tbl51InfrafamiliesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedInfrafamily = true;
        return false;
    }

    private async Task BehindSaveInfrafamily()
    {
        var x = (DataGrid)FindName("InfrafamilyDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl51InfrafamiliesList ??= [];
            ViewModel.Tbl51InfrafamiliesList = [.. Context.Tbl51Infrafamilies
                .AsNoTracking()
                .Where(com => com.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
        });
        ViewModel.RefreshInfrafamilyItems();

        if (ViewModel.Tbl51InfrafamiliesList.Count == 0)
        {
            InfrafamiliesExpander.Visibility = Visibility.Collapsed;
            InfrafamilyDataGrid.ItemsSource = null;
        }
        else
        {
            InfrafamiliesExpander.Visibility = Visibility.Visible;
            InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
        }
        InfrafamilyDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void InfrafamilyName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackInfrafamily_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Infrafamily Methods] 

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
            //  DetailPanelSubfamily.Visibility = Visibility.Visible;
            //  SubfamilyDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SubfamilyDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl48SubfamiliesList!.Count)
            {
                ViewModel.SubfamilyItems.Clear();
                foreach (var item in ViewModel.Tbl48SubfamiliesList)
                {
                    ViewModel.SubfamilyItems.Add(item);
                }

                ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[iNdx];
            }
            SubfamilyDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl48SubfamiliesList.Count == 0)
                {
                    ViewModel.Tbl48SubfamiliesList ??= [];
                    ViewModel.Tbl48SubfamiliesList = [.. Context.Tbl48Subfamilies
                            .Where(a => a.SubfamilyId == ViewModel.SubfamilySelected.SubfamilyId)];
                }
                else
                {
                    ViewModel.Tbl48SubfamiliesList ??= [];
                    ViewModel.Tbl48SubfamiliesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl48Subfamilies
                           .Where(a => a.SubfamilyId == id)]
                       : [.. Context.Tbl48Subfamilies
                           .Where(a => _search != null && a.SubfamilyName.StartsWith(_search))
                         .OrderBy(a => a.SubfamilyName)];
                }
            });

            SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;

            if (ViewModel.IsDeletingSubfamily)
            {
                if (iNdx == 0 && ViewModel.Tbl48SubfamiliesList.Count == 0)
                {
                    ViewModel.Tbl48SubfamiliesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[iNdx];
            }
            if (ViewModel.Tbl48SubfamiliesList is { Count: 0 })
            {
                ViewModel.Tbl48SubfamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl51InfrafamiliesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SubfamilyDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSubfamily = false;
    }

    private void BackDetailExpanders()
    {
        SubfamilyDetailExpander.IsExpanded = false;
        SubfamilyDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        InfrafamilyDetailExpander.IsExpanded = false;
        InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

