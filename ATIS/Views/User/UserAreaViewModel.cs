using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using Microsoft.EntityFrameworkCore;


namespace ATIS.Views.User;


public partial class UserAreaViewModel : ObservableObject
{
    private readonly IDbContextFactory<AtisDbContext> _factory;
    private readonly IDataService _data;
    private readonly IDialogService _dialog;
    private readonly LookupService _lookupService;
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;
    private int _loadVersion;
    public event EventHandler? AccountDeleted;
    public UserAreaViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        LookupService lookupService,
        IUserService user,
        IAuthenticationService authService)
    {
        _factory = factory;
        _data = new DataService(factory, user);
        _dialog = dialog;
        _lookupService = lookupService;
        _userService = user;
        _authService = authService;

        SaveCommand = new AsyncRelayCommand(SaveAsync, () => Item != null && !IsBusy);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => Item != null && !IsBusy);
    }

    public ObservableCollection<Models.TblCountry> CountryList { get; private set; } = new();

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    public void ResetProfile()
    {
        _loadVersion++;
        Item = null;
        IsBusy = false;
    }

    public async Task LoadAsync()
    {
        ResetProfile();
        var version = _loadVersion;
        var email = _userService.CurrentUser;
        if (string.IsNullOrWhiteSpace(email))
            return;

        try
        {
            IsBusy = true;

            var countries = await _lookupService.GetCountryListAsync();
            if (!IsCurrentLoad(version, email))
                return;

            await using var db = await _factory.CreateDbContextAsync();
            var profile = await db.TblUserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (!IsCurrentLoad(version, email))
                return;

            if (profile == null)
                throw new InvalidOperationException("Das angemeldete Benutzerprofil wurde nicht gefunden.");

            CountryList = new ObservableCollection<Models.TblCountry>(countries);
            OnPropertyChanged(nameof(CountryList));
            Item = profile;
        }
        catch (Exception ex)
        {
            SimpleLog.Error("Benutzerprofil konnte nicht geladen werden.");
            SimpleLog.Log(ex);
            if (IsCurrentLoad(version, email))
                await _dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
        finally
        {
            if (version == _loadVersion)
                IsBusy = false;
        }
    }

    private bool IsCurrentLoad(int version, string email) =>
        version == _loadVersion &&
        string.Equals(email, _userService.CurrentUser, StringComparison.OrdinalIgnoreCase);


    private TblUserProfile? _item;
    public TblUserProfile? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                SaveCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                SaveCommand.NotifyCanExecuteChanged();
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }
    }
    private async Task SaveAsync()
    {
        var item = Item;
        if (item is null) return;
        var version = _loadVersion;
        var email = _userService.CurrentUser;

        // nur das eigene Profil
        if (!string.Equals(item.Email, email, StringComparison.OrdinalIgnoreCase))
            return;

        if (!await _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message"))
            return;

        if (!IsCurrentLoad(version, email))
            return;

        try
        {
            IsBusy = true;
            var saved = await _data.UpsertUserProfileAsync(item);
            if (saved != null)
            {
                if (IsCurrentLoad(version, email))
                    Item = saved;
                _lookupService.InvalidateAllCaches();
            }
        }
        catch (Exception ex)
        {
            SimpleLog.Error("Benutzerprofil konnte nicht gespeichert werden.");
            SimpleLog.Log(ex);
            if (IsCurrentLoad(version, email))
                await _dialog.ShowErrorAsync("Error_Save_Title", "Error_Save_Message");
        }
        finally
        {
            if (version == _loadVersion)
                IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (Item is null) return;

        // Sicherheit: nur eigenes Profil
        if (!string.Equals(Item.Email, _userService.CurrentUser, StringComparison.OrdinalIgnoreCase))
            return;

        var confirm = await _dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");
        if (!confirm) return;

        // Passwortabfrage: an deinen Dialog-Service anpassen
        // Beispielname; falls nicht vorhanden, ersetze durch deinen eigenen Input-Dialog:
        var password = await _dialog.ShowPasswordPromptAsync("Account_Delete_Password_Title", "Account_Delete_Password_Message");
        if (string.IsNullOrWhiteSpace(password))
            return;
        try
        {
            IsBusy = true;

            await _authService.DeleteOwnAccountAsync(_userService.CurrentUser, password, CancellationToken.None);

            _userService.Clear();
            Item = null;

            await _dialog.ShowInfoAsync("Account_Delete_Done_Title", "Account_Delete_Done_Message");

            AccountDeleted?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            SimpleLog.Error("Benutzerprofil konnte nicht gelöscht werden.");
            SimpleLog.Log(ex);
            await _dialog.ShowErrorAsync("Error_Delete_Title", "Error_Delete_Message");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
