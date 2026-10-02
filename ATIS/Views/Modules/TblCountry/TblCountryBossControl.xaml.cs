using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    TblCountryBossControl Skriptdatum:   01.10.2026 12:32      

namespace ATIS.Views.Modules.TblCountry;

public sealed partial class TblCountryBossControl : UserControl
{
    public TblCountryViewModel? ViewModel => DataContext as TblCountryViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblCountryBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is TblCountryViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is TblCountryViewModel vm &&
            vm.SelectedItem != null)
        {
            CountryListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void CountryListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
