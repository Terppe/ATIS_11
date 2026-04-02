using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ATIS.DesignSystem.Controls.ListHeader;

public sealed partial class TwoColumnHeaderControl : Grid
{
    public TwoColumnHeaderControl()
    {
        Margin = new Thickness(0, 0, 0, 6);
        Padding = new Thickness(4);
        Background = new SolidColorBrush(Color.FromArgb(255, 0xEF, 0xEF, 0xEF));
        CornerRadius = new CornerRadius(4);

        InitializeComponent();
    }

    // LEFT
    public object Left
    {
        get => GetValue(LeftProperty);
        set => SetValue(LeftProperty, value);
    }

    public static readonly DependencyProperty LeftProperty =
        DependencyProperty.Register(
            nameof(Left),
            typeof(object),
            typeof(TwoColumnHeaderControl),
            new PropertyMetadata(null, OnLeftChanged));

    private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TwoColumnHeaderControl)d;
        control.LeftContent.Content = e.NewValue;
    }

    // RIGHT
    public object Right
    {
        get => GetValue(RightProperty);
        set => SetValue(RightProperty, value);
    }

    public static readonly DependencyProperty RightProperty =
        DependencyProperty.Register(
            nameof(Right),
            typeof(object),
            typeof(TwoColumnHeaderControl),
            new PropertyMetadata(null, OnRightChanged));

    private static void OnRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TwoColumnHeaderControl)d;
        control.RightContent.Content = e.NewValue;
    }
}
