using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl51InfrafamilyParentControl Skriptdatum:  21.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public sealed partial class Tbl51InfrafamilyParentControl : UserControl
{
    public Tbl51InfrafamilyViewModel? ViewModel => DataContext as Tbl51InfrafamilyViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl51InfrafamilyParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl51InfrafamilyViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl51InfrafamilyViewModel vm &&
            vm.SelectedItem != null)
        {
            InfrafamilyListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void InfrafamilyListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void InfrafamilyListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl51InfrafamilyViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl51Infrafamily)e.ClickedItem;
        }
    }



}
