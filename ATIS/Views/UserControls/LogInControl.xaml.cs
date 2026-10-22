using ATIS.Views.Auth;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class LogInControl : UserControl
{
    public AuthenticationViewModel ViewModel
    {
        get;
    }

    public LogInControl()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private void Content_TextChanged(object sender, RoutedEventArgs e)
    {
        if (!TbEmail.Text.Equals(string.Empty))
        {
            BtnLogin.IsEnabled = true;
        }
        else
        {
            BtnLogin.IsEnabled = false;
        }
    }

    private void BtnLogin_OnLostFocus(object sender, RoutedEventArgs e)
    {
        TbAuthUser.Foreground = new SolidColorBrush(Colors.Green);
    }
}
