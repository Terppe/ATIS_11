using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Windows.System;

//    MiscellaneousViewModel Skriptdatum:  09.09.2025  10:32    

namespace ATIS.Views.Main;
public partial class MiscellaneousViewModel(IDataService dataService) : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService = dataService;
    public ObservableCollection<TblMiscellaneous> MiscellaneousItems { get; } = [];
    public ObservableCollection<Tbl81Image> ImageItems { get; } = [];

    #endregion [Private Data Members]           


    //    Part 1    

    #region [Methods Miscellaneous]

    #endregion [Methods Miscellaneous ]  

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

    public string SearchMiscellaneousName
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

    #endregion All Properties

}


