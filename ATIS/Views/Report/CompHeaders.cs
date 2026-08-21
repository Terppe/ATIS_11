using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WinUI3Localizer;

namespace ATIS.Views.Report;
public class CompHeaders
{
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    public static void Header(IContainer container, int textHight)
    {
        // evtl icon
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Span(Loc("Atis")).FontSize(18).SemiBold();
                    text.Span("  ");
                    text.Span(Loc("AtisHeader")).FontSize(textHight).SemiBold();
                    text.Span("  ");
                    text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
                });
            });
        });
    }
    public static void HeaderSynonym(IContainer container, int tabLeftRel, int tabRightRel)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel);
                        columns.ConstantColumn(tabRightRel);
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Synonyms"));
                });

            });
        });
    }
    public static void HeaderName(IContainer container, int tabLeftRel, int tabRightRel)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel);
                        columns.ConstantColumn(tabRightRel);
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Names"));
                });

            });
        });
    }

    public static void HeaderTaxoHiera(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("TaxonomicHierarchy")).FontSize(textHight).SemiBold();
                });
            });
        });
    }
    public static void HeaderChildren(IContainer container, int tabRightRel, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabRightRel);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("DirectChildren")).FontSize(textHight).SemiBold();
                });
            });
        });
    }
    public static void HeaderHeaderReferences(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("References")).FontSize(textHight).SemiBold();
                });
            });
        });
    }
    public static void HeaderReferencesExperts(IContainer container, int tabLeftRel8)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Experts")).SemiBold();
                });
            });
        });
    }
    public static void HeaderReferencesOtherSources(IContainer container, int tabLeftRel8)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("OtherSources")).SemiBold();
                });
            });
        });
    }
    public static void HeaderReferencesPublications(IContainer container, int tabLeftRel8)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Publications")).SemiBold();
                });
            });
        });
    }
    public static void HeaderGeographics(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Geographics")).FontSize(textHight).SemiBold();
                });
            });
        });
    }
    public static void HeaderImages(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Images")).FontSize(textHight).SemiBold();
                });
            });
        });
    }

    public static void HeaderUsefulInfos(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Useful")).FontSize(textHight).SemiBold();
                });
            });
        });
    }
    public static void HeaderComments(IContainer container, int tabLeftRel4, int textHight)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(tabLeftRel4);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Comments")).FontSize(textHight).SemiBold();
                });
            });
        });
    }

}
