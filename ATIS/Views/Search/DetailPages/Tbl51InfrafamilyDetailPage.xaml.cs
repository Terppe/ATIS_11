using ATIS.Interfaces;
using ATIS.Views.Report.Pdf.Tbl51Infrafamilies;
using ATIS.Views.Search.DetailViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WinUI3Localizer;

//  Tbl51InfrafamilyDetailPage.xaml.cs Skriptdatum:  18.08.2026  10:32     

namespace ATIS.Views.Search.DetailPages;


public sealed partial class Tbl51InfrafamilyDetailPage : Page
{
    public Func<BreadcrumbItem, Task> NavigateBreadcrumbAsync { get; }
    private readonly IDataService _dataService;

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

        if (e.Parameter is not int id)
            return;

        ViewModel.IsLoading = true;

        try
        {
            var selected = await _dataService.GetInfrafamilyByIdAsync(id);
            if (selected == null)
                return;

            ViewModel = new Tbl51InfrafamilyDetailViewModel(selected, _dataService);
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
        try
        {
            var document = await CreateDetailDocumentAsync();
            if (document == null)
                return;

            var infrafamilyName = SanitizeFileNamePart(GetCurrentInfrafamilyName());
            var suggestedFileName =
                $"Infrafamily_Detail_{infrafamilyName}_{DateTime.Now:yyyyMMdd_HHmmss}";

            await SaveDocumentAsPdfAsync(document, suggestedFileName);
        }
        catch (Exception ex) when (
            ex is QuestPDF.Drawing.Exceptions.DocumentComposeException or
                QuestPDF.Drawing.Exceptions.DocumentLayoutException or
                IOException or UnauthorizedAccessException)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());

            var loc = Localizer.Get();
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = loc.GetLocalizedString("PdfExport_Error_Title"),
                Content = string.Format(
                    loc.GetLocalizedString("PdfExport_Error"), ex.Message),
                CloseButtonText = loc.GetLocalizedString("Dialog_Ok")
            };

            await dialog.ShowAsync();
        }
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

    //    var infrafamilyName = SanitizeFileNamePart(GetCurrentInfrafamilyName());
    //    var suggestedFileName = $"Infrafamily_Detail_{infrafamilyName}_{DateTime.Now:yyyyMMdd_HHmmss}";

    //    await SaveDocumentAsPdfAsync(document, suggestedFileName);
    //}

    private async void Breadcrumb_Click(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton { DataContext: BreadcrumbItem item })
        {
            await ViewModel.NavigateBreadcrumbAsync(item);
        }
    }

    private async Task<IDocument?> CreateDetailDocumentAsync()
    {
        if (ViewModel?.InfrafamilySelected == null)
            return null;

        var pdfModel = await Tbl51InfrafamiliesDocumentDataSource.GetInfrafamilyDetailsAsync(ViewModel.InfrafamilySelected.InfrafamilyId);
        if (pdfModel == null)
            return null;

        return new Tbl51InfrafamiliesDocument(pdfModel);
    }

    private string? GetCurrentInfrafamilyName()
    {
        return ViewModel?.InfrafamilySelected?.InfrafamilyName;
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
