using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusBossControl : UserControl
{
    public Tbl63InfratribusViewModel? ViewModel => DataContext as Tbl63InfratribusViewModel;

    public Tbl63InfratribusBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl63InfratribusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // ITEM CLICK
    // ---------------------------------------------------------

    //private void InfratribusListView_ItemClick(object sender, ItemClickEventArgs e)
    //{
    //    InfratribusListView.SelectedItem = e.ClickedItem;

    //    if (ViewModel != null)
    //        ViewModel.IsDetailVisible = true;
    //}
    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl63InfratribusViewModel vm &&
            vm.SelectedItem != null)
        {
            InfratribusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------
    // RIGHT TAP → SELECT ITEM
    // ---------------------------------------------------------

    private void InfratribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
    // HELPER: FIND ANCESTOR
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
