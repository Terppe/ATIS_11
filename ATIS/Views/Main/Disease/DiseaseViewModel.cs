using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Windows.Storage.Pickers;
using Windows.System;

//    DiseaseViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main.Disease;
public partial class DiseaseViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
    public string Search;
    private int _plantId;
    private int _animalId;

    private readonly FileOpenPicker _filePicker = new();

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };
    #endregion [Private Data Members]              

    //    Part 1    

    #region [Methods Disease]

    public ICommand SearchRunCommand => new WinUICommunity.RelayCommand(execute: delegate { SearchRun(FilterText); });

    public void SearchRun(string filterText)
    {
        DiseasesCollection?.Clear();

        if (string.IsNullOrEmpty(filterText))
        {
            AllDialogs.NoDatasetFoundWarnMessageDialog();
            return;
        }

        IsLoading = true;
        DiseasesCollection = _dataService.GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(filterText);

        DataSetCount = DiseasesCollection.Count;
        RefreshDiseaseItems();
        //   IsLoading = false;
    }

    #region Report Search

    //public void ReportSearchAll(int id, string tab)
    //{
    //    FoodsCollection?.Clear();
    //    GetTblFoodsById(id);

    //}

    #endregion Report Search

    #endregion [Methods Disease ]  

    //    Part 2    

    #region All Properties

    #region [Propertys]
    //------------IsModified--------------------------------------
    public bool IsModifiedDisease { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => DiseaseItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
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
    private int _imageDataSetCount;
    public int ImageDataSetCount
    {
        get => _imageDataSetCount;
        set
        {
            _imageDataSetCount = value; OnPropertyChanged();
        }
    }

    public string FilterText { get; private set; }

    #endregion [Propertys]

    #region Selected Properties

    private TblDisease _diseaseSelected;

    public TblDisease DiseaseSelected
    {
        get => _diseaseSelected;
        set => SetProperty(ref _diseaseSelected, value);
    }

    private Tbl81Image _imageSelected;

    public Tbl81Image ImageSelected
    {
        get => _imageSelected;
        set => SetProperty(ref _imageSelected, value);
    }

    #endregion Selected Properties     

    #region Public Properties  

    private int _diseaseDataSetCount;
    public int DiseaseDataSetCount
    {
        get => _diseaseDataSetCount;
        set
        {
            _diseaseDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchDiseaseName = "";

    public string SearchDiseaseName
    {
        get => _searchDiseaseName;
        set
        {
            _searchDiseaseName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblDisease> _tblDiseasesList;

    public ObservableCollection<TblDisease> TblDiseasesList
    {
        get => _tblDiseasesList;
        set
        {
            _tblDiseasesList = value; OnPropertyChanged();
        }
    }
    private ObservableCollection<Tbl81Image> _tbl81ImagesList;

    public ObservableCollection<Tbl81Image> Tbl81ImagesList
    {
        get => _tbl81ImagesList;
        set
        {
            _tbl81ImagesList = value; OnPropertyChanged();
        }
    }


    #endregion Public Properties

    #region Refresh Methods

    public void RefreshDiseaseItems()
    {
        DiseaseItems.Clear();
        if (TblDiseasesList != null)
        {
            foreach (var item in TblDiseasesList)
            {
                DiseaseItems.Add(item);
            }

            if (TblDiseasesList.Count == 0)
            {
                return;
            }

            if (DiseaseSelected == null && TblDiseasesList.Count != 0)
            {
                DiseaseSelected = DiseaseItems.FirstOrDefault()!;
            }
        }
    }
    public void RefreshImageItems()
    {
        ImageItems.Clear();
        if (Tbl81ImagesList != null)
        {
            foreach (var item in Tbl81ImagesList)
            {
                ImageItems.Add(item);
            }

            if (Tbl81ImagesList.Count == 0)
            {
                return;
            }

            if (ImageSelected == null && Tbl81ImagesList.Count != 0)
            {
                ImageSelected = ImageItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #region Collection

    public ObservableCollection<TblDisease> DiseasesCollection
    {
        get; set;
    } = null!;


    #endregion Collection
    #endregion All Properties

}
