using ATIS.Interfaces;
using ATIS.Views.Report.Pdf.Tbl66Genusses;
using ATIS.Views.Search.DetailViewModels;
using Microsoft.UI.Xaml.Data;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Windows.Storage.Pickers;
using WinRT.Interop;

//  Tbl66GenusDetailPage.xaml.cs Skriptdatum:  18.08.2026  10:32     

namespace ATIS.Views.Search.DetailPages;


public sealed partial class Tbl66GenusDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private readonly IDataService _dataService;

    public Tbl66GenusDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl66GenusDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl66GenusDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id)
            return;

        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetGenusByIdAsync(id);
            if (selected == null)
                return;

            ViewModel = new Tbl66GenusDetailViewModel(selected, _dataService);
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
        var document = await CreateDetailDocumentAsync();
        if (document == null)
            return;

        var genusName = SanitizeFileNamePart(GetCurrentGenusName());
        var suggestedFileName = $"Genus_Detail_{genusName}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await SaveDocumentAsPdfAsync(document, suggestedFileName);
    }

    //für später andere nutzung, z.B. Drucken
    //private async void PrintButton_Click(object sender, RoutedEventArgs e)
    //{
    //    await PrintCurrentReviewAsync();
    //}

    //private async Task PrintCurrentReviewAsync()
    //{
    //    var document = await CreateDetailDocumentAsync();
    //    if (document == null)
    //        return;

    //    var genusName = SanitizeFileNamePart(GetCurrentGenusName());
    //    var suggestedFileName = $"Genus_Detail_{genusName}_{DateTime.Now:yyyyMMdd_HHmmss}";

    //    await SaveDocumentAsPdfAsync(document, suggestedFileName);
    //}

    private async void Breadcrumb_Click(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton button)
        {
            var label = button.Content?.ToString();
            var item = ViewModel.Breadcrumb.FirstOrDefault(x => x.Label == label);

            if (item != null)
                await ViewModel.NavigateBreadcrumbAsync(item);
        }
    }

    private async Task<IDocument?> CreateDetailDocumentAsync()
    {
        if (ViewModel?.GenusSelected == null)
            return null;

        var pdfModel = await Tbl66GenussesDocumentDataSource.GetGenusDetailsAsync(ViewModel.GenusSelected.GenusId);
        if (pdfModel == null)
            return null;

        return new Tbl66GenussesDocument(pdfModel);
    }

    private string? GetCurrentGenusName()
    {
        return ViewModel?.GenusSelected?.GenusName;
    }

    private async Task SaveDocumentAsPdfAsync(IDocument document, string suggestedFileName)
    {
        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = suggestedFileName
        };

        savePicker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
            return;

        document.GeneratePdf(file.Path);
    }

    private static string SanitizeFileNamePart(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        return string.IsNullOrWhiteSpace(cleaned) ? "Unknown" : cleaned.Trim();
    }
}
