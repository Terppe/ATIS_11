namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentEditControl : UserControl
{
    public Tbl93CommentEditControl()
    {
        InitializeComponent();
    }

    public Models.Tbl93Comment? Item
    {
        get => (Models.Tbl93Comment?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl93Comment),
            typeof(Tbl93CommentEditControl),
            new PropertyMetadata(null));
}
