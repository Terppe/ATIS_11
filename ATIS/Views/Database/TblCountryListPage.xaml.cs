using System.Diagnostics;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI.UI.Controls;

namespace ATIS.Views.Database;

public sealed partial class TblCountryListPage : Page
{
    public TblCountryViewModel ViewModel { get; }
    public TblCountryListPage CommandProxy { get; set; }

    public TblCountryListPage()
    {
        try
        {
            InitializeComponent();
            ViewModel = App.GetService<TblCountryViewModel>();
            Debug.WriteLine($"Page-Konstruktor, ViewModel-Instanz: {ViewModel.GetHashCode()}");
            this.DataContext = ViewModel;
            CommandProxy = this;

            ViewModel.RequestDeleteDialog = ShowDeleteDialogAsync;
            ViewModel.RequestSaveDialog = ShowSaveDialogAsync;

            ViewModel.RequestFocusName += FocusCountryName;
            Debug.WriteLine($"PAGE VM: {ViewModel.GetHashCode()}");
            Debug.WriteLine($"Page DC: {this.DataContext?.GetType().Name} / IsLoading: {ViewModel?.IsLoading}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("🔥 PAGE INIT ERROR:");
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.InnerException?.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace); throw;
        }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        await ViewModel.LoadAsync();
    }

    // =========================================================
    // Search
    // =========================================================

    private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.LoadSearchAsync(args.QueryText);
        CountryDataGrid.Focus(FocusState.Programmatic);
    }

    private void SearchBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        SearchBox.Focus(FocusState.Programmatic);
    }
    private async Task<bool> ShowDeleteDialogAsync()
    {
        var dialog = new AtisDeleteChangesDialog
        {
            XamlRoot = Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    private async Task<ContentDialogResult> ShowSaveDialogAsync()
    {
        var dialog = new AtisSaveChangesDialog
        {
            XamlRoot = Content.XamlRoot
        };

        return await dialog.ShowAsync();
    }
    //private void CountryName_Loaded(object sender, RoutedEventArgs e)
    //{
    //    var vm = DataContext as TblCountryViewModel;

    //    if (vm?.CountrySelected?.CountryId == 0)
    //    {
    //        // Fokus erst setzen, wenn UI bereit ist
    //        DispatcherQueue.TryEnqueue(() =>
    //        {
    //            CountryName.Focus(FocusState.Programmatic);
    //        });
    //    }
    //}
    private void FocusCountryName()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            CountryName.Focus(FocusState.Programmatic);
        });
    }
    //private void CountryDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    //{
    //    (sender as DataGrid).Sort(e.Column, ViewModel.Sort);
    //}

    private void CountryDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid)!.Sort(e.Column, ViewModel.Sort);
    }

}


