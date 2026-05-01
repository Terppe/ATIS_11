

namespace ATIS.DesignSystem.Layout;

public sealed partial class BaseBossPageLayout : UserControl
{
    public BaseBossPageLayout()
    {
        InitializeComponent();
    }

    // HEADER LEFT
    public UIElement Header
    {
        get => (UIElement)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(UIElement),
            typeof(BaseBossPageLayout), new PropertyMetadata(null));

    // APPBAR (Buttons + SearchBox)
    public UIElement AppBar
    {
        get => (UIElement)GetValue(AppBarProperty);
        set => SetValue(AppBarProperty, value);
    }
    public static readonly DependencyProperty AppBarProperty =
        DependencyProperty.Register(nameof(AppBar), typeof(UIElement),
            typeof(BaseBossPageLayout), new PropertyMetadata(null));

    // LIST CONTENT
    public UIElement ListContent
    {
        get => (UIElement)GetValue(ListContentProperty);
        set => SetValue(ListContentProperty, value);
    }
    public static readonly DependencyProperty ListContentProperty =
        DependencyProperty.Register(nameof(ListContent), typeof(UIElement),
            typeof(BaseBossPageLayout), new PropertyMetadata(null));

    // DETAIL CONTENT (FK-MODULE)
    public UIElement DetailContent
    {
        get => (UIElement)GetValue(DetailContentProperty);
        set => SetValue(DetailContentProperty, value);
    }
    public static readonly DependencyProperty DetailContentProperty =
        DependencyProperty.Register(nameof(DetailContent), typeof(UIElement),
            typeof(BaseBossPageLayout), new PropertyMetadata(null));

    //public enum BossPageMode
    //{
    //    Full,   // mit Search, Sort, 8 Spalten
    //    Simple  // für Comments, Synonyms, Literature, etc.
    //}

}
