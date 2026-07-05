using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl90RefExpertBossControl Skriptdatum:  06.01.2025  10:32    

namespace ATIS.Views.Modules.Tbl90RefExpert;

public sealed partial class Tbl90RefExpertBossControl : UserControl
{
    public Tbl90RefExpertViewModel? ViewModel => DataContext as Tbl90RefExpertViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl90RefExpertBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl90RefExpertViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl90RefExpertViewModel vm &&
            vm.SelectedItem != null)
        {
            RefExpertListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void RefExpertListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
