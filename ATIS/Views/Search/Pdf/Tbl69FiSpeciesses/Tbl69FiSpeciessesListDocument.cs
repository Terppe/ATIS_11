using System.Collections.ObjectModel;
using System.Globalization;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl69FiSpeciesses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Search.Pdf.Tbl69FiSpeciesses;
internal class Tbl69FiSpeciessesListDocument : IDocument
{
    public Tbl69FiSpeciessesPdfModel Model
    {
        get;
    }
    public ObservableCollection<Tbl69FiSpecies> ViewModelFiSpeciessesCollection
    {
        get;
    }
    public Tbl69FiSpeciessesListDocument(Tbl69FiSpeciessesPdfModel model, ObservableCollection<Tbl69FiSpecies> viewModelFiSpeciessesCollection)
    {
        Model = model;
        ViewModelFiSpeciessesCollection = viewModelFiSpeciessesCollection;
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
                text.Span(Strings.StringsRes.FiSpecies).FontSize(20).SemiBold();
                text.Span("                                                                                                       ");
                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
            });
            column.Spacing(5);
            column.Item().Element(ComposeListContent);
        });
    }

    private void ComposeListContent(IContainer container)
    {
        if (ViewModelFiSpeciessesCollection.Count == 0)
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

            foreach (var item in ViewModelFiSpeciessesCollection)
            {
                table.Cell().Text("");
                table.Cell().Text($"{item.Tbl66Genusses.GenusName} {item.FiSpeciesName} {item.Subspecies} {item.Divers}" +
                                  $" {item.Author} {item.AuthorYear}");
            }
        });
    }
}
