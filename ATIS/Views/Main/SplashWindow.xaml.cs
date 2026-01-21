using System.Configuration;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;

namespace ATIS.Views.Main;

public sealed partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();

        Copyright.Text = ConfigurationManager.AppSettings.Get("Copyright");

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        var presenter = appWindow.Presenter as OverlappedPresenter;

        if (presenter == null)
            return;

        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        appWindow.IsShownInSwitchers = false;
        appWindow.Resize(new SizeInt32 { Width = 800, Height = 700 });
        presenter.SetBorderAndTitleBar(false, false);
    }
}
