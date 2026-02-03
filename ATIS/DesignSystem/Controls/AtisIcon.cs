using ATIS.DesignSystem.Enums;
using ATIS.DesignSystem.Icons;
using ATIS.DesignSystem.Services;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ATIS.DesignSystem.Controls;

public sealed class AtisIcon : ContentControl
{
    public AtisIcon()
    {
        DefaultStyleKey = typeof(AtisIcon);
    }

    public AtisIconKind Kind
    {
        get => (AtisIconKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(AtisIconKind),
            typeof(AtisIcon), new PropertyMetadata(AtisIconKind.None, OnKindChanged));

    private static void OnKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AtisIcon icon)
            icon.UpdateIcon();
    }

    private void UpdateIcon()
    {
        Content = AtisIconService.GetIcon(Kind);
    }
}

//public enum AtisIconMode
//{
//    Color,
//    Mono
//}

//public sealed class AtisIcon : ContentControl
//{
//    public string IconKey
//    {
//        get => (string)GetValue(IconKeyProperty);
//        set => SetValue(IconKeyProperty, value);
//    }

//    public static readonly DependencyProperty IconKeyProperty =
//        DependencyProperty.Register(nameof(IconKey), typeof(string), typeof(AtisIcon),
//            new PropertyMetadata(null, OnIconChanged));

//    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//        if (d is AtisIcon control)
//            control.UpdateIcon();
//    }

//    private void UpdateIcon()
//    {
//        if (string.IsNullOrEmpty(IconKey))
//            return;

//        string resourceKey = $"Icon.{IconKey}.Color";

//        if (Application.Current.Resources.TryGetValue(resourceKey, out var icon))
//        {
//            if (icon is BitmapImage bitmap)
//            {
//                Content = new ImageIcon
//                {
//                    Source = bitmap,
//                    Width = 20,
//                    Height = 20
//                };
//            }
//            else if (icon is FontIcon fontIcon)
//            {
//                fontIcon.FontSize = 16;
//                Content = fontIcon;
//            }
//        }
//    }

//    public AtisIcon()
//    {
//        DefaultStyleKey = typeof(AtisIcon);
//    }
//}
