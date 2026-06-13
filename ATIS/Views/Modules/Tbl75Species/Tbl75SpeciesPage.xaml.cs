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
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl75SpeciesPage : Page
{
    public Tbl75SpeciesViewModel ViewModel { get; }

    public Tbl75SpeciesPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<Tbl75SpeciesViewModel>();
        DataContext = ViewModel;

        Loaded += SpeciesPage_Loaded;
    }

    // ---------------------------------------------------------
    //  PAGE LOADED → LoadAsync + Focus SearchBox
    // ---------------------------------------------------------
    private async void SpeciesPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();

 //       ViewModel.SuppressFilter = true;
 //       await Task.Delay(50);
 ////       SearchBox.Focus(FocusState.Programmatic);
 //       ViewModel.SuppressFilter = false;
    }

    // ---------------------------------------------------------
    //  SEARCH SUBMITTED
    // ---------------------------------------------------------
    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Debug.WriteLine("QUERY SUBMITTED: " + sender.Text);
        await ViewModel.ApplyFilterAsync(sender.Text);
    }
}
