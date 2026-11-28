using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report.Pdf.TblFoods;

namespace ATIS.Views.Search.DetailViewModels;

public sealed class TblFoodDetailViewModel : ObservableObject
{
    public TblFood? FoodSelected { get; }

    public IAsyncRelayCommand ExportPdfCommand { get; }
    public IRelayCommand BackCommand { get; }

    public TblFoodDetailViewModel(TblFood? selectedFood)
    {
        FoodSelected = selectedFood;
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
        var selected = FoodSelected;
        if (selected == null || selected.FoodId == 0)
            return;

        var name = PdfExportService.SanitizeFileNamePart(selected.FoodName);
        var suggestedFileName = $"Food_Detail_{name}_{DateTime.Now:yyyyMMdd_HHmmss}";

        await App.GetService<IPdfExportService>().ExportAsync(async () =>
        {
            var model = await TblFoodsDocumentDataSource.GetFoodDetailsAsync(selected.FoodId);
            if (model == null)
                throw new InvalidOperationException("The food was not found for PDF export.");
            return new TblFoodsDocument(model);
        }, suggestedFileName);
    }
}
