
using Mapsui;
using Mapsui.Projections;
using Mapsui.Tiling;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    Tbl87GeographicEditControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public sealed partial class Tbl87GeographicEditControl : UserControl
{
    public Tbl87GeographicViewModel? ViewModel => DataContext as Tbl87GeographicViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl87GeographicEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl87GeographicEditControl_Loaded;
    }
    private async void Tbl87GeographicEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();

        InitializeMap();
        await ApplyStartViewAsync();
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
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

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private async void ApplyItem(Models.Tbl87Geographic? newItem)
    {
        // ⭐ WICHTIG:
        // KEIN DataContext-Wechsel!
        // KEIN Boss-Workflow!
        // KEIN Parent-Workflow!
        //
        // Bindings im XAML zeigen direkt auf Item.XXX
        // Deshalb bleibt diese Methode LEER.

        if (!_mapInitialized || newItem == null)
            return;

        await ApplyStartViewAsync();
    }

    // ---------------------------------------------------------
    // MAP Mapsui
    // ---------------------------------------------------------

    private bool _mapInitialized;

    private async void InitializeMap()
    {
        if (_mapInitialized)
            return;

        var map = new Mapsui.Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        MyMap.Map = map;
        _mapInitialized = true;

        await ApplyStartViewAsync();
    }

    private async Task ApplyStartViewAsync()
    {
        if (!_mapInitialized || MyMap.Map == null)
            return;

        var (lat, lon, level) = GetBestStartView();
        var mercator = SphericalMercator.FromLonLat(lon, lat);
        var center = new MPoint(mercator.x, mercator.y);

        // Nach dem ersten Layout/Render etwas warten, sonst greift Startup-Navigation oft nicht sichtbar
        await Task.Delay(200);

        MyMap.Map.Navigator.CenterOn(center);
        MyMap.Map.Navigator.ZoomToLevel(level);
    }
    //private void ApplyHomeView()
    //{
    //    if (!_mapInitialized || MyMap.Map == null)
    //        return;

    //    var (lat, lon, level) = GetBestStartView();

    //    var mercator = SphericalMercator.FromLonLat(lon, lat);
    //    var center = new MPoint(mercator.x, mercator.y);

    //    MyMap.Map.Home = navigator =>
    //    {
    //        navigator.CenterOn(center);
    //        navigator.ZoomToLevel(level);
    //    };

    //    MyMap.Map.Home?.Invoke(MyMap.Map.Navigator);
    //}

    private (double lat, double lon, int level) GetBestStartView()
    {
        if (Item != null && ViewModel != null && !string.IsNullOrWhiteSpace(Item.Country))
        {
            var country = ViewModel.CountryList
                .FirstOrDefault(c => string.Equals(c.Name, Item.Country, StringComparison.OrdinalIgnoreCase));

            if (country != null)
            {
                var level = country.ZoomLevel.HasValue && country.ZoomLevel.Value > 0
                    ? country.ZoomLevel.Value
                    : 6;

                return (country.Latitude, country.Longitude, level);
            }
        }

        if (Item != null && !string.IsNullOrWhiteSpace(Item.Continent))
            return GetContinentView(Item.Continent);

        return (20, 0, 2);
    }
    private (double lat, double lon, int level) GetContinentView(string? continent)
    {
        return continent switch
        {
            "Africa" => (1, 20, 4),
            "Antarctica" => (-82, 0, 2),
            "Asia" => (34, 100, 3),
            "Australia" => (-25, 133, 4),
            "Central/South America" => (-15, -60, 3),
            "Europe" => (54, 15, 4),
            "North America/Caribbean" => (40, -100, 3),
            _ => (20, 0, 2)
        };
    }
    private void CenterMapToItem()
    {
        if (!_mapInitialized || MyMap.Map == null || Item == null)
            return;

        // einfache Fallback-Position, falls noch keine Daten vorhanden
        var latitude = Item.Latitude;
        var longitude = Item.Longitude;
        var zoom = Item.ZoomLevel;

        if (latitude == 0 && longitude == 0)
        {
            latitude = 20;     // grobe Weltmitte
            longitude = 0;
            zoom = zoom <= 0 ? 2 : zoom;
        }
        else
        {
            zoom = zoom <= 0 ? 5 : zoom;
        }

        var sphericalMercator = SphericalMercator.FromLonLat(longitude, latitude);
        var center = new MPoint(sphericalMercator.x, sphericalMercator.y);

        MyMap.Map.Navigator.CenterOnAndZoomTo(center, zoom);
        
    }

    private void CenterMapToBestLocation()
    {
        if (!_mapInitialized || MyMap.Map == null || Item == null)
            return;

        double latitude;
        double longitude;
        double zoom;

        // 1) Bereits gespeicherte Koordinaten
        if (!(Item.Latitude == 0 && Item.Longitude == 0))
        {
            latitude = Item.Latitude;
            longitude = Item.Longitude;
            zoom = Item.ZoomLevel > 0 ? Item.ZoomLevel : 6;
        }
        // 2) Country aus Lookup
        else if (ViewModel != null && !string.IsNullOrWhiteSpace(Item.Country))
        {
            var country = ViewModel.CountryList
                .FirstOrDefault(c => string.Equals(c.Name, Item.Country, StringComparison.OrdinalIgnoreCase));

            if (country != null)
            {
                latitude = country.Latitude;
                longitude = country.Longitude;
                zoom = country.ZoomLevel.HasValue && country.ZoomLevel.Value > 0
                    ? country.ZoomLevel.Value
                    : 5;

                CenterMap(latitude, longitude, zoom);
                return;
            }

            // Fallback auf Kontinent
            GetContinentDefaults(Item.Continent, out latitude, out longitude, out zoom);
        }
        // 3) Kontinent
        else if (!string.IsNullOrWhiteSpace(Item.Continent))
        {
            GetContinentDefaults(Item.Continent, out latitude, out longitude, out zoom);
        }
        // 4) Welt
        else
        {
            latitude = 20;
            longitude = 0;
            zoom = 2;
        }

        CenterMap(latitude, longitude, zoom);
    }

    private void CenterMap(double latitude, double longitude, double zoom)
    {
        var sphericalMercator = SphericalMercator.FromLonLat(longitude, latitude);
        var center = new MPoint(sphericalMercator.x, sphericalMercator.y);

        MyMap.Map?.Navigator.CenterOnAndZoomTo(center, zoom);
    }
    private void GetContinentDefaults(string? continent, out double latitude, out double longitude, out double zoom)
    {
        switch (continent)
        {
            case "Africa":
                latitude = 1;
                longitude = 20;
                zoom = 3;
                break;

            case "Antarctica":
                latitude = -82;
                longitude = 0;
                zoom = 2;
                break;

            case "Asia":
                latitude = 34;
                longitude = 100;
                zoom = 3;
                break;

            case "Australia":
                latitude = -25;
                longitude = 133;
                zoom = 4;
                break;

            case "Central/South America":
                latitude = -15;
                longitude = -60;
                zoom = 3;
                break;

            case "Europe":
                latitude = 54;
                longitude = 15;
                zoom = 4;
                break;

            case "North America/Caribbean":
                latitude = 40;
                longitude = -100;
                zoom = 3;
                break;

            default:
                latitude = 20;
                longitude = 0;
                zoom = 2;
                break;
        }
    }
    // ---------------------------------------------------------
    // VALIDATION HELPER
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


    // ---------------------------------------------------------                   
    // VALID YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    // ---------------------------------------------------------                   
    // AUTHOR YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }

    private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }

    // ---------------------------------------------------------                   
    // HTTP VALIDATION
    // ---------------------------------------------------------                   

    private void HttpBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(HttpBox, Item?.HasHttpError ?? false);
    }

    private void HttpBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(HttpBox, Item?.HasHttpError ?? false);
    }

    private void ComboContinent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = sender as ComboBox;
        if (cmb == null)
        {
            return;
        }

        var text = ((ComboBox)sender).SelectedItem as string;
        switch (text)
        {
            case "":    //neu
                break;
            case "Africa":
                break;
            case "Antarctica":
                break;
            case "Asia":
                break;
            case "Australia":
                break;
            case "Central/South America":
                break;
            case "Europe":
                break;
            case "North America/Caribbean":
                break;
        }

        CenterMapToBestLocation();
    }

    // ---------------------------------------------------------                   
    // DETAIL TAB VALIDATION
    // ---------------------------------------------------------                   
    private int SelectedTabIndex = 0;

    private void DetailTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.Tag is string tagText &&
            int.TryParse(tagText, out int index))
        {
            SelectedTabIndex = index;
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }
    private void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(MapTab, MapTabUnderline, SelectedTabIndex == 0);
        SetTabButtonStyle(DataTab, DataTabUnderline, SelectedTabIndex == 1);
        SetTabButtonStyle(LatitudeTab, LatitudeTabUnderline, SelectedTabIndex == 2);
    }
    private void UpdateDetailContentVisibility()
    {
        MapGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        DataGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        LatitudeGrid.Visibility = SelectedTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void CountryCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CenterMapToBestLocation();
    }
}
