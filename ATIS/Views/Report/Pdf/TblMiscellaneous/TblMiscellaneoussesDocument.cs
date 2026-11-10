using System.Configuration;
using System.Globalization;
using ATIS.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinUI3Localizer;

namespace ATIS.Views.Report.Pdf.TblMiscellaneous;
public class TblMiscellaneoussesDocument(TblMiscellaneoussesPdfModel model) : IDocument
{
    private TblMiscellaneoussesPdfModel Model { get; } = model;
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

  //  private static int _tabLeftRel;
    private static readonly int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
  //  private const int TabLeftRel4 = 4;
    private const int TabLeftRel8 = 8;
 //   private static int _tabLeftLastRel;
 //   private static int _tabRightLastRel;
    private static readonly int TabRightRel8 = _tabRightRel - 8;
    private const int TextHight = 12;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    private static string FormatValidity(bool? valid, string? validYear)
    {
        string status = valid switch
        {
            true => Loc("Valid"),
            false => Loc("Invalid"),
            null => string.Empty
        };

        if (string.IsNullOrWhiteSpace(status))
            return string.IsNullOrWhiteSpace(validYear) ? string.Empty : validYear;

        return string.IsNullOrWhiteSpace(validYear)
            ? status
            : $"{status} {Loc("Since")} {validYear}";
    }

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.MarginVertical(50);
                page.MarginLeft(30);
                page.MarginRight(10);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Size(PageSizes.A4);
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
    }

    private void ComposeHeader(IContainer container)
    {
        //  CompHeaders.Header(container, TextHight);
        // evtl icon
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Span(Loc("Atis")).FontSize(18).SemiBold();
                    text.Span("  ");
                    text.Span(Loc("AtisHeader")).FontSize(TextHight).SemiBold();
                    text.Span("                                   ");
                    text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();

                });
                column.Item().Text(text =>
                {
                    text.Span("  ");
                    text.Span("  ");
                    text.Span(Model.MiscellaneousName ?? string.Empty).FontSize(TextHight).SemiBold().Italic();
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Element(ComposeHeaderMiscellaneous);
            column.Spacing(5);
            column.Item().Element(ComposeMiscellaneousContent);
        });
    }

    private void ComposeHeaderMiscellaneous(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Span(Loc("CountId"));
                    text.Span($"{Model.CountId}");
                });
            });
        });
    }

    private void ComposeMiscellaneousContent(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Miscellaneous"));
                    table.Cell().Text($@"{Model.MiscellaneousTree?.MiscellaneousName}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("CurrentStanding"));
                    table.Cell().Text(ValidityFormatter.Format(Model.Valid, Model.ValidYear));
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("RecordUpdate"));
                    table.Cell().Text($"{Model.UpdaterDate:d}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("ShortInfo"));
                    table.Cell().Text($"{Model.ShortInfo}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Thema1"));
                    table.Cell().Text($"{Model.Thema1}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Thema2"));
                    table.Cell().Text($"{Model.Thema2}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Info"));
                    table.Cell().Text($"{Model.Info}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.ConstantColumn(TabRightRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("Memo"));
                    table.Cell().Text($"{Model.Memo}");
                });
            });
        });
    }







}
