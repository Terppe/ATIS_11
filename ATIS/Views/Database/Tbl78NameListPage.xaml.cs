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

//  Tbl78NameListPage.xaml.cs Skriptdatum:  02.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl78NameListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl78NameViewModel ViewModel { get; set; }
    private Tbl78Name SingleName { get; set; }
    private Tbl75Species SingleSpecies { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl78NameListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl78NameViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor] 

    #region [Name Methods]

    private async void GetNamesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl78NamesList?.Clear();

            ViewModel.NameItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl78NamesList?.Clear();

                NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
                ViewModel.Tbl75SpeciessesList?.Clear();

                SpeciessesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingName = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl78NamesList ??= [];
                ViewModel.Tbl78NamesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl78Names
                .Where(a => a.NameId == id)]
                : [.. AppData.Data.Tbl78Names
                .Where(a => _search != null && a.NameName.StartsWith(_search))
                .OrderBy(a => a.NameName)];
            });

            NameDetailExpander.Visibility = Visibility.Collapsed;
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.NameDataSetCount = ViewModel.Tbl78NamesList.Count;

            if (ViewModel.Tbl78NamesList is { Count: 0 })
            {
                ViewModel.Tbl78NamesList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();

                AppBarButtonDeleteName.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelName.Visibility = Visibility.Visible;
                NameDetailExpander.Visibility = Visibility.Collapsed;
                SpeciessesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl78NameListPage));
            }
            else
            {
                NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
                ViewModel.NameSelected = ViewModel.Tbl78NamesList[0];
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

    private async void NameDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await NameDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task NameDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.NameSelected == null)
        {
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.NameSelected != null)
        {
            //--------------------------Species-----------------------------    
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
            .Where(com => com.SpeciesId == ViewModel.NameSelected.SpeciesId)];
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

    private async void AddName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddName();
            NameName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(NameName);
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

    private async Task AddName()
    {
        ViewModel.IsInEditName = true;
        ViewModel.IsNewName = true;

        await ViewModel.AddNameAsync();

        if (ViewModel.Tbl78NamesList != null)
        {
            ViewModel.NameSelected = ViewModel.Tbl78NamesList[0];
        }

        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;

        NameName.PlaceholderText = StringsRes.EnterText;
    }

    private void NameSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        NameSearchBox.Visibility = Visibility.Visible;
        NameSearchBox.Focus(FocusState.Programmatic);
        NameSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void NameDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl78NamesList.Sort);
    }

    private async void ViewNameHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewNameHyperlink();
            NameName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(NameName);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewNameHyperlink()
    {
        ViewModel.IsInEditName = false;
        ViewModel.IsNewName = false;
        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;

        if (NameDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleName = AppData.Data.SingleName(ViewModel.NameSelected.NameId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsNameAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewName)
        {
            NameDetailExpander.IsExpanded = false;
            NameDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelNameEdits();
        }
    }

    private void CancelNameEdits()
    {
        var x = (DataGrid)FindName("NameDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];
            SingleName = AppData.Data.SingleName(ViewModel.NameSelected.NameId);
            NameSingles();
        }
        else
        {
            NameDetailExpander.IsExpanded = false;
            NameDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditName = false;
        ViewModel.IsNewName = false;
    }

    private void NameSingles()
    {
        ViewModel.NameSelected.NameName = SingleName.NameName;
        ViewModel.NameSelected.SpeciesId = SingleName.SpeciesId;
        ViewModel.NameSelected.Valid = SingleName.Valid;
        ViewModel.NameSelected.ValidYear = SingleName.ValidYear;
        ViewModel.NameSelected.Info = SingleName.Info;
        ViewModel.NameSelected.Language = SingleName.Language;
        ViewModel.NameSelected.Memo = SingleName.Memo;
    }

    private async Task RevertChangesNameAsync()
    {
        ViewModel.IsInEditName = false;
        if (ViewModel.IsModifiedName)
        {
            await RefreshNameAsync();
            ViewModel.IsModifiedName = false;
        }
    }

    private Task RefreshNameAsync()
    {
        if (NameDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.NameSelected.NameId == 0)
            {
                return Task.CompletedTask;
            }
            SingleName = AppData.Data.SingleName(ViewModel.NameSelected.NameId);
            NameSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteName();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteName()
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
                await ViewModel.DeleteNameAsync(ViewModel.NameSelected);
                await Refresh();
                NameDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesNameAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveName();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveName()
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
                if (await BeforeSaveName())
                {
                    return;
                }
                await ViewModel.SaveNameAsync(ViewModel.NameSelected);
                await BehindSaveName(ViewModel.NameSelected);
                await Refresh();
                ViewModel.IsNewName = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedName = true;
                await RevertChangesNameAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveName()
    {
        _search = NameSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl78NamesList ??= [];
            ViewModel.Tbl78NamesList = [.. AppData.Data.Tbl78Names
            .Where(a => a.NameName == ViewModel.NameSelected.NameName &&
                        a.Language == ViewModel.NameSelected.Language &&
                        a.SpeciesId == ViewModel.NameSelected.SpeciesId)];
        });
            if (ViewModel.Tbl78NamesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        ViewModel.IsModifiedName = true;
        return false;
    }

    private async Task BehindSaveName(Tbl78Name viewModelNameSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl78NamesList ??= [];
            if (NameSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl78NamesList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl78Names
                    .Where(a => a.NameId == id)]
                    : [.. AppData.Data.Tbl78Names
                    .Where(a => _search != null && a.NameName.StartsWith(_search))
                    .OrderBy(a => a.NameName)
                    .ThenBy(a => a.Language)];
            }
        });

        ViewModel.RefreshNameItems();

        if (ViewModel.Tbl78NamesList.Count == 0)
        {
            ViewModel.Tbl78NamesList = [.. AppData.Data.Tbl78Names
                .Where(a => a.NameId == viewModelNameSelected.NameId)];
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
            await Refresh();
            NameDataGrid.SelectedIndex = 0;
        }
        else
        {
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
            var index = ViewModel.NameItems.ToList().FindIndex(item => item.NameId == viewModelNameSelected.NameId);
            NameDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.NameDataSetCount = ViewModel.Tbl78NamesList.Count;
    }

    private void BackName_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Name Methods]        

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
            SingleSpecies = AppData.Data.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
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

            SingleSpecies = AppData.Data.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
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
        ViewModel.SpeciesSelected.Creature = SingleSpecies.Creature;
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
                SingleSpecies = AppData.Data.SingleSpecies(ViewModel.SpeciesSelected.SpeciesId);
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
            ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
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
            ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
                .Where(com => com.SpeciesId == viewModelSpeciesSelected.SpeciesId)];
        });
        ViewModel.RefreshSpeciesItems();

        if (ViewModel.Tbl75SpeciessesList.Count == 0)
        {
            ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
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
            var x = (DataGrid)FindName("NameDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl78NamesList!.Count)
            {
                ViewModel.NameItems.Clear();
                foreach (var item in ViewModel.Tbl78NamesList)
                {
                    ViewModel.NameItems.Add(item);
                }

                ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];
            }
            NameDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl78NamesList ??= [];
                ViewModel.Tbl78NamesList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl78Names
                .Where(a => a.NameId == id)]
                : [.. AppData.Data.Tbl78Names
                .Where(a => _search != null && a.NameName.StartsWith(_search))
                .OrderBy(a => a.NameName)];
            });
            if (ViewModel.Tbl78NamesList is { Count: 0 })
            {
                ViewModel.Tbl78NamesList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;

            if (ViewModel.IsDeletingName)
            {
                if (iNdx == 0 && ViewModel.Tbl78NamesList.Count == 0)
                {
                    ViewModel.Tbl78NamesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];
            }
        }
        NameDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingName = false;
    }

    private void BackDetailExpanders()
    {
        NameDetailExpander.IsExpanded = false;
        NameDetailExpander.Visibility = Visibility.Collapsed;

        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteName.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteName.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteName.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteName.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

