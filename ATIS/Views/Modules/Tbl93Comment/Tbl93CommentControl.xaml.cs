using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentControl : UserControl
{
    public Tbl93CommentControl()
    {
        InitializeComponent();
    }

    public object? ParentViewModel
    {
        get => GetValue(ParentViewModelProperty);
        set => SetValue(ParentViewModelProperty, value);
    }

    public static readonly DependencyProperty ParentViewModelProperty =
        DependencyProperty.Register(
            nameof(ParentViewModel),
            typeof(object),
            typeof(Tbl93CommentControl),
            new PropertyMetadata(null));
}
