using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Main.Miscellaneous;

public partial class MiscellaneousViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // =========================================================
    // Constructor
    // =========================================================
    public MiscellaneousViewModel(IDataService dataService)
    {
        _dataService = dataService;

        SearchCommand = new AsyncRelayCommand<string>(SearchAsync);
        PdfMiscellaneousCommand = new RelayCommand(ExecutePdfMiscellaneous, CanExecutePdfMiscellaneous);
        EditMiscellaneousCommand = new RelayCommand(ExecuteEditMiscellaneous, CanExecuteEditMiscellaneous);
        DeleteMiscellaneousCommand = new RelayCommand(ExecuteDeleteMiscellaneous, CanExecuteDeleteMiscellaneous);
    }

    // =========================================================
    // Collections
    // =========================================================
    public ObservableCollection<TblMiscellaneous> Miscellaneousses { get; } = [];

    // =========================================================
    // Loading Indicator
    // =========================================================
    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

    // =========================================================
    // Dataset Count
    // =========================================================
    private int _miscellaneousDataSetCount;
    public int MiscellaneousDataSetCount
    {
        get => _miscellaneousDataSetCount;
        set => SetProperty(ref _miscellaneousDataSetCount, value);
    }
    public string MiscellaneousDataSetCountText =>
        $"{MiscellaneousDataSetCount} Datensätze gefunden";

    // =========================================================
    // Search Placeholder (optional, falls Localizer nicht genutzt)
    // =========================================================
    public string SearchPlaceholder => "Suche nach Verschiedenem…";

    // =========================================================
    // Search Logic
    // =========================================================
    public ICommand SearchCommand { get; }

    public async Task SearchAsync(string query)
    {
        try
        {
            IsLoading = true;
            Miscellaneousses.Clear();

            if (string.IsNullOrWhiteSpace(query))
            {
                MiscellaneousDataSetCount = 0;
                OnPropertyChanged(nameof(MiscellaneousDataSetCountText));
                AllDialogs.NoDatasetFoundWarnMessageDialog();
                return;
            }

            var result = await _dataService.GetMiscellaneoussesByNameOrIdAsync(query);

            foreach (var item in result)
                Miscellaneousses.Add(item);

            MiscellaneousDataSetCount = Miscellaneousses.Count;
            OnPropertyChanged(nameof(MiscellaneousDataSetCountText));

            MiscellaneousSelected = Miscellaneousses.FirstOrDefault();

            if (Miscellaneousses.Count == 0)
                AllDialogs.NoDatasetFoundWarnMessageDialog();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void UpdateSuggestions(string text)
    {
        // optional: Suggestion-Logik
    }

    // =========================================================
    // Selected Item
    // =========================================================
    private TblMiscellaneous _miscellaneousSelected;
    public TblMiscellaneous MiscellaneousSelected
    {
        get => _miscellaneousSelected;
        set
        {
            if (SetProperty(ref _miscellaneousSelected, value))
            {
                (PdfMiscellaneousCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (EditMiscellaneousCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (DeleteMiscellaneousCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    // =========================================================
    // Commands (PDF, Edit, Delete)
    // =========================================================
    public ICommand PdfMiscellaneousCommand { get; }
    public ICommand EditMiscellaneousCommand { get; }
    public ICommand DeleteMiscellaneousCommand { get; }

    private bool CanExecutePdfMiscellaneous() => MiscellaneousSelected?.MiscellaneousId > 0;
    private void ExecutePdfMiscellaneous()
    {
        // später: PDF-Service
    }

    private bool CanExecuteEditMiscellaneous() => MiscellaneousSelected != null;
    private void ExecuteEditMiscellaneous()
    {
        // später: Edit-Service
    }

    private bool CanExecuteDeleteMiscellaneous() => MiscellaneousSelected != null;
    private void ExecuteDeleteMiscellaneous()
    {
        // später: Delete-Service
    }
}

//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using ATIS.Helpers;
//using ATIS.Interfaces;
//using ATIS.Models;
//using ATIS.Services;
//using Windows.Storage.Pickers;
//using Windows.System;

////    DiseaseViewModel Skriptdatum:  09.09.2025  10:32    

//namespace ATIS.Views.Main.Disease;
//public partial class DiseaseViewModel(IDataService dataService) : ObservableObject
//{
//    #region [Private Data Members]
//    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
//    private static readonly AtisDbContext Context = new();
//    private readonly IDataService _dataService = dataService;
//    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
//    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];
//    public string Search;

//    private readonly FileOpenPicker _filePicker = new();

//    private readonly List<Data> _fileStreamList = [];
//    public struct Data
//    {
//        public int ImageCountId;
//        public byte[] ImageFileStream;
//    };
//    #endregion [Private Data Members]              

//    //    Part 1    

//    #region [Methods Disease]

//    public ICommand SearchRunCommand => new WinUICommunity.RelayCommand(execute: delegate { SearchRun(FilterText); });

//    public void SearchRun(string filterText)
//    {
//        DiseasesCollection?.Clear();

//        if (string.IsNullOrEmpty(filterText))
//        {
//            AllDialogs.NoDatasetFoundWarnMessageDialog();
//            return;
//        }

//        IsLoading = true;
//        DiseasesCollection = _dataService.GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(filterText);

//        DataSetCount = DiseasesCollection.Count;
//        RefreshDiseaseItems();
//        //   IsLoading = false;
//    }

//    #region Report Search

//    //public void ReportSearchAll(int id, string tab)
//    //{
//    //    FoodsCollection?.Clear();
//    //    GetTblFoodsById(id);

//    //}

//    #endregion Report Search

//    #endregion [Methods Disease ]  

//    //    Part 2    

//    #region All Properties

//    #region [Propertys]
//    //------------IsModified--------------------------------------
//    public bool IsModifiedDisease { get; set; }
//    //------------Load--------------------------------------
//    private bool IsLoaded => DiseaseItems != null;
//    public bool IsNotLoaded => !IsLoaded;

//    private bool _isLoading;
//    public bool IsLoading
//    {
//        get => _isLoading;
//        set => SetProperty(ref _isLoading, value);
//    }
//    private int _dataSetCount;
//    public int DataSetCount
//    {
//        get => _dataSetCount;
//        set
//        {
//            _dataSetCount = value; OnPropertyChanged();
//        }
//    }
//    private int _imageDataSetCount;
//    public int ImageDataSetCount
//    {
//        get => _imageDataSetCount;
//        set
//        {
//            _imageDataSetCount = value; OnPropertyChanged();
//        }
//    }

//    public string FilterText { get; private set; }

//    #endregion [Propertys]

//    #region Selected Properties

//    private TblDisease _diseaseSelected;

//    public TblDisease DiseaseSelected
//    {
//        get => _diseaseSelected;
//        set => SetProperty(ref _diseaseSelected, value);
//    }

//    private Tbl81Image _imageSelected;

//    public Tbl81Image ImageSelected
//    {
//        get => _imageSelected;
//        set => SetProperty(ref _imageSelected, value);
//    }

//    #endregion Selected Properties     

//    #region Public Properties  

//    private int _diseaseDataSetCount;
//    public int DiseaseDataSetCount
//    {
//        get => _diseaseDataSetCount;
//        set
//        {
//            _diseaseDataSetCount = value; OnPropertyChanged();
//        }
//    }

//    private string _searchDiseaseName = "";

//    public string SearchDiseaseName
//    {
//        get => _searchDiseaseName;
//        set
//        {
//            _searchDiseaseName = value; OnPropertyChanged();
//        }
//    }

//    private ObservableCollection<TblDisease> _tblDiseasesList;

//    public ObservableCollection<TblDisease> TblDiseasesList
//    {
//        get => _tblDiseasesList;
//        set
//        {
//            _tblDiseasesList = value; OnPropertyChanged();
//        }
//    }
//    private ObservableCollection<Tbl81Image> _tbl81ImagesList;

//    public ObservableCollection<Tbl81Image> Tbl81ImagesList
//    {
//        get => _tbl81ImagesList;
//        set
//        {
//            _tbl81ImagesList = value; OnPropertyChanged();
//        }
//    }


//    #endregion Public Properties

//    #region Refresh Methods

//    public void RefreshDiseaseItems()
//    {
//        DiseaseItems.Clear();
//        if (TblDiseasesList != null)
//        {
//            foreach (var item in TblDiseasesList)
//            {
//                DiseaseItems.Add(item);
//            }

//            if (TblDiseasesList.Count == 0)
//            {
//                return;
//            }

//            if (DiseaseSelected == null && TblDiseasesList.Count != 0)
//            {
//                DiseaseSelected = DiseaseItems.FirstOrDefault()!;
//            }
//        }
//    }
//    public void RefreshImageItems()
//    {
//        ImageItems.Clear();
//        if (Tbl81ImagesList != null)
//        {
//            foreach (var item in Tbl81ImagesList)
//            {
//                ImageItems.Add(item);
//            }

//            if (Tbl81ImagesList.Count == 0)
//            {
//                return;
//            }

//            if (ImageSelected == null && Tbl81ImagesList.Count != 0)
//            {
//                ImageSelected = ImageItems.FirstOrDefault()!;
//            }
//        }
//    }

//    #endregion Refresh Methods

//    #region Collection

//    public ObservableCollection<TblDisease> DiseasesCollection
//    {
//        get; set;
//    } = null!;


//    #endregion Collection

//    #endregion All Properties

//}
