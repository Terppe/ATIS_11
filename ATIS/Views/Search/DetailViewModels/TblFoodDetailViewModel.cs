using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblFoods;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class TblFoodDetailViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    // -----------------------------
    //  SELECTED Food
    // -----------------------------
    public TblFood? FoodSelected { get; }

    // -----------------------------
    //  COMMANDS
    // -----------------------------
    public IAsyncRelayCommand ExportPdfCommand { get; }

    // ⭐ BackCommand
    public IRelayCommand BackCommand { get; }

    // -----------------------------
    //  CONSTRUCTOR
    // -----------------------------
    public TblFoodDetailViewModel(TblFood selectedFood, IDataService? dataService)
    {
        FoodSelected = selectedFood;
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
        var selected = FoodSelected;
        if (selected == null || selected.FoodId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.FoodName);
        var suggestedFileName = $"Food_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await TblFoodsDocumentDataSource.GetFoodDetailsAsync(selected.FoodId);
            return model == null ? null : new TblFoodsDocument(model);
        }, suggestedFileName);
    }

}
