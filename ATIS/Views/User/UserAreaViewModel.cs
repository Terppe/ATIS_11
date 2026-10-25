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

            System.Diagnostics.Debug.WriteLine($"[UserArea] CurrentUser='{email}'");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserArea LoadAsync] {ex}");
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
            System.Diagnostics.Debug.WriteLine($"[UserArea SaveAsync] {ex}");
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
            System.Diagnostics.Debug.WriteLine($"[UserArea DeleteAsync] {ex}");
            await _dialog.ShowErrorAsync("Error_Delete_Title", "Error_Delete_Message");
        }
        finally
        {
            IsBusy = false;
        }
    }
}


//public partial class UserAreaViewModel : BaseBossListViewModel<Models.TblUserProfile>   //ObservableObject
//{

//    private readonly LookupService _lookupService;
//    public ICommand SortCommand { get; }
//    public IRelayCommand BackCommand { get; }
//    private int _currentParentId;
//    private bool _wasEmptyBeforeSave;
//    private Func<int, Task<IEnumerable<Models.TblUserProfile>>>? _activeLoader;
//    private readonly Action<string> _langHandler;
//    private readonly IUserService _userService;
//    private readonly IDbContextFactory<AtisDbContext> _factory;
//    private readonly IDataService _data;
//    private readonly IDialogService _dialog;
//    private readonly IUserService _user;

//    // ---------------------------------------------------------                   
//    // CTOR
//    // ---------------------------------------------------------                   
//    public UserAreaViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        HierarchyCoordinator coordinator,
//        LookupService lookupService,
//        IUserService user)
//        : base(new DataService(factory, user), dialog, coordinator)
//    {

//        _lookupService = lookupService;
//        _userService = user;

//        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
//        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

//        RegisterHeader("Id", "Id");
//        RegisterHeader("Name", "Name");

//        _langHandler = _ => ApplyHeaders();
//        App.LanguageChanged += _langHandler;

//        ApplyHeaders();

//        _factory = factory;
//        _data = new DataService(factory, user);
//        _dialog = dialog;
//        _lookupService = lookupService;
//        _user = user;

//    }

//    public async Task LoadReferenceLookupsAsync()
//    {
//        //await UserProfileReferenceViewModel.LoadReferenceLookupsAsync();
//    }


//    // ---------------------------------------------------------
//    // LOOKUPS
//    // ---------------------------------------------------------
//    public ObservableCollection<string> CountryNameList { get; private set; } = new();
//    public ObservableCollection<Models.TblCountry> CountryList { get; private set; } = new();
//    public Dictionary<int, string?> CountryLookup { get; private set; } = new();
//    public async Task LoadCountryLookupAsync()
//    {
//        try
//        {
//            System.Diagnostics.Debug.WriteLine("[VM] LoadCountryLookupAsync START");

//            var list = await _lookupService.GetCountryListAsync();
//            var lookup = await _lookupService.GetCountryLookupAsync();


//            CountryList.Clear();
//            foreach (var item in list)
//                CountryList.Add(item);

//            CountryLookup = lookup;
//            OnPropertyChanged(nameof(CountryLookup));
//            OnPropertyChanged(nameof(CountryList));
//        }
//        catch (Exception ex)
//        {
//            CountryList.Clear();
//            CountryLookup = new Dictionary<int, string?>();
//            OnPropertyChanged(nameof(CountryLookup));
//            OnPropertyChanged(nameof(CountryList));
//        }
//    }
//    // ---------------------------------------------------------                  
//    // SORTING HEADERS
//    // ---------------------------------------------------------    
//    private void ApplyHeaders()
//    {
//        var loc = Localizer.Get();
//        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_UserProfileId")) ? "Id" : loc.GetLocalizedString("HeaderSort_UserProfileId"));
//        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_UserProfileName")) ? "Name" : loc.GetLocalizedString("HeaderSort_UserProfileName"));
//    }

//    public void Dispose()
//    {
//        App.LanguageChanged -= _langHandler;
//    }

//    // ---------------------------------------------------------               
//    // UI STATE
//    // ---------------------------------------------------------                   

//    #region UI STATE
//    // ---------------------------------------------------------                   

//    private string? _email;
//    public string? Email { get => _email; set => SetProperty(ref _email, value); }

//    private string? _resetCode;
//    public string? ResetCode { get => _resetCode; set => SetProperty(ref _resetCode, value); }

//    private string? _newPassword;
//    public string? NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }

//    private string _status = string.Empty;
//    public string Status { get => _status; set => SetProperty(ref _status, value); }

