
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl90RefExpert;

public sealed partial class Tbl90RefExpertPage : Page
{
    public Tbl90RefExpertViewModel ViewModel { get; }

    public Tbl90RefExpertPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<Tbl90RefExpertViewModel>();
        DataContext = ViewModel;
        Loaded += (_, _) =>
        {
            //       ViewModel.ApplyWidths(ActualWidth);
            SearchBox.Focus(FocusState.Programmatic);
        };
        //SizeChanged += (_, _) =>
        //{
        //    //        ViewModel.ApplyWidths(ActualWidth);
        //};
      //  ViewModel.SearchCompleted += OnSearchCompleted;
    }

    private void SearchBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ViewModel.SearchCommand.Execute(null);
        }
    }

    private async void OnSearchCompleted()
    {
        await Task.Delay(10);

        if (ViewModel.SelectedItem != null)
        {
            RefExpertDataGrid.Focus(FocusState.Programmatic);
        }
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (DataContext is Tbl90RefExpertViewModel vm)
        {
            vm.SearchText = sender.Text;
            vm.SearchCommand.Execute(null);
        }
    }
    private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid)!.Sort(e.Column, ViewModel.Sort);
    }


    private void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ViewModel.SearchCommand.Execute(null);
        }
    }
}
