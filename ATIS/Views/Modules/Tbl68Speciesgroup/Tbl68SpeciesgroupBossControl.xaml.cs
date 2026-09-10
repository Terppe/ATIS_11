using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

//    Tbl68SpeciesgroupBossControl Skriptdatum:  10.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl68Speciesgroup;

public sealed partial class Tbl68SpeciesgroupBossControl : UserControl
{
    public Tbl68SpeciesgroupViewModel? ViewModel => DataContext as Tbl68SpeciesgroupViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl68SpeciesgroupBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl68SpeciesgroupViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl68SpeciesgroupViewModel vm &&
            vm.SelectedItem != null)
        {
            SpeciesgroupListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SpeciesgroupListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
