
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    TblCountryEditControl Skriptdatum:   01.10.2026 12:32      

namespace ATIS.Views.Modules.TblCountry;

public sealed partial class TblCountryEditControl : UserControl
{
    public TblCountryViewModel? ViewModel => DataContext as TblCountryViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblCountryEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += TblCountryEditControl_Loaded;
    }
    private void TblCountryEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.TblCountry? Item
    {
        get => (Models.TblCountry?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.TblCountry),
            typeof(TblCountryEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TblCountryEditControl)d;
        var item = e.NewValue as Models.TblCountry;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.TblCountry? newItem)
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
    // Country NAME VALIDATION
    // ---------------------------------------------------------                   

    private void CountryNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(CountryNameBox, Item?.HasNameError ?? false);
    }

    private void CountryNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(CountryNameBox, Item?.HasNameError ?? false);
    }

    // ---------------------------------------------------------                   
    // Continent VALIDATION
    // ---------------------------------------------------------                   

    private void ContinentBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ContinentBox, Item?.HasContinentError ?? false);
    }

    private void ContinentBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ContinentBox, Item?.HasContinentError ?? false);
    }

    // ---------------------------------------------------------                   
    // VALID YEAR VALIDATION
    // ---------------------------------------------------------                   

    private void RegexBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(RegexBox, Item?.HasRegexError ?? false);  
    }

    private void RegexBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(RegexBox, Item?.HasRegexError ?? false);
    }

    // ---------------------------------------------------------                   
    // AUTHOR YEAR VALIDATION
    // ---------------------------------------------------------                   

    //private void AuthorYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    //}

    //private void AuthorYearBox_OnLoaded(object sender, RoutedEventArgs e)
    //{
    //    UpdateValidationState(AuthorYearBox, Item?.HasAuthorYearError ?? false);
    //}

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
        //SetTabButtonStyle(EngNameTab, EngNameTabUnderline, SelectedTabIndex == 0);
        //SetTabButtonStyle(GerNameTab, GerNameTabUnderline, SelectedTabIndex == 1);
        //SetTabButtonStyle(FraNameTab, FraNameTabUnderline, SelectedTabIndex == 2);
        //SetTabButtonStyle(SpaNameTab, SpaNameTabUnderline, SelectedTabIndex == 3);
    }
    private void UpdateDetailContentVisibility()
    {
        //EngNameGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        //GerNameGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        //FraNameGrid.Visibility = SelectedTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        //SpaNameGrid.Visibility = SelectedTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
    }




}
