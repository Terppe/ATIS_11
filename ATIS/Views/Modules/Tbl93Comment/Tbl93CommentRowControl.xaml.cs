using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentRowControl : UserControl
{
    public Tbl93CommentRowControl()
    {
        InitializeComponent();
    }

    public Models.Tbl93Comment Comment
    {
        get => (Models.Tbl93Comment)GetValue(CommentProperty);
        set => SetValue(CommentProperty, value);
    }

    public static readonly DependencyProperty CommentProperty =
        DependencyProperty.Register(
            nameof(Comment),
            typeof(Models.Tbl93Comment),
            typeof(Tbl93CommentRowControl),
            new PropertyMetadata(null));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(Tbl93CommentRowControl),
            new PropertyMetadata(false, OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl93CommentRowControl)d;
        control.UpdateSelectionState();
    }

    private void UpdateSelectionState()
    {
        VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Normal", true);
    }

    private void Root_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (!IsSelected)
            VisualStateManager.GoToState(this, "Hover", true);
    }

    private void Root_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!IsSelected)
            VisualStateManager.GoToState(this, "Normal", true);
    }

    private void Root_Tapped(object sender, TappedRoutedEventArgs e)
    {
        CommentTapped?.Invoke(this, Comment);
    }

    public event EventHandler<Models.Tbl93Comment>? CommentTapped;

    public event EventHandler<Models.Tbl93Comment>? NewRequested;
    public event EventHandler<Models.Tbl93Comment>? DeleteRequested;

    private void New_Click(object sender, RoutedEventArgs e)
        => NewRequested?.Invoke(this, Comment);

    private void Delete_Click(object sender, RoutedEventArgs e)
        => DeleteRequested?.Invoke(this, Comment);

    public Tbl93CommentViewModel ViewModel
    {
        get => (Tbl93CommentViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(Tbl93CommentViewModel),
            typeof(Tbl93CommentRowControl),
            new PropertyMetadata(null));

}
