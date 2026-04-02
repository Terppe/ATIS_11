using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using static ATIS.Views.Report.PdfModels;
using RelayCommand = WinUICommunity.RelayCommand;

namespace ATIS.Views.Search;
public partial class SearchViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]

    private readonly AtisDbContext _context = new();
    private static int _fishId;
    private static int _plantId;

    #endregion [Private Data Members]


    private void ClearAllCollections()
    {
        DiseasesCollection?.Clear();
        FoodsCollection?.Clear();
        MiscellaneoussesCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();

        GeographicsCollection?.Clear();
        SynonymsCollection?.Clear();
        ImagesCollection?.Clear();
        NamesCollection?.Clear();
        SpeciessesSubCollection?.Clear();
        SpeciessesCollection?.Clear();
        GenussesCollection?.Clear();
        InfratribussesCollection?.Clear();
        SubtribussesCollection?.Clear();
        TribussesCollection?.Clear();
        SupertribussesCollection?.Clear();
        InfrafamiliesCollection?.Clear();
        SubfamiliesCollection?.Clear();
        FamiliesCollection?.Clear();
        SuperfamiliesCollection?.Clear();
        InfraordosCollection?.Clear();
        SubordosCollection?.Clear();
        OrdosCollection?.Clear();
        LegiosCollection?.Clear();
        InfraclassesCollection?.Clear();
        SubclassesCollection?.Clear();
        ClassesCollection?.Clear();
        SuperclassesCollection?.Clear();
        SubphylumsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();
    }

    #region Search
    public ICommand SearchRunCommand => new WinUICommunity.RelayCommand(execute: delegate { SearchRun(FilterText); });

    public void SearchRun(string filterText)
    {
        ClearAllCollections();

        if (string.IsNullOrEmpty(filterText))
        {
            AllDialogs.NoDatasetFoundWarnMessageDialog();
            return;
        }

        SelectedTabChange(filterText);
    }

    private void SelectedTabChange(string filterText)
    {
        if (_selectedMainTabIndex == 0)  //Genus
        {
            IsLoading = true;
            GenussesCollection = dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(filterText);

            DataSetCount = GenussesCollection.Count;
            RefreshGenusItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 1)  //Species
        {
            IsLoading = true;
            SpeciessesCollection =
                dataService
                    .GetTbl75SpeciessesCollectionOrderByGenusNameAndSpeciesNameAndSubspeciesAndDiversFromFilterText(filterText);

            DataSetCount = SpeciessesCollection.Count;
            OnPropertyChanged(nameof(SpeciessesCollection));
            RefreshSpeciesItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 2 && _selectedTribusTabIndex == 0)  //Infratribus
        {
            IsLoading = true;
            InfratribussesCollection = dataService.GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(filterText);
            DataSetCount = InfratribussesCollection.Count;

            OnPropertyChanged(nameof(InfratribussesCollection));
            RefreshInfratribusItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 2 && _selectedTribusTabIndex == 1)  //Subtribus
        {
            IsLoading = true;
            SubtribussesCollection = dataService.GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(filterText);
            DataSetCount = SubtribussesCollection.Count;

            OnPropertyChanged(nameof(SubtribussesCollection));
            RefreshSubtribusItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 2 && _selectedTribusTabIndex == 2)  //Tribus
        {
            IsLoading = true;
            TribussesCollection = dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(filterText);
            DataSetCount = TribussesCollection.Count;

            OnPropertyChanged(nameof(TribussesCollection));
            RefreshTribusItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 2 && _selectedTribusTabIndex == 3)  //Supertribus
        {
            IsLoading = true;
            SupertribussesCollection = dataService.GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(filterText);
            DataSetCount = SupertribussesCollection.Count;

            OnPropertyChanged(nameof(SupertribussesCollection));
            RefreshSupertribusItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 3 && _selectedFamilyTabIndex == 0)  //Infrafamily
        {
            IsLoading = true;
            InfrafamiliesCollection = dataService.GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(filterText);
            DataSetCount = InfrafamiliesCollection.Count;

            OnPropertyChanged(nameof(InfrafamiliesCollection));
            RefreshInfrafamilyItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 3 && _selectedFamilyTabIndex == 1)  //Subfamily
        {
            IsLoading = true;
            SubfamiliesCollection = dataService.GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(filterText);
            DataSetCount = SubfamiliesCollection.Count;

            OnPropertyChanged(nameof(SubfamiliesCollection));
            RefreshSubfamilyItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 3 && _selectedFamilyTabIndex == 2)  //Family
        {
            IsLoading = true;
            FamiliesCollection = dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(filterText);
            DataSetCount = FamiliesCollection.Count;

            OnPropertyChanged(nameof(FamiliesCollection));
            RefreshFamilyItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 3 && _selectedFamilyTabIndex == 3)  //Superfamily
        {
            IsLoading = true;
            SuperfamiliesCollection = dataService.GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(filterText);
            DataSetCount = SuperfamiliesCollection.Count;

            OnPropertyChanged(nameof(SuperfamiliesCollection));
            RefreshSuperfamilyItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 4 && _selectedOrdoTabIndex == 0)  //Infraordo
        {
            IsLoading = true;
            InfraordosCollection = dataService.GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(filterText);
            DataSetCount = InfraordosCollection.Count;

            OnPropertyChanged(nameof(InfraordosCollection));
            RefreshInfraordoItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 4 && _selectedOrdoTabIndex == 1)  //Subordo
        {
            IsLoading = true;
            SubordosCollection = dataService.GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(filterText);
            DataSetCount = SubordosCollection.Count;

            OnPropertyChanged(nameof(SubordosCollection));
            RefreshSubordoItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 4 && _selectedOrdoTabIndex == 2)  //Ordo
        {
            IsLoading = true;
            OrdosCollection = dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(filterText);
            DataSetCount = OrdosCollection.Count;

            OnPropertyChanged(nameof(OrdosCollection));
            RefreshOrdoItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 4 && _selectedOrdoTabIndex == 3)  //Legio
        {
            IsLoading = true;
            LegiosCollection = dataService.GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(filterText);
            DataSetCount = LegiosCollection.Count;

            OnPropertyChanged(nameof(LegiosCollection));
            RefreshLegioItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 5 && _selectedClassTabIndex == 0)  //Infraclass
        {
            IsLoading = true;
            InfraclassesCollection = dataService.GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(filterText);
            DataSetCount = InfraclassesCollection.Count;

            OnPropertyChanged(nameof(InfraclassesCollection));
            RefreshInfraclassItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 5 && _selectedClassTabIndex == 1)  //Subclass
        {
            IsLoading = true;
            SubclassesCollection = dataService.GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(filterText);
            DataSetCount = SubclassesCollection.Count;

            OnPropertyChanged(nameof(SubclassesCollection));
            RefreshSubclassItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 5 && _selectedClassTabIndex == 2)  //Class
        {
            IsLoading = true;
            ClassesCollection = dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(filterText);
            DataSetCount = ClassesCollection.Count;

            OnPropertyChanged(nameof(ClassesCollection));
            RefreshClassItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 5 && _selectedClassTabIndex == 3)  //Superclass
        {
            IsLoading = true;
            SuperclassesCollection = dataService.GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(filterText);
            DataSetCount = SuperclassesCollection.Count;

            OnPropertyChanged(nameof(SuperclassesCollection));
            RefreshSuperclassItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 6 && _selectedPhylumTabIndex == 0)  //Subphylum
        {
            IsLoading = true;
            SubphylumsCollection = dataService.GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(filterText);
            DataSetCount = SubphylumsCollection.Count;

            OnPropertyChanged(nameof(SubphylumsCollection));
            RefreshSubphylumItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 6 && _selectedPhylumTabIndex == 1)  //Phylum
        {
            IsLoading = true;
            PhylumsCollection = dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(filterText);
            DataSetCount = PhylumsCollection.Count;

            OnPropertyChanged(nameof(PhylumsCollection));
            RefreshPhylumItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 7 && _selectedRegnumTabIndex == 0)  //Regnum
        {
            IsLoading = true;
            RegnumsCollection = dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(filterText);
            DataSetCount = RegnumsCollection.Count;

            OnPropertyChanged(nameof(RegnumsCollection));
            RefreshRegnumItems();
            IsLoading = false;
        }
        if (_selectedMainTabIndex == 8 && _selectedExtraTabIndex == 0)  //Disease
        {
            IsLoading = true;
            DiseasesCollection = dataService.GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(filterText);
            DataSetCount = DiseasesCollection.Count;

            OnPropertyChanged(nameof(DiseasesCollection));
            RefreshDiseaseItems();
            IsLoading = false;
            return;
        }

        if (_selectedMainTabIndex == 8 && _selectedExtraTabIndex == 1)  //Food
        {
            IsLoading = true;
            FoodsCollection = dataService.GetTblFoodsCollectionOrderByFoodNameFromFilterText(filterText);
            DataSetCount = FoodsCollection.Count;

            OnPropertyChanged(nameof(FoodsCollection));
            RefreshFoodItems();
            IsLoading = false;
            return;
        }
        if (_selectedMainTabIndex == 8 && _selectedExtraTabIndex == 2)  //Miscellaneous
        {
            IsLoading = true;
            MiscellaneoussesCollection = dataService.GetTblMiscellaneoussesCollectionOrderByMiscellaneousNameFromFilterText(filterText);
            DataSetCount = MiscellaneoussesCollection.Count;

            OnPropertyChanged(nameof(MiscellaneoussesCollection));
            RefreshMiscellaneousItems();
            IsLoading = false;
            return;
        }

    }

    #endregion

    #region "Public Commands to change TabItems"

    private int _selectedMainTabIndex;

    private int _selectedReportTabIndex;
    private int _selectedReportTribusTabIndex;
    private int _selectedReportFamilyTabIndex;
    private int _selectedReportOrdoTabIndex;
    private int _selectedReportClassTabIndex;
    private int _selectedReportPhylumTabIndex;
    private int _selectedReportRegnumTabIndex;
    private int _selectedReportExtraTabIndex;

    public int SelectedMainTabIndex
    {
        get => _selectedMainTabIndex;
        set
        {
            if (value == _selectedMainTabIndex)
            {
                return;
            }

            _selectedMainTabIndex = value; OnPropertyChanged();

            if (_selectedMainTabIndex == 0)
            {
                ClearAllCollections();
                Tbl63InfratribussesAllList ??= [];
                Tbl63InfratribussesAllList = dataService.GetInfratribusAsyncAlt();
                GenussesCollection = dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(FilterText);
                DataSetCount = GenussesCollection.Count;
                RefreshGenusItems();
                return;
            }

            if (_selectedMainTabIndex == 1)
            {
                ClearAllCollections();
                Tbl66GenussesAllList ??= [];
                Tbl66GenussesAllList = dataService.GetGenusAsync();
                SpeciessesCollection = dataService.GetTbl75SpeciessesCollectionOrderByGenusNameAndSpeciesNameAndSubspeciesAndDiversFromFilterText(FilterText);

                DataSetCount = SpeciessesCollection.Count;
                RefreshSpeciesItems();
                return;
            }

            if (_selectedMainTabIndex == 2)  //Tribus
            {
                ClearAllCollections();
                Tbl54SupertribussesAllList ??= [];
                Tbl54SupertribussesAllList = dataService.GetSupertribusAsync();
                TribussesCollection = dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(FilterText);
                DataSetCount = TribussesCollection.Count;

                RefreshTribusItems();
                SelectedTribusTabIndex = 2;
                return;
            }
            if (_selectedMainTabIndex == 3)  //Family
            {
                ClearAllCollections();
                Tbl42SuperfamiliesAllList ??= [];
                Tbl42SuperfamiliesAllList = dataService.GetSuperfamilyAsync();
                FamiliesCollection = dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(FilterText);

                DataSetCount = FamiliesCollection.Count;
                RefreshFamilyItems();
                SelectedFamilyTabIndex = 2;
                return;
            }
            if (_selectedMainTabIndex == 4)  //Ordo
            {
                ClearAllCollections();
                Tbl30LegiosAllList ??= [];
                Tbl30LegiosAllList = dataService.GetLegioAsync();
                OrdosCollection = dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(FilterText);
                DataSetCount = OrdosCollection.Count;

                RefreshOrdoItems();
                SelectedOrdoTabIndex = 2;
                return;
            }
            if (_selectedMainTabIndex == 5)  //Class
            {
                ClearAllCollections();
                Tbl18SuperclassesAllList ??= [];
                Tbl18SuperclassesAllList = dataService.GetSuperclassAsync();
                ClassesCollection = dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(FilterText);
                DataSetCount = ClassesCollection.Count;

                RefreshClassItems();
                SelectedClassTabIndex = 2;
                return;
            }
            if (_selectedMainTabIndex == 6)  //Phylum
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = dataService.GetRegnumAsync();

                PhylumsCollection ??= [];
                PhylumsCollection = dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
                DataSetCount = PhylumsCollection.Count;

                RefreshPhylumItems();
                SelectedPhylumTabIndex = 1;
                return;
            }
            if (_selectedMainTabIndex == 7)  //Regnum
            {
                ClearAllCollections();
                RegnumsCollection ??= [];
                RegnumsCollection = dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(FilterText);

                DataSetCount = RegnumsCollection.Count;
                RefreshRegnumItems();

                SelectedRegnumTabIndex = 0;
            }

            if (_selectedMainTabIndex == 8)  //Extra
            {
                ClearAllCollections();
                FoodsCollection = dataService.GetTblFoodsCollectionOrderByFoodNameFromFilterText(FilterText);
                DataSetCount = FoodsCollection.Count;

                OnPropertyChanged(nameof(FoodsCollection));
                RefreshFoodItems();

                SelectedExtraTabIndex = 1;
            }

        }
    }

    public int SelectedReportTribusTabIndex
    {
        get => _selectedReportTribusTabIndex;
        set
        {
            if (value == _selectedReportTribusTabIndex)
            {
                return;
            }

            _selectedReportTribusTabIndex = value; OnPropertyChanged();

            if (_selectedReportTribusTabIndex == 0)
            {
            }

            if (_selectedReportTribusTabIndex == 1)
            {
            }

            if (_selectedReportTribusTabIndex == 2)
            {
            }

            if (_selectedReportTribusTabIndex == 3)
            {
            }
        }
    }
    public int SelectedReportFamilyTabIndex
    {
        get => _selectedReportFamilyTabIndex;
        set
        {
            if (value == _selectedReportFamilyTabIndex)
            {
                return;
            }

            _selectedReportFamilyTabIndex = value; OnPropertyChanged();

            if (_selectedReportFamilyTabIndex == 0)
            {
            }

            if (_selectedReportFamilyTabIndex == 1)
            {
            }

            if (_selectedReportFamilyTabIndex == 2)
            {
            }

            if (_selectedReportFamilyTabIndex == 3)
            {
            }
        }
    }
    public int SelectedReportOrdoTabIndex
    {
        get => _selectedReportOrdoTabIndex;
        set
        {
            if (value == _selectedReportOrdoTabIndex)
            {
                return;
            }

            _selectedReportOrdoTabIndex = value; OnPropertyChanged();

            if (_selectedReportOrdoTabIndex == 0)
            {
            }

            if (_selectedReportOrdoTabIndex == 1)
            {
            }

            if (_selectedReportOrdoTabIndex == 2)
            {
            }

            if (_selectedReportOrdoTabIndex == 3)
            {
            }
        }
    }
    public int SelectedReportClassTabIndex
    {
        get => _selectedReportClassTabIndex;
        set
        {
            if (value == _selectedReportClassTabIndex)
            {
                return;
            }

            _selectedReportClassTabIndex = value; OnPropertyChanged();

            if (_selectedReportClassTabIndex == 0)
            {
            }

            if (_selectedReportClassTabIndex == 1)
            {
            }

            if (_selectedReportClassTabIndex == 2)
            {
            }

            if (_selectedReportClassTabIndex == 3)
            {
            }
        }
    }
    public int SelectedReportPhylumTabIndex
    {
        get => _selectedReportPhylumTabIndex;
        set
        {
            if (value == _selectedReportPhylumTabIndex)
            {
                return;
            }

            _selectedReportPhylumTabIndex = value; OnPropertyChanged();

            if (_selectedReportPhylumTabIndex == 0)
            {
            }

            if (_selectedReportPhylumTabIndex == 1)
            {
            }
        }
    }
    public int SelectedReportRegnumTabIndex
    {
        get => _selectedReportRegnumTabIndex;
        set
        {
            if (value == _selectedReportRegnumTabIndex)
            {
                return;
            }

            _selectedReportRegnumTabIndex = value; OnPropertyChanged();

            if (_selectedReportRegnumTabIndex == 0)
            {
            }
        }
    }

    public int SelectedReportExtraTabIndex
    {
        get => _selectedReportExtraTabIndex;
        set
        {
            if (value == _selectedReportExtraTabIndex)
            {
                return;
            }

            _selectedReportExtraTabIndex = value; OnPropertyChanged();

            if (_selectedReportPhylumTabIndex == 0)
            {
            }

            if (_selectedReportPhylumTabIndex == 1)
            {
            }
            if (_selectedReportPhylumTabIndex == 2)
            {
            }
        }
    }

    private int _selectedTribusTabIndex;
    private int _selectedFamilyTabIndex;
    private int _selectedOrdoTabIndex;
    private int _selectedClassTabIndex;
    private int _selectedPhylumTabIndex;
    private int _selectedRegnumTabIndex;
    private int _selectedExtraTabIndex;

    public int SelectedReportTabIndex
    {
        get => _selectedReportTabIndex;
        set
        {
            if (value == _selectedReportTabIndex)
            {
                return;
            }

            _selectedReportTabIndex = value; OnPropertyChanged();

            if (_selectedReportTabIndex == 0)
            {
            }

            if (_selectedReportTabIndex == 1)
            {
            }

            if (_selectedReportTabIndex == 2)
            {
            }

            if (_selectedReportTabIndex == 3)
            {
            }
        }
    }
    public int SelectedTribusTabIndex
    {
        get => _selectedTribusTabIndex;
        set
        {
            if (value == _selectedTribusTabIndex)
            {
                return;
            }

            _selectedTribusTabIndex = value; OnPropertyChanged();

            if (_selectedTribusTabIndex == 0)
            {
                ClearAllCollections();
                Tbl60SubtribussesAllList ??= [];
                Tbl60SubtribussesAllList = dataService.GetSubtribusAsync();
                InfratribussesCollection = dataService.GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(FilterText);
                DataSetCount = InfratribussesCollection.Count;

                OnPropertyChanged(nameof(InfratribussesCollection));
                RefreshInfratribusItems();
            }

            if (_selectedTribusTabIndex == 1)
            {
                ClearAllCollections();
                Tbl57TribussesAllList ??= [];
                Tbl57TribussesAllList = dataService.GetTribusAsync();
                SubtribussesCollection = dataService.GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(FilterText);
                DataSetCount = SubtribussesCollection.Count;

                OnPropertyChanged(nameof(SubtribussesCollection));
                RefreshSubtribusItems();
            }

            if (_selectedTribusTabIndex == 2)
            {
                ClearAllCollections();
                Tbl54SupertribussesAllList ??= [];
                Tbl54SupertribussesAllList = dataService.GetSupertribusAsync();
                TribussesCollection = dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(FilterText);
                DataSetCount = TribussesCollection.Count;

                OnPropertyChanged(nameof(TribussesCollection));
                RefreshTribusItems();
            }

            if (_selectedTribusTabIndex == 3)
            {
                ClearAllCollections();
                Tbl51InfrafamiliesAllList ??= [];
                Tbl51InfrafamiliesAllList = dataService.GetInfrafamilyAsync();
                SupertribussesCollection = dataService.GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(FilterText);
                DataSetCount = SupertribussesCollection.Count;

                OnPropertyChanged(nameof(SupertribussesCollection));
                RefreshSupertribusItems();
            }
        }
    }
    public int SelectedFamilyTabIndex
    {
        get => _selectedFamilyTabIndex;
        set
        {
            if (value == _selectedFamilyTabIndex)
            {
                return;
            }

            _selectedFamilyTabIndex = value; OnPropertyChanged();

            if (_selectedFamilyTabIndex == 0)
            {
                ClearAllCollections();
                Tbl48SubfamiliesAllList ??= [];
                Tbl48SubfamiliesAllList = dataService.GetSubfamilyAsync();
                InfrafamiliesCollection = dataService.GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(FilterText);
                DataSetCount = InfrafamiliesCollection.Count;

                OnPropertyChanged(nameof(InfrafamiliesCollection));
                RefreshInfrafamilyItems();
            }

            if (_selectedFamilyTabIndex == 1)
            {
                ClearAllCollections();
                Tbl45FamiliesAllList ??= [];
                Tbl45FamiliesAllList = dataService.GetFamilyAsync();
                SubfamiliesCollection = dataService.GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(FilterText);
                DataSetCount = SubfamiliesCollection.Count;

                OnPropertyChanged(nameof(SubfamiliesCollection));
                RefreshSubfamilyItems();
            }

            if (_selectedFamilyTabIndex == 2)
            {
                ClearAllCollections();
                Tbl42SuperfamiliesAllList ??= [];
                Tbl42SuperfamiliesAllList = dataService.GetSuperfamilyAsync();
                FamiliesCollection = dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(FilterText);
                DataSetCount = FamiliesCollection.Count;

                OnPropertyChanged(nameof(FamiliesCollection));
                RefreshFamilyItems();
            }

            if (_selectedFamilyTabIndex == 3)
            {
                ClearAllCollections();
                Tbl39InfraordosAllList ??= [];
                Tbl39InfraordosAllList = dataService.GetInfraordoAsync();
                SuperfamiliesCollection = dataService.GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(FilterText);
                DataSetCount = SuperfamiliesCollection.Count;

                OnPropertyChanged(nameof(SuperfamiliesCollection));
                RefreshSuperfamilyItems();
            }
        }
    }
    public int SelectedOrdoTabIndex
    {
        get => _selectedOrdoTabIndex;
        set
        {
            if (value == _selectedOrdoTabIndex)
            {
                return;
            }

            _selectedOrdoTabIndex = value; OnPropertyChanged();

            if (_selectedOrdoTabIndex == 0)
            {
                ClearAllCollections();
                Tbl36SubordosAllList ??= [];
                Tbl36SubordosAllList = dataService.GetSubordoAsync();
                InfraordosCollection = dataService.GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(FilterText);
                DataSetCount = InfraordosCollection.Count;

                OnPropertyChanged(nameof(InfraordosCollection));
                RefreshInfraordoItems();
            }

            if (_selectedOrdoTabIndex == 1)
            {
                ClearAllCollections();
                Tbl33OrdosAllList ??= [];
                Tbl33OrdosAllList = dataService.GetOrdoAsync();
                SubordosCollection = dataService.GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(FilterText);
                DataSetCount = SubordosCollection.Count;

                OnPropertyChanged(nameof(SubordosCollection));
                RefreshSubordoItems();
            }

            if (_selectedOrdoTabIndex == 2)
            {
                ClearAllCollections();
                Tbl30LegiosAllList ??= [];
                Tbl30LegiosAllList = dataService.GetLegioAsync();
                OrdosCollection = dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(FilterText);
                DataSetCount = OrdosCollection.Count;

                OnPropertyChanged(nameof(OrdosCollection));
                RefreshOrdoItems();
            }

            if (_selectedOrdoTabIndex == 3)
            {
                ClearAllCollections();
                Tbl27InfraclassesAllList ??= [];
                Tbl27InfraclassesAllList = dataService.GetInfraclassAsync();
                LegiosCollection = dataService.GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(FilterText);
                DataSetCount = LegiosCollection.Count;

                OnPropertyChanged(nameof(LegiosCollection));
                RefreshLegioItems();
            }
        }
    }
    public int SelectedClassTabIndex
    {
        get => _selectedClassTabIndex;
        set
        {
            if (value == _selectedClassTabIndex)
            {
                return;
            }

            _selectedClassTabIndex = value; OnPropertyChanged();

            if (_selectedClassTabIndex == 0)
            {
                ClearAllCollections();
                Tbl24SubclassesAllList ??= [];
                Tbl24SubclassesAllList = dataService.GetSubclassAsync();
                InfraclassesCollection = dataService.GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(FilterText);
                DataSetCount = InfraclassesCollection.Count;

                OnPropertyChanged(nameof(InfraclassesCollection));
                RefreshInfraclassItems();
            }

            if (_selectedClassTabIndex == 1)
            {
                ClearAllCollections();
                Tbl21ClassesAllList ??= [];
                Tbl21ClassesAllList = dataService.GetClassAsync();
                SubclassesCollection = dataService.GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(FilterText);
                DataSetCount = SubclassesCollection.Count;

                OnPropertyChanged(nameof(SubclassesCollection));
                RefreshSubclassItems();
            }

            if (_selectedClassTabIndex == 2)
            {
                ClearAllCollections();
                Tbl18SuperclassesAllList ??= [];
                Tbl18SuperclassesAllList = dataService.GetSuperclassAsync();
                ClassesCollection = dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(FilterText);
                DataSetCount = ClassesCollection.Count;

                OnPropertyChanged(nameof(ClassesCollection));
                RefreshClassItems();
            }

            //if (_selectedClassTabIndex == 3)
            //{
            //    ClearAllCollections();
            //    Tbl15SubdivisionsAllList ??= [];
            //    Tbl15SubdivisionsAllList = dataService.GetSubdivisionAsync();
            //    Tbl12SubphylumsAllList ??= [];
            //    Tbl12SubphylumsAllList = dataService.GetSubphylumAsync();
            //    SuperclassesCollection = dataService.GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(FilterText);
            //    DataSetCount = SuperclassesCollection.Count;

            //    OnPropertyChanged(nameof(SuperclassesCollection));
            //    RefreshSuperclassItems();
            //}
        }
    }
    public int SelectedPhylumTabIndex
    {
        get => _selectedPhylumTabIndex;
        set
        {
            if (value == _selectedPhylumTabIndex)
            {
                return;
            }

            _selectedPhylumTabIndex = value; OnPropertyChanged();

            if (_selectedPhylumTabIndex == 0)
            {
                ClearAllCollections();
                Tbl06PhylumsAllList ??= [];
                Tbl06PhylumsAllList = dataService.GetPhylumAsync();
                SubphylumsCollection = dataService.GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(FilterText);
                DataSetCount = SubphylumsCollection.Count;

                OnPropertyChanged(nameof(SubphylumsCollection));
                RefreshSubphylumItems();
            }

            if (_selectedPhylumTabIndex == 1)
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = dataService.GetRegnumAsync();
                PhylumsCollection = dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
                DataSetCount = PhylumsCollection.Count;

                OnPropertyChanged(nameof(PhylumsCollection));
                RefreshPhylumItems();
            }

        }
    }
    public int SelectedRegnumTabIndex
    {
        get => _selectedRegnumTabIndex;
        set
        {
            if (value == _selectedRegnumTabIndex)
            {
                return;
            }

            _selectedRegnumTabIndex = value; OnPropertyChanged();

            if (_selectedRegnumTabIndex == 0)
            {
                ClearAllCollections();
                RegnumsCollection = dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(FilterText);
                DataSetCount = RegnumsCollection.Count;

                OnPropertyChanged(nameof(RegnumsCollection));
                RefreshRegnumItems();
            }
        }
    }
    public int SelectedExtraTabIndex
    {
        get => _selectedExtraTabIndex;
        set
        {
            if (value == _selectedExtraTabIndex)
            {
                return;
            }

            _selectedExtraTabIndex = value; OnPropertyChanged();

            if (_selectedExtraTabIndex == 0)
            {
                ClearAllCollections();
                DiseasesCollection = dataService.GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(FilterText);
                DataSetCount = DiseasesCollection.Count;

                OnPropertyChanged(nameof(DiseasesCollection));
                RefreshDiseaseItems();
            }
            if (_selectedExtraTabIndex == 1)
            {
                ClearAllCollections();
                FoodsCollection = dataService.GetTblFoodsCollectionOrderByFoodNameFromFilterText(FilterText);
                DataSetCount = FoodsCollection.Count;

                OnPropertyChanged(nameof(FoodsCollection));
                RefreshFoodItems();
            }
            if (_selectedExtraTabIndex == 2)
            {
                ClearAllCollections();
                MiscellaneoussesCollection = dataService.GetTblMiscellaneoussesCollectionOrderByMiscellaneousNameFromFilterText(FilterText);
                DataSetCount = MiscellaneoussesCollection.Count;

                OnPropertyChanged(nameof(MiscellaneoussesCollection));
                RefreshMiscellaneousItems();
            }

        }
    }

    #endregion "Public Commands to change TabItems"

    #region Search

    public void SearchAll(int id, string tab)
    {
        ClearAllCollections();
        //if (_context.Tbl15Subdivisions != null)
        //{
        //    var plantaeRegnum = _context.Tbl15Subdivisions.FirstOrDefault(e => e.SubdivisionName == "Plantae#Regnum#");
        //    if (plantaeRegnum != null)
        //    {
        //        _plantId = plantaeRegnum.SubdivisionId;
        //    }
        //}

        if (_context.Tbl12Subphylums != null)
        {
            var animaliaRegnum = _context.Tbl12Subphylums.FirstOrDefault(e => e.SubphylumName == "Animalia#Regnum#");

            if (animaliaRegnum != null)
            {
                _fishId = animaliaRegnum.SubphylumId;
            }
        }

        switch (tab)
        {
            case "Tbl03Regnums":
                GetTbl03RegnumsById(id);
                break;
            case "Tbl06Phylums":
                GetTbl06PhylumsById(id);
                break;
            case "Tbl12Subphylums":
                GetTbl12SubphylumsById(id);
                break;
            case "Tbl18Superclasses":
                GetTbl18SuperclassesById(id);
                break;
            case "Tbl21Classes":
                GetTbl21ClassesById(id);
                break;
            case "Tbl24Subclasses":
                GetTbl24SubclassesById(id);
                break;
            case "Tbl27Infraclasses":
                GetTbl27InfraclassesById(id);
                break;
            case "Tbl30Legios":
                GetTbl30LegiosById(id);
                break;
            case "Tbl33Ordos":
                GetTbl33OrdosById(id);
                break;
            case "Tbl36Subordos":
                GetTbl36SubordosById(id);
                break;
            case "Tbl39Infraordos":
                GetTbl39InfraordosById(id);
                break;
            case "Tbl42Superfamilies":
                GetTbl42SuperfamiliesById(id);
                break;
            case "Tbl45Families":
                GetTbl45FamiliesById(id);
                break;
            case "Tbl48Subfamilies":
                GetTbl48SubfamiliesById(id);
                break;
            case "Tbl51Infrafamilies":
                GetTbl51InfrafamiliesById(id);
                break;
            case "Tbl54Supertribusses":
                GetTbl54SupertribussesById(id);
                break;
            case "Tbl57Tribusses":
                GetTbl57TribussesById(id);
                break;
            case "Tbl60Subtribusses":
                GetTbl60SubtribussesById(id);
                break;
            case "Tbl63Infratribusses":
                GetTbl63InfratribussesById(id);
                break;
            case "Tbl66Genusses":
                GetTbl66GenussesById(id);
                break;
            case "Tbl75Speciesses":
                var species = dataService.GetSpeciesSingleModelBySpeciesId(id);
                if (string.IsNullOrEmpty(species.Divers))
                {
                    GetTbl75SpeciessesById(id);
                }
                else
                {
                    GetTbl75SpeciessesSubById(id);
                }
                break;
            case "Tbl75SpeciessesSub":
                GetTbl75SpeciessesSubById(id);
                break;
            //case "TblDiseases":
            //    GetTblDiseasesById(id);
            //    break;
            //case "TblFoods":
            //    GetTblFoodsById(id);
            //    break;
            //case "TblMiscellaneousses":
            //    GetTblMiscellaneoussesById(id);
            //    break;

                
        }

    }

    #endregion Report Search

    #region AllReportSearch

    private void Refresh()
    {
        if (RegnumsCollection != null)
        {
            RefreshRegnumItems();
        }
        if (PhylumsCollection != null)
        {
            RefreshPhylumItems();
        }
        if (SubphylumsCollection != null)
        {
            RefreshSubphylumItems();
        }
        if (SuperclassesCollection != null)
        {
            RefreshSuperclassItems();
        }
        if (ClassesCollection != null)
        {
            RefreshClassItems();
        }
        if (SubclassesCollection != null)
        {
            RefreshSubclassItems();
        }
        if (InfraclassesCollection != null)
        {
            RefreshInfraclassItems();
        }
        if (LegiosCollection != null)
        {
            RefreshLegioItems();
        }
        if (OrdosCollection != null)
        {
            RefreshOrdoItems();
        }
        if (SubordosCollection != null)
        {
            RefreshSubordoItems();
        }
        if (InfraordosCollection != null)
        {
            RefreshInfraordoItems();
        }
        if (SuperfamiliesCollection != null)
        {
            RefreshSuperfamilyItems();
        }
        if (FamiliesCollection != null)
        {
            RefreshFamilyItems();
        }
        if (SubfamiliesCollection != null)
        {
            RefreshSubfamilyItems();
        }
        if (InfrafamiliesCollection != null)
        {
            RefreshInfrafamilyItems();
        }
        if (SupertribussesCollection != null)
        {
            RefreshSupertribusItems();
        }
        if (TribussesCollection != null)
        {
            RefreshTribusItems();
        }
        if (SubtribussesCollection != null)
        {
            RefreshSubtribusItems();
        }
        if (InfratribussesCollection != null)
        {
            RefreshInfratribusItems();
        }
        if (GenussesCollection != null)
        {
            RefreshGenusItems();
        }
        if (SpeciessesCollection != null)
        {
            RefreshSpeciesItems();
        }
        if (SpeciessesSubCollection != null)
        {
            RefreshSpeciesSubItems();
        }
        if (SpeciessesCollection != null)
        {
            RefreshSpeciesItems();
        }
        if (SpeciessesSubCollection != null)
        {
            RefreshSpeciesSubItems();
        }


        if (SpeciessesSpeciesNameCollection != null)
        {
            //  RefreshSpeciesSpeciesNameItems();
        }

        if (NamesCollection != null)
        {
            RefreshNameItems();
        }
        if (ImagesCollection != null)
        {
            RefreshImageItems();
        }
        if (SynonymsCollection != null)
        {
            RefreshSynonymItems();
        }
        if (GeographicsCollection != null)
        {
            RefreshGeographicItems();
        }
        if (ExpertsCollection != null)
        {
            RefreshReferenceExpertItems();
        }
        if (SourcesCollection != null)
        {
            RefreshReferenceSourceItems();
        }
        if (AuthorsCollection != null)
        {
            RefreshReferenceAuthorItems();
        }
        if (CommentsCollection != null)
        {
            RefreshCommentItems();
        }
    }

    #region Regnum
    public void GetTbl03RegnumsById(int id)
    {
        RegnumsCollection = dataService.CollRegnumsByRegnumId(id);

        //direct children
        PhylumsCollection = dataService.CollPhylumsByRegnumIdAndHash(id);
        //DivisionsCollection = dataService.CollDivisionsByRegnumIdAndHash(id);
        //------------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByRegnumId(id);
        SourcesCollection = dataService.CollSourcesByRegnumId(id);
        AuthorsCollection = dataService.CollAuthorsByRegnumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByRegnumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Phylum
    public void GetTbl06PhylumsById(int id)
    {
        PhylumsCollection = dataService.CollPhylumsByPhylumId(id);

        var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(id);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SubphylumsCollection = dataService.CollSubphylumsByPhylumIdAndHash(id);
        //------------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByPhylumId(id);
        SourcesCollection = dataService.CollSourcesByPhylumId(id);
        AuthorsCollection = dataService.CollAuthorsByPhylumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByPhylumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subphylum
    public void GetTbl12SubphylumsById(int id)
    {
        SubphylumsCollection = dataService.CollSubphylumsBySubphylumId(id);

        var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(id);
        PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
        var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SuperclassesCollection = dataService.CollSuperclassesBySubphylumIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubphylumId(id);
        SourcesCollection = dataService.CollSourcesBySubphylumId(id);
        AuthorsCollection = dataService.CollAuthorsBySubphylumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubphylumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Superclass
    public void GetTbl18SuperclassesById(int id)
    {
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassId(id);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(id);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(id);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
  //      if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        ClassesCollection = dataService.CollClassesBySuperclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySuperclassId(id);
        SourcesCollection = dataService.CollSourcesBySuperclassId(id);
        AuthorsCollection = dataService.CollAuthorsBySuperclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySuperclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Class
    public void GetTbl21ClassesById(int id)
    {
        ClassesCollection = dataService.CollClassesByClassId(id);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(id);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
      //  var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        SubclassesCollection = dataService.CollSubclassesByClassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByClassId(id);
        SourcesCollection = dataService.CollSourcesByClassId(id);
        AuthorsCollection = dataService.CollAuthorsByClassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByClassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subclass
    public void GetTbl24SubclassesById(int id)
    {
        SubclassesCollection = dataService.CollSubclassesBySubclassId(id);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(id);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
    //    var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        InfraclassesCollection = dataService.CollInfraclassesBySubclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubclassId(id);
        SourcesCollection = dataService.CollSourcesBySubclassId(id);
        AuthorsCollection = dataService.CollAuthorsBySubclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infraclass
    public void GetTbl27InfraclassesById(int id)
    {
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassId(id);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(id);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
      //  var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        LegiosCollection = dataService.CollLegiosByInfraclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByInfraclassId(id);
        SourcesCollection = dataService.CollSourcesByInfraclassId(id);
        AuthorsCollection = dataService.CollAuthorsByInfraclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByInfraclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Legio
    public void GetTbl30LegiosById(int id)
    {
        LegiosCollection = dataService.CollLegiosByLegioId(id);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(id);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
      //  var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        OrdosCollection = dataService.CollOrdosByLegioIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByLegioId(id);
        SourcesCollection = dataService.CollSourcesByLegioId(id);
        AuthorsCollection = dataService.CollAuthorsByLegioId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByLegioId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Ordo
    public void GetTbl33OrdosById(int id)
    {
        OrdosCollection = dataService.CollOrdosByOrdoId(id);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(id);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
    //    var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubordosCollection = dataService.CollSubordosByOrdoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByOrdoId(id);
        SourcesCollection = dataService.CollSourcesByOrdoId(id);
        AuthorsCollection = dataService.CollAuthorsByOrdoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByOrdoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subordo
    public void GetTbl36SubordosById(int id)
    {
        SubordosCollection = dataService.CollSubordosBySubordoId(id);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(id);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
     //   var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfraordosCollection = dataService.CollInfraordosBySubordoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubordoId(id);
        SourcesCollection = dataService.CollSourcesBySubordoId(id);
        AuthorsCollection = dataService.CollAuthorsBySubordoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubordoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infraordo
    public void GetTbl39InfraordosById(int id)
    {
        InfraordosCollection = dataService.CollInfraordosByInfraordoId(id);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(id);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
      //  var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SuperfamiliesCollection = dataService.CollSuperfamiliesByInfraordoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByInfraordoId(id);
        SourcesCollection = dataService.CollSourcesByInfraordoId(id);
        AuthorsCollection = dataService.CollAuthorsByInfraordoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByInfraordoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Superfamily
    public void GetTbl42SuperfamiliesById(int id)
    {
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyId(id);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(id);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        FamiliesCollection = dataService.CollFamiliesBySuperfamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySuperfamilyId(id);
        SourcesCollection = dataService.CollSourcesBySuperfamilyId(id);
        AuthorsCollection = dataService.CollAuthorsBySuperfamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySuperfamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Family
    public void GetTbl45FamiliesById(int id)
    {
        FamiliesCollection = dataService.CollFamiliesByFamilyId(id);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(id);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubfamiliesCollection = dataService.CollSubfamiliesByFamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByFamilyId(id);
        SourcesCollection = dataService.CollSourcesByFamilyId(id);
        AuthorsCollection = dataService.CollAuthorsByFamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByFamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subfamily
    public void GetTbl48SubfamiliesById(int id)
    {
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyId(id);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(id);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfrafamiliesCollection = dataService.CollInfrafamiliesBySubfamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubfamilyId(id);
        SourcesCollection = dataService.CollSourcesBySubfamilyId(id);
        AuthorsCollection = dataService.CollAuthorsBySubfamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubfamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infrafamily
    public void GetTbl51InfrafamiliesById(int id)
    {
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyId(id);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(id);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SupertribussesCollection = dataService.CollSupertribussesByInfrafamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByInfrafamilyId(id);
        SourcesCollection = dataService.CollSourcesByInfrafamilyId(id);
        AuthorsCollection = dataService.CollAuthorsByInfrafamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByInfrafamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Supertribus
    public void GetTbl54SupertribussesById(int id)
    {
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusId(id);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(id);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        TribussesCollection = dataService.CollTribussesBySupertribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySupertribusId(id);
        SourcesCollection = dataService.CollSourcesBySupertribusId(id);
        AuthorsCollection = dataService.CollAuthorsBySupertribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySupertribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Tribus
    public void GetTbl57TribussesById(int id)
    {
        TribussesCollection = dataService.CollTribussesByTribusId(id);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(id);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubtribussesCollection = dataService.CollSubtribussesByTribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByTribusId(id);
        SourcesCollection = dataService.CollSourcesByTribusId(id);
        AuthorsCollection = dataService.CollAuthorsByTribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByTribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subtribus
    public void GetTbl60SubtribussesById(int id)
    {
        SubtribussesCollection = dataService.CollSubtribussesBySubtribusId(id);
        var tribusId = dataService.TribusIdFromSubtribussesCollectionSelect(id);
        TribussesCollection = dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfratribussesCollection = dataService.CollInfratribussesBySubtribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubtribusId(id);
        SourcesCollection = dataService.CollSourcesBySubtribusId(id);
        AuthorsCollection = dataService.CollAuthorsBySubtribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubtribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infratribus
    public void GetTbl63InfratribussesById(int id)
    {
        InfratribussesCollection = dataService.CollInfratribussesByInfratribusId(id);
        var subtribusId = dataService.SubtribusIdFromInfratribussesCollectionSelect(id);
        SubtribussesCollection = dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        GenussesCollection = dataService.CollGenussesByInfratribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByInfratribusId(id);
        SourcesCollection = dataService.CollSourcesByInfratribusId(id);
        AuthorsCollection = dataService.CollAuthorsByInfratribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByInfratribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Genus
    public void GetTbl66GenussesById(int id)
    {
        GenussesCollection = dataService.CollGenussesByGenusId(id);
        var infratribusId = dataService.InfratribusIdFromGenussesCollectionSelect(id);
        InfratribussesCollection = dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }


        //direct children
        //      FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndSubspeciesIsNullAndHash(id);
        SpeciessesCollection = dataService.CollSpeciessesByGenusIdAndHash(id);
        //FiSpeciessesCollection = new ObservableCollection<Tbl69FiSpecies>(_context.Tbl69FiSpeciesses
        //    .Include(a => a.Tbl66Genusses)
        //    .Where(e => e.GenusId == id &&
        //                e.Subspecies == null &&
        //                e.FiSpeciesName.Contains("#") == false));

        //  PlSpeciessesCollection = _dataService.CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(id);
       // SpeciessesCollection = dataService.CollSpeciessesByGenusIdAndHash(id);


        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByGenusId(id);
        SourcesCollection = dataService.CollSourcesByGenusId(id);
        AuthorsCollection = dataService.CollAuthorsByGenusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByGenusId(id);

        //----------Refresh-------------
        Refresh();
    }


    #endregion

    #region Speciesgroup

    //private void GetTbl68SpeciesgroupsById(int id)
    //{

    //}

    #endregion

    #region Species
    public void GetTbl75SpeciessesById(int id)
    {
        var fiSpecies = dataService.GetSpeciesSingleModelBySpeciesId(id);
        SpeciessesCollection = dataService.CollSpeciessesBySpeciesId(id);

        NamesCollection = dataService.CollNamesBySpeciesIdAndHash(id);
        SynonymsCollection = dataService.CollSynonymsBySpeciesIdAndHash(id);
        ImagesCollection = dataService.CollImagesBySpeciesId(id);
        GeographicsCollection = dataService.CollGeographicsBySpeciesId(id);

        var genusId = dataService.GenusIdFromSpeciessesCollectionSelect(id);
        GenussesCollection = dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SpeciessesSubCollection = dataService.CollSpeciessesBySpeciesNameAndNotEmptySubspecies(fiSpecies.GenusId, fiSpecies.SpeciesName!);
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySpeciesId(id);
        SourcesCollection = dataService.CollSourcesBySpeciesId(id);
        AuthorsCollection = dataService.CollAuthorsBySpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region SpeciesSub
    public void GetTbl75SpeciessesSubById(int id)
    {
        ////  var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        //FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);
        ////  FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);
        //FiSpeciessesSubCollection = FiSpeciessesCollection;

        //  var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        SpeciessesSubCollection = dataService.CollSpeciessesBySpeciesId(id);

        var fiSpeciessingle = dataService.GetSpeciesSingleBySpeciesId(id);
        var fiSpeciesId = fiSpeciessingle.SpeciesId;
        SpeciessesCollection = dataService.CollSpeciessesBySpeciesId(fiSpeciesId);


        // FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndFiSpeciesNameAndEmptySubspecies(fiSpecies.GenusId, fiSpecies.FiSpeciesName);
        NamesCollection = dataService.CollNamesBySpeciesIdAndHash(id);
        SynonymsCollection = dataService.CollSynonymsBySpeciesIdAndHash(id);
        ImagesCollection = dataService.CollImagesBySpeciesId(id);
        GeographicsCollection = dataService.CollGeographicsBySpeciesId(id);

        var genusId = dataService.GenusIdFromSpeciessesCollectionSelect(id);
        GenussesCollection = dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        //if (subphylumId == _fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        //}
        //if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySpeciesId(id);
        SourcesCollection = dataService.CollSourcesBySpeciesId(id);
        AuthorsCollection = dataService.CollAuthorsBySpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #endregion


    #region Selected Properties

    private Tbl03Regnum _regnumSelected = null!;
    public Tbl03Regnum RegnumSelected
    {
        get => _regnumSelected;
        set => SetProperty(ref _regnumSelected, value);
    }

    private Tbl06Phylum _phylumSelected = null!;
    public Tbl06Phylum PhylumSelected
    {
        get => _phylumSelected;
        set => SetProperty(ref _phylumSelected, value);
    }

    private Tbl12Subphylum _subphylumSelected = null!;
    public Tbl12Subphylum SubphylumSelected
    {
        get => _subphylumSelected;
        set => SetProperty(ref _subphylumSelected, value);
    }

    private Tbl18Superclass _superclassSelected = null!;
    public Tbl18Superclass SuperclassSelected
    {
        get => _superclassSelected;
        set => SetProperty(ref _superclassSelected, value);
    }

    private Tbl21Class _classSelected = null!;
    public Tbl21Class ClassSelected
    {
        get => _classSelected;
        set => SetProperty(ref _classSelected, value);
    }

    private Tbl24Subclass _subclassSelected = null!;
    public Tbl24Subclass SubclassSelected
    {
        get => _subclassSelected;
        set => SetProperty(ref _subclassSelected, value);
    }

    private Tbl27Infraclass _infraclassSelected = null!;
    public Tbl27Infraclass InfraclassSelected
    {
        get => _infraclassSelected;
        set => SetProperty(ref _infraclassSelected, value);
    }

    private Tbl30Legio _legioSelected = null!;
    public Tbl30Legio LegioSelected
    {
        get => _legioSelected;
        set => SetProperty(ref _legioSelected, value);
    }

    private Tbl33Ordo _ordoSelected = null!;
    public Tbl33Ordo OrdoSelected
    {
        get => _ordoSelected;
        set => SetProperty(ref _ordoSelected, value);
    }

    private Tbl36Subordo _subordoSelected = null!;
    public Tbl36Subordo SubordoSelected
    {
        get => _subordoSelected;
        set => SetProperty(ref _subordoSelected, value);
    }

    private Tbl39Infraordo _infraordoSelected = null!;
    public Tbl39Infraordo InfraordoSelected
    {
        get => _infraordoSelected;
        set => SetProperty(ref _infraordoSelected, value);
    }

    private Tbl42Superfamily _superfamilySelected = null!;
    public Tbl42Superfamily SuperfamilySelected
    {
        get => _superfamilySelected;
        set => SetProperty(ref _superfamilySelected, value);
    }

    private Tbl45Family _familySelected = null!;
    public Tbl45Family FamilySelected
    {
        get => _familySelected;
        set => SetProperty(ref _familySelected, value);
    }

    private Tbl48Subfamily _subfamilySelected = null!;
    public Tbl48Subfamily SubfamilySelected
    {
        get => _subfamilySelected;
        set => SetProperty(ref _subfamilySelected, value);
    }

    private Tbl51Infrafamily _infrafamilySelected = null!;
    public Tbl51Infrafamily InfrafamilySelected
    {
        get => _infrafamilySelected;
        set => SetProperty(ref _infrafamilySelected, value);
    }

    private Tbl54Supertribus _supertribusSelected = null!;
    public Tbl54Supertribus SupertribusSelected
    {
        get => _supertribusSelected;
        set => SetProperty(ref _supertribusSelected, value);
    }

    private Tbl57Tribus _tribusSelected = null!;
    public Tbl57Tribus TribusSelected
    {
        get => _tribusSelected;
        set => SetProperty(ref _tribusSelected, value);
    }

    private Tbl60Subtribus _subtribusSelected = null!;
    public Tbl60Subtribus SubtribusSelected
    {
        get => _subtribusSelected;
        set => SetProperty(ref _subtribusSelected, value);
    }

    private Tbl63Infratribus _infratribusSelected = null!;
    public Tbl63Infratribus InfratribusSelected
    {
        get => _infratribusSelected;
        set => SetProperty(ref _infratribusSelected, value);
    }

    private Tbl66Genus _genusSelected = null!;
    public Tbl66Genus GenusSelected
    {
        get => _genusSelected;
        set => SetProperty(ref _genusSelected, value);
    }

    private Tbl68Speciesgroup _speciesgroupSelected = null!;
    public Tbl68Speciesgroup SpeciesgroupSelected
    {
        get => _speciesgroupSelected;
        set => SetProperty(ref _speciesgroupSelected, value);
    }

    private Tbl75Species _speciesSelected = null!;
    public Tbl75Species SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
    }

    private Tbl75Species _speciesSubSelected = null!;
    public Tbl75Species SpeciesSubSelected
    {
        get => _speciesSubSelected;
        set => SetProperty(ref _speciesSubSelected, value);
    }

    private Tbl78Name _nameSelected = null!;
    public Tbl78Name NameSelected
    {
        get => _nameSelected;
        set => SetProperty(ref _nameSelected, value);
    }

    private Tbl81Image _imageSelected = null!;
    public Tbl81Image ImageSelected
    {
        get => _imageSelected;
        set => SetProperty(ref _imageSelected, value);
    }

    private Tbl84Synonym _synonymSelected = null!;
    public Tbl84Synonym SynonymSelected
    {
        get => _synonymSelected;
        set => SetProperty(ref _synonymSelected, value);
    }

    private Tbl87Geographic _geographicSelected = null!;
    public Tbl87Geographic GeographicSelected
    {
        get => _geographicSelected;
        set => SetProperty(ref _geographicSelected, value);
    }

    private Tbl90Reference _referenceSelected = null!;
    public Tbl90Reference ReferenceSelected
    {
        get => _referenceSelected;
        set => SetProperty(ref _referenceSelected, value);
    }

    private Tbl90Reference _referenceExpertSelected = null!;
    public Tbl90Reference ReferenceExpertSelected
    {
        get => _referenceExpertSelected;
        set => SetProperty(ref _referenceExpertSelected, value);
    }

    private Tbl90Reference _referenceSourceSelected = null!;
    public Tbl90Reference ReferenceSourceSelected
    {
        get => _referenceSourceSelected;
        set => SetProperty(ref _referenceSourceSelected, value);
    }
    private Tbl90Reference _referenceAuthorSelected = null!;
    public Tbl90Reference ReferenceAuthorSelected
    {
        get => _referenceAuthorSelected;
        set => SetProperty(ref _referenceAuthorSelected, value);
    }

    private Tbl93Comment _commentSelected = null!;
    public Tbl93Comment CommentSelected
    {
        get => _commentSelected;
        set => SetProperty(ref _commentSelected, value);
    }
    private TblDisease _diseaseSelected = null!;
    public TblDisease DiseaseSelected
    {
        get => _diseaseSelected;
        set => SetProperty(ref _diseaseSelected, value);
    }

    private TblFood _foodSelected = null!;
    public TblFood FoodSelected
    {
        get => _foodSelected;
        set => SetProperty(ref _foodSelected, value);
    }
    private TblMiscellaneous _miscellaneousSelected = null!;
    public TblMiscellaneous MiscellaneousSelected
    {
        get => _miscellaneousSelected;
        set => SetProperty(ref _miscellaneousSelected, value);
    }
    
    #endregion

    #region Refresh Methods

    private void RefreshRegnumItems()
    {
        RegnumItems.Clear();
        foreach (var item in RegnumsCollection)
        {
            RegnumItems.Add(item);
        }

        if (RegnumSelected == null && RegnumsCollection.Count != 0)
        {
            RegnumSelected = RegnumItems.FirstOrDefault();
        }
    }
    private void RefreshPhylumItems()
    {
        PhylumItems.Clear();
        foreach (var item in PhylumsCollection)
        {
            PhylumItems.Add(item);
        }
        if (PhylumSelected == null && PhylumsCollection.Count != 0)
        {
            PhylumSelected = PhylumItems.FirstOrDefault();
        }
    }
    private void RefreshSubphylumItems()
    {
        SubphylumItems.Clear();
        foreach (var item in SubphylumsCollection)
        {
            SubphylumItems.Add(item);
        }
        if (SubphylumSelected == null && SubphylumsCollection.Count != 0)
        {
            SubphylumSelected = SubphylumItems.First();
        }

    }
    private void RefreshSuperclassItems()
    {
        SuperclassItems.Clear();
        foreach (var item in SuperclassesCollection)
        {
            SuperclassItems.Add(item);
        }
        if (SuperclassSelected == null && SuperclassesCollection.Count != 0)
        {
            SuperclassSelected = SuperclassItems.First();
        }
    }
    private void RefreshClassItems()
    {
        ClassItems.Clear();
        foreach (var item in ClassesCollection)
        {
            ClassItems.Add(item);
        }
        if (ClassSelected == null && ClassesCollection.Count != 0)
        {
            ClassSelected = ClassItems.First();
        }
    }
    private void RefreshSubclassItems()
    {
        SubclassItems.Clear();
        foreach (var item in SubclassesCollection)
        {
            SubclassItems.Add(item);
        }
        if (SubclassSelected == null && SubclassesCollection.Count != 0)
        {
            SubclassSelected = SubclassItems.First();
        }
    }
    private void RefreshInfraclassItems()
    {
        InfraclassItems.Clear();
        foreach (var item in InfraclassesCollection)
        {
            InfraclassItems.Add(item);
        }
        if (InfraclassSelected == null && InfraclassesCollection.Count != 0)
        {
            InfraclassSelected = InfraclassItems.First();
        }
    }
    private void RefreshLegioItems()
    {
        LegioItems.Clear();
        foreach (var item in LegiosCollection)
        {
            LegioItems.Add(item);
        }
        if (LegioSelected == null && LegiosCollection.Count != 0)
        {
            LegioSelected = LegioItems.First();
        }
    }
    private void RefreshOrdoItems()
    {
        OrdoItems.Clear();
        foreach (var item in OrdosCollection)
        {
            OrdoItems.Add(item);
        }
        if (OrdoSelected == null && OrdosCollection.Count != 0)
        {
            OrdoSelected = OrdoItems.First();
        }
    }
    private void RefreshSubordoItems()
    {
        SubordoItems.Clear();
        foreach (var item in SubordosCollection)
        {
            SubordoItems.Add(item);
        }
        if (SubordoSelected == null && SubordosCollection.Count != 0)
        {
            SubordoSelected = SubordoItems.First();
        }
    }
    private void RefreshInfraordoItems()
    {
        InfraordoItems.Clear();
        foreach (var item in InfraordosCollection)
        {
            InfraordoItems.Add(item);
        }
        if (InfraordoSelected == null && InfraordosCollection.Count != 0)
        {
            InfraordoSelected = InfraordoItems.First();
        }
    }
    private void RefreshSuperfamilyItems()
    {
        SuperfamilyItems.Clear();
        foreach (var item in SuperfamiliesCollection)
        {
            SuperfamilyItems.Add(item);
        }
        if (SuperfamilySelected == null && SuperfamiliesCollection.Count != 0)
        {
            SuperfamilySelected = SuperfamilyItems.First();
        }
    }
    private void RefreshFamilyItems()
    {
        FamilyItems.Clear();
        foreach (var item in FamiliesCollection)
        {
            FamilyItems.Add(item);
        }
        if (FamilySelected == null && FamiliesCollection.Count != 0)
        {
            FamilySelected = FamilyItems.First();
        }
    }
    private void RefreshSubfamilyItems()
    {
        SubfamilyItems.Clear();
        foreach (var item in SubfamiliesCollection)
        {
            SubfamilyItems.Add(item);
        }
        if (SubfamilySelected == null && SubfamiliesCollection.Count != 0)
        {
            SubfamilySelected = SubfamilyItems.First();
        }
    }
    private void RefreshInfrafamilyItems()
    {
        InfrafamilyItems.Clear();
        foreach (var item in InfrafamiliesCollection)
        {
            InfrafamilyItems.Add(item);
        }
        if (InfrafamilySelected == null && InfrafamiliesCollection.Count != 0)
        {
            InfrafamilySelected = InfrafamilyItems.First();
        }
    }
    private void RefreshSupertribusItems()
    {
        SupertribusItems.Clear();
        foreach (var item in SupertribussesCollection)
        {
            SupertribusItems.Add(item);
        }
        if (SupertribusSelected == null && SupertribussesCollection.Count != 0)
        {
            SupertribusSelected = SupertribusItems.First();
        }
    }
    private void RefreshTribusItems()
    {
        TribusItems.Clear();
        foreach (var item in TribussesCollection)
        {
            TribusItems.Add(item);
        }
        if (TribusSelected == null && TribussesCollection.Count != 0)
        {
            TribusSelected = TribusItems.First();
        }
    }
    private void RefreshSubtribusItems()
    {
        SubtribusItems.Clear();
        foreach (var item in SubtribussesCollection)
        {
            SubtribusItems.Add(item);
        }
        if (SubtribusSelected == null && SubtribussesCollection.Count != 0)
        {
            SubtribusSelected = SubtribusItems.First();
        }
    }
    private void RefreshInfratribusItems()
    {
        InfratribusItems.Clear();
        foreach (var item in InfratribussesCollection)
        {
            InfratribusItems.Add(item);
        }
        if (InfratribusSelected == null && InfratribussesCollection.Count != 0)
        {
            InfratribusSelected = InfratribusItems.First();
        }
    }
    private void RefreshGenusItems()
    {
        GenusItems.Clear();
        foreach (var item in GenussesCollection)
        {
            GenusItems.Add(item);
        }
        if (GenusSelected == null && GenussesCollection.Count != 0)
        {
            GenusSelected = GenusItems.First();
        }
    }
    private void RefreshSpeciesItems()
    {
        SpeciesItems.Clear();
        foreach (var item in SpeciessesCollection)
        {
            SpeciesItems.Add(item);
        }
        if (SpeciesSelected == null && SpeciessesCollection.Count != 0)
        {
            SpeciesSelected = SpeciesItems.First();
        }
    }
    private void RefreshSpeciesSubItems()
    {
        SpeciesSubItems.Clear();
        foreach (var item in SpeciessesSubCollection)
        {
            SpeciesSubItems.Add(item);
        }
        if (SpeciesSubSelected == null && SpeciessesSubCollection.Count != 0)
        {
            SpeciesSubSelected = SpeciesSubItems.First();
        }

    }
    private void RefreshNameItems()
    {
        NameItems.Clear();
        foreach (var item in NamesCollection)
        {
            NameItems.Add(item);
        }
        if (NameSelected == null && NamesCollection.Count != 0)
        {
            NameSelected = NameItems.First();
        }
    }
    private void RefreshImageItems()
    {
        ImageItems.Clear();
        foreach (var item in ImagesCollection)
        {
            ImageItems.Add(item);
        }
        if (ImageSelected == null && ImagesCollection.Count != 0)
        {
            ImageSelected = ImageItems.First();
        }
    }
    private void RefreshSynonymItems()
    {
        SynonymItems.Clear();
        foreach (var item in SynonymsCollection)
        {
            SynonymItems.Add(item);
        }
        if (SynonymSelected == null && SynonymsCollection.Count != 0)
        {
            SynonymSelected = SynonymItems.First();
        }
    }
    private void RefreshGeographicItems()
    {
        GeographicItems.Clear();
        foreach (var item in GeographicsCollection)
        {
            GeographicItems.Add(item);
        }
        if (GeographicSelected == null && GeographicsCollection.Count != 0)
        {
            GeographicSelected = GeographicItems.First();
        }
    }
    private void RefreshReferenceExpertItems()
    {
        ReferenceExpertItems.Clear();
        foreach (var item in ExpertsCollection)
        {
            ReferenceExpertItems.Add(item);
        }
        if (ReferenceExpertSelected == null && ExpertsCollection.Count != 0)
        {
            ReferenceExpertSelected = ReferenceExpertItems.First();
        }
    }
    private void RefreshReferenceSourceItems()
    {
        ReferenceSourceItems.Clear();
        foreach (var item in SourcesCollection)
        {
            ReferenceSourceItems.Add(item);
        }
        if (ReferenceSourceSelected == null && SourcesCollection.Count != 0)
        {
            ReferenceSourceSelected = ReferenceSourceItems.First();
        }

    }
    private void RefreshReferenceAuthorItems()
    {
        ReferenceAuthorItems.Clear();
        foreach (var item in AuthorsCollection)
        {
            ReferenceAuthorItems.Add(item);
        }
        if (ReferenceAuthorSelected == null && AuthorsCollection.Count != 0)
        {
            ReferenceAuthorSelected = ReferenceAuthorItems.First();
        }
    }
    private void RefreshCommentItems()
    {
        CommentItems.Clear();
        foreach (var item in CommentsCollection)
        {
            CommentItems.Add(item);
        }
        if (CommentSelected == null && CommentsCollection.Count != 0)
        {
            CommentSelected = CommentItems.First();
        }

    }
    private void RefreshDiseaseItems()
    {
        DiseaseItems.Clear();
        foreach (var item in DiseasesCollection)
        {
            DiseaseItems.Add(item);
        }
        if (DiseaseSelected == null && DiseasesCollection.Count != 0)
        {
            DiseaseSelected = DiseaseItems.First();
        }
    }
    private void RefreshFoodItems()
    {
        FoodItems.Clear();
        foreach (var item in FoodsCollection)
        {
            FoodItems.Add(item);
        }
        if (FoodSelected == null && FoodsCollection.Count != 0)
        {
            FoodSelected = FoodItems.First();
        }
    }
    private void RefreshMiscellaneousItems()
    {
        MiscellaneousItems.Clear();
        foreach (var item in MiscellaneoussesCollection)
        {
            MiscellaneousItems.Add(item);
        }
        if (MiscellaneousSelected == null && MiscellaneoussesCollection.Count != 0)
        {
            MiscellaneousSelected = MiscellaneousItems.First();
        }
    }

    #endregion

    #region Items

    public ObservableCollection<Tbl03Regnum> RegnumItems { get; } = [];
    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = [];
    public ObservableCollection<Tbl12Subphylum> SubphylumItems { get; } = [];
    public ObservableCollection<Tbl18Superclass> SuperclassItems { get; } = [];
    public ObservableCollection<Tbl21Class> ClassItems { get; } = [];
    public ObservableCollection<Tbl24Subclass> SubclassItems { get; } = [];
    public ObservableCollection<Tbl27Infraclass> InfraclassItems { get; } = [];
    public ObservableCollection<Tbl30Legio> LegioItems { get; } = [];
    public ObservableCollection<Tbl33Ordo> OrdoItems { get; } = [];
    public ObservableCollection<Tbl36Subordo> SubordoItems { get; } = [];
    public ObservableCollection<Tbl39Infraordo> InfraordoItems { get; } = [];
    public ObservableCollection<Tbl42Superfamily> SuperfamilyItems { get; } = [];
    public ObservableCollection<Tbl45Family> FamilyItems { get; } = [];
    public ObservableCollection<Tbl48Subfamily> SubfamilyItems { get; } = [];
    public ObservableCollection<Tbl51Infrafamily> InfrafamilyItems { get; } = [];
    public ObservableCollection<Tbl54Supertribus> SupertribusItems { get; } = [];
    public ObservableCollection<Tbl57Tribus> TribusItems { get; } = [];
    public ObservableCollection<Tbl60Subtribus> SubtribusItems { get; } = [];
    public ObservableCollection<Tbl63Infratribus> InfratribusItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesSubItems { get; } = [];
    public ObservableCollection<Tbl78Name> NameItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
    public ObservableCollection<Tbl84Synonym> SynonymItems { get; } = [];
    public ObservableCollection<Tbl87Geographic> GeographicItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
    public ObservableCollection<TblFood> FoodItems { get; } = [];
    public ObservableCollection<TblMiscellaneous> MiscellaneousItems { get; } = [];
    #endregion Items

    #region "Collections"

    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsAllList;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get => _tbl03RegnumsAllList;
        set
        {
            _tbl03RegnumsAllList = value; OnPropertyChanged();
        }
    }

    private int _dataSetCount;
    public int DataSetCount
    {
        get => _dataSetCount;
        set
        {
            _dataSetCount = value; OnPropertyChanged();
        }
    }
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string FilterText { get; set; } = null!;

    public ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; }

    public ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;

    public ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = null!;

    public ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = null!;

    public ObservableCollection<Tbl27Infraclass> InfraclassesCollection { get; set; } = null!;

    public ObservableCollection<Tbl30Legio> LegiosCollection { get; set; } = null!;

    public ObservableCollection<Tbl33Ordo> OrdosCollection { get; set; } = null!;

    public ObservableCollection<Tbl36Subordo> SubordosCollection { get; set; } = null!;

    public ObservableCollection<Tbl39Infraordo> InfraordosCollection { get; set; } = null!;

    public ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection { get; set; } = null!;

    public ObservableCollection<Tbl45Family> FamiliesCollection { get; set; } = null!;

    public ObservableCollection<Tbl48Subfamily> SubfamiliesCollection { get; set; } = null!;

    public ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection { get; set; } = null!;

    public ObservableCollection<Tbl54Supertribus> SupertribussesCollection { get; set; } = null!;

    public ObservableCollection<Tbl57Tribus> TribussesCollection { get; set; } = null!;

    public ObservableCollection<Tbl60Subtribus> SubtribussesCollection { get; set; } = null!;

    public ObservableCollection<Tbl63Infratribus> InfratribussesCollection { get; set; } = null!;

    public ObservableCollection<Tbl66Genus> GenussesCollection { get; set; } = null!;

    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupsCollection { get; set; } = null!;

    public ObservableCollection<Tbl75Species> SpeciessesCollection { get; set; } = null!;

    public ObservableCollection<Tbl75Species> SpeciessesSubCollection { get; set; } = null!;

    public ObservableCollection<Tbl75Species> SpeciessesSpeciesNameCollection { get; set; } = null!;

    public ObservableCollection<Tbl78Name> NamesCollection { get; set; } = null!;

    public ObservableCollection<Tbl81Image> ImagesCollection { get; set; } = null!;

    public ObservableCollection<Tbl84Synonym> SynonymsCollection { get; set; } = null!;

    public ObservableCollection<Tbl87Geographic> GeographicsCollection { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefAuthorsList { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefSourcesList { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefExpertsList { get; set; } = null!;

    public ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    public ObservableCollection<TblDisease> DiseasesCollection { get; set; } = null!;
    public ObservableCollection<TblFood> FoodsCollection { get; set; } = null!;
    public ObservableCollection<TblMiscellaneous> MiscellaneoussesCollection { get; set; } = null!;

    #endregion "Collections"

    #region Public Properties  

    //private Data _dataSet;

    //public Data DataSet
    //{
    //    get => _dataSet;
    //    set
    //    {
    //        _dataSet = value; OnPropertyChanged(nameof(DataSet));
    //    }
    //}

    private string _searchSpeciesName = "";

    public string SearchSpeciesName
    {
        get => _searchSpeciesName;
        set
        {
            _searchSpeciesName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciessesList = null!;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesList
    {
        get => _tbl75SpeciessesList;
        set
        {
            _tbl75SpeciessesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList = null!;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value;
            OnPropertyChanged();
        }
    }



    private ObservableCollection<Tbl66Genus> _tbl66GenussesList = null!;

    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get => _tbl66GenussesList;
        set
        {
            _tbl66GenussesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList = null!;

    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl63Infratribus> _tbl63InfratribussesAllList = null!;
    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesAllList
    {
        get => _tbl63InfratribussesAllList;
        set
        {
            _tbl63InfratribussesAllList = value; OnPropertyChanged(nameof(Tbl63InfratribussesAllList));
        }
    }

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsList = null!;

    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsList
    {
        get => _tbl68SpeciesgroupsList;
        set
        {
            _tbl68SpeciesgroupsList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsAllList = null!;

    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsAllList
    {
        get => _tbl68SpeciesgroupsAllList;
        set
        {
            _tbl68SpeciesgroupsAllList = value;
            OnPropertyChanged();
        }
    }



    private ObservableCollection<Tbl78Name> _tbl78NamesList = null!;

    public ObservableCollection<Tbl78Name> Tbl78NamesList
    {
        get => _tbl78NamesList;
        set
        {
            _tbl78NamesList = value;
            OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl81Image> _tbl81ImagesList = null!;

    public ObservableCollection<Tbl81Image> Tbl81ImagesList
    {
        get => _tbl81ImagesList;
        set
        {
            _tbl81ImagesList = value;
            OnPropertyChanged();
        }
    }


    private string _searchSynonymName = string.Empty;

    public string SearchSynonymName
    {
        get => _searchSynonymName;
        set
        {
            _searchSynonymName = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl84Synonym> _tbl84SynonymsList = null!;

    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsList
    {
        get => _tbl84SynonymsList;
        set
        {
            _tbl84SynonymsList = value; OnPropertyChanged();
        }
    }



    private ObservableCollection<Tbl87Geographic> _tbl87GeographicsList = null!;

    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsList
    {
        get => _tbl87GeographicsList;
        set
        {
            _tbl87GeographicsList = value;
            OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl60Subtribus> _tbl60SubtribussesAllList;
    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesAllList
    {
        get => _tbl60SubtribussesAllList;
        set
        {
            _tbl60SubtribussesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl57Tribus> _tbl57TribussesAllList;
    public ObservableCollection<Tbl57Tribus> Tbl57TribussesAllList
    {
        get => _tbl57TribussesAllList;
        set
        {
            _tbl57TribussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl54Supertribus> _tbl54SupertribussesAllList;
    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesAllList
    {
        get => _tbl54SupertribussesAllList;
        set
        {
            _tbl54SupertribussesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl51Infrafamily> _tbl51InfrafamiliesAllList;
    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesAllList
    {
        get => _tbl51InfrafamiliesAllList;
        set
        {
            _tbl51InfrafamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl45Family> _tbl45FamiliesAllList;
    public ObservableCollection<Tbl45Family> Tbl45FamiliesAllList
    {
        get => _tbl45FamiliesAllList;
        set
        {
            _tbl45FamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl48Subfamily> _tbl48SubfamiliesList = null!;
    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesList
    {
        get => _tbl48SubfamiliesList;
        set
        {
            _tbl48SubfamiliesList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl48Subfamily> _tbl48SubfamiliesAllList = null!;
    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesAllList
    {
        get => _tbl48SubfamiliesAllList;
        set
        {
            _tbl48SubfamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl51Infrafamily> _tbl51InfrafamiliesList = null!;
    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesList
    {
        get => _tbl51InfrafamiliesList;
        set
        {
            _tbl51InfrafamiliesList = value; OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl42Superfamily> _tbl42SuperfamiliesAllList;
    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesAllList
    {
        get => _tbl42SuperfamiliesAllList;
        set
        {
            _tbl42SuperfamiliesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl39Infraordo> _tbl39InfraordosAllList;
    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosAllList
    {
        get => _tbl39InfraordosAllList;
        set
        {
            _tbl39InfraordosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl36Subordo> _tbl36SubordosAllList;
    public ObservableCollection<Tbl36Subordo> Tbl36SubordosAllList
    {
        get => _tbl36SubordosAllList;
        set
        {
            _tbl36SubordosAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl33Ordo> _tbl33OrdosAllList;
    public ObservableCollection<Tbl33Ordo> Tbl33OrdosAllList
    {
        get => _tbl33OrdosAllList;
        set
        {
            _tbl33OrdosAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl30Legio> _tbl30LegiosAllList = null!;
    public ObservableCollection<Tbl30Legio> Tbl30LegiosAllList
    {
        get => _tbl30LegiosAllList;
        set
        {
            _tbl30LegiosAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl27Infraclass> _tbl27InfraclassesAllList;
    public ObservableCollection<Tbl27Infraclass> Tbl27InfraclassesAllList
    {
        get => _tbl27InfraclassesAllList;
        set
        {
            _tbl27InfraclassesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl24Subclass> _tbl24SubclassesAllList;
    public ObservableCollection<Tbl24Subclass> Tbl24SubclassesAllList
    {
        get => _tbl24SubclassesAllList;
        set
        {
            _tbl24SubclassesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl21Class> _tbl21ClassesAllList;
    public ObservableCollection<Tbl21Class> Tbl21ClassesAllList
    {
        get => _tbl21ClassesAllList;
        set
        {
            _tbl21ClassesAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl18Superclass> _tbl18SuperclassesAllList;

    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesAllList
    {
        get => _tbl18SuperclassesAllList;
        set
        {
            _tbl18SuperclassesAllList = value;
            OnPropertyChanged();
        }
    }
    //private ObservableCollection<Tbl15Subdivision> _tbl15SubdivisionsAllList = null!;
    //public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsAllList
    //{
    //    get => _tbl15SubdivisionsAllList;
    //    set
    //    {
    //        _tbl15SubdivisionsAllList = value; OnPropertyChanged();
    //    }
    //}

    private ObservableCollection<Tbl12Subphylum> _tbl12SubphylumsAllList;
    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsAllList
    {
        get => _tbl12SubphylumsAllList;
        set
        {
            _tbl12SubphylumsAllList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsAllList;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsAllList
    {
        get => _tbl06PhylumsAllList;
        set
        {
            _tbl06PhylumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefSource> _tbl90SourcesAllList = null!;

    public ObservableCollection<Tbl90RefSource> Tbl90SourcesAllList
    {
        get => _tbl90SourcesAllList;
        set
        {
            _tbl90SourcesAllList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefExpert> _tbl90ExpertsAllList = null!;

    public ObservableCollection<Tbl90RefExpert> Tbl90ExpertsAllList
    {
        get => _tbl90ExpertsAllList;
        set
        {
            _tbl90ExpertsAllList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceAuthorsList = null!;

    public ObservableCollection<Tbl90Reference> Tbl90ReferenceAuthorsList
    {
        get => _tbl90ReferenceAuthorsList;
        set
        {
            _tbl90ReferenceAuthorsList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceSourcesList = null!;

    public ObservableCollection<Tbl90Reference> Tbl90ReferenceSourcesList
    {
        get => _tbl90ReferenceSourcesList;
        set
        {
            _tbl90ReferenceSourcesList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceExpertsList = null!;

    public ObservableCollection<Tbl90Reference> Tbl90ReferenceExpertsList
    {
        get => _tbl90ReferenceExpertsList;
        set
        {
            _tbl90ReferenceExpertsList = value;
            OnPropertyChanged();
        }
    }



    private ObservableCollection<Tbl93Comment> _tbl93CommentsList = null!;

    public ObservableCollection<Tbl93Comment> Tbl93CommentsList
    {
        get => _tbl93CommentsList;
        set
        {
            _tbl93CommentsList = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<TblCountry> _tblCountriesAllList = null!;

    public ObservableCollection<TblCountry> TblCountriesAllList
    {
        get => _tblCountriesAllList;
        set
        {
            _tblCountriesAllList = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<TblDisease> _tblDiseasesList = null!;

    public ObservableCollection<TblDisease> TblDiseasesList
    {
        get => _tblDiseasesList;
        set
        {
            _tblDiseasesList = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<TblFood> _tblFoodsList = null!;

    public ObservableCollection<TblFood> TblFoodsList
    {
        get => _tblFoodsList;
        set
        {
            _tblFoodsList = value;
            OnPropertyChanged();
        }
    }
    private ObservableCollection<TblMiscellaneous> _tblMiscellaneoussList = null!;

    public ObservableCollection<TblMiscellaneous> TblMiscellaneoussList
    {
        get => _tblMiscellaneoussList;
        set
        {
            _tblMiscellaneoussList = value;
            OnPropertyChanged();
        }
    }

    //private void GetValueContinent()
    //{
    //    _continents = new List<Continent>()
    //        {
    //            new() { Name = "Africa" },
    //            new() { Name = "Antarctica" },
    //            new() { Name = "Asia" },
    //            new() { Name = "Australia" },
    //            new() { Name = "Central/South America" },
    //            new() { Name = "Europe" },
    //            new() { Name = "North America/Caribbean" }
    //        };

    //    _selectedContinent = new Continent();
    //}

    private List<Continent> _continents = null!;

    public List<Continent> Continents
    {
        get => _continents;
        set
        {
            _continents = value;
            OnPropertyChanged();
        }
    }

    private Continent _selectedContinent = null!;

    public Continent SelectedContinent
    {
        get => _selectedContinent;
        set
        {
            _selectedContinent = value;
            OnPropertyChanged();
        }
    }


    public class Continent
    {
        public string Name
        {
            get; set;
        }
    }

    private ObservableCollection<TblCountry> _tblCountriesList = null!;

    public ObservableCollection<TblCountry> TblCountriesList
    {
        get => _tblCountriesList;
        set
        {
            _tblCountriesList = value;
            OnPropertyChanged();
        }
    }



    private ObservableCollection<Tbl90RefAuthor> _tbl90AuthorsAllList = null!;

    public ObservableCollection<Tbl90RefAuthor> Tbl90AuthorsAllList
    {
        get => _tbl90AuthorsAllList;
        set
        {
            _tbl90AuthorsAllList = value;
            OnPropertyChanged();
        }
    }
    //private void GetValueLanguage()
    //{
    //    _languages = new List<Language>()
    //        {
    //            new() { Name = "GER" },
    //            new() { Name = "ENG" },
    //            new() { Name = "FRE" },
    //            new() { Name = "POR" }
    //        };

    //    _selectedLanguage = new Language();
    //}

    private List<Language> _languages = null!;

    public List<Language> Languages
    {
        get => _languages;
        set
        {
            _languages = value;
            OnPropertyChanged(nameof(Languages));
        }
    }

    private Language _selectedLanguage = null!;

    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            OnPropertyChanged(nameof(SelectedLanguage));
        }
    }

    public class Language
    {
        public string Name
        {
            get; set;
        }
    }
    //private void GetValueMimeType()
    //{
    //    _mimeTypes = new List<MimeType>()
    //        {
    //            new() { Name = "jpg" },
    //            new() { Name = "png" },
    //            new() { Name = "bmp" },
    //            new() { Name = "tiff" },
    //            new() { Name = "gif" },
    //            new() { Name = "icon" },
    //            new() { Name = "jpeg" },
    //            new() { Name = "wmf" },
    //            new() { Name = "wmv" },
    //            new() { Name = "mpg" },
    //            new() { Name = "mp4" },
    //            new() { Name = "avi" },
    //            new() { Name = "mov" },
    //            new() { Name = "swf" },
    //            new() { Name = "flv" }
    //        };

    //    _selectedMimeType = new MimeType();
    //}

    private List<MimeType> _mimeTypes = null!;

    public List<MimeType> MimeTypes
    {
        get => _mimeTypes;
        set
        {
            _mimeTypes = value;
            OnPropertyChanged(nameof(MimeTypes));
        }
    }

    private MimeType _selectedMimeType = null!;

    public MimeType SelectedMimeType
    {
        get => _selectedMimeType;
        set
        {
            _selectedMimeType = value;
            OnPropertyChanged(nameof(SelectedMimeType));
        }
    }

    public class MimeType
    {
        public string Name
        {
            get; set;
        }
    }
    public static RelayCommand OpenCommand
    {
        get; set;
    }
    private string _selectedPath = null!;

    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            _selectedPath = value; OnPropertyChanged(nameof(SelectedPath));
        }
    }



    public readonly string DefaultPath = null!;



    #endregion Public Properties


}
