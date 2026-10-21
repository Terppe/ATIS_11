using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl90Reference;

public sealed partial class Tbl90ReferenceEditControl : UserControl
{
    private int SelectedTabIndex = 0;

    public Tbl90ReferenceEditControl()
    {
        InitializeComponent();
        Loaded += OnControlLoaded;
    }

    private void OnControlLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }


    private async void DetailTabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.Tag is string tagText &&
            int.TryParse(tagText, out int index))
        {
            SelectedTabIndex = index;
            UpdateTabButtonStates();
            UpdateDetailContentVisibility();
        }
    }
    private void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }

    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(SpeciesGenusTab, SpeciesGenusTabUnderline, SelectedTabIndex == 0);
        SetTabButtonStyle(TribusTab, TribusTabUnderline, SelectedTabIndex == 1);
        SetTabButtonStyle(FamilyTab, FamilyTabUnderline, SelectedTabIndex == 2);
        SetTabButtonStyle(OrdoTab, OrdoTabUnderline, SelectedTabIndex == 3);
        SetTabButtonStyle(ClassTab, ClassTabUnderline, SelectedTabIndex == 4);
        SetTabButtonStyle(PhylumRegnumTab, PhylumRegnumTabUnderline, SelectedTabIndex == 5);
    }
    private void UpdateDetailContentVisibility()
    {
        SpeciesGenusGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        TribusGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        FamilyGrid.Visibility = SelectedTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        OrderGrid.Visibility = SelectedTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        ClassGrid.Visibility = SelectedTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        PhylumRegnumGrid.Visibility = SelectedTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
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

        if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
            VisualStateManager.GoToState(box, "Focused", true);
        else
            VisualStateManager.GoToState(box, "Normal", true);
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

        if (control.Item == null)
            return;

        if (!control.IsLoaded)
            return;

        control.UpdateComboVisibility();
    }
    //private static async void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    var control = (Tbl90ReferenceEditControl)d;
    //    if (!control.IsLoaded) return;
    //    control.UpdateAllValidationStates();
    //}

    // ---------------------------------------------------------
    //  Validation State Updates
    // ---------------------------------------------------------

    private void UpdateAllValidationStates()
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
        UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);
    }

    // ---------------------------------------------------------
    //  DependencyProperty: ViewModel
    // ---------------------------------------------------------

    public Tbl90ReferenceViewModel? ViewModel
    {
        get => (Tbl90ReferenceViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(Tbl90ReferenceViewModel),
            typeof(Tbl90ReferenceEditControl),
            new PropertyMetadata(null));

    // ---------------------------------------------------------
    //  VisualState-Update (1:1 wie Comment)
    // ---------------------------------------------------------

    //private void UpdateValidationState(TextBox box, bool hasError)
    //{
    //    if (box == null)
    //        return;

    //    if (hasError)
    //    {
    //        VisualStateManager.GoToState(box, "Invalid", true);
    //        return;
    //    }

    //    if (box.FocusState == FocusState.Pointer || box.FocusState == FocusState.Keyboard)
    //        VisualStateManager.GoToState(box, "Focused", true);
    //    else
    //        VisualStateManager.GoToState(box, "Normal", true);
    //}

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
