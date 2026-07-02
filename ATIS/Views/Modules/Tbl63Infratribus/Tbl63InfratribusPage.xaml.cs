using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public sealed partial class Tbl63InfratribusPage : Page
{
    public Tbl63InfratribusViewModel ViewModel { get; }

    public Tbl63InfratribusPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl63InfratribusViewModel>();
        DataContext = ViewModel;

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAsync();
  //      SearchBox?.Focus(FocusState.Programmatic);
    }

    private async void SearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.ApplyFilterAsync(sender.Text);
    }
}
