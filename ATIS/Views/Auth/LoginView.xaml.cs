

namespace ATIS.Views.Auth;

public sealed partial class LoginView : UserControl
{
    public AuthenticationViewModel ViewModel { get; }

    public LoginView()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoginCommand.ExecuteAsync(PwdBox.Password);
    }
}
