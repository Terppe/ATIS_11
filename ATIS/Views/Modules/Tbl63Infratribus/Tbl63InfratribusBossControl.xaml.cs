
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl63InfratribusBossControl Skriptdatum:  30.12.2024  10:32    

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusBossControl : UserControl
{
    public Tbl63InfratribusViewModel? ViewModel => DataContext as Tbl63InfratribusViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public Tbl63InfratribusBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is Tbl63InfratribusViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl63InfratribusViewModel vm &&
            vm.SelectedItem != null)
        {
            InfratribusListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void InfratribusListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
 

   
  
