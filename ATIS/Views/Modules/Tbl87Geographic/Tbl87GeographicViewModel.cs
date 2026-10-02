using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.TblCountry;
using Microsoft.EntityFrameworkCore;
using WinUI3Localizer;

//    Tbl87GeographicViewModel Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public class Tbl87GeographicViewModel : BaseBossListViewModel<Models.Tbl87Geographic>
{

    private readonly LookupService _lookupService;
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }
    private int _currentParentId;
    private bool _wasEmptyBeforeSave;
    private Func<int, Task<IEnumerable<Models.Tbl87Geographic>>>? _activeLoader;
    private readonly Action<string> _langHandler;

    // ---------------------------------------------------------                   
    // CTOR
    // ---------------------------------------------------------                   
    public Tbl87GeographicViewModel(
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

    // ---------------------------------------------------------
    // LOOKUPS
    // ---------------------------------------------------------
    public ObservableCollection<string> CountryNameList { get; private set; } = new();
    public ObservableCollection<Models.TblCountry> CountryList { get; private set; } = new();
    public ObservableCollection<Models.Tbl75Species> SpeciesList { get; private set; } = new();
    public Dictionary<int, string?> CountryLookup { get; private set; } = new();
    public Dictionary<int, string?> SpeciesLookup { get; private set; } = new();

    public async Task LoadSpeciesLookupAsync()
    {
        try
        {
            var list = await _lookupService.GetSpeciesListAsync();
            var lookup = await _lookupService.GetSpeciesLookupAsync();

            SpeciesList.Clear();
            foreach (var item in list)
                SpeciesList.Add(item);

            SpeciesLookup = lookup;
            OnPropertyChanged(nameof(SpeciesLookup));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadSpeciesLookupAsync] {ex}");

            SpeciesList.Clear();
            SpeciesLookup = new Dictionary<int, string?>();
            OnPropertyChanged(nameof(SpeciesLookup));
        }
    }
    public async Task LoadCountryLookupAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[VM] LoadCountryLookupAsync START");

            var list = await _lookupService.GetCountryListAsync();
            var lookup = await _lookupService.GetCountryLookupAsync();

            System.Diagnostics.Debug.WriteLine($"[VM] list count = {list?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"[VM] lookup count = {lookup?.Count ?? 0}");

            CountryList.Clear();
            foreach (var item in list)
                CountryList.Add(item);

            CountryLookup = lookup;
            OnPropertyChanged(nameof(CountryLookup));
            OnPropertyChanged(nameof(CountryList));

            System.Diagnostics.Debug.WriteLine($"[VM] CountryList final = {CountryList.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadCountryLookupAsync] {ex}");

            CountryList.Clear();
            CountryLookup = new Dictionary<int, string?>();
            OnPropertyChanged(nameof(CountryLookup));
            OnPropertyChanged(nameof(CountryList));
        }
    }
    //public async Task LoadCountryLookupAsync()
    //{
    //    try
    //    {
    //        var list = await _lookupService.GetCountryListAsync();
    //        var lookup = await _lookupService.GetCountryLookupAsync();

    //        CountryList.Clear();
    //        foreach (var item in list)
    //            CountryList.Add(item);

    //        CountryLookup = lookup;
    //        OnPropertyChanged(nameof(CountryLookup));
    //    }
    //    catch (Exception ex)
    //    {
    //        System.Diagnostics.Debug.WriteLine($"[LoadCountryLookupAsync] {ex}");

    //        CountryList.Clear();
    //        CountryLookup = new Dictionary<int, string?>();
    //        OnPropertyChanged(nameof(CountryLookup));
    //    }
    //}

    // ---------------------------------------------------------                  
    // SORTING HEADERS
    // ---------------------------------------------------------    
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_GeographicId")) ? "Id" : loc.GetLocalizedString("HeaderSort_GeographicId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_GeographicName")) ? "Name" : loc.GetLocalizedString("HeaderSort_GeographicName"));
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
        _activeLoader = null;
        _currentParentId = 0;
        IsInParentMode = false;

        try
        {
            System.Diagnostics.Debug.WriteLine("[LoadAsync] before species");
            await LoadSpeciesLookupAsync();
            System.Diagnostics.Debug.WriteLine("[LoadAsync] after species");

            System.Diagnostics.Debug.WriteLine("[LoadAsync] before country");
            await LoadCountryLookupAsync();
            System.Diagnostics.Debug.WriteLine("[LoadAsync] after country");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> Lookup] {ex}");
        }

        Items = new ObservableCollection<Models.Tbl87Geographic>();
        OnPropertyChanged(nameof(HasItems));

        try
        {
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoadAsync -> ReloadAsync] {ex}");
            await Dialog.ShowErrorAsync("Error_Load_Title", "Error_Load_Message");
        }
    }
    // ---------------------------------------------------------
    // LOAD FOR VK 
    // ---------------------------------------------------------
    public async Task LoadForSpeciesAsync(int speciesId)
        => await LoadInternalAsync(speciesId, Data.GetGeographicForSpeciesAsync);

    // ---------------------------------------------------------
    // INTERNAL LOADER (Boss + Parent)
    // ---------------------------------------------------------
    private async Task LoadInternalAsync(
        int parentId,
        Func<int, Task<IEnumerable<Models.Tbl87Geographic>>> loader)
    {
        _currentParentId = parentId;
        _activeLoader = loader;

        var items = await loader(parentId);
        Items = new ObservableCollection<Models.Tbl87Geographic>(items);
        OnPropertyChanged(nameof(HasItems));

        DataSetCount = Items.Count;
        // Liste anzeigen, Editor erst nach Auswahl eines Eintrags.
        SelectedItem = null;
        Item = null;
    }

    // ---------------------------------------------------------
    // LOAD ITEMS (Boss)
    // ---------------------------------------------------------
    protected override async Task<IEnumerable<Models.Tbl87Geographic>> LoadItemsAsync()
    {
        if (_activeLoader != null && _currentParentId > 0)
            return await _activeLoader(_currentParentId);

        if (!string.IsNullOrWhiteSpace(SearchText))
            return await Data.SearchGeographicWithSpeciesAndGenusAsync(SearchText);

        return Items ?? Enumerable.Empty<Models.Tbl87Geographic>();
    }

    // ---------------------------------------------------------
    // SELECTED ITEM
    // ---------------------------------------------------------

    protected override async Task OnSelectedItemChangedAsync()
    {
        var selected = SelectedItem;
        Item = null;

        if (selected != null)
        {
            await EnsureCountryLookupLoadedAsync();
        }

        Item = selected;
    }

    private bool _countryLookupLoaded;

    public async Task EnsureCountryLookupLoadedAsync()
    {
        if (_countryLookupLoaded && CountryList.Count > 0)
            return;

        await LoadCountryLookupAsync();
        _countryLookupLoaded = CountryList.Count > 0;
    }

    // ---------------------------------------------------------
    // CHILDREN
    // ---------------------------------------------------------
    protected override Task LoadChildrenAsync(int entityId)
    {
        return Task.CompletedTask;
    }
    protected override Task ClearChildCollectionsAsync()
    {
        return Task.CompletedTask;
    }
    
    private Models.Tbl87Geographic? _item;

    public Models.Tbl87Geographic? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasActiveItem));
            }
        }
    }
    public bool HasActiveItem => Item != null;

    public void PrepareViewState()
    {
        // Keine Auswahl: EditControl schließen
        if (SelectedItem == null)
        {
            Item = null;
            SelectedDetailTabIndex = 0;
            OnPropertyChanged(nameof(IsDetailVisible));
        }
        else
        {
            // Falls bereits ein Datensatz ausgewählt ist,
            // EditControl mit diesem Datensatz synchronisieren.
            Item = SelectedItem;
        }

        UpdateRefreshState();
        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    // ---------------------------------------------------------
    // SORTING
    // ---------------------------------------------------------
    protected override object GetSortKey(Models.Tbl87Geographic item, string column)
        => column switch
        {
            "Id" => item.GeographicId,
            "Name" => item.Info ?? "",
            _ => ""
        };


    // ---------------------------------------------------------
    // NEW
    // ---------------------------------------------------------
    protected override Task NewAsync()
    {
        return Task.CompletedTask;
    }

    protected override Models.Tbl87Geographic CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl87Geographic
        {
            SpeciesId = _currentParentId
        };
    }

    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl87Geographic?> SaveItemAsync(Models.Tbl87Geographic item)
    {
        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_RecordExists_Title", "Error_RecordExists_Message");
            return null;
        }

        var saved = await Data.UpsertGeographicAsync(item);

        if (saved != null)
        {
            _lookupService.InvalidateAllCaches();
        }

        return saved;
    }

    protected override Task OnBeforeReloadAfterSaveAsync(Models.Tbl87Geographic item)
    {
        if (_wasEmptyBeforeSave && IsBossMode)
        {
            SearchText = item.Info;
            IsInParentMode = false;
        }

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // DELETE
    // ---------------------------------------------------------
    protected override async Task<bool> DeleteItemAsync(Models.Tbl87Geographic item)
    {
        if (item.GeographicId == 0)
            return false;

        await Data.DeleteGeographicAsync(item.GeographicId);
        return true;
    }

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl87Geographic item)
        => Dialog.ShowYesNoAsync("Confirm_Save_Title", "Confirm_Save_Message");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl87Geographic item)
        => Dialog.ShowYesNoAsync("Confirm_Delete_Title", "Confirm_Delete_Message");

    protected override int GetId(Models.Tbl87Geographic item)
        => item.GeographicId;

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        // SelectedItem zurücksetzen -> Detail schließen
        SelectedItem = null;
        OnPropertyChanged(nameof(IsDetailVisible));

        // Bei Simple-Page NICHT hart auf true setzen
        // IsInParentMode = true;

        SelectedDetailTabIndex = 0;

        NewCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }
}
