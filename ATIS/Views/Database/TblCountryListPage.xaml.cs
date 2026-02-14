using System.Diagnostics;
using ATIS.ViewModels;
using ATIS.Views.ContentDialogs;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using WinUI3Localizer;

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
            DataContext = ViewModel;

            ViewModel.ApplyCulture();
            ViewModel.LoadCommand.Execute(null);

            Debug.WriteLine($"[CTOR] TblCountryListPage Hash={GetHashCode()} VM={ViewModel?.GetHashCode()} DC={DataContext?.GetType().Name}");
            if (ViewModel is null)
                throw new InvalidOperationException("TblCountryViewModel is not registered.");

            Debug.WriteLine($"Page-Konstruktor, ViewModel-Instanz: {ViewModel.GetHashCode()}");

            CommandProxy = this;


            ViewModel.RequestDeleteDialog = ShowDeleteDialogAsync;
            ViewModel.RequestSaveDialog = ShowSaveDialogAsync;

            ViewModel.RequestFocusName += FocusCountryName;

            var vm = (ViewModels.TblCountryViewModel)DataContext;
            vm.ScrollToSelectedRequest = ScrollToSelected;

            Debug.WriteLine($"PAGE VM: {ViewModel.GetHashCode()}");
            Debug.WriteLine($"Page DC: {DataContext?.GetType().Name} / IsLoading: {ViewModel?.IsLoading}");
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
        Debug.WriteLine($"[NAVIGATED] TblCountryListPage Hash={GetHashCode()} VM={ViewModel?.GetHashCode()} DC={DataContext?.GetType().Name}");
        await ViewModel.LoadAsync();
    }

    // =========================================================
    // Search
    // =========================================================

    private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Debug.WriteLine("🔥 QuerySubmitted fired: " + args.QueryText);

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
    private void FocusCountryName()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            CountryName.Focus(FocusState.Programmatic);
        });
    }

    private void CountryDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        (sender as DataGrid)!.Sort(e.Column, ViewModel.Sort);
    }


    public void ScrollToSelected()
    {
        if (CountryDataGrid?.SelectedItem != null)
        {
            CountryDataGrid.ScrollIntoView(
                CountryDataGrid.SelectedItem,
                null // oder eine konkrete Spalte
            );
        }
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (DataContext is TblCountryViewModel vm)
            vm.CurrentPageWidth = e.NewSize.Width;
    }

    private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
    }
}


