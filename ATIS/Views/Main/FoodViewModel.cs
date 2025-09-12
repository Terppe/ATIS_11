using System.Collections.ObjectModel;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

//    FoodViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main;

public partial class FoodViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<TblFood> FoodItems { get; } = [];
    public string Search;

    private readonly FileOpenPicker _filePicker = new();

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };

    #endregion [Private Data Members]      

    #region [Constructor]
    public FoodViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblFoodsAllList ??= [];
        TblFoodsAllList = [.. _dataService.GetFoodAsync()];
    }
    public FoodViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1    


    #region [Methods Food]

    public async Task AddFoodAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditFood = true;
            IsNewFood = true;

            TblFoodsList ??= [];
            TblFoodsList.Insert(index: 0, item: new TblFood {  });
        });
    }

    public async Task SaveFoodAsync(TblFood viewModelFoodSelected, string viewModelSelectedPath)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewFood)
            {
                IsNewFood = false;
            }
        });

        TblFood result = null;
        try
        {
            if (IsModifiedFood)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertFoodAsync(viewModelFoodSelected, viewModelSelectedPath);
                    IsModifiedFood = false;
                });
            }
            else
            {
                IsModifiedFood = true;
            }
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
                SimpleLog.Log(e);
                return;
            }
        }
        catch (Exception ex)
        {
            await AllDialogs.ErrorMessageDialogAsync(ex.Message);
            SimpleLog.Log(ex);
            return;
        }

        if (result != null)
        {
            IsInEditFood = false;
            IsModifiedFood = false;
            IsExistingFood = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteFoodAsync(TblFood viewModelFoodSelected)
    {
        TblFood result = null;
        try
        {
            var deleteFoodId = viewModelFoodSelected.FoodId;
            await DeleteFood(deleteFoodId);

            result = viewModelFoodSelected;
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException != null)
            {
                await AllDialogs.ErrorMessageDialogAsync(e.InnerException.ToString());
            }

            SimpleLog.Log(e);
        }
        catch (Exception e)
        {
            await AllDialogs.ErrorMessageDialogAsync(e.Message);
            SimpleLog.Log(e);
            throw new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2, e);
        }

        if (result != null)
        {
            IsInEditFood = false;
            IsModifiedFood = false;
            IsExistingFood = true;
            IsDeletingFood = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private static async Task DeleteFood(int foodid)
    {
        var food = await Context.TblFoods.FirstOrDefaultAsync(b => b.FoodId == foodid);
        if (food != null)
        {
            Context.TblFoods.Remove(food);
            await Context.SaveChangesAsync();
        }
    }

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
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingFood;
    public bool IsDeletingFood
    {
        get => _isDeletingFood;
        set => SetProperty(ref _isDeletingFood, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewFood;
    public bool IsNewFood
    {
        get => _isNewFood;
        set => SetProperty(ref _isNewFood, value);
    }

    //----------IsExisting-----------------------------------------
    private bool _isExistingFood;
    public bool IsExistingFood
    {
        get => _isExistingFood;
        set => SetProperty(ref _isExistingFood, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditFood = false;
    public bool IsInEditFood
    {
        get => _isInEditFood;
        set => SetProperty(ref _isInEditFood, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditFood()
    {
        IsInEditFood = true;
        IsModifiedFood = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------                                      
    public async void EndEditFood() => await SaveFoodAsync(FoodSelected, SelectedPath);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshFood => !IsModifiedFood && IsExistingFood;
    //--------------CanRevert-------------------------------------
    public bool CanRevertFood => IsModifiedFood && IsExistingFood;

    #endregion [Propertys]     

    #region Selected Properties

    private TblFood _foodSelected;

    public TblFood FoodSelected
    {
        get => _foodSelected;
        set
        {
            SetProperty(ref _foodSelected, value);
            //ImageSelected loose ImageFileStream
            if (_foodSelected == null || _foodSelected == null && _foodSelected.CountId == 0)
            {
                return;
            }

            _dataSet = _fileStreamList.Find(t => t.ImageCountId == _foodSelected.CountId);

            _foodSelected.Filestream = _dataSet.ImageFileStream;
            _selectedFilestream = _dataSet.ImageFileStream;
        }
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

    //---------------------------------------
    private Data _dataSet;

    public Data DataSet
    {
        get => _dataSet;
        set
        {
            _dataSet = value; OnPropertyChanged();
        }
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

    //---------------------------------------------------

    private ObservableCollection<TblFood> _tblFoodsList;
    public ObservableCollection<TblFood> TblFoodsList
    {
        get => _tblFoodsList;
        set
        {
            _tblFoodsList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblFood> _tblFoodsAllList;
    public ObservableCollection<TblFood> TblFoodsAllList
    {
        get => _tblFoodsAllList;
        set
        {
            _tblFoodsAllList = value; OnPropertyChanged();
        }
    }


    #region Image

    private byte[] _selectedFilestream = null!;
    public byte[] SelectedFilestream
    {
        get => _selectedFilestream;
        set
        {
            _selectedFilestream = value; OnPropertyChanged();
        }
    }

    private byte[] _imageBuffer = null!;
    public byte[] ImageBuffer
    {
        get => _imageBuffer;
        set
        {
            _imageBuffer = value;
            _ = LoadImageAsync();
        }
    }

    private ImageSource _imageSource = null!;
    public ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            _imageSource = value; OnPropertyChanged();
        }
    }

    private async Task LoadImageAsync()
    {
        using var ms = new InMemoryRandomAccessStream();
        // Writes the image byte array in an InMemoryRandomAccessStream
        // that is needed to set the source of BitmapImage.
        using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
        {
            writer.WriteBytes(_imageBuffer);
            await writer.StoreAsync();
        }

        var image = new BitmapImage();
        await image.SetSourceAsync(ms);
        ImageSource = image;
    }

    #endregion



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
