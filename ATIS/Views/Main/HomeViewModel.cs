using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;

namespace ATIS.Views.Main;

public partial class HomeViewModel : ObservableObject
{
    // =========================================================
    #region [Private Data Members]
    // =========================================================

    public HomeViewModel ViewModel { get; }
    private readonly IDataService _dataService;

    // =========================================================
    #endregion [Private Data Members]
    // =========================================================


    // =========================================================
    #region [Constructor]
    // =========================================================

    public HomeViewModel(IDataService dataService)
    {
        _dataService = dataService;

    }
    // =========================================================
    #endregion [Constructor]
    // =========================================================


    // =========================================================
    #region [Methods Home]
    // =========================================================

    // =========================================================
    #endregion [Methods Home]
    // =========================================================

    // =========================================================
    #region [All Properties]
    // =========================================================

    // =========================================================
    #endregion [All Properties]
    // =========================================================


}
