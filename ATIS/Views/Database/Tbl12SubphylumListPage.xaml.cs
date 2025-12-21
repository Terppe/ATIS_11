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

//  Tbl12SubphylumListPage.xaml.cs Skriptdatum:  15.06.2024  12:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl12SubphylumListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl12SubphylumViewModel ViewModel { get; }
    private Tbl12Subphylum SingleSubphylum { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl18Superclass SingleSuperclass { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl12SubphylumListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl12SubphylumViewModel>();
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

    #region [Subphylum Methods]

    private async void GetSubphylumsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl12SubphylumsList?.Clear();

            ViewModel.SubphylumItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl12SubphylumsList?.Clear();

                SubphylumDataGrid.ItemsSource = ViewModel.Tbl12SubphylumsList;
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();

                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperclassesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSubphylum = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl12SubphylumsList ??= [];
                ViewModel.Tbl12SubphylumsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl12Subphylums
                .Where(a => a.SubphylumId == id)]
                : [.. Context.Tbl12Subphylums
                .Where(a => _search != null && a.SubphylumName.StartsWith(_search))
                .OrderBy(a => a.SubphylumName)];
            });

            SubphylumDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            SuperclassesExpander.Visibility = Visibility.Collapsed;
            SuperclassDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SubphylumDataSetCount = ViewModel.Tbl12SubphylumsList.Count;

            if (ViewModel.Tbl12SubphylumsList is { Count: 0 })
            {
                ViewModel.Tbl12SubphylumsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();

                AppBarButtonDeleteSubphylum.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSubphylum.Visibility = Visibility.Visible;
                SubphylumDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                SuperclassesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl12SubphylumListPage));
            }
            else
            {
                SubphylumDataGrid.ItemsSource = ViewModel.Tbl12SubphylumsList;
                ViewModel.SubphylumSelected = ViewModel.Tbl12SubphylumsList[0];
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

    private async void SubphylumDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SubphylumDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SubphylumDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SubphylumSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SubphylumSelected != null)
        {
            //----------------------------------Reference--------------------------------       
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90ReferencesList ??= [];
                ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
            .Where(refe => refe.SubphylumId == ViewModel.SubphylumSelected.SubphylumId)];
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
            .Where(com => com.SubphylumId == ViewModel.SubphylumSelected.SubphylumId)];
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
            .Where(com => com.SubphylumId == ViewModel.SubphylumSelected.SubphylumId)];
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
        }
        ViewModel.IsLoading = false;
    }

    private async void AddSubphylum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSubphylum();
            SubphylumName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SubphylumName);
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

    private async Task AddSubphylum()
    {
        ViewModel.IsInEditSubphylum = true;
        ViewModel.IsNewSubphylum = true;

        await ViewModel.AddSubphylumAsync();

        if (ViewModel.Tbl12SubphylumsList != null)
        {
            ViewModel.SubphylumSelected = ViewModel.Tbl12SubphylumsList[0];
        }

        SubphylumDetailExpander.IsExpanded = true;
        SubphylumDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void SubphylumSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SubphylumSearchBox.Visibility = Visibility.Visible;
        SubphylumSearchBox.Focus(FocusState.Programmatic);
        SubphylumSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SubphylumDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl12SubphylumsList.Sort);
    }


    private async void ViewSubphylumHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSubphylumHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSubphylumHyperlink()
    {
        ViewModel.IsInEditSubphylum = false;
        ViewModel.IsNewSubphylum = false;
        SubphylumDetailExpander.IsExpanded = true;
        SubphylumDetailExpander.Visibility = Visibility.Visible;

        if (SubphylumDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSubphylum = App.SingleSubphylum(ViewModel.SubphylumSelected.SubphylumId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSubphylumAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSubphylum)
        {
            SubphylumDetailExpander.IsExpanded = false;
            SubphylumDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSubphylumEdits();
        }
    }

    private void CancelSubphylumEdits()
    {
        var x = (DataGrid)FindName("SubphylumDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SubphylumSelected = ViewModel.Tbl12SubphylumsList[iNdx];
            SingleSubphylum = App.SingleSubphylum(ViewModel.SubphylumSelected.SubphylumId);
            SubphylumSingles();
        }
        else
        {
            SubphylumDetailExpander.IsExpanded = false;
            SubphylumDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSubphylum = false;
        ViewModel.IsNewSubphylum = false;
    }

    private void SubphylumSingles()
    {
        ViewModel.SubphylumSelected.SubphylumName = SingleSubphylum.SubphylumName;
        ViewModel.SubphylumSelected.PhylumId = SingleSubphylum.PhylumId;
        ViewModel.SubphylumSelected.Valid = SingleSubphylum.Valid;
        ViewModel.SubphylumSelected.ValidYear = SingleSubphylum.ValidYear;
        ViewModel.SubphylumSelected.Author = SingleSubphylum.Author;
        ViewModel.SubphylumSelected.AuthorYear = SingleSubphylum.AuthorYear;
        ViewModel.SubphylumSelected.Info = SingleSubphylum.Info;
        ViewModel.SubphylumSelected.Synonym = SingleSubphylum.Synonym;
        ViewModel.SubphylumSelected.EngName = SingleSubphylum.EngName;
        ViewModel.SubphylumSelected.GerName = SingleSubphylum.GerName;
        ViewModel.SubphylumSelected.FraName = SingleSubphylum.FraName;
        ViewModel.SubphylumSelected.PorName = SingleSubphylum.PorName;
        ViewModel.SubphylumSelected.Memo = SingleSubphylum.Memo;
    }
    private async Task RevertChangesSubphylumAsync()
    {
        ViewModel.IsInEditSubphylum = false;
        if (ViewModel.IsModifiedSubphylum)
        {
            await RefreshSubphylumAsync();
            ViewModel.IsModifiedSubphylum = false;
        }
    }
    private Task RefreshSubphylumAsync()
    {
        if (SubphylumDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.SubphylumSelected.SubphylumId == 0)
            {
                return Task.CompletedTask;
            }
            SingleSubphylum = App.SingleSubphylum(ViewModel.SubphylumSelected.SubphylumId);
            SubphylumSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSubphylum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSubphylum();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSubphylum()
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
                await ViewModel.DeleteSubphylumAsync(ViewModel.SubphylumSelected);
                await Refresh();
                SubphylumDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSubphylumAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSubphylum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSubphylum();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSubphylum()
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
                if (await BeforeSaveSubphylum())
                {
                    return;
                }
                await ViewModel.SaveSubphylumAsync(ViewModel.SubphylumSelected);
                await BehindSaveSubphylum(ViewModel.SubphylumSelected);
                await Refresh();
                ViewModel.IsNewSubphylum = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSubphylum = true;
                await RevertChangesSubphylumAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSubphylum()
    {
        _search = SubphylumSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl12SubphylumsList ??= [];
            ViewModel.Tbl12SubphylumsList = [.. Context.Tbl12Subphylums
                .Where(a => a.SubphylumName == ViewModel.SubphylumSelected.SubphylumName &&
                        a.PhylumId == ViewModel.SubphylumSelected.PhylumId)];
        });
        if (ViewModel.IsNewSubphylum)
        {
            if (ViewModel.Tbl12SubphylumsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSubphylum = true;
        return false;
    }

    private async Task BehindSaveSubphylum(Tbl12Subphylum viewModelSubphylumSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl12SubphylumsList ??= [];
            if (SubphylumSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl12SubphylumsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl12Subphylums
                    .Where(a => a.SubphylumId == id)]
                    : [.. Context.Tbl12Subphylums
                    .Where(a => _search != null && a.SubphylumName.StartsWith(_search))
                    .OrderBy(a => a.SubphylumName)];
            }
        });

        ViewModel.RefreshSubphylumItems();

        if (ViewModel.Tbl12SubphylumsList.Count == 0)
        {
            ViewModel.Tbl12SubphylumsList = [.. Context.Tbl12Subphylums
                .Where(a => a.SubphylumId == viewModelSubphylumSelected.SubphylumId)];
            SubphylumDataGrid.ItemsSource = ViewModel.Tbl12SubphylumsList;
            await Refresh();
            SubphylumDataGrid.SelectedIndex = 0;
        }
        else
        {
            SubphylumDataGrid.ItemsSource = ViewModel.Tbl12SubphylumsList;
            var index = ViewModel.SubphylumItems.ToList().FindIndex(item => item.SubphylumId == viewModelSubphylumSelected.SubphylumId);
            SubphylumDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.SubphylumDataSetCount = ViewModel.Tbl12SubphylumsList.Count;
    }

    private void SubphylumName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSubphylum_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Subphylum Methods]                    

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
            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SubphylumSelected.SubphylumId);
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
                        a.SubphylumId == ViewModel.ReferenceSelected.SubphylumId &&
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
                .Where(refe => refe.SubphylumId == viewModelReferenceSelected.SubphylumId)];
        });
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(com => com.SubphylumId == viewModelReferenceSelected.SubphylumId)];
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

            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SubphylumSelected.SubphylumId);
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
            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                                  ViewModel.SubphylumSelected.SubphylumId);

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

            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                                   ViewModel.SubphylumSelected.SubphylumId);
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
            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                           ViewModel.SubphylumSelected.SubphylumId);

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

            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                            ViewModel.SubphylumSelected.SubphylumId);
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
            SingleReference = App.SingleReferenceSubphylum(ViewModel.ReferenceSelected.ReferenceId,
                    ViewModel.SubphylumSelected.SubphylumId);

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
            SingleComment = App.SingleCommentSubphylum(ViewModel.CommentSelected.CommentId, ViewModel.SubphylumSelected.SubphylumId);
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

            SingleComment = App.SingleCommentSubphylum(ViewModel.CommentSelected.CommentId, ViewModel.SubphylumSelected.SubphylumId);
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
                SingleComment = App.SingleCommentSubphylum(ViewModel.CommentSelected.CommentId, ViewModel.SubphylumSelected.SubphylumId);
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
                        a.SubphylumId == ViewModel.CommentSelected.SubphylumId)];
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
                .Where(com => com.SubphylumId == viewModelCommentSelected.SubphylumId)];
        });
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .Where(com => com.SubphylumId == viewModelCommentSelected.SubphylumId)];
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

    #region [Superclass Methods]

    private void DataGrid_Superclass_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl18SuperclassesList.Sort);
    }

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
            SingleSuperclass = App.SingleSuperclassSubphylum(ViewModel.SuperclassSelected.SuperclassId,
                    ViewModel.SubphylumSelected.SubphylumId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSuperclassAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSuperclass)
        {
            SuperclassDetailExpander.IsExpanded = false;
            SuperclassDetailExpander.Visibility = Visibility.Collapsed;
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
        if (iNdx is not -1)
        {
            ViewModel.SuperclassSelected = ViewModel.Tbl18SuperclassesList[iNdx];

            SingleSuperclass = App.SingleSuperclassSubphylum(ViewModel.SuperclassSelected.SuperclassId, ViewModel.SubphylumSelected.SubphylumId);
            SuperclassSingles();
        }
        else
        {
            SuperclassDetailExpander.IsExpanded = false;
            SuperclassDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSuperclass = false;
        ViewModel.IsNewSuperclass = false;
    }

    private void SuperclassSingles()
    {
        ViewModel.SuperclassSelected.SuperclassName = SingleSuperclass.SuperclassName;
        ViewModel.SuperclassSelected.SubphylumId = SingleSuperclass.SubphylumId;
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
                SingleSuperclass = App.SingleSuperclassSubphylum(ViewModel.SuperclassSelected.SuperclassId, ViewModel.SubphylumSelected.SubphylumId);
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
                await BehindSaveSuperclass(ViewModel.SuperclassSelected);
                await Refresh();
                ViewModel.IsNewSuperclass = false;
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
                        a.SubphylumId == ViewModel.SuperclassSelected.SubphylumId)];
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

    private async Task BehindSaveSuperclass(Tbl18Superclass viewModelSuperclassSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl18SuperclassesList ??= [];
            ViewModel.Tbl18SuperclassesList = [.. Context.Tbl18Superclasses
                .Where(com => com.SubphylumId == viewModelSuperclassSelected.SubphylumId)];
        });
        ViewModel.RefreshSuperclassItems();

        if (ViewModel.Tbl18SuperclassesList.Count == 0)
        {
            ViewModel.Tbl18SuperclassesList = [.. Context.Tbl18Superclasses
                .Where(com => com.SubphylumId == viewModelSuperclassSelected.SubphylumId)];
            SuperclassDataGrid.ItemsSource = ViewModel.Tbl18SuperclassesList;
            await Refresh();
            SuperclassesExpander.Visibility = Visibility.Collapsed;
            SuperclassDataGrid.ItemsSource = null;
        }
        else
        {
            SuperclassesExpander.Visibility = Visibility.Visible;
            SuperclassDataGrid.ItemsSource = ViewModel.Tbl18SuperclassesList;
            SuperclassDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.SuperclassDataSetCount = ViewModel.Tbl18SuperclassesList.Count;
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
            //  DetailPanelSubphylum.Visibility = Visibility.Visible;
            //  SubphylumDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SubphylumDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl12SubphylumsList!.Count)
            {
                ViewModel.SubphylumItems.Clear();
                foreach (var item in ViewModel.Tbl12SubphylumsList)
                {
                    ViewModel.SubphylumItems.Add(item);
                }

                ViewModel.SubphylumSelected = ViewModel.Tbl12SubphylumsList[iNdx];
            }
            SubphylumDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl12SubphylumsList.Count == 0)
                {
                    ViewModel.Tbl12SubphylumsList ??= [];
                    ViewModel.Tbl12SubphylumsList = [.. Context.Tbl12Subphylums
                            .Where(a => a.SubphylumId == ViewModel.SubphylumSelected.SubphylumId)];
                }
                else
                {
                    ViewModel.Tbl12SubphylumsList ??= [];
                    ViewModel.Tbl12SubphylumsList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl12Subphylums
                           .Where(a => a.SubphylumId == id)]
                       : [.. Context.Tbl12Subphylums
                           .Where(a => _search != null && a.SubphylumName.StartsWith(_search))
                         .OrderBy(a => a.SubphylumName)];
                }
            });

            SubphylumDataGrid.ItemsSource = ViewModel.Tbl12SubphylumsList;

            if (ViewModel.IsDeletingSubphylum)
            {
                if (iNdx == 0 && ViewModel.Tbl12SubphylumsList.Count == 0)
                {
                    ViewModel.Tbl12SubphylumsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SubphylumSelected = ViewModel.Tbl12SubphylumsList[iNdx];
            }
            if (ViewModel.Tbl12SubphylumsList is { Count: 0 })
            {
                ViewModel.Tbl12SubphylumsList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl18SuperclassesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }
        SubphylumDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSubphylum = false;
    }

    private void BackDetailExpanders()
    {
        SubphylumDetailExpander.IsExpanded = false;
        SubphylumDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        SuperclassDetailExpander.IsExpanded = false;
        SuperclassDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteSubphylum.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteSubphylum.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Icon.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteSubphylum.Visibility = Visibility.Visible;
        AppBarButtonAddReference.Visibility = Visibility.Visible;
        AppBarButtonAddComment.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteSubphylum.Visibility = Visibility.Collapsed;
        AppBarButtonAddReference.Visibility = Visibility.Collapsed;
        AppBarButtonAddComment.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }
    #endregion [Refresh Methods] 


}

