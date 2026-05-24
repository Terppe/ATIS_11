

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusParentControl : UserControl
{
    public Tbl60SubtribusViewModel? ViewModel => DataContext as Tbl60SubtribusViewModel;

    public Tbl60SubtribusParentControl()
    {
        InitializeComponent();
    }

    private void SubtribusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl60SubtribusViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl60Subtribus)e.ClickedItem;
            vm.OpenEditCommand.Execute(null);
        }
    }

    private void SubtribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
