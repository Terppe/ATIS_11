using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report;
using ATIS.Views.Report.Pdf.TblFoods;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class TblFoodDetailPage : Page
{
    private readonly IDataService _dataService;
    private TblFood? _item;

    public TblFoodDetailPage()
    {
        _dataService = App.GetService<IDataService>();
        InitializeComponent();
    }

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
        var item = _item ?? throw new InvalidOperationException("No Food is loaded.");
        var name = PdfExportService.SanitizeFileNamePart(item.FoodName);
        var suggestedFileName = $"Food_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(
            () => Task.FromResult<IDocument?>(new TblFoodsDocument(new TblFoodsPdfModel
            {
                FoodName = item.FoodName ?? string.Empty,
                CountId = item.CountId,
                Valid = item.Valid,
                ValidYear = item.ValidYear ?? string.Empty,
                ShortInfo = item.ShortInfo ?? string.Empty,
                FoodVolume = item.FoodVolume ?? string.Empty,
                Symptom = item.Symptom ?? string.Empty,
                Info = item.Info ?? string.Empty,
                Memo = item.Memo ?? string.Empty,
                Writer = item.Writer ?? string.Empty,
                WriterDate = item.WriterDate,
                Updater = item.Updater ?? string.Empty,
                UpdaterDate = item.UpdaterDate,
                FoodTree = new PdfModels.Food { FoodName = item.FoodName ?? string.Empty }
            })), suggestedFileName);
    }
}
