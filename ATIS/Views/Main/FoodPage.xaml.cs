using ATIS.Helpers;
using ATIS.Models;
using ATIS.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FoodPage : Page
{
    #region [Private Data Members]

    public FoodViewModel ViewModel { get; }
    private static readonly AtisDbContext Context = new();
    private string _search;

    #endregion [Private Data Members]

    #region [Constructor]

    public FoodPage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<FoodViewModel>();
            InitializeComponent();
            SimpleLog.Info(" FoodPage aufgerufen ");
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
    #endregion [Constructor]
}
