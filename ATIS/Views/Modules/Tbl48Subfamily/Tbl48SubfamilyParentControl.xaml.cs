using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl48SubfamilyParentControl Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl48Subfamily;

public sealed partial class Tbl48SubfamilyParentControl : UserControl
{
    public Tbl48SubfamilyViewModel? ViewModel => DataContext as Tbl48SubfamilyViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl48SubfamilyParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl48SubfamilyViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl48SubfamilyViewModel vm &&
            vm.SelectedItem != null)
        {
            SubfamilyListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SubfamilyListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void SubfamilyListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl48SubfamilyViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl48Subfamily)e.ClickedItem;
        }
    }



}
