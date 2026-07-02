

using System.Diagnostics;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusEditControl : UserControl
{
    public Tbl60SubtribusEditControl()
    {
        InitializeComponent();
    }

    public Tbl60SubtribusViewModel? ViewModel
        => DataContext as Tbl60SubtribusViewModel;

    // ---------------------------------------------------------
    // DependencyProperty: Item
    // ---------------------------------------------------------

    public Models.Tbl60Subtribus? Item
    {
        get => (Models.Tbl60Subtribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl60Subtribus),
            typeof(Tbl60SubtribusEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl60SubtribusEditControl)d;
        var item = e.NewValue as Models.Tbl60Subtribus;

        control.ApplyItem(item);
    }

    private void ApplyItem(Models.Tbl60Subtribus? newItem)
    {
        // ⭐ WICHTIG:
        // KEIN DataContext-Wechsel!
        // KEIN Boss-Workflow!
        // KEIN Parent-Workflow!
        //
        // Bindings im XAML zeigen direkt auf Item.XXX
        // Deshalb bleibt diese Methode LEER.
    }

    // ---------------------------------------------------------
    // Validation Helper
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
    // SubtribusName Validation
    // ---------------------------------------------------------

    private void SubtribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SubtribusNameBox, Item?.HasSubtribusNameError ?? false);
    }

    private void SubtribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SubtribusNameBox, Item?.HasSubtribusNameError ?? false);
    }

    // ---------------------------------------------------------
    // ValidYear Validation
    // ---------------------------------------------------------

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    // ---------------------------------------------------------
    // AuthorYear Validation
    // ---------------------------------------------------------

    private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }

    private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }
}




