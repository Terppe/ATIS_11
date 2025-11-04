
using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using static ATIS.Views.Report.PdfModels;

//    MiscellaneousViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main;

public partial class TblMiscellaneousViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
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

    #region [Constructor]
    public TblMiscellaneousViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblMiscellaneoussesList ??= [];
        TblMiscellaneoussesList = [.. _dataService.GetMiscellaneousAsync()];
        TblMiscellaneoussesAllList ??= [];
        TblMiscellaneoussesAllList = [.. _dataService.GetMiscellaneousAsync()];
    }
    public TblMiscellaneousViewModel()
    {
    }
    #endregion [Constructor]      


    //    Part 1    


    #region [Methods Miscellaneous]

    public async Task AddMiscellaneousAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditMiscellaneous = true;
            IsNewMiscellaneous = true;

            TblMiscellaneoussesList ??= [];
            TblMiscellaneoussesList.Insert(index: 0, item: new TblMiscellaneous { });
        });
    }

    public async Task SaveMiscellaneousAsync(TblMiscellaneous viewModelMiscellaneousSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewMiscellaneous)
            {
                IsNewMiscellaneous = false;
            }
        });

        TblMiscellaneous result = null;
        try
        {
            if (IsModifiedMiscellaneous)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertMiscellaneousAsync(viewModelMiscellaneousSelected);
                    IsModifiedMiscellaneous = false;
                });
            }
            else
            {
                IsModifiedMiscellaneous = true;
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
            IsInEditMiscellaneous = false;
            IsModifiedMiscellaneous = false;
            IsExistingMiscellaneous = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteMiscellaneousAsync(TblMiscellaneous viewModelMiscellaneousSelected)
    {
        TblMiscellaneous result = null;
        try
        {
            var deleteMiscellaneousId = viewModelMiscellaneousSelected.MiscellaneousId;
            await DeleteMiscellaneous(deleteMiscellaneousId);

            result = viewModelMiscellaneousSelected;
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
            IsInEditMiscellaneous = false;
            IsModifiedMiscellaneous = false;
            IsExistingMiscellaneous = true;
            IsDeletingMiscellaneous = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteMiscellaneous(int miscellaneousid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var miscellaneous = await Context.TblMiscellaneousses.FirstOrDefaultAsync(b => b.MiscellaneousId == miscellaneousid);
            if (miscellaneous != null)
            {
                await RemoveExtras.RemoveImagesByMiscellaneousId(miscellaneousid);

                Context.TblMiscellaneousses.Remove(miscellaneous);
                await Context.SaveChangesAsync();
            }
        });
    }

    #endregion [Methods Miscellaneous ]  

    #region [Methods Image]
    public async Task AddImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            //Wenn auf Tbl75Speciesses umgestellt wird raus _plantId und _animalId
            //    _plantId = 1;  //weil in Tbl72PlSpeciesses nicht mehr gespeichert wird

            //var plantaeRegnum = Context.Tbl72PlSpeciesses.FirstOrDefault(x => x.PlSpeciesName == "Plantae#Regnum#");

            //if (plantaeRegnum != null)
            //{
            //    _plantId = plantaeRegnum.PlSpeciesId;
            //}
            //else
            //{
            //    await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
            //    return;
            //}

            var animalRegnum = Context.Tbl75Speciesses.FirstOrDefault(x => x.SpeciesName == "Animalia#Regnum#");

            if (animalRegnum != null)
            {
                _animalId = animalRegnum.SpeciesId;
            }
            else
            {
                await AllDialogs.NoDatasetFoundInfoMessageDialogAsync();
                return;
            }

            IsInEditImage = true;
            IsNewImage = true;

            var miscellaneousId = MiscellaneousSelected.MiscellaneousId;

            Tbl81ImagesList ??= [];
            //oder     Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { SpeciesId = 2, MiscellaneousId = miscellaneousId});
            Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { SpeciesId = _animalId, MiscellaneousId = miscellaneousId });
        });
    }

    public async Task SaveImageAsync(Tbl81Image viewModelImageSelected, string viewModelSelectedPath)
    {
        if (IsNewImage)
        {
            IsNewImage = false;
        }

        Tbl81Image result = null;
        try
        {
            if (IsModifiedImage)
            {
                result = await _dataService.UpsertImageAsync(viewModelImageSelected, viewModelSelectedPath);
                IsModifiedImage = false;
            }
            else
            {
                IsModifiedImage = true;
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
            IsInEditImage = false;
            IsModifiedImage = false;
            IsExistingImage = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteImageAsync(Tbl81Image viewModelImageSelected)
    {
        Tbl81Image result = null;
        try
        {
            var deleteImageId = viewModelImageSelected.ImageId;
            await DeleteImage(deleteImageId);

            result = viewModelImageSelected;
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
            IsInEditImage = false;
            IsModifiedImage = false;
            IsExistingImage = true;
            IsDeletingImage = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    public static async Task DeleteImage(int imageid)
    {
        var image = await Context.Tbl81Images.FirstOrDefaultAsync(co => co.ImageId == imageid);

        Context.Tbl81Images.Remove(image);
        await Context.SaveChangesAsync();
    }

    private static byte[] LoadImageData(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new BinaryReader(fs);
        var imageBytes = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return imageBytes;
    }

    #endregion [Methods Image]    



    //    Part 2    


    #region Image

    private byte[] _selectedFilestream = null!;
    public byte[] SelectedFilestream
    {
        get => _selectedFilestream;
        set
        {
            _selectedFilestream = value; OnPropertyChanged(nameof(SelectedFilestream));
        }
    }

    //   public new event PropertyChangedEventHandler PropertyChanged;

    private byte[] _imageBuffer = null!;
    public byte[] ImageBuffer
    {
        get => _imageBuffer;
        set
        {
            _imageBuffer = value;
            using var loadImageTask = LoadImageAsync();
        }
    }

    private ImageSource _imageSource = null!;
    public ImageSource ImageSource
    {
        get => _imageSource;
        set
        {
            _imageSource = value; OnPropertyChanged(nameof(ImageSource));
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
    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedMiscellaneous { get; set; }
    public bool IsModifiedImage { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => MiscellaneousItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingMiscellaneous;
    public bool IsDeletingMiscellaneous
    {
        get => _isDeletingMiscellaneous;
        set => SetProperty(ref _isDeletingMiscellaneous, value);
    }
    private bool _isDeletingImage;
    public bool IsDeletingImage
    {
        get => _isDeletingImage;
        set => SetProperty(ref _isDeletingImage, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewMiscellaneous;
    public bool IsNewMiscellaneous
    {
        get => _isNewMiscellaneous;
        set => SetProperty(ref _isNewMiscellaneous, value);
    }
    private bool _isNewImage;
    public bool IsNewImage
    {
        get => _isNewImage;
        set => SetProperty(ref _isNewImage, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingMiscellaneous;
    public bool IsExistingMiscellaneous
    {
        get => _isExistingMiscellaneous;
        set => SetProperty(ref _isExistingMiscellaneous, value);
    }
    private bool _isExistingImage;
    public bool IsExistingImage
    {
        get => _isExistingImage;
        set => SetProperty(ref _isExistingImage, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditMiscellaneous = false;
    public bool IsInEditMiscellaneous
    {
        get => _isInEditMiscellaneous;
        set => SetProperty(ref _isInEditMiscellaneous, value);
    }
    private bool _isInEditImage = false;
    public bool IsInEditImage
    {
        get => _isInEditImage;
        set => SetProperty(ref _isInEditImage, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditMiscellaneous()
    {
        IsInEditMiscellaneous = true;
        IsModifiedMiscellaneous = false;
    }
    public void StartEditImage()
    {
        IsInEditImage = true;
        IsModifiedImage = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditMiscellaneous() => await SaveMiscellaneousAsync(MiscellaneousSelected);
    public async void EndEditImage() => await SaveImageAsync(ImageSelected, SelectedPath);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshMiscellaneous => !IsModifiedMiscellaneous && IsExistingMiscellaneous;
    public bool CanRefreshImage => !IsModifiedImage && IsExistingImage;
    //--------------CanRevert-------------------------------------
    public bool CanRevertMiscellaneous => IsModifiedMiscellaneous && IsExistingMiscellaneous;
    public bool CanRevertImage => IsModifiedImage && IsExistingImage;

    #endregion [Propertys]           

    #region Selected Properties

    private TblMiscellaneous _miscellaneousSelected;
    public TblMiscellaneous MiscellaneousSelected
    {
        get => _miscellaneousSelected;
        set => SetProperty(ref _miscellaneousSelected, value);
    }

    private Tbl81Image _imageSelected;

    public Tbl81Image ImageSelected
    {
        get => _imageSelected;
        set
        {
            SetProperty(ref _imageSelected, value);
            //ImageSelected loose ImageFileStream
            if (_imageSelected == null || _imageSelected == null && _imageSelected.CountId == 0)
            {
                return;
            }

            _dataSet = _fileStreamList.Find(t => t.ImageCountId == _imageSelected.CountId);

            _imageSelected.Filestream = _dataSet.ImageFileStream;
            _selectedFilestream = _dataSet.ImageFileStream;
        }
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
    private int _imageDataSetCount;
    public int ImageDataSetCount
    {
        get => _imageDataSetCount;
        set
        {
            _imageDataSetCount = value; OnPropertyChanged();
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

    private ObservableCollection<TblMiscellaneous> _tblMiscellaneoussesList;
    public ObservableCollection<TblMiscellaneous> TblMiscellaneoussesList
    {
        get => _tblMiscellaneoussesList;
        set
        {
            _tblMiscellaneoussesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblMiscellaneous> _tblMiscellaneoussesAllList;
    public ObservableCollection<TblMiscellaneous> TblMiscellaneoussesAllList
    {
        get => _tblMiscellaneoussesAllList;
        set
        {
            _tblMiscellaneoussesAllList = value; OnPropertyChanged();
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

    #endregion All Properties     



}
