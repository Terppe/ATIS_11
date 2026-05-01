using ATIS.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentParentControl : UserControl
{
    public Tbl93CommentViewModel? ViewModel => DataContext as Tbl93CommentViewModel;

    public Tbl93CommentParentControl()
    {
        InitializeComponent();
       // AtisListViewZebraHelper.EnableZebra(CommentListView);
    }

    private void CommentListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // SelectedItem ist durch TwoWay Binding in der Regel schon gesetzt,
        // aber zur Sicherheit:

        if (DataContext is Tbl93CommentViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl93Comment)e.ClickedItem;
            vm.OpenEditCommand.Execute(null);
        }
    }

    private void CommentListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not ListView listView) return;

        var container = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);
        if (container is null) return;

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
}
