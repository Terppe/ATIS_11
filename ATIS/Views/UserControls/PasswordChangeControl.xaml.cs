using ATIS.Views.Authentication;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class PasswordChangeControl : UserControl
{
    public AuthenticationViewModel ViewModel
    {
        get;
    }

    public PasswordChangeControl()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private void Content_TextChanged(object sender, RoutedEventArgs e)
    {
        if (!TbEmail.Text.Equals(string.Empty) && TxtPassNew.Password.Equals(TxtConfirmPassNew.Password))
            BtnChange.IsEnabled = true;
        else
            BtnChange.IsEnabled = false;
    }

}
