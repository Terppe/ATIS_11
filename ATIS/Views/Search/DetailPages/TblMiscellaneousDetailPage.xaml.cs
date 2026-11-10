using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class TblMiscellaneousDetailPage : Page
{
    private readonly IDataService _dataService;
    private TblMiscellaneous? _item;
    public TblMiscellaneousDetailPage()
    {
        _dataService = App.GetService<IDataService>();
        InitializeComponent();
        ViewModel = new TblMiscellaneousDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    private TblMiscellaneousDetailViewModel ViewModel { get; set; }
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not int id)
            throw new ArgumentException("A Miscellaneous ID is required.", nameof(e));
        _item = null;
        DataContext = null;
        ExportPdfButton.IsEnabled = false;
        LoadingRing.Visibility = Visibility.Visible;
        LoadingRing.IsActive = true;
        try
        {
            _item = await _dataService.GetMiscellaneousByIdAsync(id)
                    ?? throw new KeyNotFoundException(
                        $"Miscellaneous {id} was not found.");

            ViewModel = new TblMiscellaneousDetailViewModel(_item, _dataService);

            DataContext = _item;
            ExportPdfButton.IsEnabled = true;
        }
        finally
        {
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
    private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.ExportPdfCommand.ExecuteAsync(null);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

}
