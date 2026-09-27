
//    Tbl81ImageEditControl Skriptdatum:  13.09.2026  10:32    

using System.Diagnostics;
using ATIS.Helpers;
using ATIS.Models;
using log4net;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ATIS.Views.Modules.Tbl81Image;

public sealed partial class Tbl81ImageEditControl : UserControl
{
    public Tbl81ImageViewModel? ViewModel => DataContext as Tbl81ImageViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl81ImageEditControl()
    {
        InitializeComponent();
        SelectedTabIndex = 0;
        Loaded += Tbl66GenusEditControl_Loaded;
    }
    private void Tbl66GenusEditControl_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    // ---------------------------------------------------------
    //      DEPENDENCY PROPERTY: ITEM
    // ---------------------------------------------------------

    public Models.Tbl81Image? Item
    {
        get => (Models.Tbl81Image?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl81Image),
            typeof(Tbl81ImageEditControl),
            new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl81ImageEditControl)d;
        var item = e.NewValue as Models.Tbl81Image;

        control.ApplyItem(item);
    }

    // ---------------------------------------------------------                    
    // APPLY ITEM
    // ---------------------------------------------------------                   

    private void ApplyItem(Models.Tbl81Image? newItem)
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
    // ValidYear VALIDATION
    // ---------------------------------------------------------                   

    private void ValidYearBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
    }

    private void ValidYearBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
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
        SetTabButtonStyle(ImageTab, ImageTabUnderline, SelectedTabIndex == 0);
        SetTabButtonStyle(VideoTab, VideoTabUnderline, SelectedTabIndex == 1);
    }
    private void UpdateDetailContentVisibility()
    {
        ImageGrid.Visibility = SelectedTabIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
        VideoGrid.Visibility = SelectedTabIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.SelectFileForCurrentItemAsync();
    }

    private void DpShotDate_OnGotFocus(object sender, RoutedEventArgs e)
    {
        ShotDateBorder.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 103, 192));
        ShotDateBorder.BorderThickness = new Thickness(2);
    }

    private void DpShotDate_OnLostFocus(object sender, RoutedEventArgs e)
    {
        ShotDateBorder.BorderBrush = (Brush)Application.Current.Resources["AtisTextBoxBorderBrush"];
        ShotDateBorder.BorderThickness = new Thickness(1);
    }
}
