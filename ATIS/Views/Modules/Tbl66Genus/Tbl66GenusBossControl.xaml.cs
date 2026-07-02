using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusBossControl : UserControl
{
    public Tbl66GenusViewModel? ViewModel => DataContext as Tbl66GenusViewModel;

    public Tbl66GenusBossControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  ItemClick → Select + Show Detail
    // ---------------------------------------------------------
    private void GenusListView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        GenusListView.SelectedItem = e.ClickedItem;

       // if (ViewModel != null)
          //  ViewModel.IsDetailVisible = true;
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
