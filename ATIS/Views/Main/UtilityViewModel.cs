using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;

namespace ATIS.Views.Main;
public partial class UtilityViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<Tbl78Name> NameItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
    public ObservableCollection<Tbl84Synonym> SynonymItems { get; } = [];
    public ObservableCollection<Tbl87Geographic> GeographicItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceExpertItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceSourceItems { get; } = [];
    public ObservableCollection<Tbl90Reference> ReferenceAuthorItems { get; } = [];

    #endregion [Private Data Members]


    #region Public Commands to change TabItems
    private int _selectedUtilitiesTabIndex;

    public int SelectedUtilitiesTabIndex
    {
        get => _selectedUtilitiesTabIndex;
        set
        {
            if (value == _selectedUtilitiesTabIndex)
            {
                return;
            }

            _selectedUtilitiesTabIndex = value; OnPropertyChanged();

            if (_selectedUtilitiesTabIndex == 0)
            {
                //GenussesCollection = _dataService.GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(FilterText);
                //DataSetCount = GenussesCollection.Count;

                //OnPropertyChanged(nameof(GenussesCollection));
                //RefreshGenusItems();
            }

        }
    }

    #endregion Public Commands to change TabItems

    #region All Properties

    #region [Propertys]

    public bool IsModified { get; set; }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isDeleting;
    public bool IsDeleting
    {
        get => _isDeleting;
        set => SetProperty(ref _isDeleting, value);
    }


    private bool _isInEdit = false;
    public bool IsInEdit
    {
        get => _isInEdit;
        set => SetProperty(ref _isInEdit, value);
    }
    public void StartEdit()
    {
        IsInEdit = true;
        IsModified = false;
    }
    public static void BeginEdit()
    {
        // Not used.
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

    private bool _isNewReferenceExpert;
    public bool IsNewReferenceExpert
    {
        get => _isNewReferenceExpert;
        set => SetProperty(ref _isNewReferenceExpert, value);
    }

    private bool _isNewReferenceSource;
    public bool IsNewReferenceSource
    {
        get => _isNewReferenceSource;
        set => SetProperty(ref _isNewReferenceSource, value);
    }

    private bool _isNewReferenceAuthor;
    public bool IsNewReferenceAuthor
    {
        get => _isNewReferenceAuthor;
        set => SetProperty(ref _isNewReferenceAuthor, value);
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

    private Tbl09Division _divisionSelected = null!;
    public Tbl09Division DivisionSelected
    {
        get => _divisionSelected;
        set => SetProperty(ref _divisionSelected, value);
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

    private ObservableCollection<Tbl09Division> _tbl09DivisionsList;
    public ObservableCollection<Tbl09Division> Tbl09DivisionsList
    {
        get => _tbl09DivisionsList;
        set
        {
            _tbl09DivisionsList = value; OnPropertyChanged(nameof(Tbl09DivisionsList));
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


    //private void RefreshReferenceExpertItems()
    //{
    //    ReferenceExpertItems.Clear();
    //    if (Tbl90ReferenceExpertsList != null)
    //    {
    //        foreach (var item in Tbl90ReferenceExpertsList)
    //        {
    //            ReferenceExpertItems.Add(item);
    //        }

    //        if (Tbl90ReferenceExpertsList.Count == 0) return;
    //        if (ReferenceExpertSelected == null && Tbl90ReferenceExpertsList.Count != 0)
    //        {
    //            ReferenceExpertSelected = ReferenceExpertItems.FirstOrDefault()!;
    //        }
    //    }
    //}
    //private void RefreshReferenceSourceItems()
    //{
    //    ReferenceSourceItems.Clear();
    //    if (Tbl90ReferenceSourcesList != null)
    //    {
    //        foreach (var item in Tbl90ReferenceSourcesList)
    //        {
    //            ReferenceSourceItems.Add(item);
    //        }

    //        if (Tbl90ReferenceSourcesList.Count == 0) return;
    //        if (ReferenceSourceSelected == null && Tbl90ReferenceSourcesList.Count != 0)
    //        {
    //            ReferenceSourceSelected = ReferenceSourceItems.FirstOrDefault()!;
    //        }
    //    }
    //}
    //private void RefreshReferenceAuthorItems()
    //{
    //    ReferenceAuthorItems.Clear();
    //    if (Tbl90ReferenceAuthorsList != null)
    //    {
    //        foreach (var item in Tbl90ReferenceAuthorsList)
    //        {
    //            ReferenceAuthorItems.Add(item);
    //        }

    //        if (Tbl90ReferenceAuthorsList.Count == 0) return;
    //        if (ReferenceAuthorSelected == null && Tbl90ReferenceAuthorsList.Count != 0)
    //        {
    //            ReferenceAuthorSelected = ReferenceAuthorItems.FirstOrDefault()!;
    //        }
    //    }
    //}

    #endregion

    #endregion

}
