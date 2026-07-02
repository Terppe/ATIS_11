using System.Diagnostics;
using ATIS.Views.Modules.Tbl60Subtribus;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusEditControl : UserControl
{
    public Tbl63InfratribusEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // ViewModel (optional debugging)
    // ---------------------------------------------------------

    public Tbl63InfratribusViewModel? ViewModel
        => DataContext as Tbl63InfratribusViewModel;

    // ---------------------------------------------------------
    // DependencyProperty: Item
    // ---------------------------------------------------------

    public Models.Tbl63Infratribus? Item
    {
        get => (Models.Tbl63Infratribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl63Infratribus),
            typeof(Tbl63InfratribusEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl63InfratribusEditControl)d;
        var item = e.NewValue as Models.Tbl63Infratribus;

        control.ApplyItem(item);
    }

    private void ApplyItem(Models.Tbl63Infratribus? newItem)
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

    private void InfratribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(InfratribusNameBox, Item?.HasInfratribusNameError ?? false);
    }

    private void InfratribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(InfratribusNameBox, Item?.HasInfratribusNameError ?? false);
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
