using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.System;

namespace ATIS.Views.Modules.Tbl66Genus;

public sealed partial class Tbl66GenusPage : Page
{
    public Tbl66GenusViewModel ViewModel { get; }

    public Tbl66GenusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl66GenusViewModel>();
        DataContext = ViewModel;

        Loaded += GenusPage_Loaded;
    }

    // ---------------------------------------------------------
    //  PAGE LOADED → LoadAsync + Focus SearchBox
    // ---------------------------------------------------------
    private async void GenusPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();

        ViewModel.SuppressFilter = true;
        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
        ViewModel.SuppressFilter = false;
    }

    // ---------------------------------------------------------
    //  SEARCH SUBMITTED
    // ---------------------------------------------------------
    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Debug.WriteLine("QUERY SUBMITTED: " + sender.Text);
        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    // ---------------------------------------------------------
    //  SEARCH KEY HANDLING
    // ---------------------------------------------------------
    private async void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            await ViewModel.ApplyFilterAsync(SearchBox.Text);
        }
        else if (e.Key == VirtualKey.Escape)
        {
            SearchBox.Text = "";
            await ViewModel.ApplyFilterAsync("");
        }
    }

    // ---------------------------------------------------------
    //  OPTIONAL HOVER EVENTS
    // ---------------------------------------------------------
    private void Row_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // optional: Hover-Effekt
    }

    private void Row_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        // optional: Hover-Effekt
    }
}
