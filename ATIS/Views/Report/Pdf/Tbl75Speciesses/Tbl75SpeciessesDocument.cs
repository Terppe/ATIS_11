using System.Configuration;
using System.Globalization;
using ATIS.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinUI3Localizer;
using Image = QuestPDF.Infrastructure.Image;

namespace ATIS.Views.Report.Pdf.Tbl75Speciesses;
public class Tbl75SpeciessesDocument : IDocument
{
    private Tbl75SpeciessesPdfModel Model { get; }
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    private static int _tabLeftRel;
    private static int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
    private const int TabLeftRel4 = 4;
    private const int TabLeftRel8 = 8;
    private static int _tabLeftLastRel;
    private static int _tabRightLastRel;
    private static readonly int TabRightRel8 = _tabRightRel - 8;
    private const int TextHight = 12;

    public Tbl75SpeciessesDocument(Tbl75SpeciessesPdfModel model)
    {
        Model = model;
    }
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
                    text.Span($"{Model.GenusTree?.GenusName ?? string.Empty} {Model.SpeciesTree?.SpeciesName ?? string.Empty} {Model.SpeciesTree?.Subspecies ?? string.Empty}" +
                              $"{Model.SpeciesTree?.Divers ?? string.Empty}").FontSize(TextHight).SemiBold().Italic();
                    text.Span($" {Model.Author ?? string.Empty}").FontSize(TextHight).SemiBold();
                    text.Span($" {Model.AuthorYear ?? string.Empty}").FontSize(TextHight).SemiBold();
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

