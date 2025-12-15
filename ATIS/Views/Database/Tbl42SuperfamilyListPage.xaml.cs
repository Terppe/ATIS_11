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

//  Tbl42SuperfamilyListPage.xaml.cs Skriptdatum:  15.06.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl42SuperfamilyListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl42SuperfamilyViewModel ViewModel { get; }
    private Tbl42Superfamily SingleSuperfamily { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl45Family SingleFamily { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl42SuperfamilyListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl42SuperfamilyViewModel>();
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

    #region [Superfamily Methods]

    private async void GetSuperfamiliesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl42SuperfamiliesList?.Clear();

            ViewModel.SuperfamilyItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl42SuperfamiliesList?.Clear();

                SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl45FamiliesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                FamiliesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSuperfamily = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl42SuperfamiliesList ??= [];
                ViewModel.Tbl42SuperfamiliesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl42Superfamilies
                .Where(a => a.SuperfamilyId == id)]
                : [.. Context.Tbl42Superfamilies
                .Where(a => _search != null && a.SuperfamilyName.StartsWith(_search))
                .OrderBy(a => a.SuperfamilyName)];
            });

            SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            FamiliesExpander.Visibility = Visibility.Collapsed;
            FamilyDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SuperfamilyDataSetCount = ViewModel.Tbl42SuperfamiliesList.Count;

            if (ViewModel.Tbl42SuperfamiliesList is { Count: 0 })
            {
                ViewModel.Tbl42SuperfamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl45FamiliesList?.Clear();

                AppBarButtonDeleteSuperfamily.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSuperfamily.Visibility = Visibility.Visible;
                SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                FamiliesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl42SuperfamilyListPage));
            }
            else
            {
                SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
                ViewModel.SuperfamilySelected = ViewModel.Tbl42SuperfamiliesList[0];
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

    private async void SuperfamilyDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SuperfamilyDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SuperfamilyDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SuperfamilySelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SuperfamilySelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.SuperfamilyId == ViewModel.SuperfamilySelected.SuperfamilyId)];
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
            .Where(com => com.SuperfamilyId == ViewModel.SuperfamilySelected.SuperfamilyId)];
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
            //--------------------------Family-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl45FamiliesList ??= [];
                ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
            .Where(com => com.SuperfamilyId == ViewModel.SuperfamilySelected.SuperfamilyId)];
            });
            ViewModel.FamilyDataSetCount = ViewModel.Tbl45FamiliesList.Count;
            ViewModel.RefreshFamilyItems();

            if (ViewModel.Tbl45FamiliesList.Count == 0)
            {
                FamiliesExpander.Visibility = Visibility.Collapsed;
                FamilyDataGrid.ItemsSource = null;
            }
            else
            {
                FamiliesExpander.Visibility = Visibility.Visible;
                FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSuperfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSuperfamily();
            SuperfamilyName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SuperfamilyName);
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

    private async Task AddSuperfamily()
    {
        ViewModel.IsInEditSuperfamily = true;
        ViewModel.IsNewSuperfamily = true;

        await ViewModel.AddSuperfamilyAsync();

        if (ViewModel.Tbl42SuperfamiliesList != null)
        {
            ViewModel.SuperfamilySelected = ViewModel.Tbl42SuperfamiliesList[0];
        }

        SuperfamilyDetailExpander.IsExpanded = true;
        SuperfamilyDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void SuperfamilySearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SuperfamilySearchBox.Visibility = Visibility.Visible;
        SuperfamilySearchBox.Focus(FocusState.Programmatic);
        SuperfamilySearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SuperfamilyDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl42SuperfamiliesList.Sort);
    }

    private void AddNewSuperfamilyCanceled(object sender, EventArgs e) => SuperfamilyDetailExpander.IsExpanded = false;

    private async void ViewSuperfamilyHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSuperfamilyHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSuperfamilyHyperlink()
    {
        ViewModel.IsInEditSuperfamily = false;
        ViewModel.IsNewSuperfamily = false;
        SuperfamilyDetailExpander.IsExpanded = true;
        SuperfamilyDetailExpander.Visibility = Visibility.Visible;

        if (SuperfamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSuperfamily = App.SingleSuperfamily(ViewModel.SuperfamilySelected.SuperfamilyId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSuperfamilyAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSuperfamily)
        {
            SuperfamilyDetailExpander.IsExpanded = false;
            SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSuperfamilyEdits();
        }
    }

    private void CancelSuperfamilyEdits()
    {
        var x = (DataGrid)FindName("SuperfamilyDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SuperfamilySelected = ViewModel.Tbl42SuperfamiliesList[iNdx];
            SingleSuperfamily = App.SingleSuperfamily(ViewModel.SuperfamilySelected.SuperfamilyId);
            SuperfamilySingles();
        }
        else
        {
            SuperfamilyDetailExpander.IsExpanded = false;
            SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSuperfamily = false;
        ViewModel.IsNewSuperfamily = false;
    }

    private void SuperfamilySingles()
    {
        ViewModel.SuperfamilySelected.SuperfamilyName = SingleSuperfamily.SuperfamilyName;
        ViewModel.SuperfamilySelected.InfraordoId = SingleSuperfamily.InfraordoId;
        ViewModel.SuperfamilySelected.Valid = SingleSuperfamily.Valid;
        ViewModel.SuperfamilySelected.ValidYear = SingleSuperfamily.ValidYear;
        ViewModel.SuperfamilySelected.Author = SingleSuperfamily.Author;
        ViewModel.SuperfamilySelected.AuthorYear = SingleSuperfamily.AuthorYear;
        ViewModel.SuperfamilySelected.Info = SingleSuperfamily.Info;
        ViewModel.SuperfamilySelected.Synonym = SingleSuperfamily.Synonym;
        ViewModel.SuperfamilySelected.EngName = SingleSuperfamily.EngName;
        ViewModel.SuperfamilySelected.GerName = SingleSuperfamily.GerName;
        ViewModel.SuperfamilySelected.FraName = SingleSuperfamily.FraName;
        ViewModel.SuperfamilySelected.PorName = SingleSuperfamily.PorName;
        ViewModel.SuperfamilySelected.Memo = SingleSuperfamily.Memo;
    }
    private async Task RevertChangesSuperfamilyAsync()
    {
        ViewModel.IsInEditSuperfamily = false;
        if (ViewModel.IsModifiedSuperfamily)
        {
            await RefreshSuperfamilyAsync();
            ViewModel.IsModifiedSuperfamily = false;
        }
    }
    private Task RefreshSuperfamilyAsync()
    {
        if (SuperfamilyDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.SuperfamilySelected.SuperfamilyId == 0)
            {
                return Task.CompletedTask;
            }
            SingleSuperfamily = App.SingleSuperfamily(ViewModel.SuperfamilySelected.SuperfamilyId);
            SuperfamilySingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSuperfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSuperfamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSuperfamily()
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
                await ViewModel.DeleteSuperfamilyAsync(ViewModel.SuperfamilySelected);
                await Refresh();
                SuperfamilyDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSuperfamilyAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSuperfamily_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSuperfamily();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSuperfamily()
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
                if (await BeforeSaveSuperfamily())
                {
                    return;
                }
                await ViewModel.SaveSuperfamilyAsync(ViewModel.SuperfamilySelected);
                await BehindSaveSuperfamily(ViewModel.SuperfamilySelected);
                await Refresh();
                ViewModel.IsNewSuperfamily = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSuperfamily = true;
                await RevertChangesSuperfamilyAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSuperfamily()
    {
        _search = SuperfamilySearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl42SuperfamiliesList ??= [];
            ViewModel.Tbl42SuperfamiliesList = [.. Context.Tbl42Superfamilies
                .Where(a => a.SuperfamilyName == ViewModel.SuperfamilySelected.SuperfamilyName &&
                        a.InfraordoId == ViewModel.SuperfamilySelected.InfraordoId)];
        });
        if (ViewModel.IsNewSuperfamily)
        {
            if (ViewModel.Tbl42SuperfamiliesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSuperfamily = true;
        return false;
    }

    private async Task BehindSaveSuperfamily(Tbl42Superfamily viewModelSuperfamilySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl42SuperfamiliesList ??= [];
            if (SuperfamilySearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl42SuperfamiliesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl42Superfamilies
                    .Where(a => a.SuperfamilyId == id)]
                    : [.. Context.Tbl42Superfamilies
                    .Where(a => _search != null && a.SuperfamilyName.StartsWith(_search))
                    .OrderBy(a => a.SuperfamilyName)];
            }
        });

        ViewModel.RefreshSuperfamilyItems();

        if (ViewModel.Tbl42SuperfamiliesList.Count == 0)
        {
            ViewModel.Tbl42SuperfamiliesList = [.. Context.Tbl42Superfamilies
                .Where(a => a.SuperfamilyId == viewModelSuperfamilySelected.SuperfamilyId)];
            SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
            await Refresh();
            SuperfamilyDataGrid.SelectedIndex = 0;
        }
        else
        {
            SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
            var index = ViewModel.SuperfamilyItems.ToList().FindIndex(item => item.SuperfamilyId == viewModelSuperfamilySelected.SuperfamilyId);
            SuperfamilyDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.SuperfamilyDataSetCount = ViewModel.Tbl42SuperfamiliesList.Count;
    }

    private void SuperfamilyName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSuperfamily_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Superfamily Methods]                    

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
            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SuperfamilySelected.SuperfamilyId);
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
                        a.SuperfamilyId == ViewModel.ReferenceSelected.SuperfamilyId &&
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
                .Where(refe => refe.SuperfamilyId == viewModelReferenceSelected.SuperfamilyId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.SuperfamilyId == viewModelReferenceSelected.SuperfamilyId)];
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

            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SuperfamilySelected.SuperfamilyId);
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
            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SuperfamilySelected.SuperfamilyId);

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

            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.SuperfamilySelected.SuperfamilyId);
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
            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.SuperfamilySelected.SuperfamilyId);

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

            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.SuperfamilySelected.SuperfamilyId);
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
            SingleReference = App.SingleReferenceSuperfamily(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.SuperfamilySelected.SuperfamilyId);

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
            SingleComment = App.SingleCommentSuperfamily(ViewModel.CommentSelected.CommentId, ViewModel.SuperfamilySelected.SuperfamilyId);
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

            SingleComment = App.SingleCommentSuperfamily(ViewModel.CommentSelected.CommentId, ViewModel.SuperfamilySelected.SuperfamilyId);
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
                SingleComment = App.SingleCommentSuperfamily(ViewModel.CommentSelected.CommentId, ViewModel.SuperfamilySelected.SuperfamilyId);
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
                        a.SuperfamilyId == ViewModel.CommentSelected.SuperfamilyId)];
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
                .Where(com => com.SuperfamilyId == viewModelCommentSelected.SuperfamilyId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.SuperfamilyId == viewModelCommentSelected.SuperfamilyId)];
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

    #region [Family Methods]

    private void DataGrid_Family_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl45FamiliesList.Sort);
    }

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
            SingleFamily = App.SingleFamilySuperfamily(ViewModel.FamilySelected.FamilyId,
                    ViewModel.SuperfamilySelected.SuperfamilyId);
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

            SingleFamily = App.SingleFamilySuperfamily(ViewModel.FamilySelected.FamilyId, ViewModel.SuperfamilySelected.SuperfamilyId);
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
    private async Task RefreshFamilyAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (FamilyDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleFamily = App.SingleFamilySuperfamily(ViewModel.FamilySelected.FamilyId, ViewModel.SuperfamilySelected.SuperfamilyId);
                FamilySingles();
            }
        });
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
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedFamily = true;
                await RevertChangesFamilyAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveFamily()
    {
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
            ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
                .Where(com => com.SuperfamilyId == viewModelFamilySelected.SuperfamilyId)];
        });
        ViewModel.RefreshFamilyItems();

        if (ViewModel.Tbl45FamiliesList.Count == 0)
        {
            ViewModel.Tbl45FamiliesList = [.. Context.Tbl45Families
                .Where(com => com.SuperfamilyId == viewModelFamilySelected.SuperfamilyId)];
            FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
            await Refresh();
            FamiliesExpander.Visibility = Visibility.Collapsed;
            FamilyDataGrid.ItemsSource = null;
        }
        else
        {
            FamiliesExpander.Visibility = Visibility.Visible;
            FamilyDataGrid.ItemsSource = ViewModel.Tbl45FamiliesList;
            FamilyDataGrid.SelectedIndex = 0;
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
            //  DetailPanelSuperfamily.Visibility = Visibility.Visible;
            //  SuperfamilyDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SuperfamilyDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl42SuperfamiliesList!.Count)
            {
                ViewModel.SuperfamilyItems.Clear();
                foreach (var item in ViewModel.Tbl42SuperfamiliesList)
                {
                    ViewModel.SuperfamilyItems.Add(item);
                }

                ViewModel.SuperfamilySelected = ViewModel.Tbl42SuperfamiliesList[iNdx];
            }
            SuperfamilyDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl42SuperfamiliesList.Count == 0)
                {
                    ViewModel.Tbl42SuperfamiliesList ??= [];
                    ViewModel.Tbl42SuperfamiliesList = [.. Context.Tbl42Superfamilies
                            .Where(a => a.SuperfamilyId == ViewModel.SuperfamilySelected.SuperfamilyId)];
                }
                else
                {
                    ViewModel.Tbl42SuperfamiliesList ??= [];
                    ViewModel.Tbl42SuperfamiliesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl42Superfamilies
                           .Where(a => a.SuperfamilyId == id)]
                       : [.. Context.Tbl42Superfamilies
                           .Where(a => _search != null && a.SuperfamilyName.StartsWith(_search))
                         .OrderBy(a => a.SuperfamilyName)];
                }
            });

            SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;

            if (ViewModel.IsDeletingSuperfamily)
            {
                if (iNdx == 0 && ViewModel.Tbl42SuperfamiliesList.Count == 0)
                {
                    ViewModel.Tbl42SuperfamiliesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SuperfamilySelected = ViewModel.Tbl42SuperfamiliesList[iNdx];
            }
            if (ViewModel.Tbl42SuperfamiliesList is { Count: 0 })
            {
                ViewModel.Tbl42SuperfamiliesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl45FamiliesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SuperfamilyDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSuperfamily = false;
    }

    private void BackDetailExpanders()
    {
        SuperfamilyDetailExpander.IsExpanded = false;
        SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        FamilyDetailExpander.IsExpanded = false;
        FamilyDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteSuperfamily.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteSuperfamily.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteSuperfamily.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteSuperfamily.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

