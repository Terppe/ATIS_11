using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl03RegnumParentControl Skriptdatum:  30.12.2024  12:32      

namespace ATIS.Views.Modules.Tbl03Regnum;

public sealed partial class Tbl03RegnumParentControl : UserControl
{
    public Tbl03RegnumViewModel? ViewModel => DataContext as Tbl03RegnumViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl03RegnumParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl03RegnumViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl03RegnumViewModel vm &&
            vm.SelectedItem != null)
        {
            RegnumListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void RegnumListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void RegnumListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl03RegnumViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl03Regnum)e.ClickedItem;
        }
    }



}
