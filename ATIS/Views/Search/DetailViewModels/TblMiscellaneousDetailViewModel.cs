using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblMiscellaneous;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class TblMiscellaneousDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;


    // -----------------------------
    //  TITLE
    // -----------------------------
    public string Title => $"Miscellaneous: {MiscellaneousSelected?.MiscellaneousName}";

    // -----------------------------
    //  SELECTED Miscellaneous
    // -----------------------------
    public TblMiscellaneous? MiscellaneousSelected { get; }

    // -----------------------------
    //  COMMANDS
    // -----------------------------
    public IAsyncRelayCommand ExportPdfCommand { get; }

    // ⭐ BackCommand
    public IRelayCommand BackCommand { get; }


    // -----------------------------
    //  CONSTRUCTOR
    // -----------------------------
    public TblMiscellaneousDetailViewModel(TblMiscellaneous selectedMiscellaneous, IDataService? dataService)
    {
        MiscellaneousSelected = selectedMiscellaneous;
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
    //  ISLOADING PROPERTY
    // -----------------------------
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // -----------------------------              
    //  PDF EXPORT
    // -----------------------------
    private async Task ExportPdfAsync()
    {
        var selected = MiscellaneousSelected;
        if (selected == null || selected.MiscellaneousId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.MiscellaneousName);
        var suggestedFileName = $"Miscellaneous_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await TblMiscellaneoussesDocumentDataSource.GetMiscellaneousDetailsAsync(selected.MiscellaneousId);
            return model == null ? null : new TblMiscellaneoussesDocument(model);
        }, suggestedFileName);
    }

}


