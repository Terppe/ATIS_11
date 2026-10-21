


namespace ATIS.Views.Modules.TblMiscellaneous;

public sealed partial class TblMiscellaneousEditControl : UserControl
{
    public TblMiscellaneousViewModel? ViewModel => DataContext as TblMiscellaneousViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblMiscellaneousEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.TblMiscellaneous? Item
    {
        get => (Models.TblMiscellaneous?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.TblMiscellaneous),
            typeof(TblMiscellaneousEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TblMiscellaneousEditControl)d;
        var item = e.NewValue as Models.TblMiscellaneous;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.TblMiscellaneous? newItem)
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
    // Miscellaneous NAME VALIDATION
    // ---------------------------------------------------------                   
    private void MiscellaneousNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(MiscellaneousNameBox, Item?.HasMiscellaneousNameError ?? false);
    }

    private void MiscellaneousNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(MiscellaneousNameBox, Item?.HasMiscellaneousNameError ?? false);
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

    // ---------------------------------------------------------                   
    // ShortInfo NAME VALIDATION
    // ---------------------------------------------------------                   
    private void ShortInfoBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ShortInfoBox, Item?.HasShortInfoError ?? false);
    }

    private void ShortInfoBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ShortInfoBox, Item?.HasShortInfoError ?? false);
    }


}
