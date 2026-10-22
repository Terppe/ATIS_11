
using ATIS.Interfaces;

namespace ATIS.Views.Auth;

public class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    public event EventHandler? LoginSucceeded;
    private string _email = string.Empty;
    private readonly IUserService _userService;
    public string Email
    {
        get => _email;
        set { if (SetProperty(ref _email, value)) OnPropertyChanged(nameof(CanLogin)); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { if (SetProperty(ref _password, value)) OnPropertyChanged(nameof(CanLogin)); }
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { if (SetProperty(ref _isBusy, value)) OnPropertyChanged(nameof(CanLogin)); }
    }

    public bool CanLogin =>
        !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    public IAsyncRelayCommand LoginCommand { get; }
    public IRelayCommand<string?> PasswordChangedCommand { get; }

    public LoginViewModel(IAuthenticationService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }
    private async Task LoginAsync()
    {
        StatusMessage = "LoginAsync gestartet";
        System.Diagnostics.Debug.WriteLine("LoginAsync gestartet");

        if (!CanLogin) return;

        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var user = await Task.Run(() => _authService.AuthenticateUser(Email.Trim(), Password));

            // DEBUG
            System.Diagnostics.Debug.WriteLine(user == null
                ? "user = null"
                : $"user.Email='{user.Email}', user.Role='{user.Role}'");

            if (user != null)
            {
                _userService.SetUser(user.Email, user.Role);
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
                return;
            }
            //if (user != null)
            //{
            //    Thread.CurrentPrincipal = new CustomPrincipal
            //    {
            //        Identity = new CustomIdentity(user.Email, user.Role)
            //    };
            //    System.Diagnostics.Debug.WriteLine($"principal email='{((CustomPrincipal)Thread.CurrentPrincipal).Identity.Email}'");

            //    StatusMessage = $"Willkommen {user.Email}";
            //    LoginSucceeded?.Invoke(this, EventArgs.Empty);
            //    return;
            //}
            else
                {
                StatusMessage = "Login fehlgeschlagen";
            }
        }
        catch (UnauthorizedAccessException)
        {
            StatusMessage = "Ungültige E-Mail oder Passwort.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.ToString(); // temporär zur Diagnose
        }
        finally
        {
            IsBusy = false;
            Password = string.Empty;
        }
    }
}
