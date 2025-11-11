using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Windows.Storage.Pickers;
using Windows.System;

//    MiscellaneousViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main.Miscellaneous;
public partial class MiscellaneousViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<TblMiscellaneous> MiscellaneousItems { get; } = [];
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

    #region [Methods Food]

    public ICommand SearchRunCommand => new WinUICommunity.RelayCommand(execute: delegate { SearchRun(FilterText); });

    public void SearchRun(string filterText)
    {
        MiscellaneoussesCollection?.Clear();

        if (string.IsNullOrEmpty(filterText))
        {
            AllDialogs.NoDatasetFoundWarnMessageDialog();
            return;
        }

        IsLoading = true;
        MiscellaneoussesCollection = _dataService.GetTblMiscellaneoussesCollectionOrderByMiscellaneousNameFromFilterText(filterText);

        DataSetCount = MiscellaneoussesCollection.Count;
        RefreshMiscellaneousItems();
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
    public bool IsModifiedMiscellaneous { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => MiscellaneousItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    #endregion [Propertys]

    #region Selected Properties

    private TblMiscellaneous _miscellaneousSelected;

    public TblMiscellaneous MiscellaneousSelected
    {
        get => _miscellaneousSelected;
        set => SetProperty(ref _miscellaneousSelected, value);
    }


    #endregion Selected Properties     

    #region Public Properties  

    private int _miscellaneousDataSetCount;
    public int MiscellaneousDataSetCount
    {
        get => _miscellaneousDataSetCount;
        set
        {
            _miscellaneousDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchMiscellaneousName = "";

    public string SearchFoodName
    {
        get => _searchMiscellaneousName;
        set
        {
            _searchMiscellaneousName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblMiscellaneous> _tblMiscellaneoussesList;

    public ObservableCollection<TblMiscellaneous> TblMiscellaneoussesList
    {
        get => _tblMiscellaneoussesList;
        set
        {
            _tblMiscellaneoussesList = value; OnPropertyChanged();
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

    #endregion Public Properties

    #region Refresh Methods

    public void RefreshMiscellaneousItems()
    {
        MiscellaneousItems.Clear();
        if (TblMiscellaneoussesList != null)
        {
            foreach (var item in TblMiscellaneoussesList)
            {
                MiscellaneousItems.Add(item);
            }

            if (TblMiscellaneoussesList.Count == 0)
            {
                return;
            }

            if (MiscellaneousSelected == null && TblMiscellaneoussesList.Count != 0)
            {
                MiscellaneousSelected = MiscellaneousItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #region Collection

    public ObservableCollection<TblMiscellaneous> MiscellaneoussesCollection
    {
        get; set;
    } = null!;


    #endregion Collection

    #endregion All Properties

}


