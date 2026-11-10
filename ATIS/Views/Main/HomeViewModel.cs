using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Main;

public partial class HomeViewModel : ObservableObject
{
    // =========================================================
    #region [Private Data Members]
    // =========================================================

    public HomeViewModel ViewModel { get; }
 //   private readonly IDataService _dataService;
 //   private readonly LookupService _lookupService;

    // =========================================================
    #endregion [Private Data Members]
    // =========================================================


    // =========================================================
    #region [Constructor]
    // =========================================================

    public HomeViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        IUserService user)
    {
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
