

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using System.Diagnostics;
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
            Debug.WriteLine("CLICK: " + e.ClickedItem);
            vm.SelectedItem = (Models.Tbl60Subtribus)e.ClickedItem;
        //    vm.IsDetailVisible = true;
            Debug.WriteLine("DETAIL VISIBLE = TRUE");
         //   vm.SelectedTabIndex = 0;
       //     Debug.WriteLine("TAB = " + vm.SelectedTabIndex);
            Debug.WriteLine("CLICK: SelectedItem=" + vm.SelectedItem?.SubtribusName);
         //   Debug.WriteLine("CLICK: IsDetailVisible=" + vm.IsDetailVisible);
      //      Debug.WriteLine("CLICK: SelectedTabIndex=" + vm.SelectedTabIndex);
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
