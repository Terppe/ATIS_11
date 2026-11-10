using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class TblFoodDetailPage : Page
{
    private readonly IDataService _dataService;
    private TblFood? _item;
    public TblFoodDetailPage()
    {
        _dataService = App.GetService<IDataService>();
        InitializeComponent();
        ViewModel = new TblFoodDetailViewModel(default, _dataService);
        DataContext = ViewModel;
    }
    public TblFoodDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not int id)
            throw new ArgumentException("A Food ID is required.", nameof(e));
        _item = null;
        DataContext = null;
        ExportPdfButton.IsEnabled = false;
        LoadingRing.Visibility = Visibility.Visible;
        LoadingRing.IsActive = true;
        try
        {
            _item = await _dataService.GetFoodByIdAsync(id)
                ?? throw new KeyNotFoundException($"Food {id} was not found.");

            ViewModel = new TblFoodDetailViewModel(_item, _dataService);

            DataContext = _item;
            ExportPdfButton.IsEnabled = true;
        }
        finally
        {
            LoadingRing.IsActive = false;
            LoadingRing.Visibility = Visibility.Collapsed;
        }
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }
    private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.ExportPdfCommand.ExecuteAsync(null);
    }
}
