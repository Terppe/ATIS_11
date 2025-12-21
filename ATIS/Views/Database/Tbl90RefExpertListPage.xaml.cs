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

//  Tbl90RefExpertListPage.xaml.cs Skriptdatum:  06.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl90RefExpertListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl90RefExpertViewModel ViewModel { get; }
    private Tbl90RefExpert SingleRefExpert { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl90RefExpertListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl90RefExpertViewModel>();
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

    #region [RefExpert Methods]

    private async void GetRefExpertsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl90RefExpertsList?.Clear();

            ViewModel.RefExpertItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl90RefExpertsList?.Clear();

                RefExpertDataGrid.ItemsSource = ViewModel.Tbl90RefExpertsList;
                RefExpertDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingRefExpert = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefExpertsList ??= [];
                ViewModel.Tbl90RefExpertsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefExperts
                .Where(a => a.RefExpertId == id)]
                : [.. Context.Tbl90RefExperts
                .Where(a => _search != null && a.RefExpertName.StartsWith(_search))
                .OrderBy(a => a.RefExpertName)];

            });
            RefExpertDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.RefExpertDataSetCount = ViewModel.Tbl90RefExpertsList.Count;

            if (ViewModel.Tbl90RefExpertsList is { Count: 0 })
            {
                ViewModel.Tbl90RefExpertsList?.Clear();

                AppBarButtonDeleteRefExpert.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelRefExpert.Visibility = Visibility.Visible;
                RefExpertDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl90RefExpertListPage));
            }
            else
            {
                RefExpertDataGrid.ItemsSource = ViewModel.Tbl90RefExpertsList;
                ViewModel.RefExpertSelected = ViewModel.Tbl90RefExpertsList[0];
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

    private async void RefExpertDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await RefExpertDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task RefExpertDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.RefExpertSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddRefExpert_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddRefExpert();
            RefExpertName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(RefExpertName);
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

    private async Task AddRefExpert()
    {
        ViewModel.IsInEditRefExpert = true;
        ViewModel.IsNewRefExpert = true;

        await ViewModel.AddRefExpertAsync();

        if (ViewModel.Tbl90RefExpertsList != null)
        {
            ViewModel.RefExpertSelected = ViewModel.Tbl90RefExpertsList[0];
        }

        RefExpertDetailExpander.IsExpanded = true;
        RefExpertDetailExpander.Visibility = Visibility.Visible;

        RefExpertName.PlaceholderText = StringsRes.EnterText;
    }

    private void RefExpertSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        RefExpertSearchBox.Visibility = Visibility.Visible;
        RefExpertSearchBox.Focus(FocusState.Programmatic);
        RefExpertSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void RefExpertDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl90RefExpertsList.Sort);
    }

    private async void ViewRefExpertHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewRefExpertHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewRefExpertHyperlink()
    {
        ViewModel.IsInEditRefExpert = false;
        ViewModel.IsNewRefExpert = false;
        RefExpertDetailExpander.IsExpanded = true;
        RefExpertDetailExpander.Visibility = Visibility.Visible;

        if (RefExpertDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleRefExpert = App.SingleRefExpert(ViewModel.RefExpertSelected.RefExpertId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsRefExpertAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewRefExpert)
        {
            RefExpertDetailExpander.IsExpanded = false;
            RefExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelRefExpertEdits();
        }
    }

    private void CancelRefExpertEdits()
    {
        var x = (DataGrid)FindName("RefExpertDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.RefExpertSelected = ViewModel.Tbl90RefExpertsList[iNdx];
            SingleRefExpert = App.SingleRefExpert(ViewModel.RefExpertSelected.RefExpertId);
            RefExpertSingles();
        }
        else
        {
            RefExpertDetailExpander.IsExpanded = false;
            RefExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditRefExpert = false;
        ViewModel.IsNewRefExpert = false;
    }

    private void RefExpertSingles()
    {
        ViewModel.RefExpertSelected.RefExpertName = SingleRefExpert.RefExpertName;
        ViewModel.RefExpertSelected.Valid = SingleRefExpert.Valid;
        ViewModel.RefExpertSelected.ValidYear = SingleRefExpert.ValidYear;
        ViewModel.RefExpertSelected.Info = SingleRefExpert.Info;
        ViewModel.RefExpertSelected.Notes = SingleRefExpert.Notes;
        ViewModel.RefExpertSelected.Memo = SingleRefExpert.Memo;
    }

    private async Task RevertChangesRefExpertAsync()
    {
        ViewModel.IsInEditRefExpert = false;
        if (ViewModel.IsModifiedRefExpert)
        {
            await RefreshRefExpertAsync();
            ViewModel.IsModifiedRefExpert = false;
        }
    }

    private Task RefreshRefExpertAsync()
    {
        if (RefExpertDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.RefExpertSelected.RefExpertId == 0)
            {
                return Task.CompletedTask;
            }
            SingleRefExpert = App.SingleRefExpert(ViewModel.RefExpertSelected.RefExpertId);
            RefExpertSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteRefExpert_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteRefExpert();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteRefExpert()
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
                await ViewModel.DeleteRefExpertAsync(ViewModel.RefExpertSelected);
                await Refresh();
                RefExpertDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesRefExpertAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveRefExpert_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveRefExpert();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveRefExpert()
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
                if (await BeforeSaveRefExpert())
                {
                    return;
                }
                await ViewModel.SaveRefExpertAsync(ViewModel.RefExpertSelected);
                await BehindSaveRefExpert(ViewModel.RefExpertSelected);
                ViewModel.IsNewRefExpert = false;
                await Refresh();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedRefExpert = true;
                await RevertChangesRefExpertAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveRefExpert()
    {
        _search = RefExpertSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefExpertsList ??= [];
            ViewModel.Tbl90RefExpertsList = [.. Context.Tbl90RefExperts
            .Where(a => a.RefExpertName == ViewModel.RefExpertSelected.RefExpertName &&
                        a.Info == ViewModel.RefExpertSelected.Info &&
                        a.Notes == ViewModel.RefExpertSelected.Notes)];
        });
        if (ViewModel.IsNewRefExpert)
        {
            if (ViewModel.Tbl90RefExpertsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedRefExpert = true;
        return false;
    }

    private async Task BehindSaveRefExpert(Tbl90RefExpert viewModelRefExpertSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90RefExpertsList ??= [];
            if (RefExpertSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl90RefExpertsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl90RefExperts
                    .Where(a => a.RefExpertId == id)]
                    : [.. Context.Tbl90RefExperts
                    .Where(a => _search != null && a.RefExpertName.StartsWith(_search))
                    .OrderBy(a => a.RefExpertName)];
            }
        });

        ViewModel.RefreshRefExpertItems();

        if (ViewModel.Tbl90RefExpertsList.Count == 0)
        {
            ViewModel.Tbl90RefExpertsList = [.. Context.Tbl90RefExperts
                .Where(a => a.RefExpertId == viewModelRefExpertSelected.RefExpertId)];
            RefExpertDataGrid.ItemsSource = ViewModel.Tbl90RefExpertsList;
            await Refresh();
            RefExpertDataGrid.SelectedIndex = 0;
        }
        else
        {
            RefExpertDataGrid.ItemsSource = ViewModel.Tbl90RefExpertsList;
            var index = ViewModel.RefExpertItems.ToList().FindIndex(item => item.RefExpertId == viewModelRefExpertSelected.RefExpertId);
            RefExpertDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.RefExpertDataSetCount = ViewModel.Tbl90RefExpertsList.Count;
    }

    private void BackRefExpert_Click(object sender, RoutedEventArgs e)
    {
        RefExpertDetailExpander.IsExpanded = false;
        RefExpertDetailExpander.Visibility = Visibility.Collapsed;
    }

    private void BackDetailExpanders()
    {
        RefExpertDetailExpander.IsExpanded = false;
        RefExpertDetailExpander.Visibility = Visibility.Collapsed;
    }
    #endregion [RefExpert Methods]        


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
            var x = (DataGrid)FindName("RefExpertDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl90RefExpertsList!.Count)
            {
                ViewModel.RefExpertItems.Clear();
                foreach (var item in ViewModel.Tbl90RefExpertsList)
                {
                    ViewModel.RefExpertItems.Add(item);
                }

                ViewModel.RefExpertSelected = ViewModel.Tbl90RefExpertsList[iNdx];
            }
            RefExpertDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl90RefExpertsList ??= [];
                ViewModel.Tbl90RefExpertsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl90RefExperts
                .Where(a => a.RefExpertId == id)]
                : [.. Context.Tbl90RefExperts
                .Where(a => _search != null && a.RefExpertName.StartsWith(_search))
                .OrderBy(a => a.RefExpertName)];
            });
            if (ViewModel.Tbl90RefExpertsList is { Count: 0 })
            {
                ViewModel.Tbl90RefExpertsList?.Clear();
                ViewModel.IsLoading = false;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            RefExpertDataGrid.ItemsSource = ViewModel.Tbl90RefExpertsList;

            if (ViewModel.IsDeletingRefExpert)
            {
                if (iNdx == 0 && ViewModel.Tbl90RefExpertsList.Count == 0)
                {
                    ViewModel.Tbl90RefExpertsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.RefExpertSelected = ViewModel.Tbl90RefExpertsList[iNdx];
            }
        }
        RefExpertDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingRefExpert = false;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteRefExpert.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteRefExpert.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteRefExpert.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteRefExpert.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

