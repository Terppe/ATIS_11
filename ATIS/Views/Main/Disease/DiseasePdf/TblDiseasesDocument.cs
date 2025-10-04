using System.Configuration;
using System.Globalization;
using ATIS.Views.Report;
using ATIS.Views.Report.Pdf.Tbl75Speciesses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Main.Disease.DiseasePdf
{
    public class TblDiseasesDocument : IDocument
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

        public TblDiseasesDocument(TblDiseasesPdfModel model)
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
                        //text.Span($" {Model.Author}").FontSize(TextHight).SemiBold();
                        //text.Span($" {Model.AuthorYear}").FontSize(TextHight).SemiBold();
                    });
                });
            });

        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                //Taxonomy and Nomenclature
                column.Item().Element(ComposeHeaderTaxoNomen);
                column.Spacing(5);
                column.Item().Element(ComposeTaxoNomenContent);
                column.Spacing(5);

                ////Taxonomy Hierarchy
                //column.Item().Element(ComposeHeaderTaxoHiera);
                //column.Spacing(5);
                //column.Item().Element(ComposeTaxoHieraContent);
                //column.Spacing(5);

                ////Direct Children
                //column.Item().Element(ComposeHeaderChildren);
                //column.Spacing(5);
                //column.Item().Element(ComposeChildrenPhylumContent);
                //column.Spacing(5);
                //column.Item().Element(ComposeChildrenDivisionContent);
                //column.Spacing(5);

                ////Refernces
                //column.Item().Element(ComposeHeaderReferences);
                //column.Spacing(5);
                ////Refernces Experts
                //column.Item().Element(ComposeHeaderReferencesExperts);
                //column.Item().Element(ComposeReferencesExperts);
                //column.Spacing(5);
                ////Refernces Sources
                //column.Item().Element(ComposeHeaderReferencesOtherSources);
                //column.Item().Element(ComposeReferencesOtherSources);
                //column.Spacing(5);
                ////Refernces Authors
                //column.Item().Element(ComposeHeaderReferencesPublications);
                //column.Item().Element(ComposeReferencesPublications);
                //column.Spacing(5);
                //Comments
                //column.Item().Element(ComposeHeaderComments);
                //column.Item().Element(ComposeComments);
                //Images
                column.Item().Element(ComposeHeaderImages);
                column.Item().Element(ComposeImages);

            });
        }
        private void ComposeHeaderTaxoNomen(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    //column.Item().Text(text =>
                    //{
                    //    text.Span($"{Model.RegnumName}").FontSize(TextHight).SemiBold().Italic();
                    //    text.Span($" {Model.Subregnum}").FontSize(TextHight).SemiBold().Italic();
                    //    text.Span($" {Model.Author}").FontSize(TextHight).SemiBold();
                    //    text.Span($" {Model.AuthorYear}").FontSize(TextHight).SemiBold();
                    //});
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
                        table.Cell().Text(Strings.StringsRes.Regnum);
                        table.Cell().Text($@"{Model.DiseaseTree?.DiseaseName}");
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
                        table.Cell().Text($"{Model.Symptom}[ENG] {Model.Reason}[GER] {Model.Therapie}[FRA] {Model.Curability}[SPA]");
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

        //private void ComposeHeaderChildren(IContainer container)
        //{
        //    if (TblDiseasesDocumentDataSource.DiseasesCollection.Count == 0 && TblDiseasesDocumentDataSource.DiseasesCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    CompHeaders.HeaderChildren(container, _tabRightRel, TextHight);
        //}
        //private void ComposeChildrenPhylumContent(IContainer container)
        //{
        //    if (TblDiseasesDocumentDataSource.DiseasesCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(_tabLeftLastRel + 4);
        //            columns.ConstantColumn(_tabRightLastRel - 4);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.PhylumsCollection)
        //        {
        //            table.Cell().Text("");
        //            table.Cell().Text(Strings.StringsRes.Phylum);
        //            table.Cell().Text($"{item.PhylumName} {item.Author} {item.AuthorYear}");
        //        }
        //    });
        //}
        //private void ComposeChildrenDivisionContent(IContainer container)
        //{
        //    if (TblFoodsDocumentDataSource.DivisionsCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(_tabLeftLastRel + 4);
        //            columns.ConstantColumn(_tabRightLastRel - 4);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.DivisionsCollection)
        //        {
        //            table.Cell().Text("");
        //            table.Cell().Text(Strings.StringsRes.Division);
        //            table.Cell().Text($"{item.DivisionName} {item.Author} {item.AuthorYear}");
        //        }
        //    });
        //}


        //private void ComposeHeaderReferences(IContainer container)
        //{
        //    CompHeaders.HeaderHeaderReferences(container, TabLeftRel4, TextHight);
        //}

        //private void ComposeHeaderReferencesExperts(IContainer container)
        //{
        //    CompHeaders.HeaderReferencesExperts(container, TabLeftRel8);
        //}

        //private void ComposeReferencesExperts(IContainer container)
        //{
        //    if (TblDiseasesDocumentDataSource.ExpertsCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(TabLeftRel8);
        //            columns.ConstantColumn(TabRightRel8);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.ExpertsCollection)
        //        {
        //            for (var i = 0; i < 4; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportExpert);
        //                    table.Cell().Text(item.Tbl90RefExperts.RefExpertName);
        //                }
        //                if (i == 1)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportNotes);
        //                    table.Cell().Text(item.Tbl90RefExperts.Notes);
        //                }
        //                if (i == 2)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportRefFor);
        //                    table.Cell().Text(item.Info);
        //                }
        //                if (i == 3)
        //                {
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                }
        //            }
        //        }
        //    });
        //}

        //private void ComposeHeaderReferencesOtherSources(IContainer container)
        //{
        //    CompHeaders.HeaderReferencesOtherSources(container, TabLeftRel8);
        //}

        //private void ComposeReferencesOtherSources(IContainer container)
        //{
        //    if (TblDiseasesDocumentDataSource.SourcesCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(TabLeftRel8);
        //            columns.ConstantColumn(TabRightRel8);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.SourcesCollection)
        //        {
        //            for (var i = 0; i < 5; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportSource);
        //                    table.Cell().Text(item.Tbl90RefSources.RefSourceName);
        //                }
        //                if (i == 1)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportAcquired);
        //                    table.Cell().Text(item.Tbl90RefSources.SourceYear);
        //                }
        //                if (i == 2)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportNotes);
        //                    table.Cell().Text(item.Tbl90RefSources.Notes);
        //                }
        //                if (i == 3)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportRefFor);
        //                    table.Cell().Text(item.Info);
        //                }
        //                if (i == 4)
        //                {
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                }
        //            }
        //        }
        //    });
        //}

        //private void ComposeHeaderReferencesPublications(IContainer container)
        //{
        //    CompHeaders.HeaderReferencesPublications(container, TabLeftRel8);
        //}

        //private void ComposeReferencesPublications(IContainer container)
        //{
        //    if (TblDiseasesDocumentDataSource.AuthorsCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(TabLeftRel8);
        //            columns.ConstantColumn(TabRightRel8);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.AuthorsCollection)
        //        {
        //            for (var i = 0; i < 11; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportAuthor);
        //                    table.Cell().Text(item.Tbl90RefAuthors.RefAuthorName);
        //                }
        //                if (i == 1)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportPublicationDate);
        //                    table.Cell().Text(item.Tbl90RefAuthors.PublicationYear);
        //                }
        //                if (i == 2)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportArticleTitle);
        //                    table.Cell().Text(item.Tbl90RefAuthors.ArticelTitle);
        //                }
        //                if (i == 3)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportBookName);
        //                    table.Cell().Text(item.Tbl90RefAuthors.BookName);
        //                }
        //                if (i == 4)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportPages);
        //                    table.Cell().Text(item.Tbl90RefAuthors.Page1);
        //                }
        //                if (i == 5)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportPublisher);
        //                    table.Cell().Text(item.Tbl90RefAuthors.Publisher);
        //                }
        //                if (i == 6)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportPublicationPlace);
        //                    table.Cell().Text(item.Tbl90RefAuthors.PublicationPlace);
        //                }
        //                if (i == 7)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportIsbn);
        //                    table.Cell().Text(item.Tbl90RefAuthors.ISBN);
        //                }
        //                if (i == 8)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportNotes);
        //                    table.Cell().Text(item.Tbl90RefAuthors.Notes);
        //                }
        //                if (i == 9)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportRefFor);
        //                    table.Cell().Text(item.Info);
        //                }
        //                if (i == 10)
        //                {
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                }
        //            }
        //        }
        //    });
        //}


        //private void ComposeHeaderComments(IContainer container)
        //{
        //    CompHeaders.HeaderComments(container, TabLeftRel4, TextHight);
        //}

        //private void ComposeComments(IContainer container)
        //{
        //    if (TblFoodsDocumentDataSource.CommentsCollection.Count == 0)
        //    {
        //        return;
        //    }

        //    container.Table(table =>
        //    {
        //        table.ColumnsDefinition(columns =>
        //        {
        //            columns.ConstantColumn(TabLeftRel8);
        //            columns.ConstantColumn(TabRightRel8);
        //            columns.RelativeColumn();
        //        });

        //        foreach (var item in TblFoodsDocumentDataSource.CommentsCollection)
        //        {
        //            for (var i = 0; i < 3; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportInfo);
        //                    table.Cell().Text(item.Info);
        //                }
        //                if (i == 1)
        //                {
        //                    table.Cell().Text("");
        //                    table.Cell().Text(Strings.StringsRes.ReportMemo);
        //                    table.Cell().Text(item.Memo);
        //                }
        //                if (i == 2)
        //                {
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                    table.Cell().Text(" ");
        //                }
        //            }
        //        }
        //    });
        //}
        private void ComposeHeaderImages(IContainer container)
        {
            CompHeaders.HeaderImages(container, TabLeftRel4, TextHight);
        }

        private void ComposeImages(IContainer container)
        {
            if (TblDiseasesDocumentDataSource.ImagesCollection.Count == 0)
            {
                return;
            }

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(TabLeftRel8);
                    columns.ConstantColumn(TabRightRel8);
                    columns.RelativeColumn();
                });

                foreach (var item in Tbl75SpeciessesDocumentDataSource.ImagesCollection)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        if (i == 0)
                        {
                            table.Cell().Text("");
                            table.Cell().Text("");
                            //       table.Cell().Text(Placeholders.LoremIpsum());
                            //       table.Cell().Image(Placeholders.Image(200, 100));
                            table.Cell().Image(item.Filestream!).FitArea();
                        }
                        if (i == 1)
                        {
                            table.Cell().Text("");
                            table.Cell().Text(Strings.StringsRes.ReportImageShot);
                            table.Cell().Text(item.ShotDate.ToString());
                        }
                        if (i == 2)
                        {
                            table.Cell().Text("");
                            table.Cell().Text(Strings.StringsRes.ReportImageMimeType);
                            table.Cell().Text(item.ImageMimeType);
                        }
                        if (i == 3)
                        {
                            table.Cell().Text("");
                            table.Cell().Text(Strings.StringsRes.ReportInfo);
                            table.Cell().Text(item.Info);
                        }
                        if (i == 4)
                        {
                            table.Cell().Text("");
                            table.Cell().Text(Strings.StringsRes.ReportNotes);
                            table.Cell().Text(item.Memo);
                        }
                        if (i == 5)
                        {
                            table.Cell().Text(" ");
                            table.Cell().Text(" ");
                            table.Cell().Text(" ");
                        }
                    }
                }
            });

        }

    }
}