//    private string _errorMessage = string.Empty;
//    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

//    private string? _role = "User";
//    public string? Role { get => _role; set => SetProperty(ref _role, value); }

//    private string? _password;
//    public string? Password { get => _password; set => SetProperty(ref _password, value); }

//    private bool _isBusy;
//    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

//    private bool _resetCodeSent; // steuert Stufe 2
//    public bool ResetCodeSent { get => _resetCodeSent; set => SetProperty(ref _resetCodeSent, value); }

//    private string? _currentPassword;
//    public string? CurrentPassword { get => _currentPassword; set => SetProperty(ref _currentPassword, value); }

//    private string _statusMessage = string.Empty;
//    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

//    private bool _canLogin;
//    public bool CanLogin { get => !_canLogin; set => SetProperty(ref _canLogin, !value); }

//    private bool _canRegister;
//    public bool CanRegister { get => !_canRegister; set => SetProperty(ref _canRegister, !value); }

//    public IAsyncRelayCommand<string?> LoginCommand { get; }
//    public IAsyncRelayCommand<string?> RegisterCommand { get; }
//    public IAsyncRelayCommand ForgotPasswordCommand { get; }
//    public IAsyncRelayCommand ChangePasswordCommand { get; }
//    public IAsyncRelayCommand ResetPasswordWithCodeCommand { get; }

//    #endregion

//    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
//    {
//        SelectedDetailTabIndex = index;
//    });

//    private int _selectedDetailTabIndex;
//    public int SelectedDetailTabIndex
//    {
//        get => _selectedDetailTabIndex;
//        set
//        {
//            if (SetProperty(ref _selectedDetailTabIndex, value))
//            {
//                _ = UpdateButtonStatesAsync();
//            }
//        }
//    }

//    private async Task UpdateButtonStatesAsync()
//    {


//        // Tab 0 = Boss-Modus
//        IsInParentMode = SelectedDetailTabIndex != 0;

//        //if (SelectedDetailTabIndex == 2)
//        //    await UserProfileReferenceViewModel.LoadReferenceLookupsAsync();

//        UpdateRefreshState();

//        NewCommand.NotifyCanExecuteChanged();
//        RefreshCommand.NotifyCanExecuteChanged();
//    }

//    public async Task ApplyFilterAsync(string? filter)
//    {
//        if (IsBossMode)
//        {
//            SearchText = filter;
//            await ReloadAsync();
//        }
//    }

//    public bool IsDetailVisible => SelectedItem != null;
//    public bool IsEditMode => SelectedItem != null;
//    public bool HasItems => Items != null && Items.Count > 0;

//    // ---------------------------------------------------------                  
//    // LOAD ASYNC (Boss)
//    // ---------------------------------------------------------                  
//    public async Task LoadAsync()
//    {
//        try
//        {
//            IsBusy = true;

//            CountryList.Clear();
//            foreach (var c in await _lookupService.GetCountryListAsync())
//                CountryList.Add(c);

//            var email = _userService.CurrentUser;
//            if (string.IsNullOrWhiteSpace(email)) return;

//            await using var db = await _factory.CreateDbContextAsync();
//            Item = await db.TblUserProfiles
//                .AsNoTracking()
//                .FirstOrDefaultAsync(u => u.Email == email);
//        }
//        catch (Exception ex)
//        {
//            System.Diagnostics.Debug.WriteLine($"[UserArea LoadAsync] {ex}");
//            await _dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
//        }
//        finally
//        {
//            IsBusy = false;
//        }


//        //// Boss-Start: Parent-Kontext löschen
//        //_activeLoader = null;
//        //_currentParentId = 0;

//        //// Für Simple-Page sinnvoll: Boss-Modus aktiv
//        //IsInParentMode = false;

//        //// Lookup nur versuchen, Fehler darf UI nicht stoppen
//        //try
//        //{

//        //    await LoadCountryLookupAsync();
//        //}
//        //catch (Exception ex)
//        //{
//        //    System.Diagnostics.Debug.WriteLine($"[LoadAsync -> LoadCountryLookupAsync] {ex}");
//        //    // absichtlich weiterlaufen
//        //}

//        //Items = new ObservableCollection<Models.TblUserProfile>();
//        //OnPropertyChanged(nameof(HasItems));

//        //// Hauptdaten trotzdem laden
//        //try
//        //{
//        //    await ReloadAsync();
//        //}
//        //catch (Exception ex)
//        //{
//        //    System.Diagnostics.Debug.WriteLine($"[LoadAsync -> ReloadAsync] {ex}");
//        //    // optional: Dialog zeigen
//        //    await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
//        //}
//    }

