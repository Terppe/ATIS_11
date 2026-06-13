using System.Diagnostics;
using ATIS.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Media;
using ATIS.Views.Modules.Tbl60Subtribus;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl93Comment;

public sealed partial class Tbl93CommentParentControl : UserControl
{
    public Tbl93CommentViewModel? ViewModel => DataContext as Tbl93CommentViewModel;

    public Tbl93CommentParentControl()
    {
        InitializeComponent();
        CommentListView.SelectionChanged += (_, __) => Debug.WriteLine("SELECTION!");
    }

    private void CommentListView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (ViewModel == null)
            return;

        ViewModel.SelectedItem = (Models.Tbl93Comment)e.ClickedItem;
      //  ViewModel.IsDetailVisible = true;
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
