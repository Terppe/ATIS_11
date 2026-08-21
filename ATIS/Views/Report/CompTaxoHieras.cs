using QuestPDF.Fluent;
using WinUI3Localizer;

namespace ATIS.Views.Report;
public abstract class CompTaxoHieras
{
    private static string Loc(string key) => Localizer.Get().GetLocalizedString(key);
    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }
    public static void ComposeTaxoHierRegnum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Regnum modelRegnumTree)
    {
        if (modelRegnumTree == null || !IsVisibleTaxonName(modelRegnumTree.RegnumName))
            return;

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Regnum"));
            table.Cell()
                .Text(
                    $"{modelRegnumTree.RegnumName ?? string.Empty} {modelRegnumTree.Subregnum ?? string.Empty} {modelRegnumTree.Author ?? string.Empty}" +
                    $" {modelRegnumTree.AuthorYear ?? string.Empty} {modelRegnumTree.EngName ?? string.Empty} {modelRegnumTree.GerName ?? string.Empty}" +
                    $" {modelRegnumTree.FraName ?? string.Empty} {modelRegnumTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierPhylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Phylum modelPhylumTree)
    {
        if (modelPhylumTree == null || !IsVisibleTaxonName(modelPhylumTree.PhylumName))
            return;

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Phylum"));
            table.Cell().Text($"{modelPhylumTree.PhylumName ?? string.Empty} {modelPhylumTree.Author ?? string.Empty} {modelPhylumTree.AuthorYear ?? string.Empty}" +
                              $" {modelPhylumTree.EngName ?? string.Empty} {modelPhylumTree.GerName ?? string.Empty}" +
                              $" {modelPhylumTree.FraName ?? string.Empty} {modelPhylumTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierSubphylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subphylum modelSubphylumTree)
    {
        if (modelSubphylumTree == null || !IsVisibleTaxonName(modelSubphylumTree.SubphylumName))
            return;

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Subphylum"));
            table.Cell().Text($"{modelSubphylumTree.SubphylumName ?? string.Empty} {modelSubphylumTree.Author ?? string.Empty} {modelSubphylumTree.AuthorYear ?? string.Empty}" +
                              $" {modelSubphylumTree.EngName ?? string.Empty} {modelSubphylumTree.GerName ?? string.Empty}" +
                              $" {modelSubphylumTree.FraName ?? string.Empty} {modelSubphylumTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierSuperclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Superclass modelSuperclassTree)
    {
        if (modelSuperclassTree == null || !IsVisibleTaxonName(modelSuperclassTree.SuperclassName))
            return;

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Superclass"));
            table.Cell().Text($"{modelSuperclassTree.SuperclassName ?? string.Empty} {modelSuperclassTree.Author ?? string.Empty} {modelSuperclassTree.AuthorYear ?? string.Empty}" +
                              $" {modelSuperclassTree.EngName ?? string.Empty} {modelSuperclassTree.GerName ?? string.Empty}" +
                              $" {modelSuperclassTree.FraName ?? string.Empty} {modelSuperclassTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierClass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Class modelClassTree)
    {
        if (modelClassTree == null || !IsVisibleTaxonName(modelClassTree.ClassName))
            return;

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Class"));
            table.Cell().Text($"{modelClassTree.ClassName ?? string.Empty} {modelClassTree.Author ?? string.Empty} {modelClassTree.AuthorYear ?? string.Empty}" +
                              $" {modelClassTree.EngName ?? string.Empty} {modelClassTree.GerName ?? string.Empty}" +
                              $" {modelClassTree.FraName ?? string.Empty} {modelClassTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierSubclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subclass modelSubclassTree)
    {
        if (modelSubclassTree == null || !IsVisibleTaxonName(modelSubclassTree.SubclassName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Subclass"));
            table.Cell().Text(
                $"{modelSubclassTree.SubclassName ?? string.Empty} {modelSubclassTree.Author ?? string.Empty} {modelSubclassTree.AuthorYear ?? string.Empty}" +
                $" {modelSubclassTree.EngName ?? string.Empty} {modelSubclassTree.GerName ?? string.Empty}" +
                $" {modelSubclassTree.FraName ?? string.Empty} {modelSubclassTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierInfraclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Infraclass modelInfraclassTree)
    {
        if (modelInfraclassTree == null || !IsVisibleTaxonName(modelInfraclassTree.InfraclassName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Infraclass"));
            table.Cell().Text($"{modelInfraclassTree.InfraclassName ?? string.Empty} {modelInfraclassTree.Author ?? string.Empty} {modelInfraclassTree.AuthorYear ?? string.Empty}" +
                              $" {modelInfraclassTree.EngName ?? string.Empty} {modelInfraclassTree.GerName ?? string.Empty}" +
                              $" {modelInfraclassTree.FraName ?? string.Empty} {modelInfraclassTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierLegio(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Legio modelLegioTree)
    {
        if (modelLegioTree == null || !IsVisibleTaxonName(modelLegioTree.LegioName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Legio"));
            table.Cell().Text($"{modelLegioTree.LegioName ?? string.Empty} {modelLegioTree.Author ?? string.Empty} {modelLegioTree.AuthorYear ?? string.Empty}" +
                              $" {modelLegioTree.EngName ?? string.Empty} {modelLegioTree.GerName ?? string.Empty}" +
                              $" {modelLegioTree.FraName ?? string.Empty} {modelLegioTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierOrdo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Ordo modelOrdoTree)
    {
        if (modelOrdoTree == null || !IsVisibleTaxonName(modelOrdoTree.OrdoName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Ordo"));
            table.Cell().Text($"{modelOrdoTree.OrdoName ?? string.Empty} {modelOrdoTree.Author ?? string.Empty} {modelOrdoTree.AuthorYear ?? string.Empty}" +
                              $" {modelOrdoTree.EngName ?? string.Empty} {modelOrdoTree.GerName ?? string.Empty}" +
                              $" {modelOrdoTree.FraName ?? string.Empty} {modelOrdoTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierSubordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Subordo modelSubordoTree)
    {
        if (modelSubordoTree == null || !IsVisibleTaxonName(modelSubordoTree.SubordoName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Subordo"));
            table.Cell().Text($"{modelSubordoTree.SubordoName ?? string.Empty} {modelSubordoTree.Author ?? string.Empty} {modelSubordoTree.AuthorYear ?? string.Empty}" +
                              $" {modelSubordoTree.EngName ?? string.Empty} {modelSubordoTree.GerName ?? string.Empty}" +
                              $" {modelSubordoTree.FraName ?? string.Empty} {modelSubordoTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierInfraordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Infraordo modelInfraordoTree)
    {
        if (modelInfraordoTree == null || !IsVisibleTaxonName(modelInfraordoTree.InfraordoName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Infraordo"));
            table.Cell().Text($"{modelInfraordoTree.InfraordoName ?? string.Empty}   {modelInfraordoTree.Author ?? string.Empty} {modelInfraordoTree.AuthorYear ?? string.Empty}" +
                              $" {modelInfraordoTree.EngName ?? string.Empty} {modelInfraordoTree.GerName ?? string.Empty}" +
                              $" {modelInfraordoTree.FraName ?? string.Empty} {modelInfraordoTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierSuperfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Superfamily modelSuperfamilyTree)
    {
        if (modelSuperfamilyTree == null || !IsVisibleTaxonName(modelSuperfamilyTree.SuperfamilyName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Superfamily"));
            table.Cell().Text($"{modelSuperfamilyTree.SuperfamilyName ?? string.Empty} {modelSuperfamilyTree.Author ?? string.Empty} {modelSuperfamilyTree.AuthorYear ?? string.Empty}" +
                              $" {modelSuperfamilyTree.EngName ?? string.Empty} {modelSuperfamilyTree.GerName ?? string.Empty}" +
                              $" {modelSuperfamilyTree.FraName ?? string.Empty} {modelSuperfamilyTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierFamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel,
        PdfModels.Family modelFamilyTree)
    {
        if (modelFamilyTree == null || !IsVisibleTaxonName(modelFamilyTree.FamilyName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Family"));
            table.Cell().Text($"{modelFamilyTree.FamilyName ?? string.Empty} {modelFamilyTree.Author ?? string.Empty} {modelFamilyTree.AuthorYear ?? string.Empty}" +
                              $" {modelFamilyTree.EngName ?? string.Empty} {modelFamilyTree.GerName ?? string.Empty}" +
                              $" {modelFamilyTree.FraName ?? string.Empty} {modelFamilyTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierSubfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subfamily modelSubfamilyTree)
    {
        if (modelSubfamilyTree == null || !IsVisibleTaxonName(modelSubfamilyTree.SubfamilyName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Subfamily"));
            table.Cell().Text($"{modelSubfamilyTree.SubfamilyName ?? string.Empty} {modelSubfamilyTree.Author ?? string.Empty} {modelSubfamilyTree.AuthorYear ?? string.Empty}" +
                              $" {modelSubfamilyTree.EngName ?? string.Empty} {modelSubfamilyTree.GerName ?? string.Empty}" +
                              $" {modelSubfamilyTree.FraName ?? string.Empty} {modelSubfamilyTree.PorName ?? string.Empty}");
        });
    }
    public static void ComposeTaxoHierInfrafamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infrafamily modelInfrafamilyTree)
    {
        if (modelInfrafamilyTree == null || !IsVisibleTaxonName(modelInfrafamilyTree.InfrafamilyName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Infrafamily"));
            table.Cell().Text($"{modelInfrafamilyTree.InfrafamilyName ?? string.Empty} {modelInfrafamilyTree.Author ?? string.Empty} {modelInfrafamilyTree.AuthorYear ?? string.Empty}" +
                              $" {modelInfrafamilyTree.EngName ?? string.Empty} {modelInfrafamilyTree.GerName ?? string.Empty}" +
                              $" {modelInfrafamilyTree.FraName ?? string.Empty} {modelInfrafamilyTree.PorName ?? string.Empty}");
        });
    }


    public static void ComposeTaxoHierSupertribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Supertribus modelSupertribusTree)
    {
        if (modelSupertribusTree == null || !IsVisibleTaxonName(modelSupertribusTree.SupertribusName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Supertribus"));
            table.Cell().Text($"{modelSupertribusTree.SupertribusName ?? string.Empty} {modelSupertribusTree.Author ?? string.Empty} {modelSupertribusTree.AuthorYear ?? string.Empty}" +
                              $" {modelSupertribusTree.EngName ?? string.Empty} {modelSupertribusTree.GerName ?? string.Empty}" +
                              $" {modelSupertribusTree.FraName ?? string.Empty} {modelSupertribusTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierTribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Tribus modelTribusTree)
    {
        if (modelTribusTree == null || !IsVisibleTaxonName(modelTribusTree.TribusName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Tribus"));
            table.Cell().Text($"{modelTribusTree.TribusName ?? string.Empty} {modelTribusTree.Author ?? string.Empty} {modelTribusTree.AuthorYear ?? string.Empty}" +
                              $" {modelTribusTree.EngName ?? string.Empty} {modelTribusTree.GerName ?? string.Empty}" +
                              $" {modelTribusTree.FraName ?? string.Empty} {modelTribusTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierSubtribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subtribus modelSubtribusTree)
    {
        if (modelSubtribusTree == null || !IsVisibleTaxonName(modelSubtribusTree.SubtribusName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Subtribus"));
            table.Cell().Text($"{modelSubtribusTree.SubtribusName ?? string.Empty} {modelSubtribusTree.Author ?? string.Empty} {modelSubtribusTree.AuthorYear ?? string.Empty}" +
                              $" {modelSubtribusTree.EngName ?? string.Empty} {modelSubtribusTree.GerName ?? string.Empty}" +
                              $" {modelSubtribusTree.FraName ?? string.Empty} {modelSubtribusTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierInfratribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infratribus modelInfratribusTree)
    {
        if (modelInfratribusTree == null || !IsVisibleTaxonName(modelInfratribusTree.InfratribusName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Infratribus"));
            table.Cell().Text($"{modelInfratribusTree.InfratribusName ?? string.Empty} {modelInfratribusTree.Author ?? string.Empty} {modelInfratribusTree.AuthorYear ?? string.Empty}" +
                              $" {modelInfratribusTree.EngName ?? string.Empty} {modelInfratribusTree.GerName ?? string.Empty}" +
                              $" {modelInfratribusTree.FraName ?? string.Empty} {modelInfratribusTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierGenus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Genus modelGenusTree)
    {
        if (modelGenusTree == null || !IsVisibleTaxonName(modelGenusTree.GenusName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Genus"));
            table.Cell().Text($"{modelGenusTree.GenusName ?? string.Empty} {modelGenusTree.Author ?? string.Empty} {modelGenusTree.AuthorYear ?? string.Empty}" +
                              $" {modelGenusTree.EngName ?? string.Empty} {modelGenusTree.GerName ?? string.Empty}" +
                              $" {modelGenusTree.FraName ?? string.Empty} {modelGenusTree.PorName ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierSpecies(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Species modelSpeciesTree)
    {
        if (modelSpeciesTree == null || !IsVisibleTaxonName(modelSpeciesTree.SpeciesName))
            return;
        if (string.IsNullOrEmpty(modelSpeciesTree.Subspecies))
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
                table.Cell().Text(Loc("Species"));
                table.Cell().Text($"{modelSpeciesTree.Tbl66Genusses?.GenusName ?? string.Empty} {modelSpeciesTree.SpeciesName ?? string.Empty}" +
                                  $" {modelSpeciesTree.Author ?? string.Empty} {modelSpeciesTree.AuthorYear ?? string.Empty}");
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
                table.Cell().Text(Loc("Species"));
                table.Cell().Text($"{modelSpeciesTree.Tbl66Genusses?.GenusName ?? string.Empty} {modelSpeciesTree.SpeciesName ?? string.Empty}" +
                                  $" {modelSpeciesTree.Author ?? string.Empty} {modelSpeciesTree.AuthorYear ?? string.Empty}");
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
                table.Cell().Text(Loc("Subspecies"));
                table.Cell().Text($"{modelSpeciesTree.Tbl66Genusses?.GenusName ?? string.Empty} {modelSpeciesTree.SpeciesName ?? string.Empty}" +
                                  $" {modelSpeciesTree.Subspecies ?? string.Empty} {modelSpeciesTree.Divers ?? string.Empty}" +
                                  $" {modelSpeciesTree.Author ?? string.Empty} {modelSpeciesTree.AuthorYear ?? string.Empty}");
            });
        }
    }

    //public static void ComposeTaxoHierPlSpecies(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.PlSpecies modelPlSpeciesTree)
    //{
    //    if (string.IsNullOrEmpty(modelPlSpeciesTree.Subspecies))
    //    {
    //        column.Item().Table(table =>
    //        {
    //            table.ColumnsDefinition(columns =>
    //            {
    //                columns.ConstantColumn(tabLeftRel);
    //                columns.ConstantColumn(tabRightRel);
    //                columns.RelativeColumn();
    //            });
    //            table.Cell().Text("");
    //            table.Cell().Text(Strings.StringsRes.PlSpecies);
    //            table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
    //                              $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
    //        });
    //    }
    //    else
    //    {
    //        column.Item().Table(table =>
    //        {
    //            table.ColumnsDefinition(columns =>
    //            {
    //                columns.ConstantColumn(tabLeftRel);
    //                columns.ConstantColumn(tabRightRel);
    //                columns.RelativeColumn();
    //            });
    //            table.Cell().Text("");
    //            table.Cell().Text(Strings.StringsRes.PlSpecies);
    //            table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
    //                              $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
    //        });
    //        column.Item().Table(table =>
    //        {
    //            table.ColumnsDefinition(columns =>
    //            {
    //                columns.ConstantColumn(tabLeftRel + 4);
    //                columns.ConstantColumn(tabRightRel - 4);
    //                columns.RelativeColumn();
    //            });
    //            table.Cell().Text("");
    //            table.Cell().Text(Strings.StringsRes.Subspecies);
    //            table.Cell().Text($"{modelPlSpeciesTree.Tbl66Genusses?.GenusName} {modelPlSpeciesTree.PlSpeciesName}" +
    //                              $" {modelPlSpeciesTree.Subspecies} {modelPlSpeciesTree.Divers}" +
    //                              $" {modelPlSpeciesTree.Author} {modelPlSpeciesTree.AuthorYear}");
    //        });
    //    }
    //}

    public static void ComposeTaxoHierSynonym(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Synonym modelSynonymsTree)
    {
        if (modelSynonymsTree == null || !IsVisibleTaxonName(modelSynonymsTree.SynonymName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Synonym"));
            table.Cell().Text($"{modelSynonymsTree?.SynonymName ?? string.Empty} {modelSynonymsTree?.Author ?? string.Empty} {modelSynonymsTree?.AuthorYear ?? string.Empty}");
        });
    }

    public static void ComposeTaxoHierName(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Name modelNamesTree)
    {
        if (modelNamesTree == null || !IsVisibleTaxonName(modelNamesTree.NameName))
            return;
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(tabLeftRel);
                columns.ConstantColumn(tabRightRel);
                columns.RelativeColumn();
            });
            table.Cell().Text("");
            table.Cell().Text(Loc("Name"));
            table.Cell().Text($"{modelNamesTree.NameName ?? string.Empty} [{modelNamesTree.Language ?? string.Empty}]"); 
        });
    }
}
