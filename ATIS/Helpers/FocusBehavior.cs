using Microsoft.Xaml.Interactivity;

namespace ATIS.Helpers;
public class FocusBehavior : Behavior<Control>
{
    protected override void OnAttached()
    {
        AssociatedObject.Loaded += AssociatedObject_Loaded;
        base.OnAttached();
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
        AssociatedObject.Loaded -= AssociatedObject_Loaded;
        if (HasInitialFocus || IsFocused)
        {
            GotFocus();
        }
    }

    private void GotFocus()
    {
        AssociatedObject.Focus(FocusState.Programmatic);
        if (IsSelectAll)
        {
            if (AssociatedObject is TextBox)
            {
                (AssociatedObject as TextBox)?.SelectAll();
            }
            else if (AssociatedObject is PasswordBox)
            {
                (AssociatedObject as PasswordBox)?.SelectAll();
            }
            else if (AssociatedObject is TextBox)
            {
                (AssociatedObject as TextBox)?.SelectAll();
            }
        }
    }

    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(FocusBehavior),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    if ((bool)e.NewValue)
                    {
                        ((FocusBehavior)d).GotFocus();
                    }
                }));

    public bool IsFocused
    {
        get
        {
            if ((bool)GetValue(IsFocusedProperty))
            {
                return true;
            }

            return false;
        }
        set => SetValue(IsFocusedProperty, value);
    }

    public static readonly DependencyProperty HasInitialFocusProperty =
        DependencyProperty.Register(
            "HasInitialFocus",
            typeof(bool),
            typeof(FocusBehavior),
            new PropertyMetadata(false, null));

    public bool HasInitialFocus
    {
        get
        {
            if ((bool)GetValue(HasInitialFocusProperty))
            {
                return true;
            }

            return false;
        }
        set => SetValue(HasInitialFocusProperty, value);
    }

    public static readonly DependencyProperty IsSelectAllProperty =
        DependencyProperty.Register(
            "IsSelectAll",
            typeof(bool),
            typeof(FocusBehavior),
            new PropertyMetadata(false, null));

    public bool IsSelectAll
    {
        get
        {
            if ((bool)GetValue(IsSelectAllProperty))
            {
                return true;
            }

            return false;
        }
        set => SetValue(IsSelectAllProperty, value);
    }

}
