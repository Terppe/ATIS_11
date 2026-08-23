using System.Collections.ObjectModel;
using System.Globalization;
using ATIS.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinUI3Localizer;

namespace ATIS.Views.Search.Pdf.Tbl21Classes;

public class Tbl21ClassesListDocument : IDocument
{
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    public ObservableCollection<Tbl21Class> VisibleItems { get; }

    public Tbl21ClassesListDocument(ObservableCollection<Tbl21Class> visibleItems)
    {
        VisibleItems = visibleItems;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
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
                text.Span(Loc("List")).FontSize(20).SemiBold();
                text.Span("          ");
                text.Span(Loc("Class")).FontSize(20).SemiBold();
                text.Span("          ");
                text.Span($"({VisibleItems.Count})").FontSize(14);

                text.Span("                                                                                                       ");
                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
            });

            column.Spacing(5);
            column.Item().Element(ComposeListContent);
        });
    }

    private void ComposeListContent(IContainer container)
    {
        if (VisibleItems.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(4);
                columns.ConstantColumn(104);
                columns.RelativeColumn();
            });

            foreach (var item in VisibleItems)
            {
                table.Cell().Text("");
                table.Cell().Text(item.ClassName ?? string.Empty);
                table.Cell().Text(string.Join(" ", new[]
                {
                    item.Author,
                    item.AuthorYear,
                    item.EngName,
                    item.GerName,
                    item.FraName,
                    item.PorName
                }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }
        });
    }
}

//internal class Tbl21ClassesListDocument : IDocument
//{
//    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

//    public Tbl21ClassesPdfModel Model
//    {
//        get;
//    }
//    public ObservableCollection<Tbl21Class> ViewModelClassesCollection
//    {
//        get;
//    }
//    public Tbl21ClassesListDocument(Tbl21ClassesPdfModel model, ObservableCollection<Tbl21Class> viewModelClassesCollection)
//    {
//        Model = model;
//        ViewModelClassesCollection = viewModelClassesCollection;
//    }

//    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
//    public void Compose(IDocumentContainer container)
//    {
//        container
//            .Page(page =>
//            {
//                page.MarginVertical(50);
//                page.MarginLeft(30);
//                page.MarginRight(20);
//                page.DefaultTextStyle(x => x.FontSize(10));
//                page.Size(PageSizes.A4);
//                page.Content().Element(ComposeContent);
//                page.Footer().AlignCenter().Text(text =>
//                {
//                    text.CurrentPageNumber();
//                    text.Span(" / ");
//                    text.TotalPages();
//                });
//            });
//    }

//    private void ComposeContent(IContainer container)
//    {
//        container.PaddingVertical(10).Column(column =>
//        {
//            column.Item().Text(text =>
//            {
//                text.Span(Strings.StringsRes.List).FontSize(20).SemiBold();
//                text.Span("          ");
//                text.Span(Loc("Class")).FontSize(20).SemiBold();

//                text.Span("                                                                                                       ");
//                text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
//            });
//            column.Spacing(5);
//            column.Item().Element(ComposeListContent);
//        });
//    }

//    private void ComposeListContent(IContainer container)
//    {
//        if (ViewModelClassesCollection.Count == 0)
//        {
//            return;
//        }

//        container.Table(table =>
//        {
//            table.ColumnsDefinition(columns =>
//            {
//                columns.ConstantColumn(4);
//                columns.ConstantColumn(104);
//                columns.RelativeColumn();
//            });

//            foreach (var item in ViewModelClassesCollection)
//            {
//                table.Cell().Text("");
//                table.Cell().Text($"{item.ClassName}");
//                table.Cell().Text($"{item.Author} {item.AuthorYear}  {item.EngName} {item.GerName} {item.FraName} {item.PorName}");
//            }
//        });
//    }
//}
