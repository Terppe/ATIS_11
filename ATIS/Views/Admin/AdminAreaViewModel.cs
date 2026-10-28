using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using ATIS.Helpers;
using ATIS.Infrastructure;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using log4net;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.System;
using WinUI3Localizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WinUI3Localizer.LanguageDictionary;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace ATIS.Views.Admin;

public partial class AdminAreaViewModel : BaseBossListViewModel<Models.TblUserProfile>
{

    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }
    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.TblUserProfile>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public AdminAreaViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

        _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();
    }

    public async Task LoadReferenceLookupsAsync()
    {
        //await UserProfileReferenceViewModel.LoadReferenceLookupsAsync();
    }

    // ---------------------------------------------------------
    // CHILD-VMs
    // ---------------------------------------------------------
    //public Tbl93CommentViewModel UserProfileCommentViewModel { get; }
    //public Tbl90ReferenceViewModel UserProfileReferenceViewModel { get; }

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<string> CountryNameList { get; private set; } = new();
    public ObservableCollection<Models.TblCountry> CountryList { get; private set; } = new();
    public Dictionary<int, string?> CountryLookup { get; private set; } = new();
    public async Task LoadCountryLookupAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[VM] LoadCountryLookupAsync START");

            var list = await _lookupService.GetCountryListAsync();
            var lookup = await _lookupService.GetCountryLookupAsync();


            CountryList.Clear();
            foreach (var item in list)
                CountryList.Add(item);

            CountryLookup = lookup;
            OnPropertyChanged(nameof(CountryLookup));
            OnPropertyChanged(nameof(CountryList));
        }
        catch (Exception ex)
        {
            CountryList.Clear();
            CountryLookup = new Dictionary<int, string?>();
            OnPropertyChanged(nameof(CountryLookup));
            OnPropertyChanged(nameof(CountryList));
        }
    }
    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_UserProfileId")) ? "Id" : loc.GetLocalizedString("HeaderSort_UserProfileId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_UserProfileName")) ? "Name" : loc.GetLocalizedString("HeaderSort_UserProfileName"));
    }

    public void Dispose()
    {
        App.LanguageChanged -= _langHandler;
    }

    // ---------------------------------------------------------               
    // UI STATE
    // ---------------------------------------------------------                   

    public ICommand SelectDetailTabCommand => new CommunityToolkit.Mvvm.Input.RelayCommand<int>(index =>
    {
        SelectedDetailTabIndex = index;
    });

    private int _selectedDetailTabIndex;
    public int SelectedDetailTabIndex
    {
        get => _selectedDetailTabIndex;
        set
        {
            if (SetProperty(ref _selectedDetailTabIndex, value))
            {
                _ = UpdateButtonStatesAsync();
            }
        }
    }

    private async Task UpdateButtonStatesAsync()
    {


        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

        //if (SelectedDetailTabIndex == 2)
        //    await UserProfileReferenceViewModel.LoadReferenceLookupsAsync();

        UpdateRefreshState();

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    public async Task ApplyFilterAsync(string? filter)
    {
        if (IsBossMode)
        {
            SearchText = filter;
            await ReloadAsync();
        }
    }

    public bool IsDetailVisible => SelectedItem != null;
    public bool IsEditMode => SelectedItem != null;
    public bool HasItems => Items != null && Items.Count > 0;

    // ---------------------------------------------------------                  
    // LOAD ASYNC (Boss)
    // ---------------------------------------------------------                  
    public async Task LoadAsync()
    {

        // Boss-Start: Parent-Kontext löschen
        _activeLoader = null;
        _currentParentId = 0;

        // Für Simple-Page sinnvoll: Boss-Modus aktiv
        IsInParentMode = false;

        // Lookup nur versuchen, Fehler darf UI nicht stoppen
        try
        {

            await LoadCountryLookupAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> LoadCountryLookupAsync] {ex}");
            // absichtlich weiterlaufen
        }

        Items = new ObservableCollection<Models.TblUserProfile>();
        OnPropertyChanged(nameof(HasItems));

        // Hauptdaten trotzdem laden
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> ReloadAsync] {ex}");
            // optional: Dialog zeigen
            await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
    }

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.TblUserProfile>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.TblUserProfile>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.TblUserProfile>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchUserProfileAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.TblUserProfile>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------
    protected override async Task OnSelectedItemChangedAsync()
    {
        // 1. BACK → SelectedItem == null
        if (SelectedItem == null)
        {
            OnPropertyChanged(nameof(IsDetailVisible));
            //      await ClearChildCollectionsAsync();
            SelectedDetailTabIndex = 0;
            return;
        }

        // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
        //if (IsInParentMode)
        //{
        //    await LoadChildrenAsync(GetId(SelectedItem));
        //    return;
        //}

        // 3. Boss-Modus → volle Detail-Logik
        OnPropertyChanged(nameof(IsDetailVisible));
        await LoadChildrenAsync(GetId(SelectedItem));
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        //await UserProfileCommentViewModel.LoadForUserProfileAsync(entityId);
        //await UserProfileReferenceViewModel.LoadForUserProfileAsync(entityId);

        //await NULLViewModel.LoadUserProfileLookupAsync();
        //await NULLViewModel.LoadForUserProfileAsync(entityId);


        //// ⭐ HIER MUSS DER MODUS GESETZT WERDEN
        //NULLViewModel.IsInParentMode = true;
    }

    //protected override Task ClearChildCollectionsAsync()
    //{
    //    UserProfileCommentViewModel.Items?.Clear();
    //    UserProfileReferenceViewModel.Items?.Clear();
    //  //  NULLViewModel.Items?.Clear();

    //    if (IsBossMode)
    //    {
    //        UserProfileCommentViewModel.SelectedItem = null;
    //        UserProfileReferenceViewModel.SelectedItem = null;
    //   //     NULLViewModel.SelectedItem = null;
    //    }

    //    return Task.CompletedTask;
    //}

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.TblUserProfile item, string column)
        => column switch
        {
            "Id" => item.UserProfileId,
            "Name" => item.Email ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.TblUserProfile CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.TblUserProfile
        {
            UserProfileId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.TblUserProfile?> SaveItemAsync(Models.TblUserProfile item)
    {
        if (string.IsNullOrWhiteSpace(item.Email))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertUserProfileAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblUserProfile item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.Email;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.TblUserProfile item)
    {
        if (item.UserProfileId == 0)
            return false;

        //if (await Data.UserProfileHasNULLAsync(item.UserProfileId) ||
        //    await Data.UserProfileHasReferenceAsync(item.UserProfileId) ||
        //    await Data.UserProfileHasCommentAsync(item.UserProfileId))
        //{
        //    await Dialog.ShowErrorAsync("Error_DeleteNotAllowed_Title", "Error_DeleteNotAllowed_Message");
        //    return false;
        //}

        await Data.DeleteUserProfileAsync(item.UserProfileId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.TblUserProfile item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.TblUserProfile item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.TblUserProfile item)
        => item.UserProfileId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        // ⭐ Back MUSS SelectedItem löschen
        SelectedItem = null;

        // ⭐ Detail schließen
        OnPropertyChanged(nameof(IsDetailVisible));

        // Parent-Modus bleibt aktiv
        IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
