using System.Configuration;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WinUI3Localizer;

namespace ATIS.Views.Report.Pdf.Tbl21Classes;

public class Tbl21ClassesDocument : IDocument
{
    private Tbl21ClassesPdfModel Model { get; }
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);

    private static int _tabLeftRel;
    private static int _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab")); //150
    private const int TabLeftRel4 = 4;
    private const int TabLeftRel8 = 8;
    private static int _tabLeftLastRel;
    private static int _tabRightLastRel;
    private static readonly int TabRightRel8 = _tabRightRel - 8;
    private const int TextHight = 12;

    public Tbl21ClassesDocument(Tbl21ClassesPdfModel model)
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
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Span(Loc("Atis")).FontSize(18).SemiBold();
                    text.Span("  ");
                    text.Span(Loc("AtisHeader")).FontSize(TextHight).SemiBold();
                    text.Span("  ");
                    text.Span(DateTime.Now.ToString(CultureInfo.InstalledUICulture)).NormalPosition();
                });

                column.Item().Text(text =>
                {
                    text.Span(Model.ClassName ?? string.Empty).FontSize(TextHight).SemiBold().Italic();
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
                    table.Cell().Text(Loc("Class"));
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

                    var names = new List<string>();

                    if (!string.IsNullOrWhiteSpace(Model.EngName))
                        names.Add($"{Model.EngName} [ENG]");

                    if (!string.IsNullOrWhiteSpace(Model.GerName))
                        names.Add($"{Model.GerName} [GER]");

                    if (!string.IsNullOrWhiteSpace(Model.FraName))
                        names.Add($"{Model.FraName} [FRA]");

                    if (!string.IsNullOrWhiteSpace(Model.PorName))
                        names.Add($"{Model.PorName} [SPA]");

                    table.Cell().Text("");
                    table.Cell().Text(Loc("Names"));
                    table.Cell().Text(string.Join(" ", names));
                });

                //column.Item().Table(table =>
                //{
                //    table.ColumnsDefinition(columns =>
                //    {
                //        columns.ConstantColumn(TabLeftRel8);
                //        columns.ConstantColumn(TabRightRel8);
                //        columns.RelativeColumn();
                //    });
                //    table.Cell().Text("");
                //    table.Cell().Text(Loc("Names"));
                //    table.Cell().Text($"{Model.EngName}[ENG] {Model.GerName}[GER] {Model.FraName}[FRA] {Model.PorName}[SPA]");
                //});
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(TabLeftRel8);
                        columns.RelativeColumn();
                    });
                    table.Cell().Text("");
                    table.Cell().Text(Loc("TaxoStatus")).SemiBold();
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
                    table.Cell().Text($"{Model.Valid} {Model.ValidYear ?? string.Empty}");
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
                    table.Cell().Text(Loc("CredibilityRating"));
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
                if (Model.SubphylumTree != null && !string.IsNullOrWhiteSpace(Model.SubphylumTree!.SubphylumName))
                {
                    _tabLeftRel += 4;
                    _tabRightRel -= 4;
                    CompTaxoHieras.ComposeTaxoHierSubphylum(column, _tabLeftRel, _tabRightRel, Model.SubphylumTree);
                }
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
                    _tabLeftLastRel = _tabLeftRel;
                    _tabRightLastRel = _tabRightRel;
                    CompTaxoHieras.ComposeTaxoHierClass(column, _tabLeftRel, _tabRightRel, Model.ClassTree);
                }
            });
            _tabRightRel = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReportPdfRightTab"));
            _tabLeftRel = 0;
        });
    }

    private void ComposeHeaderChildren(IContainer container)
    {
        if (Tbl21ClassesDocumentDataSource.SubclassesCollection.Count == 0)
        {
            return;
        }

        CompHeaders.HeaderChildren(container, _tabRightRel, TextHight);
    }

    private void ComposeChildrenContent(IContainer container)
    {
        if (Tbl21ClassesDocumentDataSource.SubclassesCollection.Count == 0)
        {
            return;
        }

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(_tabLeftLastRel + 4);
                columns.ConstantColumn(_tabRightLastRel - 4);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl21ClassesDocumentDataSource.SubclassesCollection)
            {
                table.Cell().Text("");
                table.Cell().Text(Loc("Subclass"));
                table.Cell().Text($"{item.SubclassName ?? string.Empty} {item.Author ?? string.Empty} {item.AuthorYear ?? string.Empty}");
            }
        });
    }

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
        if (Tbl21ClassesDocumentDataSource.ExpertsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl21ClassesDocumentDataSource.ExpertsCollection)
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
        CompHeaders.HeaderReferencesOtherSources(container, TabLeftRel8);
    }

    private void ComposeReferencesOtherSources(IContainer container)
    {
        if (Tbl21ClassesDocumentDataSource.SourcesCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl21ClassesDocumentDataSource.SourcesCollection)
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
        CompHeaders.HeaderReferencesPublications(container, TabLeftRel8);
    }

    private void ComposeReferencesPublications(IContainer container)
    {
        if (Tbl21ClassesDocumentDataSource.AuthorsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl21ClassesDocumentDataSource.AuthorsCollection)
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
        CompHeaders.HeaderComments(container, TabLeftRel4, TextHight);
    }

    private void ComposeComments(IContainer container)
    {
        if (Tbl21ClassesDocumentDataSource.CommentsCollection.Count == 0)
            return;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(TabLeftRel8);
                columns.ConstantColumn(TabRightRel8);
                columns.RelativeColumn();
            });

            foreach (var item in Tbl21ClassesDocumentDataSource.CommentsCollection)
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
