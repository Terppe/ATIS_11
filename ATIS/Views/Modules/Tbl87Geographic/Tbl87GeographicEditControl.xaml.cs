using System.Diagnostics;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;
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
    #region Nested types / constants
    // ---------------------------------------------------------
    private void CenterMap(double latitude, double longitude, double zoomLevel)
    {
        LogDebug($"CenterMap: lat={latitude}, lon={longitude}, zoomLevel={zoomLevel}");

        var sphericalMercator = SphericalMercator.FromLonLat(longitude, latitude);
        var center = new MPoint(sphericalMercator.x, sphericalMercator.y);
        var resolution = ConvertLegacyZoomToResolution(zoomLevel);

        MyMap.Map?.Navigator.CenterOnAndZoomTo(center, resolution);
        MyMap.Refresh();
    }

    private static readonly Dictionary<string, (double Latitude, double Longitude, double ZoomLevel)> _continentDefaults
        = new()
        {
            ["Africa"] = (1, 20, 3),
            ["Antarctica"] = (-82, 0, 2),
            ["Asia"] = (34, 100, 3),
            ["Australia"] = (-25, 133, 4),
            ["Central/South America"] = (-15, -60, 4.6),
            ["Europe"] = (54, 15, 4),
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

    public void ResetMapToWorld()
    {
        const double latitude = 20;
        const double longitude = 0;
        const double zoomLevel = 3;

        if (Item != null)
        {
            Item.Latitude = latitude;
            Item.Longitude = longitude;
            Item.ZoomLevel = zoomLevel;
        }

        CenterMap(latitude, longitude, zoomLevel);
    }
    #endregion

    #region Fields / state
    // ---------------------------------------------------------

    private bool _isInitializingCombos;
    private bool _isApplyingItem;
    private bool _isRefreshingView;
    private bool _mapInitialized;
    private ILayer? _markerLayer;
    private ILayer? _lineLayer;
    private bool _addPointMode = false;
    private bool _replacePointMode = false;
    private int _selectedPointIndex = -1;
    private int _contextPointIndex = -1;
    private (double lat, double lon)? _contextWorldPosition = null;
    private readonly List<(double lat, double lon)> _editPoints = new();
    private bool _hasFallbackPoint;
    private double? _lastMarkerResolution;
    private bool _viewportChangedHooked;
    private bool _mapEventsHooked;
    private MPoint? _pointerPressedScreenPosition;
    private double? _pointerPressedX;
    private double? _pointerPressedY;
    private const double ClickTolerance = 6d;
    private Models.Tbl87Geographic? _pendingItem;
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
        SelectedTabIndex = 0;
        Loaded += Tbl87GeographicEditControl_Loaded;
        Unloaded += Tbl87GeographicEditControl_Unloaded;
    }
    private async void Tbl87GeographicEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();

        if (DataContext is Tbl87GeographicViewModel vm)
        {
            vm.RequestMapReset -= ResetMapToWorld;
            vm.RequestMapReset += ResetMapToWorld;
        }

        InitializeMap();
        HookMapEvents();
        UpdateMapGeometry();
    }
    private void Tbl87GeographicEditControl_Unloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is Tbl87GeographicViewModel vm)
            vm.RequestMapReset -= ResetMapToWorld;

        UnhookMapEvents();
    }

    #endregion

    // ---------------------------------------------------------
    // MAP Mapsui
    // ---------------------------------------------------------
    #region Initialization / loading
    // ---------------------------------------------------------
    private async void InitializeMap()
    {
        LoggingWidget.ShowLoggingInMap = ActiveMode.No;

        if (_mapInitialized)
            return;

        var map = new Mapsui.Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        MyMap.Map = map;
        _mapInitialized = true;

        if (_pendingItem != null)
        {
            var pending = _pendingItem;
            _pendingItem = null;

            LogDebug($"Map initialized -> applying pending itemId={pending.GeographicId}");

            ApplyItem(pending);
            return;
        }

        if (Item != null)
        {
            LogDebug("Map initialized -> applying start view");
            await ApplyStartViewAsync();
        }
    }
    private async Task ApplyStartViewAsync()
    {
        if (!_mapInitialized || MyMap?.Map == null || Item == null)
        {
            LogGuard(nameof(ApplyStartViewAsync), "map or item not ready");
            return;
        }

        await Task.Delay(1000);

        var points = HasRealEditPoints()
            ? _editPoints.Select(p => (Latitude: p.lat, Longitude: p.lon)).ToList()
            : GetStoredPoints();

        LogDebug($"ApplyStartViewAsync: points.Count={points.Count}, zoom={Item.ZoomLevel}");

        if (points.Count == 0)
            return;

        var zoomLevel = Item.ZoomLevel > 0 ? Item.ZoomLevel : 6;
        var zoomResolution = ConvertLegacyZoomToResolution(zoomLevel);

        if (points.Count == 1)
        {
            var singleLat = points[0].Latitude;
            var singleLon = points[0].Longitude;

            var worldCenter = SphericalMercator.FromLonLat(singleLon, singleLat);
            MyMap.Map.Navigator.CenterOn(new MPoint(worldCenter.x, worldCenter.y));
            MyMap.Map.Navigator.ZoomTo(zoomResolution);

            MyMap.Refresh();
            return;
        }

        var minLat = points.Min(p => p.Latitude);
        var maxLat = points.Max(p => p.Latitude);
        var minLon = points.Min(p => p.Longitude);
        var maxLon = points.Max(p => p.Longitude);

        var centerLat = (minLat + maxLat) / 2.0;
        var centerLon = (minLon + maxLon) / 2.0;

        var worldCenterMulti = SphericalMercator.FromLonLat(centerLon, centerLat);
        MyMap.Map.Navigator.CenterOn(new MPoint(worldCenterMulti.x, worldCenterMulti.y));
        MyMap.Map.Navigator.ZoomTo(zoomResolution);

        MyMap.Refresh();
    }
    private async Task RefreshViewFromCurrentItemAsync()
    {
        if (_isRefreshingView)
            return;

        if (!_mapInitialized || MyMap?.Map == null || Item == null)
        {
            LogGuard(nameof(RefreshViewFromCurrentItemAsync), "map or item not ready");
            return;
        }

        try
        {
            _isRefreshingView = true;

            LogDebug($"RefreshView: itemId={Item.GeographicId}");

            _selectedPointIndex = -1;

            LoadPointsFromItem();

            var points = GetStoredPoints();

            if (points.Count == 0)
            {
                LogDebug("RefreshView: fallback -> CenterMapToBestLocation");
                CenterMapToBestLocation();
                UpdateMapGeometry();
                return;
            }

            LogDebug("RefreshView: points found -> ApplyStartViewAsync");
            await ApplyStartViewAsync();

            UpdateMapGeometry();

            await Task.Delay(200);
            MyMap.Refresh();
        }
        finally
        {
            _isRefreshingView = false;
        }
    }
    private void MyMapNavigator_ViewportChanged(object? sender, EventArgs e)
    {
        if (MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var resolution = MyMap.Map.Navigator.Viewport.Resolution;

        if (_lastMarkerResolution.HasValue &&
            Math.Abs(_lastMarkerResolution.Value - resolution) < 0.0001)
            return;

        _lastMarkerResolution = resolution;
        UpdateMapGeometry();
    }
    private void HookMapEvents()
    {
        if (_mapEventsHooked)
            return;

        if (MyMap?.Map?.Navigator == null)
            return;

        MyMap.Map.Navigator.ViewportChanged += MyMapNavigator_ViewportChanged;
        _mapEventsHooked = true;
    }
    private void UnhookMapEvents()
    {
        if (!_mapEventsHooked)
            return;

        if (MyMap?.Map?.Navigator == null)
            return;

        MyMap.Map.Navigator.ViewportChanged -= MyMapNavigator_ViewportChanged;
        _mapEventsHooked = false;
    }

    #endregion

    #region Map positioning / navigation
    // ---------------------------------------------------------
    private void CenterMapToBestLocation()
    {
        if (!_mapInitialized || MyMap?.Map == null || Item == null)
            return;

        var points = GetStoredPoints();
        if (points.Count > 0)
            return;

        double latitude;
        double longitude;
        double zoom;

        if (!(Item.Latitude == 0 && Item.Longitude == 0))
        {
            latitude = Item.Latitude;
            longitude = Item.Longitude;
            zoom = Item.ZoomLevel > 0 ? Item.ZoomLevel : 6;

            LogDebug("CenterMapToBestLocation: stored coords");
            CenterMap(latitude, longitude, zoom);
            return;
        }

        if (ViewModel != null && !string.IsNullOrWhiteSpace(Item.Country))
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

                SetSingleFallbackPoint(latitude, longitude);

                LogDebug("CenterMapToBestLocation: country fallback");
                CenterMap(latitude, longitude, zoom);
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(Item.Continent))
        {
            GetContinentDefaults(Item.Continent, out latitude, out longitude, out zoom);

            SetSingleFallbackPoint(latitude, longitude);

            LogDebug("CenterMapToBestLocation: continent fallback");
            CenterMap(latitude, longitude, zoom);
            return;
        }

        SetSingleFallbackPoint(20, 0);

        LogDebug("CenterMapToBestLocation: world fallback");
        CenterMap(20, 0, 3);
    }
    #endregion

    #region Rendering
    // ---------------------------------------------------------
    private List<Coordinate> OrderCoordinatesForPolygon(List<Coordinate> coordinates)
    {
        if (coordinates == null || coordinates.Count < 3)
            return coordinates ?? new List<Coordinate>();

        var centerX = coordinates.Average(c => c.X);
        var centerY = coordinates.Average(c => c.Y);

        return coordinates
            .OrderBy(c => Math.Atan2(c.Y - centerY, c.X - centerX))
            .ToList();
    }

    private void UpdateLines()
    {
        if (!_mapInitialized || MyMap?.Map == null)
            return;

        if (_lineLayer != null)
        {
            MyMap.Map.Layers.Remove(_lineLayer);
            _lineLayer = null;
        }

        if (_editPoints == null || _editPoints.Count < 2)
        {
            MyMap.Refresh();
            return;
        }

        var features = new List<IFeature>();

        var coordinates = _editPoints
            .Select(p =>
            {
                var xy = SphericalMercator.FromLonLat(p.lon, p.lat);
                return new Coordinate(xy.x, xy.y);
            })
            .ToList();

        if (coordinates.Count == 2)
        {
            var lineFeature = new GeometryFeature
            {
                Geometry = new LineString(coordinates.ToArray())
            };

            lineFeature.Styles.Add(new VectorStyle
            {
                Line = new Pen(Color.Red, 2)
            });

            features.Add(lineFeature);
        }
        else if (coordinates.Count >= 3)
        {
            var orderedCoordinates = OrderCoordinatesForPolygon(coordinates);
            var ringCoordinates = new List<Coordinate>(orderedCoordinates);

            var first = ringCoordinates[0];
            var last = ringCoordinates[^1];

            if (first.X != last.X || first.Y != last.Y)
                ringCoordinates.Add(new Coordinate(first.X, first.Y));

            var closedLineFeature = new GeometryFeature
            {
                Geometry = new LineString(ringCoordinates.ToArray())
            };

            closedLineFeature.Styles.Add(new VectorStyle
            {
                Line = new Pen(Color.Red, 2)
            });

            features.Add(closedLineFeature);
        }

        _lineLayer = new MemoryLayer
        {
            Name = "EditLinesLayer",
            Features = features
        };

        MyMap.Map.Layers.Add(_lineLayer);
        MyMap.Refresh();
    }
    private void SetSingleFallbackPoint(double latitude, double longitude)
    {
        _editPoints.Clear();
        _editPoints.Add((latitude, longitude));
        _hasFallbackPoint = true;
        _selectedPointIndex = -1;
        UpdateMapGeometry();
    }
    private void UpdatePoints()
    {
        if (!_mapInitialized || MyMap?.Map == null || MyMap.Map.Navigator?.Viewport == null)
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

        for (int i = 0; i < _editPoints.Count; i++)
        {
            var point = _editPoints[i];
            var isFallbackPoint = _hasFallbackPoint && _editPoints.Count == 1 && i == 0;
            var isSelectedPoint = i == _selectedPointIndex;

            double pixelRadius;
            Color outlineColor;
            Color fillColor;
            double lineWidth;

            if (isFallbackPoint)
            {
                pixelRadius = 8.0d;
                outlineColor = Color.Orange;
                fillColor = new Color(255, 200, 0, 80);
                lineWidth = 2.5d;
            }
            else if (isSelectedPoint)
            {
                pixelRadius = 7.0d;
                outlineColor = Color.LimeGreen;
                fillColor = new Color(0, 255, 0, 60);
                lineWidth = 2.5d;
            }
            else
            {
                pixelRadius = 5.5d;
                outlineColor = Color.Red;
                fillColor = new Color(255, 0, 0, 40);
                lineWidth = 2d;
            }

            var worldRadius = resolution * pixelRadius;

            var sphericalMercator = SphericalMercator.FromLonLat(point.lon, point.lat);
            var circleGeometry = CreateCircle(sphericalMercator.x, sphericalMercator.y, worldRadius);

            var feature = new GeometryFeature
            {
                Geometry = circleGeometry
            };

            feature.Styles.Add(new VectorStyle
            {
                Fill = new Brush(fillColor),
                Outline = new Pen(outlineColor, lineWidth)
            });

            features.Add(feature);
        }

        _markerLayer = new MemoryLayer
        {
            Name = "SelectedLocationMarkers",
            Features = features,
            MinVisible = 0,
            MaxVisible = double.MaxValue
        };

        MyMap.Map.Layers.Add(_markerLayer);
        MyMap.Refresh();
    }
    private void UpdateMapGeometry()
    {
        UpdateLines();
        UpdatePoints();
        MyMap.Refresh();
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
    #endregion

    #region Point editing / interaction logic
    // ---------------------------------------------------------
    private void LoadPointsFromItem()
    {
        _editPoints.Clear();

        foreach (var p in GetStoredPoints())
            _editPoints.Add((p.Latitude, p.Longitude));

        _hasFallbackPoint = false;
        _selectedPointIndex = -1;
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
    private void DeleteSelectedPoint()
    {
        if (_selectedPointIndex < 0 || _selectedPointIndex >= _editPoints.Count)
            return;

        _editPoints.RemoveAt(_selectedPointIndex);

        _selectedPointIndex = -1;
        _replacePointMode = false;
        _addPointMode = false;

        SyncItemBeforeSave();
        UpdateMapGeometry();
        MyMap.Refresh();
    }
    #endregion

    #region Map/UI event handlers
    // ---------------------------------------------------------
    private void MyMap_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        if (!_mapInitialized || MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var position = e.GetPosition(MyMap);
        var hitIndex = FindPointAtScreenPosition(position.X, position.Y);

        // 1. Replace / Move mode
        if (_replacePointMode)
        {
            if (_selectedPointIndex < 0 || _selectedPointIndex >= _editPoints.Count)
                return;

            var worldPosition = MyMap.Map.Navigator.Viewport.ScreenToWorld(position.X, position.Y);
            var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

            _editPoints[_selectedPointIndex] = (lonLat.lat, lonLat.lon);

            _hasFallbackPoint = false;
            _replacePointMode = false;
            _addPointMode = false;

            SyncItemBeforeSave();
            UpdateMapGeometry();
            return;
        }

        // 2. Tap on existing point => select it
        if (hitIndex >= 0)
        {
            _selectedPointIndex = hitIndex;
            UpdateMapGeometry();
            return;
        }

        // 3. Tap on empty map => add new point
        if (_editPoints.Count >= 4)
            return;

        var world = MyMap.Map.Navigator.Viewport.ScreenToWorld(position.X, position.Y);
        var p = SphericalMercator.ToLonLat(world.X, world.Y);

        AddPointAt(p.lat, p.lon);

        _selectedPointIndex = -1;
        _contextPointIndex = -1;
        _contextWorldPosition = null;
    }
    private void MyMap_OnMapPointerPressed(object? sender, MapEventArgs e)
    {
        _pointerPressedX = e.ScreenPosition.X;
        _pointerPressedY = e.ScreenPosition.Y;
    }
    private void MyMap_OnMapPointerReleased(object? sender, MapEventArgs e)
    {
        if (MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var screenX = e.ScreenPosition.X;
        var screenY = e.ScreenPosition.Y;

        bool isClick = false;

        if (_pointerPressedX.HasValue && _pointerPressedY.HasValue)
        {
            var dx = screenX - _pointerPressedX.Value;
            var dy = screenY - _pointerPressedY.Value;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            isClick = distance <= ClickTolerance;
        }

        _pointerPressedX = null;
        _pointerPressedY = null;

        if (!isClick)
            return;

        var worldPosition = e.WorldPosition;
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // Replace-Modus
        if (_replacePointMode && _selectedPointIndex >= 0 && _selectedPointIndex < _editPoints.Count)
        {
            _editPoints[_selectedPointIndex] = (lonLat.lat, lonLat.lon);

            _hasFallbackPoint = false;
            _replacePointMode = false;
            _addPointMode = false;

            SyncItemBeforeSave();
            UpdateMapGeometry();
            return;
        }

        // Normalmodus: Punkt selektieren
        var hitIndex = FindPointAtScreenPosition(screenX, screenY);
        if (hitIndex >= 0)
        {
            _selectedPointIndex = hitIndex;
            UpdateMapGeometry();
            MyMap.Refresh();
            return;
        }

        _selectedPointIndex = -1;
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

        UpdateMapGeometry();
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
    private void AddPointAt(double lat, double lon)
    {
        if (_hasFallbackPoint)
        {
            _editPoints.Clear();
            _hasFallbackPoint = false;
        }

        if (_editPoints.Count >= 4)
            return;

        _editPoints.Add((lat, lon));
        _selectedPointIndex = -1;

        SyncItemBeforeSave();
        UpdateMapGeometry();
    }
    private void MenuAddPoint_Click(object sender, RoutedEventArgs e)
    {
        if (_contextWorldPosition == null)
            return;

        var p = _contextWorldPosition.Value;
        AddPointAt(p.lat, p.lon);

        _addPointMode = false;
        _replacePointMode = false;
        _selectedPointIndex = -1;

        _contextPointIndex = -1;
        _contextWorldPosition = null;
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
    private void MenuMovePoint_Click(object sender, RoutedEventArgs e)
    {
        if (_contextPointIndex < 0)
            return;

        _selectedPointIndex = _contextPointIndex;
        _addPointMode = false;
        _replacePointMode = true;

        _contextPointIndex = -1;
        _contextWorldPosition = null;

        UpdateMapGeometry();
    }
    public void FlushMapStateToItem()
    {
        SavePointsToItem();
        SaveLegacyZoomToItem();
    }
    private bool HasRealEditPoints()
    {
        if (_editPoints.Count == 0)
            return false;

        if (_editPoints.Count > 1)
            return true;

        if (_hasFallbackPoint && _editPoints.Count == 1)
            return false;

        return true;
    }
    private bool HasOnlyWorldFallbackPoint()
    {
        if (Item == null)
            return false;

        var onlyFirstPoint =
            !(Item.Latitude == 0 && Item.Longitude == 0) &&
            (Item.Latitude1 == 0 && Item.Longitude1 == 0) &&
            (Item.Latitude2 == 0 && Item.Longitude2 == 0) &&
            (Item.Latitude3 == 0 && Item.Longitude3 == 0);

        if (!onlyFirstPoint)
            return false;

        return Item.Latitude == 20 &&
               Item.Longitude == 0 &&
               Math.Abs(Item.ZoomLevel - 3) < 0.01;
    }
    private List<(double Latitude, double Longitude)> GetStoredPoints()
    {
        var points = new List<(double Latitude, double Longitude)>();

        if (Item == null)
            return points;

        if (HasOnlyWorldFallbackPoint())
            return points;

        if (!(Item.Latitude == 0 && Item.Longitude == 0))
            points.Add((Item.Latitude, Item.Longitude));

        if (!(Item.Latitude1 == 0 && Item.Longitude1 == 0))
            points.Add((Item.Latitude1, Item.Longitude1));

        if (!(Item.Latitude2 == 0 && Item.Longitude2 == 0))
            points.Add((Item.Latitude2, Item.Longitude2));

        if (!(Item.Latitude3 == 0 && Item.Longitude3 == 0))
            points.Add((Item.Latitude3, Item.Longitude3));

        return points;
    }
    private void ContinentCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
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

        if (!_mapInitialized)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "map not initialized");
            return;
        }

        if (_pendingItem != null)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "pending item exists");
            return;
        }

        if (ViewModel == null || Item == null)
        {
            LogGuard(nameof(ContinentCombo_OnSelectionChanged), "view model or item is null");
            return;
        }

        // Wenn bereits echte Punkte bearbeitet wurden, nichts mehr automatisch verschieben
        if (HasRealEditPoints())
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "real edit points already exist");
            return;
        }
        var continent = (sender as ComboBox)?.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(continent))
        {
            CenterMapToBestLocation();
            return;
        }

        GetContinentDefaults(continent, out var latitude, out var longitude, out var zoomLevel);

        Item.Continent = continent;
        Item.Latitude = latitude;
        Item.Longitude = longitude;
        Item.ZoomLevel = zoomLevel;

        SetSingleFallbackPoint(latitude, longitude);
        CenterMap(latitude, longitude, zoomLevel);
    }
    private void CountryCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
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

        if (!_mapInitialized)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "map not initialized");
            return;
        }

        if (_pendingItem != null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "pending item exists");
            return;
        }

        if (ViewModel == null || Item == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "view model or item is null");
            return;
        }

        if (HasRealEditPoints())
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "real edit points already exist");
            return;
        }
        var selectedCountry = (sender as ComboBox)?.SelectedItem as Models.TblCountry;

        if (selectedCountry == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "selected country is null");
            return;
        }

        Item.Country = selectedCountry.Name;
        Item.Latitude = selectedCountry.Latitude;
        Item.Longitude = selectedCountry.Longitude;
        Item.ZoomLevel = selectedCountry.ZoomLevel.HasValue && selectedCountry.ZoomLevel.Value > 0
            ? selectedCountry.ZoomLevel.Value
            : 5;

        SetSingleFallbackPoint(Item.Latitude, Item.Longitude);
        CenterMap(Item.Latitude, Item.Longitude, Item.ZoomLevel);
    }

    #endregion

    #region Persistence / save
    // ---------------------------------------------------------
    private void SavePointsToItem()
    {
        if (Item == null)
            return;

        Item.Latitude = 0;
        Item.Longitude = 0;
        Item.Latitude1 = 0;
        Item.Longitude1 = 0;
        Item.Latitude2 = 0;
        Item.Longitude2 = 0;
        Item.Latitude3 = 0;
        Item.Longitude3 = 0;

        if (_editPoints.Count > 0)
        {
            Item.Latitude = _editPoints[0].lat;
            Item.Longitude = _editPoints[0].lon;
        }

        if (_editPoints.Count > 1)
        {
            Item.Latitude1 = _editPoints[1].lat;
            Item.Longitude1 = _editPoints[1].lon;
        }

        if (_editPoints.Count > 2)
        {
            Item.Latitude2 = _editPoints[2].lat;
            Item.Longitude2 = _editPoints[2].lon;
        }

        if (_editPoints.Count > 3)
        {
            Item.Latitude3 = _editPoints[3].lat;
            Item.Longitude3 = _editPoints[3].lon;
        }
    }
    private void SaveLegacyZoomToItem()
    {
        if (Item == null || MyMap?.Map?.Navigator?.Viewport == null)
            return;

        var resolution = MyMap.Map.Navigator.Viewport.Resolution;
        Item.ZoomLevel = ConvertResolutionToLegacyZoom(resolution);
    }
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        SavePointsToItem();
        EnsureLocationDefaultsInItem();
        SaveLegacyZoomToItem();

        LogDebug($"SaveButton_Click: itemId={Item?.GeographicId}, zoom={Item?.ZoomLevel}");
    }
    private void EnsureLocationDefaultsInItem()
    {
        if (Item == null)
            return;

        var hasPoints =
            !(Item.Latitude == 0 && Item.Longitude == 0) ||
            !(Item.Latitude1 == 0 && Item.Longitude1 == 0) ||
            !(Item.Latitude2 == 0 && Item.Longitude2 == 0) ||
            !(Item.Latitude3 == 0 && Item.Longitude3 == 0);

        if (hasPoints)
            return;

        if (ViewModel != null && !string.IsNullOrWhiteSpace(Item.Country))
        {
            var country = ViewModel.CountryList?
                .FirstOrDefault(c => string.Equals(c.Name, Item.Country, StringComparison.OrdinalIgnoreCase));

            if (country != null)
            {
                Item.Latitude = country.Latitude;
                Item.Longitude = country.Longitude;
                Item.ZoomLevel = country.ZoomLevel.HasValue && country.ZoomLevel.Value > 0
                    ? country.ZoomLevel.Value
                    : 5;
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(Item.Continent))
        {
            GetContinentDefaults(Item.Continent, out var latitude, out var longitude, out var zoomLevel);
            Item.Latitude = latitude;
            Item.Longitude = longitude;
            Item.ZoomLevel = zoomLevel;
            return;
        }

        Item.Latitude = 20;
        Item.Longitude = 0;
        Item.ZoomLevel = 3;
    }
    private void SyncItemBeforeSave()
    {
        SavePointsToItem();
        EnsureLocationDefaultsInItem();
        SaveLegacyZoomToItem();
    }
    #endregion

    #region Legacy/compat helpers (nur wenn wirklich noch benutzt)
    // ---------------------------------------------------------
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
    #endregion

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
    private async void ApplyItem(Models.Tbl87Geographic? newItem)
    {
        // Applies the current item to the control.
        // If the map is not initialized yet, the item is stored in _pending Item
        // and applied once initialization completes.
        LogDebug($"ApplyItem: mapInitialized={_mapInitialized}, itemId={newItem?.GeographicId}");

        if (newItem == null)
        {
            LogGuard(nameof(ApplyItem), "newItem is null");
            return;
        }

        if (!_mapInitialized)
        {
            _pendingItem = newItem;
            LogDebug($"ApplyItem deferred: itemId={newItem.GeographicId}");
            return;
        }

        _isApplyingItem = true;
        _isInitializingCombos = true;

        try
        {
            _pendingItem = null;
            await RefreshViewFromCurrentItemAsync();
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

    #region DETAIL TAB VALIDATION
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
    #endregion

}
