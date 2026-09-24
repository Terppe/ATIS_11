using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl84SynonymParentControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl84Synonym;

public sealed partial class Tbl84SynonymParentControl : UserControl
{
    public Tbl84SynonymViewModel? ViewModel => DataContext as Tbl84SynonymViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl84SynonymParentControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl84SynonymViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl84SynonymViewModel vm &&
            vm.SelectedItem != null)
        {
            SynonymListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SynonymListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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

    private void SynonymListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl84SynonymViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl84Synonym)e.ClickedItem;
        }
    }



}
