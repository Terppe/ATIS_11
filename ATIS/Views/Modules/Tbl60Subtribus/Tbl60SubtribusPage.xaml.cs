using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public sealed partial class Tbl60SubtribusPage : Page
{
    public Tbl60SubtribusViewModel ViewModel { get; }

    public Tbl60SubtribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl60SubtribusViewModel>();
        DataContext = ViewModel;

        Loaded += SubtribusPage_Loaded;
       // ViewModel.NewRequested += OnNewRequested;
    }

    private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();
        Loaded -= SubtribusPage_Loaded; // Unsubscribe!

          TabViewBase.SelectedIndex = 0; // Tab „Edit Subtribus“

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

    //private void OnNewRequested(object? sender, EventArgs e)
    //{
    //    TabViewBase.SelectedIndex = 0; // Edit-Tab aktivieren
    //}


}
