using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl60SubtribusParentControl Skriptdatum:  25.06.2026  10:32    

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusParentControl : UserControl
{
    public Tbl60SubtribusViewModel? ViewModel => DataContext as Tbl60SubtribusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl60SubtribusParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl60SubtribusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl60SubtribusViewModel vm &&
            vm.SelectedItem != null)
        {
            SubtribusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SubtribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void SubtribusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl60SubtribusViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl60Subtribus)e.ClickedItem;
        }
    }



}
