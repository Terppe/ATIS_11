using ATIS.Views.Modules.Tbl60Subtribus;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Modules.Tbl57Tribus;

public sealed partial class Tbl57TribusEditControl : UserControl
{
    public Tbl57TribusEditControl()
    {
        InitializeComponent();
    }
    public Tbl57TribusViewModel? ViewModel => DataContext as Tbl57TribusViewModel;

    // ---------------------------------------------------------
    //  DependencyProperty: Item
    // ---------------------------------------------------------

    public Models.Tbl57Tribus? Item
    {
        get => (Models.Tbl57Tribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl57Tribus),
            typeof(Tbl57TribusEditControl),
            new PropertyMetadata(new Models.Tbl57Tribus()));


    private void OnSupertribusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;
        if (SupertribusCombo.SelectedValue is int id)
            Item.SupertribusId = id;
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

    private void TribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(TribusNameBox, Item?.HasTribusNameError ?? false);
    }
    private void TribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(TribusNameBox, Item?.HasTribusNameError ?? false);
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
