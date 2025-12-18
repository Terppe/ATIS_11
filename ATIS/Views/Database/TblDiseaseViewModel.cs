using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;

//    TblDiseaseViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Database;

public partial class TblDiseaseViewModel : ObservableObject
{

    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
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

    #region [Constructor]
    public TblDiseaseViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        TblDiseasesList ??= [];
        TblDiseasesList = [.. _dataService.GetDiseaseAsync()];
        TblDiseasesAllList ??= [];
        TblDiseasesAllList = [.. _dataService.GetDiseaseAsync()];
    }

    public TblDiseaseViewModel()
    {
    }

    #endregion [Constructor]

    //    Part 1    


    #region [Methods Disease]

    public async Task AddDiseaseAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditDisease = true;
            IsNewDisease = true;

            TblDiseasesList ??= [];
            TblDiseasesList.Insert(index: 0, item: new TblDisease());
        });
    }

    public async Task SaveDiseaseAsync(TblDisease viewModelDiseaseSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewDisease)
            {
                IsNewDisease = false;
            }
        });

        TblDisease result = null;
        try
        {
            if (IsModifiedDisease)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertDiseaseAsync(viewModelDiseaseSelected);
                    IsModifiedDisease = false;
                });
            }
            else
            {
                IsModifiedDisease = true;
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
            IsInEditDisease = false;
            IsModifiedDisease = false;
            IsExistingDisease = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    public async Task DeleteDiseaseAsync(TblDisease viewModelDiseaseSelected)
    {
        TblDisease result = null;
        try
        {
            var deleteDiseaseId = viewModelDiseaseSelected.DiseaseId;
            await DeleteDisease(deleteDiseaseId);

            result = viewModelDiseaseSelected;
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
            IsInEditDisease = false;
            IsModifiedDisease = false;
            IsExistingDisease = true;
            IsDeletingDisease = true;
        }
        else
        {
            _ = new DeletionException(StringsRes.UnableToDelete1 + StringsRes.UnableToDelete2);
        }
    }

    private async Task DeleteDisease(int diseaseid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {

            var disease = await Context.TblDiseases.FirstOrDefaultAsync(b => b.DiseaseId == diseaseid);
        if (disease != null)
        {
            await RemoveExtras.RemoveImagesByDiseaseId(diseaseid);

            Context.TblDiseases.Remove(disease);
            await Context.SaveChangesAsync();
        }
        });
    }

    #endregion [Methods Disease ]  

    #region [Methods Image]
    public async Task AddImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditImage = true;
            IsNewImage = true;

            var diseaseId = DiseaseSelected.DiseaseId;

            Tbl81ImagesList ??= [];
            Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { DiseaseId = diseaseId });  //Fehlt FilestreamID ???
            return Task.CompletedTask;
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
    public bool IsModifiedDisease { get; set; }
    public bool IsModifiedImage { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => DiseaseItems != null;
    public bool IsNotLoaded => !IsLoaded;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingDisease;
    public bool IsDeletingDisease
    {
        get => _isDeletingDisease;
        set => SetProperty(ref _isDeletingDisease, value);
    }
    private bool _isDeletingImage;
    public bool IsDeletingImage
    {
        get => _isDeletingImage;
        set => SetProperty(ref _isDeletingImage, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewDisease;
    public bool IsNewDisease
    {
        get => _isNewDisease;
        set => SetProperty(ref _isNewDisease, value);
    }

    private bool _isNewImage;
    public bool IsNewImage
    {
        get => _isNewImage;
        set => SetProperty(ref _isNewImage, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingDisease;
    public bool IsExistingDisease
    {
        get => _isExistingDisease;
        set => SetProperty(ref _isExistingDisease, value);
    }
    private bool _isExistingImage;
    public bool IsExistingImage
    {
        get => _isExistingImage;
        set => SetProperty(ref _isExistingImage, value);
    }

    //-----------IsInEdit----------------------------------------                      
    private bool _isInEditDisease = false;
    public bool IsInEditDisease
    {
        get => _isInEditDisease;
        set => SetProperty(ref _isInEditDisease, value);
    }
    private bool _isInEditImage = false;
    public bool IsInEditImage
    {
        get => _isInEditImage;
        set => SetProperty(ref _isInEditImage, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditDisease()
    {
        IsInEditDisease = true;
        IsModifiedDisease = false;
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
    public async void EndEditDisease() => await SaveDiseaseAsync(DiseaseSelected);
    public async void EndEditImage() => await SaveImageAsync(ImageSelected, SelectedPath);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshDisease => !IsModifiedDisease && IsExistingDisease;
    public bool CanRefreshImage => !IsModifiedImage && IsExistingImage;
    //--------------CanRevert-------------------------------------
    public bool CanRevertDisease => IsModifiedDisease && IsExistingDisease;
    public bool CanRevertImage => IsModifiedImage && IsExistingImage;

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

    private int _diseaseDataSetCount;

    public int DiseaseDataSetCount
    {
        get => _diseaseDataSetCount;
        set
        {
            _diseaseDataSetCount = value; OnPropertyChanged();
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

    //---------------------------------------------------

    private ObservableCollection<TblDisease> _tblDiseasesList;
    public ObservableCollection<TblDisease> TblDiseasesList
    {
        get => _tblDiseasesList;
        set
        {
            _tblDiseasesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<TblDisease> _tblDiseasesAllList;
    public ObservableCollection<TblDisease> TblDiseasesAllList
    {
        get => _tblDiseasesAllList;
        set
        {
            _tblDiseasesAllList = value; OnPropertyChanged();
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

    #endregion All Properties 



}
