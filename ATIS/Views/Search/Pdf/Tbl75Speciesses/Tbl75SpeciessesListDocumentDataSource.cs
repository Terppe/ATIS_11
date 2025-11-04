using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl75Speciesses;

namespace ATIS.Views.Search.Pdf.Tbl75Speciesses;
internal abstract class Tbl75SpeciessesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl75Species SpeciesSingle = new();

    public static Tbl75SpeciessesPdfModel GetSpeciesListDetails(ObservableCollection<Tbl75Species> speciessesCollection)
    {

        SpeciessesCollection = speciessesCollection;

        return new Tbl75SpeciessesPdfModel
        {
            SpeciesName = SpeciesSingle.SpeciesName,
            Subspecies = SpeciesSingle.Subspecies,
            Divers = SpeciesSingle.Divers,
            CountId = SpeciesSingle.CountId,
            Author = SpeciesSingle.Author,
            AuthorYear = SpeciesSingle.AuthorYear,
            Valid = SpeciesSingle.Valid,
            ValidYear = SpeciesSingle.ValidYear,
            MemoSpecies = SpeciesSingle.MemoSpecies,
            TradeName = SpeciesSingle.TradeName,
            Importer = SpeciesSingle.Importer,
            ImportingYear = SpeciesSingle.ImportingYear,
            TypeSpecies = SpeciesSingle.TypeSpecies,
            LNumber = SpeciesSingle.LNumber,
            LOrigin = SpeciesSingle.LOrigin,
            LdaOrigin = SpeciesSingle.LdaOrigin,
            LdaNumber = SpeciesSingle.LdaNumber,
            BasinLength = SpeciesSingle.BasinLength,
            FishLength = SpeciesSingle.FishLength,
            Karnivore = SpeciesSingle.Karnivore,
            Herbivore = SpeciesSingle.Herbivore,
            Limnivore = SpeciesSingle.Limnivore,
            Omnivore = SpeciesSingle.Omnivore,
            MemoFoods = SpeciesSingle.MemoFoods,
            Difficult1 = SpeciesSingle.Difficult1,
            Difficult2 = SpeciesSingle.Difficult2,
            Difficult3 = SpeciesSingle.Difficult3,
            Difficult4 = SpeciesSingle.Difficult4,
            RegionTop = SpeciesSingle.RegionTop,
            RegionMiddle = SpeciesSingle.RegionMiddle,
            RegionBottom = SpeciesSingle.RegionBottom,
            MemoRegion = SpeciesSingle.MemoRegion,
            MemoTech = SpeciesSingle.MemoTech,
            Ph1 = SpeciesSingle.Ph1,
            Ph2 = SpeciesSingle.Ph2,
            Temp1 = SpeciesSingle.Temp1,
            Temp2 = SpeciesSingle.Temp2,
            Hardness1 = SpeciesSingle.Hardness1,
            Hardness2 = SpeciesSingle.Hardness2,
            CarboHardness1 = SpeciesSingle.CarboHardness1,
            CarboHardness2 = SpeciesSingle.CarboHardness2,
            MemoHusbandry = SpeciesSingle.MemoHusbandry,
            MemoBuilt = SpeciesSingle.MemoBuilt,
            MemoColor = SpeciesSingle.MemoColor,
            MemoSozial = SpeciesSingle.MemoSozial,
            MemoDomorphism = SpeciesSingle.MemoDomorphism,
            MemoSpecial = SpeciesSingle.MemoSpecial,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now,
            Updater = Environment.UserName,
            UpdaterDate = DateTime.Now,
            MemoBreeding = SpeciesSingle.MemoBreeding,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl75Species> SpeciessesCollection { get; set; } = null!;
    #endregion "Collections"

}
