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

    public ObservableCollection<Models.TblCountry> CountryList { get; } = new();

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;

            CountryList.Clear();
            foreach (var c in await _lookupService.GetCountryListAsync())
                CountryList.Add(c);

            var email = _userService.CurrentUser;
            if (string.IsNullOrWhiteSpace(email)) return;

            await using var db = await _factory.CreateDbContextAsync();
            Item = await db.TblUserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            SimpleLog.Error("Benutzerprofil konnte nicht geladen werden.");
            SimpleLog.Log(ex);
            await _dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
        finally
        {
            IsBusy = false;
        }
    }


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
        if (Item is null) return;

        // nur das eigene Profil
        if (!string.Equals(Item.Email, _userService.CurrentUser, StringComparison.OrdinalIgnoreCase))
            return;

        if (!await _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message"))
            return;

        try
        {
            IsBusy = true;
            var saved = await _data.UpsertUserProfileAsync(Item);
            if (saved != null)
            {
                Item = saved;
                _lookupService.InvalidateAllCaches();
            }
        }
        catch (Exception ex)
        {
            SimpleLog.Error("Benutzerprofil konnte nicht gespeichert werden.");
            SimpleLog.Log(ex);
            await _dialog.ShowErrorAsync("Error_Save_Title", "Error_Save_Message");
        }
        finally
        {
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

