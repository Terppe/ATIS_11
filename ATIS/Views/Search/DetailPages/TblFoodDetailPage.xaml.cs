using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report;
using ATIS.Views.Report.Pdf.TblFoods;
using QuestPDF.Fluent;
using Windows.Storage.Pickers;
using WinRT.Interop;

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
        var document = new TblFoodsDocument(new TblFoodsPdfModel
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
        });

        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = $"Food_Detail_{SanitizeFileNamePart(item.FoodName)}_{DateTime.Now:yyyyMMdd_HHmmss}"
        };
        savePicker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });
        InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(App.MainWindow));
        var file = await savePicker.PickSaveFileAsync();
        if (file != null)
            document.GeneratePdf(file.Path);
    }

    private static string SanitizeFileNamePart(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Where(c => !invalidChars.Contains(c)).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "Unknown" : cleaned;
    }
}
