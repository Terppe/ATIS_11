
//    Tbl66GenusEditControl Skriptdatum:  13.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusEditControl : UserControl
{
    public Tbl66GenusViewModel? ViewModel => DataContext as Tbl66GenusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl66GenusEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl66Genus? Item
    {
        get => (Models.Tbl66Genus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl66Genus),
            typeof(Tbl66GenusEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl66GenusEditControl)d;
        var item = e.NewValue as Models.Tbl66Genus;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl66Genus? newItem)
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

    private void GenusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(GenusNameBox, Item?.HasGenusNameError ?? false);
    }

    private void GenusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(GenusNameBox, Item?.HasGenusNameError ?? false);
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
