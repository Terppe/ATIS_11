using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl24SubclassParentControl Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl24Subclass;

public sealed partial class Tbl24SubclassParentControl : UserControl
{
    public Tbl24SubclassViewModel? ViewModel => DataContext as Tbl24SubclassViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl24SubclassParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl24SubclassViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl24SubclassViewModel vm &&
            vm.SelectedItem != null)
        {
            SubclassListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SubclassListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void SubclassListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl24SubclassViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl24Subclass)e.ClickedItem;
        }
    }



}
