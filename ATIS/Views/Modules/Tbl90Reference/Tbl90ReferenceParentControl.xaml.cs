using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Modules.Tbl90Reference;

public sealed partial class Tbl90ReferenceParentControl : UserControl
{
    public Tbl90ReferenceViewModel? ViewModel => DataContext as Tbl90ReferenceViewModel;

    public Tbl90ReferenceParentControl()
    {
        InitializeComponent();
        // AtisListViewZebraHelper.EnableZebra(ReferenceListView);
    }

    private void ReferenceListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (ViewModel == null)
            return;

        // SelectedItem ist durch TwoWay Binding meist schon gesetzt
        ViewModel.SelectedItem = (Models.Tbl90Reference)e.ClickedItem;
        ViewModel.OpenEditCommand.Execute(null);
    }

    private void ReferenceListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
