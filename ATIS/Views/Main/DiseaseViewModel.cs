using System.Collections.ObjectModel;
using Windows.System;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;

//    DiseaseViewModel Skriptdatum:  13.06.2024  10:32

namespace ATIS.Views.Main;

public partial class DiseaseViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl96Disease> DiseaseItems { get; } = [];
    public string Search;

    #endregion [Private Data Members]      

    #region [Constructor]
    public DiseaseViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public DiseaseViewModel()
    {

    }

    #endregion [Constructor]  

    //    Part 1    

    #region [Methods Disease]


    #endregion [Methods Disease]  

}
