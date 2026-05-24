using Microsoft.UI.Xaml.Input;
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

}
