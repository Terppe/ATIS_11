using QuestPDF.Fluent;

namespace ATIS.Views.Report;
public class CompTaxoHieras
{
    public static void ComposeTaxoHierRegnum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Regnum modelRegnumTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Regnum);
            table.Cell()
                .Text(
                    $"{modelRegnumTree.RegnumName} {modelRegnumTree.Subregnum} {modelRegnumTree.Author}" +
                    $" {modelRegnumTree.AuthorYear} {modelRegnumTree.EngName} {modelRegnumTree.GerName}" +
                    $" {modelRegnumTree.FraName} {modelRegnumTree.PorName}");
        });
    }

    public static void ComposeTaxoHierPhylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Phylum modelPhylumTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Phylum);
            table.Cell().Text($"{modelPhylumTree.PhylumName} {modelPhylumTree.Author} {modelPhylumTree.AuthorYear}" +
                              $" {modelPhylumTree.EngName} {modelPhylumTree.GerName}" +
                              $" {modelPhylumTree.FraName} {modelPhylumTree.PorName}");
        });
    }


    public static void ComposeTaxoHierDivision(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Division modelDivisionTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Division);
            table.Cell().Text($"{modelDivisionTree.DivisionName} {modelDivisionTree.Author} {modelDivisionTree.AuthorYear}" +
                              $" {modelDivisionTree.EngName} {modelDivisionTree.GerName}" +
                              $" {modelDivisionTree.FraName} {modelDivisionTree.PorName}");
        });
    }

 
    public static void ComposeTaxoHierSubphylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subphylum modelSubphylumTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subphylum);
            table.Cell().Text($"{modelSubphylumTree.SubphylumName} {modelSubphylumTree.Author} {modelSubphylumTree.AuthorYear}" +
                              $" {modelSubphylumTree.EngName} {modelSubphylumTree.GerName}" +
                              $" {modelSubphylumTree.FraName} {modelSubphylumTree.PorName}");
        });
    }


    public static void ComposeTaxoHierSubdivision(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subdivision modelSubdivisionTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subdivision);
            table.Cell().Text($"{modelSubdivisionTree.SubdivisionName} {modelSubdivisionTree.Author} {modelSubdivisionTree.AuthorYear}" +
                              $" {modelSubdivisionTree.EngName} {modelSubdivisionTree.GerName}" +
                              $" {modelSubdivisionTree.FraName} {modelSubdivisionTree.PorName}");
        });
    }

    public static void ComposeTaxoHierSuperclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Superclass modelSuperclassTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Superclass);
            table.Cell().Text($"{modelSuperclassTree.SuperclassName} {modelSuperclassTree.Author} {modelSuperclassTree.AuthorYear}" +
                              $" {modelSuperclassTree.EngName} {modelSuperclassTree.GerName}" +
                              $" {modelSuperclassTree.FraName} {modelSuperclassTree.PorName}");
        });
    }
    public static void ComposeTaxoHierClass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Class modelClassTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Class);
            table.Cell().Text($"{modelClassTree.ClassName} {modelClassTree.Author} {modelClassTree.AuthorYear}" +
                              $" {modelClassTree.EngName} {modelClassTree.GerName}" +
                              $" {modelClassTree.FraName} {modelClassTree.PorName}");
        });
    }
    public static void ComposeTaxoHierSubclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subclass modelSubclassTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subclass);
            if (modelSubclassTree != null)
            {
                table.Cell().Text(
                    $"{modelSubclassTree.SubclassName} {modelSubclassTree.Author} {modelSubclassTree.AuthorYear}" +
                    $" {modelSubclassTree.EngName} {modelSubclassTree.GerName}" +
                    $" {modelSubclassTree.FraName} {modelSubclassTree.PorName}");
            }
        });
    }
    public static void ComposeTaxoHierInfraclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Infraclass modelInfraclassTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Infraclass);
            table.Cell().Text($"{modelInfraclassTree.InfraclassName} {modelInfraclassTree.Author} {modelInfraclassTree.AuthorYear}" +
                              $" {modelInfraclassTree.EngName} {modelInfraclassTree.GerName}" +
                              $" {modelInfraclassTree.FraName} {modelInfraclassTree.PorName}");
        });
    }

    public static void ComposeTaxoHierLegio(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Legio modelLegioTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Legio);
            table.Cell().Text($"{modelLegioTree.LegioName} {modelLegioTree.Author} {modelLegioTree.AuthorYear}" +
                              $" {modelLegioTree.EngName} {modelLegioTree.GerName}" +
                              $" {modelLegioTree.FraName} {modelLegioTree.PorName}");
        });
    }
    public static void ComposeTaxoHierOrdo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Ordo modelOrdoTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Ordo);
            table.Cell().Text($"{modelOrdoTree.OrdoName} {modelOrdoTree.Author} {modelOrdoTree.AuthorYear}" +
                              $" {modelOrdoTree.EngName} {modelOrdoTree.GerName}" +
                              $" {modelOrdoTree.FraName} {modelOrdoTree.PorName}");
        });
    }

    public static void ComposeTaxoHierSubordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subordo modelSubordoTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subordo);
            table.Cell().Text($"{modelSubordoTree.SubordoName} {modelSubordoTree.Author} {modelSubordoTree.AuthorYear}" +
                              $" {modelSubordoTree.EngName} {modelSubordoTree.GerName}" +
                              $" {modelSubordoTree.FraName} {modelSubordoTree.PorName}");
        });
    }
    public static void ComposeTaxoHierInfraordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Infraordo modelInfraordoTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Infraordo);
            table.Cell().Text($"{modelInfraordoTree.InfraordoName} {modelInfraordoTree.Author} {modelInfraordoTree.AuthorYear}" +
                              $" {modelInfraordoTree.EngName} {modelInfraordoTree.GerName}" +
                              $" {modelInfraordoTree.FraName} {modelInfraordoTree.PorName}");
        });
    }
    public static void ComposeTaxoHierSuperfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Superfamily modelSuperfamilyTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Superfamily);
            table.Cell().Text($"{modelSuperfamilyTree.SuperfamilyName} {modelSuperfamilyTree.Author} {modelSuperfamilyTree.AuthorYear}" +
                              $" {modelSuperfamilyTree.EngName} {modelSuperfamilyTree.GerName}" +
                              $" {modelSuperfamilyTree.FraName} {modelSuperfamilyTree.PorName}");
        });
    }
    public static void ComposeTaxoHierFamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Family modelFamilyTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Family);
            table.Cell().Text($"{modelFamilyTree.FamilyName} {modelFamilyTree.Author} {modelFamilyTree.AuthorYear}" +
                              $" {modelFamilyTree.EngName} {modelFamilyTree.GerName}" +
                              $" {modelFamilyTree.FraName} {modelFamilyTree.PorName}");
        });
    }
    public static void ComposeTaxoHierSubfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subfamily modelSubfamilyTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subfamily);
            table.Cell().Text($"{modelSubfamilyTree.SubfamilyName} {modelSubfamilyTree.Author} {modelSubfamilyTree.AuthorYear}" +
                              $" {modelSubfamilyTree.EngName} {modelSubfamilyTree.GerName}" +
                              $" {modelSubfamilyTree.FraName} {modelSubfamilyTree.PorName}");
        });
    }
    public static void ComposeTaxoHierInfrafamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infrafamily modelInfrafamilyTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Infrafamily);
            table.Cell().Text($"{modelInfrafamilyTree.InfrafamilyName} {modelInfrafamilyTree.Author} {modelInfrafamilyTree.AuthorYear}" +
                              $" {modelInfrafamilyTree.EngName} {modelInfrafamilyTree.GerName}" +
                              $" {modelInfrafamilyTree.FraName} {modelInfrafamilyTree.PorName}");
        });
    }


    public static void ComposeTaxoHierSupertribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Supertribus modelSupertribusTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Supertribus);
            table.Cell().Text($"{modelSupertribusTree.SupertribusName} {modelSupertribusTree.Author} {modelSupertribusTree.AuthorYear}" +
                              $" {modelSupertribusTree.EngName} {modelSupertribusTree.GerName}" +
                              $" {modelSupertribusTree.FraName} {modelSupertribusTree.PorName}");
        });
    }

    public static void ComposeTaxoHierTribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Tribus modelTribusTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Tribus);
            table.Cell().Text($"{modelTribusTree.TribusName} {modelTribusTree.Author} {modelTribusTree.AuthorYear}" +
                              $" {modelTribusTree.EngName} {modelTribusTree.GerName}" +
                              $" {modelTribusTree.FraName} {modelTribusTree.PorName}");
        });
    }

    public static void ComposeTaxoHierSubtribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subtribus modelSubtribusTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Subtribus);
            table.Cell().Text($"{modelSubtribusTree.SubtribusName} {modelSubtribusTree.Author} {modelSubtribusTree.AuthorYear}" +
                              $" {modelSubtribusTree.EngName} {modelSubtribusTree.GerName}" +
                              $" {modelSubtribusTree.FraName} {modelSubtribusTree.PorName}");
        });
    }

    public static void ComposeTaxoHierInfratribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infratribus modelInfratribusTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Infratribus);
            table.Cell().Text($"{modelInfratribusTree.InfratribusName} {modelInfratribusTree.Author} {modelInfratribusTree.AuthorYear}" +
                              $" {modelInfratribusTree.EngName} {modelInfratribusTree.GerName}" +
                              $" {modelInfratribusTree.FraName} {modelInfratribusTree.PorName}");
        });
    }

    public static void ComposeTaxoHierGenus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Genus modelGenusTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Genus);
            table.Cell().Text($"{modelGenusTree.GenusName} {modelGenusTree.Author} {modelGenusTree.AuthorYear}" +
                              $" {modelGenusTree.EngName} {modelGenusTree.GerName}" +
                              $" {modelGenusTree.FraName} {modelGenusTree.PorName}");
        });
    }

    public static void ComposeTaxoHierFiSpecies(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.FiSpecies modelFiSpeciesTree)
    {
        if (string.IsNullOrEmpty(modelFiSpeciesTree.Subspecies))
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel);
                    columns.ConstantColumn(tabRightRel);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.FiSpecies);
                table.Cell().Text($"{modelFiSpeciesTree.Tbl66Genusses?.GenusName} {modelFiSpeciesTree.FiSpeciesName}" +
                                  $" {modelFiSpeciesTree.Author} {modelFiSpeciesTree.AuthorYear}");
            });
        }
        else
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel);
                    columns.ConstantColumn(tabRightRel);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.FiSpecies);
                table.Cell().Text($"{modelFiSpeciesTree.Tbl66Genusses?.GenusName} {modelFiSpeciesTree.FiSpeciesName}" +
                                  $" {modelFiSpeciesTree.Author} {modelFiSpeciesTree.AuthorYear}");
            });
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel+4);
                    columns.ConstantColumn(tabRightRel-4);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.Subspecies);
                table.Cell().Text($"{modelFiSpeciesTree.Tbl66Genusses?.GenusName} {modelFiSpeciesTree.FiSpeciesName}" +
                                  $" {modelFiSpeciesTree.Subspecies} {modelFiSpeciesTree.Divers}" +
                                  $" {modelFiSpeciesTree.Author} {modelFiSpeciesTree.AuthorYear}");
            });
        }
    }

    public static void ComposeTaxoHierPlSpecies(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.PlSpecies modelPlSpeciesTree)
    {
        if (string.IsNullOrEmpty(modelPlSpeciesTree.Subspecies))
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel);
                    columns.ConstantColumn(tabRightRel);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.PlSpecies);
                table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
                                  $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
            });
        }
        else
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel);
                    columns.ConstantColumn(tabRightRel);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.PlSpecies);
                table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
                                  $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
            });
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(tabLeftRel + 4);
                    columns.ConstantColumn(tabRightRel - 4);
                    columns.RelativeColumn();
                });
                table.Cell().Text("");
                table.Cell().Text(Strings.StringsRes.Subspecies);
                table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
                                  $" {modelPlSpeciesTree.Subspecies} {modelPlSpeciesTree.Divers}" +
                                  $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
            });
        }
    }

    public static void ComposeTaxoHierSynonym(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Synonym modelSynonymsTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Synonym);
            table.Cell().Text($"{modelSynonymsTree?.SynonymName} {modelSynonymsTree?.Author} {modelSynonymsTree?.AuthorYear}");
        });
    }

    public static void ComposeTaxoHierName(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Name modelNamesTree)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Strings.StringsRes.Name);
            table.Cell().Text($"{modelNamesTree.NameName} [{modelNamesTree.Language}]"); 
        });
    }
}
