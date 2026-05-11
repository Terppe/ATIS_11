using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusParentControl : UserControl
{
    public Tbl63InfratribusViewModel? ViewModel => DataContext as Tbl63InfratribusViewModel;

    public Tbl63InfratribusParentControl()
    {
        InitializeComponent();
    }

    private void InfratribusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl63InfratribusViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl63Infratribus)e.ClickedItem;
            vm.OpenEditCommand.Execute(null);
        }
    }

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
