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

    private static string JoinParts(params string?[] parts)
    {
        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static void ComposeTaxoRow(
        ColumnDescriptor column,
        int tabLeftRel,
        int tabRightRel,
        string labelKey,
        string text)
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
            table.Cell().Text(Loc(labelKey));
            table.Cell().Text(text);
        });
    }

    public static void ComposeTaxoHierRegnum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Regnum model)
    {
        if (model == null || !IsVisibleTaxonName(model.RegnumName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Regnum",
            JoinParts(model.RegnumName, model.Subregnum, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierPhylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Phylum model)
    {
        if (model == null || !IsVisibleTaxonName(model.PhylumName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Phylum",
            JoinParts(model.PhylumName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSubphylum(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subphylum model)
    {
        if (model == null || !IsVisibleTaxonName(model.SubphylumName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Subphylum",
            JoinParts(model.SubphylumName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSuperclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Superclass model)
    {
        if (model == null || !IsVisibleTaxonName(model.SuperclassName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Superclass",
            JoinParts(model.SuperclassName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierClass(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Class model)
    {
        if (model == null || !IsVisibleTaxonName(model.ClassName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Class",
            JoinParts(model.ClassName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSubclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subclass model)
    {
        if (model == null || !IsVisibleTaxonName(model.SubclassName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Subclass",
            JoinParts(model.SubclassName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierInfraclass(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infraclass model)
    {
        if (model == null || !IsVisibleTaxonName(model.InfraclassName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Infraclass",
            JoinParts(model.InfraclassName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierLegio(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Legio model)
    {
        if (model == null || !IsVisibleTaxonName(model.LegioName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Legio",
            JoinParts(model.LegioName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierOrdo(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Ordo model)
    {
        if (model == null || !IsVisibleTaxonName(model.OrdoName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Ordo",
            JoinParts(model.OrdoName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSubordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subordo model)
    {
        if (model == null || !IsVisibleTaxonName(model.SubordoName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Subordo",
            JoinParts(model.SubordoName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierInfraordo(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infraordo model)
    {
        if (model == null || !IsVisibleTaxonName(model.InfraordoName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Infraordo",
            JoinParts(model.InfraordoName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSuperfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Superfamily model)
    {
        if (model == null || !IsVisibleTaxonName(model.SuperfamilyName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Superfamily",
            JoinParts(model.SuperfamilyName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierFamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Family model)
    {
        if (model == null || !IsVisibleTaxonName(model.FamilyName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Family",
            JoinParts(model.FamilyName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSubfamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subfamily model)
    {
        if (model == null || !IsVisibleTaxonName(model.SubfamilyName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Subfamily",
            JoinParts(model.SubfamilyName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierInfrafamily(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infrafamily model)
    {
        if (model == null || !IsVisibleTaxonName(model.InfrafamilyName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Infrafamily",
            JoinParts(model.InfrafamilyName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSupertribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Supertribus model)
    {
        if (model == null || !IsVisibleTaxonName(model.SupertribusName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Supertribus",
            JoinParts(model.SupertribusName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierTribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Tribus model)
    {
        if (model == null || !IsVisibleTaxonName(model.TribusName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Tribus",
            JoinParts(model.TribusName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSubtribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Subtribus model)
    {
        if (model == null || !IsVisibleTaxonName(model.SubtribusName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Subtribus",
            JoinParts(model.SubtribusName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierInfratribus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Infratribus model)
    {
        if (model == null || !IsVisibleTaxonName(model.InfratribusName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Infratribus",
            JoinParts(model.InfratribusName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierGenus(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Genus model)
    {
        if (model == null || !IsVisibleTaxonName(model.GenusName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Genus",
            JoinParts(model.GenusName, model.Author, model.AuthorYear, model.EngName, model.GerName, model.FraName, model.PorName));
    }

    public static void ComposeTaxoHierSpecies(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Species model)
    {
        if (model == null || !IsVisibleTaxonName(model.SpeciesName))
            return;

        var speciesText = JoinParts(
            model.Tbl66Genusses?.GenusName,
            model.SpeciesName,
            model.Author,
            model.AuthorYear);

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Species", speciesText);

        if (string.IsNullOrWhiteSpace(model.Subspecies))
            return;

        var subspeciesText = JoinParts(
            model.Tbl66Genusses?.GenusName,
            model.SpeciesName,
            model.Subspecies,
            model.Divers,
            model.Author,
            model.AuthorYear);

        ComposeTaxoRow(column, tabLeftRel + 4, tabRightRel - 4, "Subspecies", subspeciesText);
    }

    public static void ComposeTaxoHierSynonym(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Synonym model)
    {
        if (model == null || !IsVisibleTaxonName(model.SynonymName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Synonym",
            JoinParts(model.SynonymName, model.Author, model.AuthorYear));
    }

    public static void ComposeTaxoHierName(ColumnDescriptor column, int tabLeftRel, int tabRightRel, PdfModels.Name model)
    {
        if (model == null || !IsVisibleTaxonName(model.NameName))
            return;

        ComposeTaxoRow(column, tabLeftRel, tabRightRel, "Name",
            JoinParts(model.NameName, string.IsNullOrWhiteSpace(model.Language) ? null : $"[{model.Language}]"));
    }
}
