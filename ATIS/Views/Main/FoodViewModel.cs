using System.Collections.ObjectModel;
using Windows.System;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;

//    FoodViewModel Skriptdatum:  13.06.2024  10:32
    
namespace ATIS.Views.Main;

public partial class FoodViewModel : ObservableObject
{
    #region [Private Data Members]
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public IJsonNavigationViewService JsonNavigationViewService;
    private static readonly AtisDbContext Context = new();
    private readonly IDataService _dataService;
    public ObservableCollection<Tbl95Food> FoodItems { get; } = [];
    public string Search;

    #endregion [Private Data Members]      

    #region [Constructor]
    public FoodViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    public FoodViewModel()
    {

    }

    #endregion [Constructor]  

    //    Part 1    

    #region [Methods Food]

    #endregion [Methods Food]

}
