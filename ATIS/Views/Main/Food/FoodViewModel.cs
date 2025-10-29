using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;

//    FoodViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main.Food;

public partial class FoodViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<TblFood> FoodItems { get; } = [];
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
        FoodsCollection?.Clear();

        if (string.IsNullOrEmpty(filterText))
        {
            AllDialogs.NoDatasetFoundWarnMessageDialog();
            return;
        }

        IsLoading = true;
        FoodsCollection = _dataService.GetTblFoodsCollectionOrderByFoodNameFromFilterText(filterText);

        DataSetCount = FoodsCollection.Count;
        RefreshFoodItems();
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
    public bool IsModifiedFood { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => FoodItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    #endregion [Propertys]

    #region Selected Properties

    private TblFood _foodSelected;

    public TblFood FoodSelected
    {
        get => _foodSelected;
        set => SetProperty(ref _foodSelected, value);
    }


    #endregion Selected Properties     

    #region Public Properties  

    private int _foodDataSetCount;
    public int FoodDataSetCount
    {
        get => _foodDataSetCount;
        set
        {
            _foodDataSetCount = value; OnPropertyChanged();
        }
    }

    private string _searchFoodName = "";

    public string SearchFoodName
    {
        get => _searchFoodName;
        set
        {
            _searchFoodName = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblFood> _tblFoodsList;

    public ObservableCollection<TblFood> TblFoodsList
    {
        get => _tblFoodsList;
        set
        {
            _tblFoodsList = value; OnPropertyChanged();
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

    public void RefreshFoodItems()
    {
        FoodItems.Clear();
        if (TblFoodsList != null)
        {
            foreach (var item in TblFoodsList)
            {
                FoodItems.Add(item);
            }

            if (TblFoodsList.Count == 0)
            {
                return;
            }

            if (FoodSelected == null && TblFoodsList.Count != 0)
            {
                FoodSelected = FoodItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

    #region Collection

    public ObservableCollection<TblFood> FoodsCollection
    {
        get; set;
    } = null!;


    #endregion Collection

    #endregion All Properties

}
