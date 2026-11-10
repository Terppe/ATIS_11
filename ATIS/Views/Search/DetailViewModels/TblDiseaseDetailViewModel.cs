using ATIS;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblDisease;

public sealed class TblDiseaseDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  SELECTED Disease
    // -----------------------------
    public TblDisease? DiseaseSelected { get; }

    // -----------------------------
    //  COMMANDS
    // -----------------------------
    public IAsyncRelayCommand ExportPdfCommand { get; }

    // ⭐ BackCommand
    public IRelayCommand BackCommand { get; }

    // -----------------------------
    //  CONSTRUCTOR
    // -----------------------------
    public TblDiseaseDetailViewModel(TblDisease selectedDisease, IDataService? dataService)
    {
        DiseaseSelected = selectedDisease;
        _dataService = dataService;

        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);

        // ⭐ BackCommand initialisieren
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }

    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------
    private void ExecuteBack()
    {
        var frame = App.MainPageInstance?.NavFrame;
        if (frame?.CanGoBack == true)
            frame.GoBack();
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        var selected = DiseaseSelected;
        if (selected == null || selected.DiseaseId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.DiseaseName);
        var suggestedFileName = $"Disease_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await TblDiseasesDocumentDataSource.GetDiseaseDetailsAsync(selected.DiseaseId);
            return model == null ? null : new TblDiseasesDocument(model);
        }, suggestedFileName);
    }

}


