using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.DesignSystem.Components;

public sealed partial class Base4List4DetailPageLayout : UserControl
{
    public Base4List4DetailPageLayout()
    {
        InitializeComponent();

        DataContextChanged += (_, __) =>
        {
            Apply(HeaderPresenter);
            Apply(AppBarPresenter);

            Apply(List1Presenter);
            Apply(List2Presenter);
            Apply(List3Presenter);
            Apply(List4Presenter);

            Apply(Detail1Presenter);
            Apply(Detail2Presenter);
            Apply(Detail3Presenter);
            Apply(Detail4Presenter);
        };
    }

    private void Apply(ContentPresenter presenter)
    {
        if (presenter?.Content is FrameworkElement fe)
            fe.DataContext = DataContext;
    }

    // ---------------------------------------------------------
    // HEADER
    // ---------------------------------------------------------
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.HeaderPresenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    // ---------------------------------------------------------
    // APPBAR
    // ---------------------------------------------------------
    public object AppBar
    {
        get => GetValue(AppBarProperty);
        set => SetValue(AppBarProperty, value);
    }

    public static readonly DependencyProperty AppBarProperty =
        DependencyProperty.Register(
            nameof(AppBar),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.AppBarPresenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    // ---------------------------------------------------------
    // LIST 1–4
    // ---------------------------------------------------------
    public object List1Content
    {
        get => GetValue(List1ContentProperty);
        set => SetValue(List1ContentProperty, value);
    }

    public static readonly DependencyProperty List1ContentProperty =
        DependencyProperty.Register(
            nameof(List1Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.List1Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object List2Content
    {
        get => GetValue(List2ContentProperty);
        set => SetValue(List2ContentProperty, value);
    }

    public static readonly DependencyProperty List2ContentProperty =
        DependencyProperty.Register(
            nameof(List2Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.List2Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object List3Content
    {
        get => GetValue(List3ContentProperty);
        set => SetValue(List3ContentProperty, value);
    }

    public static readonly DependencyProperty List3ContentProperty =
        DependencyProperty.Register(
            nameof(List3Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.List3Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object List4Content
    {
        get => GetValue(List4ContentProperty);
        set => SetValue(List4ContentProperty, value);
    }

    public static readonly DependencyProperty List4ContentProperty =
        DependencyProperty.Register(
            nameof(List4Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.List4Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    // ---------------------------------------------------------
    // DETAIL 1–4
    // ---------------------------------------------------------
    public object Detail1Content
    {
        get => GetValue(Detail1ContentProperty);
        set => SetValue(Detail1ContentProperty, value);
    }

    public static readonly DependencyProperty Detail1ContentProperty =
        DependencyProperty.Register(
            nameof(Detail1Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.Detail1Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object Detail2Content
    {
        get => GetValue(Detail2ContentProperty);
        set => SetValue(Detail2ContentProperty, value);
    }

    public static readonly DependencyProperty Detail2ContentProperty =
        DependencyProperty.Register(
            nameof(Detail2Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.Detail2Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object Detail3Content
    {
        get => GetValue(Detail3ContentProperty);
        set => SetValue(Detail3ContentProperty, value);
    }

    public static readonly DependencyProperty Detail3ContentProperty =
        DependencyProperty.Register(
            nameof(Detail3Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.Detail3Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));

    public object Detail4Content
    {
        get => GetValue(Detail4ContentProperty);
        set => SetValue(Detail4ContentProperty, value);
    }

    public static readonly DependencyProperty Detail4ContentProperty =
        DependencyProperty.Register(
            nameof(Detail4Content),
            typeof(object),
            typeof(Base4List4DetailPageLayout),
            new PropertyMetadata(null, (d, e) =>
            {
                var c = (Base4List4DetailPageLayout)d;
                c.Detail4Presenter.Content = e.NewValue;
                if (e.NewValue is FrameworkElement fe)
                    fe.DataContext = c.DataContext;
            }));
}
