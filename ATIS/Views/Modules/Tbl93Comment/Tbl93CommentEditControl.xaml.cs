using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentEditControl : UserControl
{
    public Tbl93CommentEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  DependencyProperty: Item  (wie Subtribus)
    // ---------------------------------------------------------

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

    // ---------------------------------------------------------
    //  VisualState-Update (wie Subtribus)
    // ---------------------------------------------------------

    private void UpdateValidationState(TextBox box, bool hasError)
    {
        if (box == null)
            return;

        if (hasError)
        {
            VisualStateManager.GoToState(box, "Invalid", true);
            return;
        }

        if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
            VisualStateManager.GoToState(box, "Focused", true);
        else
            VisualStateManager.GoToState(box, "Normal", true);
    }

    // ---------------------------------------------------------
    //  ValidYear
    // ---------------------------------------------------------

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    // ---------------------------------------------------------
    //  Info
    // ---------------------------------------------------------

    private void InfoBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);

    private void InfoBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);
}
