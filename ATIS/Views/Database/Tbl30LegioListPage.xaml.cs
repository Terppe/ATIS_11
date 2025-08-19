
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

//  Tbl30LegioListPage.xaml.cs Skriptdatum:  15.06.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl30LegioListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl30LegioViewModel ViewModel { get; }
    private Tbl30Legio SingleLegio { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl33Ordo SingleOrdo { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl30LegioListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl30LegioViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Legio Methods]

    private async void GetLegiosByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl30LegiosList?.Clear();

            ViewModel.LegioItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl30LegiosList?.Clear();

                LegioDataGrid.ItemsSource = ViewModel.Tbl30LegiosList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl33OrdosList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                OrdosExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingLegio = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl30LegiosList ??= [];
                ViewModel.Tbl30LegiosList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl30Legios
                .Where(a => a.LegioId == id)]
                : [.. Context.Tbl30Legios
                .Where(a => _search != null && a.LegioName.StartsWith(_search))
                .OrderBy(a => a.LegioName)];
            });

            LegioDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            OrdosExpander.Visibility = Visibility.Collapsed;
            OrdoDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.LegioDataSetCount = ViewModel.Tbl30LegiosList.Count;

            if (ViewModel.Tbl30LegiosList is { Count: 0 })
            {
                ViewModel.Tbl30LegiosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl33OrdosList?.Clear();

                AppBarButtonDeleteLegio.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddOrdo.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelLegio.Visibility = Visibility.Visible;
                LegioDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                OrdosExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl30LegioListPage));
            }
            else
            {
                LegioDataGrid.ItemsSource = ViewModel.Tbl30LegiosList;
                ViewModel.LegioSelected = ViewModel.Tbl30LegiosList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void LegioDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await LegioDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task LegioDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.LegioSelected == null)
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
            .Where(refe => refe.LegioId == ViewModel.LegioSelected.LegioId)];
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
            .Where(com => com.LegioId == ViewModel.LegioSelected.LegioId)];
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
        //--------------------------Ordo-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl33OrdosList ??= [];
            ViewModel.Tbl33OrdosList = [.. Context.Tbl33Ordos
            .AsNoTracking()
            .Where(com => com.LegioId == ViewModel.LegioSelected.LegioId)];
        });
        ViewModel.OrdoDataSetCount = ViewModel.Tbl33OrdosList.Count;
        ViewModel.RefreshOrdoItems();

        if (ViewModel.Tbl33OrdosList.Count == 0)
        {
            OrdosExpander.Visibility = Visibility.Collapsed;
            OrdoDataGrid.ItemsSource = null;
        }
        else
        {
            OrdosExpander.Visibility = Visibility.Visible;
            OrdoDataGrid.ItemsSource = ViewModel.Tbl33OrdosList;
        }
        ViewModel.IsLoading = false;
    }

    private async void AddLegio_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddLegio();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddLegio()
    {
        LegioName.Loaded += (s, e) =>
        {
            LegioName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditLegio = true;
        ViewModel.IsNewLegio = true;

        await ViewModel.AddLegioAsync();

        if (ViewModel.Tbl30LegiosList != null)
        {
            ViewModel.LegioSelected = ViewModel.Tbl30LegiosList[0];
        }

        LegioDetailExpander.IsExpanded = true;
        LegioDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewLegio = false;
        LegioName.PlaceholderText = StringsRes.EnterText;
    }

    private void LegioSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        LegioSearchBox.Visibility = Visibility.Visible;
        LegioSearchBox.Focus(FocusState.Programmatic);
        LegioSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void LegioDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl30LegiosList.Sort);
    }

    private void AddNewLegioCanceled(object sender, EventArgs e) => LegioDetailExpander.IsExpanded = false;

    private async void ViewLegioHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewLegioHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewLegioHyperlink()
    {
        ViewModel.IsInEditLegio = false;
        ViewModel.IsNewLegio = false;
        LegioDetailExpander.IsExpanded = true;
        LegioDetailExpander.Visibility = Visibility.Visible;

        if (LegioDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleLegio = App.SingleLegio(ViewModel.LegioSelected.LegioId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsLegioAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewLegio)
        {
            LegioDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelLegioEdits();
        }
    }

    private void CancelLegioEdits()
    {
        var x = (DataGrid)FindName("LegioDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditLegio = false;
            ViewModel.IsNewLegio = false;
            LegioDetailExpander.IsExpanded = false;
            LegioDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.LegioSelected = ViewModel.Tbl30LegiosList[iNdx];

        SingleLegio = App.SingleLegio(ViewModel.LegioSelected.LegioId);
        LegioSingles();

        ViewModel.IsInEditLegio = false;
        ViewModel.IsNewLegio = false;
        LegioDetailExpander.IsExpanded = true;
        LegioDetailExpander.Visibility = Visibility.Visible;
    }

    private void LegioSingles()
    {
        ViewModel.LegioSelected.LegioName = SingleLegio.LegioName;
        ViewModel.LegioSelected.InfraclassId = SingleLegio.InfraclassId;
        ViewModel.LegioSelected.Valid = SingleLegio.Valid;
        ViewModel.LegioSelected.ValidYear = SingleLegio.ValidYear;
        ViewModel.LegioSelected.Author = SingleLegio.Author;
        ViewModel.LegioSelected.AuthorYear = SingleLegio.AuthorYear;
        ViewModel.LegioSelected.Info = SingleLegio.Info;
        ViewModel.LegioSelected.Synonym = SingleLegio.Synonym;
        ViewModel.LegioSelected.EngName = SingleLegio.EngName;
        ViewModel.LegioSelected.GerName = SingleLegio.GerName;
        ViewModel.LegioSelected.FraName = SingleLegio.FraName;
        ViewModel.LegioSelected.PorName = SingleLegio.PorName;
        ViewModel.LegioSelected.Memo = SingleLegio.Memo;
    }
    private async Task RevertChangesLegioAsync()
    {
        ViewModel.IsInEditLegio = false;
        if (ViewModel.IsModifiedLegio)
        {
            await RefreshLegioAsync();
            ViewModel.IsModifiedLegio = false;
        }
    }
    private Task RefreshLegioAsync()
    {
        if (LegioDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleLegio = App.SingleLegio(ViewModel.LegioSelected.LegioId);
            LegioSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteLegio_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteLegio();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteLegio()
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
                await ViewModel.DeleteLegioAsync(ViewModel.LegioSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesLegioAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveLegio_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveLegio();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveLegio()
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
                if (await BeforeSaveLegio())
                {
                    return;
                }
                await ViewModel.SaveLegioAsync(ViewModel.LegioSelected);
                await BehindSaveLegio();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedLegio = true;
                await RevertChangesLegioAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveLegio()
    {
        _search = LegioSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl30LegiosList ??= [];
            ViewModel.Tbl30LegiosList = [.. Context.Tbl30Legios
                .Where(a => a.LegioName == ViewModel.LegioSelected.LegioName &&
                        a.InfraclassId == ViewModel.LegioSelected.InfraclassId)];
        });
        if (ViewModel.IsNewLegio)
        {
            if (ViewModel.Tbl30LegiosList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedLegio = true;
        return false;
    }

    private async Task BehindSaveLegio()
    {
        var x = (DataGrid)FindName("LegioDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl30LegiosList ??= [];
            ViewModel.Tbl30LegiosList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl30Legios
                    .Where(a => a.LegioId == id)]
                : [.. Context.Tbl30Legios
                    .Where(a => _search != null && a.LegioName.StartsWith(_search))
                    .OrderBy(a => a.LegioName)];
        });

        ViewModel.RefreshLegioItems();

        if (ViewModel.Tbl30LegiosList.Count == 0)
        {
            LegioDataGrid.ItemsSource = null;
        }
        else
        {
            LegioDataGrid.ItemsSource = ViewModel.Tbl30LegiosList;
        }
        LegioDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void LegioName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackLegio_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Legio Methods]                    

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
            SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);
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
                        a.LegioId == ViewModel.ReferenceSelected.LegioId &&
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
                .Where(refe => refe.FamilyId == ViewModel.LegioSelected.LegioId)];
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

        SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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
            SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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

        SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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
            SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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

        SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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
            SingleReference = App.SingleReferenceLegio(ViewModel.ReferenceSelected.ReferenceId, ViewModel.LegioSelected.LegioId);

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
            SingleComment = App.SingleCommentLegio(ViewModel.CommentSelected.CommentId, ViewModel.LegioSelected.LegioId);
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

        SingleComment = App.SingleCommentLegio(ViewModel.CommentSelected.CommentId, ViewModel.LegioSelected.LegioId);
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
                SingleComment = App.SingleCommentLegio(ViewModel.CommentSelected.CommentId, ViewModel.LegioSelected.LegioId);
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
                        a.LegioId == ViewModel.CommentSelected.LegioId)];
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
                .Where(com => com.LegioId == ViewModel.LegioSelected.LegioId)];
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

    #region [Ordo Methods]

    private async void AddOrdo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddOrdo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddOrdo()
    {
        OrdoName.Loaded += (s, e) =>
        {
            OrdoName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditOrdo = true;
        ViewModel.IsNewOrdo = true;

        await ViewModel.AddOrdoAsync();

        if (ViewModel.Tbl33OrdosList != null)
        {
            ViewModel.OrdoSelected = ViewModel.Tbl33OrdosList[0];
        }

        OrdoDetailExpander.IsExpanded = true;
        OrdoDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewOrdo = false;
    }

    private void DataGrid_Ordo_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl33OrdosList.Sort);
    }

    private void AddNewOrdoCanceled(object sender, EventArgs e) => OrdosExpander.IsExpanded = false;

    private async void ViewOrdoHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewOrdoHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewOrdoHyperlink()
    {
        ViewModel.IsInEditOrdo = false;
        ViewModel.IsNewOrdo = false;
        OrdoDetailExpander.IsExpanded = true;
        OrdoDetailExpander.Visibility = Visibility.Visible;

        if (OrdoDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleOrdo = App.SingleOrdoLegio(ViewModel.OrdoSelected.OrdoId,
                    ViewModel.LegioSelected.LegioId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsOrdoAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewOrdo)
        {
            OrdoDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelOrdoEdits();
        }
    }

    private void CancelOrdoEdits()
    {
        var x = (DataGrid)FindName("OrdoDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditOrdo = false;
            ViewModel.IsNewOrdo = false;
            OrdoDetailExpander.IsExpanded = false;
            OrdoDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }
        ViewModel.OrdoSelected = ViewModel.Tbl33OrdosList[iNdx];

        SingleOrdo = App.SingleOrdoLegio(ViewModel.OrdoSelected.OrdoId, ViewModel.LegioSelected.LegioId);
        OrdoSingles();

        ViewModel.IsInEditOrdo = false;
        ViewModel.IsNewOrdo = false;
        OrdoDetailExpander.IsExpanded = true;
        OrdoDetailExpander.Visibility = Visibility.Visible;
    }

    private void OrdoSingles()
    {
        ViewModel.OrdoSelected.OrdoName = SingleOrdo.OrdoName;
        ViewModel.OrdoSelected.LegioId = SingleOrdo.LegioId;
        ViewModel.OrdoSelected.Valid = SingleOrdo.Valid;
        ViewModel.OrdoSelected.ValidYear = SingleOrdo.ValidYear;
        ViewModel.OrdoSelected.Author = SingleOrdo.Author;
        ViewModel.OrdoSelected.AuthorYear = SingleOrdo.AuthorYear;
        ViewModel.OrdoSelected.Info = SingleOrdo.Info;
        ViewModel.OrdoSelected.Synonym = SingleOrdo.Synonym;
        ViewModel.OrdoSelected.EngName = SingleOrdo.EngName;
        ViewModel.OrdoSelected.GerName = SingleOrdo.GerName;
        ViewModel.OrdoSelected.FraName = SingleOrdo.FraName;
        ViewModel.OrdoSelected.PorName = SingleOrdo.PorName;
        ViewModel.OrdoSelected.Memo = SingleOrdo.Memo;
    }

    private async Task RevertChangesOrdoAsync()
    {
        ViewModel.IsInEditOrdo = false;
        if (ViewModel.IsModifiedOrdo)
        {
            await RefreshOrdoAsync();
            ViewModel.IsModifiedOrdo = false;
        }
    }
    private async Task RefreshOrdoAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (OrdoDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleOrdo = App.SingleOrdoLegio(ViewModel.OrdoSelected.OrdoId, ViewModel.LegioSelected.LegioId);
                OrdoSingles();
            }
        });
    }

    private async void SaveOrdo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveOrdo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveOrdo()
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
                if (await BeforeSaveOrdo())
                {
                    return;
                }
                await ViewModel.SaveOrdoAsync(ViewModel.OrdoSelected);
                await BehindSaveOrdo();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedOrdo = true;
                await RevertChangesOrdoAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveOrdo()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl33OrdosList ??= [];
            ViewModel.Tbl33OrdosList = [.. Context.Tbl33Ordos
            .Where(a => a.OrdoName == ViewModel.OrdoSelected.OrdoName &&
                        a.LegioId == ViewModel.OrdoSelected.LegioId)];
        });
        if (ViewModel.IsNewOrdo)
        {
            if (ViewModel.Tbl33OrdosList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedOrdo = true;
        return false;
    }

    private async Task BehindSaveOrdo()
    {
        var x = (DataGrid)FindName("OrdoDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl33OrdosList ??= [];
            ViewModel.Tbl33OrdosList = [.. Context.Tbl33Ordos
                .AsNoTracking()
                .Where(com => com.LegioId == ViewModel.LegioSelected.LegioId)];
        });
        ViewModel.RefreshOrdoItems();

        if (ViewModel.Tbl33OrdosList.Count == 0)
        {
            OrdosExpander.Visibility = Visibility.Collapsed;
            OrdoDataGrid.ItemsSource = null;
        }
        else
        {
            OrdosExpander.Visibility = Visibility.Visible;
            OrdoDataGrid.ItemsSource = ViewModel.Tbl33OrdosList;
        }
        OrdoDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void OrdoName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackOrdo_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Ordo Methods] 

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
            //  DetailPanelLegio.Visibility = Visibility.Visible;
            //  LegioDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("LegioDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl30LegiosList!.Count)
            {
                ViewModel.LegioItems.Clear();
                foreach (var item in ViewModel.Tbl30LegiosList)
                {
                    ViewModel.LegioItems.Add(item);
                }

                ViewModel.LegioSelected = ViewModel.Tbl30LegiosList[iNdx];
            }
            LegioDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl30LegiosList.Count == 0)
                {
                    ViewModel.Tbl30LegiosList ??= [];
                    ViewModel.Tbl30LegiosList = [.. Context.Tbl30Legios
                            .Where(a => a.LegioId == ViewModel.LegioSelected.LegioId)];
                }
                else
                {
                    ViewModel.Tbl30LegiosList ??= [];
                    ViewModel.Tbl30LegiosList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl30Legios
                           .Where(a => a.LegioId == id)]
                       : [.. Context.Tbl30Legios
                           .Where(a => _search != null && a.LegioName.StartsWith(_search))
                         .OrderBy(a => a.LegioName)];
                }
            });

            LegioDataGrid.ItemsSource = ViewModel.Tbl30LegiosList;

            if (ViewModel.IsDeletingLegio)
            {
                if (iNdx == 0 && ViewModel.Tbl30LegiosList.Count == 0)
                {
                    ViewModel.Tbl30LegiosList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.LegioSelected = ViewModel.Tbl30LegiosList[iNdx];
            }
            if (ViewModel.Tbl30LegiosList is { Count: 0 })
            {
                ViewModel.Tbl30LegiosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl33OrdosList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        LegioDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingLegio = false;
    }

    private void BackDetailExpanders()
    {
        LegioDetailExpander.IsExpanded = false;
        LegioDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        OrdoDetailExpander.IsExpanded = false;
        OrdoDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

