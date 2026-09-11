
//    Tbl90RefExpertEditControl Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public sealed partial class Tbl90RefExpertEditControl : UserControl
{
    public Tbl90RefExpertViewModel? ViewModel => DataContext as Tbl90RefExpertViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefExpertEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl90RefExpert? Item
    {
        get => (Models.Tbl90RefExpert?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl90RefExpert),
            typeof(Tbl90RefExpertEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl90RefExpertEditControl)d;
        var item = e.NewValue as Models.Tbl90RefExpert;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl90RefExpert? newItem)
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
    // VALIDATION HELPER
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
    // RefExpert NAME VALIDATION
    // ---------------------------------------------------------                   

    private void RefExpertNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(RefExpertNameBox, Item?.HasRefExpertNameError ?? false);
    }

    private void RefExpertNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(RefExpertNameBox, Item?.HasRefExpertNameError ?? false);
    }

    // ---------------------------------------------------------                   
    // VALID YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }


}
