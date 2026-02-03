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

//  Tbl63InfratribusListPage.xaml.cs Skriptdatum:  30.12.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl63InfratribusListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl63InfratribusViewModel ViewModel { get; set; }
    private Tbl63Infratribus SingleInfratribus { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl66Genus SingleGenus { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl63InfratribusListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        //if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl63InfratribusViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        //else
        //{
        //    AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        //}

    }

    #endregion [Constructor]   

    #region [Infratribus Methods]

    private async void GetInfratribussesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl63InfratribussesList?.Clear();

            ViewModel.InfratribusItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl63InfratribussesList?.Clear();

                InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl66GenussesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                GenussesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingInfratribus = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl63InfratribussesList ??= [];
                ViewModel.Tbl63InfratribussesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl63Infratribusses
                .Where(a => a.InfratribusId == id)]
                : [.. AppData.Data.Tbl63Infratribusses
                .Where(a => _search != null && a.InfratribusName.StartsWith(_search))
                .OrderBy(a => a.InfratribusName)];
            });

            InfratribusDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            GenussesExpander.Visibility = Visibility.Collapsed;
            GenusDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.InfratribusDataSetCount = ViewModel.Tbl63InfratribussesList.Count;

            if (ViewModel.Tbl63InfratribussesList is { Count: 0 })
            {
                ViewModel.Tbl63InfratribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl66GenussesList?.Clear();

                AppBarButtonDeleteInfratribus.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelInfratribus.Visibility = Visibility.Visible;
                InfratribusDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                GenussesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl63InfratribusListPage));
            }
            else
            {
                InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
                ViewModel.InfratribusSelected = ViewModel.Tbl63InfratribussesList[0];
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

    private async void InfratribusDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await InfratribusDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task InfratribusDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.InfratribusSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.InfratribusSelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
            .Where(refe => refe.InfratribusId == ViewModel.InfratribusSelected.InfratribusId)];
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
            .Where(com => com.InfratribusId == ViewModel.InfratribusSelected.InfratribusId)];
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
            //--------------------------Genus-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl66GenussesList ??= [];
                ViewModel.Tbl66GenussesList = [.. AppData.Data.Tbl66Genusses
            .Where(com => com.InfratribusId == ViewModel.InfratribusSelected.InfratribusId)];
            });
            ViewModel.GenusDataSetCount = ViewModel.Tbl66GenussesList.Count;
            ViewModel.RefreshGenusItems();

            if (ViewModel.Tbl66GenussesList.Count == 0)
            {
                GenussesExpander.Visibility = Visibility.Collapsed;
                GenusDataGrid.ItemsSource = null;
            }
            else
            {
                GenussesExpander.Visibility = Visibility.Visible;
                GenusDataGrid.ItemsSource = ViewModel.Tbl66GenussesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddInfratribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddInfratribus();
            InfratribusName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(InfratribusName);
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

    private async Task AddInfratribus()
    {
        ViewModel.IsInEditInfratribus = true;
        ViewModel.IsNewInfratribus = true;

        await ViewModel.AddInfratribusAsync();

        if (ViewModel.Tbl63InfratribussesList != null)
        {
            ViewModel.InfratribusSelected = ViewModel.Tbl63InfratribussesList[0];
        }

        InfratribusDetailExpander.IsExpanded = true;
        InfratribusDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void InfratribusSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        InfratribusSearchBox.Visibility = Visibility.Visible;
        InfratribusSearchBox.Focus(FocusState.Programmatic);
        InfratribusSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void InfratribusDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl63InfratribussesList.Sort);
    }

    private async void ViewInfratribusHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewInfratribusHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewInfratribusHyperlink()
    {
        ViewModel.IsInEditInfratribus = false;
        ViewModel.IsNewInfratribus = false;
        InfratribusDetailExpander.IsExpanded = true;
        InfratribusDetailExpander.Visibility = Visibility.Visible;

        if (InfratribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleInfratribus = AppData.Data.SingleInfratribus(ViewModel.InfratribusSelected.InfratribusId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsInfratribusAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewInfratribus)
        {
            InfratribusDetailExpander.IsExpanded = false;
            InfratribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelInfratribusEdits();
        }
    }

    private void CancelInfratribusEdits()
    {
        var x = (DataGrid)FindName("InfratribusDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.InfratribusSelected = ViewModel.Tbl63InfratribussesList[iNdx];
            SingleInfratribus = AppData.Data.SingleInfratribus(ViewModel.InfratribusSelected.InfratribusId);
            InfratribusSingles();
        }
        else
        {
            InfratribusDetailExpander.IsExpanded = false;
            InfratribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditInfratribus = false;
        ViewModel.IsNewInfratribus = false;
    }

    private void InfratribusSingles()
    {
        ViewModel.InfratribusSelected.InfratribusName = SingleInfratribus.InfratribusName;
        ViewModel.InfratribusSelected.SubtribusId = SingleInfratribus.SubtribusId;
        ViewModel.InfratribusSelected.Valid = SingleInfratribus.Valid;
        ViewModel.InfratribusSelected.ValidYear = SingleInfratribus.ValidYear;
        ViewModel.InfratribusSelected.Author = SingleInfratribus.Author;
        ViewModel.InfratribusSelected.AuthorYear = SingleInfratribus.AuthorYear;
        ViewModel.InfratribusSelected.Info = SingleInfratribus.Info;
        ViewModel.InfratribusSelected.Synonym = SingleInfratribus.Synonym;
        ViewModel.InfratribusSelected.EngName = SingleInfratribus.EngName;
        ViewModel.InfratribusSelected.GerName = SingleInfratribus.GerName;
        ViewModel.InfratribusSelected.FraName = SingleInfratribus.FraName;
        ViewModel.InfratribusSelected.PorName = SingleInfratribus.PorName;
        ViewModel.InfratribusSelected.Memo = SingleInfratribus.Memo;
    }

    private async Task RevertChangesInfratribusAsync()
    {
        ViewModel.IsInEditInfratribus = false;
        if (ViewModel.IsModifiedInfratribus)
        {
            await RefreshInfratribusAsync();
            ViewModel.IsModifiedInfratribus = false;
        }
    }

    private Task RefreshInfratribusAsync()
    {
        if (InfratribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.InfratribusSelected.InfratribusId == 0)
            {
                return Task.CompletedTask;
            }
            SingleInfratribus = AppData.Data.SingleInfratribus(ViewModel.InfratribusSelected.InfratribusId);
            InfratribusSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteInfratribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteInfratribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteInfratribus()
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
                await ViewModel.DeleteInfratribusAsync(ViewModel.InfratribusSelected);
                await Refresh();
                InfratribusDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesInfratribusAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveInfratribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveInfratribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveInfratribus()
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
                if (await BeforeSaveInfratribus())
                {
                    return;
                }
                await ViewModel.SaveInfratribusAsync(ViewModel.InfratribusSelected);
                await BehindSaveInfratribus(ViewModel.InfratribusSelected);
                await Refresh();
                ViewModel.IsNewInfratribus = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfratribus = true;
                await RevertChangesInfratribusAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfratribus()
    {
        _search = InfratribusSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl63InfratribussesList ??= [];
            ViewModel.Tbl63InfratribussesList = [.. AppData.Data.Tbl63Infratribusses
                .Where(a => a.InfratribusName == ViewModel.InfratribusSelected.InfratribusName &&
                        a.SubtribusId == ViewModel.InfratribusSelected.SubtribusId)];
        });
        if (ViewModel.IsNewInfratribus)
        {
            if (ViewModel.Tbl63InfratribussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedInfratribus = true;
        return false;
    }

    private async Task BehindSaveInfratribus(Tbl63Infratribus viewModelInfratribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl63InfratribussesList ??= [];
            if (InfratribusSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl63InfratribussesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl63Infratribusses
                    .Where(a => a.InfratribusId == id)]
                    : [.. AppData.Data.Tbl63Infratribusses
                    .Where(a => _search != null && a.InfratribusName.StartsWith(_search))
                    .OrderBy(a => a.InfratribusName)];
            }
        });

        ViewModel.RefreshInfratribusItems();

        if (ViewModel.Tbl63InfratribussesList.Count == 0)
        {
            ViewModel.Tbl63InfratribussesList = [.. AppData.Data.Tbl63Infratribusses
                .Where(a => a.InfratribusId == viewModelInfratribusSelected.InfratribusId)];
            InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
            await Refresh();
            InfratribusDataGrid.SelectedIndex = 0;
        }
        else
        {
            InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
            var index = ViewModel.InfratribusItems.ToList().FindIndex(item => item.InfratribusId == viewModelInfratribusSelected.InfratribusId);
            InfratribusDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.InfratribusDataSetCount = ViewModel.Tbl63InfratribussesList.Count;
    }

    private void InfratribusName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackInfratribus_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Infratribus Methods]                    

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
            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId, ViewModel.InfratribusSelected.InfratribusId);
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
                        a.InfratribusId == ViewModel.ReferenceSelected.InfratribusId &&
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
                .Where(refe => refe.InfratribusId == viewModelReferenceSelected.InfratribusId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. AppData.Data.Tbl90References
                .Where(com => com.InfratribusId == viewModelReferenceSelected.InfratribusId)];
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

            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfratribusSelected.InfratribusId);
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
            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.InfratribusSelected.InfratribusId);

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

            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.InfratribusSelected.InfratribusId);
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
            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.InfratribusSelected.InfratribusId);

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

            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.InfratribusSelected.InfratribusId);
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
            SingleReference = AppData.Data.SingleReferenceInfratribus(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.InfratribusSelected.InfratribusId);

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

    //private void AddNewCommentCanceled(object sender, EventArgs e) => CommentsExpander.IsExpanded = false;

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
            SingleComment = AppData.Data.SingleCommentInfratribus(ViewModel.CommentSelected.CommentId, ViewModel.InfratribusSelected.InfratribusId);
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

            SingleComment = AppData.Data.SingleCommentInfratribus(ViewModel.CommentSelected.CommentId, ViewModel.InfratribusSelected.InfratribusId);
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
                SingleComment = AppData.Data.SingleCommentInfratribus(ViewModel.CommentSelected.CommentId, ViewModel.InfratribusSelected.InfratribusId);
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
                        a.InfratribusId == ViewModel.CommentSelected.InfratribusId)];
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
                .Where(com => com.InfratribusId == viewModelCommentSelected.InfratribusId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. AppData.Data.Tbl93Comments
                .Where(com => com.InfratribusId == viewModelCommentSelected.InfratribusId)];
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

    #region [Genus Methods]

    private void DataGrid_Genus_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl66GenussesList.Sort);
    }

    private async void ViewGenusHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewGenusHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewGenusHyperlink()
    {
        ViewModel.IsInEditGenus = false;
        ViewModel.IsNewGenus = false;
        GenusDetailExpander.IsExpanded = true;
        GenusDetailExpander.Visibility = Visibility.Visible;

        if (GenusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleGenus = AppData.Data.SingleGenusInfratribus(ViewModel.GenusSelected.GenusId,
                    ViewModel.InfratribusSelected.InfratribusId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsGenusAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewGenus)
        {
            GenusDetailExpander.IsExpanded = false;
            GenusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelGenusEdits();
        }
    }

    private void CancelGenusEdits()
    {
        var x = (DataGrid)FindName("GenusDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.GenusSelected = ViewModel.Tbl66GenussesList[iNdx];

            SingleGenus = AppData.Data.SingleGenusInfratribus(ViewModel.GenusSelected.GenusId, ViewModel.InfratribusSelected.InfratribusId);
            GenusSingles();
        }
        else
        {
            GenusDetailExpander.IsExpanded = false;
            GenusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditGenus = false;
        ViewModel.IsNewGenus = false;
    }

    private void GenusSingles()
    {
        ViewModel.GenusSelected.GenusName = SingleGenus.GenusName;
        ViewModel.GenusSelected.InfratribusId = SingleGenus.InfratribusId;
        ViewModel.GenusSelected.Valid = SingleGenus.Valid;
        ViewModel.GenusSelected.ValidYear = SingleGenus.ValidYear;
        ViewModel.GenusSelected.Author = SingleGenus.Author;
        ViewModel.GenusSelected.AuthorYear = SingleGenus.AuthorYear;
        ViewModel.GenusSelected.Info = SingleGenus.Info;
        ViewModel.GenusSelected.Synonym = SingleGenus.Synonym;
        ViewModel.GenusSelected.EngName = SingleGenus.EngName;
        ViewModel.GenusSelected.GerName = SingleGenus.GerName;
        ViewModel.GenusSelected.FraName = SingleGenus.FraName;
        ViewModel.GenusSelected.PorName = SingleGenus.PorName;
        ViewModel.GenusSelected.Memo = SingleGenus.Memo;
    }

    private async Task RevertChangesGenusAsync()
    {
        ViewModel.IsInEditGenus = false;
        if (ViewModel.IsModifiedGenus)
        {
            await RefreshGenusAsync();
            ViewModel.IsModifiedGenus = false;
        }
    }

    private async Task RefreshGenusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (GenusDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleGenus = AppData.Data.SingleGenusInfratribus(ViewModel.GenusSelected.GenusId, ViewModel.InfratribusSelected.InfratribusId);
                GenusSingles();
            }
        });
    }

    private async void SaveGenus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveGenus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveGenus()
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
                if (await BeforeSaveGenus())
                {
                    return;
                }
                await ViewModel.SaveGenusAsync(ViewModel.GenusSelected);
                await BehindSaveGenus(ViewModel.GenusSelected);
                await Refresh();
                ViewModel.IsNewGenus = false;
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedGenus = true;
                await RevertChangesGenusAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveGenus()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl66GenussesList ??= [];
            ViewModel.Tbl66GenussesList = [.. AppData.Data.Tbl66Genusses
            .Where(a => a.GenusName == ViewModel.GenusSelected.GenusName &&
                        a.InfratribusId == ViewModel.GenusSelected.InfratribusId)];
        });
        if (ViewModel.IsNewGenus)
        {
            if (ViewModel.Tbl66GenussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedGenus = true;
        return false;
    }

    private async Task BehindSaveGenus(Tbl66Genus viewModelGenusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl66GenussesList ??= [];
            ViewModel.Tbl66GenussesList = [.. AppData.Data.Tbl66Genusses
                .Where(com => com.InfratribusId == viewModelGenusSelected.InfratribusId)];
        });
        ViewModel.RefreshGenusItems();

        if (ViewModel.Tbl66GenussesList.Count == 0)
        {
            ViewModel.Tbl66GenussesList = [.. AppData.Data.Tbl66Genusses
                .Where(com => com.InfratribusId == viewModelGenusSelected.InfratribusId)];
            GenusDataGrid.ItemsSource = ViewModel.Tbl66GenussesList;
            await Refresh();
            GenussesExpander.Visibility = Visibility.Collapsed;
            GenusDataGrid.ItemsSource = null;
        }
        else
        {
            GenussesExpander.Visibility = Visibility.Visible;
            GenusDataGrid.ItemsSource = ViewModel.Tbl66GenussesList;
            GenusDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.GenusDataSetCount = ViewModel.Tbl66GenussesList.Count;
    }

    private void GenusName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackGenus_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Genus Methods] 

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
            //  DetailPanelInfratribus.Visibility = Visibility.Visible;
            //  InfratribusDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("InfratribusDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl63InfratribussesList!.Count)
            {
                ViewModel.InfratribusItems.Clear();
                foreach (var item in ViewModel.Tbl63InfratribussesList)
                {
                    ViewModel.InfratribusItems.Add(item);
                }

                ViewModel.InfratribusSelected = ViewModel.Tbl63InfratribussesList[iNdx];
            }
            InfratribusDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl63InfratribussesList.Count == 0)
                {
                    ViewModel.Tbl63InfratribussesList ??= [];
                    ViewModel.Tbl63InfratribussesList = [.. AppData.Data.Tbl63Infratribusses
                            .Where(a => a.InfratribusId == ViewModel.InfratribusSelected.InfratribusId)];
                }
                else
                {
                    ViewModel.Tbl63InfratribussesList ??= [];
                    ViewModel.Tbl63InfratribussesList = int.TryParse(_search, out var id)
                        ? [.. AppData.Data.Tbl63Infratribusses
                           .Where(a => a.InfratribusId == id)]
                       : [.. AppData.Data.Tbl63Infratribusses
                           .Where(a => _search != null && a.InfratribusName.StartsWith(_search))
                         .OrderBy(a => a.InfratribusName)];
                }
            });

            InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;

            if (ViewModel.IsDeletingInfratribus)
            {
                if (iNdx == 0 && ViewModel.Tbl63InfratribussesList.Count == 0)
                {
                    ViewModel.Tbl63InfratribussesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.InfratribusSelected = ViewModel.Tbl63InfratribussesList[iNdx];
            }
            if (ViewModel.Tbl63InfratribussesList is { Count: 0 })
            {
                ViewModel.Tbl63InfratribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl66GenussesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        InfratribusDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingInfratribus = false;
    }

    private void BackDetailExpanders()
    {
        InfratribusDetailExpander.IsExpanded = false;
        InfratribusDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        GenusDetailExpander.IsExpanded = false;
        GenusDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteInfratribus.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteInfratribus.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteInfratribus.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteInfratribus.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

