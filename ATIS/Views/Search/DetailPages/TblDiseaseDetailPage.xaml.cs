using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search.DetailViewModels;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class TblDiseaseDetailPage : Page
{
    private readonly IDataService _dataService;
    private TblDisease? _item;
    public TblDiseaseDetailPage()
    {
        _dataService = App.GetService<IDataService>();
        InitializeComponent();
        ViewModel = new TblDiseaseDetailViewModel(default, _dataService);
        DataContext = ViewModel;
    }
    public TblDiseaseDetailViewModel ViewModel { get; private set; }
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is not int id)
            throw new ArgumentException("A Disease ID is required.", nameof(e));
        _item = null;
        DataContext = null;
        ExportPdfButton.IsEnabled = false;
        LoadingRing.Visibility = Visibility.Visible;
        LoadingRing.IsActive = true;
        try
        {
            _item = await _dataService.GetDiseaseByIdAsync(id)
                ?? throw new KeyNotFoundException($"Disease {id} was not found.");

            ViewModel = new TblDiseaseDetailViewModel(_item, _dataService);

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
