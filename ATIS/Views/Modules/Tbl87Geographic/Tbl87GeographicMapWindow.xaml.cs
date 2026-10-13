using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Microsoft.UI;
using WinRT.Interop;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl87Geographic
{
    public sealed partial class Tbl87GeographicMapWindow : Window
    {
        public event EventHandler? MapStateCommitted;
        private bool _hasMapChanges;

        public Tbl87GeographicMapWindow(Models.Tbl87Geographic item)
        {
            this.InitializeComponent();

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

        //private void MapControl_CloseRequested(object? sender, EventArgs e)
        //{
        //    MapControl.FlushMapStateToItem();
        //    //      MapControl.CloseRequested -= MapControl_CloseRequested;
        //    this.Close();
        //}
        private void MapControl_MapChanged(object? sender, EventArgs e)
        {
            _hasMapChanges = true;
        }

        private void MapControl_CloseRequested(object? sender, EventArgs e)
        {
            MapControl.FlushMapStateToItem();

            if (_hasMapChanges)
                MapStateCommitted?.Invoke(this, EventArgs.Empty);

            this.Close();
        }


    }
}
