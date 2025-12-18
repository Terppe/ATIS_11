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

//  Tbl90RefAuthorListPage.xaml.cs Skriptdatum:  06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl90RefAuthorListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl90RefAuthorViewModel ViewModel { get; }
    private Tbl90RefAuthor SingleRefAuthor { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl90RefAuthorListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl90RefAuthorViewModel>();
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

    #region [RefAuthor Methods]

    private async void GetRefAuthorsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl90RefAuthorsList?.Clear();

            ViewModel.RefAuthorItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl90RefAuthorsList?.Clear();

                RefAuthorDataGrid.ItemsSource = ViewModel.Tbl90RefAuthorsList;
                RefAuthorDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingRefAuthor = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefAuthorsList ??= [];
                ViewModel.Tbl90RefAuthorsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefAuthors
                .Where(a => a.RefAuthorId == id)]
                : [.. Context.Tbl90RefAuthors
                .Where(a => _search != null && a.RefAuthorName.StartsWith(_search))
                .OrderBy(a => a.RefAuthorName)];
            });

            RefAuthorDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.RefAuthorDataSetCount = ViewModel.Tbl90RefAuthorsList.Count;

            if (ViewModel.Tbl90RefAuthorsList is { Count: 0 })
            {
                ViewModel.Tbl90RefAuthorsList?.Clear();

                AppBarButtonDeleteRefAuthor.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelRefAuthor.Visibility = Visibility.Visible;
                RefAuthorDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl90RefAuthorListPage));
            }
            else
            {
                RefAuthorDataGrid.ItemsSource = ViewModel.Tbl90RefAuthorsList;
                ViewModel.RefAuthorSelected = ViewModel.Tbl90RefAuthorsList[0];
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

    private async void RefAuthorDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await RefAuthorDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task RefAuthorDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.RefAuthorSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddRefAuthor_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddRefAuthor();
            RefAuthorName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(RefAuthorName);
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

    private async Task AddRefAuthor()
    {
        ViewModel.IsInEditRefAuthor = true;
        ViewModel.IsNewRefAuthor = true;

        await ViewModel.AddRefAuthorAsync();

        if (ViewModel.Tbl90RefAuthorsList != null)
        {
            ViewModel.RefAuthorSelected = ViewModel.Tbl90RefAuthorsList[0];
        }

        RefAuthorDetailExpander.IsExpanded = true;
        RefAuthorDetailExpander.Visibility = Visibility.Visible;

        RefAuthorName.PlaceholderText = StringsRes.EnterText;
    }

    private void RefAuthorSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        RefAuthorSearchBox.Visibility = Visibility.Visible;
        RefAuthorSearchBox.Focus(FocusState.Programmatic);
        RefAuthorSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void RefAuthorDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90RefAuthorsList.Sort);
    }

    private void AddNewRefAuthorCanceled(object sender, EventArgs e) => RefAuthorDetailExpander.IsExpanded = false;

    private async void ViewRefAuthorHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewRefAuthorHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewRefAuthorHyperlink()
    {
        ViewModel.IsInEditRefAuthor = false;
        ViewModel.IsNewRefAuthor = false;
        RefAuthorDetailExpander.IsExpanded = true;
        RefAuthorDetailExpander.Visibility = Visibility.Visible;

        if (RefAuthorDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleRefAuthor = App.SingleRefAuthor(ViewModel.RefAuthorSelected.RefAuthorId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsRefAuthorAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewRefAuthor)
        {
            RefAuthorDetailExpander.IsExpanded = false;
            RefAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelRefAuthorEdits();
        }
    }

    private void CancelRefAuthorEdits()
    {
        var x = (DataGrid)FindName("RefAuthorDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.RefAuthorSelected = ViewModel.Tbl90RefAuthorsList[iNdx];
            SingleRefAuthor = App.SingleRefAuthor(ViewModel.RefAuthorSelected.RefAuthorId);
            RefAuthorSingles();
        }
        else
        {
            RefAuthorDetailExpander.IsExpanded = false;
            RefAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditRefAuthor = false;
        ViewModel.IsNewRefAuthor = false;
    }

    private void RefAuthorSingles()
    {
        ViewModel.RefAuthorSelected.RefAuthorName = SingleRefAuthor.RefAuthorName;
        ViewModel.RefAuthorSelected.Valid = SingleRefAuthor.Valid;
        ViewModel.RefAuthorSelected.ValidYear = SingleRefAuthor.ValidYear;
        ViewModel.RefAuthorSelected.Info = SingleRefAuthor.Info;
        ViewModel.RefAuthorSelected.ArticelTitle = SingleRefAuthor.ArticelTitle;
        ViewModel.RefAuthorSelected.BookName = SingleRefAuthor.BookName;
        ViewModel.RefAuthorSelected.ISBN = SingleRefAuthor.ISBN;
        ViewModel.RefAuthorSelected.Page1 = SingleRefAuthor.Page1;
        ViewModel.RefAuthorSelected.Notes = SingleRefAuthor.Notes;
        ViewModel.RefAuthorSelected.Memo = SingleRefAuthor.Memo;
        ViewModel.RefAuthorSelected.Publisher = SingleRefAuthor.Publisher;
        ViewModel.RefAuthorSelected.PublicationPlace = SingleRefAuthor.PublicationPlace;
        ViewModel.RefAuthorSelected.PublicationYear = SingleRefAuthor.PublicationYear;
    }
    private async Task RevertChangesRefAuthorAsync()
    {
        ViewModel.IsInEditRefAuthor = false;
        if (ViewModel.IsModifiedRefAuthor)
        {
            await RefreshRefAuthorAsync();
            ViewModel.IsModifiedRefAuthor = false;
        }
    }
    private Task RefreshRefAuthorAsync()
    {
        if (RefAuthorDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.RefAuthorSelected.RefAuthorId == 0)
            {
                return Task.CompletedTask;
            }
            SingleRefAuthor = App.SingleRefAuthor(ViewModel.RefAuthorSelected.RefAuthorId);
            RefAuthorSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteRefAuthor_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteRefAuthor();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteRefAuthor()
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
                await ViewModel.DeleteRefAuthorAsync(ViewModel.RefAuthorSelected);
                await Refresh();
                RefAuthorDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesRefAuthorAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveRefAuthor_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveRefAuthor();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveRefAuthor()
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
                if (await BeforeSaveRefAuthor())
                {
                    return;
                }
                await ViewModel.SaveRefAuthorAsync(ViewModel.RefAuthorSelected);
                await BehindSaveRefAuthor(ViewModel.RefAuthorSelected);
                await Refresh();
                ViewModel.IsNewRefAuthor = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedRefAuthor = true;
                await RevertChangesRefAuthorAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveRefAuthor()
    {
        _search = RefAuthorSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefAuthorsList ??= [];
            ViewModel.Tbl90RefAuthorsList = [.. Context.Tbl90RefAuthors
            .Where(a => a.RefAuthorName == ViewModel.RefAuthorSelected.RefAuthorName &&
                        a.ArticelTitle == ViewModel.RefAuthorSelected.ArticelTitle &&
                        a.BookName == ViewModel.RefAuthorSelected.BookName &&
                        a.Page1 == ViewModel.RefAuthorSelected.Page1 &&
                        a.Info == ViewModel.RefAuthorSelected.Info &&
                        a.ISBN == ViewModel.RefAuthorSelected.ISBN &&
                        a.Publisher == ViewModel.RefAuthorSelected.Publisher &&
                        a.PublicationPlace == ViewModel.RefAuthorSelected.PublicationPlace)];
        });
        if (ViewModel.IsNewRefAuthor)
        {
            if (ViewModel.Tbl90RefAuthorsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedRefAuthor = true;
        return false;
    }

    private async Task BehindSaveRefAuthor(Tbl90RefAuthor viewModelRefAuthorSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefAuthorsList ??= [];
            if (RefAuthorSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl90RefAuthorsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl90RefAuthors
                    .Where(a => a.RefAuthorId == id)]
                    : [.. Context.Tbl90RefAuthors
                    .Where(a => _search != null && a.RefAuthorName.StartsWith(_search))
                    .OrderBy(a => a.RefAuthorName)];
            }
        });

        ViewModel.RefreshRefAuthorItems();

        if (ViewModel.Tbl90RefAuthorsList.Count == 0)
        {
            ViewModel.Tbl90RefAuthorsList = [.. Context.Tbl90RefAuthors
                .Where(a => a.RefAuthorId == viewModelRefAuthorSelected.RefAuthorId)];
            RefAuthorDataGrid.ItemsSource = ViewModel.Tbl90RefAuthorsList;
            await Refresh();
            RefAuthorDataGrid.SelectedIndex = 0;
        }
        else
        {
            RefAuthorDataGrid.ItemsSource = ViewModel.Tbl90RefAuthorsList;
            var index = ViewModel.RefAuthorItems.ToList().FindIndex(item => item.RefAuthorId == viewModelRefAuthorSelected.RefAuthorId);
            RefAuthorDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.RefAuthorDataSetCount = ViewModel.Tbl90RefAuthorsList.Count;
    }


    private void BackRefAuthor_Click(object sender, RoutedEventArgs e)
    {
        RefAuthorDetailExpander.IsExpanded = false;
        RefAuthorDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [RefAuthor Methods]        
    private void BackDetailExpanders()
    {
        RefAuthorDetailExpander.IsExpanded = false;
        RefAuthorDetailExpander.Visibility = Visibility.Collapsed;
    }

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
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("RefAuthorDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl90RefAuthorsList!.Count)
            {
                ViewModel.RefAuthorItems.Clear();
                foreach (var item in ViewModel.Tbl90RefAuthorsList)
                {
                    ViewModel.RefAuthorItems.Add(item);
                }

                ViewModel.RefAuthorSelected = ViewModel.Tbl90RefAuthorsList[iNdx];
            }
            RefAuthorDataGrid.SelectedIndex = iNdx;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefAuthorsList ??= [];
                ViewModel.Tbl90RefAuthorsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefAuthors
                .Where(a => a.RefAuthorId == id)]
                : [.. Context.Tbl90RefAuthors
                .Where(a => _search != null && a.RefAuthorName.StartsWith(_search))
                .OrderBy(a => a.RefAuthorName)];
            });
            if (ViewModel.Tbl90RefAuthorsList is { Count: 0 })
            {
                ViewModel.Tbl90RefAuthorsList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            RefAuthorDataGrid.ItemsSource = ViewModel.Tbl90RefAuthorsList;

            if (ViewModel.IsDeletingRefAuthor)
            {
                if (iNdx == 0 && ViewModel.Tbl90RefAuthorsList.Count == 0)
                {
                    ViewModel.Tbl90RefAuthorsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.RefAuthorSelected = ViewModel.Tbl90RefAuthorsList[iNdx];
            }
        }
        RefAuthorDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingRefAuthor = false;
    }

    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteRefAuthor.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteRefAuthor.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteRefAuthor.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteRefAuthor.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

