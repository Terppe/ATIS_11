
using System.ComponentModel;
using System.Diagnostics;
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

        //Fokus auf Search
        Loaded += (_, _) => { SearchBox.Focus(FocusState.Programmatic); };

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ShouldFocusFirstField)
            && ViewModel.ShouldFocusFirstField)
        {
            FocusRefExpertName();
        }
    }

    public void FocusRefExpertName()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            RefExpertName.Focus(FocusState.Programmatic);
        });
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
