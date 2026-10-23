using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Views.Auth;

public sealed partial class RegisterView : UserControl
{
    public AuthenticationViewModel ViewModel { get; }

    public RegisterView()
    {
        ViewModel = App.GetService<AuthenticationViewModel>();
        InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.Equals(PwdBox.Password, ConfirmPwdBox.Password, StringComparison.Ordinal))
        {
            ViewModel.ErrorMessage = "Passwörter stimmen nicht überein.";
            return;
        }

        await ViewModel.RegisterCommand.ExecuteAsync(PwdBox.Password);
    }
}
