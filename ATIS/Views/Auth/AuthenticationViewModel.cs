using System.Text.RegularExpressions;
using ATIS.Interfaces;

namespace ATIS.Views.Auth;

public partial class AuthenticationViewModel : ObservableObject
{
    private readonly IAuthenticationService _authService;
    private readonly IUserService _userService;

    public AuthenticationViewModel(
        IAuthenticationService authService,
        IUserService userService)
    {
        _authService = authService;
        _userService = userService;

        LoginCommand = new AsyncRelayCommand<string?>(LoginAsync);
        RegisterCommand = new AsyncRelayCommand<string?>(RegisterAsync);
        ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordAsync);
        ChangePasswordCommand = new AsyncRelayCommand(ChangePasswordAsync);

    }

    private string? _email;
    public string? Email { get => _email; set => SetProperty(ref _email, value); }

    private string? _firstName;
    public string? FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

    private string? _lastName;
    public string? LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

    private string? _role = "Developer";
    public string? Role { get => _role; set => SetProperty(ref _role, value); }

    private string? _password;
    public string? Password { get => _password; set => SetProperty(ref _password, value); }

    private string? _currentPassword;
    public string? CurrentPassword { get => _currentPassword; set => SetProperty(ref _currentPassword, value); }

    private string? _newPassword;
    public string? NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

    private string _status = string.Empty;
    public string Status { get => _status; set => SetProperty(ref _status, value); }

    private string _errorMessage = string.Empty;
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    private string _statusMessage = string.Empty;
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    private bool _isBusy;
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    private bool _canLogin;
    public bool CanLogin { get => !_canLogin; set => SetProperty(ref _canLogin, !value); }

    private bool _canRegister;
    public bool CanRegister { get => !_canRegister; set => SetProperty(ref _canRegister, !value); }

    public IAsyncRelayCommand<string?> LoginCommand { get; }
    public IAsyncRelayCommand<string?> RegisterCommand { get; }
    public IAsyncRelayCommand ForgotPasswordCommand { get; }
    public IAsyncRelayCommand ChangePasswordCommand { get; }



    public event EventHandler? LoginSucceeded;
    public event EventHandler? RegisterSucceeded;
    public event EventHandler? PasswordChangedSucceeded;

    private static bool IsValidEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return Regex.IsMatch(value,
            @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
    }

    private async Task LoginAsync(string? clearTextPassword)
    {
        if (IsBusy) return;
        ErrorMessage = "";
        Status = "";

        if (!IsValidEmail(Email))
        {
            ErrorMessage = "Email muss das Format xxx@xxx.xxx haben.";
            return;
        }

        if (string.IsNullOrWhiteSpace(clearTextPassword))
        {
            ErrorMessage = "Passwort ist erforderlich.";
            return;
        }

        try
        {
            IsBusy = true;

            var user = await _authService.AuthenticateUserAsync(Email!, clearTextPassword);
            if (user is null)
            {
                ErrorMessage = "Ungültige Anmeldedaten.";
                return;
            }


            _userService.SetUser(user.Email, user.Role);
            Status = "Login erfolgreich.";
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RegisterAsync(string? clearTextPassword)
    {
        if (IsBusy) return;
        ErrorMessage = "";
        Status = "";

        if (!IsValidEmail(Email))
        {
            ErrorMessage = "Email muss das Format xxx@xxx.xxx haben.";
            return;
        }

        if (string.IsNullOrWhiteSpace(clearTextPassword))
        {
            ErrorMessage = "Passwort ist erforderlich.";
            return;
        }

        try
        {
            IsBusy = true;

            var created = await _authService.RegisterAsync(
                email: Email!,
                password: clearTextPassword,
                role: Role);

            Status = $"Registrierung erfolgreich (ID: {created.UserProfileId}).";
            RegisterSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ForgotPasswordAsync()
    {
        if (IsBusy) return;
        ErrorMessage = "";
        Status = "";

        if (!IsValidEmail(Email))
        {
            ErrorMessage = "Bitte gültige Email eingeben.";
            return;
        }

        try
        {
            IsBusy = true;
            var exists = await _authService.RequestPasswordResetAsync(Email!);

            // bewusst neutral (kein User-Leak)
            Status = exists
                ? "Falls ein Konto existiert, wurden Reset-Schritte ausgelöst."
                : "Falls ein Konto existiert, wurden Reset-Schritte ausgelöst.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ChangePasswordAsync()
    {
        if (IsBusy) return;
        ErrorMessage = "";
        Status = "";

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Email ist erforderlich.";
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Aktuelles und neues Passwort sind erforderlich.";
            return;
        }

        try
        {
            IsBusy = true;
            await _authService.ChangePasswordByEmailAsync(Email!, CurrentPassword!, NewPassword!);

            Status = "Passwort erfolgreich geändert.";
            PasswordChangedSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