            //Direct Children
            column.Item().Element(ComposeHeaderChildren);
            column.Spacing(5);
            column.Item().Element(ComposeChildrenContent);
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
        if (Tbl75SpeciessesDocumentDataSource.SynonymsCollection.Count == 0)
        {
        }
        else
        {
            CompHeaders.HeaderSynonym(container, TabLeftRel8, TabRightRel8);
        }
    }

    private void ComposeSynonymContent(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.SynonymsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciessesDocumentDataSource.SynonymsCollection)
            {
                table.Cell().Text("");
                table.Cell().Text("");
                table.Cell().Text($@"{item.SynonymName} {item.Author} {item.AuthorYear}");
            }
        });
    }

    private void ComposeHeaderName(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.NamesCollection.Count == 0)
        {
        }
        else
        {
            CompHeaders.HeaderName(container, TabLeftRel8, TabRightRel8);
        }
    }
    private void ComposeNameContent(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.NamesCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciessesDocumentDataSource.NamesCollection)
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
                column.Item().Text(text =>
                {
                    text.Span(Loc("TaxonomicId"));
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
                    table.Cell().Text(Loc("TaxonomyAndNomenclatura")).FontSize(TextHight).SemiBold();
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
                    table.Cell().Text(Loc("Regnum"));
                    table.Cell().Text($@"{Model.RegnumTree?.RegnumName ?? string.Empty} {Model.RegnumTree?.Subregnum ?? string.Empty}");
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
                    table.Cell().Text(Loc("TaxonomicRank"));
                    table.Cell().Text(Loc("Species"));
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
                    table.Cell().Text(Loc("TaxonomicStatus")).SemiBold();
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
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("DataQualityIndicator")).SemiBold();
                });
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("RecordCredibilityRating"));
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
                    table.Cell().Text(Loc("TradeName"));
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
                    table.Cell().Text(Loc("MemoSpecies"));
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
                    table.Cell().Text(Loc("ImporterWithYear"));
                    table.Cell().Text($"{Model.Importer} {Model.ImportingYear}");
                });

            });
        });
    }

    private void ComposeHeaderTaxoHiera(IContainer container)
    {
        CompHeaders.HeaderTaxoHiera(container, TabLeftRel4, TextHight);
    }
    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    private void ComposeTaxoHieraContent(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                var visibleIndex = 0;
                var reportPdfRightTab = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab"));

                void ComposeVisibleTaxon(string? name, Action<int, int> composeAction, bool rememberAsLast = false)
                {
                    if (!IsVisibleTaxonName(name))
                        return;

                    var left = 8 + (visibleIndex++ * 4);
                    var right = Math.Max(0, reportPdfRightTab - left);

                    if (rememberAsLast)
                    {
                        _tabLeftLastRel = left;
                        _tabRightLastRel = right;
                    }

                    composeAction(left, right);
                }

                ComposeVisibleTaxon(
                    Model.RegnumTree?.RegnumName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierRegnum(column, left, right, Model.RegnumTree!));

                ComposeVisibleTaxon(
                    Model.PhylumTree?.PhylumName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierPhylum(column, left, right, Model.PhylumTree!));

                ComposeVisibleTaxon(
                    Model.SubphylumTree?.SubphylumName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSubphylum(column, left, right, Model.SubphylumTree!));

                ComposeVisibleTaxon(
                    Model.SuperclassTree?.SuperclassName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSuperclass(column, left, right, Model.SuperclassTree!));

                ComposeVisibleTaxon(
                    Model.ClassTree?.ClassName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierClass(column, left, right, Model.ClassTree!));

                ComposeVisibleTaxon(
                    Model.SubclassTree?.SubclassName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSubclass(column, left, right, Model.SubclassTree!));

                ComposeVisibleTaxon(
                    Model.InfraclassTree?.InfraclassName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierInfraclass(column, left, right, Model.InfraclassTree!));

                ComposeVisibleTaxon(
                    Model.LegioTree?.LegioName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierLegio(column, left, right, Model.LegioTree!));

                ComposeVisibleTaxon(
                    Model.OrdoTree?.OrdoName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierOrdo(column, left, right, Model.OrdoTree!));

                ComposeVisibleTaxon(
                    Model.SubordoTree?.SubordoName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSubordo(column, left, right, Model.SubordoTree!));

                ComposeVisibleTaxon(
                    Model.InfraordoTree?.InfraordoName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierInfraordo(column, left, right, Model.InfraordoTree!));

                ComposeVisibleTaxon(
                    Model.SuperfamilyTree?.SuperfamilyName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSuperfamily(column, left, right, Model.SuperfamilyTree!));

                ComposeVisibleTaxon(
                    Model.FamilyTree?.FamilyName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierFamily(column, left, right, Model.FamilyTree!));

                ComposeVisibleTaxon(
                    Model.SubfamilyTree?.SubfamilyName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSubfamily(column, left, right, Model.SubfamilyTree!));

                ComposeVisibleTaxon(
                    Model.InfrafamilyTree?.InfrafamilyName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierInfrafamily(column, left, right, Model.InfrafamilyTree!));

                ComposeVisibleTaxon(
                    Model.SupertribusTree?.SupertribusName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSupertribus(column, left, right, Model.SupertribusTree!));

                ComposeVisibleTaxon(
                    Model.TribusTree?.TribusName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierTribus(column, left, right, Model.TribusTree!));

                ComposeVisibleTaxon(
                    Model.SubtribusTree?.SubtribusName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSubtribus(column, left, right, Model.SubtribusTree!));

                ComposeVisibleTaxon(
                    Model.InfratribusTree?.InfratribusName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierInfratribus(column, left, right, Model.InfratribusTree!));

                ComposeVisibleTaxon(
                    Model.GenusTree?.GenusName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierGenus(column, left, right, Model.GenusTree!));

                ComposeVisibleTaxon(
                    Model.SpeciesTree?.SpeciesName,
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSpecies(column, left, right, Model.SpeciesTree!),
                    rememberAsLast: true);

            });
        });
    }

    private void ComposeHeaderChildren(IContainer container)
    {
        //   if (Tbl75SpeciessesDocumentDataSource.SpeciessesCollection.Count == 0)
        if (!string.IsNullOrEmpty(Model.SpeciesTree?.Subspecies))
        {
            return;
        }

        CompHeaders.HeaderChildren(container, _tabRightRel, TextHight);
    }
    private void ComposeChildrenContent(IContainer container)
    {
        var visibleItems = Tbl75SpeciessesDocumentDataSource.SpeciesSubsCollection
            .Where(x => IsVisibleTaxonName(x.Subspecies))
            .ToList();

        if (visibleItems.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(_tabLeftLastRel + 4);
                columns.ConstantColumn(_tabRightLastRel - 4);
                columns.RelativeColumn();
            });

            foreach (var item in visibleItems)
            {
                table.Cell().Text("");
                table.Cell().Text(Loc("Subspecies"));
  //              table.Cell().Text($"{item.Tbl66Genusses.GenusName ?? string.Empty}{item.SpeciesName ?? string.Empty} {item.Subspecies ?? string.Empty} {item.Divers ?? string.Empty}");
            }
        });
    }

    //private void ComposeChildrenContent(IContainer container)
    //{
    //    if (!string.IsNullOrEmpty(Model.SpeciesTree?.Subspecies))
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

    //        foreach (var item in Tbl75SpeciessesDocumentDataSource.SpeciesSubsCollection)
    //        {
    //            table.Cell().Text("");
    //            table.Cell().Text(Strings.StringsRes.Subspecies);
    //            table.Cell().Text($"{item.Genus.GenusName} {item.SpeciesName}" +
    //                              $" {item.Subspecies} {item.Divers} {item.Author} {item.AuthorYear}");
    //            //     table.Cell().Text($"{item.Tbl66Genusses.GenusName} {item.FiSpeciesName} {item.Subspecies} {item.Divers} {item.Author} {item.AuthorYear}");

    //        }
    //    });
    //}

    private void ComposeHeaderReferences(IContainer container)
    {
        var hasExperts = Tbl75SpeciessesDocumentDataSource.ExpertsCollection?.Count > 0;
        var hasSources = Tbl75SpeciessesDocumentDataSource.SourcesCollection?.Count > 0;
        var hasAuthors = Tbl75SpeciessesDocumentDataSource.AuthorsCollection?.Count > 0;

        if (!hasExperts && !hasSources && !hasAuthors)
            return;

        CompHeaders.HeaderHeaderReferences(container, TabLeftRel4, TextHight);
    }

    private void ComposeHeaderReferencesExperts(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.ExpertsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesExperts(container, TabLeftRel8);
    }

    private void ComposeReferencesExperts(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.ExpertsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl75SpeciessesDocumentDataSource.ExpertsCollection)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Expert"));
                        table.Cell().Text(item.Tbl90RefExperts?.RefExpertName ?? string.Empty);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Notes"));
                        table.Cell().Text(item.Tbl90RefExperts?.Notes ?? string.Empty);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Referencefor"));
                        table.Cell().Text(item.Info ?? string.Empty);
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
        if (Tbl75SpeciessesDocumentDataSource.SourcesCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesOtherSources(container, TabLeftRel8);
    }

    private void ComposeReferencesOtherSources(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.SourcesCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl75SpeciessesDocumentDataSource.SourcesCollection)
            {
                for (var i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Source"));
                        table.Cell().Text(item.Tbl90RefSources?.RefSourceName ?? string.Empty);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Acquired"));
                        table.Cell().Text(item.Tbl90RefSources?.SourceYear ?? string.Empty);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Notes"));
                        table.Cell().Text(item.Tbl90RefSources?.Notes ?? string.Empty);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Referencefor"));
                        table.Cell().Text(item.Info ?? string.Empty);
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
        if (Tbl75SpeciessesDocumentDataSource.AuthorsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesPublications(container, TabLeftRel8);
    }

    private void ComposeReferencesPublications(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.AuthorsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl75SpeciessesDocumentDataSource.AuthorsCollection)
            {
                for (var i = 0; i < 11; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Author"));
                        table.Cell().Text(item.Tbl90RefAuthors?.RefAuthorName ?? string.Empty);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("PublicationDate"));
                        table.Cell().Text(item.Tbl90RefAuthors?.PublicationYear ?? string.Empty);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("ArticleTitle"));
                        table.Cell().Text(item.Tbl90RefAuthors?.ArticelTitle ?? string.Empty);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("BookName"));
                        table.Cell().Text(item.Tbl90RefAuthors?.BookName ?? string.Empty);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Pages"));
                        table.Cell().Text(item.Tbl90RefAuthors?.Page1 ?? string.Empty);
                    }
                    if (i == 5)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Publisher"));
                        table.Cell().Text(item.Tbl90RefAuthors?.Publisher ?? string.Empty);
                    }
                    if (i == 6)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("PublicationPlace"));
                        table.Cell().Text(item.Tbl90RefAuthors?.PublicationPlace ?? string.Empty);
                    }
                    if (i == 7)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Isbn"));
                        table.Cell().Text(item.Tbl90RefAuthors?.ISBN ?? string.Empty);
                    }
                    if (i == 8)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Notes"));
                        table.Cell().Text(item.Tbl90RefAuthors?.Notes ?? string.Empty);
                    }
                    if (i == 9)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Referencefor"));
                        table.Cell().Text(item.Info ?? string.Empty);
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
        if (Tbl75SpeciessesDocumentDataSource.GeographicsCollection.Count == 0)
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

            foreach (var item in Tbl75SpeciessesDocumentDataSource.GeographicsCollection)
            {

                for (var i = 0; i < 10; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Address"));
                        table.Cell().Text(item.Address);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Continent"));
                        table.Cell().Text(item.Continent);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Country"));
                        table.Cell().Text(item.Country);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Http"));
                        table.Cell().Text(item.Http);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("LatitudeLongitude"));
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
                        table.Cell().Text(Loc("Memo"));
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
        if (Tbl75SpeciessesDocumentDataSource.ImagesCollection.Count == 0)
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
                        if (item.Filestream != null)
                        {
                            try
                            {
                                using var ms = new MemoryStream(item.Filestream);
                                using var img = Image.FromStream(ms); // Test: ist es ein echtes Bild?

                                table.Cell().Image(item.Filestream).FitArea();
                            }
                            catch (Exception ex)
                            {
                                var loc = Localizer.Get();

                                var errorDialog = new ContentDialog
                                {
                                    Title = loc.GetLocalizedString("PdfImage_Error_Title"),
                                    Content = string.Format(loc.GetLocalizedString("PdfImage_Error"), ex.ToString()),
                                    CloseButtonText = loc.GetLocalizedString("Dialog_Ok"),
                                    XamlRoot = App.MainPageInstance.Content.XamlRoot
                                };

                                 errorDialog.ShowAsync();
                            }
                        }
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("ShotDate"));
                        table.Cell().Text(item.ShotDate.ToString() ?? string.Empty);
                    }
                    if (i == 2)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("ImageMimeType"));
                        table.Cell().Text(item.ImageMimeType ?? string.Empty);
                    }
                    if (i == 3)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Info"));
                        table.Cell().Text(item.Info ?? string.Empty);
                    }
                    if (i == 4)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Notes"));
                        table.Cell().Text(item.Memo ?? string.Empty);
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
        if (Tbl75SpeciessesDocumentDataSource.CommentsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderComments(container, TabLeftRel4, TextHight);
    }
    private void ComposeComments(IContainer container)
    {
        if (Tbl75SpeciessesDocumentDataSource.CommentsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl75SpeciessesDocumentDataSource.CommentsCollection)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Info"));
                        table.Cell().Text(item.Info ?? string.Empty);
                    }
                    if (i == 1)
                    {
                        table.Cell().Text("");
                        table.Cell().Text(Loc("Memo"));
                        table.Cell().Text(item.Memo ?? string.Empty);
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
                    table.Cell().Text(Loc("Technic")).SemiBold();
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
                    table.Cell().Text(Loc("LNumberLOrigin"));
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
                    table.Cell().Text(Loc("LdaNumberLdaOrigin"));
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
                    table.Cell().Text(Loc("BasinLength"));
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
                    table.Cell().Text(Loc("FishLength"));
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
                    table.Cell().Text(Loc("Karnivore"));
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
                    table.Cell().Text(Loc("Herbivore"));
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
                    table.Cell().Text(Loc("Limnivore"));
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
                    table.Cell().Text(Loc("Omnivore"));
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
                    table.Cell().Text(Loc("MemoFood")).SemiBold();
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
                    table.Cell().Text(Loc("Memo")).SemiBold();
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
                    table.Cell().Text(Loc("Difficult1"));
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
                    table.Cell().Text(Loc("Difficult2"));
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
                    table.Cell().Text(Loc("Difficult3"));
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
                    table.Cell().Text(Loc("Difficult4"));
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
                    table.Cell().Text(Loc("RegionTop"));
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
                    table.Cell().Text(Loc("RegionMiddle"));
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
                    table.Cell().Text(Loc("RegionBottom"));
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
                    table.Cell().Text(Loc("MemoRegion")).SemiBold();
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
                    table.Cell().Text(Loc("Memo")).SemiBold();
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
                    table.Cell().Text(Loc("Ph12"));
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
                    table.Cell().Text(Loc("Temp12"));
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
                    table.Cell().Text(Loc("Hardness12"));
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
                    table.Cell().Text(Loc("CarboHardness12"));
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
                    table.Cell().Text(Loc("TypeSpecies"));
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
                    table.Cell().Text(Loc("MemoTechnic")).SemiBold();
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
                    table.Cell().Text(Loc("MemoHusbandry")).SemiBold();
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
                    table.Cell().Text(Loc("MemoColor")).SemiBold();
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
                    table.Cell().Text(Loc("MemoBreeding")).SemiBold();
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
                    table.Cell().Text(Loc("MemoBuilt")).SemiBold();
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
                    table.Cell().Text(Loc("MemoSozial")).SemiBold();
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
                    table.Cell().Text(Loc("MemoDimorphism")).SemiBold();
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
                    table.Cell().Text(Loc("MemoSpecial")).SemiBold();
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
                    table.Cell().Text(Loc("Technic")).SemiBold();
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
                    table.Cell().Text(Loc("BasinHeight"));
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
                    table.Cell().Text(Loc("PlantLength"));
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
                    table.Cell().Text(Loc("Difficult1"));
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
                    table.Cell().Text(Loc("Difficult2"));
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
                    table.Cell().Text(Loc("Difficult3"));
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
                    table.Cell().Text(Loc("Difficult4"));
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
                    table.Cell().Text(Loc("Ph12"));
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
                    table.Cell().Text(Loc("Temp12"));
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
                    table.Cell().Text(Loc("Hardness12"));
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
                    table.Cell().Text(Loc("CarboHardness12"));
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
                    table.Cell().Text(Loc("MemoReproduction")).SemiBold();
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
                    table.Cell().Text(Loc("MemoCulture")).SemiBold();
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
                    table.Cell().Text(Loc("MemoBuilt")).SemiBold();
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
                    table.Cell().Text(Loc("MemoColor")).SemiBold();
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
                    table.Cell().Text(Loc("MemoGlobal")).SemiBold();
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
                    table.Cell().Text($"{Model.MemoGlobal}");
                });
            });
        });
    }

}
