using ATIS.Interfaces;
using ATIS.Views.Report.Pdf.Tbl75Speciesses;
using ATIS.Views.Search.DetailViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Windows.Storage.Pickers;
using WinRT.Interop;

//  Tbl75SpeciesDetailPage.xaml.cs Skriptdatum:  18.08.2026  10:32     

namespace ATIS.Views.Search.DetailPages;


public sealed partial class Tbl75SpeciesDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private readonly IDataService _dataService;

    public Tbl75SpeciesDetailPage()
    {
        InitializeComponent();
        _dataService = App.Services.GetRequiredService<IDataService>();
        NavigateBreadcrumbAsync = OnNavigateBreadcrumbAsync;

        ViewModel = new Tbl75SpeciesDetailViewModel(default, _dataService);
        DataContext = ViewModel;
        ViewModel.IsLoading = true;
    }

    public Tbl75SpeciesDetailViewModel ViewModel { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is not int id)
            return;

        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetSpeciesWithGenusByIdAsync(id);
            if (selected == null)
                return;

            ViewModel = new Tbl75SpeciesDetailViewModel(selected, _dataService); DataContext = ViewModel;

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

        var speciesName = SanitizeFileNamePart(GetCurrentSpeciesName());
        var suggestedFileName = $"Species_Detail_{speciesName}_{DateTime.Now:yyyyMMdd_HHmmss}";

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

    //    var speciesName = SanitizeFileNamePart(GetCurrentSpeciesName());
    //    var suggestedFileName = $"Species_Detail_{speciesName}_{DateTime.Now:yyyyMMdd_HHmmss}";

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
        if (ViewModel?.SpeciesSelected == null)
            return null;

        var pdfModel = await Tbl75SpeciessesDocumentDataSource.GetSpeciesDetailsAsync(ViewModel.SpeciesSelected.SpeciesId);
        if (pdfModel == null)
            return null;

        return new Tbl75SpeciessesDocument(pdfModel);
    }

    private string? GetCurrentSpeciesName()
    {
        return ViewModel?.SpeciesSelected?.SpeciesName;
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
