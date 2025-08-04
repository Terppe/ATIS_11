using ATIS.Views.Authentication;

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
}
