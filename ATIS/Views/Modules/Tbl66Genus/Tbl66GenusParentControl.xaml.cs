
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl66GenusParentControl Skriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusParentControl : UserControl
{
    public Tbl66GenusViewModel? ViewModel => DataContext as Tbl66GenusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl66GenusParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl66GenusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl66GenusViewModel vm &&
            vm.SelectedItem != null)
        {
            GenusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void GenusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void GenusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl66GenusViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl66Genus)e.ClickedItem;
        }
    }



}
