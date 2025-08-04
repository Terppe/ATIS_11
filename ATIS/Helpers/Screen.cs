using System.Configuration;
using Microsoft.UI;

namespace ATIS.Helpers;
public class Screen
{
    public static void CenterToScreen(IntPtr hWnd)
    {
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        if (appWindow is not null)
        {
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                appWindow.Move(CenteredPosition);
            }
        }
    }

    public static void ScreenWindowCenterMethod(IntPtr hWnd)
    {
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        if (appWindow is not null)
        {
            var width = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportWidth"));
            var height = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportHeight"));
            var scale = App.MainRoot.RasterizationScale;

            appWindow.Resize(new((int)(width * scale), (int)(height * scale)));
            appWindow.MoveInZOrderAtTop();

            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var centeredPosition = appWindow.Position;
                centeredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                centeredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);


                var wk = App.WindowsCounter;
                if (wk > 0)
                {
                    centeredPosition.X += 35;
                    centeredPosition.Y += 35;
                }

                appWindow.Move(centeredPosition);
            }
        }
        App.WindowsCounter += 1;   //counter 1 window add
    }
}
