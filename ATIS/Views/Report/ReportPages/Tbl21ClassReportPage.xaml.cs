

using ATIS.Views.Report.Pdf.Tbl21Classes;
using ATIS.Views.Search;
using QuestPDF.Fluent;

namespace ATIS.Views.Report.ReportPages;

public sealed partial class Tbl21ClassReportPage : Page
{
    private SearchResultItem? _item;

    public string HeaderText => _item is null
        ? "Kein Datensatz"
        : $"{_item.EntityType} – {_item.Title}";

    public string SubHeaderText => _item is null
        ? string.Empty
        : $"ID: {_item.EntityId}  |  {_item.Subtitle}";


    public Tbl21ClassReportPage()
    {
        InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is SearchResultItem item)
        {
            _item = item;
            Bindings.Update();
        }
    }

    private async void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (_item is null)
            return;

        if (!string.Equals(_item.EntityType, "Class", StringComparison.OrdinalIgnoreCase))
        {
            var unsupportedDialog = new ContentDialog
            {
                Title = "PDF",
                Content = $"Für '{_item.EntityType}' ist der PDF-Export aktuell noch nicht verdrahtet.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await unsupportedDialog.ShowAsync();
            return;
        }

        try
        {
            //var model = Tbl21ClassesDocumentDataSource.GetClassDetailsAsync(_item.EntityId);
            //var document = new Tbl21ClassesDocument(await model);
            var model = await Tbl21ClassesDocumentDataSource.GetClassDetailsAsync(_item.EntityId);
            var document = new Tbl21ClassesDocument(model);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var safeTitle = string.IsNullOrWhiteSpace(_item.Title)
                ? $"Class_{_item.EntityId}"
                : string.Concat(_item.Title.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeTitle}_{_item.EntityId}.pdf";
            var filePath = Path.Combine(desktopPath, fileName);

            document.GeneratePdf(filePath);

            var successDialog = new ContentDialog
            {
                Title = "PDF exportiert",
                Content = $"Die PDF wurde erstellt:\n{filePath}",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await successDialog.ShowAsync();
        }
        catch (Exception ex)
        {
            var errorDialog = new ContentDialog
            {
                Title = "PDF-Fehler",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        Frame?.Navigate(typeof(SearchPage));
    }

    private void TaxonomyNodeLink_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("TaxonomyNodeLink_Click reached");
    }
}
