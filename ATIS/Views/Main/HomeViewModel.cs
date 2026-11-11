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
    #region [All Properties]
    // =========================================================

    // =========================================================
    #endregion [All Properties]
    // =========================================================


}
