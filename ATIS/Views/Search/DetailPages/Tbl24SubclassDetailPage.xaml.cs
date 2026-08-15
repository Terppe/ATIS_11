using ATIS.Interfaces;
using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class Tbl24SubclassDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private IDataService _dataService;

    public Tbl24SubclassDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl24SubclassDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl24SubclassDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id) return;

        // falls ViewModel schon existiert:
        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetSubclassByIdAsync(id);
            if (selected == null) return;

            ViewModel = new Tbl24SubclassDetailViewModel(selected, _dataService);
            DataContext = ViewModel;

            ViewModel.IsLoading = true; // nach New nochmal sicher setzen
            await ViewModel.InitializeAsync();
            Bindings.Update();
        }
        finally
        {
            if (ViewModel != null)
                ViewModel.IsLoading = false;
        }
    }


    //protected override async void OnNavigatedTo(NavigationEventArgs e)
    //{
    //    base.OnNavigatedTo(e);

    //    if (e.Parameter is int id)
    //    {
    //        var selected = await _dataService.GetSubclassByIdAsync(id); 
    //        if (selected == null) return;

    //        ViewModel = new Tbl24SubclassDetailViewModel(selected, _dataService);
    //        DataContext = ViewModel;

    //        await ViewModel.InitializeAsync();
    //        Bindings.Update();
    //    }
    //}
    public Task OnNavigateBreadcrumbAsync(BreadcrumbItem item)
    {
        // gleiche Logik wie Breadcrumb_Click / bestehende Navigation
        return ViewModel.NavigateBreadcrumbAsync(item);
    }
    private void ExecuteBack()
    {
        var frame = App.MainPageInstance.NavFrame;

        if (frame.CanGoBack)
            frame.GoBack();
    }
    private async void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        await PrintCurrentReviewAsync();
    }
    private Task PrintCurrentReviewAsync()
    {
        // TODO
        return Task.CompletedTask;
    }
    private void OpenPdf_Click(object sender, RoutedEventArgs e)
    {
        // Frame?.Navigate(typeof(Tbl21ClassReportPage), $"{_entityType}|{_entityId}");
    }

    private async void Breadcrumb_Click(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton button)
        {
            var label = button.Content?.ToString();

            var item = ViewModel.Breadcrumb.FirstOrDefault(x => x.Label == label);

            if (item != null)
            {

                await ViewModel.NavigateBreadcrumbAsync(item);
            }
        }
    }

}

