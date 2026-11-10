using ATIS.Interfaces;
using ATIS.Views.Search.DetailViewModels;

//  Tbl39InfraordoDetailPage.xaml.cs Skriptdatum:  15.06.2024  10:32     

namespace ATIS.Views.Search.DetailPages;


public sealed partial class Tbl39InfraordoDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private readonly IDataService _dataService;

    public Tbl39InfraordoDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl39InfraordoDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl39InfraordoDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id)
            return;

        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetInfraordoByIdAsync(id);
            if (selected == null)
                return;

            ViewModel = new Tbl39InfraordoDetailViewModel(selected, _dataService);
            DataContext = ViewModel;

            ViewModel.IsLoading = true;
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
        return ViewModel.NavigateBreadcrumbAsync(item);
    }

    private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.ExportPdfCommand.ExecuteAsync(null);
    }

    private async void Breadcrumb_Click(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton { DataContext: BreadcrumbItem item })
        {
            await ViewModel.NavigateBreadcrumbAsync(item);
        }
    }
}
