using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    Tbl87GeographicEditControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public sealed partial class Tbl87GeographicEditControl : UserControl
{
    #region Nested types / constants

    private static readonly Dictionary<string, (double Latitude, double Longitude, double ZoomLevel)> _continentDefaults
        = new()
        {
            ["Africa"] = (1, 20, 3.5),
            ["Antarctica"] = (-82, 0, 3),
            ["Asia"] = (34, 100, 3.5),
            ["Australia"] = (-25, 133, 4),
            ["Central/South America"] = (-15, -60, 4.6),
            ["Europe"] = (54, 15, 4.5),
            ["North America/Caribbean"] = (40, -100, 4.4)
        };

    private void GetContinentDefaults(string? continent, out double latitude, out double longitude, out double zoomLevel)
    {
        if (!string.IsNullOrWhiteSpace(continent) && _continentDefaults.TryGetValue(continent, out var value))
        {
            latitude = value.Latitude;
            longitude = value.Longitude;
            zoomLevel = value.ZoomLevel;
            return;
        }

        latitude = 20;
        longitude = 0;
        zoomLevel = 3;
    }

    #endregion

    #region Fields / state
    // ---------------------------------------------------------

    private bool _isInitializingCombos;
    private bool _isApplyingItem;
    private bool _mapFallbackActive;
    private bool _mapEditedByUser;

    #endregion

    #region small Helpers

    [System.Diagnostics.Conditional("DEBUG")]
    private void LogDebug(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    private void LogGuard(string source, string reason)
    {
        System.Diagnostics.Debug.WriteLine($"{source}: skip ({reason})");
    }

    #endregion

    #region ctor / loaded
    // ---------------------------------------------------------
    public Tbl87GeographicViewModel? ViewModel => DataContext as Tbl87GeographicViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl87GeographicEditControl()
    {
        InitializeComponent();
    }

    #endregion

    private Tbl87GeographicMapWindow? _mapWindow;

    private void OpenMapButton_Click(object sender, RoutedEventArgs e)
    {
        if (Item == null)
            return;

        if (_mapWindow != null)
        {
            _mapWindow.Activate();
            return;
        }
        LogDebug($"OpenMapButton_Click: id={Item?.GeographicId}, lat={Item?.Latitude}, lon={Item?.Longitude}, zoom={Item?.ZoomLevel}");
        _mapWindow = new Tbl87GeographicMapWindow(Item);
        _mapWindow.MapStateCommitted += MapWindow_MapStateCommitted;
        _mapWindow.Closed += MapWindow_Closed;
        _mapWindow.Activate();
    }
    private void MapWindow_MapStateCommitted(object? sender, EventArgs e)
    {
        _mapEditedByUser = true;
        _mapFallbackActive = false;
    }

    private void MapWindow_Closed(object sender, WindowEventArgs args)
    {
        if (_mapWindow != null)
        {
            _mapWindow.MapStateCommitted -= MapWindow_MapStateCommitted;
            _mapWindow.Closed -= MapWindow_Closed;
            _mapWindow = null;
        }
    }

    private bool HasStoredPoints(Models.Tbl87Geographic? item)
    {
        if (item == null)
            return false;

        // Erst ab dem zweiten Punkt als "echte gespeicherte Punkte" behandeln
        return
            !(item.Latitude1 == 0 && item.Longitude1 == 0) ||
            !(item.Latitude2 == 0 && item.Longitude2 == 0) ||
            !(item.Latitude3 == 0 && item.Longitude3 == 0);
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {

        LogDebug($"SaveButton_Click: itemId={Item?.GeographicId}, zoom={Item?.ZoomLevel}");
    }

    private void ContinentCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HasStoredPoints(Item))
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "stored points already exist");
            return;
        }
        if (_isInitializingCombos)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "initializing");
            return;
        }

        if (_isApplyingItem)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "applying item");
            return;
        }

        if (ViewModel == null || Item == null)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "view model or item is null");
            return;
        }

        if (_mapEditedByUser)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "map already edited by user");
            return;
        }

        var continent = (sender as ComboBox)?.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(continent))
            return;

        Item.Continent = continent;

        // Nur grober Fallback
        GetContinentDefaults(continent, out var latitude, out var longitude, out var zoomLevel);

        // Continent nur anwenden, wenn noch kein Country gewählt ist
        if (string.IsNullOrWhiteSpace(Item.Country))
        {
            Item.Latitude = latitude;
            Item.Longitude = longitude;

            if (Item.ZoomLevel <= 0 || _mapFallbackActive)
                Item.ZoomLevel = zoomLevel;

            _mapFallbackActive = true;
        }
    }
    private void CountryCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HasStoredPoints(Item))
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "stored points already exist");
            return;
        }
        if (_isInitializingCombos)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "initializing");
            return;
        }

        if (_isApplyingItem)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "applying item");
            return;
        }

        if (ViewModel == null || Item == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "view model or item is null");
            return;
        }

        if (_mapEditedByUser)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "map already edited by user");
            return;
        }

        var selectedCountry = (sender as ComboBox)?.SelectedItem as Models.TblCountry;
        if (selectedCountry == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "selected country is null");
            return;
        }

        Item.Country = selectedCountry.Name;

        // Country ist präziser als Continent und darf Fallback überschreiben
        Item.Latitude = selectedCountry.Latitude;
        Item.Longitude = selectedCountry.Longitude;

        Item.ZoomLevel = selectedCountry.ZoomLevel.HasValue && selectedCountry.ZoomLevel.Value > 0
            ? selectedCountry.ZoomLevel.Value
            : 5;

        _mapFallbackActive = true;
    }


    #region DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------
    public Models.Tbl87Geographic? Item
    {
        get => (Models.Tbl87Geographic?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl87Geographic),
            typeof(Tbl87GeographicEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl87GeographicEditControl)d;
        var item = e.NewValue as Models.Tbl87Geographic;

        control.LogDebug(
            $"OnItemChanged: oldId={(e.OldValue as Models.Tbl87Geographic)?.GeographicId}, newId={item?.GeographicId}");

        control.ApplyItem(item);
    }
    #endregion

    #region APPLY ITEM
    // ---------------------------------------------------------
    //
    private void ResetMapFallbackFlags()
    {
        _mapFallbackActive = false;
        _mapEditedByUser = false;
    }
    private async void ApplyItem(Models.Tbl87Geographic? newItem)
    {
        LogDebug($"ApplyItem: itemId={newItem?.GeographicId}");
        LogDebug($"ApplyItem values: id={newItem?.GeographicId}, lat={newItem?.Latitude}, lon={newItem?.Longitude}, zoom={newItem?.ZoomLevel}, country={newItem?.Country}, continent={newItem?.Continent}");

        if (newItem == null)
        {
            ResetMapFallbackFlags();
            LogGuard(nameof(ApplyItem), "newItem is null");
            return;
        }

        if (newItem.GeographicId == 0)
        {
            ResetMapFallbackFlags();
        }

        _isApplyingItem = true;
        _isInitializingCombos = true;

        try
        {
            // aktuell keine Map-Logik mehr im EditControl
        }
        finally
        {
            _isInitializingCombos = false;
            _isApplyingItem = false;
        }
    }
    #endregion

    #region VALIDATION HELPER
    // ---------------------------------------------------------
    private void UpdateValidationState(TextBox box, bool hasError)
    {
        if (box == null)
            return;

        if (hasError)
        {
            VisualStateManager.GoToState(box, "Invalid", true);
            return;
        }

        if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
            VisualStateManager.GoToState(box, "Focused", true);
        else
            VisualStateManager.GoToState(box, "Normal", true);
    }
    #endregion

    #region VALID YEAR VALIDATION
    // ---------------------------------------------------------                   
    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }
    #endregion

    #region AUTHOR YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }

    private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }
    #endregion

    #region HTTP VALIDATION
    // ---------------------------------------------------------                   

    private void HttpBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(HttpBox, Item?.HasHttpError ?? false);
    }

    private void HttpBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(HttpBox, Item?.HasHttpError ?? false);
    }
    #endregion
}
