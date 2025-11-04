using System.Collections.ObjectModel;
using System.Globalization;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl72PlSpeciesses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Search.Pdf.Tbl72PlSpeciesses;
internal class Tbl72PlSpeciessesListDocument : IDocument
{
    public Tbl72PlSpeciessesPdfModel Model
    {
        get;
    }
    public ObservableCollection<Tbl75Species> ViewModelPlSpeciessesCollection
    {
        get;
    }
    public Tbl72PlSpeciessesListDocument(Tbl72PlSpeciessesPdfModel model, ObservableCollection<Tbl75Species> viewModelPlSpeciessesCollection)
    {
        Model = model;
        ViewModelPlSpeciessesCollection = viewModelPlSpeciessesCollection;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    [Obsolete("Obsolete")]
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

    [Obsolete("Obsolete")]
    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text(text =>
            {
                text.Span(Strings.StringsRes.List).FontSize(20).SemiBold();
                text.Span("          ");
                text.Span(Strings.StringsRes.PlSpecies).FontSize(20).SemiBold();
                text.Span("                                                                                                       ");
                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
            });
            column.Spacing(5);
            column.Item().Element(ComposeListContent);
        });
    }

    [Obsolete]
    private void ComposeListContent(IContainer container)
    {
        if (ViewModelPlSpeciessesCollection.Count == 0)
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

            foreach (var item in ViewModelPlSpeciessesCollection)
            {
                table.Cell().Text("");
                table.Cell().Text($"{item.Tbl66Genusses.GenusName} {item.SpeciesName} {item.Subspecies} {item.Divers}" +
                                  $" {item.Author} {item.AuthorYear}").WrapAnywhere();
            }
        });
    }

}
