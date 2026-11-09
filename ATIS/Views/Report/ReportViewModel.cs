using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Models;

namespace ATIS.Views.Report;

public class ReportViewModel : ObservableObject
{
    #region [Private Data Members]

    private readonly IDialogService _dialogService;
    private readonly IDataService _dataService;
    //private static int _fishId;
    //private static int _plantId;
    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
    public ObservableCollection<TblFood> FoodItems { get; set; } = [];
    public ObservableCollection<TblMiscellaneous> MiscellaneousItems { get; } = [];

    #endregion [Private Data Members]

    #region [Constructor]

    public ReportViewModel(IDataService dataService, IDialogService dialogService)
    {
        _dataService = dataService;
        _dialogService = dialogService;
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
    private readonly System.Threading.SemaphoreSlim _reportLoadGate = new(1, 1);
    public ICommand SearchRunCommand => new AsyncRelayCommand(() => SearchRunAsync(FilterText));

    public async Task SearchRunAsync(string filterText)
    {
        await _reportLoadGate.WaitAsync();

        try
        {
            ClearAllCollections();
            DataSetCount = 0;

            if (string.IsNullOrWhiteSpace(filterText))
                return;

            IsLoading = true;

            await PrepareReportLookupsAsync();
            await SelectedTabChange(filterText);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Report loading failed: {0}", ex);
            await _dialogService.ShowErrorAsync(
                "Error_LoadingData_Title",
                "Error_LoadingData_Message");
        }
        finally
        {
            IsLoading = false;
            _reportLoadGate.Release();
        }
    }
    //private void SetGenusResults(IEnumerable<Tbl66Genus> results)
    //{
    //    GenussesCollection = new ObservableCollection<Tbl66Genus>(results);
    //    DataSetCount = GenussesCollection.Count;
    //    RefreshGenusItems();
    //}

    private async Task PrepareReportLookupsAsync()
    {
        switch (_selectedMainTabIndex)
        {
            case 0:
                var infratribusses = await _dataService.GetAllInfratribusAsync();
                Tbl63InfratribussesAllList =
                    new ObservableCollection<Tbl63Infratribus>(infratribusses);
                break;

            case 1:
                var genusses = await _dataService.GetAllGenusAsync();
                Tbl66GenussesAllList =
                    new ObservableCollection<Tbl66Genus>(genusses);
                break;

            case 3:
                switch (_selectedTribusTabIndex)
                {
                    case 0:
                        var infratribus = await _dataService.GetAllInfratribusAsync();
                        Tbl63InfratribussesAllList = new ObservableCollection<Tbl63Infratribus>(infratribus);
                        break;
                    case 1:
                        var subtribus = await _dataService.GetAllSubtribusAsync();
                        Tbl60SubtribussesAllList = new ObservableCollection<Tbl60Subtribus>(subtribus);
                        break;
                    case 2:
                        var tribus = await _dataService.GetAllTribusAsync();
                        Tbl57TribussesAllList = new ObservableCollection<Tbl57Tribus>(tribus);
                        break;
                    case 3:
                        var supertribus = await _dataService.GetAllSupertribusAsync();
                        Tbl54SupertribussesAllList = new ObservableCollection<Tbl54Supertribus>(supertribus);
                        break;
                }
                break;

            case 4:
                switch (_selectedFamilyTabIndex)
                {
                    case 0:
                        var infrafamily = await _dataService.GetAllInfrafamilyAsync();
                        Tbl51InfrafamiliesAllList = new ObservableCollection<Tbl51Infrafamily>(infrafamily);
                        break;
                    case 1:
                        var subfamily = await _dataService.GetAllSubfamilyAsync();
                        Tbl48SubfamiliesAllList = new ObservableCollection<Tbl48Subfamily>(subfamily);
                        break;
                    case 2:
                        var family = await _dataService.GetAllFamilyAsync();
                        Tbl45FamiliesAllList = new ObservableCollection<Tbl45Family>(family);
                        break;
                    case 3:
                        var superfamily = await _dataService.GetAllSuperfamilyAsync();
                        Tbl42SuperfamiliesAllList = new ObservableCollection<Tbl42Superfamily>(superfamily);
                        break;
                }
                break;

            case 5:
                switch (_selectedOrdoTabIndex)
                {
                    case 0:
                        var infraordos = await _dataService.GetAllInfraordoAsync();
                        Tbl39InfraordosAllList = new ObservableCollection<Tbl39Infraordo>(infraordos);
                        break;
                    case 1:
                        var subordos = await _dataService.GetAllSubordoAsync();
                        Tbl36SubordosAllList = new ObservableCollection<Tbl36Subordo>(subordos);
                        break;
                    case 2:
                        var ordos = await _dataService.GetAllOrdoAsync();
                        Tbl33OrdosAllList = new ObservableCollection<Tbl33Ordo>(ordos);
                        break;
                    case 3:
                        var legios = await _dataService.GetAllLegioAsync();
                        Tbl30LegiosAllList = new ObservableCollection<Tbl30Legio>(legios);
                        break;
                }
                break;

            case 6:
                switch (_selectedClassTabIndex)
                {
                    case 0:
                        var infraclasses = await _dataService.GetAllInfraclassAsync();
                        Tbl27InfraclassesAllList = new ObservableCollection<Tbl27Infraclass>(infraclasses);
                        break;
                    case 1:
                        var subclasses = await _dataService.GetAllSubclassAsync();
                        Tbl24SubclassesAllList = new ObservableCollection<Tbl24Subclass>(subclasses);
                        break;
                    case 2:
                        var classes = await _dataService.GetAllClassAsync();
                        Tbl21ClassesAllList = new ObservableCollection<Tbl21Class>(classes);
                        break;
                    case 3:
                        var superclasses = await _dataService.GetAllSuperclassAsync();
                        Tbl18SuperclassesAllList = new ObservableCollection<Tbl18Superclass>(superclasses);
                        break;
                }
                break;

            case 7:
                switch (_selectedPhylumTabIndex)
                {
                    case 0:
                        var subphylums = await _dataService.GetAllSubphylumAsync();
                        Tbl12SubphylumsAllList = new ObservableCollection<Tbl12Subphylum>(subphylums);
                        break;
                    case 1:
                        var phylums = await _dataService.GetAllPhylumAsync();
                        Tbl06PhylumsAllList = new ObservableCollection<Tbl06Phylum>(phylums);
                        break;
                }
                break;

            case 8:
                if (_selectedRegnumTabIndex == 0)
                {
                    var regnums = await _dataService.GetAllRegnumAsync();
                    Tbl03RegnumsAllList = new ObservableCollection<Tbl03Regnum>(regnums);
                }
                break;
        }
    }

    private async Task SelectedTabChange(string filterText)
    {
        if (_selectedMainTabIndex == 0) // Genus
        {
            var results = await _dataService.SearchGenusForReportAsync(filterText);
            GenussesCollection = new ObservableCollection<Tbl66Genus>(results);
            DataSetCount = GenussesCollection.Count;

            OnPropertyChanged(nameof(GenussesCollection));
            RefreshGenusItems();
        }
        if (_selectedMainTabIndex == 1)  //Species
        {
            var results = await _dataService.SearchSpeciesForReportAsync(filterText);
            SpeciessesCollection = new ObservableCollection<Tbl75Species>(results);
            DataSetCount = SpeciessesCollection.Count;

            OnPropertyChanged(nameof(SpeciessesCollection));
            RefreshSpeciesItems();

        }
        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 0)
        {
            var results = await _dataService.SearchInfratribusForReportAsync(filterText);
            InfratribussesCollection = new ObservableCollection<Tbl63Infratribus>(results);
            DataSetCount = InfratribussesCollection.Count;

            OnPropertyChanged(nameof(InfratribussesCollection));
            RefreshInfratribusItems();
        }

        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 1)
        {
            var results = await _dataService.SearchSubtribusForReportAsync(filterText);
            SubtribussesCollection = new ObservableCollection<Tbl60Subtribus>(results);
            DataSetCount = SubtribussesCollection.Count;

            OnPropertyChanged(nameof(SubtribussesCollection));
            RefreshSubtribusItems();
        }

        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 2)
        {
            var results = await _dataService.SearchTribusForReportAsync(filterText);
            TribussesCollection = new ObservableCollection<Tbl57Tribus>(results);
            DataSetCount = TribussesCollection.Count;

            OnPropertyChanged(nameof(TribussesCollection));
            RefreshTribusItems();
        }

        if (_selectedMainTabIndex == 3 && _selectedTribusTabIndex == 3)
        {
            var results = await _dataService.SearchSupertribusForReportAsync(filterText);
            SupertribussesCollection = new ObservableCollection<Tbl54Supertribus>(results);
            DataSetCount = SupertribussesCollection.Count;

            OnPropertyChanged(nameof(SupertribussesCollection));
            RefreshSupertribusItems();
        }


        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 0)
        {
            var results = await _dataService.SearchInfrafamilyForReportAsync(filterText);
            InfrafamiliesCollection = new ObservableCollection<Tbl51Infrafamily>(results);
            DataSetCount = InfrafamiliesCollection.Count;

            OnPropertyChanged(nameof(InfrafamiliesCollection));
            RefreshInfrafamilyItems();
        }

        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 1)
        {
            var results = await _dataService.SearchSubfamilyForReportAsync(filterText);
            SubfamiliesCollection = new ObservableCollection<Tbl48Subfamily>(results);
            DataSetCount = SubfamiliesCollection.Count;

            OnPropertyChanged(nameof(SubfamiliesCollection));
            RefreshSubfamilyItems();
        }

        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 2)
        {
            var results = await _dataService.SearchFamilyForReportAsync(filterText);
            FamiliesCollection = new ObservableCollection<Tbl45Family>(results);
            DataSetCount = FamiliesCollection.Count;

            OnPropertyChanged(nameof(FamiliesCollection));
            RefreshFamilyItems();
        }

        if (_selectedMainTabIndex == 4 && _selectedFamilyTabIndex == 3)
        {
            var results = await _dataService.SearchSuperfamilyForReportAsync(filterText);
            SuperfamiliesCollection = new ObservableCollection<Tbl42Superfamily>(results);
            DataSetCount = SuperfamiliesCollection.Count;

            OnPropertyChanged(nameof(SuperfamiliesCollection));
            RefreshSuperfamilyItems();
        }


        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 0)
        {
            var results = await _dataService.SearchInfraordoForReportAsync(filterText);
            InfraordosCollection = new ObservableCollection<Tbl39Infraordo>(results);
            DataSetCount = InfraordosCollection.Count;

            OnPropertyChanged(nameof(InfraordosCollection));
            RefreshInfraordoItems();
        }

        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 1)
        {
            var results = await _dataService.SearchSubordoForReportAsync(filterText);
            SubordosCollection = new ObservableCollection<Tbl36Subordo>(results);
            DataSetCount = SubordosCollection.Count;

            OnPropertyChanged(nameof(SubordosCollection));
            RefreshSubordoItems();
        }

        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 2)
        {
            var results = await _dataService.SearchOrdoForReportAsync(filterText);
            OrdosCollection = new ObservableCollection<Tbl33Ordo>(results);
            DataSetCount = OrdosCollection.Count;

            OnPropertyChanged(nameof(OrdosCollection));
            RefreshOrdoItems();
        }

        if (_selectedMainTabIndex == 5 && _selectedOrdoTabIndex == 3)
        {
            var results = await _dataService.SearchLegioForReportAsync(filterText);
            LegiosCollection = new ObservableCollection<Tbl30Legio>(results);
            DataSetCount = LegiosCollection.Count;

            OnPropertyChanged(nameof(LegiosCollection));
            RefreshLegioItems();
        }


        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 0)
        {
            var results = await _dataService.SearchInfraclassForReportAsync(filterText);
            InfraclassesCollection = new ObservableCollection<Tbl27Infraclass>(results);
            DataSetCount = InfraclassesCollection.Count;

            OnPropertyChanged(nameof(InfraclassesCollection));
            RefreshInfraclassItems();
        }

        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 1)
        {
            var results = await _dataService.SearchSubclassForReportAsync(filterText);
            SubclassesCollection = new ObservableCollection<Tbl24Subclass>(results);
            DataSetCount = SubclassesCollection.Count;

            OnPropertyChanged(nameof(SubclassesCollection));
            RefreshSubclassItems();
        }

        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 2)
        {
            var results = await _dataService.SearchClassForReportAsync(filterText);
            ClassesCollection = new ObservableCollection<Tbl21Class>(results);
            DataSetCount = ClassesCollection.Count;

            OnPropertyChanged(nameof(ClassesCollection));
            RefreshClassItems();
        }

        if (_selectedMainTabIndex == 6 && _selectedClassTabIndex == 3)
        {
            var results = await _dataService.SearchSuperclassForReportAsync(filterText);
            SuperclassesCollection = new ObservableCollection<Tbl18Superclass>(results);
            DataSetCount = SuperclassesCollection.Count;

            OnPropertyChanged(nameof(SuperclassesCollection));
            RefreshSuperclassItems();
        }


        if (_selectedMainTabIndex == 7 && _selectedPhylumTabIndex == 0)
        {
            var results = await _dataService.SearchSubphylumForReportAsync(filterText);
            SubphylumsCollection = new ObservableCollection<Tbl12Subphylum>(results);
            DataSetCount = SubphylumsCollection.Count;

            OnPropertyChanged(nameof(SubphylumsCollection));
            RefreshSubphylumItems();
        }

        if (_selectedMainTabIndex == 7 && _selectedPhylumTabIndex == 1)
        {
            var results = await _dataService.SearchPhylumForReportAsync(filterText);
            PhylumsCollection = new ObservableCollection<Tbl06Phylum>(results);
            DataSetCount = PhylumsCollection.Count;

            OnPropertyChanged(nameof(PhylumsCollection));
            RefreshPhylumItems();
        }


        if (_selectedMainTabIndex == 8 && _selectedRegnumTabIndex == 0)
        {
            var results = await _dataService.SearchRegnumForReportAsync(filterText);
            RegnumsCollection = new ObservableCollection<Tbl03Regnum>(results);
            DataSetCount = RegnumsCollection.Count;

            OnPropertyChanged(nameof(RegnumsCollection));
            RefreshRegnumItems();
        }
    }

    private async Task LoadGenusForTabChangeAsync(string filterText)
    {
        IsLoading = true;

        try
        {
            var results = await _dataService.SearchGenusForReportAsync(filterText);

            if (_selectedMainTabIndex == 0)
            {
                //SetGenusResults(results);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Loading genus tab failed: {0}", ex);
            await _dialogService.ShowErrorAsync("Error_LoadingData_Title", "Error_LoadingData_Message");
        }
        finally
        {
            IsLoading = false;
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

    public int SelectedMainTabIndex
    {
        get => _selectedMainTabIndex;
        set
        {
            if (value == _selectedMainTabIndex)
                return;

            _selectedMainTabIndex = value;
            OnPropertyChanged();

            switch (value)
            {
                case 3:
                    _selectedTribusTabIndex = 2;
                    OnPropertyChanged(nameof(SelectedTribusTabIndex));
                    break;
                case 4:
                    _selectedFamilyTabIndex = 2;
                    OnPropertyChanged(nameof(SelectedFamilyTabIndex));
                    break;
                case 5:
                    _selectedOrdoTabIndex = 2;
                    OnPropertyChanged(nameof(SelectedOrdoTabIndex));
                    break;
                case 6:
                    _selectedClassTabIndex = 2;
                    OnPropertyChanged(nameof(SelectedClassTabIndex));
                    break;
                case 7:
                    _selectedPhylumTabIndex = 1;
                    OnPropertyChanged(nameof(SelectedPhylumTabIndex));
                    break;
                case 8:
                    _selectedRegnumTabIndex = 0;
                    OnPropertyChanged(nameof(SelectedRegnumTabIndex));
                    break;
            }

            _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedTribusTabIndex
    {
        get => _selectedTribusTabIndex;
        set
        {
            if (value == _selectedTribusTabIndex)
                return;

            _selectedTribusTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 3)
                _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedFamilyTabIndex
    {
        get => _selectedFamilyTabIndex;
        set
        {
            if (value == _selectedFamilyTabIndex)
                return;

            _selectedFamilyTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 4)
                _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedOrdoTabIndex
    {
        get => _selectedOrdoTabIndex;
        set
        {
            if (value == _selectedOrdoTabIndex)
                return;

            _selectedOrdoTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 5)
                _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedClassTabIndex
    {
        get => _selectedClassTabIndex;
        set
        {
            if (value == _selectedClassTabIndex)
                return;

            _selectedClassTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 6)
                _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedPhylumTabIndex
    {
        get => _selectedPhylumTabIndex;
        set
        {
            if (value == _selectedPhylumTabIndex)
                return;

            _selectedPhylumTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 7)
                _ = SearchRunAsync(FilterText);
        }
    }

    public int SelectedRegnumTabIndex
    {
        get => _selectedRegnumTabIndex;
        set
        {
            if (value == _selectedRegnumTabIndex)
                return;

            _selectedRegnumTabIndex = value;
            OnPropertyChanged();

            if (_selectedMainTabIndex == 8)
                _ = SearchRunAsync(FilterText);
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

    private int _selectedTribusTabIndex;
    private int _selectedFamilyTabIndex;
    private int _selectedOrdoTabIndex;
    private int _selectedClassTabIndex;
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

    #endregion "Public Commands to change TabItems"

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

    //private Tbl09Division _divisionSelected = null!;
    //public Tbl09Division DivisionSelected
    //{
    //    get => _divisionSelected;
    //    set => SetProperty(ref _divisionSelected, value);
    //}

    private Tbl12Subphylum _subphylumSelected = null!;
    public Tbl12Subphylum SubphylumSelected
    {
        get => _subphylumSelected;
        set => SetProperty(ref _subphylumSelected, value);
    }
    //private Tbl15Subdivision _subdivisionSelected = null!;
    //public Tbl15Subdivision SubdivisionSelected
    //{
    //    get => _subdivisionSelected;
    //    set => SetProperty(ref _subdivisionSelected, value);
    //}

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

    //private Tbl72PlSpecies _plSpeciesSelected = null!;
    //public Tbl72PlSpecies PlSpeciesSelected
    //{
    //    get => _plSpeciesSelected;
    //    set => SetProperty(ref _plSpeciesSelected, value);
    //}

    //private Tbl72PlSpecies _plSpeciesSubSelected = null!;
    //public Tbl72PlSpecies PlSpeciesSubSelected
    //{
    //    get => _plSpeciesSubSelected;
    //    set => SetProperty(ref _plSpeciesSubSelected, value);
    //}

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

    private TblDisease _diseaseSelected = null!;
    public TblDisease DiseaseSelected
    {
        get => _diseaseSelected;
        set => SetProperty(ref _diseaseSelected, value);
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
    //private void RefreshPlSpeciesItems()
    //{
    //    PlSpeciesItems.Clear();
    //    foreach (var item in PlSpeciessesCollection)
    //    {
    //        PlSpeciesItems.Add(item);
    //    }
    //    if (PlSpeciesSelected == null && PlSpeciessesCollection.Count != 0)
    //    {
    //        PlSpeciesSelected = PlSpeciesItems.First();
    //    }
    //}
    //private void RefreshPlSpeciesSubItems()
    //{
    //    PlSpeciesSubItems.Clear();
    //    foreach (var item in PlSpeciessesSubCollection)
    //    {
    //        PlSpeciesSubItems.Add(item);
    //    }
    //    if (PlSpeciesSubSelected == null && PlSpeciessesSubCollection.Count != 0)
    //    {
    //        PlSpeciesSubSelected = PlSpeciesSubItems.First();
    //    }
    //}
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
    public ObservableCollection<Tbl06Phylum> PhylumItems { get; } = new();
    public ObservableCollection<Tbl12Subphylum> SubphylumItems { get; } = new();
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
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = new();
    public ObservableCollection<Tbl75Species> SpeciesSubItems { get; } = new();
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
    //private ObservableCollection<Tbl09Division> _tbl09DivisionsAllList = null!;
    //public ObservableCollection<Tbl09Division> Tbl09DivisionsAllList
    //{
    //    get => _tbl09DivisionsAllList;
    //    set
    //    {
    //        _tbl09DivisionsAllList = value; OnPropertyChanged();
    //    }
    //}

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
    private ObservableCollection<TblDisease> _tblDiseasesList = null!;
    public ObservableCollection<TblDisease> TblDiseasesList
    {
        get => _tblDiseasesList;
        set
        {
            _tblDiseasesList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<TblFood> _tblFoodsList = null!;
    public ObservableCollection<TblFood> TblFoodsList
    {
        get => _tblFoodsList;
        set
        {
            _tblFoodsList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<TblMiscellaneous> _tblMiscellaneoussList = null!;
    public ObservableCollection<TblMiscellaneous> TblMiscellaneoussList
    {
        get => _tblMiscellaneoussList;
        set
        {
            _tblMiscellaneoussList = value; OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    private Language _selectedLanguage = null!;

    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    private MimeType _selectedMimeType = null!;

    public MimeType SelectedMimeType
    {
        get => _selectedMimeType;
        set
        {
            _selectedMimeType = value;
            OnPropertyChanged();
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
            _selectedPath = value; OnPropertyChanged();
        }
    }



    public readonly string DefaultPath = null!;



    #endregion Public Properties

}
