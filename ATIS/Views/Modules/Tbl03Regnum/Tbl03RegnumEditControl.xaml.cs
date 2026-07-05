
//    Tbl03RegnumEditControl Skriptdatum:  30.12.2024  12:32      

namespace ATIS.Views.Modules.Tbl03Regnum;

public sealed partial class Tbl03RegnumEditControl : UserControl
{
    public Tbl03RegnumViewModel? ViewModel => DataContext as Tbl03RegnumViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl03RegnumEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl03Regnum? Item
    {
        get => (Models.Tbl03Regnum?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl03Regnum),
            typeof(Tbl03RegnumEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl03RegnumEditControl)d;
        var item = e.NewValue as Models.Tbl03Regnum;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl03Regnum? newItem)
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

    private void RegnumNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(RegnumNameBox, Item?.HasRegnumNameError ?? false);
    }

    private void RegnumNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(RegnumNameBox, Item?.HasRegnumNameError ?? false);
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
