
//    Tbl87GeographicEditControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public sealed partial class Tbl87GeographicEditControl : UserControl
{
    public Tbl87GeographicViewModel? ViewModel => DataContext as Tbl87GeographicViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl87GeographicEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl87Geographic? Item
    {
        get => (Models.Tbl87Geographic?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl87Geographic),
            typeof(Tbl87GeographicEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl87GeographicEditControl)d;
        var item = e.NewValue as Models.Tbl87Geographic;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl87Geographic? newItem)
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
    // Geographic NAME VALIDATION
    // ---------------------------------------------------------                   

    //private void GeographicNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    UpdateValidationState(GeographicNameBox, Item?.HasGeographicNameError ?? false);
    //}

    //private void GeographicNameBox_OnLoaded(object sender, RoutedEventArgs e)
    //{
    //    UpdateValidationState(GeographicNameBox, Item?.HasGeographicNameError ?? false);
    //}

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
