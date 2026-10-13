using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using WinRT.Interop;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Modules.Tbl81Image;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl81ImagePreviewWindow : Window
{
    public Tbl81ImagePreviewWindow(Models.Tbl81Image item)
    {
        this.InitializeComponent();

        PreviewControl.Item = item;

        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Title = GetPreviewTitle(item);
        appWindow.Resize(new SizeInt32(1200, 800));
        appWindow.Move(new PointInt32(100, 80));
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

