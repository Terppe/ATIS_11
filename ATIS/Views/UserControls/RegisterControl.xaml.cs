using ATIS.Views.Auth;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class RegisterControl : UserControl
{
    public AuthenticationViewModel ViewModel
    {
        get;
    }

    public RegisterControl()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private void Content_TextChanged(object sender, RoutedEventArgs e)
    {
        if (!TbEmail.Text.Equals(string.Empty) && TxtPass.Password.Equals(TxtConfirmPass.Password))
            BtRegisterSave.IsEnabled = true;
        else
            BtRegisterSave.IsEnabled = false;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        BtRegisterSave.IsEnabled = false;
        TbEmail.Text = "";
        TxtPass.Password = "";
        TxtConfirmPass.Password = "";
    }
}
