
//    Tbl90RefAuthorEditControl Skriptdatum:  11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public sealed partial class Tbl90RefAuthorEditControl : UserControl
{
    public Tbl90RefAuthorViewModel? ViewModel => DataContext as Tbl90RefAuthorViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------  

    public Tbl90RefAuthorEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl90RefAuthor? Item
    {
        get => (Models.Tbl90RefAuthor?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl90RefAuthor),
            typeof(Tbl90RefAuthorEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl90RefAuthorEditControl)d;
        var item = e.NewValue as Models.Tbl90RefAuthor;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl90RefAuthor? newItem)
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
    // RefAuthor NAME VALIDATION
    // ---------------------------------------------------------                   

    private void RefAuthorNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(RefAuthorNameBox, Item?.HasRefAuthorNameError ?? false);
    }

    private void RefAuthorNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(RefAuthorNameBox, Item?.HasRefAuthorNameError ?? false);
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
    // PUBLICATION YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void PublicationYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(PublicationYearBox, Item?.HasPublicationYearError ?? false);
    }

    private void PublicationYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(PublicationYearBox, Item?.HasPublicationYearError ?? false);
    }
}

