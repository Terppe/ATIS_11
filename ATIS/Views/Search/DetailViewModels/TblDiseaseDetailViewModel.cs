using ATIS;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblDisease;

public sealed class TblDiseaseDetailViewModel : ObservableObject
{
    public TblDisease? DiseaseSelected { get; }

    public IAsyncRelayCommand ExportPdfCommand { get; }
    public IRelayCommand BackCommand { get; }

    public TblDiseaseDetailViewModel(TblDisease? selectedDisease)
    {
        DiseaseSelected = selectedDisease;

        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }

    private void ExecuteBack()
    {
        var frame = App.MainPageInstance?.NavFrame;
        if (frame?.CanGoBack == true)
            frame.GoBack();
    }

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
            return new TblDiseasesDocument(model);
        }, suggestedFileName);
    }
}
