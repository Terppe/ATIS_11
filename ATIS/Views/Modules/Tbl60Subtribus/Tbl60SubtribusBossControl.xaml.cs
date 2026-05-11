
using ATIS.DesignSystem.Layout;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusBossControl : UserControl
{
    public Tbl60SubtribusViewModel? ViewModel => DataContext as Tbl60SubtribusViewModel;

    public Tbl60SubtribusBossControl()
    {
        InitializeComponent();
    }

    private void SubtribusListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        SubtribusListView.SelectedItem = e.ClickedItem;

        if (ViewModel != null)
            ViewModel.IsDetailVisible = true;
    }

    private void SubtribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not ListView listView) return;

        var container = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);
        if (container is null) return;

        listView.SelectedItem = listView.ItemFromContainer(container);
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
}
