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

//  Tbl39InfraordoListPage.xaml.cs Skriptdatum:  15.06.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl39InfraordoListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl39InfraordoViewModel ViewModel { get; set; }
    private Tbl39Infraordo SingleInfraordo { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl42Superfamily SingleSuperfamily { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl39InfraordoListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl39InfraordoViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }

    }

    #endregion [Constructor]   

    #region [Infraordo Methods]

    private async void GetInfraordosByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl39InfraordosList?.Clear();

            ViewModel.InfraordoItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl39InfraordosList?.Clear();

                InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl42SuperfamiliesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperfamiliesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingInfraordo = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl39InfraordosList ??= [];
                ViewModel.Tbl39InfraordosList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl39Infraordos
                .Where(a => a.InfraordoId == id)]
                : [.. AppData.Data.Tbl39Infraordos
                .Where(a => _search != null && a.InfraordoName.StartsWith(_search))
                .OrderBy(a => a.InfraordoName)];
            });

            InfraordoDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SuperfamiliesExpander.Visibility = Visibility.Collapsed;
            SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.InfraordoDataSetCount = ViewModel.Tbl39InfraordosList.Count;

            if (ViewModel.Tbl39InfraordosList is { Count: 0 })
            {
                ViewModel.Tbl39InfraordosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl42SuperfamiliesList?.Clear();

                AppBarButtonDeleteInfraordo.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelInfraordo.Visibility = Visibility.Visible;
                InfraordoDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperfamiliesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl39InfraordoListPage));
            }
            else
            {
                InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
                ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[0];
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

    private async void InfraordoDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await InfraordoDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task InfraordoDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.InfraordoSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.InfraordoSelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
            .Where(refe => refe.InfraordoId == ViewModel.InfraordoSelected.InfraordoId)];
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
            .Where(com => com.InfraordoId == ViewModel.InfraordoSelected.InfraordoId)];
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
            //--------------------------Superfamily-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl42SuperfamiliesList ??= [];
                ViewModel.Tbl42SuperfamiliesList = [.. AppData.Data.Tbl42Superfamilies
            .Where(com => com.InfraordoId == ViewModel.InfraordoSelected.InfraordoId)];
            });
            ViewModel.SuperfamilyDataSetCount = ViewModel.Tbl42SuperfamiliesList.Count;
            ViewModel.RefreshSuperfamilyItems();

            if (ViewModel.Tbl42SuperfamiliesList.Count == 0)
            {
                SuperfamiliesExpander.Visibility = Visibility.Collapsed;
                SuperfamilyDataGrid.ItemsSource = null;
            }
            else
            {
                SuperfamiliesExpander.Visibility = Visibility.Visible;
                SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddInfraordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddInfraordo();
            InfraordoName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfraordoName);
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

    private async Task AddInfraordo()
    {
        ViewModel.IsInEditInfraordo = true;
        ViewModel.IsNewInfraordo = true;

        await ViewModel.AddInfraordoAsync();

        if (ViewModel.Tbl39InfraordosList != null)
        {
            ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[0];
        }

        InfraordoDetailExpander.IsExpanded = true;
        InfraordoDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void InfraordoSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        InfraordoSearchBox.Visibility = Visibility.Visible;
        InfraordoSearchBox.Focus(FocusState.Programmatic);
        InfraordoSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void InfraordoDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl39InfraordosList.Sort);
    }

    private async void ViewInfraordoHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewInfraordoHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewInfraordoHyperlink()
    {
        ViewModel.IsInEditInfraordo = false;
        ViewModel.IsNewInfraordo = false;
        InfraordoDetailExpander.IsExpanded = true;
        InfraordoDetailExpander.Visibility = Visibility.Visible;

        if (InfraordoDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleInfraordo = AppData.Data.SingleInfraordo(ViewModel.InfraordoSelected.InfraordoId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsInfraordoAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewInfraordo)
        {
            InfraordoDetailExpander.IsExpanded = false;
            InfraordoDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelInfraordoEdits();
        }
    }

    private void CancelInfraordoEdits()
    {
        var x = (DataGrid)FindName("InfraordoDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[iNdx];
            SingleInfraordo = AppData.Data.SingleInfraordo(ViewModel.InfraordoSelected.InfraordoId);
            InfraordoSingles();
        }
        else
        {
            InfraordoDetailExpander.IsExpanded = false;
            InfraordoDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditInfraordo = false;
        ViewModel.IsNewInfraordo = false;
    }

    private void InfraordoSingles()
    {
        ViewModel.InfraordoSelected.InfraordoName = SingleInfraordo.InfraordoName;
        ViewModel.InfraordoSelected.SubordoId = SingleInfraordo.SubordoId;
        ViewModel.InfraordoSelected.Valid = SingleInfraordo.Valid;
        ViewModel.InfraordoSelected.ValidYear = SingleInfraordo.ValidYear;
        ViewModel.InfraordoSelected.Author = SingleInfraordo.Author;
        ViewModel.InfraordoSelected.AuthorYear = SingleInfraordo.AuthorYear;
        ViewModel.InfraordoSelected.Info = SingleInfraordo.Info;
        ViewModel.InfraordoSelected.Synonym = SingleInfraordo.Synonym;
        ViewModel.InfraordoSelected.EngName = SingleInfraordo.EngName;
        ViewModel.InfraordoSelected.GerName = SingleInfraordo.GerName;
        ViewModel.InfraordoSelected.FraName = SingleInfraordo.FraName;
        ViewModel.InfraordoSelected.PorName = SingleInfraordo.PorName;
        ViewModel.InfraordoSelected.Memo = SingleInfraordo.Memo;
    }

    private async Task RevertChangesInfraordoAsync()
    {
        ViewModel.IsInEditInfraordo = false;
        if (ViewModel.IsModifiedInfraordo)
        {
            await RefreshInfraordoAsync();
            ViewModel.IsModifiedInfraordo = false;
        }
    }

    private Task RefreshInfraordoAsync()
    {
        if (InfraordoDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.InfraordoSelected.InfraordoId == 0)
            {
                return Task.CompletedTask;
            }
            SingleInfraordo = AppData.Data.SingleInfraordo(ViewModel.InfraordoSelected.InfraordoId);
            InfraordoSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteInfraordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteInfraordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteInfraordo()
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
                await ViewModel.DeleteInfraordoAsync(ViewModel.InfraordoSelected);
                await Refresh();
                InfraordoDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesInfraordoAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveInfraordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveInfraordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveInfraordo()
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
                if (await BeforeSaveInfraordo())
                {
                    return;
                }
                await ViewModel.SaveInfraordoAsync(ViewModel.InfraordoSelected);
                await BehindSaveInfraordo(ViewModel.InfraordoSelected);
                await Refresh();
                ViewModel.IsNewInfraordo = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfraordo = true;
                await RevertChangesInfraordoAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfraordo()
    {
        _search = InfraordoSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl39InfraordosList ??= [];
            ViewModel.Tbl39InfraordosList = [.. AppData.Data.Tbl39Infraordos
                .Where(a => a.InfraordoName == ViewModel.InfraordoSelected.InfraordoName &&
                        a.SubordoId == ViewModel.InfraordoSelected.SubordoId)];
        });
        if (ViewModel.IsNewInfraordo)
        {
            if (ViewModel.Tbl39InfraordosList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedInfraordo = true;
        return false;
    }

    private async Task BehindSaveInfraordo(Tbl39Infraordo viewModelInfraordoSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl39InfraordosList ??= [];
            if (InfraordoSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl39InfraordosList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl39Infraordos
                    .Where(a => a.InfraordoId == id)]
                    : [.. AppData.Data.Tbl39Infraordos
                    .Where(a => _search != null && a.InfraordoName.StartsWith(_search))
                    .OrderBy(a => a.InfraordoName)];
            }
        });

        ViewModel.RefreshInfraordoItems();

        if (ViewModel.Tbl39InfraordosList.Count == 0)
        {
            ViewModel.Tbl39InfraordosList = [.. AppData.Data.Tbl39Infraordos
                .Where(a => a.InfraordoId == viewModelInfraordoSelected.InfraordoId)];
            InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
            await Refresh();
            InfraordoDataGrid.SelectedIndex = 0;
        }
        else
        {
            InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
            var index = ViewModel.InfraordoItems.ToList().FindIndex(item => item.InfraordoId == viewModelInfraordoSelected.InfraordoId);
            InfraordoDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.InfraordoDataSetCount = ViewModel.Tbl39InfraordosList.Count;
    }

    private void InfraordoName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackInfraordo_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Infraordo Methods]                    

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
            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.InfraordoSelected.InfraordoId);
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
                        a.InfraordoId == ViewModel.ReferenceSelected.InfraordoId &&
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
                .Where(refe => refe.InfraordoId == viewModelReferenceSelected.InfraordoId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(com => com.InfraordoId == viewModelReferenceSelected.InfraordoId)];
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

            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfraordoSelected.InfraordoId);
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
            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfraordoSelected.InfraordoId);

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

            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.InfraordoSelected.InfraordoId);
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
            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.InfraordoSelected.InfraordoId);

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

            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.InfraordoSelected.InfraordoId);
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
            SingleReference = AppData.Data.SingleReferenceInfraordo(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.InfraordoSelected.InfraordoId);

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
            SingleComment = AppData.Data.SingleCommentInfraordo(ViewModel.CommentSelected.CommentId, ViewModel.InfraordoSelected.InfraordoId);
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

            SingleComment = AppData.Data.SingleCommentInfraordo(ViewModel.CommentSelected.CommentId, ViewModel.InfraordoSelected.InfraordoId);
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
                SingleComment = AppData.Data.SingleCommentInfraordo(ViewModel.CommentSelected.CommentId, ViewModel.InfraordoSelected.InfraordoId);
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
                        a.InfraordoId == ViewModel.CommentSelected.InfraordoId)];
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
                .Where(com => com.InfraordoId == viewModelCommentSelected.InfraordoId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
                .Where(com => com.InfraordoId == viewModelCommentSelected.InfraordoId)];
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

    #region [Superfamily Methods]

    private void DataGrid_Superfamily_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl42SuperfamiliesList.Sort);
    }

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
            SingleSuperfamily = AppData.Data.SingleSuperfamilyInfraordo(ViewModel.SuperfamilySelected.SuperfamilyId,
                    ViewModel.InfraordoSelected.InfraordoId);
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

            SingleSuperfamily = AppData.Data.SingleSuperfamilyInfraordo(ViewModel.SuperfamilySelected.SuperfamilyId, ViewModel.InfraordoSelected.InfraordoId);
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
    private async Task RefreshSuperfamilyAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SuperfamilyDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSuperfamily = AppData.Data.SingleSuperfamilyInfraordo(ViewModel.SuperfamilySelected.SuperfamilyId, ViewModel.InfraordoSelected.InfraordoId);
                SuperfamilySingles();
            }
        });
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
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSuperfamily = true;
                await RevertChangesSuperfamilyAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSuperfamily()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl42SuperfamiliesList ??= [];
            ViewModel.Tbl42SuperfamiliesList = [.. AppData.Data.Tbl42Superfamilies
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
            ViewModel.Tbl42SuperfamiliesList = [.. AppData.Data.Tbl42Superfamilies
                .Where(com => com.InfraordoId == viewModelSuperfamilySelected.InfraordoId)];
        });
        ViewModel.RefreshSuperfamilyItems();

        if (ViewModel.Tbl42SuperfamiliesList.Count == 0)
        {
            ViewModel.Tbl42SuperfamiliesList = [.. AppData.Data.Tbl42Superfamilies
                .Where(com => com.InfraordoId == viewModelSuperfamilySelected.InfraordoId)];
            SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
            await Refresh();
            SuperfamiliesExpander.Visibility = Visibility.Collapsed;
            SuperfamilyDataGrid.ItemsSource = null;
        }
        else
        {
            SuperfamiliesExpander.Visibility = Visibility.Visible;
            SuperfamilyDataGrid.ItemsSource = ViewModel.Tbl42SuperfamiliesList;
            SuperfamilyDataGrid.SelectedIndex = 0;
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
            //  DetailPanelInfraordo.Visibility = Visibility.Visible;
            //  InfraordoDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("InfraordoDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl39InfraordosList!.Count)
            {
                ViewModel.InfraordoItems.Clear();
                foreach (var item in ViewModel.Tbl39InfraordosList)
                {
                    ViewModel.InfraordoItems.Add(item);
                }

                ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[iNdx];
            }
            InfraordoDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl39InfraordosList.Count == 0)
                {
                    ViewModel.Tbl39InfraordosList ??= [];
                    ViewModel.Tbl39InfraordosList = [.. AppData.Data.Tbl39Infraordos
                            .Where(a => a.InfraordoId == ViewModel.InfraordoSelected.InfraordoId)];
                }
                else
                {
                    ViewModel.Tbl39InfraordosList ??= [];
                    ViewModel.Tbl39InfraordosList = int.TryParse(_search, out var id)
                        ? [.. AppData.Data.Tbl39Infraordos
                           .Where(a => a.InfraordoId == id)]
                       : [.. AppData.Data.Tbl39Infraordos
                           .Where(a => _search != null && a.InfraordoName.StartsWith(_search))
                         .OrderBy(a => a.InfraordoName)];
                }
            });

            InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;

            if (ViewModel.IsDeletingInfraordo)
            {
                if (iNdx == 0 && ViewModel.Tbl39InfraordosList.Count == 0)
                {
                    ViewModel.Tbl39InfraordosList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[iNdx];
            }
            if (ViewModel.Tbl39InfraordosList is { Count: 0 })
            {
                ViewModel.Tbl39InfraordosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl42SuperfamiliesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        InfraordoDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingInfraordo = false;
    }

    private void BackDetailExpanders()
    {
        InfraordoDetailExpander.IsExpanded = false;
        InfraordoDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SuperfamilyDetailExpander.IsExpanded = false;
        SuperfamilyDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteInfraordo.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteInfraordo.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteInfraordo.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteInfraordo.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

