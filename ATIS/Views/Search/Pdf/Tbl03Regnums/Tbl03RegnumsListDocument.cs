using System.Collections.ObjectModel;
using System.Globalization;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl03Regnums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Search.Pdf.Tbl03Regnums;

public class Tbl03RegnumsListDocument : IDocument
{
    public Tbl03RegnumsPdfModel Model
    {
        get;
    }

    public ObservableCollection<Tbl03Regnum> ViewModelRegnumsCollection { get; }

    public Tbl03RegnumsListDocument(Tbl03RegnumsPdfModel model, ObservableCollection<Tbl03Regnum> viewModelRegnumsCollection)
    {
        Model = model;
        ViewModelRegnumsCollection = viewModelRegnumsCollection;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.MarginVertical(50);
                page.MarginLeft(30);
                page.MarginRight(20);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Size(PageSizes.A4);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text(text =>
            {
                text.Span(Strings.StringsRes.List).FontSize(20).SemiBold();
                text.Span("          ");
                text.Span(Strings.StringsRes.Regnum).FontSize(20).SemiBold();
                text.Span("                                                                                                       ");
                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
            });

            column.Spacing(5);
            column.Item().Element(ComposeListContent);
        });
    }

    private void ComposeListContent(IContainer container)
    {
        if (ViewModelRegnumsCollection.Count == 0)
        {
            return;
        }

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(4);
                columns.ConstantColumn(150);
                columns.RelativeColumn();
            });
            foreach (var item in ViewModelRegnumsCollection)
            {
                table.Cell().Text("");
                table.Cell().Text($"{item.RegnumName} {item.Subregnum}");
                table.Cell().Text($"{item.Author} {item.AuthorYear}  {item.EngName} {item.GerName} {item.FraName} {item.PorName}");
            }
        });
    }
}
