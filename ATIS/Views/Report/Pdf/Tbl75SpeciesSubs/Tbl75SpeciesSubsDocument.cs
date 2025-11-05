using System.Configuration;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ATIS.Views.Report.Pdf.Tbl75SpeciesSubs;
internal class Tbl75SpeciesSubsDocument : IDocument
{
    private Tbl75SpeciesSubsPdfModel Model
    {
        get;
    }

    private static int _tabLeftRel;
    private static int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
    private const int TabLeftRel4 = 4;
    private const int TabLeftRel8 = 8;
    private static readonly int TabRightRel8 = _tabRightRel - 8;
    private const int TextHight = 14;

    public Tbl75SpeciesSubsDocument(Tbl75SpeciesSubsPdfModel model)
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
                page.MarginRight(20);
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
                    text.Span($"                                  ");
                    text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();

                });
                column.Item().Text(text =>
                {
                    text.Span($"{Model.GenusTree?.GenusName} {Model.SpeciesTree?.SpeciesName} {Model.SpeciesTree?.Subspecies}" +
                              $"{Model.SpeciesTree?.Divers}").FontSize(TextHight).SemiBold().Italic();
                    text.Span($" {Model.SpeciesTree?.Author} {Model.SpeciesTree?.AuthorYear}").FontSize(TextHight).SemiBold();
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

            //Direct Synonym
            column.Item().Element(ComposeHeaderSynonym);
            column.Item().Element(ComposeSynonymContent);
            //Direct Name
            column.Item().Element(ComposeHeaderName);
            column.Item().Element(ComposeNameContent);

            //Rest Taxonomen
            column.Item().Element(ComposeTaxoNomenContentRest);
            column.Spacing(5);

            //Taxonomy Hierarchy
            column.Item().Element(ComposeHeaderTaxoHiera);
            column.Spacing(5);
            column.Item().Element(ComposeTaxoHieraContent);
            column.Spacing(5);

            //Refernces
            column.Item().Element(ComposeHeaderReferences);
            column.Spacing(5);
            //Refernces Experts
            column.Item().Element(ComposeHeaderReferencesExperts);
            column.Item().Element(ComposeReferencesExperts);
            column.Spacing(5);
            //Refernces Sources
            column.Item().Element(ComposeHeaderReferencesOtherSources);
            column.Item().Element(ComposeReferencesOtherSources);
            column.Spacing(5);
            //Refernces Authors
            column.Item().Element(ComposeHeaderReferencesPublications);
            column.Item().Element(ComposeReferencesPublications);
            column.Spacing(5);
            //Geographics
            column.Item().Element(ComposeHeaderGeographics);
            column.Item().Element(ComposeGeographics);
            //Images
            column.Item().Element(ComposeHeaderImages);
            column.Item().Element(ComposeImages);
            //Comments
            column.Item().Element(ComposeHeaderComments);
            column.Item().Element(ComposeComments);
            //usefull information
            column.Item().Element(ComposeHeaderUsefulInfos);
            column.Item().Element(ComposeUsefulInfos);
            column.Item().Element(ComposePlantUsefulInfos);
        });
    }

    private void ComposeHeaderSynonym(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.SynonymsCollection.Count == 0)
        {
        }
        else
        {
            CompHeaders.HeaderSynonym(container, TabLeftRel8, TabRightRel8);
        }
    }

    private void ComposeSynonymContent(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.SynonymsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.SynonymsCollection)
            {
                table.Cell().Text("");
                table.Cell().Text("");
                table.Cell().Text($@"{item.SynonymName} {item.Author} {item.AuthorYear}");
            }
        });
    }

    private void ComposeHeaderName(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.NamesCollection.Count == 0)
        {
        }
        else
        {
            CompHeaders.HeaderName(container, TabLeftRel8, TabRightRel8);
        }
    }
    private void ComposeNameContent(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.NamesCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.NamesCollection)
            {
                table.Cell().Text("");
                table.Cell().Text("");
                table.Cell().Text($@"{item.NameName} [{item.Language}]");
            }
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
                //    text.Span("ATIS  ").FontSize(24).SemiBold();
                //    text.Span(Strings.StringsRes.AtisHeader).FontSize(TextHight).SemiBold();
                //    text.Span("        ");
                //    text.Span(DateTime.Now.ToString(CultureInfo.InvariantCulture));

                //});
                //column.Item().Text(text =>
                //{
                //    text.Span($"{Model.GenusTree?.GenusName} {Model.FiSpeciesTree?.FiSpeciesName} {Model.FiSpeciesTree?.Subspecies}" +
                //              $"{Model.FiSpeciesTree?.Divers}").FontSize(TextHight).SemiBold().Italic();
                //    text.Span($" {Model.FiSpeciesTree?.Author} {Model.FiSpeciesTree?.AuthorYear}").FontSize(TextHight).SemiBold();
                //});
                column.Item().Text(text =>
                {
                    text.Span(Strings.StringsRes.ReportTaxonomicId);
                    text.Span($" {Model.CountId}");
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
                    table.Cell().Text($@"{Model.RegnumTree?.RegnumName} {Model.RegnumTree?.Subregnum}");
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
                    table.Cell().Text(Strings.StringsRes.ReportTaxoRank).FontSize(TextHight).SemiBold(); 
                    table.Cell().Text(Strings.StringsRes.FiSpecies);
                });

            });
        });
    }

    private void ComposeTaxoNomenContentRest(IContainer container)
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
                    table.Cell().Text(Strings.StringsRes.ReportTradeName);
                    table.Cell().Text($"{Model.TradeName}");
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
                    table.Cell().Text($"{Model.MemoSpecies}");
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
                    table.Cell().Text(Strings.StringsRes.ReportImporterWithYear);
                    table.Cell().Text($"{Model.Importer} {Model.ImportingYear}");
                });
                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Strings.StringsRes.ReportTypeSpecies).SemiBold();
                //});
                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.ConstantColumn(TabRightRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Strings.StringsRes.ReportCurrentStand);
                //    table.Cell().Text($"{Model.TypeSpecies}");
                //});

                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.ConstantColumn(TabRightRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Strings.StringsRes.ReportBasinLength);
                //    table.Cell().Text($"{Model.BasinLength}");
                //});
                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.ConstantColumn(TabRightRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Strings.StringsRes.ReportFishLength);
                //    table.Cell().Text($"{Model.FishLength}");
                //});
                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.ConstantColumn(TabRightRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Strings.StringsRes.ReportNotes);
                //    table.Cell().Text($"{Model.MemoRegion}");
                //});

            });
        });
    }

    private void ComposeHeaderTaxoHiera(IContainer container)
    {
        CompHeaders.HeaderTaxoHiera(container, TabLeftRel4, TextHight);
    }
    private void ComposeTaxoHieraContent(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                if (Model.RegnumTree != null && !string.IsNullOrWhiteSpace(Model.RegnumTree!.RegnumName))
                {
                    _tabLeftRel += 8;
                    _tabRightRel -= 8;
                    CompTaxoHieras.ComposeTaxoHierRegnum(column, _tabLeftRel, _tabRightRel, Model.RegnumTree);
                }

                if (Model.PhylumTree != null && !string.IsNullOrWhiteSpace(Model.PhylumTree!.PhylumName))
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierPhylum(column, _tabLeftRel, _tabRightRel, Model.PhylumTree);
                }

                //if (Model.DivisionTree != null && !string.IsNullOrWhiteSpace(Model.DivisionTree!.DivisionName))
                //{
                //    _tabLeftRel += 4;
                //    _tabRightRel -= 4;
                //    CompTaxoHieras.ComposeTaxoHierDivision(column, _tabLeftRel, _tabRightRel, Model.DivisionTree);
                //}

                if (Model.SubphylumTree != null && !string.IsNullOrWhiteSpace(Model.SubphylumTree!.SubphylumName))
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubphylum(column, _tabLeftRel, _tabRightRel, Model.SubphylumTree);
                }
                //if (Model.SubdivisionTree != null && !string.IsNullOrWhiteSpace(Model.SubdivisionTree!.SubdivisionName))
                //{
                //    _tabLeftRel += 4;
                //    _tabRightRel -= 4;
                //    CompTaxoHieras.ComposeTaxoHierSubdivision(column, _tabLeftRel, _tabRightRel, Model.SubdivisionTree);
                //}
                if (Model.SuperclassTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSuperclass(column, _tabLeftRel, _tabRightRel, Model.SuperclassTree);
                }
                if (Model.ClassTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierClass(column, _tabLeftRel, _tabRightRel, Model.ClassTree);
                }
                if (Model.SubclassTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubclass(column, _tabLeftRel, _tabRightRel, Model.SubclassTree);
                }
                if (Model.InfraclassTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierInfraclass(column, _tabLeftRel, _tabRightRel, Model.InfraclassTree);
                }
                if (Model.LegioTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierLegio(column, _tabLeftRel, _tabRightRel, Model.LegioTree);
                }
                if (Model.OrdoTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierOrdo(column, _tabLeftRel, _tabRightRel, Model.OrdoTree);
                }
                if (Model.SubordoTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubordo(column, _tabLeftRel, _tabRightRel, Model.SubordoTree);
                }
                if (Model.InfraordoTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierInfraordo(column, _tabLeftRel, _tabRightRel, Model.InfraordoTree);
                }
                if (Model.SuperfamilyTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSuperfamily(column, _tabLeftRel, _tabRightRel, Model.SuperfamilyTree);
                }
                if (Model.FamilyTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierFamily(column, _tabLeftRel, _tabRightRel, Model.FamilyTree);
                }
                if (Model.SubfamilyTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubfamily(column, _tabLeftRel, _tabRightRel, Model.SubfamilyTree);
                }
                if (Model.InfrafamilyTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierInfrafamily(column, _tabLeftRel, _tabRightRel, Model.InfrafamilyTree);
                }
                if (Model.SupertribusTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSupertribus(column, _tabLeftRel, _tabRightRel, Model.SupertribusTree);
                }
                if (Model.TribusTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierTribus(column, _tabLeftRel, _tabRightRel, Model.TribusTree);
                }
                if (Model.SubtribusTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubtribus(column, _tabLeftRel, _tabRightRel, Model.SubtribusTree);
                }
                if (Model.InfratribusTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierInfratribus(column, _tabLeftRel, _tabRightRel, Model.InfratribusTree);
                }
                if (Model.GenusTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;

                    CompTaxoHieras.ComposeTaxoHierGenus(column, _tabLeftRel, _tabRightRel, Model.GenusTree);
                }
                if (Model.SpeciesTree != null)
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;

                    CompTaxoHieras.ComposeTaxoHierSpecies(column, _tabLeftRel, _tabRightRel, Model.SpeciesTree);
                }
            });
            _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab"));
            _tabLeftRel = 0;

        });
    }

    //private void ComposeHeaderChildren(IContainer container)
    //{
    //    //   if (Tbl69FiSpeciessesDocumentDataSource.FiSpeciessesCollection.Count == 0)
    //    if (string.IsNullOrEmpty(Model.FiSpeciesTree?.Subspecies))
    //    {
    //        return;
    //    }

    //    CompHeaders.HeaderChildren(container, _tabRightRel, TextHight);
    //}

    //private void ComposeChildrenContent(IContainer container)
    //{
    //    if (string.IsNullOrEmpty(Model.FiSpeciesTree?.Subspecies))
    //    {
    //        return;
    //    }
    //    //subspecies wird nicht gefunden
    //    container.Table(table =>
    //    {
    //        foreach (var item in Tbl69FiSpeciessesDocumentDataSource.FiSpeciessesCollection)
    //        {
    //            table.ColumnsDefinition(columns =>
    //            {
    //                columns.ConstantColumn(_tabLeftLastRel + 4);
    //                columns.ConstantColumn(_tabRightLastRel - 4);
    //                columns.RelativeColumn();
    //            });
    //            table.Cell().Text("");
    //            table.Cell().Text(Strings.StringsRes.Subspecies);
    //            table.Cell().Text($"{item.Tbl66Genusses.GenusName} {item.FiSpeciesName}" +
    //                              $" {item.Subspecies} {item.Divers} {item.Author} {item.AuthorYear}").WrapAnywhere();

    //        }
    //    });
    //}

    private void ComposeHeaderReferences(IContainer container)
    {
        CompHeaders.HeaderHeaderReferences(container, TabLeftRel4, TextHight);
    }

    private void ComposeHeaderReferencesExperts(IContainer container)
    {
        CompHeaders.HeaderReferencesExperts(container, TabLeftRel8);
    }

    private void ComposeReferencesExperts(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.ExpertsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.ExpertsCollection)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportExpert);
                        table.Cell().Text(item.Tbl90RefExperts.RefExpertName);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportNotes);
                        table.Cell().Text(item.Tbl90RefExperts.Notes);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportRefFor);
                        table.Cell().Text(item.Info);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                    }
                }
            }
        });
    }

    private void ComposeHeaderReferencesOtherSources(IContainer container)
    {
        CompHeaders.HeaderReferencesOtherSources(container, TabLeftRel8);
    }

    private void ComposeReferencesOtherSources(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.SourcesCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.SourcesCollection)
            {
                for (var i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportSource);
                        table.Cell().Text(item.Tbl90RefSources.RefSourceName);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportAcquired);
                        table.Cell().Text(item.Tbl90RefSources.SourceYear);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportNotes);
                        table.Cell().Text(item.Tbl90RefSources.Notes);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportRefFor);
                        table.Cell().Text(item.Info);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                    }
                }
            }
        });
    }

    private void ComposeHeaderReferencesPublications(IContainer container)
    {
        CompHeaders.HeaderReferencesPublications(container, TabLeftRel8);
    }

    private void ComposeReferencesPublications(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.AuthorsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.AuthorsCollection)
            {
                for (var i = 0; i < 11; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportAuthor);
                        table.Cell().Text(item.Tbl90RefAuthors.RefAuthorName);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportPublicationDate);
                        table.Cell().Text(item.Tbl90RefAuthors.PublicationYear);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportArticleTitle);
                        table.Cell().Text(item.Tbl90RefAuthors.ArticelTitle);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportBookName);
                        table.Cell().Text(item.Tbl90RefAuthors.BookName);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportPages);
                        table.Cell().Text(item.Tbl90RefAuthors.Page1);
                    }
                    if (i == 5)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportPublisher);
                        table.Cell().Text(item.Tbl90RefAuthors.Publisher);
                    }
                    if (i == 6)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportPublicationPlace);
                        table.Cell().Text(item.Tbl90RefAuthors.PublicationPlace);
                    }
                    if (i == 7)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportIsbn);
                        table.Cell().Text(item.Tbl90RefAuthors.ISBN);
                    }
                    if (i == 8)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportNotes);
                        table.Cell().Text(item.Tbl90RefAuthors.Notes);
                    }
                    if (i == 9)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportRefFor);
                        table.Cell().Text(item.Info);
                    }
                    if (i == 10)
                    {
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                    }
                }
            }
        });
    }

    private void ComposeHeaderGeographics(IContainer container)
    {
        CompHeaders.HeaderGeographics(container, TabLeftRel4, TextHight);
    }
    private void ComposeGeographics(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.GeographicsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.GeographicsCollection)
            {
                for (var i = 0; i < 10; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportAddress);
                        table.Cell().Text(item.Address);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportContinent);
                        table.Cell().Text(item.Continent);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportCountry);
                        table.Cell().Text(item.Country);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportHttp);
                        table.Cell().Text(item.Http);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportLatitudeLongitude);
                        table.Cell().Text($"{item.Latitude}  //  {item.Longitude}");
                    }
                    if (i == 5)
                    {
                        table.Cell().Text("");
                        table.Cell().Text("");
                        table.Cell().Text($"{item.Latitude1}  //  {item.Longitude1}");
                    }
                    if (i == 6)
                    {
                        table.Cell().Text("");
                        table.Cell().Text("");
                        table.Cell().Text($"{item.Latitude2}  //  {item.Longitude2}");
                    }
                    if (i == 7)
                    {
                        table.Cell().Text("");
                        table.Cell().Text("");
                        table.Cell().Text($"{item.Latitude3}  //  {item.Longitude3}");
                    }
                    if (i == 8)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportMemo);
                        table.Cell().Text(item.Memo);
                    }
                    if (i == 9)
                    {
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                    }
                }
            }
        });
    }

    private void ComposeHeaderImages(IContainer container)
    {
        CompHeaders.HeaderImages(container, TabLeftRel4, TextHight);
    }

    private void ComposeImages(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.ImagesCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.ImagesCollection)
            {
                for (var i = 0; i < 6; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text("");
                        //       table.Cell().Text(Placeholders.LoremIpsum());
                        //       table.Cell().Image(Placeholders.Image(200, 100));
                        table.Cell().Image(item.Filestream).FitArea();
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

    private void ComposeHeaderComments(IContainer container)
    {
        CompHeaders.HeaderComments(container, TabLeftRel4, TextHight);
    }
    private void ComposeComments(IContainer container)
    {
        if (Tbl75SpeciesSubsDocumentDataSource.CommentsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciesSubsDocumentDataSource.CommentsCollection)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportInfo);
                        table.Cell().Text(item.Info);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Strings.StringsRes.ReportMemo);
                        table.Cell().Text(item.Memo);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                        table.Cell().Text(" ");
                    }
                }
            }
        });
    }
    private void ComposeHeaderUsefulInfos(IContainer container)
    {
        CompHeaders.HeaderUsefulInfos(container, TabLeftRel4, TextHight);
    }

    private void ComposeUsefulInfos(IContainer container)
    {
        if (Model.Creature == "Plantae")
        {
            return;
        }

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportTechnic).SemiBold();
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
                    table.Cell().Text(Strings.StringsRes.ReportLNumberLOrigin);
                    table.Cell().Text($"{Model.LNumber} / {Model.LOrigin}");
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
                    table.Cell().Text(Strings.StringsRes.ReportLdaNumberLdaOrigin);
                    table.Cell().Text($"{Model.LdaNumber} / {Model.LdaOrigin}");
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
                    table.Cell().Text(Strings.StringsRes.ReportBasinLength);
                    table.Cell().Text($"{Model.BasinLength}");
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
                    table.Cell().Text(Strings.StringsRes.ReportFishLength);
                    table.Cell().Text($"{Model.FishLength}");
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
                    table.Cell().Text(Strings.StringsRes.ReportKarnivore);
                    table.Cell().Text($"{Model.Karnivore}");
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
                    table.Cell().Text(Strings.StringsRes.ReportHerbivore);
                    table.Cell().Text($"{Model.Herbivore}");
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
                    table.Cell().Text(Strings.StringsRes.ReportLimnivore);
                    table.Cell().Text($"{Model.Limnivore}");
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
                    table.Cell().Text(Strings.StringsRes.ReportOmnivore);
                    table.Cell().Text($"{Model.Omnivore}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoFood).SemiBold();
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
                    table.Cell().Text(Strings.StringsRes.ReportMemo).SemiBold();
                    table.Cell().Text($"{Model.MemoFoods}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult1);
                    table.Cell().Text($"{Model.Difficult1}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult2);
                    table.Cell().Text($"{Model.Difficult2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult3);
                    table.Cell().Text($"{Model.Difficult3}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult4);
                    table.Cell().Text($"{Model.Difficult4}");
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
                    table.Cell().Text(Strings.StringsRes.ReportRegionTop);
                    table.Cell().Text($"{Model.RegionTop}");
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
                    table.Cell().Text(Strings.StringsRes.ReportRegionMiddle);
                    table.Cell().Text($"{Model.RegionMiddle}");
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
                    table.Cell().Text(Strings.StringsRes.ReportRegionBottom);
                    table.Cell().Text($"{Model.RegionBottom}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoRegion).SemiBold();
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
                    table.Cell().Text(Strings.StringsRes.ReportMemo).SemiBold();
                    table.Cell().Text($"{Model.MemoRegion}");
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
                    table.Cell().Text(Strings.StringsRes.ReportPh12);
                    table.Cell().Text($"{Model.Ph1}/{Model.Ph2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportTemp12);
                    table.Cell().Text($"{Model.Temp1}/{Model.Temp2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportHardness12);
                    table.Cell().Text($"{Model.Hardness1}/{Model.Hardness2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportPh12);
                    table.Cell().Text($"{Model.Ph1}/{Model.Ph2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportTemp12);
                    table.Cell().Text($"{Model.Temp1}/{Model.Temp2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportHardness12);
                    table.Cell().Text($"{Model.Hardness1}/{Model.Hardness2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportCarboHardness12);
                    table.Cell().Text($"{Model.CarboHardness1}/{Model.CarboHardness2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportTypeSpecies);
                    table.Cell().Text($"{Model.TypeSpecies}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoTechnic).SemiBold();
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
                    table.Cell().Text($"{Model.MemoTech}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoHusbandry).SemiBold();
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
                    table.Cell().Text($"{Model.MemoHusbandry}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoColor).SemiBold();
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
                    table.Cell().Text($"{Model.MemoColor}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoBreeding).SemiBold();
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
                    table.Cell().Text($"{Model.MemoBreeding}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoBuilt).SemiBold();
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
                    table.Cell().Text($"{Model.MemoBuilt}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoSozial).SemiBold();
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
                    table.Cell().Text($"{Model.MemoSozial}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoDimorphism).SemiBold();
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
                    table.Cell().Text($"{Model.MemoDomorphism}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoSpecial).SemiBold();
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
                    table.Cell().Text($"{Model.MemoSpecial}");
                });
            });
        });
    }

    private void ComposePlantUsefulInfos(IContainer container)
    {
        if (Model.Creature != "Plantae")
        {
            return;
        }

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportTechnic).SemiBold();
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
                    table.Cell().Text(Strings.StringsRes.ReportBasinHeight);
                    table.Cell().Text($"{Model.BasinHeight}");
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
                    table.Cell().Text(Strings.StringsRes.ReportPlantLength);
                    table.Cell().Text($"{Model.PlantLength}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult1);
                    table.Cell().Text($"{Model.Difficult1}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult2);
                    table.Cell().Text($"{Model.Difficult2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult3);
                    table.Cell().Text($"{Model.Difficult3}");
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
                    table.Cell().Text(Strings.StringsRes.ReportDifficult4);
                    table.Cell().Text($"{Model.Difficult4}");
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
                    table.Cell().Text(Strings.StringsRes.ReportPh12);
                    table.Cell().Text($"{Model.Ph1}/{Model.Ph2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportTemp12);
                    table.Cell().Text($"{Model.Temp1}/{Model.Temp2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportHardness12);
                    table.Cell().Text($"{Model.Hardness1}/{Model.Hardness2}");
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
                    table.Cell().Text(Strings.StringsRes.ReportCarboHardness12);
                    table.Cell().Text($"{Model.CarboHardness1}/{Model.CarboHardness2}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoReproduction).SemiBold();
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
                    table.Cell().Text($"{Model.MemoReproduction}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoCulture).SemiBold();
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
                    table.Cell().Text($"{Model.MemoCulture}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoBuilt).SemiBold();
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
                    table.Cell().Text($"{Model.MemoBuilt}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoColor).SemiBold();
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
                    table.Cell().Text($"{Model.MemoColor}");
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Strings.StringsRes.ReportMemoGlobal).SemiBold();
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
                    table.Cell().Text($"{Model.MemoGlobal}");
                });
            });
        });
    }

}
