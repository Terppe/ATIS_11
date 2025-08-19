
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

//  Tbl09DivisionListPage.xaml.cs Skriptdatum:  15.06.2024   12:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl09DivisionListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl09DivisionViewModel ViewModel { get; }
    private Tbl09Division SingleDivision { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl15Subdivision SingleSubdivision { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl09DivisionListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl09DivisionViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Division Methods]

    private async void GetDivisionsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl09DivisionsList?.Clear();

            ViewModel.DivisionItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl09DivisionsList?.Clear();

                DivisionDataGrid.ItemsSource = ViewModel.Tbl09DivisionsList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl15SubdivisionsList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubdivisionsExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingDivision = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl09DivisionsList ??= [];
                ViewModel.Tbl09DivisionsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl09Divisions
                .Where(a => a.DivisionId == id)]
                : [.. Context.Tbl09Divisions
                .Where(a => _search != null && a.DivisionName.StartsWith(_search))
                .OrderBy(a => a.DivisionName)];
            });

            DivisionDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SubdivisionsExpander.Visibility = Visibility.Collapsed;
            SubdivisionDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.DivisionDataSetCount = ViewModel.Tbl09DivisionsList.Count;

            if (ViewModel.Tbl09DivisionsList is { Count: 0 })
            {
                ViewModel.Tbl09DivisionsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl15SubdivisionsList?.Clear();

                AppBarButtonDeleteDivision.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddSubdivision.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelDivision.Visibility = Visibility.Visible;
                DivisionDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SubdivisionsExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl09DivisionListPage));
            }
            else
            {
                DivisionDataGrid.ItemsSource = ViewModel.Tbl09DivisionsList;
                ViewModel.DivisionSelected = ViewModel.Tbl09DivisionsList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void DivisionDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await DivisionDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DivisionDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.DivisionSelected == null)
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
            .Where(refe => refe.DivisionId == ViewModel.DivisionSelected.DivisionId)];
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
            .Where(com => com.DivisionId == ViewModel.DivisionSelected.DivisionId)];
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
        //--------------------------Subdivision-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl15SubdivisionsList ??= [];
            ViewModel.Tbl15SubdivisionsList = [.. Context.Tbl15Subdivisions
            .AsNoTracking()
            .Where(com => com.DivisionId == ViewModel.DivisionSelected.DivisionId)];
        });
        ViewModel.SubdivisionDataSetCount = ViewModel.Tbl15SubdivisionsList.Count;
        ViewModel.RefreshSubdivisionItems();

        if (ViewModel.Tbl15SubdivisionsList.Count == 0)
        {
            SubdivisionsExpander.Visibility = Visibility.Collapsed;
            SubdivisionDataGrid.ItemsSource = null;
        }
        else
        {
            SubdivisionsExpander.Visibility = Visibility.Visible;
            SubdivisionDataGrid.ItemsSource = ViewModel.Tbl15SubdivisionsList;
        }
        ViewModel.IsLoading = false;
    }

    private async void AddDivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddDivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddDivision()
    {
        DivisionName.Loaded += (s, e) =>
        {
            DivisionName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditDivision = true;
        ViewModel.IsNewDivision = true;

        await ViewModel.AddDivisionAsync();

        if (ViewModel.Tbl09DivisionsList != null)
        {
            ViewModel.DivisionSelected = ViewModel.Tbl09DivisionsList[0];
        }

        DivisionDetailExpander.IsExpanded = true;
        DivisionDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewDivision = false;
        DivisionName.PlaceholderText = StringsRes.EnterText;
    }

    private void DivisionSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        DivisionSearchBox.Visibility = Visibility.Visible;
        DivisionSearchBox.Focus(FocusState.Programmatic);
        DivisionSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void DivisionDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl09DivisionsList.Sort);
    }

    private void AddNewDivisionCanceled(object sender, EventArgs e) => DivisionDetailExpander.IsExpanded = false;

    private async void ViewDivisionHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewDivisionHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewDivisionHyperlink()
    {
        ViewModel.IsInEditDivision = false;
        ViewModel.IsNewDivision = false;
        DivisionDetailExpander.IsExpanded = true;
        DivisionDetailExpander.Visibility = Visibility.Visible;

        if (DivisionDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleDivision = App.SingleDivision(ViewModel.DivisionSelected.DivisionId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsDivisionAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewDivision)
        {
            DivisionDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelDivisionEdits();
        }
    }

    private void CancelDivisionEdits()
    {
        var x = (DataGrid)FindName("DivisionDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditDivision = false;
            ViewModel.IsNewDivision = false;
            DivisionDetailExpander.IsExpanded = false;
            DivisionDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.DivisionSelected = ViewModel.Tbl09DivisionsList[iNdx];

        SingleDivision = App.SingleDivision(ViewModel.DivisionSelected.DivisionId);
        DivisionSingles();

        ViewModel.IsInEditDivision = false;
        ViewModel.IsNewDivision = false;
        DivisionDetailExpander.IsExpanded = true;
        DivisionDetailExpander.Visibility = Visibility.Visible;
    }

    private void DivisionSingles()
    {
        ViewModel.DivisionSelected.DivisionName = SingleDivision.DivisionName;
        ViewModel.DivisionSelected.RegnumId = SingleDivision.RegnumId;
        ViewModel.DivisionSelected.Valid = SingleDivision.Valid;
        ViewModel.DivisionSelected.ValidYear = SingleDivision.ValidYear;
        ViewModel.DivisionSelected.Author = SingleDivision.Author;
        ViewModel.DivisionSelected.AuthorYear = SingleDivision.AuthorYear;
        ViewModel.DivisionSelected.Info = SingleDivision.Info;
        ViewModel.DivisionSelected.Synonym = SingleDivision.Synonym;
        ViewModel.DivisionSelected.EngName = SingleDivision.EngName;
        ViewModel.DivisionSelected.GerName = SingleDivision.GerName;
        ViewModel.DivisionSelected.FraName = SingleDivision.FraName;
        ViewModel.DivisionSelected.PorName = SingleDivision.PorName;
        ViewModel.DivisionSelected.Memo = SingleDivision.Memo;
    }
    private async Task RevertChangesDivisionAsync()
    {
        ViewModel.IsInEditDivision = false;
        if (ViewModel.IsModifiedDivision)
        {
            await RefreshDivisionAsync();
            ViewModel.IsModifiedDivision = false;
        }
    }
    private Task RefreshDivisionAsync()
    {
        if (DivisionDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleDivision = App.SingleDivision(ViewModel.DivisionSelected.DivisionId);
            DivisionSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteDivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteDivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteDivision()
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
                await ViewModel.DeleteDivisionAsync(ViewModel.DivisionSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesDivisionAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveDivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveDivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveDivision()
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
                if (await BeforeSaveDivision())
                {
                    return;
                }
                await ViewModel.SaveDivisionAsync(ViewModel.DivisionSelected);
                await BehindSaveDivision();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedDivision = true;
                await RevertChangesDivisionAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveDivision()
    {
        _search = DivisionSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl09DivisionsList ??= [];
            ViewModel.Tbl09DivisionsList = [.. Context.Tbl09Divisions
                .Where(a => a.DivisionName == ViewModel.DivisionSelected.DivisionName &&
                        a.RegnumId == ViewModel.DivisionSelected.RegnumId)];
        });
        if (ViewModel.IsNewDivision)
        {
            if (ViewModel.Tbl09DivisionsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedDivision = true;
        return false;
    }

    private async Task BehindSaveDivision()
    {
        var x = (DataGrid)FindName("DivisionDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl09DivisionsList ??= [];
            ViewModel.Tbl09DivisionsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl09Divisions
                    .Where(a => a.DivisionId == id)]
                : [.. Context.Tbl09Divisions
                    .Where(a => _search != null && a.DivisionName.StartsWith(_search))
                    .OrderBy(a => a.DivisionName)];
        });

        ViewModel.RefreshDivisionItems();

        if (ViewModel.Tbl09DivisionsList.Count == 0)
        {
            DivisionDataGrid.ItemsSource = null;
        }
        else
        {
            DivisionDataGrid.ItemsSource = ViewModel.Tbl09DivisionsList;
        }
        DivisionDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void DivisionName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackDivision_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Division Methods]                    

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
            SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);
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
                        a.DivisionId == ViewModel.ReferenceSelected.DivisionId &&
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
                .Where(refe => refe.FamilyId == ViewModel.DivisionSelected.DivisionId)];
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

        SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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
            SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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

        SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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
            SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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

        SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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
            SingleReference = App.SingleReferenceDivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.DivisionSelected.DivisionId);

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
            SingleComment = App.SingleCommentDivision(ViewModel.CommentSelected.CommentId, ViewModel.DivisionSelected.DivisionId);
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

        SingleComment = App.SingleCommentDivision(ViewModel.CommentSelected.CommentId, ViewModel.DivisionSelected.DivisionId);
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
                SingleComment = App.SingleCommentDivision(ViewModel.CommentSelected.CommentId, ViewModel.DivisionSelected.DivisionId);
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
                        a.DivisionId == ViewModel.CommentSelected.DivisionId)];
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
                .Where(com => com.DivisionId == ViewModel.DivisionSelected.DivisionId)];
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

    #region [Subdivision Methods]

    private async void AddSubdivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubdivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSubdivision()
    {
        SubdivisionName.Loaded += (s, e) =>
        {
            SubdivisionName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSubdivision = true;
        ViewModel.IsNewSubdivision = true;

        await ViewModel.AddSubdivisionAsync();

        if (ViewModel.Tbl15SubdivisionsList != null)
        {
            ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[0];
        }

        SubdivisionDetailExpander.IsExpanded = true;
        SubdivisionDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewSubdivision = false;
    }

    private void DataGrid_Subdivision_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl15SubdivisionsList.Sort);
    }

    private void AddNewSubdivisionCanceled(object sender, EventArgs e) => SubdivisionsExpander.IsExpanded = false;

    private async void ViewSubdivisionHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubdivisionHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubdivisionHyperlink()
    {
        ViewModel.IsInEditSubdivision = false;
        ViewModel.IsNewSubdivision = false;
        SubdivisionDetailExpander.IsExpanded = true;
        SubdivisionDetailExpander.Visibility = Visibility.Visible;

        if (SubdivisionDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubdivision = App.SingleSubdivisionDivision(ViewModel.SubdivisionSelected.SubdivisionId,
                    ViewModel.DivisionSelected.DivisionId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSubdivisionAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubdivision)
        {
            SubdivisionDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelSubdivisionEdits();
        }
    }

    private void CancelSubdivisionEdits()
    {
        var x = (DataGrid)FindName("SubdivisionDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditSubdivision = false;
            ViewModel.IsNewSubdivision = false;
            SubdivisionDetailExpander.IsExpanded = false;
            SubdivisionDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }
        ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[iNdx];

        SingleSubdivision = App.SingleSubdivisionDivision(ViewModel.SubdivisionSelected.SubdivisionId, ViewModel.DivisionSelected.DivisionId);
        SubdivisionSingles();

        ViewModel.IsInEditSubdivision = false;
        ViewModel.IsNewSubdivision = false;
        SubdivisionDetailExpander.IsExpanded = true;
        SubdivisionDetailExpander.Visibility = Visibility.Visible;
    }

    private void SubdivisionSingles()
    {
        ViewModel.SubdivisionSelected.SubdivisionName = SingleSubdivision.SubdivisionName;
        ViewModel.SubdivisionSelected.DivisionId = SingleSubdivision.DivisionId;
        ViewModel.SubdivisionSelected.Valid = SingleSubdivision.Valid;
        ViewModel.SubdivisionSelected.ValidYear = SingleSubdivision.ValidYear;
        ViewModel.SubdivisionSelected.Author = SingleSubdivision.Author;
        ViewModel.SubdivisionSelected.AuthorYear = SingleSubdivision.AuthorYear;
        ViewModel.SubdivisionSelected.Info = SingleSubdivision.Info;
        ViewModel.SubdivisionSelected.Synonym = SingleSubdivision.Synonym;
        ViewModel.SubdivisionSelected.EngName = SingleSubdivision.EngName;
        ViewModel.SubdivisionSelected.GerName = SingleSubdivision.GerName;
        ViewModel.SubdivisionSelected.FraName = SingleSubdivision.FraName;
        ViewModel.SubdivisionSelected.PorName = SingleSubdivision.PorName;
        ViewModel.SubdivisionSelected.Memo = SingleSubdivision.Memo;
    }

    private async Task RevertChangesSubdivisionAsync()
    {
        ViewModel.IsInEditSubdivision = false;
        if (ViewModel.IsModifiedSubdivision)
        {
            await RefreshSubdivisionAsync();
            ViewModel.IsModifiedSubdivision = false;
        }
    }
    private async Task RefreshSubdivisionAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SubdivisionDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSubdivision = App.SingleSubdivisionDivision(ViewModel.SubdivisionSelected.SubdivisionId, ViewModel.DivisionSelected.DivisionId);
                SubdivisionSingles();
            }
        });
    }

    private async void SaveSubdivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubdivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubdivision()
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
                if (await BeforeSaveSubdivision())
                {
                    return;
                }
                await ViewModel.SaveSubdivisionAsync(ViewModel.SubdivisionSelected);
                await BehindSaveSubdivision();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubdivision = true;
                await RevertChangesSubdivisionAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubdivision()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl15SubdivisionsList ??= [];
            ViewModel.Tbl15SubdivisionsList = [.. Context.Tbl15Subdivisions
            .Where(a => a.SubdivisionName == ViewModel.SubdivisionSelected.SubdivisionName &&
                        a.DivisionId == ViewModel.SubdivisionSelected.DivisionId)];
        });
        if (ViewModel.IsNewSubdivision)
        {
            if (ViewModel.Tbl15SubdivisionsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubdivision = true;
        return false;
    }

    private async Task BehindSaveSubdivision()
    {
        var x = (DataGrid)FindName("SubdivisionDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl15SubdivisionsList ??= [];
            ViewModel.Tbl15SubdivisionsList = [.. Context.Tbl15Subdivisions
                .AsNoTracking()
                .Where(com => com.DivisionId == ViewModel.DivisionSelected.DivisionId)];
        });
        ViewModel.RefreshSubdivisionItems();

        if (ViewModel.Tbl15SubdivisionsList.Count == 0)
        {
            SubdivisionsExpander.Visibility = Visibility.Collapsed;
            SubdivisionDataGrid.ItemsSource = null;
        }
        else
        {
            SubdivisionsExpander.Visibility = Visibility.Visible;
            SubdivisionDataGrid.ItemsSource = ViewModel.Tbl15SubdivisionsList;
        }
        SubdivisionDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void SubdivisionName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubdivision_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subdivision Methods] 

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
            //  DetailPanelDivision.Visibility = Visibility.Visible;
            //  DivisionDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("DivisionDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl09DivisionsList!.Count)
            {
                ViewModel.DivisionItems.Clear();
                foreach (var item in ViewModel.Tbl09DivisionsList)
                {
                    ViewModel.DivisionItems.Add(item);
                }

                ViewModel.DivisionSelected = ViewModel.Tbl09DivisionsList[iNdx];
            }
            DivisionDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl09DivisionsList.Count == 0)
                {
                    ViewModel.Tbl09DivisionsList ??= [];
                    ViewModel.Tbl09DivisionsList = [.. Context.Tbl09Divisions
                            .Where(a => a.DivisionId == ViewModel.DivisionSelected.DivisionId)];
                }
                else
                {
                    ViewModel.Tbl09DivisionsList ??= [];
                    ViewModel.Tbl09DivisionsList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl09Divisions
                           .Where(a => a.DivisionId == id)]
                       : [.. Context.Tbl09Divisions
                           .Where(a => _search != null && a.DivisionName.StartsWith(_search))
                         .OrderBy(a => a.DivisionName)];
                }
            });

            DivisionDataGrid.ItemsSource = ViewModel.Tbl09DivisionsList;

            if (ViewModel.IsDeletingDivision)
            {
                if (iNdx == 0 && ViewModel.Tbl09DivisionsList.Count == 0)
                {
                    ViewModel.Tbl09DivisionsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.DivisionSelected = ViewModel.Tbl09DivisionsList[iNdx];
            }
            if (ViewModel.Tbl09DivisionsList is { Count: 0 })
            {
                ViewModel.Tbl09DivisionsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl15SubdivisionsList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        DivisionDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingDivision = false;
    }

    private void BackDetailExpanders()
    {
        DivisionDetailExpander.IsExpanded = false;
        DivisionDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SubdivisionDetailExpander.IsExpanded = false;
        SubdivisionDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

