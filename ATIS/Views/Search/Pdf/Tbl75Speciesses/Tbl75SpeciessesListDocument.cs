using System.Collections.ObjectModel;
using System.Globalization;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl75Speciesses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Search.Pdf.Tbl75Speciesses;

internal class Tbl75SpeciessesListDocument(
    Tbl75SpeciessesPdfModel model,
    ObservableCollection<Tbl75Species> viewModelSpeciessesCollection) : IDocument
{

    public Tbl75SpeciessesPdfModel Model { get; } = model;

    private ObservableCollection<Tbl75Species> ViewModelSpeciessesCollection { get; } = viewModelSpeciessesCollection;

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
                text.Span(Strings.StringsRes.Species).FontSize(20).SemiBold();
                text.Span(
                    "                                                                                                       ");
                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
            });
            column.Spacing(5);
            column.Item().Element(ComposeListContent);
        });
    }

    private void ComposeListContent(IContainer container)
    {
        if (ViewModelSpeciessesCollection.Count == 0)
        {
            return;
        }

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(4);
                columns.RelativeColumn();
            });

            foreach (var item in ViewModelSpeciessesCollection)
            {
                table.Cell().Text("");
                table.Cell()
                    .Text($"{item.Genus.GenusName} {item.SpeciesName} {item.Subspecies} {item.Divers}" +
                          $" {item.Author} {item.AuthorYear}");
            }
        });
    }
}
