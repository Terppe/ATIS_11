using System.Configuration;
using WinUIEx;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Main;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashWindow : WindowEx
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
        {
            return;
        }

        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        appWindow.IsShownInSwitchers = false;
        appWindow.Resize(new SizeInt32 { Width = 800, Height = 700 });
        presenter.SetBorderAndTitleBar(false, false);
    }
}
