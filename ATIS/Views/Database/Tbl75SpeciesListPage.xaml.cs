using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Authentication;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using WinRT.Interop;

//  Tbl75SpeciesListPage.xaml.cs Skriptdatum:  03.07.2025  10:32     

namespace ATIS.Views.Database;
public sealed partial class Tbl75SpeciesListPage : Page, Maps
{
    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public Tbl75SpeciesViewModel ViewModel { get; }
    private Tbl69FiSpecies SingleSpecies { get; set; }
    private Tbl90Reference SingleReference { get; set; }
    private Tbl93Comment SingleComment { get; set; }
    private Tbl78Name SingleName { get; set; }
    private Tbl81Image SingleImage { get; set; }
    private Tbl84Synonym SingleSynonym { get; set; }
    private Tbl87Geographic SingleGeographic { get; set; }
    public TblCountry SingleCountry { get; set; }

    private static readonly AtisDbContext Context = new();
    private string _search;
    private readonly FileOpenPicker _filePicker = new();


    private double centerOf1LongX = 0;
    private double centerOf1LatiY = 0;
    private int zoom = 0;

    #endregion [Private Data Members]

    #region [Constructor]

    public Tbl75SpeciesListPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<Tbl75SpeciesViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor]

    #region [Species Methods]

    private async void GetSpeciessesByNameOrId(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        try
        {
            _search = args.QueryText;
            ViewModel.IsLoading = true;

            ViewModel.Tbl75SpeciessesList?.Clear();
            ViewModel.SpeciesItems.Clear();

            const int milliseconds = 1500;
            await Task.Delay(milliseconds);

            if (string.IsNullOrEmpty(_search))
            {
                ViewModel.IsLoading = false;
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
            //if _search = "" und count = 0 you see the whole Tab thousands of datasets
            //change to % for all Datasets in 20 Parts of sets


            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.Tbl75SpeciessesList ??= [];
                //search about GenusName
                ViewModel.Tbl75SpeciessesList = int.TryParse(_search, out var id)
                    ? [.. Context.Tbl69FiSpeciesses
                    .Where(e => e.FiSpeciesId == id)]
                    : [.. Context.Tbl69FiSpeciesses
                    .Where(e => e.Tbl66Genusses.GenusName.StartsWith(_search))
                    .OrderBy(a => a.Tbl66Genusses.GenusName)
                    .ThenBy(a => a.FiSpeciesName)
                    .ThenBy(a => a.Subspecies)
                    .ThenBy(a => a.Divers)];
            });

            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            NamesExpander.Visibility = Visibility.Collapsed;
            NameDetailExpander.Visibility = Visibility.Collapsed;
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDetailExpander.Visibility = Visibility.Collapsed;
            SynonymsExpander.Visibility = Visibility.Collapsed;
            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            GeographicsExpander.Visibility = Visibility.Collapsed;
            GeographicDetailExpander.Visibility = Visibility.Collapsed;

            ViewModel.SpeciesDataSetCount = ViewModel.Tbl75SpeciessesList.Count;

            if (ViewModel.Tbl75SpeciessesList is { Count: 0 })
            {
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl78NamesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();
                ViewModel.Tbl84SynonymsList?.Clear();
                ViewModel.Tbl87GeographicsList?.Clear();

                AppBarButtonDeleteSpecies.IsEnabled = false;
                AppBarButtonAddName.IsEnabled = false;
                AppBarButtonAddImage.IsEnabled = false;
                AppBarButtonAddSynonym.IsEnabled = false;
                AppBarButtonAddGeographic.IsEnabled = false;
                AppBarButtonAddReference.IsEnabled = false;
                AppBarButtonAddComment.IsEnabled = false;
                AppBarButtonSync.IsEnabled = false;

                DetailPanelSpecies.Visibility = Visibility.Visible;
                SpeciesDetailExpander.Visibility = Visibility.Collapsed;
                ReferencesExpander.Visibility = Visibility.Collapsed;
                CommentsExpander.Visibility = Visibility.Collapsed;
                NamesExpander.Visibility = Visibility.Collapsed;
                ImagesExpander.Visibility = Visibility.Collapsed;
                SynonymsExpander.Visibility = Visibility.Collapsed;
                GeographicsExpander.Visibility = Visibility.Collapsed;

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                Frame.Navigate(typeof(Tbl75SpeciesListPage));
            }
            else
            {
                SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;
                ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[0];
            }
            ViewModel.IsLoading = false;
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void SpeciesDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
    {
        try
        {
            await SpeciesDataGridCurrentCellChanged();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SpeciesDataGridCurrentCellChanged()
    {
        ViewModel.IsLoading = true;

        if (ViewModel.SpeciesSelected == null)
        {
            return;
        }

        //delay loading
        const int milliseconds = 1500;
        await Task.Delay(milliseconds);

        //----------------------------------Reference--------------------------------
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(refe => refe.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.ReferenceDataSetCount = ViewModel.Tbl90ReferencesList.Count;
        ViewModel.RefreshReferenceItems();

        if (ViewModel.Tbl90ReferencesList.Count == 0)
        {
            ReferencesExpander.Visibility = Visibility.Collapsed;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
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
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.CommentDataSetCount = ViewModel.Tbl93CommentsList.Count;
        ViewModel.RefreshCommentItems();

        if (ViewModel.Tbl93CommentsList.Count == 0)
        {
            CommentsExpander.Visibility = Visibility.Collapsed;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            CommentDataGrid.ItemsSource = null;
        }
        else
        {
            CommentsExpander.Visibility = Visibility.Visible;
            CommentDataGrid.ItemsSource = ViewModel.Tbl93CommentsList;
        }

        //--------------------------Name-----------------------------          
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl78NamesList ??= [];
            ViewModel.Tbl78NamesList = [.. Context.Tbl78Names
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.NameDataSetCount = ViewModel.Tbl78NamesList.Count;
        ViewModel.RefreshNameItems();

        if (ViewModel.Tbl78NamesList.Count == 0)
        {
            NamesExpander.Visibility = Visibility.Collapsed;
            NameDetailExpander.Visibility = Visibility.Collapsed;
            NameDataGrid.ItemsSource = null;
        }
        else
        {
            NamesExpander.Visibility = Visibility.Visible;
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
        }

        //--------------------------Image-----------------------------          
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl81ImagesList ??= [];
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.ImageDataSetCount = ViewModel.Tbl81ImagesList.Count;
        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDetailExpander.Visibility = Visibility.Collapsed;
            ImageDataGrid.ItemsSource = null;
        }
        else
        {
            ImagesExpander.Visibility = Visibility.Visible;
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
        }

        //--------------------------Synonym-----------------------------          
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl84SynonymsList ??= [];
            ViewModel.Tbl84SynonymsList = [.. Context.Tbl84Synonyms
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.SynonymDataSetCount = ViewModel.Tbl84SynonymsList.Count;
        ViewModel.RefreshSynonymItems();

        if (ViewModel.Tbl84SynonymsList.Count == 0)
        {
            SynonymsExpander.Visibility = Visibility.Collapsed;
            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            SynonymDataGrid.ItemsSource = null;
        }
        else
        {
            SynonymsExpander.Visibility = Visibility.Visible;
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
        }

        //--------------------------Geographic-----------------------------          
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl87GeographicsList ??= [];
            ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });

        ViewModel.GeographicDataSetCount = ViewModel.Tbl87GeographicsList.Count;
        ViewModel.RefreshGeographicItems();

        if (ViewModel.Tbl87GeographicsList.Count == 0)
        {
            GeographicsExpander.Visibility = Visibility.Collapsed;
            GeographicDetailExpander.Visibility = Visibility.Collapsed;
            GeographicDataGrid.ItemsSource = null;
        }
        else
        {
            GeographicsExpander.Visibility = Visibility.Visible;
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
        }

        ViewModel.IsLoading = false;

    }

    private async void AddSpecies_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSpecies();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSpecies()
    {
        SpeciesName.Loaded += (s, e) =>
        {
            SpeciesName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSpecies = true;
        ViewModel.IsNewSpecies = true;

        await ViewModel.AddSpeciesAsync();

        if (ViewModel.Tbl75SpeciessesList != null)
        {
            ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[0];
        }

        DetailPanelSpecies.Visibility = Visibility.Visible;
        SpeciesDetailExpander.IsExpanded = true;
        SpeciesDetailExpander.Visibility = Visibility.Visible;

        ReferencesExpander.Visibility = Visibility.Collapsed;
        ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
        CommentsExpander.Visibility = Visibility.Collapsed;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        ViewModel.IsNewSpecies = false;
        SpeciesName.PlaceholderText = StringsRes.EnterText;

        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;

    }

    private void SpeciesSearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SpeciesSearchBox.Visibility = Visibility.Visible;
        SpeciesSearchBox.Focus(FocusState.Programmatic);
        SpeciesSearchBox.PlaceholderText = StringsRes.SearchGenusnameOrSpeciesId;
    }

    private void SpeciesDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl75SpeciessesList.Sort);
    }

    private void AddNewSpeciesCanceled(object sender, EventArgs e) => SpeciesDetailExpander.IsExpanded = false;
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
            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.FiSpeciesId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsSpeciesAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSpecies)
        {
            SpeciesDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditSpecies = false;
            ViewModel.IsNewSpecies = false;
            SpeciesDetailExpander.IsExpanded = false;
            SpeciesDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[iNdx];

        SpeciesSingles();

        ViewModel.IsInEditSpecies = false;
        ViewModel.IsNewSpecies = false;
        SpeciesDetailExpander.IsExpanded = true;
        SpeciesDetailExpander.Visibility = Visibility.Visible;
    }

    private void SpeciesSingles()
    {
        ViewModel.SpeciesSelected.FiSpeciesName = SingleSpecies.FiSpeciesName;
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
        ViewModel.SpeciesSelected.MemoSpecial = SingleSpecies.MemoSpecial;
        ViewModel.SpeciesSelected.MemoBreeding = SingleSpecies.MemoBreeding;
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
            SingleSpecies = App.SingleSpecies(ViewModel.SpeciesSelected.FiSpeciesId);

            SpeciesSingles();
        });
    }

    private async void DeleteSpecies_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteSpecies();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteSpecies()
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
                await ViewModel.DeleteSpeciesAsync(ViewModel.SpeciesSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesSpeciesAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

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
                await BehindSaveSpecies();
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
        _search = SpeciesSearchBox.Text;
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            ViewModel.Tbl75SpeciessesList = [.. Context.Tbl69FiSpeciesses
                .Where(a => a.FiSpeciesName == ViewModel.SpeciesSelected.FiSpeciesName &&
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

    private async Task BehindSaveSpecies()
    {
        var x = (DataGrid)FindName("SpeciesDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl75SpeciessesList ??= [];
            //search about GenusName
            ViewModel.Tbl75SpeciessesList = int.TryParse(_search, out var id)
            ? [.. Context.Tbl69FiSpeciesses
                .Where(e => e.FiSpeciesId == id)]
            : [.. Context.Tbl69FiSpeciesses
                .Where(e => e.Tbl66Genusses.GenusName.StartsWith(_search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.FiSpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)];
        });

        SpeciesDataGrid.SelectedIndex = iNdx;

        await Refresh();
        BackDetailExpanders();
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

    #region [Reference Methods]

    private void AddNewReferenceCanceled(object sender, EventArgs e) => ReferencesExpander.IsExpanded = false;

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
            SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);
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
                await BehindSaveReference();
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
                        a.FiSpeciesId == ViewModel.ReferenceSelected.FiSpeciesId &&
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
    private async Task BehindSaveReference()
    {
        var x = (DataGrid)FindName("ReferenceDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl90ReferencesList ??= [];
            ViewModel.Tbl90ReferencesList = [.. Context.Tbl90References
                .Where(refe => refe.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
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
        ReferenceDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceExpert()
    {
        InfoReferenceExpert.Loaded += (s, e) =>
        {
            InfoReferenceExpert.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceExpertAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceExpert)
        {
            ReferenceExpertDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceExpert = false;
            ViewModel.IsNewReferenceExpert = false;
            ReferenceExpertDetailExpander.IsExpanded = false;
            ReferenceExpertDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

        ReferenceExpertSingles();

        ViewModel.IsInEditReferenceExpert = false;
        ViewModel.IsNewReferenceExpert = false;
        ReferenceExpertDetailExpander.IsExpanded = true;
        ReferenceExpertDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceSource()
    {
        InfoReferenceSource.Loaded += (s, e) =>
        {
            InfoReferenceSource.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceSourceAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceSource)
        {
            ReferenceSourceDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceSource = false;
            ViewModel.IsNewReferenceSource = false;
            ReferenceSourceDetailExpander.IsExpanded = false;
            ReferenceSourceDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

        ReferenceSourceSingles();

        ViewModel.IsInEditReferenceSource = false;
        ViewModel.IsNewReferenceSource = false;
        ReferenceSourceDetailExpander.IsExpanded = true;
        ReferenceSourceDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddReferenceAuthor()
    {
        InfoReferenceAuthor.Loaded += (s, e) =>
        {
            InfoReferenceAuthor.Focus(FocusState.Programmatic);
        };

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
        ViewModel.IsNewReference = false;
    }

    private void CancelEditsReferenceAuthorAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewReferenceAuthor)
        {
            ReferenceAuthorDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditReferenceAuthor = false;
            ViewModel.IsNewReferenceAuthor = false;
            ReferenceAuthorDetailExpander.IsExpanded = false;
            ReferenceAuthorDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ReferenceSelected = ViewModel.Tbl90ReferencesList[iNdx];

        SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

        ReferenceAuthorSingles();

        ViewModel.IsInEditReferenceAuthor = false;
        ViewModel.IsNewReferenceAuthor = false;
        ReferenceAuthorDetailExpander.IsExpanded = true;
        ReferenceAuthorDetailExpander.Visibility = Visibility.Visible;
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
            SingleReference = App.SingleReferenceSpecies(ViewModel.ReferenceSelected.ReferenceId, ViewModel.SpeciesSelected.FiSpeciesId);

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
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddComment()
    {
        InfoComment.Loaded += (s, e) =>
        {
            InfoComment.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditComment = true;
        ViewModel.IsNewComment = true;

        await ViewModel.AddComment();

        if (ViewModel.Tbl93CommentsList != null)
        {
            ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[0];
        }

        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;

        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;

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
            SingleComment = App.SingleCommentSpecies(ViewModel.CommentSelected.CommentId, ViewModel.SpeciesSelected.FiSpeciesId);
        }
        return Task.CompletedTask;
    }

    private void CancelEditsCommentAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewComment)
        {
            CommentDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditComment = false;
            ViewModel.IsNewComment = false;
            CommentDetailExpander.IsExpanded = false;
            CommentDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.CommentSelected = ViewModel.Tbl93CommentsList[iNdx];

        SingleComment = App.SingleCommentSpecies(ViewModel.CommentSelected.CommentId, ViewModel.SpeciesSelected.FiSpeciesId);
        CommentSingles();

        ViewModel.IsInEditComment = false;
        ViewModel.IsNewComment = false;
        CommentDetailExpander.IsExpanded = true;
        CommentDetailExpander.Visibility = Visibility.Visible;
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
                SingleComment = App.SingleCommentSpecies(ViewModel.CommentSelected.CommentId, ViewModel.SpeciesSelected.FiSpeciesId);
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
                await BehindSaveComment();
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
                        a.FiSpeciesId == ViewModel.CommentSelected.FiSpeciesId)];

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
    private async Task BehindSaveComment()
    {
        var x = (DataGrid)FindName("CommentDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl93CommentsList ??= [];
            ViewModel.Tbl93CommentsList = [.. Context.Tbl93Comments
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
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
        CommentDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackComment_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Comment Methods]

    #region [Name Methods]

    private async void AddName_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddName();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddName()
    {
        NameName.Loaded += (s, e) =>
        {
            NameName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditName = true;
        ViewModel.IsNewName = true;

        await ViewModel.AddNameAsync();

        if (ViewModel.Tbl78NamesList != null)
        {
            ViewModel.NameSelected = ViewModel.Tbl78NamesList[0];
        }

        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;

        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;
    }

    private void DataGrid_Name_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl78NamesList.Sort);
    }

    private void AddNewNameCanceled(object sender, EventArgs e) => NamesExpander.IsExpanded = false;

    private async void ViewNameHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewNameHyperlink();
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
            SingleName = App.SingleNameSpecies(ViewModel.NameSelected.NameId, ViewModel.SpeciesSelected.FiSpeciesId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsNameAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewName)
        {
            NameDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditName = false;
            ViewModel.IsNewName = false;
            NameDetailExpander.IsExpanded = false;
            NameDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.NameSelected = ViewModel.Tbl78NamesList[iNdx];

        SingleName = App.SingleNameSpecies(ViewModel.NameSelected.NameId, ViewModel.SpeciesSelected.FiSpeciesId);
        NameSingles();

        ViewModel.IsInEditName = false;
        ViewModel.IsNewName = false;
        NameDetailExpander.IsExpanded = true;
        NameDetailExpander.Visibility = Visibility.Visible;
    }

    private void NameSingles()
    {
        ViewModel.NameSelected.NameName = SingleName.NameName;
        ViewModel.NameSelected.FiSpeciesId = SingleName.FiSpeciesId;
        ViewModel.NameSelected.Valid = SingleName.Valid;
        ViewModel.NameSelected.ValidYear = SingleName.ValidYear;
        ViewModel.NameSelected.Info = SingleName.Info;
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
    private async Task RefreshNameAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (NameDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleName = App.SingleNameSpecies(ViewModel.NameSelected.NameId, ViewModel.SpeciesSelected.FiSpeciesId);
                NameSingles();
            }
        });
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
                await BehindSaveName();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedName = true;
                await RevertChangesNameAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> BeforeSaveName()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl78NamesList ??= [];
            ViewModel.Tbl78NamesList = [.. Context.Tbl78Names
                .Where(a => a.NameName == ViewModel.NameSelected.NameName &&
                            a.FiSpeciesId == ViewModel.NameSelected.FiSpeciesId)];
        });
        if (ViewModel.IsNewName)
        {
            if (ViewModel.Tbl78NamesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedName = true;
        return false;
    }

    private async Task BehindSaveName()
    {
        var x = (DataGrid)FindName("NameDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl78NamesList ??= [];
            ViewModel.Tbl78NamesList = [.. Context.Tbl78Names
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
        ViewModel.RefreshNameItems();

        if (ViewModel.Tbl78NamesList.Count == 0)
        {
            NamesExpander.Visibility = Visibility.Collapsed;
            NameDataGrid.ItemsSource = null;
        }
        else
        {
            NamesExpander.Visibility = Visibility.Visible;
            NameDataGrid.ItemsSource = ViewModel.Tbl78NamesList;
        }
        NameDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackName_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Name Methods]

    #region [Image Methods]

    private async void AddImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddImage()
    {
        InfoName.Loaded += (s, e) =>
        {
            InfoName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditImage = true;
        ViewModel.IsNewImage = true;

        await ViewModel.AddImageAsync();

        if (ViewModel.Tbl81ImagesList != null)
        {
            ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[0];
        }

        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;

        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;

        //New Image search
        OpenFileDialog();
        ViewModel.RefreshImageItems();
    }

    private void DataGrid_Image_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl81ImagesList.Sort);
    }

    private void AddNewImageCanceled(object sender, EventArgs e) => ImagesExpander.IsExpanded = false;

    private async void ViewImageHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewImageHyperlink();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }
    private Task ViewImageHyperlink()
    {
        ViewModel.IsInEditImage = false;
        ViewModel.IsNewImage = false;
        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;
        if (ImageDataGrid.SelectedIndex == -1)
        {
            _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
        }
        else
        {
            SingleImage = App.SingleImageSpecies(ViewModel.ImageSelected.ImageId, ViewModel.SpeciesSelected.FiSpeciesId);
        }

        return Task.CompletedTask;
    }

    private void CancelEditsImageAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewImage)
        {
            ImageDetailExpander.IsExpanded = false;
            _ = Refresh();
        }
        else
        {
            CancelImageEdits();
        }
    }
    private void CancelImageEdits()
    {
        var x = (DataGrid)FindName("ImageDataGrid");
        var iNdx = x.SelectedIndex;
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditImage = false;
            ViewModel.IsNewImage = false;
            ImageDetailExpander.IsExpanded = false;
            ImageDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.ImageSelected = ViewModel.Tbl81ImagesList[iNdx];

        SingleImage = App.SingleImageSpecies(ViewModel.ImageSelected.ImageId, ViewModel.SpeciesSelected.FiSpeciesId);
        ImageSingles();

        ViewModel.IsInEditImage = false;
        ViewModel.IsNewImage = false;
        ImageDetailExpander.IsExpanded = true;
        ImageDetailExpander.Visibility = Visibility.Visible;
    }
    private void ImageSingles()
    {
        ViewModel.ImageSelected.FiSpeciesId = SingleImage.FiSpeciesId;
        ViewModel.ImageSelected.Valid = SingleImage.Valid;
        ViewModel.ImageSelected.ValidYear = SingleImage.ValidYear;
        ViewModel.ImageSelected.Filestream = SingleImage.Filestream;
        ViewModel.ImageSelected.FilestreamId = SingleImage.FilestreamId;
        ViewModel.ImageSelected.ImageData = SingleImage.ImageData;
        ViewModel.ImageSelected.ImageMimeType = SingleImage.ImageMimeType;
        ViewModel.ImageSelected.Info = SingleImage.Info;
        ViewModel.ImageSelected.Memo = SingleImage.Memo;
        ViewModel.ImageSelected.ShotDate = SingleImage.ShotDate;
    }
    private async Task RevertChangesImageAsync()
    {
        ViewModel.IsInEditImage = false;
        if (ViewModel.IsModifiedImage)
        {
            await RefreshImageAsync();
            ViewModel.IsModifiedImage = false;
        }
    }
    private async Task RefreshImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (ImageDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleImage = App.SingleImageSpecies(ViewModel.ImageSelected.ImageId, ViewModel.SpeciesSelected.FiSpeciesId);
                ImageSingles();
            }
        });
    }

    private async void DeleteImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await DeleteImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task DeleteImage()
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
                await ViewModel.DeleteImageAsync(ViewModel.ImageSelected);
                await Refresh();
                break;
            case DeleteDialogResult.DontDelete:
                await RevertChangesImageAsync();
                break;
            case DeleteDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void SaveImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SaveImage();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task SaveImage()
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
                if (await BeforeSaveImage())
                {
                    return;
                }
                await ViewModel.SaveImageAsync(ViewModel.ImageSelected, ViewModel.SelectedPath);
                await BehindSaveImage();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedImage = true;
                await RevertChangesImageAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private async Task<bool> BeforeSaveImage()
    {
        ViewModel.Tbl81ImagesList ??= [];
        ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
            .Where(a => a.Info == ViewModel.ImageSelected.Info &&
                        a.PlSpeciesId == ViewModel.ImageSelected.PlSpeciesId &&
                        a.FiSpeciesId == ViewModel.ImageSelected.FiSpeciesId)];

        if (ViewModel.IsNewImage)
        {
            if (ViewModel.Tbl81ImagesList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedImage = true;
        return false;
    }
    private async Task BehindSaveImage()
    {
        var x = (DataGrid)FindName("ImageDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl81ImagesList ??= [];
            ViewModel.Tbl81ImagesList = [.. Context.Tbl81Images
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
        ViewModel.RefreshImageItems();

        if (ViewModel.Tbl81ImagesList.Count == 0)
        {
            ImagesExpander.Visibility = Visibility.Collapsed;
            ImageDataGrid.ItemsSource = null;
        }
        else
        {
            ImagesExpander.Visibility = Visibility.Visible;
            ImageDataGrid.ItemsSource = ViewModel.Tbl81ImagesList;
        }
        ImageDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private static byte[] LoadImageData(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new BinaryReader(fs);
        var imageBytes = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return imageBytes;
    }

    private void BackImage_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    private async void OpenFileDialog()
    {
        try
        {
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(_filePicker, hwnd);
            _filePicker.FileTypeFilter.Add("*");
            _filePicker.ViewMode = PickerViewMode.Thumbnail;
            //_filePicker.FileTypeFilter.Add("*");
            _filePicker.FileTypeFilter.Add(".jpg");
            _filePicker.FileTypeFilter.Add(".jpeg");
            _filePicker.FileTypeFilter.Add(".png");
            _filePicker.FileTypeFilter.Add(".gif");
            _filePicker.FileTypeFilter.Add(".jpe");
            _filePicker.FileTypeFilter.Add(".bmp");
            _filePicker.FileTypeFilter.Add(".ico");
            _filePicker.FileTypeFilter.Add(".tif");
            _filePicker.FileTypeFilter.Add(".tiff");
            _filePicker.FileTypeFilter.Add(".hpd");
            _filePicker.FileTypeFilter.Add(".jxr");
            _filePicker.FileTypeFilter.Add(".wdp");
            _filePicker.FileTypeFilter.Add(".hpd");
            var file = await _filePicker.PickSingleFileAsync();
            if (file != null && file.Path != null)
            {
                SelectedPathImage.Text = file.Path;
                Image.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async void ChangeImage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(_filePicker, hwnd);
            _filePicker.FileTypeFilter.Add("*");
            _filePicker.ViewMode = PickerViewMode.Thumbnail;
            //_filePicker.FileTypeFilter.Add("*");
            _filePicker.FileTypeFilter.Add(".jpg");
            _filePicker.FileTypeFilter.Add(".jpeg");
            _filePicker.FileTypeFilter.Add(".png");
            _filePicker.FileTypeFilter.Add(".gif");
            _filePicker.FileTypeFilter.Add(".jpe");
            _filePicker.FileTypeFilter.Add(".bmp");
            _filePicker.FileTypeFilter.Add(".ico");
            _filePicker.FileTypeFilter.Add(".tif");
            _filePicker.FileTypeFilter.Add(".tiff");
            _filePicker.FileTypeFilter.Add(".hpd");
            _filePicker.FileTypeFilter.Add(".jxr");
            _filePicker.FileTypeFilter.Add(".wdp");
            _filePicker.FileTypeFilter.Add(".hpd");
            var file = await _filePicker.PickSingleFileAsync();
            if (file != null && file.Path != null)
            {
                SelectedPathImage.Text = file.Path;
                Image.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    #endregion [Image Methods]

    #region [Synonym Methods]

    private async void AddSynonym_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddSynonym();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddSynonym()
    {
        SynonymName.Loaded += (s, e) =>
        {
            SynonymName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditSynonym = true;
        ViewModel.IsNewSynonym = true;

        await ViewModel.AddSynonymAsync();

        if (ViewModel.Tbl84SynonymsList != null)
        {
            ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[0];
        }

        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;

        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;
    }

    private void DataGrid_Synonym_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl84SynonymsList.Sort);
    }
    private void AddNewSynonymCanceled(object sender, EventArgs e) => SynonymsExpander.IsExpanded = false;

    private async void ViewSynonymHyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ViewSynonymHyperlink();
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
            SingleSynonym = App.SingleSynonymSpecies(ViewModel.SynonymSelected.SynonymId, ViewModel.SpeciesSelected.FiSpeciesId);
        }

        return Task.CompletedTask;
    }
    private void CancelEditsSynonymAsync(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsNewSynonym)
        {
            SynonymDetailExpander.IsExpanded = false;
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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditSynonym = false;
            ViewModel.IsNewSynonym = false;
            SynonymDetailExpander.IsExpanded = false;
            SynonymDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.SynonymSelected = ViewModel.Tbl84SynonymsList[iNdx];

        SingleSynonym = App.SingleSynonymSpecies(ViewModel.SynonymSelected.SynonymId, ViewModel.SpeciesSelected.FiSpeciesId);
        SynonymSingles();

        ViewModel.IsInEditSynonym = false;
        ViewModel.IsNewSynonym = false;
        SynonymDetailExpander.IsExpanded = true;
        SynonymDetailExpander.Visibility = Visibility.Visible;
    }
    private void SynonymSingles()
    {
        ViewModel.SynonymSelected.SynonymName = SingleSynonym.SynonymName;
        ViewModel.SynonymSelected.FiSpeciesId = SingleSynonym.FiSpeciesId;
        ViewModel.SynonymSelected.Valid = SingleSynonym.Valid;
        ViewModel.SynonymSelected.ValidYear = SingleSynonym.ValidYear;
        ViewModel.SynonymSelected.Info = SingleSynonym.Info;
        ViewModel.SynonymSelected.Memo = SingleSynonym.Memo;
        ViewModel.SynonymSelected.Author = SingleSynonym.Author;
        ViewModel.SynonymSelected.AuthorYear = SingleSynonym.AuthorYear;
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

    private async Task RefreshSynonymAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (SynonymDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleSynonym = App.SingleSynonymSpecies(ViewModel.SynonymSelected.SynonymId, ViewModel.SpeciesSelected.FiSpeciesId);
                NameSingles();
            }
        });
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
                await BehindSaveSynonym();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedSynonym = true;
                await RevertChangesSynonymAsync();
                break;
            case SaveChangesDialogResult.Cancel:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private async Task<bool> BeforeSaveSynonym()
    {
        ViewModel.Tbl84SynonymsList ??= [];
        ViewModel.Tbl84SynonymsList = [.. Context.Tbl84Synonyms
            .Where(a => a.SynonymName == ViewModel.SynonymSelected.SynonymName &&
                        a.Author == ViewModel.SynonymSelected.Author &&
                        a.PlSpeciesId == ViewModel.SynonymSelected.PlSpeciesId &&
                        a.FiSpeciesId == ViewModel.SynonymSelected.FiSpeciesId)];

        if (ViewModel.IsNewSynonym)
        {
            if (ViewModel.Tbl84SynonymsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedSynonym = true;
        return false;
    }
    private async Task BehindSaveSynonym()
    {
        var x = (DataGrid)FindName("SynonymDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl84SynonymsList ??= [];
            ViewModel.Tbl84SynonymsList = [.. Context.Tbl84Synonyms
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
        ViewModel.RefreshSynonymItems();

        if (ViewModel.Tbl84SynonymsList.Count == 0)
        {
            SynonymsExpander.Visibility = Visibility.Collapsed;
            SynonymDataGrid.ItemsSource = null;
        }
        else
        {
            SynonymsExpander.Visibility = Visibility.Visible;
            SynonymDataGrid.ItemsSource = ViewModel.Tbl84SynonymsList;
        }
        SynonymDataGrid.SelectedIndex = iNdx;

        BackDetailExpanders();
    }

    private void BackSynonym_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Synonym Methods]

    #region [Geographic Methods]

    private async void AddGeographic_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BackDetailExpanders();
            await AddGeographic();
        }
        catch (Exception xe)
        {
            await AllDialogs.ErrorMessageDialogAsync(xe.InnerException?.ToString());
            SimpleLog.Log(xe);
        }
    }

    private async Task AddGeographic()
    {
        InfoName.Loaded += (s, e) =>
        {
            InfoName.Focus(FocusState.Programmatic);
        };

        ViewModel.IsInEditGeographic = true;
        ViewModel.IsNewGeographic = true;

        await ViewModel.AddGeographicAsync();

        if (ViewModel.Tbl87GeographicsList != null)
        {
            ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[0];
        }

        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;

        //Map
        //wenn Adress leer in Countrylist suchen
        centerOf1LongX = Convert.ToDouble(Longitude1Geographic.Text);
        centerOf1LatiY = Convert.ToDouble(Latitude1Geographic.Text);
        zoom = (int)Convert.ToDouble(ZoomLevelGeographic.Text);

        var graphSelect = ViewModel.GeographicSelected;
        MyMap.Map = await ((Maps)this).CreateMapAsync(centerOf1LongX, centerOf1LatiY, zoom, graphSelect);


        SpecificationExpanderFiSpecies.IsExpanded = false;
        FoodExpanderFiSpecies.IsExpanded = false;
        TechnikExpanderFiSpecies.IsExpanded = false;
        HusbandryExpanderFiSpecies.IsExpanded = false;
        BreedingExpanderFiSpecies.IsExpanded = false;
        BuiltExpanderFiSpecies.IsExpanded = false;
        SpecialExpanderFiSpecies.IsExpanded = false;
        ReproductionExpanderPlSpecies.IsExpanded = false;
        CultureExpanderPlSpecies.IsExpanded = false;
        GlobalExpanderPlSpecies.IsExpanded = false;
    }

    private void DataGrid_Geographic_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid).Sort(e.Column, ViewModel.Tbl87GeographicsList.Sort);
    }

    private void AddNewGeographicCanceled(object sender, EventArgs e) => GeographicsExpander.IsExpanded = false;

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
            SingleGeographic = App.SingleGeographicSpecies(ViewModel.GeographicSelected.GeographicId, ViewModel.SpeciesSelected.FiSpeciesId);

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
        if (iNdx is -1 or 0)
        {
            ViewModel.IsInEditGeographic = false;
            ViewModel.IsNewGeographic = false;
            GeographicDetailExpander.IsExpanded = false;
            GeographicDetailExpander.Visibility = Visibility.Collapsed;
            _ = Refresh();
            return;
        }

        ViewModel.GeographicSelected = ViewModel.Tbl87GeographicsList[iNdx];

        SingleGeographic = App.SingleGeographicSpecies(ViewModel.GeographicSelected.GeographicId, ViewModel.SpeciesSelected.FiSpeciesId);
        ImageSingles();

        ViewModel.IsInEditGeographic = false;
        ViewModel.IsNewGeographic = false;
        GeographicDetailExpander.IsExpanded = true;
        GeographicDetailExpander.Visibility = Visibility.Visible;
    }
    private void GeographicSingles()
    {
        ViewModel.GeographicSelected.FiSpeciesId = SingleGeographic.FiSpeciesId;
        ViewModel.GeographicSelected.Valid = SingleGeographic.Valid;
        ViewModel.GeographicSelected.ValidYear = SingleGeographic.ValidYear;
        ViewModel.GeographicSelected.Author = SingleGeographic.Author;
        ViewModel.GeographicSelected.AuthorYear = SingleGeographic.AuthorYear;
        ViewModel.GeographicSelected.Continent = SingleGeographic.Continent;
        ViewModel.GeographicSelected.Country = SingleGeographic.Country;
        ViewModel.GeographicSelected.Address = SingleGeographic.Address;
        ViewModel.GeographicSelected.Info = SingleGeographic.Info;
        ViewModel.GeographicSelected.Memo = SingleGeographic.Memo;
        ViewModel.GeographicSelected.Http = SingleGeographic.Http;
        ViewModel.GeographicSelected.ZoomLevel = SingleGeographic.ZoomLevel;
        ViewModel.GeographicSelected.Latitude = SingleGeographic.Latitude;
        ViewModel.GeographicSelected.Latitude1 = SingleGeographic.Latitude1;
        ViewModel.GeographicSelected.Latitude2 = SingleGeographic.Latitude2;
        ViewModel.GeographicSelected.Latitude3 = SingleGeographic.Latitude3;
        ViewModel.GeographicSelected.Longitude = SingleGeographic.Longitude;
        ViewModel.GeographicSelected.Longitude1 = SingleGeographic.Longitude1;
        ViewModel.GeographicSelected.Longitude2 = SingleGeographic.Longitude2;
        ViewModel.GeographicSelected.Longitude3 = SingleGeographic.Longitude3;
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
    private async Task RefreshGeographicAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (GeographicDataGrid.SelectedIndex == -1)
            {
                _ = AllDialogs.NoColumnSelectedWarnMessageDialogAsync();
            }
            else
            {
                SingleGeographic = App.SingleGeographicSpecies(ViewModel.GeographicSelected.GeographicId, ViewModel.SpeciesSelected.FiSpeciesId);
                GeographicSingles();
            }
        });
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
                break;
            case DeleteDialogResult.DontDelete:
                ViewModel.IsModifiedGeographic = true;
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
                await BehindSaveGeographic();
                break;
            case SaveChangesDialogResult.DontSave:
                ViewModel.IsModifiedGeographic = true;
                await RevertChangesGeographicAsync();
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
                        a.PlSpeciesId == ViewModel.GeographicSelected.PlSpeciesId &&
                        a.FiSpeciesId == ViewModel.GeographicSelected.FiSpeciesId)];

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

        if (ViewModel.IsNewGeographic)
        {
            if (ViewModel.Tbl87GeographicsList.Count != 0)
            {
                await AllDialogs.DatasetExistInfoMessageDialogAsync();
                return true;
            }
        }
        ViewModel.IsModifiedGeographic = true;
        return false;
    }
    private async Task BehindSaveGeographic()
    {
        var x = (DataGrid)FindName("GeographicDataGrid");
        var iNdx = x.SelectedIndex;

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            ViewModel.Tbl87GeographicsList ??= [];
            ViewModel.Tbl87GeographicsList = [.. Context.Tbl87Geographics
                .AsNoTracking()
                .Where(com => com.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
        });
        ViewModel.RefreshGeographicItems();

        if (ViewModel.Tbl87GeographicsList.Count == 0)
        {
            GeographicsExpander.Visibility = Visibility.Collapsed;
            GeographicDataGrid.ItemsSource = null;
        }
        else
        {
            GeographicsExpander.Visibility = Visibility.Visible;
            GeographicDataGrid.ItemsSource = ViewModel.Tbl87GeographicsList;
        }
        GeographicDataGrid.SelectedIndex = iNdx;

        MapExpanderGeographic.IsExpanded = true;


        // BackDetailExpanders();
    }

    private void BackGeographic_Click(object sender, RoutedEventArgs e)
    {
        BackDetailExpanders();
    }

    #endregion [Geographic Methods]

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
            //DetailPanelSpecies.Visibility = Visibility.Visible;
            //SpeciesDetailExpander.Visibility = Visibility.Visible;
        }
        else
        {
            //   Index Position ?
            var x = (DataGrid)FindName("SpeciesDataGrid");
            var iNdx = x.SelectedIndex;

            if (iNdx < ViewModel.Tbl75SpeciessesList!.Count)
            {
                ViewModel.SpeciesItems.Clear();
                foreach (var item in ViewModel.Tbl75SpeciessesList)
                {
                    ViewModel.SpeciesItems.Add(item);
                }

                ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[iNdx];
            }

            SpeciesDataGrid.SelectedIndex = iNdx;

            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (ViewModel.Tbl75SpeciessesList.Count == 0)
                {
                    ViewModel.Tbl75SpeciessesList ??= [];
                    ViewModel.Tbl75SpeciessesList = [.. Context.Tbl69FiSpeciesses
                            .Where(a => a.FiSpeciesId == ViewModel.SpeciesSelected.FiSpeciesId)];
                }
                else
                {
                    ViewModel.Tbl75SpeciessesList ??= [];
                    ViewModel.Tbl75SpeciessesList = int.TryParse(_search, out var id)
                        ? [.. Context.Tbl69FiSpeciesses
                                .Where(a => a.FiSpeciesId == id)]
                        : [.. Context.Tbl69FiSpeciesses
                                .Where(a => a.Tbl66Genusses.GenusName.StartsWith(_search))
                                .OrderBy(a => a.Tbl66Genusses.GenusName)
                                .ThenBy(a => a.FiSpeciesName)
                                .ThenBy(a => a.Subspecies)
                                .ThenBy(a => a.Divers)];
                }
            });

            SpeciesDataGrid.ItemsSource = ViewModel.Tbl75SpeciessesList;

            if (ViewModel.IsDeletingSpecies)
            {
                if (iNdx == 0 && ViewModel.Tbl75SpeciessesList.Count == 0)
                {
                    ViewModel.Tbl75SpeciessesList.Clear();
                    return;
                }
            }
            else
            {
                ViewModel.SpeciesSelected = ViewModel.Tbl75SpeciessesList[iNdx];
            }

            if (ViewModel.Tbl75SpeciessesList is { Count: 0 })
            {
                ViewModel.Tbl75SpeciessesList?.Clear();
                ViewModel.Tbl90ReferencesList?.Clear();
                ViewModel.Tbl93CommentsList?.Clear();
                ViewModel.Tbl78NamesList?.Clear();
                ViewModel.Tbl81ImagesList?.Clear();
                ViewModel.Tbl84SynonymsList?.Clear();
                ViewModel.Tbl87GeographicsList?.Clear();

                BackDetailExpanders();

                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }
        }

        SpeciesDataGrid_OnCurrentCellChanged(null, null);

        ViewModel.IsLoading = false;
        ViewModel.IsDeletingSpecies = false;
    }

    private void BackDetailExpanders()
    {
        SpeciesDetailExpander.IsExpanded = false;
        SpeciesDetailExpander.Visibility = Visibility.Collapsed;

        ReferenceDetailExpander.IsExpanded = false;
        ReferenceDetailExpander.Visibility = Visibility.Collapsed;

        CommentDetailExpander.IsExpanded = false;
        CommentDetailExpander.Visibility = Visibility.Collapsed;

        NameDetailExpander.IsExpanded = false;
        NameDetailExpander.Visibility = Visibility.Collapsed;

        ImageDetailExpander.IsExpanded = false;
        ImageDetailExpander.Visibility = Visibility.Collapsed;

        SynonymDetailExpander.IsExpanded = false;
        SynonymDetailExpander.Visibility = Visibility.Collapsed;

        GeographicDetailExpander.IsExpanded = false;
        GeographicDetailExpander.Visibility = Visibility.Collapsed;
    }

    #endregion [Refresh Methods]

}
