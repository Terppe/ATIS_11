using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Media;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentEditControl : UserControl
{
    public Tbl93CommentEditControl()
    {
        InitializeComponent();
        Debug.WriteLine(">>> EditControl Konstruktor, DataContext = " + this.DataContext);
    }

    private void UserControl_DataContextChanged(object sender, DataContextChangedEventArgs e)
    {
        Debug.WriteLine(">>> EditControl DataContextChanged: " + e.NewValue);
    }
    // ---------------------------------------------------------
    //  DependencyProperty: Item  (wie Subtribus)
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

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = e.NewValue as Models.Tbl93Comment;

        Debug.WriteLine(
            $"CommentEditControl.Item changed: Id={item?.CommentId}, " +
            $"Info='{item?.Info}', Memo='{item?.Memo}', Year='{item?.ValidYear}', Valid={item?.Valid}");
    }
    // ---------------------------------------------------------
    //  VisualState-Update (wie Subtribus)
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
}
