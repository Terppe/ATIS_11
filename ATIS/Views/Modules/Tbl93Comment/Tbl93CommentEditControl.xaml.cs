using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Media;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentEditControl : UserControl
{
    private int SelectedTabIndex = 0;
    public Tbl93CommentEditControl()
    {
        InitializeComponent();
        Loaded += OnControlLoaded;
    }

    private void OnControlLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTabButtonStates();
        UpdateDetailContentVisibility();
    }

    private static async void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Tbl93CommentEditControl)d;
        if (!control.IsLoaded) return;
        control.UpdateAllValidationStates();
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
    //  DependencyProperty: Item
    // ---------------------------------------------------------

    public Models.Tbl93Comment? Item
    {
        get => (Models.Tbl93Comment?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(
            nameof(Item),
            typeof(Models.Tbl93Comment),
            typeof(Tbl93CommentEditControl),
            new PropertyMetadata(null, OnItemChanged));


    // ---------------------------------------------------------
    //  DependencyProperty: ViewModel
    // ---------------------------------------------------------

    public Tbl93CommentViewModel? ViewModel
    {
        get => (Tbl93CommentViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(Tbl93CommentViewModel),
            typeof(Tbl93CommentEditControl),
            new PropertyMetadata(null));


    // ---------------------------------------------------------
    //  Validation State Updates
    // ---------------------------------------------------------

    private void UpdateAllValidationStates()
    {
        UpdateValidationState(ValidYearBox, Item?.HasValidYearError ?? false);
        UpdateValidationState(InfoBox, Item?.HasInfoError ?? false);
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


}