//    public TblUserProfile? Item { get; set; }

//    // ---------------------------------------------------------
//    // INTERNAL LOADER (Boss + Parent)
//    // ---------------------------------------------------------
//    private async Task LoadInternalAsync(
//        int parentId,
//        Func<int, Task<IEnumerable<Models.TblUserProfile>>> loader)
//    {
//        _currentParentId = parentId;
//        _activeLoader = loader;

//        var items = await loader(parentId);
//        Items = new ObservableCollection<Models.TblUserProfile>(items);
//        OnPropertyChanged(nameof(HasItems));

//        DataSetCount = Items.Count;
//    }

//    // ---------------------------------------------------------
//    // LOAD ITEMS (Boss)
//    // ---------------------------------------------------------
//    protected override async Task<IEnumerable<Models.TblUserProfile>> LoadItemsAsync()
//    {
//        if (_activeLoader != null && _currentParentId > 0)
//            return await _activeLoader(_currentParentId);

//        if (!string.IsNullOrWhiteSpace(SearchText))
//            return await Data.SearchUserProfileAsync(SearchText);

//        return Items ?? Enumerable.Empty<Models.TblUserProfile>();
//    }

//    // ---------------------------------------------------------
//    // SELECTED ITEM
//    // ---------------------------------------------------------
//    protected override async Task OnSelectedItemChangedAsync()
//    {
//        // 1. BACK → SelectedItem == null
//        if (SelectedItem == null)
//        {
//            OnPropertyChanged(nameof(IsDetailVisible));
//            //      await ClearChildCollectionsAsync();
//            SelectedDetailTabIndex = 0;
//            return;
//        }

//        // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
//        //if (IsInParentMode)
//        //{
//        //    await LoadChildrenAsync(GetId(SelectedItem));
//        //    return;
//        //}

//        // 3. Boss-Modus → volle Detail-Logik
//        OnPropertyChanged(nameof(IsDetailVisible));
//        await LoadChildrenAsync(GetId(SelectedItem));
//    }

//    // ---------------------------------------------------------
//    // CHILDREN
//    // ---------------------------------------------------------
//    protected override async Task LoadChildrenAsync(int entityId)
//    {
//        //await UserProfileCommentViewModel.LoadForUserProfileAsync(entityId);
//        //await UserProfileReferenceViewModel.LoadForUserProfileAsync(entityId);

//        //await NULLViewModel.LoadUserProfileLookupAsync();
//        //await NULLViewModel.LoadForUserProfileAsync(entityId);


//        //// ⭐ HIER MUSS DER MODUS GESETZT WERDEN
//        //NULLViewModel.IsInParentMode = true;
//    }

//    //protected override Task ClearChildCollectionsAsync()
//    //{
//    //    UserProfileCommentViewModel.Items?.Clear();
//    //    UserProfileReferenceViewModel.Items?.Clear();
//    //  //  NULLViewModel.Items?.Clear();

//    //    if (IsBossMode)
//    //    {
//    //        UserProfileCommentViewModel.SelectedItem = null;
//    //        UserProfileReferenceViewModel.SelectedItem = null;
//    //   //     NULLViewModel.SelectedItem = null;
//    //    }

//    //    return Task.CompletedTask;
//    //}

//    // ---------------------------------------------------------
//    // SORTING
//    // ---------------------------------------------------------
//    protected override object GetSortKey(Models.TblUserProfile item, string column)
//        => column switch
//        {
//            "Id" => item.UserProfileId,
//            "Name" => item.Email ?? "",
//            _ => ""
//        };


//    // ---------------------------------------------------------
//    // NEW
//    // ---------------------------------------------------------
//    protected override Task NewAsync()
//    {
//        return Task.CompletedTask;
//    }

//    protected override Models.TblUserProfile CreateNewItem()
//    {
//        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

//        return new Models.TblUserProfile
//        {
//            UserProfileId = _currentParentId
//        };
//    }

//    // ---------------------------------------------------------
//    // SAVE
//    // ---------------------------------------------------------
//    protected override async Task<Models.TblUserProfile?> SaveItemAsync(Models.TblUserProfile item)
//    {
//        if (string.IsNullOrWhiteSpace(item.Email))
//        {
//            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
//            return null;
//        }

//        var saved = await Data.UpsertUserProfileAsync(item);

//        if (saved != null)
//        {
//            _lookupService.InvalidateAllCaches();
//        }

