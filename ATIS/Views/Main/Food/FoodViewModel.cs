using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Windows.System;

//    FoodViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main;

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

    #endregion [Methods Food ]  

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

    #endregion All Properties

}
