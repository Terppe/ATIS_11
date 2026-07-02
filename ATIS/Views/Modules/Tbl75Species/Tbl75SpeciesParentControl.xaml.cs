using System;
using System.Collections.Generic;
using System.Diagnostics;
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

public sealed partial class Tbl75SpeciesParentControl : UserControl
{
    public Tbl75SpeciesViewModel? ViewModel => DataContext as Tbl75SpeciesViewModel;

    public Tbl75SpeciesParentControl()
    {
        InitializeComponent();
    }

    private void SpeciesListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.SelectedItem = (Models.Tbl75Species)e.ClickedItem;
        }
    }

    // ---------------------------------------------------------
    // RIGHT CLICK → Kontextmenü öffnen
    // ---------------------------------------------------------
    private void SpeciesListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is ListView listView &&
            e.OriginalSource is FrameworkElement fe &&
            fe.DataContext is Models.Tbl75Species item)
        {
            listView.SelectedItem = item;

            if (fe.ContextFlyout != null)
            {
                fe.ContextFlyout.ShowAt(fe);
            }
        }
    }
}
