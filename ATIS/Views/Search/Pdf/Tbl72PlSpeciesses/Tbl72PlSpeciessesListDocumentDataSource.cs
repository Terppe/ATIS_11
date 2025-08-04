using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl72PlSpeciesses;

namespace ATIS.Views.Search.Pdf.Tbl72PlSpeciesses;
internal class Tbl72PlSpeciessesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl72PlSpecies PlSpeciesSingle = new();

    public static Tbl72PlSpeciessesPdfModel GetPlSpeciesListDetails(ObservableCollection<Tbl72PlSpecies> plspeciessesCollection)
    {

        PlSpeciessesCollection = plspeciessesCollection;

        return new Tbl72PlSpeciessesPdfModel
        {
            PlSpeciesName = PlSpeciesSingle.PlSpeciesName,
            Subspecies = PlSpeciesSingle.Subspecies,
            Divers = PlSpeciesSingle.Divers,
            CountId = PlSpeciesSingle.CountId,
            GenusId = PlSpeciesSingle.GenusId,
            SpeciesgroupId = PlSpeciesSingle.SpeciesgroupId,
            Valid = PlSpeciesSingle.Valid,
            ValidYear = PlSpeciesSingle.ValidYear,
            MemoSpecies = PlSpeciesSingle.MemoSpecies,
            TradeName = PlSpeciesSingle.TradeName,
            Author = PlSpeciesSingle.Author,
            AuthorYear = PlSpeciesSingle.AuthorYear,
            Importer = PlSpeciesSingle.Importer,
            ImportingYear = PlSpeciesSingle.ImportingYear,
            BasinHeight = PlSpeciesSingle.BasinHeight,
            PlantLength = PlSpeciesSingle.PlantLength,
            Difficult1 = PlSpeciesSingle.Difficult1,
            Difficult2 = PlSpeciesSingle.Difficult2,
            Difficult3 = PlSpeciesSingle.Difficult3,
            Difficult4 = PlSpeciesSingle.Difficult4,
            MemoTech = PlSpeciesSingle.MemoTech,
            Ph1 = PlSpeciesSingle.Ph1,
            Ph2 = PlSpeciesSingle.Ph2,
            Temp1 = PlSpeciesSingle.Temp1,
            Temp2 = PlSpeciesSingle.Temp2,
            Hardness1 = PlSpeciesSingle.Hardness1,
            Hardness2 = PlSpeciesSingle.Hardness2,
            CarboHardness1 = PlSpeciesSingle.CarboHardness1,
            CarboHardness2 = PlSpeciesSingle.CarboHardness2,
            MemoBuilt = PlSpeciesSingle.MemoBuilt,
            MemoColor = PlSpeciesSingle.MemoColor,
            MemoReproduction = PlSpeciesSingle.MemoReproduction,
            MemoCulture = PlSpeciesSingle.MemoCulture,
            MemoGlobal = PlSpeciesSingle.MemoGlobal,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl72PlSpecies> PlSpeciessesCollection { get; set; } = null!;
    #endregion "Collections"

}
