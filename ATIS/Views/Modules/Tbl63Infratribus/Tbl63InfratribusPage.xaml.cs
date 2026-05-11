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

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //if (e.PropertyName == nameof(ViewModel.SelectedItem))
        //    UpdateRowSelection();
    }

    private async void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            await ViewModel.ApplyFilterAsync(SearchBox.Text);
     //       UpdateRowSelection();
        }
        else if (e.Key == VirtualKey.Escape)
        {
            SearchBox.Text = "";
            await ViewModel.ApplyFilterAsync("");
     //       ViewModel.SelectItem(null);
           // UpdateRowSelection();
        }
    }
    private void Row_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        // optional: Hover-Effekt
    }

    private void Row_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        // optional: Hover-Effekt
    }

    //private void Row_Tapped(object sender, TappedRoutedEventArgs e)
    //{
    //    if (sender is FrameworkElement fe && fe.DataContext is Models.Tbl63Infratribus item)
    //    {
    //        ViewModel.SelectItem(item);
    //    }
    //}

    //private void UpdateRowSelection()
    //{
    //    foreach (var row in _rows)
    //        row.IsSelected =
    //            ViewModel.SelectedItem != null &&
    //            row.Item != null &&
    //            row.Item.InfratribusId == ViewModel.SelectedItem.InfratribusId;
    //}

    //  private readonly List<Tbl63InfratribusRowControl> _rows = new();

    //private void RowRoot_Loaded(object sender, RoutedEventArgs e)
    //{

    //    //if (sender is Tbl63InfratribusRowControl row)
    //    //{
    //    //    row.ViewModelContext = ViewModel;   // ← HIER passiert die Magie
    //    //    _rows.Add(row);
    //    //}
    //    if (sender is Tbl63InfratribusRowControl row)
    //        _rows.Add(row);
    //}

    //private void RowRoot_Unloaded(object sender, RoutedEventArgs e)
    //{
    //    if (sender is Tbl63InfratribusRowControl row)
    //        _rows.Remove(row);
    //}


}
