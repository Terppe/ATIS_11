using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using RelayCommand = WinUICommunity.RelayCommand;

namespace ATIS.Views.Report;

public class ReportViewModel : ObservableObject
{
    #region [Private Data Members]

    private readonly IDataService _dataService;
    private readonly AtisDbContext _context = new();
    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
    public ObservableCollection<TblFood> FoodItems { get; } = [];
    private static int _fishId;
    private static int _plantId;

    #endregion [Private Data Members]

    #region [Constructor]

    public ReportViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public ReportViewModel()
    {
    }

    #endregion [Constructor]

    private void ClearAllCollections()
    {
        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();

        GeographicsCollection?.Clear();
        SynonymsCollection?.Clear();
        ImagesCollection?.Clear();
        NamesCollection?.Clear();
        PlSpeciessesSubCollection?.Clear();
        PlSpeciessesCollection?.Clear();
        FiSpeciessesSubCollection?.Clear();
        FiSpeciessesCollection?.Clear();
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
        SubdivisionsCollection?.Clear();
        SubphylumsCollection?.Clear();
        DivisionsCollection?.Clear();
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
            return;
        }

        IsLoading = true;

        GenussesCollection = _dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(filterText);

        DataSetCount = GenussesCollection.Count;
        RefreshGenusItems();
        IsLoading = false;
        SelectedTabChange(filterText);
    }

    private void SelectedTabChange(string filterText)
    {
        if (_selectedMainTabIndex == 0)  //Genus
        {
            IsLoading = true;
            GenussesCollection = _dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(filterText);

            DataSetCount = GenussesCollection.Count;
            RefreshGenusItems();
            IsLoading = false;
        }
        if (_selectedMainTabIndex == 1)  //FiSpecies
        {
            IsLoading = true;
            FiSpeciessesCollection =
                _dataService
                    .GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(filterText);



            //FiSpeciessesCollection = new ObservableCollection<Tbl69FiSpecies>(_context.Tbl69FiSpeciesses
            //    .Include(a => a.Tbl66Genusses)
            //    .Where(e => e.FiSpeciesName.StartsWith(filterText) &&
            //                e.FiSpeciesName.Contains("#") == false ||
            //                e.Tbl66Genusses.GenusName.Contains(filterText) ||
            //                e.Subspecies.Contains(filterText) ||
            //                e.Divers.Contains(filterText) ||
            //                e.TradeName.Contains(filterText) ||
            //                e.Author.Contains(filterText)
            //    )
            //    .OrderBy(a => a.Tbl66Genusses.GenusName)
            //    .ThenBy(a => a.FiSpeciesName)
            //    .ThenBy(a => a.Subspecies)
            //    .ThenBy(a => a.Divers)
            //);

            DataSetCount = FiSpeciessesCollection.Count;

            OnPropertyChanged(nameof(FiSpeciessesCollection));
            RefreshFiSpeciesItems();
            IsLoading = false;
        }
        if (_selectedMainTabIndex == 2)  //PlSpecies
        {
            PlSpeciessesCollection = _dataService.GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(filterText);
            DataSetCount = PlSpeciessesCollection.Count;

            OnPropertyChanged(nameof(PlSpeciessesCollection));
            RefreshPlSpeciesItems();
        }
        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 0)
        {
            InfratribussesCollection = _dataService.GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(filterText);
            DataSetCount = InfratribussesCollection.Count;

            OnPropertyChanged(nameof(InfratribussesCollection));
            RefreshInfratribusItems();
        }
        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 1)
        {
            SubtribussesCollection = _dataService.GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(filterText);
            DataSetCount = SubtribussesCollection.Count;

            OnPropertyChanged(nameof(SubtribussesCollection));
            RefreshSubtribusItems();
        }
        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 2)
        {
            TribussesCollection = _dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(filterText);
            DataSetCount = TribussesCollection.Count;

            OnPropertyChanged(nameof(TribussesCollection));
            RefreshTribusItems();
        }
        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 3)
        {
            SupertribussesCollection = _dataService.GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(filterText);
            DataSetCount = SupertribussesCollection.Count;

            OnPropertyChanged(nameof(SupertribussesCollection));
            RefreshSupertribusItems();
        }
        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 0)
        {
            InfrafamiliesCollection = _dataService.GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(filterText);
            DataSetCount = InfrafamiliesCollection.Count;

            OnPropertyChanged(nameof(InfrafamiliesCollection));
            RefreshInfrafamilyItems();
        }
        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 1)
        {
            SubfamiliesCollection = _dataService.GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(filterText);
            DataSetCount = SubfamiliesCollection.Count;

            OnPropertyChanged(nameof(SubfamiliesCollection));
            RefreshSubfamilyItems();
        }
        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 2)
        {
            FamiliesCollection = _dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(filterText);
            DataSetCount = FamiliesCollection.Count;

            OnPropertyChanged(nameof(FamiliesCollection));
            RefreshFamilyItems();
        }
        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 3)
        {
            SuperfamiliesCollection = _dataService.GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(filterText);
            DataSetCount = SuperfamiliesCollection.Count;

            OnPropertyChanged(nameof(SuperfamiliesCollection));
            RefreshSuperfamilyItems();
        }
        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 0)
        {
            InfraordosCollection = _dataService.GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(filterText);
            DataSetCount = InfraordosCollection.Count;

            OnPropertyChanged(nameof(InfraordosCollection));
            RefreshInfraordoItems();
        }
        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 1)
        {
            SubordosCollection = _dataService.GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(filterText);
            DataSetCount = SubordosCollection.Count;

            OnPropertyChanged(nameof(SubordosCollection));
            RefreshSubordoItems();
        }
        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 2)
        {
            OrdosCollection = _dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(filterText);
            DataSetCount = OrdosCollection.Count;

            OnPropertyChanged(nameof(OrdosCollection));
            RefreshOrdoItems();
        }
        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 3)
        {
            LegiosCollection = _dataService.GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(filterText);
            DataSetCount = LegiosCollection.Count;

            OnPropertyChanged(nameof(LegiosCollection));
            RefreshLegioItems();
        }
        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 0)
        {
            InfraclassesCollection = _dataService.GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(filterText);
            DataSetCount = InfraclassesCollection.Count;

            OnPropertyChanged(nameof(InfraclassesCollection));
            RefreshInfraclassItems();
        }
        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 1)
        {
            SubclassesCollection = _dataService.GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(filterText);
            DataSetCount = SubclassesCollection.Count;

            OnPropertyChanged(nameof(SubclassesCollection));
            RefreshSubclassItems();
        }
        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 2)
        {
            ClassesCollection = _dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(filterText);
            DataSetCount = ClassesCollection.Count;

            OnPropertyChanged(nameof(ClassesCollection));
            RefreshClassItems();
        }
        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 3)
        {
            SuperclassesCollection = _dataService.GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(filterText);
            DataSetCount = SuperclassesCollection.Count;

            OnPropertyChanged(nameof(SuperclassesCollection));
            RefreshSuperclassItems();
        }
        if (_selectedMainTabIndex == 7 && _selectedDivisionTabIndex == 0)
        {
            SubdivisionsCollection = _dataService.GetTbl15SubdivisionsCollectionOrderBySubdivisionNameFromFilterText(filterText);
            DataSetCount = SubdivisionsCollection.Count;

            OnPropertyChanged(nameof(SubdivisionsCollection));
            RefreshSubdivisionItems();
        }
        if (_selectedMainTabIndex == 7 && _selectedDivisionTabIndex == 1)
        {
            DivisionsCollection = _dataService.GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(filterText);
            DataSetCount = DivisionsCollection.Count;

            OnPropertyChanged(nameof(DivisionsCollection));
            RefreshDivisionItems();
        }
        if (_selectedMainTabIndex == 8 && _selectedPhylumTabIndex == 0)
        {
            SubphylumsCollection = _dataService.GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(filterText);
            DataSetCount = SubphylumsCollection.Count;

            OnPropertyChanged(nameof(SubphylumsCollection));
            RefreshSubphylumItems();
        }
        if (_selectedMainTabIndex == 8 && _selectedPhylumTabIndex == 1)
        {
            PhylumsCollection = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(filterText);
            DataSetCount = PhylumsCollection.Count;

            OnPropertyChanged(nameof(PhylumsCollection));
            RefreshPhylumItems();
        }
        if (_selectedMainTabIndex == 9 && _selectedRegnumTabIndex == 0)
        {
            RegnumsCollection = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(filterText);
            DataSetCount = RegnumsCollection.Count;

            OnPropertyChanged(nameof(RegnumsCollection));
            RefreshRegnumItems();
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
    private int _selectedReportDivisionTabIndex;
    private int _selectedReportPhylumTabIndex;
    private int _selectedReportRegnumTabIndex;

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
                Tbl63InfratribussesAllList = _dataService.GetInfratribusAsync();
                GenussesCollection = _dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(FilterText);
                DataSetCount = GenussesCollection.Count;
                RefreshGenusItems();
            }

            if (_selectedMainTabIndex == 1)
            {
                ClearAllCollections();
                Tbl66GenussesAllList ??= [];
                Tbl66GenussesAllList = _dataService.GetGenusAsync();
                FiSpeciessesCollection = _dataService.GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(FilterText);

                DataSetCount = FiSpeciessesCollection.Count;
                RefreshFiSpeciesItems();
            }

            if (_selectedMainTabIndex == 2)
            {
                ClearAllCollections();
                Tbl66GenussesAllList ??= [];
                Tbl66GenussesAllList = _dataService.GetGenusAsync();
                PlSpeciessesCollection = _dataService.GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(FilterText);

                DataSetCount = PlSpeciessesCollection.Count;
                RefreshPlSpeciesItems();
            }

            if (_selectedMainTabIndex == 3)  //Tribus
            {
                ClearAllCollections();
                Tbl54SupertribussesAllList ??= [];
                Tbl54SupertribussesAllList = _dataService.GetSupertribusAsync();
                TribussesCollection = _dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(FilterText);
                DataSetCount = TribussesCollection.Count;

                RefreshTribusItems();
                SelectedTribusTabIndex = 2;
            }
            if (_selectedMainTabIndex == 4)  //Family
            {
                ClearAllCollections();
                Tbl42SuperfamiliesAllList ??= [];
                Tbl42SuperfamiliesAllList = _dataService.GetSuperfamilyAsync();
                FamiliesCollection = _dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(FilterText);

                DataSetCount = FamiliesCollection.Count;
                RefreshFamilyItems();
                SelectedFamilyTabIndex = 2;
            }
            if (_selectedMainTabIndex == 5)  //Ordo
            {
                ClearAllCollections();
                Tbl30LegiosAllList ??= [];
                Tbl30LegiosAllList = _dataService.GetLegioAsync();
                OrdosCollection = _dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(FilterText);
                DataSetCount = OrdosCollection.Count;

                RefreshOrdoItems();
                SelectedOrdoTabIndex = 2;
            }
            if (_selectedMainTabIndex == 6)  //Class
            {
                ClearAllCollections();
                Tbl18SuperclassesAllList ??= [];
                Tbl18SuperclassesAllList = _dataService.GetSuperclassAsync();
                ClassesCollection = _dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(FilterText);
                DataSetCount = ClassesCollection.Count;

                RefreshClassItems();
                SelectedClassTabIndex = 2;
            }
            if (_selectedMainTabIndex == 7)  //Division
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = _dataService.GetRegnumAsync();
                DivisionsCollection = _dataService.GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(FilterText);
                DataSetCount = DivisionsCollection.Count;

                RefreshDivisionItems();

                SelectedDivisionTabIndex = 1;
            }
            if (_selectedMainTabIndex == 8)  //Phylum
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = _dataService.GetRegnumAsync();

                PhylumsCollection ??= [];
                PhylumsCollection = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
                DataSetCount = PhylumsCollection.Count;

                RefreshPhylumItems();
                SelectedPhylumTabIndex = 1;
            }
            if (_selectedMainTabIndex == 9)  //Regnum
            {
                ClearAllCollections();
                RegnumsCollection ??= [];
                RegnumsCollection = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(FilterText);

                DataSetCount = RegnumsCollection.Count;
                RefreshRegnumItems();

                SelectedRegnumTabIndex = 0;
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
    public int SelectedReportDivisionTabIndex
    {
        get => _selectedReportDivisionTabIndex;
        set
        {
            if (value == _selectedReportDivisionTabIndex)
            {
                return;
            }

            _selectedReportDivisionTabIndex = value; OnPropertyChanged();

            if (_selectedReportDivisionTabIndex == 0)
            {
            }

            if (_selectedReportDivisionTabIndex == 1)
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

    private int _selectedTribusTabIndex;
    private int _selectedFamilyTabIndex;
    private int _selectedOrdoTabIndex;
    private int _selectedClassTabIndex;
    private int _selectedDivisionTabIndex;
    private int _selectedPhylumTabIndex;
    private int _selectedRegnumTabIndex;

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
                Tbl60SubtribussesAllList = _dataService.GetSubtribusAsync();
                InfratribussesCollection = _dataService.GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(FilterText);
                DataSetCount = InfratribussesCollection.Count;

                OnPropertyChanged(nameof(InfratribussesCollection));
                RefreshInfratribusItems();
            }

            if (_selectedTribusTabIndex == 1)
            {
                ClearAllCollections();
                Tbl57TribussesAllList ??= [];
                Tbl57TribussesAllList = _dataService.GetTribusAsync();
                SubtribussesCollection = _dataService.GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(FilterText);
                DataSetCount = SubtribussesCollection.Count;

                OnPropertyChanged(nameof(SubtribussesCollection));
                RefreshSubtribusItems();
            }

            if (_selectedTribusTabIndex == 2)
            {
                ClearAllCollections();
                Tbl54SupertribussesAllList ??= [];
                Tbl54SupertribussesAllList = _dataService.GetSupertribusAsync();
                TribussesCollection = _dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(FilterText);
                DataSetCount = TribussesCollection.Count;

                OnPropertyChanged(nameof(TribussesCollection));
                RefreshTribusItems();
            }

            if (_selectedTribusTabIndex == 3)
            {
                ClearAllCollections();
                Tbl51InfrafamiliesAllList ??= [];
                Tbl51InfrafamiliesAllList = _dataService.GetInfrafamilyAsync();
                SupertribussesCollection = _dataService.GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(FilterText);
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
                Tbl48SubfamiliesAllList = _dataService.GetSubfamilyAsync();
                InfrafamiliesCollection = _dataService.GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(FilterText);
                DataSetCount = InfrafamiliesCollection.Count;

                OnPropertyChanged(nameof(InfrafamiliesCollection));
                RefreshInfrafamilyItems();
            }

            if (_selectedFamilyTabIndex == 1)
            {
                ClearAllCollections();
                Tbl45FamiliesAllList ??= [];
                Tbl45FamiliesAllList = _dataService.GetFamilyAsync();
                SubfamiliesCollection = _dataService.GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(FilterText);
                DataSetCount = SubfamiliesCollection.Count;

                OnPropertyChanged(nameof(SubfamiliesCollection));
                RefreshSubfamilyItems();
            }

            if (_selectedFamilyTabIndex == 2)
            {
                ClearAllCollections();
                Tbl42SuperfamiliesAllList ??= [];
                Tbl42SuperfamiliesAllList = _dataService.GetSuperfamilyAsync();
                FamiliesCollection = _dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(FilterText);
                DataSetCount = FamiliesCollection.Count;

                OnPropertyChanged(nameof(FamiliesCollection));
                RefreshFamilyItems();
            }

            if (_selectedFamilyTabIndex == 3)
            {
                ClearAllCollections();
                Tbl39InfraordosAllList ??= [];
                Tbl39InfraordosAllList = _dataService.GetInfraordoAsync();
                SuperfamiliesCollection = _dataService.GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(FilterText);
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
                Tbl36SubordosAllList = _dataService.GetSubordoAsync();
                InfraordosCollection = _dataService.GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(FilterText);
                DataSetCount = InfraordosCollection.Count;

                OnPropertyChanged(nameof(InfraordosCollection));
                RefreshInfraordoItems();
            }

            if (_selectedOrdoTabIndex == 1)
            {
                ClearAllCollections();
                Tbl33OrdosAllList ??= [];
                Tbl33OrdosAllList = _dataService.GetOrdoAsync();
                SubordosCollection = _dataService.GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(FilterText);
                DataSetCount = SubordosCollection.Count;

                OnPropertyChanged(nameof(SubordosCollection));
                RefreshSubordoItems();
            }

            if (_selectedOrdoTabIndex == 2)
            {
                ClearAllCollections();
                Tbl30LegiosAllList ??= [];
                Tbl30LegiosAllList = _dataService.GetLegioAsync();
                OrdosCollection = _dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(FilterText);
                DataSetCount = OrdosCollection.Count;

                OnPropertyChanged(nameof(OrdosCollection));
                RefreshOrdoItems();
            }

            if (_selectedOrdoTabIndex == 3)
            {
                ClearAllCollections();
                Tbl27InfraclassesAllList ??= [];
                Tbl27InfraclassesAllList = _dataService.GetInfraclassAsync();
                LegiosCollection = _dataService.GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(FilterText);
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
                Tbl24SubclassesAllList = _dataService.GetSubclassAsync();
                InfraclassesCollection = _dataService.GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(FilterText);
                DataSetCount = InfraclassesCollection.Count;

                OnPropertyChanged(nameof(InfraclassesCollection));
                RefreshInfraclassItems();
            }

            if (_selectedClassTabIndex == 1)
            {
                ClearAllCollections();
                Tbl21ClassesAllList ??= [];
                Tbl21ClassesAllList = _dataService.GetClassAsync();
                SubclassesCollection = _dataService.GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(FilterText);
                DataSetCount = SubclassesCollection.Count;

                OnPropertyChanged(nameof(SubclassesCollection));
                RefreshSubclassItems();
            }

            if (_selectedClassTabIndex == 2)
            {
                ClearAllCollections();
                Tbl18SuperclassesAllList ??= [];
                Tbl18SuperclassesAllList = _dataService.GetSuperclassAsync();
                ClassesCollection = _dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(FilterText);
                DataSetCount = ClassesCollection.Count;

                OnPropertyChanged(nameof(ClassesCollection));
                RefreshClassItems();
            }

            if (_selectedClassTabIndex == 3)
            {
                ClearAllCollections();
                Tbl15SubdivisionsAllList ??= [];
                Tbl15SubdivisionsAllList = _dataService.GetSubdivisionAsync();
                Tbl12SubphylumsAllList ??= [];
                Tbl12SubphylumsAllList = _dataService.GetSubphylumAsync();
                SuperclassesCollection = _dataService.GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(FilterText);
                DataSetCount = SuperclassesCollection.Count;

                OnPropertyChanged(nameof(SuperclassesCollection));
                RefreshSuperclassItems();
            }
        }
    }
    public int SelectedDivisionTabIndex
    {
        get => _selectedDivisionTabIndex;
        set
        {
            if (value == _selectedDivisionTabIndex)
            {
                return;
            }

            _selectedDivisionTabIndex = value; OnPropertyChanged();

            if (_selectedDivisionTabIndex == 0)
            {
                ClearAllCollections();
                Tbl09DivisionsAllList ??= [];
                Tbl09DivisionsAllList = _dataService.GetDivisionAsync();
                SubdivisionsCollection = _dataService.GetTbl15SubdivisionsCollectionOrderBySubdivisionNameFromFilterText(FilterText);
                DataSetCount = SubdivisionsCollection.Count;

                OnPropertyChanged(nameof(SubdivisionsCollection));
                RefreshSubdivisionItems();
            }

            if (_selectedDivisionTabIndex == 1)
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = _dataService.GetRegnumAsync();
                DivisionsCollection = _dataService.GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(FilterText);
                DataSetCount = DivisionsCollection.Count;

                OnPropertyChanged(nameof(DivisionsCollection));
                RefreshDivisionItems();
            }
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
                Tbl06PhylumsAllList = _dataService.GetPhylumAsync();
                SubphylumsCollection = _dataService.GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(FilterText);
                DataSetCount = SubphylumsCollection.Count;

                OnPropertyChanged(nameof(SubphylumsCollection));
                RefreshSubphylumItems();
            }

            if (_selectedPhylumTabIndex == 1)
            {
                ClearAllCollections();
                Tbl03RegnumsAllList ??= [];
                Tbl03RegnumsAllList = _dataService.GetRegnumAsync();
                PhylumsCollection = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
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
                RegnumsCollection = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(FilterText);
                DataSetCount = RegnumsCollection.Count;

                OnPropertyChanged(nameof(RegnumsCollection));
                RefreshRegnumItems();
            }
        }
    }

    #endregion "Public Commands to change TabItems"

    #region Report Search

    public void ReportSearchAll(int id, string tab)
    {
        ClearAllCollections();
        if (_context.Tbl15Subdivisions != null)
        {
            var plantaeRegnum = _context.Tbl15Subdivisions.FirstOrDefault(e => e.SubdivisionName == "Plantae#Regnum#");
            if (plantaeRegnum != null)
            {
                _plantId = plantaeRegnum.SubdivisionId;
            }
        }

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
            case "Tbl09Divisions":
                GetTbl09DivisionsById(id);
                break;
            case "Tbl12Subphylums":
                GetTbl12SubphylumsById(id);
                break;
            case "Tbl15Subdivisions":
                GetTbl15SubdivisionsById(id);
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
            //  case "Tbl68Speciesgroups":
            ////      GetTbl68SpeciesgroupsById(id);
            //      break;
            case "Tbl69FiSpeciesses":
                var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
                if (string.IsNullOrEmpty(fiSpecies.Subspecies) && string.IsNullOrEmpty(fiSpecies.Divers))
                {
                    GetTbl69FiSpeciessesById(id);
                }
                else
                {
                    GetTbl69FiSpeciessesSubById(id);
                }
                break;
            case "Tbl69FiSpeciessesSub":
                GetTbl69FiSpeciessesSubById(id);
                break;
            case "Tbl72PlSpeciesses":
                var plSpecies = _dataService.GetPlSpeciesSingleModelByPlSpeciesId(id);
                if (string.IsNullOrEmpty(plSpecies.Subspecies) && string.IsNullOrEmpty(plSpecies.Divers))
                {
                    GetTbl72PlSpeciessesById(id);
                }
                else
                {
                    GetTbl72PlSpeciessesSubById(id);
                }
                break;
            case "Tbl72PlSpeciessesSub":
                GetTbl72PlSpeciessesSubById(id);
                break;
            case "Tbl75Speciesses":
                var species = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
                if (string.IsNullOrEmpty(species.Subspecies) && string.IsNullOrEmpty(species.Divers))
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

                //case "Tbl78Names":
                //    GetTbl78NamesById(id);
                //    break;
                //case "Tbl84Synonyms":
                //    GetTbl84SynonymsById(id);
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
        if (DivisionsCollection != null)
        {
            RefreshDivisionItems();
        }
        if (SubdivisionsCollection != null)
        {
            RefreshSubdivisionItems();
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
        if (FiSpeciessesCollection != null)
        {
            RefreshFiSpeciesItems();
        }
        if (FiSpeciessesSubCollection != null)
        {
            RefreshFiSpeciesSubItems();
        }
        if (FiSpeciessesFiSpeciesNameCollection != null)
        {
            //  RefreshFiSpeciesFiSpeciesNameItems();
        }


        if (PlSpeciessesCollection != null)
        {
            RefreshPlSpeciesItems();
        }
        if (PlSpeciessesSubCollection != null)
        {
            RefreshPlSpeciesSubItems();
        }
        if (PlSpeciessesPlSpeciesNameCollection != null)
        {
            //  RefreshPlSpeciesFPlSpeciesNameItems();
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
        RegnumsCollection = _dataService.CollRegnumsByRegnumId(id);

        //direct children
        PhylumsCollection = _dataService.CollPhylumsByRegnumIdAndHash(id);
        DivisionsCollection = _dataService.CollDivisionsByRegnumIdAndHash(id);
        //------------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByRegnumId(id);
        SourcesCollection = _dataService.CollSourcesByRegnumId(id);
        AuthorsCollection = _dataService.CollAuthorsByRegnumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByRegnumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Phylum
    public void GetTbl06PhylumsById(int id)
    {
        PhylumsCollection = _dataService.CollPhylumsByPhylumId(id);

        var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(id);
        RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SubphylumsCollection = _dataService.CollSubphylumsByPhylumIdAndHash(id);
        //------------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByPhylumId(id);
        SourcesCollection = _dataService.CollSourcesByPhylumId(id);
        AuthorsCollection = _dataService.CollAuthorsByPhylumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByPhylumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Division

    public void GetTbl09DivisionsById(int id)
    {
        DivisionsCollection = _dataService.CollDivisionsByDivisionId(id);
        var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(id);
        RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SubdivisionsCollection = _dataService.CollSubdivisionsByDivisionIdAndHash(id);
        //------------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByDivisionId(id);
        SourcesCollection = _dataService.CollSourcesByDivisionId(id);
        AuthorsCollection = _dataService.CollAuthorsByDivisionId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByDivisionId(id);
        //----------Refresh-------------
        Refresh();
    }

    #endregion

    #region Subphylum
    public void GetTbl12SubphylumsById(int id)
    {
        SubphylumsCollection = _dataService.CollSubphylumsBySubphylumId(id);

        var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(id);
        PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
        var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
        RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SuperclassesCollection = _dataService.CollSuperclassesBySubphylumIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubphylumId(id);
        SourcesCollection = _dataService.CollSourcesBySubphylumId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubphylumId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubphylumId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subdivision
    public void GetTbl15SubdivisionsById(int id)
    {
        SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionId(id);

        var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(id);
        DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //direct children
        SuperclassesCollection = _dataService.CollSuperclassesBySubdivisionIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubdivisionId(id);
        SourcesCollection = _dataService.CollSourcesBySubdivisionId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubdivisionId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubdivisionId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Superclass
    public void GetTbl18SuperclassesById(int id)
    {
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassId(id);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(id);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(id);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        ClassesCollection = _dataService.CollClassesBySuperclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySuperclassId(id);
        SourcesCollection = _dataService.CollSourcesBySuperclassId(id);
        AuthorsCollection = _dataService.CollAuthorsBySuperclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySuperclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Class
    public void GetTbl21ClassesById(int id)
    {
        ClassesCollection = _dataService.CollClassesByClassId(id);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(id);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        SubclassesCollection = _dataService.CollSubclassesByClassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByClassId(id);
        SourcesCollection = _dataService.CollSourcesByClassId(id);
        AuthorsCollection = _dataService.CollAuthorsByClassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByClassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subclass
    public void GetTbl24SubclassesById(int id)
    {
        SubclassesCollection = _dataService.CollSubclassesBySubclassId(id);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(id);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        InfraclassesCollection = _dataService.CollInfraclassesBySubclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubclassId(id);
        SourcesCollection = _dataService.CollSourcesBySubclassId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infraclass
    public void GetTbl27InfraclassesById(int id)
    {
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassId(id);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(id);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        LegiosCollection = _dataService.CollLegiosByInfraclassIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByInfraclassId(id);
        SourcesCollection = _dataService.CollSourcesByInfraclassId(id);
        AuthorsCollection = _dataService.CollAuthorsByInfraclassId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByInfraclassId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Legio
    public void GetTbl30LegiosById(int id)
    {
        LegiosCollection = _dataService.CollLegiosByLegioId(id);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(id);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        OrdosCollection = _dataService.CollOrdosByLegioIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByLegioId(id);
        SourcesCollection = _dataService.CollSourcesByLegioId(id);
        AuthorsCollection = _dataService.CollAuthorsByLegioId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByLegioId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Ordo
    public void GetTbl33OrdosById(int id)
    {
        OrdosCollection = _dataService.CollOrdosByOrdoId(id);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(id);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubordosCollection = _dataService.CollSubordosByOrdoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByOrdoId(id);
        SourcesCollection = _dataService.CollSourcesByOrdoId(id);
        AuthorsCollection = _dataService.CollAuthorsByOrdoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByOrdoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subordo
    public void GetTbl36SubordosById(int id)
    {
        SubordosCollection = _dataService.CollSubordosBySubordoId(id);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(id);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfraordosCollection = _dataService.CollInfraordosBySubordoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubordoId(id);
        SourcesCollection = _dataService.CollSourcesBySubordoId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubordoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubordoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infraordo
    public void GetTbl39InfraordosById(int id)
    {
        InfraordosCollection = _dataService.CollInfraordosByInfraordoId(id);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(id);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SuperfamiliesCollection = _dataService.CollSuperfamiliesByInfraordoIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByInfraordoId(id);
        SourcesCollection = _dataService.CollSourcesByInfraordoId(id);
        AuthorsCollection = _dataService.CollAuthorsByInfraordoId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByInfraordoId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Superfamily
    public void GetTbl42SuperfamiliesById(int id)
    {
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyId(id);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(id);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        FamiliesCollection = _dataService.CollFamiliesBySuperfamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySuperfamilyId(id);
        SourcesCollection = _dataService.CollSourcesBySuperfamilyId(id);
        AuthorsCollection = _dataService.CollAuthorsBySuperfamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySuperfamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Family
    public void GetTbl45FamiliesById(int id)
    {
        FamiliesCollection = _dataService.CollFamiliesByFamilyId(id);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(id);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubfamiliesCollection = _dataService.CollSubfamiliesByFamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByFamilyId(id);
        SourcesCollection = _dataService.CollSourcesByFamilyId(id);
        AuthorsCollection = _dataService.CollAuthorsByFamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByFamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subfamily
    public void GetTbl48SubfamiliesById(int id)
    {
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyId(id);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(id);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfrafamiliesCollection = _dataService.CollInfrafamiliesBySubfamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubfamilyId(id);
        SourcesCollection = _dataService.CollSourcesBySubfamilyId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubfamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubfamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infrafamily
    public void GetTbl51InfrafamiliesById(int id)
    {
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyId(id);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(id);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SupertribussesCollection = _dataService.CollSupertribussesByInfrafamilyIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByInfrafamilyId(id);
        SourcesCollection = _dataService.CollSourcesByInfrafamilyId(id);
        AuthorsCollection = _dataService.CollAuthorsByInfrafamilyId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByInfrafamilyId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Supertribus
    public void GetTbl54SupertribussesById(int id)
    {
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusId(id);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(id);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        TribussesCollection = _dataService.CollTribussesBySupertribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySupertribusId(id);
        SourcesCollection = _dataService.CollSourcesBySupertribusId(id);
        AuthorsCollection = _dataService.CollAuthorsBySupertribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySupertribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Tribus
    public void GetTbl57TribussesById(int id)
    {
        TribussesCollection = _dataService.CollTribussesByTribusId(id);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(id);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        SubtribussesCollection = _dataService.CollSubtribussesByTribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByTribusId(id);
        SourcesCollection = _dataService.CollSourcesByTribusId(id);
        AuthorsCollection = _dataService.CollAuthorsByTribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByTribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Subtribus
    public void GetTbl60SubtribussesById(int id)
    {
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusId(id);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(id);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        InfratribussesCollection = _dataService.CollInfratribussesBySubtribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsBySubtribusId(id);
        SourcesCollection = _dataService.CollSourcesBySubtribusId(id);
        AuthorsCollection = _dataService.CollAuthorsBySubtribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsBySubtribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Infratribus
    public void GetTbl63InfratribussesById(int id)
    {
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusId(id);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(id);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        GenussesCollection = _dataService.CollGenussesByInfratribusIdAndHash(id);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByInfratribusId(id);
        SourcesCollection = _dataService.CollSourcesByInfratribusId(id);
        AuthorsCollection = _dataService.CollAuthorsByInfratribusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByInfratribusId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Genus
    public void GetTbl66GenussesById(int id)
    {
        GenussesCollection = _dataService.CollGenussesByGenusId(id);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(id);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }


        //direct children
        //      FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndSubspeciesIsNullAndHash(id);
        FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndHash(id);
        //FiSpeciessesCollection = new ObservableCollection<Tbl69FiSpecies>(_context.Tbl69FiSpeciesses
        //    .Include(a => a.Tbl66Genusses)
        //    .Where(e => e.GenusId == id &&
        //                e.Subspecies == null &&
        //                e.FiSpeciesName.Contains("#") == false));

        //  PlSpeciessesCollection = _dataService.CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(id);
        PlSpeciessesCollection = _dataService.CollPlSpeciessesByGenusIdAndHash(id);


        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByGenusId(id);
        SourcesCollection = _dataService.CollSourcesByGenusId(id);
        AuthorsCollection = _dataService.CollAuthorsByGenusId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByGenusId(id);

        //----------Refresh-------------
        Refresh();
    }


    #endregion

    #region Speciesgroup

    //private void GetTbl68SpeciesgroupsById(int id)
    //{

    //}

    #endregion

    #region FiSpecies
    public void GetTbl69FiSpeciessesById(int id)
    {
        var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);

        NamesCollection = _dataService.CollNamesByFiSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByFiSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByFiSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByFiSpeciesId(id);

        var genusId = _dataService.GenusIdFromFiSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesNameAndNotEmptySubspecies(fiSpecies.GenusId, fiSpecies.FiSpeciesName!);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByFiSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByFiSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByFiSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByFiSpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region FiSpeciesSub
    public void GetTbl69FiSpeciessesSubById(int id)
    {
        ////  var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        //FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);
        ////  FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);
        //FiSpeciessesSubCollection = FiSpeciessesCollection;

        //  var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);

        var fiSpeciessingle = _dataService.GetFiSpeciesSingleByFiSpeciesId(id);
        var fiSpeciesId = fiSpeciessingle.FiSpeciesId;
        FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(fiSpeciesId);


        // FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndFiSpeciesNameAndEmptySubspecies(fiSpecies.GenusId, fiSpecies.FiSpeciesName);
        NamesCollection = _dataService.CollNamesByFiSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByFiSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByFiSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByFiSpeciesId(id);

        var genusId = _dataService.GenusIdFromFiSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByFiSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByFiSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByFiSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByFiSpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region Species
    public void GetTbl75SpeciessesById(int id)
    {
        var fiSpecies = _dataService.GetFiSpeciesSingleModelByFiSpeciesId(id);
        FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);

        NamesCollection = _dataService.CollNamesByFiSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByFiSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByFiSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByFiSpeciesId(id);

        var genusId = _dataService.GenusIdFromFiSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //direct children
        FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesNameAndNotEmptySubspecies(fiSpecies.GenusId, fiSpecies.FiSpeciesName!);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByFiSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByFiSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByFiSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByFiSpeciesId(id);
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
        FiSpeciessesSubCollection = _dataService.CollFiSpeciessesByFiSpeciesId(id);

        var fiSpeciessingle = _dataService.GetFiSpeciesSingleByFiSpeciesId(id);
        var fiSpeciesId = fiSpeciessingle.FiSpeciesId;
        FiSpeciessesCollection = _dataService.CollFiSpeciessesByFiSpeciesId(fiSpeciesId);


        // FiSpeciessesCollection = _dataService.CollFiSpeciessesByGenusIdAndFiSpeciesNameAndEmptySubspecies(fiSpecies.GenusId, fiSpecies.FiSpeciesName);
        NamesCollection = _dataService.CollNamesByFiSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByFiSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByFiSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByFiSpeciesId(id);

        var genusId = _dataService.GenusIdFromFiSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByFiSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByFiSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByFiSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByFiSpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }
    #endregion

    #region PlSpecies

    public void GetTbl72PlSpeciessesById(int id)
    {
        var plSpecies = _dataService.GetPlSpeciesSingleModelByPlSpeciesId(id);
        PlSpeciessesCollection = _dataService.CollPlSpeciessesByPlSpeciesId(id);

        NamesCollection = _dataService.CollNamesByPlSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByPlSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByPlSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByPlSpeciesId(id);

        var genusId = _dataService.GenusIdFromPlSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId) //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        if (subdivisionId == _plantId) //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }

        //direct children
        PlSpeciessesSubCollection = _dataService.CollPlSpeciessesByPlSpeciesNameAndNotEmptySubspecies(plSpecies.GenusId, plSpecies.PlSpeciesName!);
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByPlSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByPlSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByPlSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByPlSpeciesId(id);
        //----------Refresh-------------
        Refresh();
    }

    #endregion

    #region PlSpeciesSub
    public void GetTbl72PlSpeciessesSubById(int id)
    {
        //    var plSpecies = _dataService.GetPlSpeciesSingleModelByPlSpeciesId(id);

        PlSpeciessesCollection = _dataService.CollPlSpeciessesByPlSpeciesId(id);
        //  PlSpeciessesSubCollection = _dataService.CollPlSpeciessesByPlSpeciesId(id);
        PlSpeciessesSubCollection = PlSpeciessesCollection;

        //   PlSpeciessesCollection = _dataService.CollPlSpeciessesByGenusIdAndPlSpeciesNameAndEmptySubspecies(plSpecies.GenusId, plSpecies.PlSpeciesName);
        NamesCollection = _dataService.CollNamesByPlSpeciesIdAndHash(id);
        SynonymsCollection = _dataService.CollSynonymsByPlSpeciesIdAndHash(id);
        ImagesCollection = _dataService.CollImagesByPlSpeciesId(id);
        GeographicsCollection = _dataService.CollGeographicsByPlSpeciesId(id);

        var genusId = _dataService.GenusIdFromPlSpeciessesCollectionSelect(id);
        GenussesCollection = _dataService.CollGenussesByGenusIdAndHash(genusId);
        var infratribusId = _dataService.InfratribusIdFromGenussesCollectionSelect(genusId);
        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);
        }
        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByPlSpeciesId(id);
        SourcesCollection = _dataService.CollSourcesByPlSpeciesId(id);
        AuthorsCollection = _dataService.CollAuthorsByPlSpeciesId(id);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByPlSpeciesId(id);
        //----------Refresh-------------
        Refresh();

    }
    //------------------------------------------------------------------------------
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

    private Tbl09Division _divisionSelected = null!;
    public Tbl09Division DivisionSelected
    {
        get => _divisionSelected;
        set => SetProperty(ref _divisionSelected, value);
    }

    private Tbl12Subphylum _subphylumSelected = null!;
    public Tbl12Subphylum SubphylumSelected
    {
        get => _subphylumSelected;
        set => SetProperty(ref _subphylumSelected, value);
    }
    private Tbl15Subdivision _subdivisionSelected = null!;
    public Tbl15Subdivision SubdivisionSelected
    {
        get => _subdivisionSelected;
        set => SetProperty(ref _subdivisionSelected, value);
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

    private Tbl69FiSpecies _fiSpeciesSelected = null!;
    public Tbl69FiSpecies FiSpeciesSelected
    {
        get => _fiSpeciesSelected;
        set => SetProperty(ref _fiSpeciesSelected, value);
    }

    private Tbl69FiSpecies _fiSpeciesSubSelected = null!;
    public Tbl69FiSpecies FiSpeciesSubSelected
    {
        get => _fiSpeciesSubSelected;
        set => SetProperty(ref _fiSpeciesSubSelected, value);
    }

    private Tbl72PlSpecies _plSpeciesSelected = null!;
    public Tbl72PlSpecies PlSpeciesSelected
    {
        get => _plSpeciesSelected;
        set => SetProperty(ref _plSpeciesSelected, value);
    }

    private Tbl72PlSpecies _plSpeciesSubSelected = null!;
    public Tbl72PlSpecies PlSpeciesSubSelected
    {
        get => _plSpeciesSubSelected;
        set => SetProperty(ref _plSpeciesSubSelected, value);
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
    private void RefreshDivisionItems()
    {
        DivisionItems.Clear();
        foreach (var item in DivisionsCollection)
        {
            DivisionItems.Add(item);
        }
        if (DivisionSelected == null && DivisionsCollection.Count != 0)
        {
            DivisionSelected = DivisionItems.FirstOrDefault();
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
    private void RefreshSubdivisionItems()
    {
        SubdivisionItems.Clear();
        foreach (var item in SubdivisionsCollection)
        {
            SubdivisionItems.Add(item);
        }
        if (SubdivisionSelected == null && SubdivisionsCollection.Count != 0)
        {
            SubdivisionSelected = SubdivisionItems.First();
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
    private void RefreshFiSpeciesItems()
    {
        FiSpeciesItems.Clear();
        foreach (var item in FiSpeciessesCollection)
        {
            FiSpeciesItems.Add(item);
        }
        if (FiSpeciesSelected == null && FiSpeciessesCollection.Count != 0)
        {
            FiSpeciesSelected = FiSpeciesItems.First();
        }
    }
    private void RefreshFiSpeciesSubItems()
    {
        FiSpeciesSubItems.Clear();
        foreach (var item in FiSpeciessesSubCollection)
        {
            FiSpeciesSubItems.Add(item);
        }
        if (FiSpeciesSubSelected == null && FiSpeciessesSubCollection.Count != 0)
        {
            FiSpeciesSubSelected = FiSpeciesSubItems.First();
        }

    }
    private void RefreshPlSpeciesItems()
    {
        PlSpeciesItems.Clear();
        foreach (var item in PlSpeciessesCollection)
        {
            PlSpeciesItems.Add(item);
        }
        if (PlSpeciesSelected == null && PlSpeciessesCollection.Count != 0)
        {
            PlSpeciesSelected = PlSpeciesItems.First();
        }
    }
    private void RefreshPlSpeciesSubItems()
    {
        PlSpeciesSubItems.Clear();
        foreach (var item in PlSpeciessesSubCollection)
        {
            PlSpeciesSubItems.Add(item);
        }
        if (PlSpeciesSubSelected == null && PlSpeciessesSubCollection.Count != 0)
        {
            PlSpeciesSubSelected = PlSpeciesSubItems.First();
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

    #endregion

    #region Items

    public ObservableCollection<Tbl03Regnum> RegnumItems { get; } = [];
    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = new();
    public ObservableCollection<Tbl09Division> DivisionItems { get; } = new();
    public ObservableCollection<Tbl12Subphylum> SubphylumItems { get; } = new();
    public ObservableCollection<Tbl15Subdivision> SubdivisionItems { get; } = new();
    public ObservableCollection<Tbl18Superclass> SuperclassItems { get; } = new();
    public ObservableCollection<Tbl21Class> ClassItems { get; } = new();
    public ObservableCollection<Tbl24Subclass> SubclassItems { get; } = new();
    public ObservableCollection<Tbl27Infraclass> InfraclassItems { get; } = new();
    public ObservableCollection<Tbl30Legio> LegioItems { get; } = new();
    public ObservableCollection<Tbl33Ordo> OrdoItems { get; } = new();
    public ObservableCollection<Tbl36Subordo> SubordoItems { get; } = new();
    public ObservableCollection<Tbl39Infraordo> InfraordoItems { get; } = new();
    public ObservableCollection<Tbl42Superfamily> SuperfamilyItems { get; } = new();
    public ObservableCollection<Tbl45Family> FamilyItems { get; } = new();
    public ObservableCollection<Tbl48Subfamily> SubfamilyItems { get; } = new();
    public ObservableCollection<Tbl51Infrafamily> InfrafamilyItems { get; } = new();
    public ObservableCollection<Tbl54Supertribus> SupertribusItems { get; } = new();
    public ObservableCollection<Tbl57Tribus> TribusItems { get; } = new();
    public ObservableCollection<Tbl60Subtribus> SubtribusItems { get; } = new();
    public ObservableCollection<Tbl63Infratribus> InfratribusItems { get; } = new();
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = new();
    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupItems { get; } = new();
    public ObservableCollection<Tbl69FiSpecies> FiSpeciesItems { get; } = new();
    public ObservableCollection<Tbl69FiSpecies> FiSpeciesSubItems { get; } = new();
    public ObservableCollection<Tbl72PlSpecies> PlSpeciesItems { get; } = new();
    public ObservableCollection<Tbl72PlSpecies> PlSpeciesSubItems { get; } = new();
    public ObservableCollection<Tbl78Name> NameItems { get; } = new();
    public ObservableCollection<Tbl81Image> ImageItems { get; } = new();
    public ObservableCollection<Tbl84Synonym> SynonymItems { get; } = new();
    public ObservableCollection<Tbl87Geographic> GeographicItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = new();
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = new();
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = new();
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

    public string FilterText
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; }

    public ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;

    public ObservableCollection<Tbl15Subdivision> SubdivisionsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl18Superclass> SuperclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl21Class> ClassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl24Subclass> SubclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl27Infraclass> InfraclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl30Legio> LegiosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl33Ordo> OrdosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl36Subordo> SubordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl39Infraordo> InfraordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl45Family> FamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl48Subfamily> SubfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl54Supertribus> SupertribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl57Tribus> TribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl60Subtribus> SubtribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl63Infratribus> InfratribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl66Genus> GenussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl68Speciesgroup> SpeciesgroupsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl69FiSpecies> FiSpeciessesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl69FiSpecies> FiSpeciessesSubCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl72PlSpecies> PlSpeciessesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl69FiSpecies> FiSpeciessesFiSpeciesNameCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl72PlSpecies> PlSpeciessesSubCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl72PlSpecies> PlSpeciessesPlSpeciesNameCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl78Name> NamesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl81Image> ImagesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl84Synonym> SynonymsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl87Geographic> GeographicsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> AuthorsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefAuthorsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> SourcesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefSourcesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> ExpertsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefExpertsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl93Comment> CommentsCollection
    {
        get; set;
    } = null!;

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

    private string _searchFiSpeciesName = "";

    public string SearchFiSpeciesName
    {
        get => _searchFiSpeciesName;
        set
        {
            _searchFiSpeciesName = value; OnPropertyChanged();
        }
    }
    private TblDisease _diseaseSelected = null!;
    public TblDisease DiseaseSelected
    {
        get => _diseaseSelected;
        set => SetProperty(ref _diseaseSelected, value);
    }

    private ObservableCollection<Tbl69FiSpecies> _tbl69FiSpeciessesList = null!;

    public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesList
    {
        get => _tbl69FiSpeciessesList;
        set
        {
            _tbl69FiSpeciessesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl69FiSpecies> _tbl69FiSpeciessesAllList = null!;

    public ObservableCollection<Tbl69FiSpecies> Tbl69FiSpeciessesAllList
    {
        get => _tbl69FiSpeciessesAllList;
        set
        {
            _tbl69FiSpeciessesAllList = value;
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
    private ObservableCollection<Tbl15Subdivision> _tbl15SubdivisionsAllList = null!;
    public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsAllList
    {
        get => _tbl15SubdivisionsAllList;
        set
        {
            _tbl15SubdivisionsAllList = value; OnPropertyChanged();
        }
    }

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
    private ObservableCollection<Tbl09Division> _tbl09DivisionsAllList = null!;
    public ObservableCollection<Tbl09Division> Tbl09DivisionsAllList
    {
        get => _tbl09DivisionsAllList;
        set
        {
            _tbl09DivisionsAllList = value; OnPropertyChanged();
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
    private void GetValueContinent()
    {
        _continents = new List<Continent>()
            {
                new() { Name = "Africa" },
                new() { Name = "Antarctica" },
                new() { Name = "Asia" },
                new() { Name = "Australia" },
                new() { Name = "Central/South America" },
                new() { Name = "Europe" },
                new() { Name = "North America/Caribbean" }
            };

        _selectedContinent = new Continent();
    }

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
            OnPropertyChanged(nameof(TblCountriesList));
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
    private void GetValueLanguage()
    {
        _languages = new List<Language>()
            {
                new() { Name = "GER" },
                new() { Name = "ENG" },
                new() { Name = "FRE" },
                new() { Name = "POR" }
            };

        _selectedLanguage = new Language();
    }

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
    private void GetValueMimeType()
    {
        _mimeTypes = new List<MimeType>()
            {
                new() { Name = "jpg" },
                new() { Name = "png" },
                new() { Name = "bmp" },
                new() { Name = "tiff" },
                new() { Name = "gif" },
                new() { Name = "icon" },
                new() { Name = "jpeg" },
                new() { Name = "wmf" },
                new() { Name = "wmv" },
                new() { Name = "mpg" },
                new() { Name = "mp4" },
                new() { Name = "avi" },
                new() { Name = "mov" },
                new() { Name = "swf" },
                new() { Name = "flv" }
            };

        _selectedMimeType = new MimeType();
    }

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
