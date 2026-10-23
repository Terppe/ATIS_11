

namespace ATIS.Views.Auth;

public sealed partial class ChangePasswordView : UserControl
{
    public AuthenticationViewModel ViewModel { get; }

    public ChangePasswordView()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private async void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CurrentPassword = OldPwdBox.Password;
        ViewModel.NewPassword = NewPwdBox.Password;
        await ViewModel.ChangePasswordCommand.ExecuteAsync(null);
    }
}
