using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl57TribusBossControl Skriptdatum:  22.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl57Tribus;

public sealed partial class Tbl57TribusBossControl : UserControl
{
    public Tbl57TribusViewModel? ViewModel => DataContext as Tbl57TribusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl57TribusBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl57TribusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl57TribusViewModel vm &&
            vm.SelectedItem != null)
        {
            TribusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void TribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
