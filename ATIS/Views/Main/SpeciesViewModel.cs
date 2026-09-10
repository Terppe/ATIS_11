using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Main;
public class SpeciesViewModel : ObservableObject
{
    #region [Private Data Members]
  //  private readonly IDataService _dataService;
    private readonly AtisDbContext _context = new();
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

    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];
    public ObservableCollection<Tbl93Comment> CommentItems { get; } = [];
    private readonly AllDialogs _allDialogs = new();


    #endregion [Private Data Members]

    #region [Constructor]

    public SpeciesViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookup,
        IUserService user)
       // : base(new DataService(factory, user), dialog, coordinator)
    {
   //     _dataService = dataService;
    }

    #endregion [Constructor]

    #region Public Commands to change TabItems
    private int _selectedFishesTabIndex;

    public int SelectedFishesTabIndex
    {
        get => _selectedFishesTabIndex;
        set
        {
            if (value == _selectedFishesTabIndex)
            {
                return;
            }

            _selectedFishesTabIndex = value; OnPropertyChanged();

            if (_selectedFishesTabIndex == 0)
            {
                //GenussesCollection = _dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(FilterText);
                //DataSetCount = GenussesCollection.Count;

                //OnPropertyChanged(nameof(GenussesCollection));
                //RefreshGenusItems();
            }

            if (_selectedFishesTabIndex == 1)
            {
                //FiSpeciessesCollection = _dataService.GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(FilterText);
                //DataSetCount = FiSpeciessesCollection.Count;

                //OnPropertyChanged(nameof(FiSpeciessesCollection));
                //RefreshFiSpeciesItems();
            }

            if (_selectedFishesTabIndex == 2)
            {
                //PlSpeciessesCollection = _dataService.GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(FilterText);
                //DataSetCount = PlSpeciessesCollection.Count;

                //OnPropertyChanged(nameof(PlSpeciessesCollection));
                //RefreshPlSpeciesItems();
            }

            if (_selectedFishesTabIndex == 3)  //Tribus
            {
                //TribussesCollection = _dataService.GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(FilterText);
                //DataSetCount = TribussesCollection.Count;

                //OnPropertyChanged(nameof(TribussesCollection));
                //RefreshTribusItems();
                //SelectedTribusTabIndex = 2;
            }
            if (_selectedFishesTabIndex == 4)  //Family
            {
                //FamiliesCollection = _dataService.GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(FilterText);
                //DataSetCount = FamiliesCollection.Count;

                //OnPropertyChanged(nameof(FamiliesCollection));
                //RefreshFamilyItems();
                //SelectedFamilyTabIndex = 2;
            }
            if (_selectedFishesTabIndex == 5)  //Ordo
            {
                //OrdosCollection = _dataService.GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(FilterText);
                //DataSetCount = OrdosCollection.Count;

                //OnPropertyChanged(nameof(OrdosCollection));
                //RefreshOrdoItems();
                //SelectedOrdoTabIndex = 2;
            }
            if (_selectedFishesTabIndex == 6)  //Class
            {
                //ClassesCollection = _dataService.GetTbl21ClassesCollectionOrderByClassNameFromFilterText(FilterText);
                //DataSetCount = ClassesCollection.Count;

                //OnPropertyChanged(nameof(ClassesCollection));
                //RefreshClassItems();
                //SelectedClassTabIndex = 2;
            }
            if (_selectedFishesTabIndex == 7)  //Division
            {
                //DivisionsCollection = _dataService.GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(FilterText);
                //DataSetCount = DivisionsCollection.Count;

                //OnPropertyChanged(nameof(DivisionsCollection));
                //RefreshDivisionItems();

                //SelectedDivisionTabIndex = 1;
            }
            if (_selectedFishesTabIndex == 19)  //Subphylum
            {
                //PhylumsCollection = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
                //DataSetCount = PhylumsCollection.Count;

                //OnPropertyChanged(nameof(PhylumsCollection));
                //RefreshPhylumItems();
                //SelectedPhylumTabIndex = 1;
            }
            if (_selectedFishesTabIndex == 20)  //Phylum
            {
                Tbl03RegnumsAllList ??= new ObservableCollection<Tbl03Regnum>();
                //   Tbl03RegnumsAllList = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnum();
             //   Tbl03RegnumsAllList = _dataService.GetRegnumAsync();

                //        IsLoading = true;
                //        PhylumStartModify();
                //      OnPropertyChanged(nameof(Tbl03RegnumsAllList));
                //Tbl03RegnumsAllList ??= new ObservableCollection<Tbl03Regnum>();
                //Tbl03RegnumsAllList = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnum();
                //OnPropertyChanged(nameof(Tbl03RegnumsAllList));
                //          Tbl06PhylumsList = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromPhylumId(SubphylumSelected.PhylumId);

                //         PhylumDataSetCount = Tbl06PhylumsList.Count;
                //      RefreshPhylumItems();
                //        IsLoading = false;

                //PhylumsCollection = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(FilterText);
                //DataSetCount = PhylumsCollection.Count;

                OnPropertyChanged(nameof(Tbl03RegnumsAllList));
                //OnPropertyChanged(nameof(PhylumsCollection));
                //RefreshPhylumItems();
                //SelectedPhylumTabIndex = 1;
            }
            if (_selectedFishesTabIndex == 21)  //Regnum
            {
                if (RegnumSelected != null)
                {

                    //                RegnumStartModify();
                    //         Tbl03RegnumsList = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromRegnumId(PhylumSelected.RegnumId);

                    //       RegnumDataSetCount = Tbl03RegnumsList.Count;
                    //          RefreshRegnumItems();

                    //                  IsLoading = true;
                    ////                  PhylumStartModify();
                    //                  Tbl03RegnumsAllList ??= new ObservableCollection<Tbl03Regnum>();
                    //                  Tbl03RegnumsAllList = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnum();

                    //                  Tbl06PhylumsList ??= new ObservableCollection<Tbl06Phylum>();
                    //                  Tbl06PhylumsList = _dataService.GetTbl06PhylumsCollectionOrderByPhylumNameFromRegnumId(RegnumSelected.RegnumId);
                    //                  PhylumDataSetCount = Tbl06PhylumsList.Count;
                    //                  RefreshPhylumItems();
                    //                  IsLoading = false;
                }
                //RegnumsCollection = _dataService.GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(FilterText);
                //DataSetCount = RegnumsCollection.Count;

                OnPropertyChanged(nameof(Tbl03RegnumsList));
                //RefreshRegnumItems();

                //SelectedRegnumTabIndex = 0;
            }
        }
    }

    #endregion Public Commands to change TabItems

    #region [Propertys]

    public bool IsModified { get; set; }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private int _regnumDataSetCount;
    public int RegnumDataSetCount
    {
        get => _regnumDataSetCount;
        set
        {
            _regnumDataSetCount = value; OnPropertyChanged();
        }
    }

    private int _phylumDataSetCount;
    public int PhylumDataSetCount
    {
        get => _phylumDataSetCount;
        set
        {
            _phylumDataSetCount = value; OnPropertyChanged();
        }
    }

    private int _divisionDataSetCount;
    public int DivisionDataSetCount
    {
        get => _divisionDataSetCount;
        set
        {
            _divisionDataSetCount = value; OnPropertyChanged();
        }
    }


    #endregion [Propertys]     

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



    #endregion

    #region Public Properties
    public string SearchRegnumName { get; set; } = null!;


    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsList = null!;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsList
    {
        get => _tbl03RegnumsList;
        set
        {
            _tbl03RegnumsList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl03Regnum> _tbl03RegnumsAllList = null!;
    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get => _tbl03RegnumsAllList;
        set
        {
            _tbl03RegnumsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl06Phylum> _tbl06PhylumsList = null!;
    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsList
    {
        get => _tbl06PhylumsList;
        set
        {
            _tbl06PhylumsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefAuthor> _tbl90AuthorsAllList = null!;
    public ObservableCollection<Tbl90RefAuthor> Tbl90AuthorsAllList
    {
        get => _tbl90AuthorsAllList;
        set
        {
            _tbl90AuthorsAllList = value; OnPropertyChanged();
        }
    }


    private ObservableCollection<Tbl90RefSource> _tbl90SourcesAllList = null!;
    public ObservableCollection<Tbl90RefSource> Tbl90SourcesAllList
    {
        get => _tbl90SourcesAllList;
        set
        {
            _tbl90SourcesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90RefExpert> _tbl90ExpertsAllList = null!;
    public ObservableCollection<Tbl90RefExpert> Tbl90ExpertsAllList
    {
        get => _tbl90ExpertsAllList;
        set
        {
            _tbl90ExpertsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceAuthorsList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceAuthorsList
    {
        get => _tbl90ReferenceAuthorsList;
        set
        {
            _tbl90ReferenceAuthorsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceSourcesList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceSourcesList
    {
        get => _tbl90ReferenceSourcesList;
        set
        {
            _tbl90ReferenceSourcesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl90Reference> _tbl90ReferenceExpertsList = null!;
    public ObservableCollection<Tbl90Reference> Tbl90ReferenceExpertsList
    {
        get => _tbl90ReferenceExpertsList;
        set
        {
            _tbl90ReferenceExpertsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl93Comment> _tbl93CommentsList = null!;
    public ObservableCollection<Tbl93Comment> Tbl93CommentsList
    {
        get => _tbl93CommentsList;
        set
        {
            _tbl93CommentsList = value; OnPropertyChanged();
        }
    }


    #endregion "Public Properties"            

    #region Refresh Methods

    private void RefreshRegnumItems()
    {
        RegnumItems.Clear();
        if (Tbl03RegnumsList != null)
        {
            foreach (var item in Tbl03RegnumsList)
            {
                RegnumItems.Add(item);
            }

            if (Tbl03RegnumsList.Count == 0) return;
            if (RegnumSelected == null && Tbl03RegnumsList.Count != 0)
            {
                RegnumSelected = RegnumItems.FirstOrDefault()!;
            }
        }
    }

    private void RefreshPhylumItems()
    {
        PhylumItems.Clear();
        if (Tbl06PhylumsList != null)
        {
            foreach (var item in Tbl06PhylumsList)
            {
                PhylumItems.Add(item);
            }

            if (Tbl06PhylumsList.Count == 0) return;
            if (PhylumSelected == null && Tbl06PhylumsList.Count != 0)
            {
                PhylumSelected = PhylumItems.FirstOrDefault()!;
            }
        }
    }



    #endregion

}
