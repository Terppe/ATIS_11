using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblMiscellaneous;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class TblMiscellaneousDetailViewModel : ObservableObject
{
    public string Title => $"Miscellaneous: {MiscellaneousSelected?.MiscellaneousName}";

    public TblMiscellaneous? MiscellaneousSelected { get; }

    public IAsyncRelayCommand ExportPdfCommand { get; }
    public IRelayCommand BackCommand { get; }

    public TblMiscellaneousDetailViewModel(TblMiscellaneous? selectedMiscellaneous)
    {
        MiscellaneousSelected = selectedMiscellaneous;
        ExportPdfCommand = new AsyncRelayCommand(ExportPdfAsync);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);
    }

    private void ExecuteBack()
    {
        var frame = App.MainPageInstance?.NavFrame;
        if (frame?.CanGoBack == true)
            frame.GoBack();
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private async Task ExportPdfAsync()
    {
        var selected = MiscellaneousSelected;
        if (selected == null || selected.MiscellaneousId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.MiscellaneousName);
        var suggestedFileName = $"Miscellaneous_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await TblMiscellaneoussesDocumentDataSource.GetMiscellaneousDetailsAsync(
                selected.MiscellaneousId);

            return new TblMiscellaneoussesDocument(model);
        }, suggestedFileName);
    }
}
