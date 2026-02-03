
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.DesignSystem.Components;

public sealed partial class AtisPageLayout : UserControl
{
    public AtisPageLayout()
    {
        InitializeComponent();

        DataContextChanged += (_, __) =>
        {
            ApplyDataContext(HeaderPresenter);
            ApplyDataContext(AppBarPresenter);
            ApplyDataContext(ListPresenter);
            ApplyDataContext(DetailPresenter);
        };
    }

    private void ApplyDataContext(ContentPresenter presenter)
    {
        if (presenter?.Content is FrameworkElement fe)
            fe.DataContext = DataContext;
    }

    // -------------------------
    // HEADER
    // -------------------------
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(AtisPageLayout),
            new PropertyMetadata(null, OnHeaderChanged));

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisPageLayout)d;
        control.HeaderPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }

    // -------------------------
    // APPBAR
    // -------------------------
    public object AppBar
    {
        get => GetValue(AppBarProperty);
        set => SetValue(AppBarProperty, value);
    }

    public static readonly DependencyProperty AppBarProperty =
        DependencyProperty.Register(
            nameof(AppBar),
            typeof(object),
            typeof(AtisPageLayout),
            new PropertyMetadata(null, OnAppBarChanged));

    private static void OnAppBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisPageLayout)d;
        control.AppBarPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }

    // -------------------------
    // LIST CONTENT
    // -------------------------
    public object ListContent
    {
        get => GetValue(ListContentProperty);
        set => SetValue(ListContentProperty, value);
    }

    public static readonly DependencyProperty ListContentProperty =
        DependencyProperty.Register(
            nameof(ListContent),
            typeof(object),
            typeof(AtisPageLayout),
            new PropertyMetadata(null, OnListContentChanged));

    private static void OnListContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisPageLayout)d;
        control.ListPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }

    // -------------------------
    // DETAIL CONTENT
    // -------------------------
    public object DetailContent
    {
        get => GetValue(DetailContentProperty);
        set => SetValue(DetailContentProperty, value);
    }

    public static readonly DependencyProperty DetailContentProperty =
        DependencyProperty.Register(
            nameof(DetailContent),
            typeof(object),
            typeof(AtisPageLayout),
            new PropertyMetadata(null, OnDetailContentChanged));

    private static void OnDetailContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (AtisPageLayout)d;
        control.DetailPresenter.Content = e.NewValue;

        if (e.NewValue is FrameworkElement fe)
            fe.DataContext = control.DataContext;
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(
            nameof(IsLoading),
            typeof(bool),
            typeof(AtisPageLayout),
            new PropertyMetadata(false));
}
