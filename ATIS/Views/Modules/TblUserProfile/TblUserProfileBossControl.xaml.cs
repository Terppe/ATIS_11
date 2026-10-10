using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    TblUserProfileBossControl Skriptdatum:   10.10.2026  10:32    

namespace ATIS.Views.Modules.TblUserProfile;

public sealed partial class TblUserProfileBossControl : UserControl
{
    public TblUserProfileViewModel? ViewModel => DataContext as TblUserProfileViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------

    public TblUserProfileBossControl()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            if (DataContext is TblUserProfileViewModel vm)
                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
        };
    }

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is TblUserProfileViewModel vm &&
            vm.SelectedItem != null)
        {
            UserProfileListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void UserProfileListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
