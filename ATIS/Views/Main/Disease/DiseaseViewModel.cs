using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Windows.System;

//    DiseaseViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main;
public partial class DiseaseViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<TblDisease> DiseaseItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];

    #endregion [Private Data Members]           


    //    Part 1    

    #region [Methods Disease]

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

    #endregion [Propertys]

    #region Selected Properties

    private TblDisease _ciseaseSelected;

    public TblDisease DiseaseSelected
    {
        get => _ciseaseSelected;
        set => SetProperty(ref _ciseaseSelected, value);
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

    #endregion Refresh Methods

    #endregion All Properties

}
