
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;

//    TblUserProfileEditControl Skriptdatum:   10.10.2026  10:32    

namespace ATIS.Views.Modules.TblUserProfile;

public sealed partial class TblUserProfileEditControl : UserControl
{
    public TblUserProfileViewModel? ViewModel => DataContext as TblUserProfileViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblUserProfileEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += TblUserProfileEditControl_Loaded;
    }
    private void TblUserProfileEditControl_Loaded(object sender, RoutedEventArgs e)
    {
       // UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.TblUserProfile? Item
    {
        get => (Models.TblUserProfile?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.TblUserProfile),
            typeof(TblUserProfileEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TblUserProfileEditControl)d;
        var item = e.NewValue as Models.TblUserProfile;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.TblUserProfile? newItem)
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
    // UserProfile EMAIL VALIDATION
    // ---------------------------------------------------------                   

    private void EmailBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(EmailBox, Item?.HasEmailError ?? false);
    }

    private void EmailBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(EmailBox, Item?.HasEmailError ?? false);
    }

    // ---------------------------------------------------------                   
    // UserProfile FIRSTNAME VALIDATION
    // ---------------------------------------------------------                   
    private void FirstNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(FirstNameBox, Item?.HasFirstNameError ?? false);
    }

    private void FirstNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(FirstNameBox, Item?.HasFirstNameError ?? false);
    }

    // ---------------------------------------------------------                   
    // UserProfile LASTNAME VALIDATION
    // ---------------------------------------------------------                   
    private void LastNameBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(LastNameBox, Item?.HasLastNameError ?? false);
    }

    private void LastNameBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(LastNameBox, Item?.HasLastNameError ?? false);
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
          //  UpdateTabButtonStates();
         //   UpdateDetailContentVisibility();
        }
    }



}
