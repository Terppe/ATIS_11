using System.ComponentModel;
using ATIS.Views.Database;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public sealed partial class Tbl90RefAuthorPage : Page
{
    public Tbl90RefAuthorViewModel ViewModel { get; }

    public Tbl90RefAuthorPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<Tbl90RefAuthorViewModel>();
        DataContext = ViewModel;

        Loaded += Tbl90AuthorPage_Loaded;

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    // ---------------------------------------------------------
    //  PAGE LOADED → Fokus auf Suchfeld
    // ---------------------------------------------------------
    private void Tbl90AuthorPage_Loaded(object sender, RoutedEventArgs e)
    {
        SearchBox.Focus(FocusState.Programmatic);
    }

    // ---------------------------------------------------------
    //  VIEWMODEL → Fokus auf erstes Feld nach "Neu"
    // ---------------------------------------------------------
    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ShouldFocusFirstField)
            && ViewModel.ShouldFocusFirstField)
        {
            FocusFirstField();
        }
    }

    private void FocusFirstField()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            RefAuthorName.Focus(FocusState.Programmatic);
            ViewModel.ShouldFocusFirstField = false;
        });
    }

    // ---------------------------------------------------------
    //  DATAGRID SORTING
    // ---------------------------------------------------------
    private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        if (sender is DataGrid grid)
            grid.Sort(e.Column, ViewModel.Sort);
    }

    // ---------------------------------------------------------
    //  SEARCHBOX ENTER
    // ---------------------------------------------------------
    private void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            ViewModel.SearchCommand.Execute(null);
    }
}
