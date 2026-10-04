using Azure.Core.GeoJson;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using NetTopologySuite.Geometries;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;

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
        UpdateMapGeometry();
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

        LoadPointsFromItem();
        UpdateMapGeometry();
        await ApplyStartViewAsync();
    }

    // ---------------------------------------------------------
    // MAP Mapsui
    // ---------------------------------------------------------

    private bool _mapInitialized;
    private ILayer? _markerLayer;
    private ILayer? _lineLayer;
    private bool _addPointMode = false;
    private bool _replacePointMode = false;
    private int _selectedPointIndex = -1;

    // Für Rechtsklick / Kontextmenü
    private int _contextPointIndex = -1;
    private (double lat, double lon)? _contextWorldPosition = null;

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

        var points = GetStoredPoints();

        await Task.Delay(1000);

        if (points.Count == 0)
            return;

        double centerLat;
        double centerLon;

        var zoomResolution = ConvertLegacyZoomToResolution(Item?.ZoomLevel ?? 0);

        if (points.Count == 1)
        {
            centerLat = points[0].Latitude;
            centerLon = points[0].Longitude;

            var worldCenter = SphericalMercator.FromLonLat(centerLon, centerLat);
            MyMap.Map.Navigator.CenterOn(new MPoint(worldCenter.x, worldCenter.y));
            MyMap.Map.Navigator.ZoomTo(zoomResolution);

            MyMap.Refresh();
            return;
        }

        var minLat = points.Min(p => p.Latitude);
        var maxLat = points.Max(p => p.Latitude);
        var minLon = points.Min(p => p.Longitude);
        var maxLon = points.Max(p => p.Longitude);

        centerLat = (minLat + maxLat) / 2.0;
        centerLon = (minLon + maxLon) / 2.0;

        var worldCenterMulti = SphericalMercator.FromLonLat(centerLon, centerLat);
        MyMap.Map.Navigator.CenterOn(new MPoint(worldCenterMulti.x, worldCenterMulti.y));
        MyMap.Map.Navigator.ZoomTo(zoomResolution);

        MyMap.Refresh();
    }
    private double ConvertLegacyZoomToResolution(double zoomLevel)
    {
        if (zoomLevel <= 0 || double.IsNaN(zoomLevel) || double.IsInfinity(zoomLevel))
            return 8000;

        const double baseResolution = 192000d;

        var resolution = baseResolution / Math.Pow(2d, zoomLevel - 1d);

        if (resolution <= 0 || double.IsNaN(resolution) || double.IsInfinity(resolution))
            return 8000;

        return resolution;
    }
    private double ConvertResolutionToLegacyZoom(double resolution)
    {
        if (resolution <= 0 || double.IsNaN(resolution) || double.IsInfinity(resolution))
            return 5.0;

        const double baseResolution = 192000d;

        var zoomLevel = 1d + Math.Log(baseResolution / resolution, 2d);

        if (double.IsNaN(zoomLevel) || double.IsInfinity(zoomLevel))
            return 5.0;

        return Math.Round(zoomLevel, 2);
    }
    private (double lat, double lon, double resolution) GetBestStartView()
    {
        if (Item != null && !(Item.Latitude == 0 && Item.Longitude == 0))
        {
            var resolution = SavedMapResolution > 0 ? SavedMapResolution : 5;
            return (Item.Latitude, Item.Longitude, resolution);
        }

        if (Item != null && ViewModel != null && !string.IsNullOrWhiteSpace(Item.Country))
        {
            var country = ViewModel.CountryList
                .FirstOrDefault(c => string.Equals(c.Name, Item.Country, StringComparison.OrdinalIgnoreCase));

            if (country != null)
            {
                var resolution = country.ZoomLevel.HasValue && country.ZoomLevel.Value > 0
                    ? country.ZoomLevel.Value
                    : 5;

                return (country.Latitude, country.Longitude, resolution);
            }
        }

        if (Item != null && !string.IsNullOrWhiteSpace(Item.Continent))
            return GetContinentView(Item.Continent);

        return (20, 0, 2);
    }
    private (double lat, double lon, double resolution) GetContinentView(string? continent)
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
    private Polygon CreateCircle(double centerX, double centerY, double radius, int segments = 40)
    {
        var coordinates = new Coordinate[segments + 1];

        for (int i = 0; i < segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);
            coordinates[i] = new Coordinate(x, y);
        }

        coordinates[segments] = coordinates[0];

        var linearRing = new LinearRing(coordinates);
        return new Polygon(linearRing);
    }
    private void LoadPointsFromItem()
    {
        _editPoints.Clear();

        if (Item == null)
            return;

        if (!(Item.Latitude == 0 && Item.Longitude == 0))
            _editPoints.Add((Item.Latitude, Item.Longitude));

        if (!(Item.Latitude1 == 0 && Item.Longitude1 == 0))
            _editPoints.Add((Item.Latitude1, Item.Longitude1));

        if (!(Item.Latitude2 == 0 && Item.Longitude2 == 0))
            _editPoints.Add((Item.Latitude2, Item.Longitude2));

        if (!(Item.Latitude3 == 0 && Item.Longitude3 == 0))
            _editPoints.Add((Item.Latitude3, Item.Longitude3));
    }
    private List<(double Latitude, double Longitude)> GetStoredPoints()
    {
        var points = new List<(double Latitude, double Longitude)>();

        foreach (var p in _editPoints)
            points.Add((p.lat, p.lon));

        return points;
    }
    private readonly List<(double lat, double lon)> _editPoints = new();
    private void UpdateLines()
    {
        if (!_mapInitialized || MyMap.Map == null)
            return;

        if (_lineLayer != null)
        {
            MyMap.Map.Layers.Remove(_lineLayer);
            _lineLayer = null;
        }

        // Beim Editieren vorerst keine Linien zeichnen
    }
    //private void UpdateLines()
    //{
    //    if (!_mapInitialized || MyMap.Map == null || Item == null)
    //        return;

    //    if (_lineLayer != null)
    //    {
    //        MyMap.Map.Layers.Remove(_lineLayer);
    //        _lineLayer = null;
    //    }

    //    var storedPoints = GetStoredPoints();
    //    if (storedPoints.Count < 2)
    //        return;

    //    var coordinates = new List<Coordinate>();

    //    foreach (var storedPoint in storedPoints)
    //    {
    //        var sphericalMercator = SphericalMercator.FromLonLat(storedPoint.Longitude, storedPoint.Latitude);
    //        coordinates.Add(new Coordinate(sphericalMercator.x, sphericalMercator.y));
    //    }

    //    // Bei genau 4 Punkten Polygon schließen
    //    if (coordinates.Count == 4)
    //    {
    //        coordinates.Add(new Coordinate(coordinates[0].X, coordinates[0].Y));
    //    }

    //    var lineGeometry = new LineString(coordinates.ToArray());

    //    var feature = new GeometryFeature
    //    {
    //        Geometry = lineGeometry
    //    };

    //    feature.Styles.Add(new VectorStyle
    //    {
    //        Line = new Pen(Color.Red, 2)
    //    });

    //    _lineLayer = new MemoryLayer
    //    {
    //        Name = "SelectedLocationLine",
    //        Features = new[] { feature }
    //    };

    //    MyMap.Map.Layers.Add(_lineLayer);
    //}
    private void UpdateMarkers()
    {
        if (!_mapInitialized || MyMap.Map == null)
            return;

        if (_markerLayer != null)
        {
            MyMap.Map.Layers.Remove(_markerLayer);
            _markerLayer = null;
        }

        if (_editPoints.Count == 0)
            return;

        var features = new List<IFeature>();
        var resolution = MyMap.Map.Navigator.Viewport.Resolution;
        var radius = resolution * 10d;

        for (int i = 0; i < _editPoints.Count; i++)
        {
            var point = _editPoints[i];
            var sphericalMercator = SphericalMercator.FromLonLat(point.lon, point.lat);
            var circleGeometry = CreateCircle(sphericalMercator.x, sphericalMercator.y, radius);

            var feature = new GeometryFeature
            {
                Geometry = circleGeometry
            };

            var isSelected = i == _selectedPointIndex;

            feature.Styles.Add(new VectorStyle
            {
                Fill = isSelected
                    ? new Brush(new Color(120, 0, 120, 255))
                    : new Brush(new Color(0, 0, 0, 0)),
                Outline = new Pen(isSelected ? Color.Blue : Color.Red, isSelected ? 3 : 2)
            });

            features.Add(feature);
        }

        _markerLayer = new MemoryLayer
        {
            Name = "SelectedLocationMarkers",
            Features = features
        };

        MyMap.Map.Layers.Add(_markerLayer);
        MyMap.Refresh();
    }
    private void UpdateMapGeometry()
    {
        UpdateLines();
        UpdateMarkers();
        MyMap.Refresh();
    }
    private void SavePointsToItem(List<(double lat, double lon)> points)
    {
        if (Item == null)
            return;

        Item.Latitude = points.Count > 0 ? points[0].lat : 0;
        Item.Longitude = points.Count > 0 ? points[0].lon : 0;

        Item.Latitude1 = points.Count > 1 ? points[1].lat : 0;
        Item.Longitude1 = points.Count > 1 ? points[1].lon : 0;

        Item.Latitude2 = points.Count > 2 ? points[2].lat : 0;
        Item.Longitude2 = points.Count > 2 ? points[2].lon : 0;

        Item.Latitude3 = points.Count > 3 ? points[3].lat : 0;
        Item.Longitude3 = points.Count > 3 ? points[3].lon : 0;
    }
    private double SavedMapResolution
    {
        get => Item?.ZoomLevel ?? 0;
        set
        {
            if (Item != null)
                Item.ZoomLevel = value;
        }
    }
    private void SaveLegacyZoomToItem()
    {
        if (Item == null || MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var resolution = MyMap.Map.Navigator.Viewport.Resolution;
        Item.ZoomLevel = ConvertResolutionToLegacyZoom(resolution);
    }
    private void MyMap_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (!_mapInitialized || MyMap?.Map?.Navigator == null)
            return;

        if (!_addPointMode)
            return;

        if (_editPoints.Count >= 4)
        {
            _addPointMode = false;
            return;
        }

        var position = e.GetPosition(MyMap);

        var viewport = MyMap.Map.Navigator.Viewport;
        var worldPosition = viewport.ScreenToWorld(position.X, position.Y);

        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        _editPoints.Add((lonLat.lat, lonLat.lon));

        _addPointMode = false;
        _selectedPointIndex = -1;

        SavePointsToItem(_editPoints);
        UpdateMapGeometry();
    }
    public void FlushMapStateToItem()
    {
        SavePointsToItem(_editPoints);
        SaveLegacyZoomToItem();
    }

    private void AddPointButton_Click(object sender, RoutedEventArgs e)
    {
        _addPointMode = true;
        _replacePointMode = false;
        _selectedPointIndex = -1;
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SavePointsToItem(_editPoints);
        SaveLegacyZoomToItem();
    }
    private void MenuAddPoint_Click(object sender, RoutedEventArgs e)
    {
        if (_contextWorldPosition == null)
            return;

        if (_editPoints.Count >= 4)
            return;

        var p = _contextWorldPosition.Value;
        _editPoints.Add((p.lat, p.lon));

        _addPointMode = false;
        _replacePointMode = false;
        _selectedPointIndex = -1;

        SavePointsToItem(_editPoints);
        UpdateMapGeometry();

        _contextPointIndex = -1;
        _contextWorldPosition = null;
    }
    private void DeletePointButton_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectedPoint();
    }
    private void MyMap_OnMapPointerReleased(object? sender, MapEventArgs e)
    {
        if (MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var worldPosition = e.WorldPosition;
        var screenPosition = e.ScreenPosition;
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // Replace-Modus: selektierten Punkt an neue Stelle setzen
        if (_replacePointMode && _selectedPointIndex >= 0 && _selectedPointIndex < _editPoints.Count)
        {
            _editPoints[_selectedPointIndex] = (lonLat.lat, lonLat.lon);

            _replacePointMode = false;
            SavePointsToItem(_editPoints);
            UpdateMapGeometry();
            return;
        }

        // Normalmodus: Punkt selektieren
        var hitIndex = FindPointAtScreenPosition(screenPosition.X, screenPosition.Y);
        if (hitIndex >= 0)
        {
            _selectedPointIndex = hitIndex;
            UpdateMarkers();
            MyMap.Refresh();
            return;
        }

        // Klick ins Leere: Auswahl optional aufheben
        _selectedPointIndex = -1;
        UpdateMarkers();
        MyMap.Refresh();
    }
    private int FindPointAtScreenPosition(double screenX, double screenY)
    {
        if (MyMap?.Map?.Navigator?.Viewport == null)
            return -1;

        const double hitRadius = 22d;

        for (int i = 0; i < _editPoints.Count; i++)
        {
            var point = _editPoints[i];
            var world = SphericalMercator.FromLonLat(point.lon, point.lat);
            var screen = MyMap.Map.Navigator.Viewport.WorldToScreen(world.x, world.y);

            var dx = screen.X - screenX;
            var dy = screen.Y - screenY;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance <= hitRadius)
                return i;
        }

        return -1;
    }
    private void BeginReplaceSelectedPoint()
    {
        if (_selectedPointIndex < 0 || _selectedPointIndex >= _editPoints.Count)
            return;

        _replacePointMode = true;
    }
    private void ReplacePointButton_Click(object sender, RoutedEventArgs e)
    {
        BeginReplaceSelectedPoint();
    }
    private void DeleteSelectedPoint()
    {
        if (_selectedPointIndex < 0 || _selectedPointIndex >= _editPoints.Count)
            return;

        _editPoints.RemoveAt(_selectedPointIndex);

        _selectedPointIndex = -1;
        _replacePointMode = false;
        _addPointMode = false;

        SavePointsToItem(_editPoints);
        UpdateMapGeometry();
        MyMap.Refresh();
    }

    private void MyMap_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (!_mapInitialized || MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var position = e.GetPosition(MyMap);
        var viewport = MyMap.Map.Navigator.Viewport;
        var worldPosition = viewport.ScreenToWorld(position.X, position.Y);
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        _contextPointIndex = FindPointAtScreenPosition(position.X, position.Y);
        _contextWorldPosition = (lonLat.lat, lonLat.lon);

        if (_contextPointIndex >= 0)
        {
            _selectedPointIndex = _contextPointIndex;
        }
        else
        {
            _selectedPointIndex = -1;
        }

        UpdateMarkers();
        MyMap.Refresh();

        if (Resources["PointContextFlyout"] is MenuFlyout flyout)
        {
            if (flyout.Items.Count >= 3)
            {
                if (flyout.Items[0] is MenuFlyoutItem addItem)
                    addItem.IsEnabled = _editPoints.Count < 4;

                if (flyout.Items[1] is MenuFlyoutItem changeItem)
                    changeItem.IsEnabled = _contextPointIndex >= 0;

                if (flyout.Items[2] is MenuFlyoutItem deleteItem)
                    deleteItem.IsEnabled = _contextPointIndex >= 0;
            }

            flyout.ShowAt(MyMap, new FlyoutShowOptions
            {
                Position = position
            });
        }

        e.Handled = true;
    }
    private void MenuDeletePoint_Click(object sender, RoutedEventArgs e)
    {
        if (_contextPointIndex < 0)
            return;

        _selectedPointIndex = _contextPointIndex;
        DeleteSelectedPoint();

        _contextPointIndex = -1;
        _contextWorldPosition = null;
    }

    private void MenuChangePoint_Click(object sender, RoutedEventArgs e)
    {
        if (_contextPointIndex < 0)
            return;

        _selectedPointIndex = _contextPointIndex;
        _addPointMode = false;
        _replacePointMode = true;
    }
    private void ChangePointButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedPointIndex < 0 || _selectedPointIndex >= _editPoints.Count)
            return;

        _addPointMode = false;
        _replacePointMode = true;
    }
    private void ResetModes()
    {
        _addPointMode = false;
        _replacePointMode = false;
    }

    private void ClearSelection()
    {
        _selectedPointIndex = -1;
        UpdateMarkers();
        MyMap.Refresh();
    }

    private void ResetInteractionState()
    {
        _addPointMode = false;
        _replacePointMode = false;
        _selectedPointIndex = -1;
        _contextPointIndex = -1;
        _contextWorldPosition = null;

        UpdateMarkers();
        MyMap.Refresh();
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
