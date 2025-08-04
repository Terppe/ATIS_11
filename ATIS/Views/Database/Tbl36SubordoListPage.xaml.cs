
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

//  Tbl36SubordoListPage.xaml.cs Skriptdatum:  15.06.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl36SubordoListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl36SubordoViewModel ViewModel { get; }
    private Tbl36Subordo SingleSubordo { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl39Infraordo SingleInfraordo { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl36SubordoListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl36SubordoViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Subordo Methods]

    private async void GetSubordosByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl36SubordosList?.Clear();

            ViewModel.SubordoItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl36SubordosList?.Clear();

                SubordoDataGrid.ItemsSource = ViewModel.Tbl36SubordosList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl39InfraordosList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfraordosExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSubordo = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl36SubordosList ??= [];
                ViewModel.Tbl36SubordosList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl36Subordos
                .Where(a => a.SubordoId == id)]
                : [.. Context.Tbl36Subordos
                .Where(a => _search != null && a.SubordoName.StartsWith(_search))
                .OrderBy(a => a.SubordoName)];
            });

            SubordoDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            InfraordosExpander.Visibility = Visibility.Collapsed;
            InfraordoDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SubordoDataSetCount = ViewModel.Tbl36SubordosList.Count;

            if (ViewModel.Tbl36SubordosList is { Count: 0 })
            {
                ViewModel.Tbl36SubordosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl39InfraordosList?.Clear();

                AppBarButtonDeleteSubordo.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddInfraordo.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSubordo.Visibility = Visibility.Visible;
                SubordoDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfraordosExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl36SubordoListPage));
            }
            else
            {
                SubordoDataGrid.ItemsSource = ViewModel.Tbl36SubordosList;
                ViewModel.SubordoSelected = ViewModel.Tbl36SubordosList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void SubordoDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SubordoDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SubordoDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SubordoSelected == null)
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
            .Where(refe => refe.SubordoId == ViewModel.SubordoSelected.SubordoId)];
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
            .Where(com => com.SubordoId == ViewModel.SubordoSelected.SubordoId)];
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
        //--------------------------Infraordo-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl39InfraordosList ??= [];
            ViewModel.Tbl39InfraordosList = [.. Context.Tbl39Infraordos
            .AsNoTracking()
            .Where(com => com.SubordoId == ViewModel.SubordoSelected.SubordoId)];
        });
        ViewModel.InfraordoDataSetCount = ViewModel.Tbl39InfraordosList.Count;
        ViewModel.RefreshInfraordoItems();

        if (ViewModel.Tbl39InfraordosList.Count == 0)
        {
            InfraordosExpander.Visibility = Visibility.Collapsed;
            InfraordoDataGrid.ItemsSource = null;
        }
        else
        {
            InfraordosExpander.Visibility = Visibility.Visible;
            InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSubordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSubordo()
    {
        SubordoName.Loaded += (s, e) =>
        {
            SubordoName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSubordo = true;
        ViewModel.IsNewSubordo = true;

        await ViewModel.AddSubordoAsync();

        if (ViewModel.Tbl36SubordosList != null)
        {
            ViewModel.SubordoSelected = ViewModel.Tbl36SubordosList[0];
        }

        SubordoDetailExpander.IsExpanded = true;
        SubordoDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewSubordo = false;
        SubordoName.PlaceholderText = StringsRes.EnterText;
    }

    private void SubordoSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SubordoSearchBox.Visibility = Visibility.Visible;
        SubordoSearchBox.Focus(FocusState.Programmatic);
        SubordoSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SubordoDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl36SubordosList.Sort);
    }

    private void AddNewSubordoCanceled(object sender, EventArgs e) => SubordoDetailExpander.IsExpanded = false;

    private async void ViewSubordoHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubordoHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubordoHyperlink()
    {
        ViewModel.IsInEditSubordo = false;
        ViewModel.IsNewSubordo = false;
        SubordoDetailExpander.IsExpanded = true;
        SubordoDetailExpander.Visibility = Visibility.Visible;

        if (SubordoDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubordo = App.SingleSubordo(ViewModel.SubordoSelected.SubordoId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSubordoAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubordo)
        {
            SubordoDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelSubordoEdits();
        }
    }

    private void CancelSubordoEdits()
    {
        var x = (DataGrid)FindName("SubordoDataGrid");
        var iNdx = x.SelectedIndex;
        ViewModel.SubordoSelected = ViewModel.Tbl36SubordosList[iNdx];

        SingleSubordo = App.SingleSubordo(ViewModel.SubordoSelected.SubordoId);
        SubordoSingles();

        ViewModel.IsInEditSubordo = false;
        ViewModel.IsNewSubordo = false;
        SubordoDetailExpander.IsExpanded = true;
        SubordoDetailExpander.Visibility = Visibility.Visible;
    }

    private void SubordoSingles()
    {
        ViewModel.SubordoSelected.SubordoName = SingleSubordo.SubordoName;
        ViewModel.SubordoSelected.OrdoId = SingleSubordo.OrdoId;
        ViewModel.SubordoSelected.Valid = SingleSubordo.Valid;
        ViewModel.SubordoSelected.ValidYear = SingleSubordo.ValidYear;
        ViewModel.SubordoSelected.Author = SingleSubordo.Author;
        ViewModel.SubordoSelected.AuthorYear = SingleSubordo.AuthorYear;
        ViewModel.SubordoSelected.Info = SingleSubordo.Info;
        ViewModel.SubordoSelected.Synonym = SingleSubordo.Synonym;
        ViewModel.SubordoSelected.EngName = SingleSubordo.EngName;
        ViewModel.SubordoSelected.GerName = SingleSubordo.GerName;
        ViewModel.SubordoSelected.FraName = SingleSubordo.FraName;
        ViewModel.SubordoSelected.PorName = SingleSubordo.PorName;
        ViewModel.SubordoSelected.Memo = SingleSubordo.Memo;
    }
    private async Task RevertChangesSubordoAsync()
    {
        ViewModel.IsInEditSubordo = false;
        if (ViewModel.IsModifiedSubordo)
        {
            await RefreshSubordoAsync();
            ViewModel.IsModifiedSubordo = false;
        }
    }
    private Task RefreshSubordoAsync()
    {
        if (SubordoDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubordo = App.SingleSubordo(ViewModel.SubordoSelected.SubordoId);
            SubordoSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSubordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSubordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSubordo()
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
                await ViewModel.DeleteSubordoAsync(ViewModel.SubordoSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSubordoAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSubordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubordo()
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
                if (await BeforeSaveSubordo())
                {
                    return;
                }
                await ViewModel.SaveSubordoAsync(ViewModel.SubordoSelected);
                await BehindSaveSubordo();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubordo = true;
                await RevertChangesSubordoAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubordo()
    {
        _search = SubordoSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl36SubordosList ??= [];
            ViewModel.Tbl36SubordosList = [.. Context.Tbl36Subordos
                .Where(a => a.SubordoName == ViewModel.SubordoSelected.SubordoName &&
                        a.OrdoId == ViewModel.SubordoSelected.OrdoId)];
        });
        if (ViewModel.IsNewSubordo)
        {
            if (ViewModel.Tbl36SubordosList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubordo = true;
        return false;
    }

    private async Task BehindSaveSubordo()
    {
        var x = (DataGrid)FindName("SubordoDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl36SubordosList ??= [];
            ViewModel.Tbl36SubordosList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl36Subordos
                    .Where(a => a.SubordoId == id)]
                : [.. Context.Tbl36Subordos
                    .Where(a => _search != null && a.SubordoName.StartsWith(_search))
                    .OrderBy(a => a.SubordoName)];
        });

        ViewModel.RefreshSubordoItems();

        if (ViewModel.Tbl36SubordosList.Count == 0)
        {
            SubordoDataGrid.ItemsSource = null;
        }
        else
        {
            SubordoDataGrid.ItemsSource = ViewModel.Tbl36SubordosList;
        }
        SubordoDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void SubordoName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubordo_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subordo Methods]                    

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
            SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);
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
                        a.SubordoId == ViewModel.ReferenceSelected.SubordoId &&
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
                .Where(refe => refe.FamilyId == ViewModel.SubordoSelected.SubordoId)];
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
        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
            SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
            SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
            SingleReference = App.SingleReferenceSubordo(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubordoSelected.SubordoId);

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
            SingleComment = App.SingleCommentSubordo(ViewModel.CommentSelected.CommentId, ViewModel.SubordoSelected.SubordoId);
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
        ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];

        SingleComment = App.SingleCommentSubordo(ViewModel.CommentSelected.CommentId, ViewModel.SubordoSelected.SubordoId);
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
                SingleComment = App.SingleCommentSubordo(ViewModel.CommentSelected.CommentId, ViewModel.SubordoSelected.SubordoId);
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
                        a.SubordoId == ViewModel.CommentSelected.SubordoId)];
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
                .Where(com => com.SubordoId == ViewModel.SubordoSelected.SubordoId)];
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

    #region [Infraordo Methods]

    private async void AddInfraordo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddInfraordo();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddInfraordo()
    {
        InfraordoName.Loaded += (s, e) =>
        {
            InfraordoName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditInfraordo = true;
        ViewModel.IsNewInfraordo = true;

        await ViewModel.AddInfraordoAsync();

        if (ViewModel.Tbl39InfraordosList != null)
        {
            ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[0];
        }

        InfraordoDetailExpander.IsExpanded = true;
        InfraordoDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewInfraordo = false;
    }

    private void DataGrid_Infraordo_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl39InfraordosList.Sort);
    }

    private void AddNewInfraordoCanceled(object sender, EventArgs e) => InfraordosExpander.IsExpanded = false;

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
            SingleInfraordo = App.SingleInfraordoSubordo(ViewModel.InfraordoSelected.InfraordoId,
                    ViewModel.SubordoSelected.SubordoId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsInfraordoAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewInfraordo)
        {
            InfraordoDetailExpander.IsExpanded = false;
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
        ViewModel.InfraordoSelected = ViewModel.Tbl39InfraordosList[iNdx];

        SingleInfraordo = App.SingleInfraordoSubordo(ViewModel.InfraordoSelected.InfraordoId, ViewModel.SubordoSelected.SubordoId);
        InfraordoSingles();

        ViewModel.IsInEditInfraordo = false;
        ViewModel.IsNewInfraordo = false;
        InfraordoDetailExpander.IsExpanded = true;
        InfraordoDetailExpander.Visibility = Visibility.Visible;
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
    private async Task RefreshInfraordoAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (InfraordoDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleInfraordo = App.SingleInfraordoSubordo(ViewModel.InfraordoSelected.InfraordoId, ViewModel.SubordoSelected.SubordoId);
                InfraordoSingles();
            }
        });
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
                await BehindSaveInfraordo();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfraordo = true;
                await RevertChangesInfraordoAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfraordo()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl39InfraordosList ??= [];
            ViewModel.Tbl39InfraordosList = [.. Context.Tbl39Infraordos
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

    private async Task BehindSaveInfraordo()
    {
        var x = (DataGrid)FindName("InfraordoDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl39InfraordosList ??= [];
            ViewModel.Tbl39InfraordosList = [.. Context.Tbl39Infraordos
                .AsNoTracking()
                .Where(com => com.SubordoId == ViewModel.SubordoSelected.SubordoId)];
        });
        ViewModel.RefreshInfraordoItems();

        if (ViewModel.Tbl39InfraordosList.Count == 0)
        {
            InfraordosExpander.Visibility = Visibility.Collapsed;
            InfraordoDataGrid.ItemsSource = null;
        }
        else
        {
            InfraordosExpander.Visibility = Visibility.Visible;
            InfraordoDataGrid.ItemsSource = ViewModel.Tbl39InfraordosList;
        }
        InfraordoDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
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
            //  DetailPanelSubordo.Visibility = Visibility.Visible;
            //  SubordoDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SubordoDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl36SubordosList!.Count)
            {
                ViewModel.SubordoItems.Clear();
                foreach (var item in ViewModel.Tbl36SubordosList)
                {
                    ViewModel.SubordoItems.Add(item);
                }

                ViewModel.SubordoSelected = ViewModel.Tbl36SubordosList[iNdx];
            }
            SubordoDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl36SubordosList.Count == 0)
                {
                    ViewModel.Tbl36SubordosList ??= [];
                    ViewModel.Tbl36SubordosList = [.. Context.Tbl36Subordos
                            .Where(a => a.SubordoId == ViewModel.SubordoSelected.SubordoId)];
                }
                else
                {
                    ViewModel.Tbl36SubordosList ??= [];
                    ViewModel.Tbl36SubordosList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl36Subordos
                           .Where(a => a.SubordoId == id)]
                       : [.. Context.Tbl36Subordos
                           .Where(a => _search != null && a.SubordoName.StartsWith(_search))
                         .OrderBy(a => a.SubordoName)];
                }
            });

            SubordoDataGrid.ItemsSource = ViewModel.Tbl36SubordosList;

            if (ViewModel.IsDeletingSubordo)
            {
                if (iNdx == 0 && ViewModel.Tbl36SubordosList.Count == 0)
                {
                    ViewModel.Tbl36SubordosList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SubordoSelected = ViewModel.Tbl36SubordosList[iNdx];
            }
            if (ViewModel.Tbl36SubordosList is { Count: 0 })
            {
                ViewModel.Tbl36SubordosList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl39InfraordosList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SubordoDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSubordo = false;
    }

    private void BackDetailExpanders()
    {
        SubordoDetailExpander.IsExpanded = false;
        SubordoDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        InfraordoDetailExpander.IsExpanded = false;
        InfraordoDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

