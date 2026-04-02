using System.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl90RefSource;

public sealed partial class Tbl90RefSourcePage : Page
{
    public Tbl90RefSourceViewModel ViewModel { get; }

    public Tbl90RefSourcePage()
    {
        InitializeComponent();
        ViewModel = App.GetService<Tbl90RefSourceViewModel>();
        DataContext = ViewModel;

        // Fokus auf SearchBox beim Laden
        Loaded += (_, _) => { SearchBox.Focus(FocusState.Programmatic); };

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ShouldFocusFirstField)
            && ViewModel.ShouldFocusFirstField)
        {
            FocusRefSourceName();
        }
    }

    public void FocusRefSourceName()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            RefSourceName.Focus(FocusState.Programmatic);
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
