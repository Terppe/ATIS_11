using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl33OrdoParentControl Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl33Ordo;

public sealed partial class Tbl33OrdoParentControl : UserControl
{
    public Tbl33OrdoViewModel? ViewModel => DataContext as Tbl33OrdoViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl33OrdoParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl33OrdoViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl33OrdoViewModel vm &&
            vm.SelectedItem != null)
        {
            OrdoListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void OrdoListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void OrdoListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl33OrdoViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl33Ordo)e.ClickedItem;
        }
    }



}
