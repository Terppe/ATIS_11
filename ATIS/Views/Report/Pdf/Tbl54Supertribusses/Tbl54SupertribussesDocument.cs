using System.Configuration;
using System.Globalization;
using ATIS.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinUI3Localizer;


namespace ATIS.Views.Report.Pdf.Tbl54Supertribusses;

public class Tbl54SupertribussesDocument : IDocument
{
    private Tbl54SupertribussesPdfModel Model { get; }
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    private static int _tabLeftRel;
    private static int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
    private const int TabLeftRel4 = 4;
    private const int TabLeftRel8 = 8;
    private static int _tabLeftLastRel;
    private static int _tabRightLastRel;
    private static readonly int TabRightRel8 = _tabRightRel - 8;
    private const int TextHight = 12;

    public Tbl54SupertribussesDocument(Tbl54SupertribussesPdfModel model)
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
                    text.Span(Model.SupertribusName ?? string.Empty).FontSize(TextHight).SemiBold().Italic();
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

            //Comments
            column.Item().Element(ComposeHeaderComments);
            column.Item().Element(ComposeComments);
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
                    table.Cell().Text(Loc("Supertribus"));
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
                    table.Cell().Text(Loc("Synonyms"));
                    table.Cell().Text($"{Model.Synonym ?? string.Empty}");
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
                    table.Cell().Text(Loc("Names"));
                    table.Cell().Text(NamesFormatter.Format(Model.EngName, Model.GerName, Model.FraName, Model.PorName));
                });

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
                    table.Cell().Text(Loc("Info"));
                    table.Cell().Text($"{Model.Info ?? string.Empty}");
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
                    table.Cell().Text($"{Model.Memo ?? string.Empty}");
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
                    (left, right) => CompTaxoHieras.ComposeTaxoHierSupertribus(column, left, right, Model.SupertribusTree!),
                    rememberAsLast: true);

            });
        });
    }

    private void ComposeHeaderChildren(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.TribussesCollection?.Count is not > 0)
        {
            return;
        }

        CompHeaders.HeaderChildren(container, _tabRightRel, TextHight);
    }
    private void ComposeChildrenContent(IContainer container)
    {
        var visibleItems = Tbl54SupertribussesDocumentDataSource.TribussesCollection
            .Where(x => IsVisibleTaxonName(x.TribusName))
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
                table.Cell().Text(Loc("Tribus"));
                table.Cell().Text($"{item.TribusName ?? string.Empty} {item.Author ?? string.Empty} {item.AuthorYear ?? string.Empty}");
            }
        });
    }

    private void ComposeHeaderReferences(IContainer container)
    {
        var hasExperts = Tbl54SupertribussesDocumentDataSource.ExpertsCollection?.Count > 0;
        var hasSources = Tbl54SupertribussesDocumentDataSource.SourcesCollection?.Count > 0;
        var hasAuthors = Tbl54SupertribussesDocumentDataSource.AuthorsCollection?.Count > 0;

        if (!hasExperts && !hasSources && !hasAuthors)
            return;

        CompHeaders.HeaderHeaderReferences(container, TabLeftRel4, TextHight);
    }
    private void ComposeHeaderReferencesExperts(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.ExpertsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesExperts(container, TabLeftRel8);
    }

    private void ComposeReferencesExperts(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.ExpertsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl54SupertribussesDocumentDataSource.ExpertsCollection)
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
        if (Tbl54SupertribussesDocumentDataSource.SourcesCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesOtherSources(container, TabLeftRel8);
    }

    private void ComposeReferencesOtherSources(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.SourcesCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl54SupertribussesDocumentDataSource.SourcesCollection)
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
        if (Tbl54SupertribussesDocumentDataSource.AuthorsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderReferencesPublications(container, TabLeftRel8);
    }

    private void ComposeReferencesPublications(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.AuthorsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl54SupertribussesDocumentDataSource.AuthorsCollection)
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
    private void ComposeHeaderComments(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.CommentsCollection?.Count is not > 0)
            return;

        CompHeaders.HeaderComments(container, TabLeftRel4, TextHight);
    }

    private void ComposeComments(IContainer container)
    {
        if (Tbl54SupertribussesDocumentDataSource.CommentsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl54SupertribussesDocumentDataSource.CommentsCollection)
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
}
