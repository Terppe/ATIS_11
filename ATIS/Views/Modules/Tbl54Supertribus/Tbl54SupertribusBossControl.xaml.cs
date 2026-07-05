using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl54SupertribusBossControl Skriptdatum:  22.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl54Supertribus;

public sealed partial class Tbl54SupertribusBossControl : UserControl
{
    public Tbl54SupertribusViewModel? ViewModel => DataContext as Tbl54SupertribusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl54SupertribusBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl54SupertribusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl54SupertribusViewModel vm &&
            vm.SelectedItem != null)
        {
            SupertribusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SupertribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
