
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

//  Tbl15SubdivisionListPage.xaml.cs Skriptdatum:  15.06.2024  12:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl15SubdivisionListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl15SubdivisionViewModel ViewModel { get; }
    private Tbl15Subdivision SingleSubdivision { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl18Superclass SingleSuperclass { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl15SubdivisionListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl15SubdivisionViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]   

    #region [Subdivision Methods]

    private async void GetSubdivisionsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl15SubdivisionsList?.Clear();

            ViewModel.SubdivisionItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl15SubdivisionsList?.Clear();

                SubdivisionDataGrid.ItemsSource = ViewModel.Tbl15SubdivisionsList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperclassesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSubdivision = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl15SubdivisionsList ??= [];
                ViewModel.Tbl15SubdivisionsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl15Subdivisions
                .Where(a => a.SubdivisionId == id)]
                : [.. Context.Tbl15Subdivisions
                .Where(a => _search != null && a.SubdivisionName.StartsWith(_search))
                .OrderBy(a => a.SubdivisionName)];
            });

            SubdivisionDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SuperclassesExpander.Visibility = Visibility.Collapsed;
            SuperclassDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SubdivisionDataSetCount = ViewModel.Tbl15SubdivisionsList.Count;

            if (ViewModel.Tbl15SubdivisionsList is { Count: 0 })
            {
                ViewModel.Tbl15SubdivisionsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();

                AppBarButtonDeleteSubdivision.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonAddSuperclass.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSubdivision.Visibility = Visibility.Visible;
                SubdivisionDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperclassesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl15SubdivisionListPage));
            }
            else
            {
                SubdivisionDataGrid.ItemsSource = ViewModel.Tbl15SubdivisionsList;
                ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void SubdivisionDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SubdivisionDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SubdivisionDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SubdivisionSelected == null)
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
            .Where(refe => refe.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
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
            .Where(com => com.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
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
        //--------------------------Superclass-----------------------------    
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl18SuperclassesList ??= [];
            ViewModel.Tbl18SuperclassesList = [.. Context.Tbl18Superclasses
            .AsNoTracking()
            .Where(com => com.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
        });
        ViewModel.SuperclassDataSetCount = ViewModel.Tbl18SuperclassesList.Count;
        ViewModel.RefreshSuperclassItems();

        if (ViewModel.Tbl18SuperclassesList.Count == 0)
        {
            SuperclassesExpander.Visibility = Visibility.Collapsed;
            SuperclassDataGrid.ItemsSource = null;
        }
        else
        {
            SuperclassesExpander.Visibility = Visibility.Visible;
            SuperclassDataGrid.ItemsSource = ViewModel.Tbl18SuperclassesList;
        }
        ViewModel.IsLoading = false;
    }

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

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewSubdivision = false;
        SubdivisionName.PlaceholderText = StringsRes.EnterText;
    }

    private void SubdivisionSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SubdivisionSearchBox.Visibility = Visibility.Visible;
        SubdivisionSearchBox.Focus(FocusState.Programmatic);
        SubdivisionSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SubdivisionDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl15SubdivisionsList.Sort);
    }

    private void AddNewSubdivisionCanceled(object sender, EventArgs e) => SubdivisionDetailExpander.IsExpanded = false;

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
            SingleSubdivision = App.SingleSubdivision(ViewModel.SubdivisionSelected.SubdivisionId);
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
        ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[iNdx];

        SingleSubdivision = App.SingleSubdivision(ViewModel.SubdivisionSelected.SubdivisionId);
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
    private Task RefreshSubdivisionAsync()
    {
        if (SubdivisionDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubdivision = App.SingleSubdivision(ViewModel.SubdivisionSelected.SubdivisionId);
            SubdivisionSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSubdivision_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSubdivision();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSubdivision()
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
                await ViewModel.DeleteSubdivisionAsync(ViewModel.SubdivisionSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSubdivisionAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        _search = SubdivisionSearchBox.Text;
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
            ViewModel.Tbl15SubdivisionsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl15Subdivisions
                    .Where(a => a.SubdivisionId == id)]
                : [.. Context.Tbl15Subdivisions
                    .Where(a => _search != null && a.SubdivisionName.StartsWith(_search))
                    .OrderBy(a => a.SubdivisionName)];
        });

        ViewModel.RefreshSubdivisionItems();

        if (ViewModel.Tbl15SubdivisionsList.Count == 0)
        {
            SubdivisionDataGrid.ItemsSource = null;
        }
        else
        {
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
            SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);
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
                        a.SubdivisionId == ViewModel.ReferenceSelected.SubdivisionId &&
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
                .Where(refe => refe.FamilyId == ViewModel.SubdivisionSelected.SubdivisionId)];
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

        SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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
            SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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

        SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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
            SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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

        SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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
            SingleReference = App.SingleReferenceSubdivision(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubdivisionSelected.SubdivisionId);

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
            SingleComment = App.SingleCommentSubdivision(ViewModel.CommentSelected.CommentId, ViewModel.SubdivisionSelected.SubdivisionId);
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

        SingleComment = App.SingleCommentSubdivision(ViewModel.CommentSelected.CommentId, ViewModel.SubdivisionSelected.SubdivisionId);
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
                SingleComment = App.SingleCommentSubdivision(ViewModel.CommentSelected.CommentId, ViewModel.SubdivisionSelected.SubdivisionId);
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
                        a.SubdivisionId == ViewModel.CommentSelected.SubdivisionId)];
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
                .Where(com => com.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
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

    #region [Superclass Methods]

    private async void AddSuperclass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSuperclass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSuperclass()
    {
        SuperclassName.Loaded += (s, e) =>
        {
            SuperclassName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSuperclass = true;
        ViewModel.IsNewSuperclass = true;

        await ViewModel.AddSuperclassAsync();

        if (ViewModel.Tbl18SuperclassesList != null)
        {
            ViewModel.SuperclassSelected = ViewModel.Tbl18SuperclassesList[0];
        }

        SuperclassDetailExpander.IsExpanded = true;
        SuperclassDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewSuperclass = false;
    }

    private void DataGrid_Superclass_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl18SuperclassesList.Sort);
    }

    private void AddNewSuperclassCanceled(object sender, EventArgs e) => SuperclassesExpander.IsExpanded = false;

    private async void ViewSuperclassHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSuperclassHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSuperclassHyperlink()
    {
        ViewModel.IsInEditSuperclass = false;
        ViewModel.IsNewSuperclass = false;
        SuperclassDetailExpander.IsExpanded = true;
        SuperclassDetailExpander.Visibility = Visibility.Visible;

        if (SuperclassDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSuperclass = App.SingleSuperclassSubdivision(ViewModel.SuperclassSelected.SuperclassId,
                    ViewModel.SubdivisionSelected.SubdivisionId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSuperclassAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSuperclass)
        {
            SuperclassDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelSuperclassEdits();
        }
    }

    private void CancelSuperclassEdits()
    {
        var x = (DataGrid)FindName("SuperclassDataGrid");
        var iNdx = x.SelectedIndex;
        ViewModel.SuperclassSelected = ViewModel.Tbl18SuperclassesList[iNdx];

        SingleSuperclass = App.SingleSuperclassSubdivision(ViewModel.SuperclassSelected.SuperclassId, ViewModel.SubdivisionSelected.SubdivisionId);
        SuperclassSingles();

        ViewModel.IsInEditSuperclass = false;
        ViewModel.IsNewSuperclass = false;
        SuperclassDetailExpander.IsExpanded = true;
        SuperclassDetailExpander.Visibility = Visibility.Visible;
    }

    private void SuperclassSingles()
    {
        ViewModel.SuperclassSelected.SuperclassName = SingleSuperclass.SuperclassName;
        ViewModel.SuperclassSelected.SubdivisionId = SingleSuperclass.SubdivisionId;
        ViewModel.SuperclassSelected.Valid = SingleSuperclass.Valid;
        ViewModel.SuperclassSelected.ValidYear = SingleSuperclass.ValidYear;
        ViewModel.SuperclassSelected.Author = SingleSuperclass.Author;
        ViewModel.SuperclassSelected.AuthorYear = SingleSuperclass.AuthorYear;
        ViewModel.SuperclassSelected.Info = SingleSuperclass.Info;
        ViewModel.SuperclassSelected.Synonym = SingleSuperclass.Synonym;
        ViewModel.SuperclassSelected.EngName = SingleSuperclass.EngName;
        ViewModel.SuperclassSelected.GerName = SingleSuperclass.GerName;
        ViewModel.SuperclassSelected.FraName = SingleSuperclass.FraName;
        ViewModel.SuperclassSelected.PorName = SingleSuperclass.PorName;
        ViewModel.SuperclassSelected.Memo = SingleSuperclass.Memo;
    }

    private async Task RevertChangesSuperclassAsync()
    {
        ViewModel.IsInEditSuperclass = false;
        if (ViewModel.IsModifiedSuperclass)
        {
            await RefreshSuperclassAsync();
            ViewModel.IsModifiedSuperclass = false;
        }
    }
    private async Task RefreshSuperclassAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SuperclassDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSuperclass = App.SingleSuperclassSubdivision(ViewModel.SuperclassSelected.SuperclassId, ViewModel.SubdivisionSelected.SubdivisionId);
                SuperclassSingles();
            }
        });
    }

    private async void SaveSuperclass_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSuperclass();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSuperclass()
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
                if (await BeforeSaveSuperclass())
                {
                    return;
                }
                await ViewModel.SaveSuperclassAsync(ViewModel.SuperclassSelected);
                await BehindSaveSuperclass();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSuperclass = true;
                await RevertChangesSuperclassAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSuperclass()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl18SuperclassesList ??= [];
            ViewModel.Tbl18SuperclassesList = [.. Context.Tbl18Superclasses
            .Where(a => a.SuperclassName == ViewModel.SuperclassSelected.SuperclassName &&
                        a.SubdivisionId == ViewModel.SuperclassSelected.SubdivisionId)];
        });
        if (ViewModel.IsNewSuperclass)
        {
            if (ViewModel.Tbl18SuperclassesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSuperclass = true;
        return false;
    }

    private async Task BehindSaveSuperclass()
    {
        var x = (DataGrid)FindName("SuperclassDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl18SuperclassesList ??= [];
            ViewModel.Tbl18SuperclassesList = [.. Context.Tbl18Superclasses
                .AsNoTracking()
                .Where(com => com.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
        });
        ViewModel.RefreshSuperclassItems();

        if (ViewModel.Tbl18SuperclassesList.Count == 0)
        {
            SuperclassesExpander.Visibility = Visibility.Collapsed;
            SuperclassDataGrid.ItemsSource = null;
        }
        else
        {
            SuperclassesExpander.Visibility = Visibility.Visible;
            SuperclassDataGrid.ItemsSource = ViewModel.Tbl18SuperclassesList;
        }
        SuperclassDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void SuperclassName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSuperclass_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Superclass Methods] 

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
            //  DetailPanelSubdivision.Visibility = Visibility.Visible;
            //  SubdivisionDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SubdivisionDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl15SubdivisionsList!.Count)
            {
                ViewModel.SubdivisionItems.Clear();
                foreach (var item in ViewModel.Tbl15SubdivisionsList)
                {
                    ViewModel.SubdivisionItems.Add(item);
                }

                ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[iNdx];
            }
            SubdivisionDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl15SubdivisionsList.Count == 0)
                {
                    ViewModel.Tbl15SubdivisionsList ??= [];
                    ViewModel.Tbl15SubdivisionsList = [.. Context.Tbl15Subdivisions
                            .Where(a => a.SubdivisionId == ViewModel.SubdivisionSelected.SubdivisionId)];
                }
                else
                {
                    ViewModel.Tbl15SubdivisionsList ??= [];
                    ViewModel.Tbl15SubdivisionsList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl15Subdivisions
                           .Where(a => a.SubdivisionId == id)]
                       : [.. Context.Tbl15Subdivisions
                           .Where(a => _search != null && a.SubdivisionName.StartsWith(_search))
                         .OrderBy(a => a.SubdivisionName)];
                }
            });

            SubdivisionDataGrid.ItemsSource = ViewModel.Tbl15SubdivisionsList;

            if (ViewModel.IsDeletingSubdivision)
            {
                if (iNdx == 0 && ViewModel.Tbl15SubdivisionsList.Count == 0)
                {
                    ViewModel.Tbl15SubdivisionsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SubdivisionSelected = ViewModel.Tbl15SubdivisionsList[iNdx];
            }
            if (ViewModel.Tbl15SubdivisionsList is { Count: 0 })
            {
                ViewModel.Tbl15SubdivisionsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SubdivisionDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSubdivision = false;
    }

    private void BackDetailExpanders()
    {
        SubdivisionDetailExpander.IsExpanded = false;
        SubdivisionDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SuperclassDetailExpander.IsExpanded = false;
        SuperclassDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

