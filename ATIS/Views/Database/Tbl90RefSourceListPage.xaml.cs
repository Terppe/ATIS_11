
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

//  Tbl90RefSourceListPage.xaml.cs Skriptdatum:   06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl90RefSourceListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl90RefSourceViewModel ViewModel { get; }
    private Tbl90RefSource SingleRefSource { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl90RefSourceListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl90RefSourceViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]         

    #region [RefSource Methods]

    private async void GetRefSourcesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl90RefSourcesList?.Clear();

            ViewModel.RefSourceItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl90RefSourcesList?.Clear();

                RefSourceDataGrid.ItemsSource = ViewModel.Tbl90RefSourcesList;
                RefSourceDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingRefSource = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefSourcesList ??= [];
                ViewModel.Tbl90RefSourcesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefSources
                .Where(a => a.RefSourceId == id)]
                : [.. Context.Tbl90RefSources
                .Where(a => _search != null && a.RefSourceName.StartsWith(_search))
                .OrderBy(a => a.RefSourceName)];

            });
            RefSourceDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.RefSourceDataSetCount = ViewModel.Tbl90RefSourcesList.Count;

            if (ViewModel.Tbl90RefSourcesList is { Count: 0 })
            {
                ViewModel.Tbl90RefSourcesList?.Clear();

                AppBarButtonDeleteRefSource.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelRefSource.Visibility = Visibility.Visible;
                RefSourceDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl90RefSourceListPage));
            }
            else
            {
                RefSourceDataGrid.ItemsSource = ViewModel.Tbl90RefSourcesList;
                ViewModel.RefSourceSelected = ViewModel.Tbl90RefSourcesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void RefSourceDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await RefSourceDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task RefSourceDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.RefSourceSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddRefSource_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddRefSource();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddRefSource()
    {
        RefSourceName.Loaded += (s, e) =>
        {
            RefSourceName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditRefSource = true;
        ViewModel.IsNewRefSource = true;

        await ViewModel.AddRefSourceAsync();

        if (ViewModel.Tbl90RefSourcesList != null)
        {
            ViewModel.RefSourceSelected = ViewModel.Tbl90RefSourcesList[0];
        }

        RefSourceDetailExpander.IsExpanded = true;
        RefSourceDetailExpander.Visibility = Visibility.Visible;

        RefSourceName.PlaceholderText = StringsRes.EnterText;
    }

    private void RefSourceSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        RefSourceSearchBox.Visibility = Visibility.Visible;
        RefSourceSearchBox.Focus(FocusState.Programmatic);
        RefSourceSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void RefSourceDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90RefSourcesList.Sort);
    }

    private void AddNewRefSourceCanceled(object sender, EventArgs e) => RefSourceDetailExpander.IsExpanded = false;

    private async void ViewRefSourceHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewRefSourceHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewRefSourceHyperlink()
    {
        ViewModel.IsInEditRefSource = false;
        ViewModel.IsNewRefSource = false;
        RefSourceDetailExpander.IsExpanded = true;
        RefSourceDetailExpander.Visibility = Visibility.Visible;

        if (RefSourceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleRefSource = App.SingleRefSource(ViewModel.RefSourceSelected.RefSourceId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsRefSourceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewRefSource)
        {
            RefSourceDetailExpander.IsExpanded = false;
            RefSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelRefSourceEdits();
        }
    }

    private void CancelRefSourceEdits()
    {
        var x = (DataGrid)FindName("RefSourceDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.RefSourceSelected = ViewModel.Tbl90RefSourcesList[iNdx];
            SingleRefSource = App.SingleRefSource(ViewModel.RefSourceSelected.RefSourceId);
            RefSourceSingles();
        }
        else
        {
            RefSourceDetailExpander.IsExpanded = false;
            RefSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditRefSource = false;
        ViewModel.IsNewRefSource = false;
    }

    private void RefSourceSingles()
    {
        ViewModel.RefSourceSelected.RefSourceName = SingleRefSource.RefSourceName;
        ViewModel.RefSourceSelected.Valid = SingleRefSource.Valid;
        ViewModel.RefSourceSelected.ValidYear = SingleRefSource.ValidYear;
        ViewModel.RefSourceSelected.Info = SingleRefSource.Info;
        ViewModel.RefSourceSelected.Author = SingleRefSource.Author;
        ViewModel.RefSourceSelected.SourceYear = SingleRefSource.SourceYear;
        ViewModel.RefSourceSelected.Notes = SingleRefSource.Notes;
        ViewModel.RefSourceSelected.Memo = SingleRefSource.Memo;
    }
    private async Task RevertChangesRefSourceAsync()
    {
        ViewModel.IsInEditRefSource = false;
        if (ViewModel.IsModifiedRefSource)
        {
            await RefreshRefSourceAsync();
            ViewModel.IsModifiedRefSource = false;
        }
    }
    private Task RefreshRefSourceAsync()
    {
        if (RefSourceDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleRefSource = App.SingleRefSource(ViewModel.RefSourceSelected.RefSourceId);
            RefSourceSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteRefSource_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteRefSource();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteRefSource()
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
                await ViewModel.DeleteRefSourceAsync(ViewModel.RefSourceSelected);
                await Refresh();
                RefSourceDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesRefSourceAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveRefSource_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveRefSource();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveRefSource()
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
                if (await BeforeSaveRefSource())
                {
                    return;
                }
                await ViewModel.SaveRefSourceAsync(ViewModel.RefSourceSelected);
                await BehindSaveRefSource(ViewModel.RefSourceSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedRefSource = true;
                await RevertChangesRefSourceAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveRefSource()
    {
        _search = RefSourceSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefSourcesList ??= [];
            ViewModel.Tbl90RefSourcesList = [.. Context.Tbl90RefSources
            .Where(a => a.RefSourceName == ViewModel.RefSourceSelected.RefSourceName &&
                        a.Info == ViewModel.RefSourceSelected.Info &&
                        a.SourceYear == ViewModel.RefSourceSelected.SourceYear &&
                        a.Notes == ViewModel.RefSourceSelected.Notes)];
        });
        if (ViewModel.IsNewRefSource)
        {
            if (ViewModel.Tbl90RefSourcesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedRefSource = true;
        return false;
    }

    private async Task BehindSaveRefSource(Tbl90RefSource viewModelRefSourceSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefSourcesList ??= [];
            if (RefSourceSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl90RefSourcesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl90RefSources
                    .Where(a => a.RefSourceId == id)]
                    : [.. Context.Tbl90RefSources
                    .Where(a => _search != null && a.RefSourceName.StartsWith(_search))
                    .OrderBy(a => a.RefSourceName)];
            }
        });

        ViewModel.RefreshRefSourceItems();

        if (ViewModel.Tbl90RefSourcesList.Count == 0)
        {
            ViewModel.Tbl90RefSourcesList = [.. Context.Tbl90RefSources
                .Where(a => a.RefSourceId == viewModelRefSourceSelected.RefSourceId)];
            RefSourceDataGrid.ItemsSource = ViewModel.Tbl90RefSourcesList;
            await Refresh();
            RefSourceDataGrid.SelectedIndex = 0;
        }
        else
        {
            RefSourceDataGrid.ItemsSource = ViewModel.Tbl90RefSourcesList;
            var index = ViewModel.RefSourceItems.ToList().FindIndex(item => item.RefSourceId == viewModelRefSourceSelected.RefSourceId);
            RefSourceDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.RefSourceDataSetCount = ViewModel.Tbl90RefSourcesList.Count;
    }

    private void BackRefSource_Click(object sender, RoutedEventArgs e)
    {
        RefSourceDetailExpander.IsExpanded = false;
        RefSourceDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [RefSource Methods]        

    private void BackDetailExpanders()
    {
        RefSourceDetailExpander.IsExpanded = false;
        RefSourceDetailExpander.Visibility = Visibility.Collapsed;
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
            //DetailPanelRefSource.Visibility = Visibility.Visible;
            //RefSourceDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("RefSourceDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl90RefSourcesList!.Count)
            {
                ViewModel.RefSourceItems.Clear();
                foreach (var item in ViewModel.Tbl90RefSourcesList)
                {
                    ViewModel.RefSourceItems.Add(item);
                }

                ViewModel.RefSourceSelected = ViewModel.Tbl90RefSourcesList[iNdx];
            }
            RefSourceDataGrid.SelectedIndex = iNdx;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefSourcesList ??= [];
                ViewModel.Tbl90RefSourcesList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefSources
                .Where(a => a.RefSourceId == id)]
                : [.. Context.Tbl90RefSources
                .Where(a => _search != null && a.RefSourceName.StartsWith(_search))
                .OrderBy(a => a.RefSourceName)];
            });
            if (ViewModel.Tbl90RefSourcesList is { Count: 0 })
            {
                ViewModel.Tbl90RefSourcesList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            RefSourceDataGrid.ItemsSource = ViewModel.Tbl90RefSourcesList;

            if (ViewModel.IsDeletingRefSource)
            {
                if (iNdx == 0 && ViewModel.Tbl90RefSourcesList.Count == 0)
                {
                    ViewModel.Tbl90RefSourcesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.RefSourceSelected = ViewModel.Tbl90RefSourcesList[iNdx];
            }
        }
        RefSourceDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingRefSource = false;
    }

    #endregion [Refresh Methods] 


}

