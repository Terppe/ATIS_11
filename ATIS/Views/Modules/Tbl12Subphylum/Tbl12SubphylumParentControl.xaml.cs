using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl12SubphylumParentControl Skriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Modules.Tbl12Subphylum;

public sealed partial class Tbl12SubphylumParentControl : UserControl
{
    public Tbl12SubphylumViewModel? ViewModel => DataContext as Tbl12SubphylumViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl12SubphylumParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl12SubphylumViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl12SubphylumViewModel vm &&
            vm.SelectedItem != null)
        {
            SubphylumListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SubphylumListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void SubphylumListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl12SubphylumViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl12Subphylum)e.ClickedItem;
        }
    }



}
