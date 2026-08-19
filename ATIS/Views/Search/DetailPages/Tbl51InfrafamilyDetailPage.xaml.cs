using ATIS.Interfaces;
using ATIS.Views.Search.DetailViewModels;
using Microsoft.UI.Xaml.Data;

//  Tbl51InfrafamilyDetailPage.xaml.cs Skriptdatum:  18.08.2026  10:32     

namespace ATIS.Views.Search.DetailPages;


public sealed partial class Tbl51InfrafamilyDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private IDataService _dataService;

    public Tbl51InfrafamilyDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl51InfrafamilyDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl51InfrafamilyDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id) return;

        // falls ViewModel schon existiert:
        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetInfrafamilyByIdAsync(id);
            if (selected == null) return;

            ViewModel = new Tbl51InfrafamilyDetailViewModel(selected, _dataService);
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
        // Frame?.Navigate(typeof(Tbl51InfrafamilyReportPage), $"{_entityType}|{_entityId}");
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
