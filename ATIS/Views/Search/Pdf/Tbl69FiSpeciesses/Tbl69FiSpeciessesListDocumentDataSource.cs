using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl69FiSpeciesses;

namespace ATIS.Views.Search.Pdf.Tbl69FiSpeciesses;
internal class Tbl69FiSpeciessesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl75Species FiSpeciesSingle = new();

    public static Tbl69FiSpeciessesPdfModel GetFiSpeciesListDetails(ObservableCollection<Tbl75Species> fispeciessesCollection)
    {

        FiSpeciessesCollection = fispeciessesCollection;

        return new Tbl69FiSpeciessesPdfModel
        {
            FiSpeciesName = FiSpeciesSingle.SpeciesName,
            Subspecies = FiSpeciesSingle.Subspecies,
            Divers = FiSpeciesSingle.Divers,
            CountId = FiSpeciesSingle.CountId,
            Author = FiSpeciesSingle.Author,
            AuthorYear = FiSpeciesSingle.AuthorYear,
            Valid = FiSpeciesSingle.Valid,
            ValidYear = FiSpeciesSingle.ValidYear,
            MemoSpecies = FiSpeciesSingle.MemoSpecies,
            TradeName = FiSpeciesSingle.TradeName,
            Importer = FiSpeciesSingle.Importer,
            ImportingYear = FiSpeciesSingle.ImportingYear,
            TypeSpecies = FiSpeciesSingle.TypeSpecies,
            LNumber = FiSpeciesSingle.LNumber,
            LOrigin = FiSpeciesSingle.LOrigin,
            LdaOrigin = FiSpeciesSingle.LdaOrigin,
            LdaNumber = FiSpeciesSingle.LdaNumber,
            BasinLength = FiSpeciesSingle.BasinLength,
            FishLength = FiSpeciesSingle.FishLength,
            Karnivore = FiSpeciesSingle.Karnivore,
            Herbivore = FiSpeciesSingle.Herbivore,
            Limnivore = FiSpeciesSingle.Limnivore,
            Omnivore = FiSpeciesSingle.Omnivore,
            MemoFoods = FiSpeciesSingle.MemoFoods,
            Difficult1 = FiSpeciesSingle.Difficult1,
            Difficult2 = FiSpeciesSingle.Difficult2,
            Difficult3 = FiSpeciesSingle.Difficult3,
            Difficult4 = FiSpeciesSingle.Difficult4,
            RegionTop = FiSpeciesSingle.RegionTop,
            RegionMiddle = FiSpeciesSingle.RegionMiddle,
            RegionBottom = FiSpeciesSingle.RegionBottom,
            MemoRegion = FiSpeciesSingle.MemoRegion,
            MemoTech = FiSpeciesSingle.MemoTech,
            Ph1 = FiSpeciesSingle.Ph1,
            Ph2 = FiSpeciesSingle.Ph2,
            Temp1 = FiSpeciesSingle.Temp1,
            Temp2 = FiSpeciesSingle.Temp2,
            Hardness1 = FiSpeciesSingle.Hardness1,
            Hardness2 = FiSpeciesSingle.Hardness2,
            CarboHardness1 = FiSpeciesSingle.CarboHardness1,
            CarboHardness2 = FiSpeciesSingle.CarboHardness2,
            MemoHusbandry = FiSpeciesSingle.MemoHusbandry,
            MemoBuilt = FiSpeciesSingle.MemoBuilt,
            MemoColor = FiSpeciesSingle.MemoColor,
            MemoSozial = FiSpeciesSingle.MemoSozial,
            MemoDomorphism = FiSpeciesSingle.MemoDomorphism,
            MemoSpecial = FiSpeciesSingle.MemoSpecial,
            Writer = Environment.UserName,
            WriterDate = DateTime.Now,
            Updater = Environment.UserName,
            UpdaterDate = DateTime.Now,
            MemoBreeding = FiSpeciesSingle.MemoBreeding,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl75Species> FiSpeciessesCollection { get; set; } = null!;
    #endregion "Collections"

}
