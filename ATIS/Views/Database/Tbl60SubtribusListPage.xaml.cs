
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

//  Tbl60SubtribusListPage.xaml.cs Skriptdatum:  30.12.2024  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl60SubtribusListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl60SubtribusViewModel ViewModel { get; }
    private Tbl60Subtribus SingleSubtribus { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl63Infratribus SingleInfratribus { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl60SubtribusListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl60SubtribusViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Subtribus Methods]

    private async void GetSubtribussesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl60SubtribussesList?.Clear();

            ViewModel.SubtribusItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl60SubtribussesList?.Clear();

                SubtribusDataGrid.ItemsSource = ViewModel.Tbl60SubtribussesList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl63InfratribussesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfratribussesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSubtribus = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl60SubtribussesList ??= [];
                ViewModel.Tbl60SubtribussesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl60Subtribusses
                .Where(a => a.SubtribusId == id)]
                : [.. Context.Tbl60Subtribusses
                .Where(a => _search != null && a.SubtribusName.StartsWith(_search))
                .OrderBy(a => a.SubtribusName)];
            });

            SubtribusDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            InfratribussesExpander.Visibility = Visibility.Collapsed;
            InfratribusDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SubtribusDataSetCount = ViewModel.Tbl60SubtribussesList.Count;

            if (ViewModel.Tbl60SubtribussesList is { Count: 0 })
            {
                ViewModel.Tbl60SubtribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl63InfratribussesList?.Clear();

                AppBarButtonDeleteSubtribus.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSubtribus.Visibility = Visibility.Visible;
                SubtribusDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                InfratribussesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl60SubtribusListPage));
            }
            else
            {
                SubtribusDataGrid.ItemsSource = ViewModel.Tbl60SubtribussesList;
                ViewModel.SubtribusSelected = ViewModel.Tbl60SubtribussesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void SubtribusDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SubtribusDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SubtribusDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SubtribusSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SubtribusSelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.SubtribusId == ViewModel.SubtribusSelected.SubtribusId)];
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
            .Where(com => com.SubtribusId == ViewModel.SubtribusSelected.SubtribusId)];
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
            //--------------------------Infratribus-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl63InfratribussesList ??= [];
                ViewModel.Tbl63InfratribussesList = [.. Context.Tbl63Infratribusses
            .Where(com => com.SubtribusId == ViewModel.SubtribusSelected.SubtribusId)];
            });
            ViewModel.InfratribusDataSetCount = ViewModel.Tbl63InfratribussesList.Count;
            ViewModel.RefreshInfratribusItems();

            if (ViewModel.Tbl63InfratribussesList.Count == 0)
            {
                InfratribussesExpander.Visibility = Visibility.Collapsed;
                InfratribusDataGrid.ItemsSource = null;
            }
            else
            {
                InfratribussesExpander.Visibility = Visibility.Visible;
                InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSubtribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubtribus();
            SubtribusName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SubtribusName);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSubtribus()
    {
        ViewModel.IsInEditSubtribus = true;
        ViewModel.IsNewSubtribus = true;

        await ViewModel.AddSubtribusAsync();

        if (ViewModel.Tbl60SubtribussesList != null)
        {
            ViewModel.SubtribusSelected = ViewModel.Tbl60SubtribussesList[0];
        }

        SubtribusDetailExpander.IsExpanded = true;
        SubtribusDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void SubtribusSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SubtribusSearchBox.Visibility = Visibility.Visible;
        SubtribusSearchBox.Focus(FocusState.Programmatic);
        SubtribusSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SubtribusDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl60SubtribussesList.Sort);
    }

    private void AddNewSubtribusCanceled(object sender, EventArgs e) => SubtribusDetailExpander.IsExpanded = false;

    private async void ViewSubtribusHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubtribusHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubtribusHyperlink()
    {
        ViewModel.IsInEditSubtribus = false;
        ViewModel.IsNewSubtribus = false;
        SubtribusDetailExpander.IsExpanded = true;
        SubtribusDetailExpander.Visibility = Visibility.Visible;

        if (SubtribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubtribus = App.SingleSubtribus(ViewModel.SubtribusSelected.SubtribusId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSubtribusAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubtribus)
        {
            SubtribusDetailExpander.IsExpanded = false;
            SubtribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSubtribusEdits();
        }
    }

    private void CancelSubtribusEdits()
    {
        var x = (DataGrid)FindName("SubtribusDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SubtribusSelected = ViewModel.Tbl60SubtribussesList[iNdx];
            SingleSubtribus = App.SingleSubtribus(ViewModel.SubtribusSelected.SubtribusId);
            SubtribusSingles();
        }
        else
        {
            SubtribusDetailExpander.IsExpanded = false;
            SubtribusDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSubtribus = false;
        ViewModel.IsNewSubtribus = false;
    }

    private void SubtribusSingles()
    {
        ViewModel.SubtribusSelected.SubtribusName = SingleSubtribus.SubtribusName;
        ViewModel.SubtribusSelected.TribusId = SingleSubtribus.TribusId;
        ViewModel.SubtribusSelected.Valid = SingleSubtribus.Valid;
        ViewModel.SubtribusSelected.ValidYear = SingleSubtribus.ValidYear;
        ViewModel.SubtribusSelected.Author = SingleSubtribus.Author;
        ViewModel.SubtribusSelected.AuthorYear = SingleSubtribus.AuthorYear;
        ViewModel.SubtribusSelected.Info = SingleSubtribus.Info;
        ViewModel.SubtribusSelected.Synonym = SingleSubtribus.Synonym;
        ViewModel.SubtribusSelected.EngName = SingleSubtribus.EngName;
        ViewModel.SubtribusSelected.GerName = SingleSubtribus.GerName;
        ViewModel.SubtribusSelected.FraName = SingleSubtribus.FraName;
        ViewModel.SubtribusSelected.PorName = SingleSubtribus.PorName;
        ViewModel.SubtribusSelected.Memo = SingleSubtribus.Memo;
    }
    private async Task RevertChangesSubtribusAsync()
    {
        ViewModel.IsInEditSubtribus = false;
        if (ViewModel.IsModifiedSubtribus)
        {
            await RefreshSubtribusAsync();
            ViewModel.IsModifiedSubtribus = false;
        }
    }
    private Task RefreshSubtribusAsync()
    {
        if (SubtribusDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubtribus = App.SingleSubtribus(ViewModel.SubtribusSelected.SubtribusId);
            SubtribusSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSubtribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSubtribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSubtribus()
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
                await ViewModel.DeleteSubtribusAsync(ViewModel.SubtribusSelected);
                await Refresh();
                SubtribusDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSubtribusAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSubtribus_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubtribus();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubtribus()
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
                if (await BeforeSaveSubtribus())
                {
                    return;
                }
                await ViewModel.SaveSubtribusAsync(ViewModel.SubtribusSelected);
                await BehindSaveSubtribus(ViewModel.SubtribusSelected);
                await Refresh();
                ViewModel.IsNewSubtribus = false;
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubtribus = true;
                await RevertChangesSubtribusAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubtribus()
    {
        _search = SubtribusSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl60SubtribussesList ??= [];
            ViewModel.Tbl60SubtribussesList = [.. Context.Tbl60Subtribusses
                .Where(a => a.SubtribusName == ViewModel.SubtribusSelected.SubtribusName &&
                        a.TribusId == ViewModel.SubtribusSelected.TribusId)];
        });
        if (ViewModel.IsNewSubtribus)
        {
            if (ViewModel.Tbl60SubtribussesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubtribus = true;
        return false;
    }

    private async Task BehindSaveSubtribus(Tbl60Subtribus viewModelSubtribusSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl60SubtribussesList ??= [];
            if (SubtribusSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl60SubtribussesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl60Subtribusses
                    .Where(a => a.SubtribusId == id)]
                    : [.. Context.Tbl60Subtribusses
                    .Where(a => _search != null && a.SubtribusName.StartsWith(_search))
                    .OrderBy(a => a.SubtribusName)];
            }
        });

        ViewModel.RefreshSubtribusItems();

        if (ViewModel.Tbl60SubtribussesList.Count == 0)
        {
            ViewModel.Tbl60SubtribussesList = [.. Context.Tbl60Subtribusses
                .Where(a => a.SubtribusId == viewModelSubtribusSelected.SubtribusId)];
            SubtribusDataGrid.ItemsSource = ViewModel.Tbl60SubtribussesList;
            await Refresh();
            SubtribusDataGrid.SelectedIndex = 0;
        }
        else
        {
            SubtribusDataGrid.ItemsSource = ViewModel.Tbl60SubtribussesList;
            var index = ViewModel.SubtribusItems.ToList().FindIndex(item => item.SubtribusId == viewModelSubtribusSelected.SubtribusId);
            SubtribusDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.SubtribusDataSetCount = ViewModel.Tbl60SubtribussesList.Count;
    }

    private void SubtribusName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubtribus_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subtribus Methods]                    

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
            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubtribusSelected.SubtribusId);
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
                        a.SubtribusId == ViewModel.ReferenceSelected.SubtribusId &&
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
                .Where(refe => refe.SubtribusId == viewModelReferenceSelected.SubtribusId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.SubtribusId == viewModelReferenceSelected.SubtribusId)];
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

            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SubtribusSelected.SubtribusId);
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
            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SubtribusSelected.SubtribusId);

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

            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.SubtribusSelected.SubtribusId);
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
            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.SubtribusSelected.SubtribusId);

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

            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.SubtribusSelected.SubtribusId);
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
            SingleReference = App.SingleReferenceSubtribus(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.SubtribusSelected.SubtribusId);

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
            SingleComment = App.SingleCommentSubtribus(ViewModel.CommentSelected.CommentId, ViewModel.SubtribusSelected.SubtribusId);
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

            SingleComment = App.SingleCommentSubtribus(ViewModel.CommentSelected.CommentId, ViewModel.SubtribusSelected.SubtribusId);
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
                SingleComment = App.SingleCommentSubtribus(ViewModel.CommentSelected.CommentId, ViewModel.SubtribusSelected.SubtribusId);
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
                        a.SubtribusId == ViewModel.CommentSelected.SubtribusId)];
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
                .Where(com => com.SubtribusId == viewModelCommentSelected.SubtribusId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.SubtribusId == viewModelCommentSelected.SubtribusId)];
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

    #region [Infratribus Methods]

    private void DataGrid_Infratribus_Sorting(object sender, DataGridColumnEventArgs e)
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
            SingleInfratribus = App.SingleInfratribusSubtribus(ViewModel.InfratribusSelected.InfratribusId,
                    ViewModel.SubtribusSelected.SubtribusId);
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

            SingleInfratribus = App.SingleInfratribusSubtribus(ViewModel.InfratribusSelected.InfratribusId, ViewModel.SubtribusSelected.SubtribusId);
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
    private async Task RefreshInfratribusAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (InfratribusDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleInfratribus = App.SingleInfratribusSubtribus(ViewModel.InfratribusSelected.InfratribusId, ViewModel.SubtribusSelected.SubtribusId);
                InfratribusSingles();
            }
        });
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
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedInfratribus = true;
                await RevertChangesInfratribusAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveInfratribus()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl63InfratribussesList ??= [];
            ViewModel.Tbl63InfratribussesList = [.. Context.Tbl63Infratribusses
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
            ViewModel.Tbl63InfratribussesList = [.. Context.Tbl63Infratribusses
                .Where(com => com.SubtribusId == viewModelInfratribusSelected.SubtribusId)];
        });
        ViewModel.RefreshInfratribusItems();

        if (ViewModel.Tbl63InfratribussesList.Count == 0)
        {
            ViewModel.Tbl63InfratribussesList = [.. Context.Tbl63Infratribusses
                .Where(com => com.SubtribusId == viewModelInfratribusSelected.SubtribusId)];
            InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
            await Refresh();
            InfratribussesExpander.Visibility = Visibility.Collapsed;
            InfratribusDataGrid.ItemsSource = null;
        }
        else
        {
            InfratribussesExpander.Visibility = Visibility.Visible;
            InfratribusDataGrid.ItemsSource = ViewModel.Tbl63InfratribussesList;
            InfratribusDataGrid.SelectedIndex = 0;
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
            //  DetailPanelSubtribus.Visibility = Visibility.Visible;
            //  SubtribusDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SubtribusDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl60SubtribussesList!.Count)
            {
                ViewModel.SubtribusItems.Clear();
                foreach (var item in ViewModel.Tbl60SubtribussesList)
                {
                    ViewModel.SubtribusItems.Add(item);
                }

                ViewModel.SubtribusSelected = ViewModel.Tbl60SubtribussesList[iNdx];
            }
            SubtribusDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl60SubtribussesList.Count == 0)
                {
                    ViewModel.Tbl60SubtribussesList ??= [];
                    ViewModel.Tbl60SubtribussesList = [.. Context.Tbl60Subtribusses
                            .Where(a => a.SubtribusId == ViewModel.SubtribusSelected.SubtribusId)];
                }
                else
                {
                    ViewModel.Tbl60SubtribussesList ??= [];
                    ViewModel.Tbl60SubtribussesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl60Subtribusses
                           .Where(a => a.SubtribusId == id)]
                       : [.. Context.Tbl60Subtribusses
                           .Where(a => _search != null && a.SubtribusName.StartsWith(_search))
                         .OrderBy(a => a.SubtribusName)];
                }
            });

            SubtribusDataGrid.ItemsSource = ViewModel.Tbl60SubtribussesList;

            if (ViewModel.IsDeletingSubtribus)
            {
                if (iNdx == 0 && ViewModel.Tbl60SubtribussesList.Count == 0)
                {
                    ViewModel.Tbl60SubtribussesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SubtribusSelected = ViewModel.Tbl60SubtribussesList[iNdx];
            }
            if (ViewModel.Tbl60SubtribussesList is { Count: 0 })
            {
                ViewModel.Tbl60SubtribussesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl63InfratribussesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SubtribusDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSubtribus = false;
    }

    private void BackDetailExpanders()
    {
        SubtribusDetailExpander.IsExpanded = false;
        SubtribusDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        InfratribusDetailExpander.IsExpanded = false;
        InfratribusDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

