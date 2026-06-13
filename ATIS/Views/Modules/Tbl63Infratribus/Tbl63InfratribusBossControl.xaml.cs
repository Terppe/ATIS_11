using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusBossControl : UserControl
{
    public Tbl63InfratribusViewModel? ViewModel => DataContext as Tbl63InfratribusViewModel;

    public Tbl63InfratribusBossControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  ItemClick → Select + Show Detail
    // ---------------------------------------------------------
    //private void InfratribusListView_ItemClick(object sender, ItemClickEventArgs e)
    //{
    //    InfratribusListView.SelectedItem = e.ClickedItem;

    //    if (ViewModel != null)
    //        ViewModel.IsEditVisible = true;
    //}

    // ---------------------------------------------------------
    //  RightTapped → Select row under cursor
    // ---------------------------------------------------------
    private void InfratribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not ListView listView) return;

        var container = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);
        if (container is null) return;

        listView.SelectedItem = listView.ItemFromContainer(container);
    }

    // ---------------------------------------------------------
    //  Helper: Find ancestor of type T
    // ---------------------------------------------------------
    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T match) return match;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

}
