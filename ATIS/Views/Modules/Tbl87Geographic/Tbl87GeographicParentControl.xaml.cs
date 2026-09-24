using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl87GeographicParentControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl87Geographic;

public sealed partial class Tbl87GeographicParentControl : UserControl
{
    public Tbl87GeographicViewModel? ViewModel => DataContext as Tbl87GeographicViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl87GeographicParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl87GeographicViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl87GeographicViewModel vm &&
            vm.SelectedItem != null)
        {
            GeographicListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void GeographicListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void GeographicListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl87GeographicViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl87Geographic)e.ClickedItem;
        }
    }



}
