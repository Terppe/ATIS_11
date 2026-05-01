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
    }

    private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();

        ViewModel.SuppressFilter = true;
        await Task.Delay(50);
        SearchBox.Focus(FocusState.Programmatic);
        ViewModel.SuppressFilter = false;
    }

    //private async void SubtribusPage_Loaded(object sender, RoutedEventArgs e)
    //{
    //    await Task.Delay(50);
    //    SearchBox.Focus(FocusState.Programmatic);
    //}
    //private async void SearchBox_OnLostFocus(object sender, RoutedEventArgs e)
    //{
    //    var box = (AutoSuggestBox)sender;
    //    await ViewModel.ApplyFilterAsync(box.Text);
    //}

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Debug.WriteLine("QUERY SUBMITTED: " + sender.Text);

        await ViewModel.ApplyFilterAsync(sender.Text);
    }

    //private async void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
    //{
    //    if (e.Key == VirtualKey.Enter)
    //    {
    //        await ViewModel.ApplyFilterAsync(SearchBox.Text);
    //    }
    //    else if (e.Key == VirtualKey.Escape)
    //    {
    //        SearchBox.Text = "";
    //        await ViewModel.ApplyFilterAsync("");
    //        ViewModel.SelectItem(null);
    //    }
    //}
}
