using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl81Image;

public sealed partial class Tbl81ImagePreviewWindow : Window
{
    public Tbl81ImagePreviewWindow(Models.Tbl81Image item)
    {
        InitializeComponent();

        PreviewControl.Item = item;
        PreviewControl.CloseRequested += PreviewControl_CloseRequested;

        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Title = GetPreviewTitle(item);
        appWindow.Resize(new SizeInt32(1200, 800));
        appWindow.Move(new PointInt32(100, 80));
    }
    private void PreviewControl_CloseRequested(object? sender, EventArgs e)
    {
        Close();
    }
    private static string GetPreviewTitle(Models.Tbl81Image item)
    {
        var loc = Localizer.Get();

        var raw = item.ImageMimeType;

        var titleKey = "PreviewTitle";

        if (!string.IsNullOrWhiteSpace(raw))
        {
            var value = raw.Trim().ToLowerInvariant().TrimStart('.');

            titleKey = value switch
            {
                "jpg" or "jpeg" or "png" or "bmp" or "gif" or "tif" or "tiff"
                    => "ImagePreviewTitle",

                "mp4" or "wmv" or "avi" or "mov"
                    => "VideoPreviewTitle",

                "mp3" or "wav" or "wma" or "m4a"
                    => "AudioPreviewTitle",

                _ => "PreviewTitle"
            };
        }

        return loc.GetLocalizedString(titleKey);
    }

}

