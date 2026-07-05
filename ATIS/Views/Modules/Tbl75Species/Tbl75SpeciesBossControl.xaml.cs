
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl75SpeciesBossControl Skriptdatum:  15.12.2025  10:32    

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesBossControl : UserControl
{
    public Tbl75SpeciesViewModel? ViewModel => DataContext as Tbl75SpeciesViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl75SpeciesBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl75SpeciesViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl75SpeciesViewModel vm &&
            vm.SelectedItem != null)
        {
            SpeciesListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void SpeciesListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
