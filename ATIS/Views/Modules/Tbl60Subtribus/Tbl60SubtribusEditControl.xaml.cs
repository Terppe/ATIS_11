
namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusEditControl : UserControl
{
    public Tbl60SubtribusEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  DependencyProperty: Item
    // ---------------------------------------------------------

    public Models.Tbl60Subtribus? Item
    {
        get => (Models.Tbl60Subtribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl60Subtribus),
            typeof(Tbl60SubtribusEditControl),
            new PropertyMetadata(null));

    private void OnTribusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;
        if (TribusCombo.SelectedValue is int id)
            Item.TribusId = id;
    }

    private void UpdateValidationState(TextBox box, bool hasError)
    {
        if (box == null)
            return;

        if (hasError)
        {
            VisualStateManager.GoToState(box, "Invalid", true);
            return;
        }

        // Kein Fehler → Fokus prüfen
        if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
            VisualStateManager.GoToState(box, "Focused", true);
        else
            VisualStateManager.GoToState(box, "Normal", true);
    }

    private void SubtribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SubtribusNameBox, Item?.HasSubtribusNameError ?? false);
    }
    private void SubtribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SubtribusNameBox, Item?.HasSubtribusNameError ?? false);
    }

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }
    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }


    private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    }
}   

    


