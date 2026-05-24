
using ATIS.Helpers;
using ATIS.Infrastructure;
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

//  Tbl68SpeciesgroupListPage.xaml.cs Skriptdatum:  15.12.2025  10:32         

namespace ATIS.Views.Database;

public sealed partial class Tbl68SpeciesgroupListPage : Page
{

    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl68SpeciesgroupAltViewModel ViewModel { get; set; }
    private Tbl68Speciesgroup SingleSpeciesgroup { get; set; }
    private Tbl75Species SingleSpecies { get; set; }
    //private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]   

    #region [Constructor]

    public Tbl68SpeciesgroupListPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl68SpeciesgroupAltViewModel>();
            ButtonsNotVisible();
            ButtonsIconNotVisible();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor]   

    #region [Speciesgroup Methods]
    private async void GetSpeciesgroupsByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;

            ViewModel.IsLoading = true;

            ViewModel.Tbl68SpeciesgroupsList?.Clear();

            ViewModel.SpeciesgroupItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.Tbl68SpeciesgroupsList?.Clear();

                SpeciesgroupDataGrid.ItemsSource = ViewModel.Tbl68SpeciesgroupsList;
                ViewModel.Tbl75SpeciessesList?.Clear();

                SpeciessesExpander.Visibility = Visibility.Collapsed;

                ViewModel.IsLoading = false;
                ViewModel.IsDeletingSpeciesgroup = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl68SpeciesgroupsList ??= [];
                ViewModel.Tbl68SpeciesgroupsList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupId == id)]
                : [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => _search != null && a.SpeciesgroupName.StartsWith(_search))
                .OrderBy(a => a.SpeciesgroupName)
                .ThenBy(a => a.Subspeciesgroup)];
            });

            SpeciesgroupDetailExpander.Visibility = Visibility.Collapsed;
            SpeciessesExpander.Visibility = Visibility.Collapsed;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SpeciesgroupDataSetCount = ViewModel.Tbl68SpeciesgroupsList.Count;

            if (ViewModel.Tbl68SpeciesgroupsList is { Count: 0 })
            {
                ViewModel.Tbl68SpeciesgroupsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                AppBarButtonDeleteSpeciesgroup.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSpeciesgroup.Visibility = Visibility.Visible;
                SpeciesgroupDetailExpander.Visibility = Visibility.Collapsed;
                SpeciessesExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl68SpeciesgroupListPage));
            }
            else
            {
                SpeciesgroupDataGrid.ItemsSource = ViewModel.Tbl68SpeciesgroupsList;
                ViewModel.SpeciesgroupSelected = ViewModel.Tbl68SpeciesgroupsList[0];
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

    private async void SpeciesgroupDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SpeciesgroupDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SpeciesgroupDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SpeciesgroupSelected == null)
        {
            ViewModel.IsLoading = false;
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        if (ViewModel.SpeciesgroupSelected != null)
        {
            //--------------------------Species-----------------------------     
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                ViewModel.Tbl75SpeciessesList = [.. AppData.Data.Tbl75Speciesses
            .AsNoTracking()
            .Where(com => com.SpeciesgroupId == ViewModel.SpeciesgroupSelected.SpeciesgroupId)];
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

    private async void AddSpeciesgroup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSpeciesgroup();
            SpeciesgroupName.Loaded += (s, e) =>
            {
                SetFocusses.SetFocusAndChangePlaceholderText(SpeciesgroupName);
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

    private async Task AddSpeciesgroup()
    {
        ViewModel.IsInEditSpeciesgroup = true;
        ViewModel.IsNewSpeciesgroup = true;

        await ViewModel.AddSpeciesgroupAsync();

        if (ViewModel.Tbl68SpeciesgroupsList != null)
        {
            ViewModel.SpeciesgroupSelected = ViewModel.Tbl68SpeciesgroupsList[0];
        }

        SpeciesgroupDetailExpander.IsExpanded = true;
        SpeciesgroupDetailExpander.Visibility = Visibility.Visible;

        SpeciesgroupName.PlaceholderText = StringsRes.EnterText;
    }

    private void SpeciesgroupSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SpeciesgroupSearchBox.Visibility = Visibility.Visible;
        SpeciesgroupSearchBox.Focus(FocusState.Programmatic);
        SpeciesgroupSearchBox.PlaceholderText = StringsRes.SearchNameOrId;
    }

    private void SpeciesgroupDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl68SpeciesgroupsList.Sort);
    }

    private async void ViewSpeciesgroupHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSpeciesgroupHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private Task ViewSpeciesgroupHyperlink()
    {
        ViewModel.IsInEditSpeciesgroup = false;
        ViewModel.IsNewSpeciesgroup = false;
        SpeciesgroupDetailExpander.IsExpanded = true;
        SpeciesgroupDetailExpander.Visibility = Visibility.Visible;

        if (SpeciesgroupDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleSpeciesgroup = AppData.Data.SingleSpeciesgroup(ViewModel.SpeciesgroupSelected.SpeciesgroupId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSpeciesgroupAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSpeciesgroup)
        {
            SpeciesgroupDetailExpander.IsExpanded = false;
            SpeciesgroupDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        else
        {
            CancelSpeciesgroupEdits();
        }
    }

    private void CancelSpeciesgroupEdits()
    {
        var x = (DataGrid)FindName("SpeciesgroupDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is not -1)
        {
            ViewModel.SpeciesgroupSelected = ViewModel.Tbl68SpeciesgroupsList[iNdx];
            SingleSpeciesgroup = AppData.Data.SingleSpeciesgroup(ViewModel.SpeciesgroupSelected.SpeciesgroupId);
            SpeciesgroupSingles();
        }
        else
        {
            SpeciesgroupDetailExpander.IsExpanded = false;
            SpeciesgroupDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
        }
        ViewModel.IsInEditSpeciesgroup = false;
        ViewModel.IsNewSpeciesgroup = false;
    }

    private void SpeciesgroupSingles()
    {
        ViewModel.SpeciesgroupSelected.SpeciesgroupName = SingleSpeciesgroup.SpeciesgroupName;
        ViewModel.SpeciesgroupSelected.Subspeciesgroup = SingleSpeciesgroup.Subspeciesgroup;
        ViewModel.SpeciesgroupSelected.Valid = SingleSpeciesgroup.Valid;
        ViewModel.SpeciesgroupSelected.ValidYear = SingleSpeciesgroup.ValidYear;
        ViewModel.SpeciesgroupSelected.Author = SingleSpeciesgroup.Author;
        ViewModel.SpeciesgroupSelected.AuthorYear = SingleSpeciesgroup.AuthorYear;
        ViewModel.SpeciesgroupSelected.Info = SingleSpeciesgroup.Info;
        ViewModel.SpeciesgroupSelected.Synonym = SingleSpeciesgroup.Synonym;
        ViewModel.SpeciesgroupSelected.EngName = SingleSpeciesgroup.EngName;
        ViewModel.SpeciesgroupSelected.GerName = SingleSpeciesgroup.GerName;
        ViewModel.SpeciesgroupSelected.FraName = SingleSpeciesgroup.FraName;
        ViewModel.SpeciesgroupSelected.PorName = SingleSpeciesgroup.PorName;
        ViewModel.SpeciesgroupSelected.Memo = SingleSpeciesgroup.Memo;
    }

    private async Task RevertChangesSpeciesgroupAsync()
    {
        ViewModel.IsInEditSpeciesgroup = false;
        if (ViewModel.IsModifiedSpeciesgroup)
        {
            await RefreshSpeciesgroupAsync();
            ViewModel.IsModifiedSpeciesgroup = false;
        }
    }

    private Task RefreshSpeciesgroupAsync()
    {
        if (SpeciesgroupDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            if (ViewModel.SpeciesgroupSelected.SpeciesgroupId == 0)
            {
                return Task.CompletedTask;
            }
            SingleSpeciesgroup = AppData.Data.SingleSpeciesgroup(ViewModel.SpeciesgroupSelected.SpeciesgroupId);
            SpeciesgroupSingles();
        }

        return Task.CompletedTask;
    }

    private async void DeleteSpeciesgroup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSpeciesgroup();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSpeciesgroup()
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
                await ViewModel.DeleteSpeciesgroupAsync(ViewModel.SpeciesgroupSelected);
                await Refresh();
                SpeciesgroupDataGrid.SelectedIndex = 0;
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSpeciesgroupAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveSpeciesgroup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveSpeciesgroup();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveSpeciesgroup()
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
                if (await BeforeSaveSpeciesgroup())
                {
                    return;
                }
                await ViewModel.SaveSpeciesgroupAsync(ViewModel.SpeciesgroupSelected);
                await BehindSaveSpeciesgroup(ViewModel.SpeciesgroupSelected);
                await Refresh();
                ViewModel.IsNewSpeciesgroup = false;
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSpeciesgroup = true;
                await RevertChangesSpeciesgroupAsync();
                ButtonsVisible();
                ButtonsIconVisible();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveSpeciesgroup()
    {
        _search = SpeciesgroupSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl68SpeciesgroupsList ??= [];
            ViewModel.Tbl68SpeciesgroupsList = [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupName == ViewModel.SpeciesgroupSelected.SpeciesgroupName &&
                        a.Subspeciesgroup == ViewModel.SpeciesgroupSelected.Subspeciesgroup)];
        });
        if (ViewModel.IsNewSpeciesgroup)
        {
            if (ViewModel.Tbl68SpeciesgroupsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSpeciesgroup = true;
        return false;
    }

    private async Task BehindSaveSpeciesgroup(Tbl68Speciesgroup viewModelSpeciesgroupSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl68SpeciesgroupsList ??= [];
            ViewModel.Tbl68SpeciesgroupsList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl68Speciesgroups
                    .Where(a => a.SpeciesgroupId == id)]
                : [.. AppData.Data.Tbl68Speciesgroups
                    .Where(a => _search != null && a.SpeciesgroupName.StartsWith(_search))
                    .OrderBy(a => a.SpeciesgroupName)
                    .ThenBy(a => a.Subspeciesgroup)];
        });

        ViewModel.RefreshSpeciesgroupItems();

        if (ViewModel.Tbl68SpeciesgroupsList.Count == 0)
        {
            ViewModel.Tbl68SpeciesgroupsList = [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupId == viewModelSpeciesgroupSelected.SpeciesgroupId)];
            SpeciesgroupDataGrid.ItemsSource = ViewModel.Tbl68SpeciesgroupsList;
            await Refresh();
            SpeciesgroupDataGrid.SelectedIndex = 0;
        }
        else
        {
            SpeciesgroupDataGrid.ItemsSource = ViewModel.Tbl68SpeciesgroupsList;
            var index = ViewModel.SpeciesgroupItems.ToList().FindIndex(item => item.SpeciesgroupId == viewModelSpeciesgroupSelected.SpeciesgroupId);
            SpeciesgroupDataGrid.SelectedIndex = index;
        }
        BackDetailExpanders();
        ViewModel.SpeciesgroupDataSetCount = ViewModel.Tbl68SpeciesgroupsList.Count;
    }

    private void SpeciesgroupName_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ChangeChar.UpperCaseFirstChar(sender);
    }

    private void BackSpeciesgroup_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Speciesgroup Methods]                    

    #region [Species Methods]

    private void DataGrid_Species_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl75SpeciessesList.Sort);
    }

    private void AddNewSpeciesCanceled(object sender, EventArgs e) => SpeciessesExpander.IsExpanded = false;

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
            SingleSpecies = AppData.Data.SingleSpeciesSpeciesgroup(ViewModel.SpeciesSelected.SpeciesId, ViewModel.SpeciesgroupSelected.SpeciesgroupId);
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

            SingleSpecies = AppData.Data.SingleSpeciesSpeciesgroup(ViewModel.SpeciesSelected.SpeciesId, ViewModel.SpeciesgroupSelected.SpeciesgroupId);
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
                SingleSpecies = AppData.Data.SingleSpeciesSpeciesgroup(ViewModel.SpeciesSelected.SpeciesId, ViewModel.SpeciesgroupSelected.SpeciesgroupId);
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
            var x = (DataGrid)FindName("SpeciesgroupDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl68SpeciesgroupsList!.Count)
            {
                ViewModel.SpeciesgroupItems.Clear();
                foreach (var item in ViewModel.Tbl68SpeciesgroupsList)
                {
                    ViewModel.SpeciesgroupItems.Add(item);
                }

                ViewModel.SpeciesgroupSelected = ViewModel.Tbl68SpeciesgroupsList[iNdx];
            }
            SpeciesgroupDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl68SpeciesgroupsList ??= [];
                ViewModel.Tbl68SpeciesgroupsList = int.TryParse(_search, out var id)
                ? [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => a.SpeciesgroupId == id)]
                : [.. AppData.Data.Tbl68Speciesgroups
                .Where(a => _search!= null && a.SpeciesgroupName.StartsWith(_search))
                    .OrderBy(a => a.SpeciesgroupName)
                    .ThenBy(a => a.Subspeciesgroup)];
            });
            if (ViewModel.Tbl68SpeciesgroupsList is { Count: 0 })
            {
                ViewModel.Tbl68SpeciesgroupsList?.Clear();
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.IsLoading = false;

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            SpeciesgroupDataGrid.ItemsSource = ViewModel.Tbl68SpeciesgroupsList;

            if (ViewModel.IsDeletingSpeciesgroup)
            {
                if (iNdx == 0 && ViewModel.Tbl68SpeciesgroupsList.Count == 0)
                {
                    ViewModel.Tbl68SpeciesgroupsList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SpeciesgroupSelected = ViewModel.Tbl68SpeciesgroupsList[iNdx];
            }
        }
        SpeciesgroupDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSpeciesgroup = false;
    }

    private void BackDetailExpanders()
    {
        SpeciesgroupDetailExpander.IsExpanded = false;
        SpeciesgroupDetailExpander.Visibility = Visibility.Collapsed;

        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconNotVisible()
    {
        AppBarButtonDeleteSpeciesgroup.Icon.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Icon.Visibility = Visibility.Collapsed;
    }
    private void ButtonsIconVisible()
    {
        AppBarButtonDeleteSpeciesgroup.Icon.Visibility = Visibility.Visible;
        AppBarButtonSync.Icon.Visibility = Visibility.Visible;
    }
    private void ButtonsVisible()
    {
        AppBarButtonDeleteSpeciesgroup.Visibility = Visibility.Visible;
        AppBarButtonSync.Visibility = Visibility.Visible;
    }
    private void ButtonsNotVisible()
    {
        AppBarButtonDeleteSpeciesgroup.Visibility = Visibility.Collapsed;
        AppBarButtonSync.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods]


}

