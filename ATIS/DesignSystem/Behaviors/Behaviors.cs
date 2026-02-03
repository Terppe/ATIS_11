using Microsoft.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace ATIS.DesignSystem.Behaviors;

public sealed class AtisBindingCommitBehavior : Behavior<FrameworkElement>
{
    public IRelayCommand? Command
    {
        get => (IRelayCommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(IRelayCommand),
            typeof(AtisBindingCommitBehavior), new PropertyMetadata(null));

    protected override void OnAttached()
    {
        base.OnAttached();

        if (Command != null)
            Command.CanExecuteChanged += OnCanExecuteChanged;
    }

    protected override void OnDetaching()
    {
        if (Command != null)
            Command.CanExecuteChanged -= OnCanExecuteChanged;

        base.OnDetaching();
    }

    private void OnCanExecuteChanged(object? sender, EventArgs e)
    {
        CommitBindings(AssociatedObject);
    }

    private void CommitBindings(DependencyObject parent)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);

        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is TextBox tb)
            {
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }

            CommitBindings(child);
        }
    }
}
