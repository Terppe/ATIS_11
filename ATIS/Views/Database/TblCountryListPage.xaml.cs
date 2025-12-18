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
            SimpleLog.Info(" TblCountryListPage opended ");
            ButtonsNotVisible();
            ButtonsIconNotVisible();
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
            BackDetailExpanders();
            await AddCountry();
            CountryName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(CountryName);
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

    private async Task AddCountry()
    {
        ViewModel.IsInEditCountry = true;
        ViewModel.IsNewCountry = true;

        await ViewModel.AddCountryAsync();

        if (ViewModel.TblCountriesList != null)
        {
            ViewModel.CountrySelected = ViewModel.TblCountriesList[0];
        }

        CountryDetailExpander.IsExpanded = true;
        CountryDetailExpander.Visibility = Visibility.Visible;

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
            CountryDetailExpander.Visibility = Visibility.Collapsed;
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
        if (iNdx is not -1)
        {
            ViewModel.CountrySelected = ViewModel.TblCountriesList[iNdx];
            SingleCountry = App.SingleCountry(ViewModel.CountrySelected.CountryId);
            CountrySingles();
        }
        else
        {
            CountryDetailExpander.IsExpanded = false;
            CountryDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditCountry = false;
        ViewModel.IsNewCountry = false;
    }

    private void CountrySingles()
    {
        ViewModel.CountrySelected.Name = SingleCountry.Name;
        ViewModel.CountrySelected.Regex = SingleCountry.Regex;
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
            if (ViewModel.CountrySelected.CountryId == 0)
            {
                return Task.CompletedTask;
            }
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
                CountryDataGrid.SelectedIndex = 0;
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
                await BehindSaveCountry(ViewModel.CountrySelected);
                ViewModel.IsNewCountry = false;
                await Refresh();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedCountry = true;
                await RevertChangesCountryAsync();
                ButtonsVisible();
                ButtonsIconVisible();
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

    private async Task BehindSaveCountry(TblCountry viewModelCountrySelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.TblCountriesList ??= [];
            if (CountrySearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.TblCountriesList = int.TryParse(_search, out var id)
                    ? [.. Context.TblCountries
                    .Where(a => a.CountryId == id)]
                    : [.. Context.TblCountries
                    .Where(a => _search != null && a.Name.StartsWith(_search))
                    .OrderBy(a => a.Name)];
            }
        });

        ViewModel.RefreshCountryItems();

        if (ViewModel.TblCountriesList.Count == 0)
        {
            ViewModel.TblCountriesList = [.. Context.TblCountries
                .Where(a => a.CountryId == viewModelCountrySelected.CountryId)];
            CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
            await Refresh();
            CountryDataGrid.SelectedIndex = 0;
        }
        else
        {
            CountryDataGrid.ItemsSource = ViewModel.TblCountriesList;
            var index = ViewModel.CountryItems.ToList().FindIndex(item => item.CountryId == viewModelCountrySelected.CountryId);
            CountryDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.CountryDataSetCount = ViewModel.TblCountriesList.Count;
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

    private void BackDetailExpanders()
    {
        CountryDetailExpander.IsExpanded = false;
        CountryDetailExpander.Visibility = Visibility.Collapsed;
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
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteCountry.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteCountry.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteCountry.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteCountry.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

