using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Search.DetailPages;
using ATIS.Views.Search.DetailViewModels;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Search;

public sealed partial class SearchPage : Page
{
    private readonly IDataService _dataService;

    public SearchPageViewModel ViewModel { get; }

    public SearchPage()
    {
        InitializeComponent();
        DataContext = this;

        var factory = App.Services.GetRequiredService<IDbContextFactory<AtisDbContext>>();
        var user = App.Services.GetRequiredService<IUserService>();
        _dataService = new DataService(factory, user);

        // ⭐ WICHTIG: ViewModel erzeugen und DataService übergeben
        ViewModel = new SearchPageViewModel(_dataService);
        Loaded += SearchPage_Loaded;

    }
    private bool _isLoaded;

    private async void SearchPage_Loaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        await Task.Delay(50);
        TableCombo.Focus(FocusState.Keyboard);
    }

    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.SearchCommand.ExecuteAsync(null);
    }
    private void OnTableSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selector = (SearchTemplateSelector)Resources["SearchSelector"];
        selector.SelectedTable = ViewModel.SelectedTable;

        SearchBox.Focus(FocusState.Programmatic);
    }

    private void OnRowSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0)
            return;

        var selectedClass = e.AddedItems[0] as Tbl21Class;
        if (selectedClass == null)
            return;

        var vm = new Tbl21ClassDetailViewModel(selectedClass, _dataService);

        App.MainPageInstance.NavFrame.Navigate(typeof(Tbl21ClassDetailPage), vm);
    }
}
