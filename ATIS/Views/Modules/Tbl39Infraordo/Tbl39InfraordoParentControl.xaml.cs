using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl39InfraordoParentControl Skriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Modules.Tbl39Infraordo;

public sealed partial class Tbl39InfraordoParentControl : UserControl
{
    public Tbl39InfraordoViewModel? ViewModel => DataContext as Tbl39InfraordoViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl39InfraordoParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl39InfraordoViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl39InfraordoViewModel vm &&
            vm.SelectedItem != null)
        {
            InfraordoListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void InfraordoListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void InfraordoListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl39InfraordoViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl39Infraordo)e.ClickedItem;
        }
    }



}
