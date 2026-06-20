
using ATIS.Views.Authentication;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusEditControl : UserControl
{
    public Tbl63InfratribusEditControl()
    {
        InitializeComponent();
    }
    public Tbl63InfratribusViewModel? ViewModel => DataContext as Tbl63InfratribusViewModel;

    public Models.Tbl63Infratribus? Item
    {
        get => (Models.Tbl63Infratribus?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl63Infratribus),
            typeof(Tbl63InfratribusEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = e.NewValue as Models.Tbl63Infratribus;

        var control = (Tbl63InfratribusEditControl)d;
        control.ApplyItem(item);
    }

    private void ApplyItem(Models.Tbl63Infratribus? newItem)
    {
        // NICHT den DataContext des Controls überschreiben!
        // Stattdessen nur das Item setzen.
        // Die Bindings im XAML müssen auf Item.XXX zeigen.

        // Falls du ein Root-Grid hast:
        //  ItemRoot.DataContext = newItem;
        // Wenn du kein ItemRoot hast, lass ApplyItem LEER:
    }



    private void OnSubtribusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;
        if (SubtribusCombo.SelectedValue is int id)
            Item.SubtribusId = id;
    }

    // ---------------------------------------------------------
    //  VisualState-Update (wie Subtribus / Comment)
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
    //  InfratribusName
    // ---------------------------------------------------------

    private void InfratribusNameBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(InfratribusNameBox, Item?.HasInfratribusNameError ?? false);

    private void InfratribusNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(InfratribusNameBox, Item?.HasInfratribusNameError ?? false);

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
