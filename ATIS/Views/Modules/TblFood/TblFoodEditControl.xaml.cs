
namespace ATIS.Views.Modules.TblFood;

public sealed partial class TblFoodEditControl : UserControl
{
    public TblFoodViewModel? ViewModel => DataContext as TblFoodViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblFoodEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.TblFood? Item
    {
        get => (Models.TblFood?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.TblFood),
            typeof(TblFoodEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TblFoodEditControl)d;
        var item = e.NewValue as Models.TblFood;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.TblFood? newItem)
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

    private void FoodNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(FoodNameBox, Item?.HasFoodNameError ?? false);
    }

    private void FoodNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(FoodNameBox, Item?.HasFoodNameError ?? false);
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
