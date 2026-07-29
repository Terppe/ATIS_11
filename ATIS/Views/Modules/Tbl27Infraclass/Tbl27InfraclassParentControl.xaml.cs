using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl27InfraclassParentControl Skriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Modules.Tbl27Infraclass;

public sealed partial class Tbl27InfraclassParentControl : UserControl
{
    public Tbl27InfraclassViewModel? ViewModel => DataContext as Tbl27InfraclassViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl27InfraclassParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl27InfraclassViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl27InfraclassViewModel vm &&
            vm.SelectedItem != null)
        {
            InfraclassListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void InfraclassListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void InfraclassListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl27InfraclassViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl27Infraclass)e.ClickedItem;
        }
    }



}
