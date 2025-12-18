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

//  Tbl87GeographicListPage.xaml.cs Skriptdatum:  08.07.2025  10:32      

namespace ATIS.Views.Database;

public sealed partial class Tbl87GeographicListPage : Page, Maps
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl87GeographicViewModel ViewModel { get; }
    private Tbl87Geographic SingleGeographic { get; set; }
    private Tbl75Species SingleSpecies { get; set; }
    public TblCountry SingleCountry { get; set; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    private double centerOf1LongX;
    private double centerOf1LatiY;
    private int zoom;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl87GeographicListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl87GeographicViewModel>();
            InitializeComponent();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }

        //string command = "/C notepad.exe";
        //Process.Start("cmd.exe", command);
    }
    #endregion [Constructor]         

    #region [Geographic Methods]

    private async void GetGeographicsByInfoOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl87GeographicsList?.Clear();

            ViewModel.GeographicItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl87GeographicsList?.Clear();

                GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
                ViewModel.Tbl75SpeciessesList?.Clear();

                SpeciessesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingGeographic = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                //    ViewModel.Tbl87GeographicsList ??= [];
                //   ViewModel.Tbl87GeographicsList = int.TryParse(_search, out var id)
                //       ? [.. Context.Tbl87Geographics
                //           .Where(e => e.GeographicId == id)]
                //      : [null];

                ViewModel.Tbl87GeographicsList ??= [];
                ViewModel.Tbl87GeographicsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl87Geographics
                .Where(e => e.GeographicId == id)]
                : [.. Context.Tbl87Geographics
                .Where(e => e.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];
            });

            GeographicDetailExpander.Visibility = Visibility.Collapsed;
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.GeographicDataSetCount = ViewModel.Tbl87GeographicsList.Count;

            if (ViewModel.Tbl87GeographicsList is { Count: 0 })
            {
                ViewModel.Tbl87GeographicsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();

                AppBarButtonDeleteGeographic.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelGeographic.Visibility = Visibility.Visible;
                GeographicDetailExpander.Visibility = Visibility.Collapsed;
                SpeciessesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl87GeographicListPage));
            }
            else
            {
                GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
                ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[0];
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

    private async void GeographicDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await GeographicDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task GeographicDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.GeographicSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.GeographicSelected != null)
        {
            //--------------------------Species-----------------------------   
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
            .Where(com => com.SpeciesId == ViewModel.GeographicSelected.SpeciesId)];
            });
            ViewModel.SpeciesDataSetCount = ViewModel.Tbl75SpeciessesList.Count;
            ViewModel.RefreshSpeciesItems();

            if (ViewModel.Tbl75SpeciessesList.Count == 0)
            {
                SpeciessesExpander.Visibility = Visibility.Collapsed;
                SpeciesDataGrid.ItemsSource = null;
            }
            else
            {
                SpeciessesExpander.Visibility = Visibility.Visible;
                SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            }
        }
        ViewModel.IsLoading = false;
    }

    private async void AddGeographic_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddGeographic();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddGeographic()
    {
        ViewModel.IsInEditGeographic = true;
        ViewModel.IsNewGeographic = true;

        await ViewModel.AddGeographicAsync();

        if (ViewModel.Tbl87GeographicsList != null)
        {
            ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[0];
        }

        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;

        InfoGeographic.PlaceholderText = StringsRes.EnterText;
    }

    private void GeographicSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        GeographicSearchBox.Visibility = Visibility.Visible;
        GeographicSearchBox.Focus(FocusState.Programmatic);
        GeographicSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void GeographicDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl87GeographicsList.Sort);
    }

    private void AddNewGeographicCanceled(object sender, EventArgs e) => GeographicDetailExpander.IsExpanded = false;

    private async void ViewGeographicHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewGeographicHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task ViewGeographicHyperlink()
    {
        ViewModel.IsInEditGeographic = false;
        ViewModel.IsNewGeographic = false;
        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;

        if (GeographicDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleGeographic = App.SingleGeographic(ViewModel.GeographicSelected.GeographicId);

            centerOf1LongX = Convert.ToDouble(Longitude1Geographic.Text);
            centerOf1LatiY = Convert.ToDouble(Latitude1Geographic.Text);
            zoom = (int)Convert.ToDouble(ZoomLevelGeographic.Text);

            var graphSelect = ViewModel.GeographicSelected;

            MyMap.Map = await ((Maps)this).CreateMapAsync(centerOf1LongX, centerOf1LatiY, zoom, graphSelect);
        }
    }
    private void CancelEditsGeographicAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewGeographic)
        {
            GeographicDetailExpander.IsExpanded = false;
            GeographicDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelGeographicEdits();
        }
    }

    private void CancelGeographicEdits()
    {
        var x = (DataGrid)FindName("GeographicDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];
            SingleGeographic = App.SingleGeographic(ViewModel.GeographicSelected.GeographicId);
            GeographicSingles();
        }
        else
        {
            GeographicDetailExpander.IsExpanded = false;
            GeographicDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditGeographic = false;
        ViewModel.IsNewGeographic = false;
    }

    private void GeographicSingles()
    {
        ViewModel.GeographicSelected.SpeciesId = SingleGeographic.SpeciesId;
        ViewModel.GeographicSelected.Valid = SingleGeographic.Valid;
        ViewModel.GeographicSelected.ValidYear = SingleGeographic.ValidYear;
        ViewModel.GeographicSelected.Author = SingleGeographic.Author;
        ViewModel.GeographicSelected.AuthorYear = SingleGeographic.AuthorYear;
        ViewModel.GeographicSelected.Continent = SingleGeographic.Continent;
        ViewModel.GeographicSelected.Country = SingleGeographic.Country;
        ViewModel.GeographicSelected.Address = SingleGeographic.Address;
        ViewModel.GeographicSelected.Http = SingleGeographic.Http;
        ViewModel.GeographicSelected.ZoomLevel = SingleGeographic.ZoomLevel;
        ViewModel.GeographicSelected.Jurisdiction = SingleGeographic.Jurisdiction;
        ViewModel.GeographicSelected.Latitude = SingleGeographic.Latitude;
        ViewModel.GeographicSelected.Latitude1 = SingleGeographic.Latitude1;
        ViewModel.GeographicSelected.Latitude2 = SingleGeographic.Latitude2;
        ViewModel.GeographicSelected.Latitude3 = SingleGeographic.Latitude3;
        ViewModel.GeographicSelected.Longitude = SingleGeographic.Longitude;
        ViewModel.GeographicSelected.Longitude1 = SingleGeographic.Longitude1;
        ViewModel.GeographicSelected.Longitude2 = SingleGeographic.Longitude2;
        ViewModel.GeographicSelected.Longitude3 = SingleGeographic.Longitude3;
        ViewModel.GeographicSelected.Info = SingleGeographic.Info;
        ViewModel.GeographicSelected.Memo = SingleGeographic.Memo;
    }
    private async Task RevertChangesGeographicAsync()
    {
        ViewModel.IsInEditGeographic = false;
        if (ViewModel.IsModifiedGeographic)
        {
            await RefreshGeographicAsync();
            ViewModel.IsModifiedGeographic = false;
        }
    }
    private Task RefreshGeographicAsync()
    {
        if (GeographicDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.GeographicSelected.GeographicId == 0)
            {
                return Task.CompletedTask;
            }
            SingleGeographic = App.SingleGeographic(ViewModel.GeographicSelected.GeographicId);
            GeographicSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteGeographic_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteGeographic();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteGeographic()
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
                await ViewModel.DeleteGeographicAsync(ViewModel.GeographicSelected);
                await Refresh();
                GeographicDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesGeographicAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveGeographic_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveGeographic();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveGeographic()
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
                if (await BeforeSaveGeographic())
                {
                    return;
                }
                await ViewModel.SaveGeographicAsync(ViewModel.GeographicSelected);
                await BehindSaveGeographic(ViewModel.GeographicSelected);
                await Refresh();
                ViewModel.IsNewGeographic = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedGeographic = true;
                await RevertChangesGeographicAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveGeographic()
    {
        ViewModel.Tbl87GeographicsList ??= [];
        ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
            .Where(a => a.Info == ViewModel.GeographicSelected.Info &&
                        a.Address == ViewModel.GeographicSelected.Address &&
                        a.Continent == ViewModel.GeographicSelected.Continent &&
                        a.Country == ViewModel.GeographicSelected.Country &&
                        a.Jurisdiction == ViewModel.GeographicSelected.Jurisdiction &&
                        a.SpeciesId == ViewModel.GeographicSelected.SpeciesId)];

        //Aus Countrylist Latitude und Longitude und ZoomLevel füllen
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (ViewModel.GeographicSelected.Country == null)
            {
                ViewModel.TblCountriesList ??= [];
                ViewModel.TblCountriesList = [.. Context.TblCountries
                    .Where(e => e.Name == ViewModel.GeographicSelected.Continent)];
            }
            else
            {
                ViewModel.TblCountriesList ??= [];
                ViewModel.TblCountriesList = [.. Context.TblCountries
                    .Where(e => e.Name == ViewModel.GeographicSelected.Country)];
            }
        });
        var selected = ViewModel.TblCountriesList[0];

        ViewModel.GeographicSelected.ZoomLevel = selected.ZoomLevel;
        ViewModel.GeographicSelected.Latitude = selected.Latitude;
        ViewModel.GeographicSelected.Longitude = selected.Longitude;

            if (ViewModel.Tbl87GeographicsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        ViewModel.IsModifiedGeographic = true;
        return false;
    }

    private async Task BehindSaveGeographic(Tbl87Geographic viewModelGeographicSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl87GeographicsList ??= [];
            if (GeographicSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl87GeographicsList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl87Geographics
                    .Where(a => a.GeographicId == id)]
                    : [.. Context.Tbl87Geographics
                    .Where(a => _search != null && a.Info.StartsWith(_search))
                    .OrderBy(a => a.Info)];
            }
        });

        ViewModel.RefreshGeographicItems();

        if (ViewModel.Tbl87GeographicsList.Count == 0)
        {
            ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
                .Where(a => a.GeographicId == viewModelGeographicSelected.GeographicId)];
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
            await Refresh();
            GeographicDataGrid.SelectedIndex = 0;
        }
        else
        {
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
            var index = ViewModel.GeographicItems.ToList().FindIndex(item => item.GeographicId == viewModelGeographicSelected.GeographicId);
            GeographicDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.GeographicDataSetCount = ViewModel.Tbl87GeographicsList.Count;
    }

    private void BackGeographic_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Geographic Methods]        

    #region [Species Methods]

    private async void ViewSpeciesHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSpeciesHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }

    }

    private Task ViewSpeciesHyperlink()
    {
        ViewModel.IsInEditSpecies = false;
        ViewModel.IsNewSpecies = false;
        SpeciesDetailExpander.IsExpanded = true;
        SpeciesDetailExpander.Visibility = Visibility.Visible;
        if (SpeciesDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSpeciesAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSpecies)
        {
            SpeciesDetailExpander.IsExpanded = false;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSpeciesEdits();
        }
    }

    private void CancelSpeciesEdits()
    {
        var x = (DataGrid)FindName("SpeciesDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[iNdx];

            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
            SpeciesSingles();
        }
        else
        {
            SpeciesDetailExpander.IsExpanded = false;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSpecies = false;
        ViewModel.IsNewSpecies = false;
    }

    private void SpeciesSingles()
    {
        ViewModel.SpeciesSelected.SpeciesName = SingleSpecies.SpeciesName;
        ViewModel.SpeciesSelected.Subspecies = SingleSpecies.Subspecies;
        ViewModel.SpeciesSelected.Divers = SingleSpecies.Divers;
        ViewModel.SpeciesSelected.GenusId = SingleSpecies.GenusId;
        ViewModel.SpeciesSelected.SpeciesgroupId = SingleSpecies.SpeciesgroupId;
        ViewModel.SpeciesSelected.Valid = SingleSpecies.Valid;
        ViewModel.SpeciesSelected.ValidYear = SingleSpecies.ValidYear;
        ViewModel.SpeciesSelected.MemoSpecies = SingleSpecies.MemoSpecies;
        ViewModel.SpeciesSelected.TradeName = SingleSpecies.TradeName;
        ViewModel.SpeciesSelected.Author = SingleSpecies.Author;
        ViewModel.SpeciesSelected.AuthorYear = SingleSpecies.AuthorYear;
        ViewModel.SpeciesSelected.Importer = SingleSpecies.Importer;
        ViewModel.SpeciesSelected.ImportingYear = SingleSpecies.ImportingYear;
        ViewModel.SpeciesSelected.TypeSpecies = SingleSpecies.TypeSpecies;
        ViewModel.SpeciesSelected.LNumber = SingleSpecies.LNumber;
        ViewModel.SpeciesSelected.LOrigin = SingleSpecies.LOrigin;
        ViewModel.SpeciesSelected.LdaOrigin = SingleSpecies.LdaOrigin;
        ViewModel.SpeciesSelected.LdaNumber = SingleSpecies.LdaNumber;
        ViewModel.SpeciesSelected.BasinLength = SingleSpecies.BasinLength;
        ViewModel.SpeciesSelected.FishLength = SingleSpecies.FishLength;
        ViewModel.SpeciesSelected.Karnivore = SingleSpecies.Karnivore;
        ViewModel.SpeciesSelected.Herbivore = SingleSpecies.Herbivore;
        ViewModel.SpeciesSelected.Limnivore = SingleSpecies.Limnivore;
        ViewModel.SpeciesSelected.Omnivore = SingleSpecies.Omnivore;
        ViewModel.SpeciesSelected.MemoFoods = SingleSpecies.MemoFoods;
        ViewModel.SpeciesSelected.Difficult1 = SingleSpecies.Difficult1;
        ViewModel.SpeciesSelected.Difficult2 = SingleSpecies.Difficult2;
        ViewModel.SpeciesSelected.Difficult3 = SingleSpecies.Difficult3;
        ViewModel.SpeciesSelected.Difficult4 = SingleSpecies.Difficult4;
        ViewModel.SpeciesSelected.RegionTop = SingleSpecies.RegionTop;
        ViewModel.SpeciesSelected.RegionMiddle = SingleSpecies.RegionMiddle;
        ViewModel.SpeciesSelected.RegionBottom = SingleSpecies.RegionBottom;
        ViewModel.SpeciesSelected.MemoRegion = SingleSpecies.MemoRegion;
        ViewModel.SpeciesSelected.MemoTech = SingleSpecies.MemoTech;
        ViewModel.SpeciesSelected.Ph1 = SingleSpecies.Ph1;
        ViewModel.SpeciesSelected.Ph2 = SingleSpecies.Ph2;
        ViewModel.SpeciesSelected.Temp1 = SingleSpecies.Temp1;
        ViewModel.SpeciesSelected.Temp2 = SingleSpecies.Temp2;
        ViewModel.SpeciesSelected.Hardness1 = SingleSpecies.Hardness1;
        ViewModel.SpeciesSelected.Hardness2 = SingleSpecies.Hardness2;
        ViewModel.SpeciesSelected.CarboHardness1 = SingleSpecies.CarboHardness1;
        ViewModel.SpeciesSelected.CarboHardness2 = SingleSpecies.CarboHardness2;
        ViewModel.SpeciesSelected.MemoHusbandry = SingleSpecies.MemoHusbandry;
        ViewModel.SpeciesSelected.MemoBuilt = SingleSpecies.MemoBuilt;
        ViewModel.SpeciesSelected.MemoColor = SingleSpecies.MemoColor;
        ViewModel.SpeciesSelected.MemoSozial = SingleSpecies.MemoSozial;
        ViewModel.SpeciesSelected.MemoDomorphism = SingleSpecies.MemoDomorphism;
        ViewModel.SpeciesSelected.MemoBreeding = SingleSpecies.MemoBreeding;
        ViewModel.SpeciesSelected.MemoSpecial = SingleSpecies.MemoSpecial;
    }

    private async Task RevertChangesSpeciesAsync()
    {
        ViewModel.IsInEditSpecies = false;
        if (ViewModel.IsModifiedSpecies)
        {
            await RefreshSpeciesAsync();
            ViewModel.IsModifiedSpecies = false;
        }
    }

    private async Task RefreshSpeciesAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SpeciesDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
                SpeciesSingles();
            }
        });
    }

    private async void SaveSpecies_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSpecies();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSpecies()
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
                if (await BeforeSaveSpecies())
                {
                    return;
                }
                await ViewModel.SaveSpeciesAsync(ViewModel.SpeciesSelected);
                await BehindSaveSpecies(ViewModel.SpeciesSelected);
                await Refresh();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSpecies = true;
                await RevertChangesSpeciesAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSpecies()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(a => a.SpeciesName == ViewModel.SpeciesSelected.SpeciesName &&
                        a.Subspecies == ViewModel.SpeciesSelected.Subspecies &&
                        a.Divers == ViewModel.SpeciesSelected.Divers &&
                        a.SpeciesgroupId == ViewModel.SpeciesSelected.SpeciesgroupId &&
                        a.Creature == ViewModel.SpeciesSelected.Creature &&
                        a.GenusId == ViewModel.SpeciesSelected.GenusId)];
        });
        if (ViewModel.IsNewSpecies)
        {
            if (ViewModel.Tbl75SpeciessesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSpecies = true;
        return false;
    }

    private async Task BehindSaveSpecies(Tbl75Species viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(com => com.SpeciesId == viewModelSpeciesSelected.SpeciesId)];
        });
        ViewModel.RefreshSpeciesItems();

        if (ViewModel.Tbl75SpeciessesList.Count == 0)
        {
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl75Speciesses
                .Where(com => com.SpeciesId == viewModelSpeciesSelected.SpeciesId)];
            SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            await Refresh();
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDataGrid.ItemsSource = null;
        }
        else
        {
            SpeciessesExpander.Visibility = Visibility.Visible;
            SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
            SpeciesDataGrid.SelectedIndex = 0;
        }
        BackDetailExpanders();
        ViewModel.SpeciesDataSetCount = ViewModel.Tbl75SpeciessesList.Count;
    }

    private void BackSpecies_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    private void ComboCreature_OnSelectionChangedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = sender as ComboBox;
        if (cmb == null)
        {
            return;
        }

        var text = ((ComboBox)sender).SelectedItem as string;
        switch (text)
        {
            case "":    //neu
                FoodExpanderFiSpecies.Visibility = Visibility.Visible;
                BreedingExpanderFiSpecies.Visibility = Visibility.Visible;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Visible;
                SpecialExpanderFiSpecies.Visibility = Visibility.Visible;
                ReproductionExpanderPlSpecies.Visibility = Visibility.Visible;
                CultureExpanderPlSpecies.Visibility = Visibility.Visible;
                GlobalExpanderPlSpecies.Visibility = Visibility.Visible;
                break;
            case "Animalia":
                FoodExpanderFiSpecies.Visibility = Visibility.Visible;
                BreedingExpanderFiSpecies.Visibility = Visibility.Visible;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Visible;
                SpecialExpanderFiSpecies.Visibility = Visibility.Visible;
                ReproductionExpanderPlSpecies.Visibility = Visibility.Collapsed;
                CultureExpanderPlSpecies.Visibility = Visibility.Collapsed;
                GlobalExpanderPlSpecies.Visibility = Visibility.Collapsed;
                break;
            case "Plantae":
                ReproductionExpanderPlSpecies.Visibility = Visibility.Visible;
                CultureExpanderPlSpecies.Visibility = Visibility.Visible;
                GlobalExpanderPlSpecies.Visibility = Visibility.Visible;
                FoodExpanderFiSpecies.Visibility = Visibility.Collapsed;
                BreedingExpanderFiSpecies.Visibility = Visibility.Collapsed;
                HusbandryExpanderFiSpecies.Visibility = Visibility.Collapsed;
                SpecialExpanderFiSpecies.Visibility = Visibility.Collapsed;
                break;
            case "Fungi":
                break;
            case "Archaea":
                break;
            case "Bacteria":
                break;
            case "Chromista":
                break;
            case "Protozoa":
                break;
        }

        //var cmb = sender as ComboBox;
        //if (cmb != null)
        //{
        //    handle = !cmb.IsDropDownOpen;
        //}

        //Handle();
    }
    #endregion [Species Methods]  

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
            var x = (DataGrid)FindName("GeographicDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl87GeographicsList!.Count)
            {
                ViewModel.GeographicItems.Clear();
                foreach (var item in ViewModel.Tbl87GeographicsList)
                {
                    ViewModel.GeographicItems.Add(item);
                }

                ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];
            }
            GeographicDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl87GeographicsList ??= [];
                ViewModel.Tbl87GeographicsList = int.TryParse(_search, out var id)
                ? [.. Context.Tbl87Geographics
                .Where(a => a.GeographicId == id)]
                : [.. Context.Tbl87Geographics
                .Where(a => a.Info.StartsWith(_search))
                .OrderBy(a => a.Info)];
            });
            if (ViewModel.Tbl87GeographicsList is { Count: 0 })
            {
                ViewModel.Tbl87GeographicsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;

            if (ViewModel.IsDeletingGeographic)
            {
                if (iNdx == 0 && ViewModel.Tbl87GeographicsList.Count == 0)
                {
                    ViewModel.Tbl87GeographicsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];
            }
        }
        GeographicDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingGeographic = false;
    }

    private void BackDetailExpanders()
    {
        GeographicDetailExpander.IsExpanded = false;
        GeographicDetailExpander.Visibility = Visibility.Collapsed;

        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteGeographic.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteGeographic.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteGeographic.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteGeographic.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

