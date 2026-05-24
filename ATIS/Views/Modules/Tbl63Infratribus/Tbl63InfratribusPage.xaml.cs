using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPage : Page
{
    public Tbl63InfratribusViewModel ViewModel { get; }

    public Tbl63InfratribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl63InfratribusViewModel>();
        DataContext = ViewModel;

        Loaded += InfratribusPage_Loaded;
    }
    private async void InfratribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Unsubscribe sofort, damit der Event nicht nochmal triggert
        Loaded -= InfratribusPage_Loaded;

        await ViewModel.LoadAsync();

        ViewModel.SuppressFilter = true;
        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
        ViewModel.SuppressFilter = false;
    }

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Debug.WriteLine("QUERY SUBMITTED: " + sender.Text);

        await ViewModel.ApplyFilterAsync(sender.Text);
    }





}
