using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace ATIS.Views.Modules.Tbl90Reference;

public sealed partial class Tbl90ReferenceEditControl : UserControl
{
    public Tbl90ReferenceEditControl()
    {
        InitializeComponent();

        Debug.WriteLine(
            $"ReferenceEditControl: Expert={Item?.RefExpertId}, Source={Item?.RefSourceId}, Author={Item?.RefAuthorId}, ");
    }

    // ---------------------------------------------------------
    //  DependencyProperty: Item  (wie Comment)
    // ---------------------------------------------------------

    public Models.Tbl90Reference? Item
    {
        get => (Models.Tbl90Reference?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl90Reference),
            typeof(Tbl90ReferenceEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl90ReferenceEditControl)d;
        control.UpdateComboVisibility();
    }

    // ---------------------------------------------------------
    //  VisualState-Update (1:1 wie Comment)
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
    //  ValidYear
    // ---------------------------------------------------------

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);

    // ---------------------------------------------------------
    //  Info
    // ---------------------------------------------------------

    private void InfoBox_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);

    private void InfoBox_OnTextChanged(object sender, TextChangedEventArgs e)
        => UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);

    // ---------------------------------------------------------
    //  Comboboxen anzeigen
    // ---------------------------------------------------------
    private void UpdateComboVisibility()
    {
        if (Item == null)
        {
            ExpertPanel.Visibility = Visibility.Visible;
            SourcePanel.Visibility = Visibility.Visible;
            AuthorPanel.Visibility = Visibility.Visible;
            return;
        }

        // Neuer Datensatz: alles null → alle sichtbar
        if (Item.RefExpertId == null &&
            Item.RefSourceId == null &&
            Item.RefAuthorId == null)
        {
            ExpertPanel.Visibility = Visibility.Visible;
            SourcePanel.Visibility = Visibility.Visible;
            AuthorPanel.Visibility = Visibility.Visible;
            return;
        }

        // Expert gewählt
        if (Item.RefExpertId != null)
        {
            ExpertPanel.Visibility = Visibility.Visible;
            SourcePanel.Visibility = Visibility.Collapsed;
            AuthorPanel.Visibility = Visibility.Collapsed;
            return;
        }

        // Source gewählt
        if (Item.RefSourceId != null)
        {
            ExpertPanel.Visibility = Visibility.Collapsed;
            SourcePanel.Visibility = Visibility.Visible;
            AuthorPanel.Visibility = Visibility.Collapsed;
            return;
        }

        // Author gewählt
        if (Item.RefAuthorId != null)
        {
            ExpertPanel.Visibility = Visibility.Collapsed;
            SourcePanel.Visibility = Visibility.Collapsed;
            AuthorPanel.Visibility = Visibility.Visible;
            return;
        }
    }

    //---------------------------------------------------------
    private void OnRefExpertChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;

        if (Item.RefExpertId != null)
        {
            Item.RefSourceId = null;
            Item.RefAuthorId = null;
        }

        UpdateComboVisibility();
    }

    private void OnRefSourceChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;

        if (Item.RefSourceId != null)
        {
            Item.RefExpertId = null;
            Item.RefAuthorId = null;
        }

        UpdateComboVisibility();
    }

    private void OnRefAuthorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Item == null) return;

        if (Item.RefAuthorId != null)
        {
            Item.RefExpertId = null;
            Item.RefSourceId = null;
        }

        UpdateComboVisibility();
    }
}
