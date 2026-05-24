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

//  Tbl84SynonymListPage.xaml.cs Skriptdatum:  25.01.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl84SynonymListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl84SynonymAltViewModel ViewModel { get; set; }
    private Tbl84Synonym SingleSynonym { get; set; }
    private Tbl75Species SingleSpecies { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl84SynonymListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl84SynonymAltViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor] 

    #region [Synonym Methods]

    private async void GetSynonymsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl84SynonymsList?.Clear();

            ViewModel.SynonymItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl84SynonymsList?.Clear();

                SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
                ViewModel.Tbl75SpeciessesList?.Clear();

                SpeciessesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSynonym = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl84SynonymsList ??= [];
                ViewModel.Tbl84SynonymsList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl84Synonyms
                .Where(e => e.SynonymId == id)]
                : [.. AppData.Data.Tbl84Synonyms
                .Where(e => _search != null && e.SynonymName.StartsWith(_search))
                .OrderBy(a => a.SynonymName)];
            });

            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SynonymDataSetCount = ViewModel.Tbl84SynonymsList.Count;

            if (ViewModel.Tbl84SynonymsList is { Count: 0 })
            {
                ViewModel.Tbl84SynonymsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();

                AppBarButtonDeleteSynonym.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSynonym.Visibility = Visibility.Visible;
                SynonymDetailExpander.Visibility = Visibility.Collapsed;
                SpeciessesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl84SynonymListPage));
            }
            else
            {
                SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
                ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[0];
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

    private async void SynonymDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SynonymDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SynonymDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SynonymSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SynonymSelected != null)
        {
            //--------------------------Species-----------------------------   
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
            .Where(com => com.SpeciesId == ViewModel.SynonymSelected.SpeciesId)];
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

    private async void AddSynonym_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSynonym();
            SynonymName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SynonymName);
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

    private async Task AddSynonym()
    {
        ViewModel.IsInEditSynonym = true;
        ViewModel.IsNewSynonym = true;

        await ViewModel.AddSynonymAsync();

        if (ViewModel.Tbl84SynonymsList != null)
        {
            ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[0];
        }

        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;

        SynonymName.PlaceholderText = StringsRes.EnterText;
    }

    private void SynonymSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SynonymSearchBox.Visibility = Visibility.Visible;
        SynonymSearchBox.Focus(FocusState.Programmatic);
        SynonymSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SynonymDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl84SynonymsList.Sort);
    }


    private async void ViewSynonymHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSynonymHyperlink();
            SynonymName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SynonymName);
            };
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSynonymHyperlink()
    {
        ViewModel.IsInEditSynonym = false;
        ViewModel.IsNewSynonym = false;
        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;

        if (SynonymDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSynonym = AppData.Data.SingleSynonym(ViewModel.SynonymSelected.SynonymId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSynonymAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSynonym)
        {
            SynonymDetailExpander.IsExpanded = false;
            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSynonymEdits();
        }
    }

    private void CancelSynonymEdits()
    {
        var x = (DataGrid)FindName("SynonymDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];
            SingleSynonym = AppData.Data.SingleSynonym(ViewModel.SynonymSelected.SynonymId);
            SynonymSingles();
        }
        else
        {
            SynonymDetailExpander.IsExpanded = false;
            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSynonym = false;
        ViewModel.IsNewSynonym = false;
    }

    private void SynonymSingles()
    {
        ViewModel.SynonymSelected.SynonymName = SingleSynonym.SynonymName;
        ViewModel.SynonymSelected.SpeciesId = SingleSynonym.SpeciesId;
        ViewModel.SynonymSelected.Valid = SingleSynonym.Valid;
        ViewModel.SynonymSelected.ValidYear = SingleSynonym.ValidYear;
        ViewModel.SynonymSelected.Info = SingleSynonym.Info;
        ViewModel.SynonymSelected.Author = SingleSynonym.Author;
        ViewModel.SynonymSelected.AuthorYear = SingleSynonym.AuthorYear;
        ViewModel.SynonymSelected.Memo = SingleSynonym.Memo;
    }

    private async Task RevertChangesSynonymAsync()
    {
        ViewModel.IsInEditSynonym = false;
        if (ViewModel.IsModifiedSynonym)
        {
            await RefreshSynonymAsync();
            ViewModel.IsModifiedSynonym = false;
        }
    }

    private Task RefreshSynonymAsync()
    {
        if (SynonymDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.SynonymSelected.SynonymId == 0)
            {
                return Task.CompletedTask;
            }
            SingleSynonym = AppData.Data.SingleSynonym(ViewModel.SynonymSelected.SynonymId);
            SynonymSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSynonym_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSynonym();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSynonym()
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
                await ViewModel.DeleteSynonymAsync(ViewModel.SynonymSelected);
                await Refresh();
                SynonymDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSynonymAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSynonym_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSynonym();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSynonym()
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
                if (await BeforeSaveSynonym())
                {
                    return;
                }
                await ViewModel.SaveSynonymAsync(ViewModel.SynonymSelected);
                await BehindSaveSynonym(ViewModel.SynonymSelected);
                await Refresh();
                ViewModel.IsNewSynonym = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSynonym = true;
                await RevertChangesSynonymAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSynonym()
    {
        _search = SynonymSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl84SynonymsList ??= [];
            ViewModel.Tbl84SynonymsList = [.. AppData.Data.Tbl84Synonyms
            .Where(a => a.SynonymName == ViewModel.SynonymSelected.SynonymName &&
                        a.Author == ViewModel.SynonymSelected.Author &&
                        a.AuthorYear == ViewModel.SynonymSelected.AuthorYear &&
                        a.SpeciesId == ViewModel.SynonymSelected.SpeciesId)];
        });
            if (ViewModel.Tbl84SynonymsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        ViewModel.IsModifiedSynonym = true;
        return false;
    }

    private async Task BehindSaveSynonym(Tbl84Synonym viewModelSynonymSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl84SynonymsList ??= [];
            if (SynonymSearchBox.Text != "") //not empty searchbox id or name
            {
                ViewModel.Tbl84SynonymsList = int.TryParse(_search, out var id)
                    ? [.. AppData.Data.Tbl84Synonyms
                    .Where(a => a.SynonymId == id)]
                    : [.. AppData.Data.Tbl84Synonyms
                    .Where(a => _search != null && a.SynonymName.StartsWith(_search))
                    .OrderBy(a => a.SynonymName)];
            }
        });

        ViewModel.RefreshSynonymItems();

        if (ViewModel.Tbl84SynonymsList.Count == 0)
        {
            ViewModel.Tbl84SynonymsList = [.. AppData.Data.Tbl84Synonyms
                .Where(a => a.SynonymId == viewModelSynonymSelected.SynonymId)];
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
            await Refresh();
            SynonymDataGrid.SelectedIndex = 0;
        }
        else
        {
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
            var index = ViewModel.SynonymItems.ToList().FindIndex(item => item.SynonymId == viewModelSynonymSelected.SynonymId);
            SynonymDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.SynonymDataSetCount = ViewModel.Tbl84SynonymsList.Count;
    }

    private void BackSynonym_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Synonym Methods]        

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
            var x = (DataGrid)FindName("SynonymDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl84SynonymsList!.Count)
            {
                ViewModel.SynonymItems.Clear();
                foreach (var item in ViewModel.Tbl84SynonymsList)
                {
                    ViewModel.SynonymItems.Add(item);
                }

                ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];
            }
            SynonymDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl84SynonymsList ??= [];
                ViewModel.Tbl84SynonymsList = int.TryParse(_search, out var id)
                 ? [.. AppData.Data.Tbl84Synonyms
                .Where(a => a.SynonymId == id)]
                 : [.. AppData.Data.Tbl84Synonyms
                .Where(a => _search != null && a.SynonymName.StartsWith(_search))
                .OrderBy(a => a.SynonymName)];
            });
            if (ViewModel.Tbl84SynonymsList is { Count: 0 })
            {
                ViewModel.Tbl84SynonymsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;

            if (ViewModel.IsDeletingSynonym)
            {
                if (iNdx == 0 && ViewModel.Tbl84SynonymsList.Count == 0)
                {
                    ViewModel.Tbl84SynonymsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];
            }
        }
        SynonymDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSynonym = false;
    }

    private void BackDetailExpanders()
    {
        SynonymDetailExpander.IsExpanded = false;
        SynonymDetailExpander.Visibility = Visibility.Collapsed;

        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteSynonym.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteSynonym.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteSynonym.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteSynonym.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods] 


}

