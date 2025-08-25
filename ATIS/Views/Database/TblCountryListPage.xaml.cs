
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

//  TblCountryListPage.xaml.cs Skriptdatum:   06.01.2025 12:32           

namespace ATIS.Views.Database;

public sealed partial class TblCountryListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public TblCountryViewModel ViewModel { get; }
    private TblCountry SingleCountry { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public TblCountryListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<TblCountryViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]         

    #region [Country Methods]

    private async void GetCountriesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.TblCountriesList?.Clear();

            ViewModel.CountryItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.TblCountriesList?.Clear();

                CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
                CountryDetailExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingCountry = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblCountriesList ??= [];
                ViewModel.TblCountriesList = int.TryParse(_search, out var id)
                ? [.. Context.TblCountries
                .Where(e => e.CountryId == id)]
                : [.. Context.TblCountries
                .Where(e => e.Name.StartsWith(_search))
                .OrderBy(a => a.Name)];

            });
            CountryDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.CountryDataSetCount = ViewModel.TblCountriesList.Count;

            if (ViewModel.TblCountriesList is { Count: 0 })
            {
                ViewModel.TblCountriesList?.Clear();

                AppBarButtonDeleteCountry.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelCountry.Visibility = Visibility.Visible;
                CountryDetailExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(TblCountryListPage));
            }
            else
            {
                CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
                ViewModel.CountrySelected = ViewModel.TblCountriesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void CountryDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await CountryDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task CountryDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.CountrySelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);
        ViewModel.IsLoading = false;
    }

    private async void AddCountry_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await AddCountry();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddCountry()
    {
        CountryName.Loaded += (s, e) =>
        {
            CountryName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditCountry = true;
        ViewModel.IsNewCountry = true;

        await ViewModel.AddCountryAsync();

        if (ViewModel.TblCountriesList != null)
        {
            ViewModel.CountrySelected = ViewModel.TblCountriesList[0];
        }

        CountryDetailExpander.IsExpanded = true;
        CountryDetailExpander.Visibility = Visibility.Visible;

        ViewModel.IsNewCountry = false;
        CountryName.PlaceholderText = StringsRes.EnterText;
    }

    private void CountrySearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        CountrySearchBox.Visibility = Visibility.Visible;
        CountrySearchBox.Focus(FocusState.Programmatic);
        CountrySearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void CountryDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.TblCountriesList.Sort);
    }

    private void AddNewCountryCanceled(object sender, EventArgs e) => CountryDetailExpander.IsExpanded = false;

    private async void ViewCountryHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewCountryHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewCountryHyperlink()
    {
        ViewModel.IsInEditCountry = false;
        ViewModel.IsNewCountry = false;
        CountryDetailExpander.IsExpanded = true;
        CountryDetailExpander.Visibility = Visibility.Visible;

        if (CountryDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleCountry = App.SingleCountry(ViewModel.CountrySelected.CountryId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsCountryAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewCountry)
        {
            CountryDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelCountryEdits();
        }
    }

    private void CancelCountryEdits()
    {
        var x = (DataGrid)FindName("CountryDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditCountry = false;
            ViewModel.IsNewCountry = false;
            CountryDetailExpander.IsExpanded = false;
            CountryDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.CountrySelected = ViewModel.TblCountriesList[iNdx];

        SingleCountry = App.SingleCountry(ViewModel.CountrySelected.CountryId);
        CountrySingles();

        ViewModel.IsInEditCountry = false;
        ViewModel.IsNewCountry = false;
        CountryDetailExpander.IsExpanded = true;
        CountryDetailExpander.Visibility = Visibility.Visible;
    }

    private void CountrySingles()
    {
        ViewModel.CountrySelected.Name = SingleCountry.Name;
        ViewModel.CountrySelected.Regex = SingleCountry.Regex;
        ViewModel.CountrySelected.ZoomLevel = SingleCountry.ZoomLevel;
        ViewModel.CountrySelected.Latitude = SingleCountry.Latitude;
        ViewModel.CountrySelected.Longitude = SingleCountry.Longitude;
    }
    private async Task RevertChangesCountryAsync()
    {
        ViewModel.IsInEditCountry = false;
        if (ViewModel.IsModifiedCountry)
        {
            await RefreshCountryAsync();
            ViewModel.IsModifiedCountry = false;
        }
    }
    private Task RefreshCountryAsync()
    {
        if (CountryDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleCountry = App.SingleCountry(ViewModel.CountrySelected.CountryId);
            CountrySingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteCountry_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteCountry();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteCountry()
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
                await ViewModel.DeleteCountryAsync(ViewModel.CountrySelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesCountryAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveCountry_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveCountry();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveCountry()
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
                if (await BeforeSaveCountry())
                {
                    return;
                }
                await ViewModel.SaveCountryAsync(ViewModel.CountrySelected);
                await BehindSaveCountry();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedCountry = true;
                await RevertChangesCountryAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveCountry()
    {
        _search = CountrySearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblCountriesList ??= [];
            ViewModel.TblCountriesList = [.. Context.TblCountries
            .Where(a => a.Name == ViewModel.CountrySelected.Name )];
        });
        if (ViewModel.IsNewCountry)
        {
            if (ViewModel.TblCountriesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedCountry = true;
        return false;
    }

    private async Task BehindSaveCountry()
    {
        if (string.IsNullOrEmpty(CountrySearchBox.Text))
        {
            var x = (DataGrid)FindName("CountryDataGrid");
            var iNdx = x.SelectedIndex;
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblCountriesList ??= [];
                ViewModel.TblCountriesList = [.. Context.TblCountries
                    .Where(a => a.CountryId == ViewModel.CountrySelected.CountryId)];
            });
            ViewModel.RefreshCountryItems();

            if (ViewModel.TblCountriesList.Count == 0)
            {
                CountryDetailExpander.Visibility = Visibility.Collapsed;
                CountryDataGrid.ItemsSource = null;
            }
            else
            {
                CountryDetailExpander.Visibility = Visibility.Visible;
                CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
            }

            CountryDataGrid.SelectedIndex = iNdx;

            CountryDetailExpander.IsExpanded = false;
            CountryDetailExpander.Visibility = Visibility.Collapsed;
        }
    }

    private void CountryName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackCountry_Click(object sender, RoutedEventArgs e)
    {
        CountryDetailExpander.IsExpanded = false;
        CountryDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Country Methods]                    

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
            //  DetailPanelCountry.Visibility = Visibility.Visible;
            // CountryDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("CountryDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.TblCountriesList!.Count)
            {
                ViewModel.CountryItems.Clear();
                foreach (var item in ViewModel.TblCountriesList)
                {
                    ViewModel.CountryItems.Add(item);
                }

                ViewModel.CountrySelected = ViewModel.TblCountriesList[iNdx];
            }
            CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
            CountryDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.TblCountriesList ??= [];
                ViewModel.TblCountriesList = int.TryParse(_search, out var id)
                 ? [.. Context.TblCountries
                .Where(a => a.CountryId == id)]
                 : [.. Context.TblCountries
                .Where(a => a.Name.StartsWith(_search))
                .OrderBy(a => a.Name)];
            });

            if (ViewModel.TblCountriesList is { Count: 0 })
            {
                ViewModel.TblCountriesList?.Clear();
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;

            if (ViewModel.IsDeletingCountry)
            {
                if (iNdx == 0 && ViewModel.TblCountriesList.Count == 0)
                {
                    ViewModel.TblCountriesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.CountrySelected = ViewModel.TblCountriesList[iNdx];
            }
        }
        CountryDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingCountry = false;
    }

    #endregion [Refresh Methods]


}

