using System.Xml;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;


//    Tbl75SpeciesEditControl Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesEditControl : UserControl
{
    public Tbl75SpeciesViewModel? ViewModel => DataContext as Tbl75SpeciesViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl75SpeciesEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl75SpeciesEditControl_Loaded;
    }
    private void Tbl75SpeciesEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    // DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl75Species? Item
    {
        get => (Models.Tbl75Species?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl75Species),
            typeof(Tbl75SpeciesEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl75SpeciesEditControl)d;
        var item = e.NewValue as Models.Tbl75Species;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                   
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl75Species? newItem)
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

    private void SpeciesNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SpeciesNameBox, Item?.HasSpeciesNameError ?? false);
    }

    private void SpeciesNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SpeciesNameBox, Item?.HasSpeciesNameError ?? false);
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
    // IMPORTER VALIDATION
    // ---------------------------------------------------------                   
    private void ImporterBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ImporterBox, Item?.HasImporterError ?? false);
    }

    private void ImporterBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ImporterBox, Item?.HasImporterError ?? false);
    }

    // ---------------------------------------------------------                   
    // TRADENAME VALIDATION
    // ---------------------------------------------------------                   
    private void TradeNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(TradeNameBox, Item?.HasTradeNameError ?? false);
    }

    private void TradeNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(TradeNameBox, Item?.HasTradeNameError ?? false);
    }

    // ---------------------------------------------------------                   
    // IMPORTING YEAR VALIDATION
    // ---------------------------------------------------------                   
    private void ImportingYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ImportingYearBox, Item?.HasImportingYearError ?? false);
    }

    private void ImportingYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ImportingYearBox, Item?.HasImportingYearError ?? false);
    }
    // ---------------------------------------------------------                   
    // SUBSPECIES VALIDATION
    // ---------------------------------------------------------                   
    private void SubspeciesBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(SubspeciesBox, Item?.HasSubspeciesError ?? false);
    }

    private void SubspeciesBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(SubspeciesBox, Item?.HasSubspeciesError ?? false);
    }

    // ---------------------------------------------------------                   
    // DIVERS VALIDATION
    // ---------------------------------------------------------                   

    private void DiversBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(DiversBox, Item?.HasDiversError ?? false);
    }

    private void DiversBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(DiversBox, Item?.HasDiversError ?? false);
    }

    // ---------------------------------------------------------                   
    // LNUMBER VALIDATION
    // ---------------------------------------------------------                   
    private void LNumberBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(LNumberBox, Item?.HasLNumberError ?? false);
    }

    private void LNumberBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(LNumberBox, Item?.HasLNumberError ?? false);
    }


    // ---------------------------------------------------------                   
    // LORIGIN VALIDATION
    // ---------------------------------------------------------                   
    private void LOriginBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(LOriginBox, Item?.HasLOriginError ?? false);
    }

    private void LOriginBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(LOriginBox, Item?.HasLOriginError ?? false);
    }

    // ---------------------------------------------------------                   
    // LDA NUMBER VALIDATION
    // ---------------------------------------------------------                   

    private void LdaNumberBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(LdaNumberBox, Item?.HasLDANumberError ?? false);
    }

    private void LdaNumberBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(LdaNumberBox, Item?.HasLDANumberError ?? false);
    }

    // ---------------------------------------------------------                   
    // LDAORIGIN VALIDATION
    // ---------------------------------------------------------                   

    private void LdaOriginBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(LdaOriginBox, Item?.HasLDAOriginError ?? false);
    }

    private void LdaOriginBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(LdaOriginBox, Item?.HasLDAOriginError ?? false);
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


    private void UpdateTabButtonStates()
    {
        SetTabButtonStyle(ImporterTab, ImportTabUnderline, SelectedTabIndex == 0);
        SetTabButtonStyle(SizeTab, SizeTabUnderline, SelectedTabIndex == 1);
        SetTabButtonStyle(FoodTab, FoodTabUnderline, SelectedTabIndex == 2);
        SetTabButtonStyle(TechnicTab, TechnicTabUnderline, SelectedTabIndex == 3);
        SetTabButtonStyle(HusbandryTab, HusbandryTabUnderline, SelectedTabIndex == 4);
        SetTabButtonStyle(BreedingTab, BreedingTabUnderline, SelectedTabIndex == 5);
        SetTabButtonStyle(BuiltTab, BuiltTabUnderline, SelectedTabIndex == 6);
        SetTabButtonStyle(SpecialTab, SpecialTabUnderline, SelectedTabIndex == 7);
        SetTabButtonStyle(ReproductionTab, ReproductionTabUnderline, SelectedTabIndex == 8);
        SetTabButtonStyle(CultureTab, CultureTabUnderline, SelectedTabIndex == 9);
        SetTabButtonStyle(GlobalTab, GlobalTabUnderline, SelectedTabIndex == 10);
    }

    private void UpdateDetailContentVisibility()
    {
        ImporterGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        SizeGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
        FoodGrid.Visibility = SelectedTabIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
        TechnicGrid.Visibility = SelectedTabIndex == 3 ? Visibility.Visible : Visibility.Collapsed;
        HusbandryGrid.Visibility = SelectedTabIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        BreedingGrid.Visibility = SelectedTabIndex == 5 ? Visibility.Visible : Visibility.Collapsed;
        BuiltGrid.Visibility = SelectedTabIndex == 6 ? Visibility.Visible : Visibility.Collapsed;
        SpecialGrid.Visibility = SelectedTabIndex == 7 ? Visibility.Visible : Visibility.Collapsed;
        ReproductionGrid.Visibility = SelectedTabIndex == 8 ? Visibility.Visible : Visibility.Collapsed;
        CultureGrid.Visibility = SelectedTabIndex == 9 ? Visibility.Visible : Visibility.Collapsed;
        GlobalGrid.Visibility = SelectedTabIndex == 10 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void SetTabButtonStyle(Button button, Border underline, bool isSelected)
    {
   //     button.Background = new SolidColorBrush(Colors.GhostWhite);
        button.Background = new SolidColorBrush(Colors.Transparent);
        button.FontWeight = isSelected ? FontWeights.SemiBold : FontWeights.Normal;
        underline.Background = new SolidColorBrush(isSelected ? Colors.DodgerBlue : Colors.Transparent);
    }

    private void TabsClear()
    {
        ImporterTab.Visibility = Visibility.Collapsed;
        SizeTab.Visibility = Visibility.Collapsed;
        TechnicTab.Visibility = Visibility.Collapsed;
        FoodTab.Visibility = Visibility.Collapsed;
        HusbandryTab.Visibility = Visibility.Collapsed;
        BreedingTab.Visibility = Visibility.Collapsed;
        BuiltTab.Visibility = Visibility.Collapsed;
        SpecialTab.Visibility = Visibility.Collapsed;
        ReproductionTab.Visibility = Visibility.Collapsed;
        CultureTab.Visibility = Visibility.Collapsed;
        GlobalTab.Visibility = Visibility.Collapsed;
    }

    private void TabsAnimaliaShow()
    {
        ImporterTab.Visibility = Visibility.Visible;
        SizeTab.Visibility = Visibility.Visible;
        TechnicTab.Visibility = Visibility.Visible;
        FoodTab.Visibility = Visibility.Visible;
        HusbandryTab.Visibility = Visibility.Visible;
        BreedingTab.Visibility = Visibility.Visible;
        BuiltTab.Visibility = Visibility.Visible;
        SpecialTab.Visibility = Visibility.Visible;
        //ReproductionTab.Visibility = Visibility.Visible;
        //CultureTab.Visibility = Visibility.Visible;
        //GlobalTab.Visibility = Visibility.Visible;
    }
    private void TabsPlantaeShow()
    {
        ImporterTab.Visibility = Visibility.Visible;
        SizeTab.Visibility = Visibility.Visible;
        TechnicTab.Visibility = Visibility.Visible;
        //FoodTab.Visibility = Visibility.Visible;
        //HusbandryTab.Visibility = Visibility.Visible;
        //BreedingTab.Visibility = Visibility.Visible;
        BuiltTab.Visibility = Visibility.Visible;
        SpecialTab.Visibility = Visibility.Visible;
        ReproductionTab.Visibility = Visibility.Visible;
        CultureTab.Visibility = Visibility.Visible;
        GlobalTab.Visibility = Visibility.Visible;
    }

    private void ComboCreature_OnSelectionChangedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = sender as ComboBox;
        if (cmb == null)
        {
            return;
        }


        var text = ((ComboBox)sender).SelectedItem as string;
        switch (text)
        {
            case "":    //neu
                TabsClear();
                TabsAnimaliaShow();
                TabsPlantaeShow();
                break;
            case "Animalia":
                TabsClear();
                TabsAnimaliaShow();
                break;
            case "Plantae":
                TabsClear();
                TabsPlantaeShow();
                break;
            case "Fungi":
                TabsClear();
                TabsAnimaliaShow();
                break;
            case "Archaea":
                TabsClear();
                TabsAnimaliaShow();
                break;
            case "Bacteria":
                TabsClear();
                TabsAnimaliaShow();
                break;
            case "Chromista":
                TabsClear();
                TabsAnimaliaShow();
                break;
            case "Protozoa":
                TabsClear();
                TabsAnimaliaShow();
                break;
        }

        //var cmb = sender as ComboBox;
        //if (cmb != null)
        //{
        //    handle = !cmb.IsDropDownOpen;
        //}

        //Handle();
    }

}
