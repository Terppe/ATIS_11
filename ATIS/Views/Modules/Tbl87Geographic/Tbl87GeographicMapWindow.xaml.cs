using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI;
using WinRT.Interop;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl87Geographic
{
    public sealed partial class Tbl87GeographicMapWindow : Window
    {
        public event EventHandler? MapStateCommitted;
        //  private bool _hasMapChanges;
        private readonly (double, double, double, double, double, double, double, double, double) _initialMapState;
        public Tbl87GeographicMapWindow(Models.Tbl87Geographic item)
        {
            InitializeComponent();

            _initialMapState = GetMapState(item);
            MapControl.Item = item;
            MapControl.CloseRequested += MapControl_CloseRequested;

            var loc = Localizer.Get();

            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Title = loc.GetLocalizedString("MapPreviewTitle");
            appWindow.Resize(new SizeInt32(1400, 900));
            appWindow.Move(new PointInt32(120, 100));
        }

        private void MapControl_CloseRequested(object? sender, EventArgs e)
        {
            MapControl.FlushMapStateToItem();

            //    if (_hasMapChanges)
            if (MapControl.Item is { } item && GetMapState(item) != _initialMapState)
                MapStateCommitted?.Invoke(this, EventArgs.Empty);

            Close();
        }

        private static (double, double, double, double, double, double, double, double, double) GetMapState(Models.Tbl87Geographic item)
            => (item.Latitude, item.Longitude,
                item.Latitude1, item.Longitude1,
                item.Latitude2, item.Longitude2,
                item.Latitude3, item.Longitude3,
                item.ZoomLevel);
    }
}
