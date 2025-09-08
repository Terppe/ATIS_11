using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.ContentDialogs;
using ATIS.Views.Database;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DiseasePage : Page
{
    #region [Private Data Members]

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    public DiseaseViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();

    #endregion [Private Data Members]   

    #region [Constructor]
    public DiseasePage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<DiseaseViewModel>();
            InitializeComponent();
            SimpleLog.Info(" DiseasePage opened ");
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }

    #endregion [Constructor]
}
