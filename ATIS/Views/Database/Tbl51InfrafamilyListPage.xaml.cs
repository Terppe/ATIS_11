using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info. 

//  Tbl51InfrafamilyListPage.xaml.cs Skriptdatum:  21.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl51InfrafamilyListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl51InfrafamilyViewModel ViewModel { get; set; }
    private Tbl51Infrafamily SingleInfrafamily { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl54Supertribus SingleSupertribus { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl51InfrafamilyListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl51InfrafamilyViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }

    }

    #endregion [Constructor]   

    #region [Infrafamily Methods]

    private async void GetInfrafamiliesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl51InfrafamiliesList?.Clear();

            ViewModel.InfrafamilyItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl51InfrafamiliesList?.Clear();

                InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl54SupertribussesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SupertribussesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingInfrafamily = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl51InfrafamiliesList ??= [];
                ViewModel.Tbl51InfrafamiliesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl51Infrafamilies
                .Where(a => a.InfrafamilyId == id)]
                : [.. AppData.Data.Tbl51Infrafamilies
                .Where(a => _search != null && a.InfrafamilyName.StartsWith(_search))
                .OrderBy(a => a.InfrafamilyName)];
            });

            InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SupertribussesExpander.Visibility = Visibility.Collapsed;
            SupertribusDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.InfrafamilyDataSetCount = ViewModel.Tbl51InfrafamiliesList.Count;

            if (ViewModel.Tbl51InfrafamiliesList is { Count: 0 })
            {
                ViewModel.Tbl51InfrafamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl54SupertribussesList?.Clear();

                AppBarButtonDeleteInfrafamily.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelInfrafamily.Visibility = Visibility.Visible;
                InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SupertribussesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl51InfrafamilyListPage));
            }
            else
            {
                InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
                ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[0];
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

    private async void InfrafamilyDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await InfrafamilyDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task InfrafamilyDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.InfrafamilySelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.InfrafamilySelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
            .Where(refe => refe.InfrafamilyId == ViewModel.InfrafamilySelected.InfrafamilyId)];
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
                ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
            .Where(com => com.InfrafamilyId == ViewModel.InfrafamilySelected.InfrafamilyId)];
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
            //--------------------------Supertribus-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl54SupertribussesList ??= [];
                ViewModel.Tbl54SupertribussesList = [.. AppData.Data.Tbl54Supertribusses
            .Where(com => com.InfrafamilyId == ViewModel.InfrafamilySelected.InfrafamilyId)];
            });
            ViewModel.SupertribusDataSetCount = ViewModel.Tbl54SupertribussesList.Count;
            ViewModel.RefreshSupertribusItems();

            if (ViewModel.Tbl54SupertribussesList.Count == 0)
            {
                SupertribussesExpander.Visibility = Visibility.Collapsed;
                SupertribusDataGrid.ItemsSource = null;
            }
            else
            {
                SupertribussesExpander.Visibility = Visibility.Visible;
                SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddInfrafamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddInfrafamily();
            InfrafamilyName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfrafamilyName);
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

    private async Task AddInfrafamily()
    {
        ViewModel.IsInEditInfrafamily = true;
        ViewModel.IsNewInfrafamily = true;

        await ViewModel.AddInfrafamilyAsync();

        if (ViewModel.Tbl51InfrafamiliesList != null)
        {
            ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[0];
        }

        InfrafamilyDetailExpander.IsExpanded = true;
        InfrafamilyDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void InfrafamilySearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        InfrafamilySearchBox.Visibility = Visibility.Visible;
        InfrafamilySearchBox.Focus(FocusState.Programmatic);
        InfrafamilySearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void InfrafamilyDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl51InfrafamiliesList.Sort);
    }

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
            SingleInfrafamily = AppData.Data.SingleInfrafamily(ViewModel.InfrafamilySelected.InfrafamilyId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsInfrafamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewInfrafamily)
        {
            InfrafamilyDetailExpander.IsExpanded = false;
            InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
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
        if (iNdx is not -1)
        {
            ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[iNdx];
            SingleInfrafamily = AppData.Data.SingleInfrafamily(ViewModel.InfrafamilySelected.InfrafamilyId);
            InfrafamilySingles();
        }
        else
        {
            InfrafamilyDetailExpander.IsExpanded = false;
            InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditInfrafamily = false;
        ViewModel.IsNewInfrafamily = false;
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

    private Task RefreshInfrafamilyAsync()
    {
        if (InfrafamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.InfrafamilySelected.InfrafamilyId == 0)
            {
                return Task.CompletedTask;
            }
            SingleInfrafamily = AppData.Data.SingleInfrafamily(ViewModel.InfrafamilySelected.InfrafamilyId);
            InfrafamilySingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteInfrafamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteInfrafamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteInfrafamily()
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
                await ViewModel.DeleteInfrafamilyAsync(ViewModel.InfrafamilySelected);
                await Refresh();
                InfrafamilyDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesInfrafamilyAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
                await BehindSaveInfrafamily(ViewModel.InfrafamilySelected);
                await Refresh();
                ViewModel.IsNewInfrafamily = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfrafamily = true;
                await RevertChangesInfrafamilyAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfrafamily()
    {
        _search = InfrafamilySearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl51InfrafamiliesList ??= [];
            ViewModel.Tbl51InfrafamiliesList = [.. AppData.Data.Tbl51Infrafamilies
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

    private async Task BehindSaveInfrafamily(Tbl51Infrafamily viewModelInfrafamilySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl51InfrafamiliesList ??= [];
            if (InfrafamilySearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl51InfrafamiliesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl51Infrafamilies
                    .Where(a => a.InfrafamilyId == id)]
                    : [.. AppData.Data.Tbl51Infrafamilies
                    .Where(a => _search != null && a.InfrafamilyName.StartsWith(_search))
                    .OrderBy(a => a.InfrafamilyName)];
            }
        });

        ViewModel.RefreshInfrafamilyItems();

        if (ViewModel.Tbl51InfrafamiliesList.Count == 0)
        {
            ViewModel.Tbl51InfrafamiliesList = [.. AppData.Data.Tbl51Infrafamilies
                .Where(a => a.InfrafamilyId == viewModelInfrafamilySelected.InfrafamilyId)];
            InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
            await Refresh();
            InfrafamilyDataGrid.SelectedIndex = 0;
        }
        else
        {
            InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;
            var index = ViewModel.InfrafamilyItems.ToList().FindIndex(item => item.InfrafamilyId == viewModelInfrafamilySelected.InfrafamilyId);
            InfrafamilyDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.InfrafamilyDataSetCount = ViewModel.Tbl51InfrafamiliesList.Count;
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

    #region [Reference Methods]

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
                SingleReference = AppData.Data.SingleReference(ViewModel.ReferenceSelected.ReferenceId);
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
            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.InfrafamilySelected.InfrafamilyId);
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
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(a => a.Info == ViewModel.ReferenceSelected.Info &&
                        a.InfrafamilyId == ViewModel.ReferenceSelected.InfrafamilyId &&
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
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(refe => refe.InfrafamilyId == viewModelReferenceSelected.InfrafamilyId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(com => com.InfrafamilyId == viewModelReferenceSelected.InfrafamilyId)];
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

            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfrafamilySelected.InfrafamilyId);
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
            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfrafamilySelected.InfrafamilyId);

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

            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.InfrafamilySelected.InfrafamilyId);
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
            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.InfrafamilySelected.InfrafamilyId);

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

            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.InfrafamilySelected.InfrafamilyId);
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
            SingleReference = AppData.Data.SingleReferenceInfrafamily(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.InfrafamilySelected.InfrafamilyId);

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
            SingleComment = AppData.Data.SingleCommentInfrafamily(ViewModel.CommentSelected.CommentId, ViewModel.InfrafamilySelected.InfrafamilyId);
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

            SingleComment = AppData.Data.SingleCommentInfrafamily(ViewModel.CommentSelected.CommentId, ViewModel.InfrafamilySelected.InfrafamilyId);
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
                SingleComment = AppData.Data.SingleCommentInfrafamily(ViewModel.CommentSelected.CommentId, ViewModel.InfrafamilySelected.InfrafamilyId);
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
            ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
            .Where(a => a.Info == ViewModel.CommentSelected.Info &&
                        a.Memo == ViewModel.CommentSelected.Memo &&
                        a.InfrafamilyId == ViewModel.CommentSelected.InfrafamilyId)];
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
            ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
                .Where(com => com.InfrafamilyId == viewModelCommentSelected.InfrafamilyId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
                .Where(com => com.InfrafamilyId == viewModelCommentSelected.InfrafamilyId)];
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

    #region [Supertribus Methods]

    private void DataGrid_Supertribus_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl54SupertribussesList.Sort);
    }

    private async void ViewSupertribusHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSupertribusHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSupertribusHyperlink()
    {
        ViewModel.IsInEditSupertribus = false;
        ViewModel.IsNewSupertribus = false;
        SupertribusDetailExpander.IsExpanded = true;
        SupertribusDetailExpander.Visibility = Visibility.Visible;

        if (SupertribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSupertribus = AppData.Data.SingleSupertribusInfrafamily(ViewModel.SupertribusSelected.SupertribusId,
                    ViewModel.InfrafamilySelected.InfrafamilyId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSupertribusAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSupertribus)
        {
            SupertribusDetailExpander.IsExpanded = false;
            SupertribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSupertribusEdits();
        }
    }

    private void CancelSupertribusEdits()
    {
        var x = (DataGrid)FindName("SupertribusDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SupertribusSelected = ViewModel.Tbl54SupertribussesList[iNdx];

            SingleSupertribus = AppData.Data.SingleSupertribusInfrafamily(ViewModel.SupertribusSelected.SupertribusId, ViewModel.InfrafamilySelected.InfrafamilyId);
            SupertribusSingles();
        }
        else
        {
            SupertribusDetailExpander.IsExpanded = false;
            SupertribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSupertribus = false;
        ViewModel.IsNewSupertribus = false;
    }

    private void SupertribusSingles()
    {
        ViewModel.SupertribusSelected.SupertribusName = SingleSupertribus.SupertribusName;
        ViewModel.SupertribusSelected.InfrafamilyId = SingleSupertribus.InfrafamilyId;
        ViewModel.SupertribusSelected.Valid = SingleSupertribus.Valid;
        ViewModel.SupertribusSelected.ValidYear = SingleSupertribus.ValidYear;
        ViewModel.SupertribusSelected.Author = SingleSupertribus.Author;
        ViewModel.SupertribusSelected.AuthorYear = SingleSupertribus.AuthorYear;
        ViewModel.SupertribusSelected.Info = SingleSupertribus.Info;
        ViewModel.SupertribusSelected.Synonym = SingleSupertribus.Synonym;
        ViewModel.SupertribusSelected.EngName = SingleSupertribus.EngName;
        ViewModel.SupertribusSelected.GerName = SingleSupertribus.GerName;
        ViewModel.SupertribusSelected.FraName = SingleSupertribus.FraName;
        ViewModel.SupertribusSelected.PorName = SingleSupertribus.PorName;
        ViewModel.SupertribusSelected.Memo = SingleSupertribus.Memo;
    }

    private async Task RevertChangesSupertribusAsync()
    {
        ViewModel.IsInEditSupertribus = false;
        if (ViewModel.IsModifiedSupertribus)
        {
            await RefreshSupertribusAsync();
            ViewModel.IsModifiedSupertribus = false;
        }
    }
    private async Task RefreshSupertribusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SupertribusDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSupertribus = AppData.Data.SingleSupertribusInfrafamily(ViewModel.SupertribusSelected.SupertribusId, ViewModel.InfrafamilySelected.InfrafamilyId);
                SupertribusSingles();
            }
        });
    }

    private async void SaveSupertribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSupertribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSupertribus()
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
                if (await BeforeSaveSupertribus())
                {
                    return;
                }
                await ViewModel.SaveSupertribusAsync(ViewModel.SupertribusSelected);
                await BehindSaveSupertribus(ViewModel.SupertribusSelected);
                await Refresh();
                ViewModel.IsNewSupertribus = false;
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSupertribus = true;
                await RevertChangesSupertribusAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSupertribus()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl54SupertribussesList ??= [];
            ViewModel.Tbl54SupertribussesList = [.. AppData.Data.Tbl54Supertribusses
            .Where(a => a.SupertribusName == ViewModel.SupertribusSelected.SupertribusName &&
                        a.InfrafamilyId == ViewModel.SupertribusSelected.InfrafamilyId)];
        });
        if (ViewModel.IsNewSupertribus)
        {
            if (ViewModel.Tbl54SupertribussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSupertribus = true;
        return false;
    }

    private async Task BehindSaveSupertribus(Tbl54Supertribus viewModelSupertribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl54SupertribussesList ??= [];
            ViewModel.Tbl54SupertribussesList = [.. AppData.Data.Tbl54Supertribusses
                .Where(com => com.InfrafamilyId == viewModelSupertribusSelected.InfrafamilyId)];
        });
        ViewModel.RefreshSupertribusItems();

        if (ViewModel.Tbl54SupertribussesList.Count == 0)
        {
            ViewModel.Tbl54SupertribussesList = [.. AppData.Data.Tbl54Supertribusses
                .Where(com => com.InfrafamilyId == viewModelSupertribusSelected.InfrafamilyId)];
            SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
            await Refresh();
            SupertribussesExpander.Visibility = Visibility.Collapsed;
            SupertribusDataGrid.ItemsSource = null;
        }
        else
        {
            SupertribussesExpander.Visibility = Visibility.Visible;
            SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
            SupertribusDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.SupertribusDataSetCount = ViewModel.Tbl54SupertribussesList.Count;
    }

    private void SupertribusName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSupertribus_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Supertribus Methods] 

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
            //  DetailPanelInfrafamily.Visibility = Visibility.Visible;
            //  InfrafamilyDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("InfrafamilyDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl51InfrafamiliesList!.Count)
            {
                ViewModel.InfrafamilyItems.Clear();
                foreach (var item in ViewModel.Tbl51InfrafamiliesList)
                {
                    ViewModel.InfrafamilyItems.Add(item);
                }

                ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[iNdx];
            }
            InfrafamilyDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl51InfrafamiliesList.Count == 0)
                {
                    ViewModel.Tbl51InfrafamiliesList ??= [];
                    ViewModel.Tbl51InfrafamiliesList = [.. AppData.Data.Tbl51Infrafamilies
                            .Where(a => a.InfrafamilyId == ViewModel.InfrafamilySelected.InfrafamilyId)];
                }
                else
                {
                    ViewModel.Tbl51InfrafamiliesList ??= [];
                    ViewModel.Tbl51InfrafamiliesList = int.TryParse(_search, out var id)
                        ? [.. AppData.Data.Tbl51Infrafamilies
                           .Where(a => a.InfrafamilyId == id)]
                       : [.. AppData.Data.Tbl51Infrafamilies
                           .Where(a => _search != null && a.InfrafamilyName.StartsWith(_search))
                         .OrderBy(a => a.InfrafamilyName)];
                }
            });

            InfrafamilyDataGrid.ItemsSource = ViewModel.Tbl51InfrafamiliesList;

            if (ViewModel.IsDeletingInfrafamily)
            {
                if (iNdx == 0 && ViewModel.Tbl51InfrafamiliesList.Count == 0)
                {
                    ViewModel.Tbl51InfrafamiliesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.InfrafamilySelected = ViewModel.Tbl51InfrafamiliesList[iNdx];
            }
            if (ViewModel.Tbl51InfrafamiliesList is { Count: 0 })
            {
                ViewModel.Tbl51InfrafamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl54SupertribussesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        InfrafamilyDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingInfrafamily = false;
    }

    private void BackDetailExpanders()
    {
        InfrafamilyDetailExpander.IsExpanded = false;
        InfrafamilyDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SupertribusDetailExpander.IsExpanded = false;
        SupertribusDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteInfrafamily.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteInfrafamily.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteInfrafamily.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteInfrafamily.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

