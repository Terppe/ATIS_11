using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl36SubordoEditControl Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl36Subordo;

public sealed partial class Tbl36SubordoEditControl : UserControl
{
    public Tbl36SubordoViewModel? ViewModel => DataContext as Tbl36SubordoViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl36SubordoEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl36SubordoEditControl_Loaded;
    }
    private void Tbl36SubordoEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl36Subordo? Item
    {
        get => (Models.Tbl36Subordo?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl36Subordo),
            typeof(Tbl36SubordoEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl36SubordoEditControl)d;
        var item = e.NewValue as Models.Tbl36Subordo;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl36Subordo? newItem)
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

    private void SubordoNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SubordoNameBox, Item?.HasSubordoNameError ?? false);
    }

    private void SubordoNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SubordoNameBox, Item?.HasSubordoNameError ?? false);
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
