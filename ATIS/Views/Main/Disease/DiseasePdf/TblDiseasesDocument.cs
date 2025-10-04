using System.Configuration;
using System.Globalization;
using ATIS.Views.Report;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Main.Disease.DiseasePdf
{
    public class TblDiseasesDocument() : IDocument
    {
        private TblDiseasesPdfModel Model { get; }

        private static int _tabLeftRel;
        private static int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
        private const int TabLeftRel4 = 4;
        private const int TabLeftRel8 = 8;
        private static int _tabLeftLastRel;
        private static int _tabRightLastRel;
        private static readonly int TabRightRel8 = _tabRightRel - 8;
        private const int TextHight = 12;

        public TblDiseasesDocument(TblDiseasesPdfModel model) : this()
        {
            Model = model;
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

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
                        text.Span(Strings.StringsRes.Atis).FontSize(18).SemiBold();
                        text.Span("  ");
                        text.Span(Strings.StringsRes.AtisHeader).FontSize(TextHight).SemiBold();
                        text.Span("                     ");
                        text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();

                    });
                    column.Item().Text(text =>
                    {
                        text.Span($"{Model.DiseaseName}").FontSize(TextHight).SemiBold().Italic();
                        text.Span($" {Model.ShortInfo}").FontSize(TextHight).SemiBold().Italic();
                        text.Span($" {Model.Symptom}").FontSize(TextHight).SemiBold();
                        text.Span($" {Model.Reason}").FontSize(TextHight).SemiBold();
                        text.Span($" {Model.Curability}").FontSize(TextHight).SemiBold();
                        text.Span($" {Model.Therapie}").FontSize(TextHight).SemiBold();
                    });
                });
            });

        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                //Taxonomy and Nomenclature
                //column.Item().Element(ComposeHeaderTaxoNomen);
                //column.Spacing(5);
                //column.Item().Element(ComposeTaxoNomenContent);
                //column.Spacing(5);

                //Taxonomy Hierarchy
                //column.Item().Element(ComposeHeaderTaxoHiera);
                //column.Spacing(5);
                ////       column.Item().Element(ComposeTaxoHieraContent);
                //column.Spacing(5);

                //Images
           //     column.Item().Element(ComposeHeaderImages);
                column.Spacing(5);
            });
        }
        private void ComposeHeaderTaxoNomen(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(text =>
                    {
                        text.Span($"{Model.DiseaseName}").FontSize(TextHight).SemiBold().Italic();
                        text.Span($" {Model.ShortInfo}").FontSize(TextHight).SemiBold().Italic();
                        //text.Span($" {Model.Author}").FontSize(TextHight).SemiBold();
                        //text.Span($" {Model.AuthorYear}").FontSize(TextHight).SemiBold();
                    });
                    column.Item().Text(text =>
                    {
                        text.Span(Strings.StringsRes.ReportTaxonomicId);
                        text.Span($"{Model.CountId}");
                    });
                });
            });
        }

        private void ComposeTaxoNomenContent(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(TabLeftRel4);
                            columns.RelativeColumn();
                        });
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportTaxoNomen).FontSize(TextHight).SemiBold();
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
                        table.Cell().Text(Strings.StringsRes.Information);
                        table.Cell().Text($@"{Model.DiseaseTree?.DiseaseName} {Model.DiseaseTree?.ShortInfo}");
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
                        table.Cell().Text(Strings.StringsRes.ReportTaxoRank);
                        table.Cell().Text(Strings.StringsRes.Regnum);
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
                        table.Cell().Text(Strings.StringsRes.ReportSynonyms);
                        table.Cell().Text($"{Model.Symptom}");
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
                        table.Cell().Text(Strings.StringsRes.ReportNames);
                        table.Cell().Text($"{Model.Therapie}[ENG] {Model.Therapie}[GER] {Model.Therapie}[FRA] {Model.Therapie}[SPA]");
                    });
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(TabLeftRel8);
                            columns.RelativeColumn();
                        });
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportTaxoStatus).SemiBold();
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
                        table.Cell().Text(Strings.StringsRes.ReportCurrentStand);
                        table.Cell().Text($"{Model.Valid} {Model.ValidYear}");
                    });
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(TabLeftRel8);
                            columns.RelativeColumn();
                        });
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportDataQualiIndicator).SemiBold();
                    });
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(TabLeftRel8);
                            columns.RelativeColumn();
                        });
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportRecordCrediRate);
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
                        table.Cell().Text(Strings.StringsRes.ReportRecordUpdate);
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
                        table.Cell().Text(Strings.StringsRes.ReportInfo);
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
                        table.Cell().Text(Strings.StringsRes.ReportMemo);
                        table.Cell().Text($"{Model.Memo}");
                    });
                });
            });
        }

        //private void ComposeHeaderTaxoHiera(IContainer container)
        //{
        //    CompHeaders.HeaderTaxoHiera(container, TabLeftRel4, TextHight);
        //}

        //private void ComposeTaxoHieraContent(IContainer container)
        //{
        //    container.Row(row =>
        //    {
        //        row.RelativeItem().Column(column =>
        //        {
        //            if (Model.DiseaseTree != null && !string.IsNullOrWhiteSpace(Model.DiseaseTree!.DiseaseName))
        //            {
        //                _tabLeftRel += 8;
        //                _tabRightRel -= 8;
        //                _tabLeftLastRel = _tabLeftRel;
        //                _tabRightLastRel = _tabRightRel;
        //                CompTaxoHieras.ComposeTaxoHierDisease(column, _tabLeftRel, _tabRightRel, Model.DiseaseTree);
        //            }

        //        });
        //        _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab"));
        //        _tabLeftRel = 0;
        //    });
        //}




        private void ComposeHeaderImages(IContainer container)
        {
            CompHeaders.HeaderImages(container, TabLeftRel4, TextHight);
        }


    }
}
