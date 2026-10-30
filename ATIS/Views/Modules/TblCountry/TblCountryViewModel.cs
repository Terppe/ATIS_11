using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    TblCountryViewModel Skriptdatum:   01.10.2026 12:32      

namespace ATIS.Views.Modules.TblCountry;

public class TblCountryViewModel : BaseBossListViewModel<Models.TblCountry>
{
    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.TblCountry>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public TblCountryViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {

     //   _lookupService = lookupService;

        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);

        RegisterHeader("Id", "Id");
        RegisterHeader("Name", "Name");

        _langHandler = _ => ApplyHeaders();
        App.LanguageChanged += _langHandler;

        ApplyHeaders();
    }
    
    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_CountryId")) ? "Id" : loc.GetLocalizedString("HeaderSort_CountryId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_CountryName")) ? "Name" : loc.GetLocalizedString("HeaderSort_CountryName"));
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

    protected override void OnSelectedDetailTabIndexChanged()
    {
        _ = UpdateButtonStatesAsync();
    }

    private async Task UpdateButtonStatesAsync()
    {
        // Tab 0 = Boss-Modus
        IsInParentMode = SelectedDetailTabIndex != 0;

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

        }
        catch (Exception ex)
        {
            // absichtlich weiterlaufen
        }

        Items = new ObservableCollection<Models.TblCountry>();
        OnPropertyChanged(nameof(HasItems));

        // Hauptdaten trotzdem laden
        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
    }

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.TblCountry>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.TblCountry>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.TblCountry>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchCountryAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.TblCountry>();
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
            await ClearChildCollectionsAsync();
            SelectedDetailTabIndex = 0;
            return;
        }

        // 2. Parent-Modus → Kinder laden, aber KEIN UI-Rebuild
        if (IsInParentMode)
        {
            await LoadChildrenAsync(GetId(SelectedItem));
            return;
        }

        // 3. Boss-Modus → volle Detail-Logik
        OnPropertyChanged(nameof(IsDetailVisible));
        await LoadChildrenAsync(GetId(SelectedItem));
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override  Task LoadChildrenAsync(int entityId)
    {
        return Task.CompletedTask;
    }

    protected override Task ClearChildCollectionsAsync()
    {
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.TblCountry item, string column)
        => column switch
        {
            "Id" => item.CountryId,
            "Name" => item.Name ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.TblCountry CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.TblCountry
        {
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.TblCountry?> SaveItemAsync(Models.TblCountry item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertCountryAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.TblCountry item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.Name;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.TblCountry item)
    {
        if (item.CountryId == 0)
            return false;

        await Data.DeleteCountryAsync(item.CountryId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.TblCountry item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.TblCountry item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.TblCountry item)
        => item.CountryId;

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
