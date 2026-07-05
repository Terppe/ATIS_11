

//    Tbl75SpeciesEditControl Skriptdatum:  15.12.2025  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesEditControl : UserControl
{
    public Tbl75SpeciesViewModel? ViewModel => DataContext as Tbl75SpeciesViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl75SpeciesEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl75Species? Item
    {
        get => (Models.Tbl75Species?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl75Species),
            typeof(Tbl75SpeciesEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl75SpeciesEditControl)d;
        var item = e.NewValue as Models.Tbl75Species;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl75Species? newItem)
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

    private void SpeciesNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SpeciesNameBox, Item?.HasSpeciesNameError ?? false);
    }

    private void SpeciesNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SpeciesNameBox, Item?.HasSpeciesNameError ?? false);
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
    // AUTHOR YEAR VALIDATION
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
