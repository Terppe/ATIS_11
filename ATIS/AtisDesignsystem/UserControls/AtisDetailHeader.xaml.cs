
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.AtisDesignsystem.UserControls;

public sealed partial class AtisDetailHeader : UserControl
{
    public AtisDetailHeader()
    {
        InitializeComponent();

        DataContextChanged += (_, __) =>
        {
            ApplyDataContextToContent();
        };
    }

    private void ApplyDataContextToContent()
    {
        if (LeftPresenter.Content is FrameworkElement left)
            left.DataContext = DataContext;

        if (RightPresenter.Content is FrameworkElement right)
            right.DataContext = DataContext;
    }

    // LEFT CONTENT
    public object LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public static readonly DependencyProperty LeftContentProperty =
        DependencyProperty.Register(
            nameof(LeftContent),
            typeof(object),
            typeof(AtisDetailHeader),
            new PropertyMetadata(null, OnLeftContentChanged));

    private static void OnLeftContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisDetailHeader)d;
        control.LeftPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }

    // RIGHT CONTENT
    public object RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public static readonly DependencyProperty RightContentProperty =
        DependencyProperty.Register(
            nameof(RightContent),
            typeof(object),
            typeof(AtisDetailHeader),
            new PropertyMetadata(null, OnRightContentChanged));

    private static void OnRightContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisDetailHeader)d;
        control.RightPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }
}
