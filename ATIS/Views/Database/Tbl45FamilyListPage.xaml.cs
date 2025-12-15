using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  Tbl45FamilyListPage.xaml.cs Skriptdatum:  18.10.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl45FamilyListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl45FamilyViewModel ViewModel { get; }
    private Tbl45Family SingleFamily { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl48Subfamily SingleSubfamily { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl45FamilyListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl45FamilyViewModel>();
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

    #region [Family Methods]

    private async void GetFamiliesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl45FamiliesList?.Clear();

            ViewModel.FamilyItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl45FamiliesList?.Clear();

                FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl48SubfamiliesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubfamiliesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingFamily = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl45FamiliesList ??= [];
                ViewModel.Tbl45FamiliesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl45Families
                .Where(a => a.FamilyId == id)]
                : [.. Context.Tbl45Families
                .Where(a => _search != null && a.FamilyName.StartsWith(_search))
                .OrderBy(a => a.FamilyName)];
            });

            FamilyDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SubfamiliesExpander.Visibility = Visibility.Collapsed;
            SubfamilyDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.FamilyDataSetCount = ViewModel.Tbl45FamiliesList.Count;

            if (ViewModel.Tbl45FamiliesList is { Count: 0 })
            {
                ViewModel.Tbl45FamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl48SubfamiliesList?.Clear();

                AppBarButtonDeleteFamily.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelFamily.Visibility = Visibility.Visible;
                FamilyDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubfamiliesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl45FamilyListPage));
            }
            else
            {
                FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
                ViewModel.FamilySelected = ViewModel.Tbl45FamiliesList[0];
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

    private async void FamilyDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await FamilyDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task FamilyDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.FamilySelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.FamilySelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.FamilyId == ViewModel.FamilySelected.FamilyId)];
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
            .Where(com => com.FamilyId == ViewModel.FamilySelected.FamilyId)];
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
            //--------------------------Subfamily-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl48SubfamiliesList ??= [];
                ViewModel.Tbl48SubfamiliesList = [.. Context.Tbl48Subfamilies
            .Where(com => com.FamilyId == ViewModel.FamilySelected.FamilyId)];
            });
            ViewModel.SubfamilyDataSetCount = ViewModel.Tbl48SubfamiliesList.Count;
            ViewModel.RefreshSubfamilyItems();

            if (ViewModel.Tbl48SubfamiliesList.Count == 0)
            {
                SubfamiliesExpander.Visibility = Visibility.Collapsed;
                SubfamilyDataGrid.ItemsSource = null;
            }
            else
            {
                SubfamiliesExpander.Visibility = Visibility.Visible;
                SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddFamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddFamily();
            FamilyName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(FamilyName);
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

    private async Task AddFamily()
    {
        ViewModel.IsInEditFamily = true;
        ViewModel.IsNewFamily = true;

        await ViewModel.AddFamilyAsync();

        if (ViewModel.Tbl45FamiliesList != null)
        {
            ViewModel.FamilySelected = ViewModel.Tbl45FamiliesList[0];
        }

        FamilyDetailExpander.IsExpanded = true;
        FamilyDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void FamilySearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        FamilySearchBox.Visibility = Visibility.Visible;
        FamilySearchBox.Focus(FocusState.Programmatic);
        FamilySearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void FamilyDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl45FamiliesList.Sort);
    }

    private void AddNewFamilyCanceled(object sender, EventArgs e) => FamilyDetailExpander.IsExpanded = false;

    private async void ViewFamilyHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewFamilyHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewFamilyHyperlink()
    {
        ViewModel.IsInEditFamily = false;
        ViewModel.IsNewFamily = false;
        FamilyDetailExpander.IsExpanded = true;
        FamilyDetailExpander.Visibility = Visibility.Visible;

        if (FamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleFamily = App.SingleFamily(ViewModel.FamilySelected.FamilyId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsFamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewFamily)
        {
            FamilyDetailExpander.IsExpanded = false;
            FamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelFamilyEdits();
        }
    }

    private void CancelFamilyEdits()
    {
        var x = (DataGrid)FindName("FamilyDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.FamilySelected = ViewModel.Tbl45FamiliesList[iNdx];
            SingleFamily = App.SingleFamily(ViewModel.FamilySelected.FamilyId);
            FamilySingles();
        }
        else
        {
            FamilyDetailExpander.IsExpanded = false;
            FamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditFamily = false;
        ViewModel.IsNewFamily = false;
    }

    private void FamilySingles()
    {
        ViewModel.FamilySelected.FamilyName = SingleFamily.FamilyName;
        ViewModel.FamilySelected.SuperfamilyId = SingleFamily.SuperfamilyId;
        ViewModel.FamilySelected.Valid = SingleFamily.Valid;
        ViewModel.FamilySelected.ValidYear = SingleFamily.ValidYear;
        ViewModel.FamilySelected.Author = SingleFamily.Author;
        ViewModel.FamilySelected.AuthorYear = SingleFamily.AuthorYear;
        ViewModel.FamilySelected.Info = SingleFamily.Info;
        ViewModel.FamilySelected.Synonym = SingleFamily.Synonym;
        ViewModel.FamilySelected.EngName = SingleFamily.EngName;
        ViewModel.FamilySelected.GerName = SingleFamily.GerName;
        ViewModel.FamilySelected.FraName = SingleFamily.FraName;
        ViewModel.FamilySelected.PorName = SingleFamily.PorName;
        ViewModel.FamilySelected.Memo = SingleFamily.Memo;
    }
    private async Task RevertChangesFamilyAsync()
    {
        ViewModel.IsInEditFamily = false;
        if (ViewModel.IsModifiedFamily)
        {
            await RefreshFamilyAsync();
            ViewModel.IsModifiedFamily = false;
        }
    }
    private Task RefreshFamilyAsync()
    {
        if (FamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.FamilySelected.FamilyId == 0)
            {
                return Task.CompletedTask;
            }
            SingleFamily = App.SingleFamily(ViewModel.FamilySelected.FamilyId);
            FamilySingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteFamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteFamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteFamily()
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
                await ViewModel.DeleteFamilyAsync(ViewModel.FamilySelected);
                await Refresh();
                FamilyDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesFamilyAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveFamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveFamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveFamily()
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
                if (await BeforeSaveFamily())
                {
                    return;
                }
                await ViewModel.SaveFamilyAsync(ViewModel.FamilySelected);
                await BehindSaveFamily(ViewModel.FamilySelected);
                await Refresh();
                ViewModel.IsNewFamily = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedFamily = true;
                await RevertChangesFamilyAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveFamily()
    {
        _search = FamilySearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl45FamiliesList ??= [];
            ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
                .Where(a => a.FamilyName == ViewModel.FamilySelected.FamilyName &&
                        a.SuperfamilyId == ViewModel.FamilySelected.SuperfamilyId)];
        });
        if (ViewModel.IsNewFamily)
        {
            if (ViewModel.Tbl45FamiliesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedFamily = true;
        return false;
    }

    private async Task BehindSaveFamily(Tbl45Family viewModelFamilySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl45FamiliesList ??= [];
            if (FamilySearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl45FamiliesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl45Families
                    .Where(a => a.FamilyId == id)]
                    : [.. Context.Tbl45Families
                    .Where(a => _search != null && a.FamilyName.StartsWith(_search))
                    .OrderBy(a => a.FamilyName)];
            }
        });

        ViewModel.RefreshFamilyItems();

        if (ViewModel.Tbl45FamiliesList.Count == 0)
        {
            ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
                .Where(a => a.FamilyId == viewModelFamilySelected.FamilyId)];
            FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
            await Refresh();
            FamilyDataGrid.SelectedIndex = 0;
        }
        else
        {
            FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
            var index = ViewModel.FamilyItems.ToList().FindIndex(item => item.FamilyId == viewModelFamilySelected.FamilyId);
            FamilyDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.FamilyDataSetCount = ViewModel.Tbl45FamiliesList.Count;
    }

    private void FamilyName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackFamily_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Family Methods]                    

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
            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.FamilySelected.FamilyId);
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
                        a.FamilyId == ViewModel.ReferenceSelected.FamilyId &&
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
                .Where(refe => refe.FamilyId == viewModelReferenceSelected.FamilyId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.FamilyId == viewModelReferenceSelected.FamilyId)];
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

            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.FamilySelected.FamilyId);
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
            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.FamilySelected.FamilyId);

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

            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.FamilySelected.FamilyId);
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
            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.FamilySelected.FamilyId);

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

            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.FamilySelected.FamilyId);
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
            SingleReference = App.SingleReferenceFamily(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.FamilySelected.FamilyId);

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
            SingleComment = App.SingleCommentFamily(ViewModel.CommentSelected.CommentId, ViewModel.FamilySelected.FamilyId);
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

            SingleComment = App.SingleCommentFamily(ViewModel.CommentSelected.CommentId, ViewModel.FamilySelected.FamilyId);
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
                SingleComment = App.SingleCommentFamily(ViewModel.CommentSelected.CommentId, ViewModel.FamilySelected.FamilyId);
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
                        a.FamilyId == ViewModel.CommentSelected.FamilyId)];
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
                .Where(com => com.FamilyId == viewModelCommentSelected.FamilyId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.FamilyId == viewModelCommentSelected.FamilyId)];
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

    #region [Subfamily Methods]

    private void DataGrid_Subfamily_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl48SubfamiliesList.Sort);
    }

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
            SingleSubfamily = App.SingleSubfamilyFamily(ViewModel.SubfamilySelected.SubfamilyId,
                    ViewModel.FamilySelected.FamilyId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSubfamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubfamily)
        {
            SubfamilyDetailExpander.IsExpanded = false;
            SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
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
        if (iNdx is not -1)
        {
            ViewModel.SubfamilySelected = ViewModel.Tbl48SubfamiliesList[iNdx];

            SingleSubfamily = App.SingleSubfamilyFamily(ViewModel.SubfamilySelected.SubfamilyId, ViewModel.FamilySelected.FamilyId);
            SubfamilySingles();
        }
        else
        {
            SubfamilyDetailExpander.IsExpanded = false;
            SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSubfamily = false;
        ViewModel.IsNewSubfamily = false;
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
    private async Task RefreshSubfamilyAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SubfamilyDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSubfamily = App.SingleSubfamilyFamily(ViewModel.SubfamilySelected.SubfamilyId, ViewModel.FamilySelected.FamilyId);
                SubfamilySingles();
            }
        });
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
                await BehindSaveSubfamily(ViewModel.SubfamilySelected);
                await Refresh();
                ViewModel.IsNewSubfamily = false;
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

    private async Task BehindSaveSubfamily(Tbl48Subfamily viewModelSubfamilySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl48SubfamiliesList ??= [];
            ViewModel.Tbl48SubfamiliesList = [.. Context.Tbl48Subfamilies
                .Where(com => com.FamilyId == viewModelSubfamilySelected.FamilyId)];
        });
        ViewModel.RefreshSubfamilyItems();

        if (ViewModel.Tbl48SubfamiliesList.Count == 0)
        {
            ViewModel.Tbl48SubfamiliesList = [.. Context.Tbl48Subfamilies
                .Where(com => com.FamilyId == viewModelSubfamilySelected.FamilyId)];
            SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
            await Refresh();
            SubfamiliesExpander.Visibility = Visibility.Collapsed;
            SubfamilyDataGrid.ItemsSource = null;
        }
        else
        {
            SubfamiliesExpander.Visibility = Visibility.Visible;
            SubfamilyDataGrid.ItemsSource = ViewModel.Tbl48SubfamiliesList;
            SubfamilyDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.SubfamilyDataSetCount = ViewModel.Tbl48SubfamiliesList.Count;
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
            //  DetailPanelFamily.Visibility = Visibility.Visible;
            //  FamilyDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("FamilyDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl45FamiliesList!.Count)
            {
                ViewModel.FamilyItems.Clear();
                foreach (var item in ViewModel.Tbl45FamiliesList)
                {
                    ViewModel.FamilyItems.Add(item);
                }

                ViewModel.FamilySelected = ViewModel.Tbl45FamiliesList[iNdx];
            }
            FamilyDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl45FamiliesList.Count == 0)
                {
                    ViewModel.Tbl45FamiliesList ??= [];
                    ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
                            .Where(a => a.FamilyId == ViewModel.FamilySelected.FamilyId)];
                }
                else
                {
                    ViewModel.Tbl45FamiliesList ??= [];
                    ViewModel.Tbl45FamiliesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl45Families
                           .Where(a => a.FamilyId == id)]
                       : [.. Context.Tbl45Families
                           .Where(a => _search != null && a.FamilyName.StartsWith(_search))
                         .OrderBy(a => a.FamilyName)];
                }
            });

            FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;

            if (ViewModel.IsDeletingFamily)
            {
                if (iNdx == 0 && ViewModel.Tbl45FamiliesList.Count == 0)
                {
                    ViewModel.Tbl45FamiliesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.FamilySelected = ViewModel.Tbl45FamiliesList[iNdx];
            }
            if (ViewModel.Tbl45FamiliesList is { Count: 0 })
            {
                ViewModel.Tbl45FamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl48SubfamiliesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        FamilyDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingFamily = false;
    }

    private void BackDetailExpanders()
    {
        FamilyDetailExpander.IsExpanded = false;
        FamilyDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SubfamilyDetailExpander.IsExpanded = false;
        SubfamilyDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteFamily.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteFamily.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteFamily.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteFamily.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

