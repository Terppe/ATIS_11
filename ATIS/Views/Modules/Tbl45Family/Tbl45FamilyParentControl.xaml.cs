using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl45FamilyParentControl Skriptdatum:  18.10.2025  10:32    

namespace ATIS.Views.Modules.Tbl45Family;

public sealed partial class Tbl45FamilyParentControl : UserControl
{
    public Tbl45FamilyViewModel? ViewModel => DataContext as Tbl45FamilyViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl45FamilyParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl45FamilyViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl45FamilyViewModel vm &&
            vm.SelectedItem != null)
        {
            FamilyListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void FamilyListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void FamilyListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl45FamilyViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl45Family)e.ClickedItem;
        }
    }



}
