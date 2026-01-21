
using ATIS.Views.ContentDialogs;

namespace ATIS.Views.Database;

public sealed partial class TblCountryListPage : Page
{
    public TblCountryViewModel ViewModel { get; set; }
    public TblCountryListPage CommandProxy { get; set; }

    public TblCountryListPage()
    {
        try
        {
            InitializeComponent();
            Loaded += OnLoaded;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("🔥 PAGE INIT ERROR:");
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.InnerException?.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace); throw;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel = App.GetService<TblCountryViewModel>();
        DataContext = ViewModel;
        CommandProxy = this;
        Loaded += async (_, __) =>
        {
            await ViewModel.LoadAsync();
        };
        ViewModel.RequestDeleteDialog = ShowDeleteDialogAsync;
        ViewModel.RequestSaveDialog = ShowSaveDialogAsync;
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
    private void CountryDataGrid_GotFocus(object sender, RoutedEventArgs e)
    {
 //       ViewModel.DataGridHasFocus = true;
    }

    private void CountryDataGrid_LostFocus(object sender, RoutedEventArgs e)
    {
  //      ViewModel.DataGridHasFocus = false;
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
    private void CountryName_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as TblCountryViewModel;

        if (vm?.CountrySelected?.CountryId == 0)
            CountryName.Focus(FocusState.Programmatic);
    }
}