//        return saved;
//    }

//    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblUserProfile item)
//    {
//        if (_wasEmptyBeforeSave && IsBossMode)
//        {
//            SearchText = item.Email;
//            IsInParentMode = false;
//        }

//        return Task.CompletedTask;
//    }

//    // ---------------------------------------------------------
//    // DELETE
//    // ---------------------------------------------------------
//    protected override async Task<bool> DeleteItemAsync(Models.TblUserProfile item)
//    {
//        if (item.UserProfileId == 0)
//            return false;

//        //if (await Data.UserProfileHasNULLAsync(item.UserProfileId) ||
//        //    await Data.UserProfileHasReferenceAsync(item.UserProfileId) ||
//        //    await Data.UserProfileHasCommentAsync(item.UserProfileId))
//        //{
//        //    await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
//        //    return false;
//        //}

//        await Data.DeleteUserProfileAsync(item.UserProfileId);
//        return true;
//    }

//    protected override Task<bool> ConfirmSaveAsync(Models.TblUserProfile item)
//        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

//    protected override Task<bool> ConfirmDeleteAsync(Models.TblUserProfile item)
//        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

//    protected override int GetId(Models.TblUserProfile item)
//        => item.UserProfileId;

//    // ---------------------------------------------------------
//    // BACK
//    // ---------------------------------------------------------
//    private void ExecuteBack()
//    {
//        // ⭐ Back MUSS SelectedItem löschen
//        SelectedItem = null;

//        // ⭐ Detail schließen
//        OnPropertyChanged(nameof(IsDetailVisible));

//        // Parent-Modus bleibt aktiv
//        IsInParentMode = true;

//        SelectedDetailTabIndex = 0;

//        NewCommand.NotifyCanExecuteChanged();
//        RefreshCommand.NotifyCanExecuteChanged();
//    }
//}



//public partial class UserAreaViewModel : ObservableObject
//{
//    private readonly IDbContextFactory<AtisDbContext> _factory;
//    private readonly IDataService _data;
//    private readonly IDialogService _dialog;
//    private readonly LookupService _lookupService;
//    private readonly IUserService _user;

//    public UserAreaViewModel(
//        IDbContextFactory<AtisDbContext> factory,
//        IDialogService dialog,
//        LookupService lookupService,
//        IUserService user)
//    {
//        _factory = factory;
//        _data = new DataService(factory, user);
//        _dialog = dialog;
//        _lookupService = lookupService;
//        _user = user;

//        SaveCommand = new AsyncRelayCommand(SaveAsync, () => Item != null && !IsBusy);
//    }

//    [ObservableProperty]
//    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
//    private Models.TblUserProfile? item;

//    [ObservableProperty]
//    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
//    private bool isBusy;

//    public ObservableCollection<Models.TblCountry> CountryList { get; } = new();

//    public IAsyncRelayCommand SaveCommand { get; }

//    public async Task LoadAsync()
//    {
//        try
//        {
//            IsBusy = true;

//            CountryList.Clear();
//            foreach (var c in await _lookupService.GetCountryListAsync())
//                CountryList.Add(c);

//            var email = _user.CurrentUser;
//            if (string.IsNullOrWhiteSpace(email)) return;

//            await using var db = await _factory.CreateDbContextAsync();
//            Item = await db.TblUserProfiles
//                .AsNoTracking()
//                .FirstOrDefaultAsync(u => u.Email == email);
//        }
//        catch (Exception ex)
//        {
//            System.Diagnostics.Debug.WriteLine($"[UserArea LoadAsync] {ex}");
//            await _dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
//        }
//        finally
//        {
//            IsBusy = false;
//        }
//    }

//    private async Task SaveAsync()
//    {
//        if (Item is null) return;

//        // Nur das eigene Profil darf gespeichert werden
//        if (!string.Equals(Item.Email, _user.CurrentUser, StringComparison.OrdinalIgnoreCase))
//            return;

//        if (!await _dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message"))
//            return;

//        try
//        {
//            IsBusy = true;
//            var saved = await _data.UpsertUserProfileAsync(Item);
//            if (saved != null)
//            {
//                Item = saved;
//                _lookupService.InvalidateAllCaches();
//            }
//        }
//        catch (Exception ex)
//        {
//            System.Diagnostics.Debug.WriteLine($"[UserArea SaveAsync] {ex}");
//            await _dialog.ShowErrorAsync("Error_Save_Title", "Error_Save_Message");
//        }
//        finally
//        {
//            IsBusy = false;
//        }
//    }
//}
