namespace ATIS.Views.Auth;

public sealed partial class ForgotPasswordView : UserControl
{
    public AuthenticationViewModel ViewModel { get; }

    public ForgotPasswordView()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private async void ResetPassword_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.NewPassword = NewPasswordBox.Password;
        await ViewModel.ResetPasswordWithCodeCommand.ExecuteAsync(null);
    }
}
