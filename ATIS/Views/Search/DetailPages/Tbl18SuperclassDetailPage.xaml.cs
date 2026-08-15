using ATIS.Interfaces;
using ATIS.Views.Search.DetailViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Search.DetailPages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl18SuperclassDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private IDataService _dataService;

    public Tbl18SuperclassDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl18SuperclassDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl18SuperclassDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id) return;

        // falls ViewModel schon existiert:
        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetSuperclassByIdAsync(id);
            if (selected == null) return;

            ViewModel = new Tbl18SuperclassDetailViewModel(selected, _dataService);
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
