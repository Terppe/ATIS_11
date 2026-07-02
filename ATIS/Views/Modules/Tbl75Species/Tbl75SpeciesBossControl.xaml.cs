using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ATIS.Views.Modules.Tbl66Genus;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Modules.Tbl75Species;

public sealed partial class Tbl75SpeciesBossControl : UserControl
{
    public Tbl75SpeciesViewModel? ViewModel => DataContext as Tbl75SpeciesViewModel;

    public Tbl75SpeciesBossControl()
    {
        InitializeComponent();
    }

    // ---------------------------------------------------------
    //  ItemClick → Select + Show Detail
    // ---------------------------------------------------------
    private void SpeciesListView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        SpeciesListView.SelectedItem = e.ClickedItem;

        if (ViewModel != null)
            ViewModel.IsDetailVisible = true;
    }

    // ---------------------------------------------------------
    //  RightTapped → Select row under cursor
    // ---------------------------------------------------------
    private void SpeciesListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not ListView listView)
            return;

        var container = FindAncestor<ListViewItem>(e.OriginalSource as DependencyObject);
        if (container is null)
            return;

        listView.SelectedItem = listView.ItemFromContainer(container);
    }

    // ---------------------------------------------------------
    //  Helper: Find ancestor of type T
    // ---------------------------------------------------------
    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T match)
                return match;

            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

}
