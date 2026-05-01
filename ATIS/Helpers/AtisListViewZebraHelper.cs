using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Helpers;

public static class AtisListViewZebraHelper
{
    public static void EnableZebra(ListView listView)
    {
        listView.ContainerContentChanging -= OnContainerContentChanging;
        listView.ContainerContentChanging += OnContainerContentChanging;

        listView.ActualThemeChanged -= OnActualThemeChanged;
        listView.ActualThemeChanged += OnActualThemeChanged;
    }

    private static void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (sender is not ListView lv) return;
        Refresh(lv);
    }

    public static void Refresh(ListView lv)
    {
        // nur realisierte Container neu färben
        for (int i = 0; i < lv.Items.Count; i++)
        {
            if (lv.ContainerFromIndex(i) is ListViewItem item)
            {
                bool even = (i % 2) == 0;
                var evenBrush = (Brush)Application.Current.Resources["AtisListViewRowBackgroundBrush"];
                var oddBrush = (Brush)Application.Current.Resources["AtisListViewRowAlternateBackgroundBrush"];
                item.Background = even ? evenBrush : oddBrush;
            }
        }
    }

    private static void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.ItemContainer is not ListViewItem item) return;
        if (args.InRecycleQueue) { item.ClearValue(Control.BackgroundProperty); return; }

        bool even = (args.ItemIndex % 2) == 0;
        var evenBrush = (Brush)Application.Current.Resources["AtisListViewRowBackgroundBrush"];
        var oddBrush = (Brush)Application.Current.Resources["AtisListViewRowAlternateBackgroundBrush"];
        item.Background = even ? evenBrush : oddBrush;
    }
}
