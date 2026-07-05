
//    Tbl54SupertribusEditControl Skriptdatum:  22.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl54Supertribus;

public sealed partial class Tbl54SupertribusEditControl : UserControl
{
    public Tbl54SupertribusViewModel? ViewModel => DataContext as Tbl54SupertribusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl54SupertribusEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl54Supertribus? Item
    {
        get => (Models.Tbl54Supertribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl54Supertribus),
            typeof(Tbl54SupertribusEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl54SupertribusEditControl)d;
        var item = e.NewValue as Models.Tbl54Supertribus;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl54Supertribus? newItem)
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

    private void SupertribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SupertribusNameBox, Item?.HasSupertribusNameError ?? false);
    }

    private void SupertribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SupertribusNameBox, Item?.HasSupertribusNameError ?? false);
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
