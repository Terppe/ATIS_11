using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class Tbl21ClassDetailPage : Page
{
    public Tbl21ClassDetailPage()
    {
        InitializeComponent();
    }

    public Tbl21ClassDetailViewModel ViewModel { get; private set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (Tbl21ClassDetailViewModel)e.Parameter;
        base.OnNavigatedTo(e);
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

}
