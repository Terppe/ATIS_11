using System.Diagnostics;
using ATIS.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ATIS.Services;



public sealed class PdfExportService(IDialogService dialogService) : IPdfExportService
{
    private readonly SemaphoreSlim _exportLock = new(1, 1);

    public async Task ExportAsync(Func<Task<IDocument?>> createDocumentAsync, string suggestedFileName)
    {
        ArgumentNullException.ThrowIfNull(createDocumentAsync);

        // Keep pickers and export dialogs from overlapping across pages and commands.
        await _exportLock.WaitAsync();
        try
        {
            string filePath;
            try
            {
                var document = await createDocumentAsync();
                if (document == null)
                {
                    await dialogService.ShowInfoAsync("Dialog_Info_Title", "PdfExport_Hidden_Message");
                    return;
                }

                var window = App.MainWindow
                    ?? throw new InvalidOperationException("The main window is not available for PDF export.");
                var savePicker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = SanitizeFileNamePart(suggestedFileName, "PDF")
                };
                savePicker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });
                InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(window));

                var file = await savePicker.PickSaveFileAsync();
                if (file == null)
                    return;

                filePath = file.Path;
                document.GeneratePdf(filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                await dialogService.ShowOkDialogAsync(
                    "PdfExport_Error_Title", "PdfExport_Error_Message", ex.ToString());
                return;
            }

            await dialogService.ShowOkDialogAsync(
                "PdfExport_Success_Title", "PdfExport_Success_Message", filePath);
        }
        finally
        {
            _exportLock.Release();
        }
    }

    public static string SanitizeFileNamePart(string? value, string fallback = "Unknown")
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Where(c => !invalidChars.Contains(c)).ToArray())
            .Trim().TrimEnd('.');

        return string.IsNullOrWhiteSpace(cleaned) ? fallback : cleaned;
    }
}
