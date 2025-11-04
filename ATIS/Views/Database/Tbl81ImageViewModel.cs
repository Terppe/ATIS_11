using System.Collections.ObjectModel;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.EntityFrameworkCore;
using Windows.System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using WinRT.Interop;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;


//    Tbl81ImageViewModel Skriptdatum:  25.11.2024  10:32    

namespace ATIS.Views.Database;

public partial class Tbl81ImageViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
    public ObservableCollection<Tbl66Genus> GenusItems { get; } = [];
    public ObservableCollection<Tbl75Species> SpeciesItems { get; } = [];

    private readonly FileOpenPicker _filePicker = new();

    private readonly List<Data> _fileStreamList = [];
    public struct Data
    {
        public int ImageCountId;
        public byte[] ImageFileStream;
    };

    #endregion [Private Data Members]           

    #region [Constructor]
    public Tbl81ImageViewModel(IDataService dataService)
    {
        _dataService = dataService;
        GetAllCollections();
    }

    private void GetAllCollections()
    {
        Tbl66GenussesAllList ??= [];
        Tbl66GenussesAllList = [.. _dataService.GetGenusAsync()];
        Tbl68SpeciesgroupsAllList ??= [];
        Tbl68SpeciesgroupsAllList = [.. _dataService.GetSpeciesgroupAsync()];
        Tbl75SpeciessesAllList ??= [];
        Tbl75SpeciessesAllList = [.. _dataService.GetSpeciesAsync()];
    }
    public Tbl81ImageViewModel()
    {

    }

    #endregion [Constructor]  


    //    Part 1

    #region [Methods Image]

    public async Task AddImageAsync()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            IsInEditImage = true;
            IsNewImage = true;

            //var plSpeciesId = 0;

            //var plantaeRegnum = Context.Tbl72PlSpeciesses.FirstOrDefault(x => x.PlSpeciesName == "Plantae#Regnum#");

            //if (plantaeRegnum != null)
            //{
            //    plSpeciesId = plantaeRegnum.PlSpeciesId;
            //}

            var speciesId = Tbl75SpeciessesAllList.First().SpeciesId;

            Tbl81ImagesList ??= [];
            Tbl81ImagesList.Insert(index: 0, item: new Tbl81Image { SpeciesId = speciesId });
        });
    }

    public async Task SaveImageAsync(Tbl81Image viewModelImageSelected, string selectedPath)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewImage)
            {
                IsNewImage = false;
            }
        });

        Tbl81Image result = null;
        try
        {
            if (IsModifiedImage)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertImageAsync(viewModelImageSelected, selectedPath);
                    IsModifiedImage = false;
                });
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

    private async Task DeleteImage(int imageid)
    {
        await _dispatcherQueue.EnqueueAsync(async () =>
        {
            var image = await Context.Tbl81Images.FirstOrDefaultAsync(b => b.ImageId == imageid);
            if (image != null)
            {
                Context.Tbl81Images.Remove(image);
                await Context.SaveChangesAsync();
            }
        });
    }

    public ICommand ChangeImageCommand => new CommunityToolkit.Mvvm.Input.RelayCommand(execute: delegate { var task = OpenFileDialog(); });

    private async Task OpenFileDialog()
    {
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(_filePicker, hwnd);
        _filePicker.FileTypeFilter.Add("*");
        _filePicker.ViewMode = PickerViewMode.Thumbnail;
        //_filePicker.FileTypeFilter.Add("*");
        _filePicker.FileTypeFilter.Add(".jpg");
        _filePicker.FileTypeFilter.Add(".jpeg");
        _filePicker.FileTypeFilter.Add(".png");
        _filePicker.FileTypeFilter.Add(".gif");
        _filePicker.FileTypeFilter.Add(".jpe");
        _filePicker.FileTypeFilter.Add(".bmp");
        _filePicker.FileTypeFilter.Add(".ico");
        _filePicker.FileTypeFilter.Add(".tif");
        _filePicker.FileTypeFilter.Add(".tiff");
        _filePicker.FileTypeFilter.Add(".hpd");
        _filePicker.FileTypeFilter.Add(".jxr");
        _filePicker.FileTypeFilter.Add(".wdp");
        _filePicker.FileTypeFilter.Add(".hpd");
        var file = await _filePicker.PickSingleFileAsync();
        if (file != null && file.Path != null)
        {
            SelectedPath = file.Path;
            ImageSource = new BitmapImage(new Uri(file.Path));
        }
    }


    #endregion [Methods Image]  

    #region [Methods Species]

    public async Task SaveSpeciesAsync(Tbl75Species viewModelSpeciesSelected)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (IsNewSpecies)
            {
                IsNewSpecies = false;
            }
        });

        Tbl75Species result = null;
        try
        {
            if (IsModifiedSpecies)
            {
                await _dispatcherQueue.EnqueueAsync(async () =>
                {
                    result = await _dataService.UpsertSpeciesAsync(viewModelSpeciesSelected);
                    IsModifiedSpecies = false;
                });
            }
            else
            {
                IsModifiedSpecies = true;
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
            IsInEditSpecies = false;
            IsModifiedSpecies = false;
            IsExistingSpecies = true;
        }
        else
        {
            _ = new SavingException(StringsRes.UnableToSave1 + StringsRes.UnableToSave2);
        }
    }

    #endregion [Methods Species]



    //    Part 2    


    #region All Properties

    #region [Propertys]

    //------------IsModified--------------------------------------
    public bool IsModifiedImage { get; set; }
    public bool IsModifiedSpecies { get; set; }
    //------------Load--------------------------------------
    private bool IsLoaded => ImageItems != null;
    public bool IsNotLoaded => !IsLoaded;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    //----------IsDeleting-----------------------------------------
    private bool _isDeletingImage;
    public bool IsDeletingImage
    {
        get => _isDeletingImage;
        set => SetProperty(ref _isDeletingImage, value);
    }

    private bool _isDeletingSpecies;
    public bool IsDeletingSpecies
    {
        get => _isDeletingSpecies;
        set => SetProperty(ref _isDeletingSpecies, value);
    }
    //------------IsNew---------------------------------------
    private bool _isNewImage;
    public bool IsNewImage
    {
        get => _isNewImage;
        set => SetProperty(ref _isNewImage, value);
    }

    private bool _isNewSpecies;
    public bool IsNewSpecies
    {
        get => _isNewSpecies;
        set => SetProperty(ref _isNewSpecies, value);
    }
    //----------IsExisting-----------------------------------------
    private bool _isExistingImage;
    public bool IsExistingImage
    {
        get => _isExistingImage;
        set => SetProperty(ref _isExistingImage, value);
    }
    private bool _isExistingSpecies;
    public bool IsExistingSpecies
    {
        get => _isExistingSpecies;
        set => SetProperty(ref _isExistingSpecies, value);
    }

    //-----------IsInEdit----------------------------------------
    private bool _isInEditImage = false;
    public bool IsInEditImage
    {
        get => _isInEditImage;
        set => SetProperty(ref _isInEditImage, value);
    }
    private bool _isInEditSpecies = false;
    public bool IsInEditSpecies
    {
        get => _isInEditSpecies;
        set => SetProperty(ref _isInEditSpecies, value);
    }
    //----------StartEdit-----------------------------------------
    public void StartEditImage()
    {
        IsInEditImage = true;
        IsModifiedImage = false;
    }
    public void StartEditSpecies()
    {
        IsInEditSpecies = true;
        IsModifiedSpecies = false;
    }
    //------------BeginEdit---------------------------------------
    public static void BeginEdit()
    {
        // Not used.
    }
    //---------------EndEdit------------------------------------
    public async void EndEditImage() => await SaveImageAsync(ImageSelected, SelectedPath);
    //-------------CanRefresh--------------------------------------
    public bool CanRefreshImage => !IsModifiedImage && IsExistingImage;
    public bool CanRefreshSpecies => !IsModifiedSpecies && IsExistingSpecies;
    //--------------CanRevert-------------------------------------
    public bool CanRevertImage => IsModifiedImage && IsExistingImage;
    public bool CanRevertSpecies => IsModifiedSpecies && IsExistingSpecies;

    #endregion [Propertys]     


    #region Selected Properties

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
    private Tbl66Genus _genusSelected;

    public Tbl66Genus GenusSelected
    {
        get => _genusSelected;
        set => SetProperty(ref _genusSelected, value);
    }

    private Tbl75Species _speciesSelected;
    public Tbl75Species SpeciesSelected
    {
        get => _speciesSelected;
        set => SetProperty(ref _speciesSelected, value);
    }
    #endregion Selected Properties       

    #region Public Properties

    private int _imageDataSetCount;

    public int ImageDataSetCount
    {
        get => _imageDataSetCount;
        set
        {
            _imageDataSetCount = value; OnPropertyChanged();
        }
    }
    private int _speciesDataSetCount;
    public int SpeciesDataSetCount
    {
        get => _speciesDataSetCount;
        set
        {
            _speciesDataSetCount = value; OnPropertyChanged();
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
    private ObservableCollection<Tbl75Species> _tbl75SpeciessesAllList;
    public ObservableCollection<Tbl75Species> Tbl75SpeciessesAllList
    {
        get => _tbl75SpeciessesAllList;
        set
        {
            _tbl75SpeciessesAllList = value; OnPropertyChanged();
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



    private ObservableCollection<Tbl81Image> _tbl81ImagesAllList;
    public ObservableCollection<Tbl81Image> Tbl81ImagesAllList
    {
        get => _tbl81ImagesAllList;
        set
        {
            _tbl81ImagesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get => _tbl66GenussesList;
        set
        {
            _tbl66GenussesList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl66Genus> _tbl66GenussesAllList;
    public ObservableCollection<Tbl66Genus> Tbl66GenussesAllList
    {
        get => _tbl66GenussesAllList;
        set
        {
            _tbl66GenussesAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl68Speciesgroup> _tbl68SpeciesgroupsAllList;
    public ObservableCollection<Tbl68Speciesgroup> Tbl68SpeciesgroupsAllList
    {
        get => _tbl68SpeciesgroupsAllList;
        set
        {
            _tbl68SpeciesgroupsAllList = value; OnPropertyChanged();
        }
    }

    private ObservableCollection<Tbl75Species> _tbl75SpeciesList;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesList
    {
        get => _tbl75SpeciesList;
        set
        {
            _tbl75SpeciesList = value; OnPropertyChanged();
        }
    }

    #endregion Public Properties    

    #region Refresh Methods

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

    public void RefreshGenusItems()
    {
        GenusItems.Clear();
        if (Tbl66GenussesList != null)
        {
            foreach (var item in Tbl66GenussesList)
            {
                GenusItems.Add(item);
            }

            if (Tbl66GenussesList.Count == 0)
            {
                return;
            }

            if (GenusSelected == null && Tbl66GenussesList.Count != 0)
            {
                GenusSelected = GenusItems.FirstOrDefault()!;
            }
        }
    }


    public void RefreshSpeciesItems()
    {
        SpeciesItems.Clear();
        if (Tbl75SpeciessesList != null)
        {
            foreach (var item in Tbl75SpeciessesList)
            {
                SpeciesItems.Add(item);
            }

            if (Tbl75SpeciessesList.Count == 0)
            {
                return;
            }

            if (SpeciesSelected == null && Tbl75SpeciessesList.Count != 0)
            {
                SpeciesSelected = SpeciesItems.FirstOrDefault()!;
            }
        }
    }

    #endregion Refresh Methods

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

    #endregion All Properties 

}
