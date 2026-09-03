using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl42SuperfamilyEditControl Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl42Superfamily;

public sealed partial class Tbl42SuperfamilyEditControl : UserControl
{
    public Tbl42SuperfamilyViewModel? ViewModel => DataContext as Tbl42SuperfamilyViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl42SuperfamilyEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl42SuperfamilyEditControl_Loaded;
    }
    private void Tbl42SuperfamilyEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl42Superfamily? Item
    {
        get => (Models.Tbl42Superfamily?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl42Superfamily),
            typeof(Tbl42SuperfamilyEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl42SuperfamilyEditControl)d;
        var item = e.NewValue as Models.Tbl42Superfamily;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl42Superfamily? newItem)
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

    private void SuperfamilyNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SuperfamilyNameBox, Item?.HasSuperfamilyNameError ?? false);
    }

    private void SuperfamilyNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SuperfamilyNameBox, Item?.HasSuperfamilyNameError ?? false);
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

    // ---------------------------------------------------------                   
    // DETAIL TAB VALIDATION
    // ---------------------------------------------------------                   
    private int SelectedTabIndex = 0;

    private void DetailTabButton_Click(object sender, RoutedEventArgs e)
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
        SetTabButtonStyle(EngNameTab, EngNameTabUnderline, SelectedTabIndex == 0);
        SetTabButtonStyle(GerNameTab, GerNameTabUnderline, SelectedTabIndex == 1);
        SetTabButtonStyle(FraNameTab, FraNameTabUnderline, SelectedTabIndex == 2);
        SetTabButtonStyle(SpaNameTab, SpaNameTabUnderline, SelectedTabIndex == 3);
    }
    private void UpdateDetailContentVisibility()
    {
        EngNameGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        GerNameGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        FraNameGrid.Visibility = SelectedTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        SpaNameGrid.Visibility = SelectedTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
    }
}
