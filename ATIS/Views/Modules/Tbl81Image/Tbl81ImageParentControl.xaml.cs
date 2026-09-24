using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;


//    Tbl81ImageParentControl Skriptdatum:  13.09.2026  10:32    

namespace ATIS.Views.Modules.Tbl81Image;

public sealed partial class Tbl81ImageParentControl : UserControl
{
    public Tbl81ImageViewModel? ViewModel => DataContext as Tbl81ImageViewModel;
    // ---------------------------------------------------------
    // CONSTRUCTOR
    // ---------------------------------------------------------
    public Tbl81ImageParentControl()
    {
        InitializeComponent();

        Loaded += (_, __) =>
        {
            if (DataContext is Tbl81ImageViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ImageParent] Loaded: " +
                    $"Items={vm.Items?.Count ?? -1}, " +
                    $"DataSetCount={vm.DataSetCount}, " +
                    $"ParentMode={vm.IsInParentMode}");

                vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ImageParent] Falscher DataContext: " +
                    $"{DataContext?.GetType().FullName ?? "null"}");
            }
        };
    }
    //public Tbl81ImageParentControl()
    //{
    //    InitializeComponent();
    //    Loaded += (_, __) =>
    //    {
    //        if (DataContext is Tbl81ImageViewModel vm)
    //            vm.ScrollRequested += (_, __) => ScrollToSelectedItem();
    //    };
    //}

    // ---------------------------------------------------------
    // SCROLL TO SELECTED
    // ---------------------------------------------------------

    private void ScrollToSelectedItem()
    {
        if (DataContext is Tbl81ImageViewModel vm &&
            vm.SelectedItem != null)
        {
            ImageListView.ScrollIntoView(vm.SelectedItem);
        }
    }

    // ---------------------------------------------------------                   
    // RIGHT_TAPPED
    // ---------------------------------------------------------                   

    private void ImageListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
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
            if (current is T match) return match;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    // ---------------------------------------------------------
    // ITEM CLICK
    // ---------------------------------------------------------

    private void ImageListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (DataContext is Tbl81ImageViewModel vm)
        {
            vm.SelectedItem = (Models.Tbl81Image)e.ClickedItem;
        }
    }



}
