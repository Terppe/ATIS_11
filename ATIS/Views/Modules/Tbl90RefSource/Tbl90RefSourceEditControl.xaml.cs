
//    Tbl90RefSourceEditControl Skriptdatum:   06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefSource;

public sealed partial class Tbl90RefSourceEditControl : UserControl
{
    public Tbl90RefSourceViewModel? ViewModel => DataContext as Tbl90RefSourceViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefSourceEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl90RefSource? Item
    {
        get => (Models.Tbl90RefSource?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl90RefSource),
            typeof(Tbl90RefSourceEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl90RefSourceEditControl)d;
        var item = e.NewValue as Models.Tbl90RefSource;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl90RefSource? newItem)
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
    // VALDATINHELPER
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
    // SUBTRIBUSNAME VALIDATION
    // ---------------------------------------------------------                   

    private void RefSourceNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(RefSourceNameBox, Item?.HasRefSourceNameError ?? false);
    }

    private void RefSourceNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(RefSourceNameBox, Item?.HasRefSourceNameError ?? false);
    }
}
