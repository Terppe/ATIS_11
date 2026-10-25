
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
    }
    #region small Helpers

    [System.Diagnostics.Conditional("DEBUG")]
    private void LogDebug(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    private void LogGuard(string source, string reason)
    {
        System.Diagnostics.Debug.WriteLine($"{source}: skip ({reason})");
    }

    #endregion

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

    public void ApplyItem(Models.TblUserProfile? newItem)
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
    // Combos
    // ---------------------------------------------------------                   

    private void RoleCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
                break;
            case "Administrator":
                break;
            case "Developer":
                break;
            case "Biologist":
                break;
            case "Zoologist":
                break;
            case "User":
                break;

        }
    }

    private void TitleCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
                break;
            case "Dr.":
                break;
            case "Prof.":
                break;
            case "Dipl.- Ing.":
                break;
            case "Ing. grad":
                break;
            case "Dr. Ing":
                break;
        }
    }

    private void GenderCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
                break;
            case "Female":
                break;
            case "Male":
                break;
        }
    }

    private void CountryCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (ViewModel == null || Item == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "view model or item is null");
            return;
        }

        var country = (sender as ComboBox)?.SelectedItem as Models.TblCountry;
        if (country == null)
        {
            LogGuard(nameof(CountryCombo_OnSelectionChanged), "selected country is null");
            return;
        }

        // Country und Erdteil immer setzen, unabhängig von Karte und Punkten
        Item.Country = country.Name;

    }

    private void ComboImageMimeType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
                break;
            case "jpg":
                break;
            case "png":
                break;
            case "bmp":
                break;
            case "tif":
                break;
            case "tiff":
                break;
            case "gif":
                break;
            case "ico":
                break;
            case "jpeg":
                break;
            case "webp":
                break;
            case "wmf":
                break;
            case "wmv":
                break;
            case "mpg":
                break;
            case "mp4":
                break;
            case "avi":
                break;
            case "mov":
                break;
            case "swf":
                break;
            case "flv":
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
