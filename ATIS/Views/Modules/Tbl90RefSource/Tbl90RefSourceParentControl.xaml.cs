using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl90RefSourceParentControl Skriptdatum:   11.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl90RefSource;

public sealed partial class Tbl90RefSourceParentControl : UserControl
{
    public Tbl90RefSourceViewModel? ViewModel => DataContext as Tbl90RefSourceViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefSourceParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl90RefSourceViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl90RefSourceViewModel vm &&
            vm.SelectedItem != null)
        {
            RefSourceListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void RefSourceListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void RefSourceListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl90RefSourceViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl90RefSource)e.ClickedItem;
        }
    }



}
