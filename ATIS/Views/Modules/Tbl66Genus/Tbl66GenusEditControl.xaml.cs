using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusEditControl : UserControl
{
    public Tbl66GenusEditControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  DependencyProperty: Item
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
            new PropertyMetadata(null));

    // ---------------------------------------------------------
    //  ComboBox: Infratribus
    // ---------------------------------------------------------

    private void OnInfratribusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;
        if (InfratribusCombo.SelectedValue is int id)
            Item.InfratribusId = id;
    }

    // ---------------------------------------------------------
    //  VisualState-Update
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

        if (box.FocusState == FocusState.Pointer ||
            box.FocusState == FocusState.Keyboard)
        {
            VisualStateManager.GoToState(box, "Focused", true);
        }
        else
        {
            VisualStateManager.GoToState(box, "Normal", true);
        }
    }

    // ---------------------------------------------------------
    //  GenusName
    // ---------------------------------------------------------

    private void GenusNameBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(GenusNameBox, Item?.HasGenusNameError ?? false);

    private void GenusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(GenusNameBox, Item?.HasGenusNameError ?? false);

    // ---------------------------------------------------------
    //  ValidYear
    // ---------------------------------------------------------

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    // ---------------------------------------------------------
    //  AuthorYear
    // ---------------------------------------------------------

    private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);

    private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);


}
