using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusParentControl : UserControl
{
    public Tbl66GenusViewModel? ViewModel => DataContext as Tbl66GenusViewModel;

    public Tbl66GenusParentControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  ItemClick → Select + Open Edit
    // ---------------------------------------------------------
    private void GenusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl66GenusViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl66Genus)e.ClickedItem;
            vm.OpenEditCommand.Execute(null);
        }
    }

    // ---------------------------------------------------------
    //  RightTapped → Select row under cursor
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

    // ---------------------------------------------------------
    //  Helper: Find ancestor of type T
    // ---------------------------------------------------------
    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T match)
                return match;

            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }
}
