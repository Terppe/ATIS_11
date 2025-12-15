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

//  Tbl54SupertribusListPage.xaml.cs Skriptdatum:  22.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl54SupertribusListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl54SupertribusViewModel ViewModel { get; }
    private Tbl54Supertribus SingleSupertribus { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl57Tribus SingleTribus { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl54SupertribusListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl54SupertribusViewModel>();
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

    #region [Supertribus Methods]

    private async void GetSupertribussesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl54SupertribussesList?.Clear();

            ViewModel.SupertribusItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl54SupertribussesList?.Clear();

                SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl57TribussesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                TribussesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSupertribus = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl54SupertribussesList ??= [];
                ViewModel.Tbl54SupertribussesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl54Supertribusses
                .Where(a => a.SupertribusId == id)]
                : [.. Context.Tbl54Supertribusses
                .Where(a => _search != null && a.SupertribusName.StartsWith(_search))
                .OrderBy(a => a.SupertribusName)];
            });

            SupertribusDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            TribussesExpander.Visibility = Visibility.Collapsed;
            TribusDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SupertribusDataSetCount = ViewModel.Tbl54SupertribussesList.Count;

            if (ViewModel.Tbl54SupertribussesList is { Count: 0 })
            {
                ViewModel.Tbl54SupertribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl57TribussesList?.Clear();

                AppBarButtonDeleteSupertribus.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSupertribus.Visibility = Visibility.Visible;
                SupertribusDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                TribussesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl54SupertribusListPage));
            }
            else
            {
                SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
                ViewModel.SupertribusSelected = ViewModel.Tbl54SupertribussesList[0];
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

    private async void SupertribusDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SupertribusDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SupertribusDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SupertribusSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SupertribusSelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.SupertribusId == ViewModel.SupertribusSelected.SupertribusId)];
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
            .Where(com => com.SupertribusId == ViewModel.SupertribusSelected.SupertribusId)];
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
            //--------------------------Tribus-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl57TribussesList ??= [];
                ViewModel.Tbl57TribussesList = [.. Context.Tbl57Tribusses
            .Where(com => com.SupertribusId == ViewModel.SupertribusSelected.SupertribusId)];
            });
            ViewModel.TribusDataSetCount = ViewModel.Tbl57TribussesList.Count;
            ViewModel.RefreshTribusItems();

            if (ViewModel.Tbl57TribussesList.Count == 0)
            {
                TribussesExpander.Visibility = Visibility.Collapsed;
                TribusDataGrid.ItemsSource = null;
            }
            else
            {
                TribussesExpander.Visibility = Visibility.Visible;
                TribusDataGrid.ItemsSource = ViewModel.Tbl57TribussesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSupertribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSupertribus();
            SupertribusName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SupertribusName);
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

    private async Task AddSupertribus()
    {
        ViewModel.IsInEditSupertribus = true;
        ViewModel.IsNewSupertribus = true;

        await ViewModel.AddSupertribusAsync();

        if (ViewModel.Tbl54SupertribussesList != null)
        {
            ViewModel.SupertribusSelected = ViewModel.Tbl54SupertribussesList[0];
        }

        SupertribusDetailExpander.IsExpanded = true;
        SupertribusDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void SupertribusSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SupertribusSearchBox.Visibility = Visibility.Visible;
        SupertribusSearchBox.Focus(FocusState.Programmatic);
        SupertribusSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SupertribusDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl54SupertribussesList.Sort);
    }

    private void AddNewSupertribusCanceled(object sender, EventArgs e) => SupertribusDetailExpander.IsExpanded = false;

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
            SingleSupertribus = App.SingleSupertribus(ViewModel.SupertribusSelected.SupertribusId);
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
            SingleSupertribus = App.SingleSupertribus(ViewModel.SupertribusSelected.SupertribusId);
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
    private Task RefreshSupertribusAsync()
    {
        if (SupertribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.SupertribusSelected.SupertribusId == 0)
            {
                return Task.CompletedTask;
            }
            SingleSupertribus = App.SingleSupertribus(ViewModel.SupertribusSelected.SupertribusId);
            SupertribusSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSupertribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSupertribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSupertribus()
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
                await ViewModel.DeleteSupertribusAsync(ViewModel.SupertribusSelected);
                await Refresh();
                SupertribusDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSupertribusAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSupertribus = true;
                await RevertChangesSupertribusAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSupertribus()
    {
        _search = SupertribusSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl54SupertribussesList ??= [];
            ViewModel.Tbl54SupertribussesList = [.. Context.Tbl54Supertribusses
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
            if (SupertribusSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl54SupertribussesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl54Supertribusses
                    .Where(a => a.SupertribusId == id)]
                    : [.. Context.Tbl54Supertribusses
                    .Where(a => _search != null && a.SupertribusName.StartsWith(_search))
                    .OrderBy(a => a.SupertribusName)];
            }
        });

        ViewModel.RefreshSupertribusItems();

        if (ViewModel.Tbl54SupertribussesList.Count == 0)
        {
            ViewModel.Tbl54SupertribussesList = [.. Context.Tbl54Supertribusses
                .Where(a => a.SupertribusId == viewModelSupertribusSelected.SupertribusId)];
            SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
            await Refresh();
            SupertribusDataGrid.SelectedIndex = 0;
        }
        else
        {
            SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;
            var index = ViewModel.SupertribusItems.ToList().FindIndex(item => item.SupertribusId == viewModelSupertribusSelected.SupertribusId);
            SupertribusDataGrid.SelectedIndex = index;
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
            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SupertribusSelected.SupertribusId);
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
                        a.SupertribusId == ViewModel.ReferenceSelected.SupertribusId &&
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
                .Where(refe => refe.SupertribusId == viewModelReferenceSelected.SupertribusId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.SupertribusId == viewModelReferenceSelected.SupertribusId)];
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

            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SupertribusSelected.SupertribusId);
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
            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SupertribusSelected.SupertribusId);

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

            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.SupertribusSelected.SupertribusId);
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
            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.SupertribusSelected.SupertribusId);

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

            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.SupertribusSelected.SupertribusId);
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
            SingleReference = App.SingleReferenceSupertribus(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.SupertribusSelected.SupertribusId);

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
            SingleComment = App.SingleCommentSupertribus(ViewModel.CommentSelected.CommentId, ViewModel.SupertribusSelected.SupertribusId);
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

            SingleComment = App.SingleCommentSupertribus(ViewModel.CommentSelected.CommentId, ViewModel.SupertribusSelected.SupertribusId);
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
                SingleComment = App.SingleCommentSupertribus(ViewModel.CommentSelected.CommentId, ViewModel.SupertribusSelected.SupertribusId);
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
                        a.SupertribusId == ViewModel.CommentSelected.SupertribusId)];
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
                .Where(com => com.SupertribusId == viewModelCommentSelected.SupertribusId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.SupertribusId == viewModelCommentSelected.SupertribusId)];
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

    #region [Tribus Methods]

    private void DataGrid_Tribus_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl57TribussesList.Sort);
    }

    private async void ViewTribusHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewTribusHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewTribusHyperlink()
    {
        ViewModel.IsInEditTribus = false;
        ViewModel.IsNewTribus = false;
        TribusDetailExpander.IsExpanded = true;
        TribusDetailExpander.Visibility = Visibility.Visible;

        if (TribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleTribus = App.SingleTribusSupertribus(ViewModel.TribusSelected.TribusId,
                    ViewModel.SupertribusSelected.SupertribusId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsTribusAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewTribus)
        {
            TribusDetailExpander.IsExpanded = false;
            TribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelTribusEdits();
        }
    }

    private void CancelTribusEdits()
    {
        var x = (DataGrid)FindName("TribusDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.TribusSelected = ViewModel.Tbl57TribussesList[iNdx];

            SingleTribus = App.SingleTribusSupertribus(ViewModel.TribusSelected.TribusId, ViewModel.SupertribusSelected.SupertribusId);
            TribusSingles();
        }
        else
        {
            TribusDetailExpander.IsExpanded = false;
            TribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditTribus = false;
        ViewModel.IsNewTribus = false;
    }

    private void TribusSingles()
    {
        ViewModel.TribusSelected.TribusName = SingleTribus.TribusName;
        ViewModel.TribusSelected.SupertribusId = SingleTribus.SupertribusId;
        ViewModel.TribusSelected.Valid = SingleTribus.Valid;
        ViewModel.TribusSelected.ValidYear = SingleTribus.ValidYear;
        ViewModel.TribusSelected.Author = SingleTribus.Author;
        ViewModel.TribusSelected.AuthorYear = SingleTribus.AuthorYear;
        ViewModel.TribusSelected.Info = SingleTribus.Info;
        ViewModel.TribusSelected.Synonym = SingleTribus.Synonym;
        ViewModel.TribusSelected.EngName = SingleTribus.EngName;
        ViewModel.TribusSelected.GerName = SingleTribus.GerName;
        ViewModel.TribusSelected.FraName = SingleTribus.FraName;
        ViewModel.TribusSelected.PorName = SingleTribus.PorName;
        ViewModel.TribusSelected.Memo = SingleTribus.Memo;
    }

    private async Task RevertChangesTribusAsync()
    {
        ViewModel.IsInEditTribus = false;
        if (ViewModel.IsModifiedTribus)
        {
            await RefreshTribusAsync();
            ViewModel.IsModifiedTribus = false;
        }
    }
    private async Task RefreshTribusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (TribusDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleTribus = App.SingleTribusSupertribus(ViewModel.TribusSelected.TribusId, ViewModel.SupertribusSelected.SupertribusId);
                TribusSingles();
            }
        });
    }

    private async void SaveTribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveTribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveTribus()
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
                if (await BeforeSaveTribus())
                {
                    return;
                }
                await ViewModel.SaveTribusAsync(ViewModel.TribusSelected);
                await BehindSaveTribus(ViewModel.TribusSelected);
                await Refresh();
                ViewModel.IsNewTribus = false;
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedTribus = true;
                await RevertChangesTribusAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveTribus()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl57TribussesList ??= [];
            ViewModel.Tbl57TribussesList = [.. Context.Tbl57Tribusses
            .Where(a => a.TribusName == ViewModel.TribusSelected.TribusName &&
                        a.SupertribusId == ViewModel.TribusSelected.SupertribusId)];
        });
        if (ViewModel.IsNewTribus)
        {
            if (ViewModel.Tbl57TribussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedTribus = true;
        return false;
    }

    private async Task BehindSaveTribus(Tbl57Tribus viewModelTribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl57TribussesList ??= [];
            ViewModel.Tbl57TribussesList = [.. Context.Tbl57Tribusses
                .Where(com => com.SupertribusId == viewModelTribusSelected.SupertribusId)];
        });
        ViewModel.RefreshTribusItems();

        if (ViewModel.Tbl57TribussesList.Count == 0)
        {
            ViewModel.Tbl57TribussesList = [.. Context.Tbl57Tribusses
                .Where(com => com.SupertribusId == viewModelTribusSelected.SupertribusId)];
            TribusDataGrid.ItemsSource = ViewModel.Tbl57TribussesList;
            await Refresh();
            TribussesExpander.Visibility = Visibility.Collapsed;
            TribusDataGrid.ItemsSource = null;
        }
        else
        {
            TribussesExpander.Visibility = Visibility.Visible;
            TribusDataGrid.ItemsSource = ViewModel.Tbl57TribussesList;
            TribusDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.TribusDataSetCount = ViewModel.Tbl57TribussesList.Count;
    }

    private void TribusName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackTribus_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Tribus Methods] 

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
            //  DetailPanelSupertribus.Visibility = Visibility.Visible;
            //  SupertribusDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SupertribusDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl54SupertribussesList!.Count)
            {
                ViewModel.SupertribusItems.Clear();
                foreach (var item in ViewModel.Tbl54SupertribussesList)
                {
                    ViewModel.SupertribusItems.Add(item);
                }

                ViewModel.SupertribusSelected = ViewModel.Tbl54SupertribussesList[iNdx];
            }
            SupertribusDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl54SupertribussesList.Count == 0)
                {
                    ViewModel.Tbl54SupertribussesList ??= [];
                    ViewModel.Tbl54SupertribussesList = [.. Context.Tbl54Supertribusses
                            .Where(a => a.SupertribusId == ViewModel.SupertribusSelected.SupertribusId)];
                }
                else
                {
                    ViewModel.Tbl54SupertribussesList ??= [];
                    ViewModel.Tbl54SupertribussesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl54Supertribusses
                           .Where(a => a.SupertribusId == id)]
                       : [.. Context.Tbl54Supertribusses
                           .Where(a => _search != null && a.SupertribusName.StartsWith(_search))
                         .OrderBy(a => a.SupertribusName)];
                }
            });

            SupertribusDataGrid.ItemsSource = ViewModel.Tbl54SupertribussesList;

            if (ViewModel.IsDeletingSupertribus)
            {
                if (iNdx == 0 && ViewModel.Tbl54SupertribussesList.Count == 0)
                {
                    ViewModel.Tbl54SupertribussesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SupertribusSelected = ViewModel.Tbl54SupertribussesList[iNdx];
            }
            if (ViewModel.Tbl54SupertribussesList is { Count: 0 })
            {
                ViewModel.Tbl54SupertribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl57TribussesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SupertribusDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSupertribus = false;
    }

    private void BackDetailExpanders()
    {
        SupertribusDetailExpander.IsExpanded = false;
        SupertribusDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        TribusDetailExpander.IsExpanded = false;
        TribusDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteSupertribus.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteSupertribus.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteSupertribus.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteSupertribus.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

