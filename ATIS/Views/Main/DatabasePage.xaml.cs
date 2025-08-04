

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using ATIS.Helpers;

namespace ATIS.Views.Main;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DatabasePage : Page
{
    public DatabaseViewModel ViewModel { get; }

    public DatabasePage()
    {
        if (Permission.RolePermission())
        {
            ViewModel = App.GetService<DatabaseViewModel>();
            InitializeComponent();
        }
        else
        {
            AllDialogs.WarningNoPermissionMessageDialogAsync("").ConfigureAwait(false);   //no permission
        }
    }
}
