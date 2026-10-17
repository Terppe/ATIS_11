using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Mapsui.Projections;
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
    public event Action? RequestMapReset;
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

    public static class ContinentLookup
    {
        public static string[] Split(string? value) =>
            string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        public static bool Matches(string? countryContinents, string? itemContinent)
        {
            var allowed = Split(countryContinents);

            if (allowed.Length == 0 || string.IsNullOrWhiteSpace(itemContinent))
                return true;

            return allowed.Contains(itemContinent, StringComparer.OrdinalIgnoreCase);
        }
    }
    private void ApplyHeaders()
    {
        var loc = Localizer.Get();
        UpdateHeader("Id", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_GeographicId")) ? "Id" : loc.GetLocalizedString("HeaderSort_GeographicId"));
        UpdateHeader("Name", string.IsNullOrWhiteSpace(loc.GetLocalizedString("HeaderSort_GeographicInfo")) ? "Name" : loc.GetLocalizedString("HeaderSort_GeographicInfo"));
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
            await LoadSpeciesLookupAsync();

            await LoadCountryLookupAsync();
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
            if (ReferenceEquals(_item, value))
                return;

            if (_item != null)
                _item.PropertyChanged -= Item_PropertyChanged;

            if (SetProperty(ref _item, value))
            {
                if (_item != null)
                    _item.PropertyChanged += Item_PropertyChanged;

                OnPropertyChanged(nameof(HasActiveItem));
                OnPropertyChanged(nameof(CanSave));
                OnPropertyChanged(nameof(CanDelete));
            }
        }
    }
    public bool HasActiveItem => Item != null;
    public bool CanSave
    {
        get
        {
            Debug.WriteLine($"[Geo] CanSave => Item null? {Item == null}");
            return Item != null && !string.IsNullOrWhiteSpace(Item.Info);
        }
    }

    public bool CanDelete
    {
        get
        {
            Debug.WriteLine($"[Geo] CanDelete => Item null? {Item == null}");
            return Item != null && Item.GeographicId != 0;
        }
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Models.Tbl87Geographic.Info))
            OnPropertyChanged(nameof(CanSave));
    }

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
        RequestMapReset?.Invoke();
        return Task.CompletedTask;
    }
    protected override Models.Tbl87Geographic CreateNewItem()
    {
        _wasEmptyBeforeSave = Items == null || Items.Count == 0;

        return new Models.Tbl87Geographic
        {
            SpeciesId = _currentParentId,
            Latitude = 20,
            Longitude = 0,
            ZoomLevel = 3
        };
    }
    // ---------------------------------------------------------
    // Points sortieren vor Save
    // ---------------------------------------------------------
    private class TempPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    private List<TempPoint> OrderPointsForPolygon(List<TempPoint> points)
    {
        if (points == null || points.Count < 3)
            return points ?? new List<TempPoint>();

        var projected = points
            .Select(p =>
            {
                var xy = SphericalMercator.FromLonLat(p.Longitude, p.Latitude);
                return new
                {
                    Point = p,
                    X = xy.x,
                    Y = xy.y
                };
            })
            .ToList();

        var centerX = projected.Average(p => p.X);
        var centerY = projected.Average(p => p.Y);

        return projected
            .OrderBy(p => Math.Atan2(p.Y - centerY, p.X - centerX))
            .Select(p => p.Point)
            .ToList();
    }

    private bool ArePointsEqual(List<TempPoint> a, List<TempPoint> b)
    {
        if (a == null || b == null || a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Latitude != b[i].Latitude || a[i].Longitude != b[i].Longitude)
                return false;
        }

        return true;
    }

    private void NormalizePolygonPointsBeforeSave(Models.Tbl87Geographic item)
    {
        if (item == null)
            return;

        var points = new List<TempPoint>();

        if (!(item.Latitude == 0 && item.Longitude == 0))
            points.Add(new TempPoint { Latitude = item.Latitude, Longitude = item.Longitude });

        if (!(item.Latitude1 == 0 && item.Longitude1 == 0))
            points.Add(new TempPoint { Latitude = item.Latitude1, Longitude = item.Longitude1 });

        if (!(item.Latitude2 == 0 && item.Longitude2 == 0))
            points.Add(new TempPoint { Latitude = item.Latitude2, Longitude = item.Longitude2 });

        if (!(item.Latitude3 == 0 && item.Longitude3 == 0))
            points.Add(new TempPoint { Latitude = item.Latitude3, Longitude = item.Longitude3 });

        if (points.Count != 4)
            return;

        var ordered = OrderPointsForPolygon(points);

        if (!ArePointsEqual(points, ordered))
        {
            System.Diagnostics.Debug.WriteLine("Polygon normalized before save.");
        }

        item.Latitude = ordered[0].Latitude;
        item.Longitude = ordered[0].Longitude;

        item.Latitude1 = ordered[1].Latitude;
        item.Longitude1 = ordered[1].Longitude;

        item.Latitude2 = ordered[2].Latitude;
        item.Longitude2 = ordered[2].Longitude;

        item.Latitude3 = ordered[3].Latitude;
        item.Longitude3 = ordered[3].Longitude;
    }
    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------
    protected override async Task<Models.Tbl87Geographic?> SaveItemAsync(Models.Tbl87Geographic item)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[Tbl87GeographicVM] SaveItemAsync: id={item?.GeographicId}, zoom={item?.ZoomLevel}");

        if (string.IsNullOrWhiteSpace(item.Info))
        {
            await Dialog.ShowErrorAsync("Error_MissInfo_Title", "Error_MissInfo_Message");
            return null;
        }

        if (!string.IsNullOrWhiteSpace(item.Country) && !string.IsNullOrWhiteSpace(item.Continent))
        {
            var country = CountryList.FirstOrDefault(c =>
                string.Equals(c.Name, item.Country, StringComparison.OrdinalIgnoreCase));

            if (country != null && !ContinentLookup.Matches(country.Continent, item.Continent))
            {
                var proceed = await Dialog.ShowYesNoAsync(
                    "Warning_CountryContinent_Title",
                    "Warning_CountryContinent_Message");

                if (!proceed)
                    return null;
            }
        }

        NormalizePolygonPointsBeforeSave(item);

        var saved = await Data.UpsertGeographicAsync(item);

        System.Diagnostics.Debug.WriteLine(
            $"[Tbl87GeographicVM] SaveItemAsync AFTER Upsert: id={saved?.GeographicId}, zoom={saved?.ZoomLevel}");

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
