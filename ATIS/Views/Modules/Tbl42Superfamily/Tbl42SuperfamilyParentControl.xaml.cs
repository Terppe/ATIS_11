using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl42SuperfamilyParentControl Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl42Superfamily;

public sealed partial class Tbl42SuperfamilyParentControl : UserControl
{
    public Tbl42SuperfamilyViewModel? ViewModel => DataContext as Tbl42SuperfamilyViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl42SuperfamilyParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl42SuperfamilyViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl42SuperfamilyViewModel vm &&
            vm.SelectedItem != null)
        {
            SuperfamilyListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SuperfamilyListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not ListView listView)
            return;

        var container = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);
        if (container is null)
            return;

        listView.SelectedItem = listView.ItemFromContainer(container);
        e.Handled = true;
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T match) return match;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    // ---------------------------------------------------------
    // ITEM CLICK
    // ---------------------------------------------------------

    private void SuperfamilyListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl42SuperfamilyViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl42Superfamily)e.ClickedItem;
        }
    }



}
