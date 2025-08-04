using ATIS.Views.Authentication;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class PasswordForgotControl : UserControl
{
    public AuthenticationViewModel ViewModel
    {
        get;
    }

    public PasswordForgotControl()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private void TbEmail_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!TbEmail.Text.Equals(string.Empty))
            BtnForgot.IsEnabled = true;
        else
            BtnForgot.IsEnabled = false;
    }
}
