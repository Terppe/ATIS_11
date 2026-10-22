

namespace ATIS.Views.Auth;

public sealed partial class LoginView : UserControl
{
    public LoginViewModel ViewModel { get; }

    public LoginView()
    {
        ViewModel = App.GetService<LoginViewModel>();
        InitializeComponent();
    }
    private void PwdBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel is null || sender is not PasswordBox pb) return;
        ViewModel.Password = pb.Password;
        System.Diagnostics.Debug.WriteLine($"PwdChanged: len={ViewModel.Password?.Length}");
    }
}
