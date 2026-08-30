using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;

namespace ATIS.Views.Report.Pdf.Tbl75SpeciesSubs;

public abstract class Tbl75SpeciesSubsDocumentDataSource : ObservableObject
{
    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();
    private static Tbl24Subclass _subclassSingle = new();
    private static Tbl27Infraclass _infraclassSingle = new();
    private static Tbl30Legio _legioSingle = new();
    private static Tbl33Ordo _ordoSingle = new();
    private static Tbl36Subordo _subordoSingle = new();
    private static Tbl39Infraordo _infraordoSingle = new();
    private static Tbl42Superfamily _superfamilySingle = new();
    private static Tbl45Family _familySingle = new();
    private static Tbl48Subfamily _subfamilySingle = new();
    private static Tbl51Infrafamily _infrafamilySingle = new();
    private static Tbl54Supertribus _supertribusSingle = new();
    private static Tbl57Tribus _tribusSingle = new();
    private static Tbl60Subtribus _subtribusSingle = new();
    private static Tbl63Infratribus _infratribusSingle = new();
    private static Tbl66Genus _genusSingle = new();
    private static Tbl75Species _speciesSingle = new();
    private static readonly Tbl78Name _nameSingle = new();
    private static readonly Tbl84Synonym _synonymSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl75SpeciesSubsPdfModel?> GetSpeciesSubDetailsAsync(int speciesId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var speciesSingle = await dataService.GetSpeciesByIdAsync(speciesId);
        if (speciesSingle == null)
            return null;

        _speciesSingle = speciesSingle;

        if (_speciesSingle.GenusId != 0)
            _genusSingle = await dataService.GetGenusByIdAsync(_speciesSingle.GenusId) ?? new Tbl66Genus();

        if (_genusSingle.InfratribusId != 0)
            _infratribusSingle = await dataService.GetInfratribusByIdAsync(_genusSingle.InfratribusId) ?? new Tbl63Infratribus();

        if (_infratribusSingle.SubtribusId != 0)
            _subtribusSingle = await dataService.GetSubtribusByIdAsync(_infratribusSingle.SubtribusId) ?? new Tbl60Subtribus();

        if (_subtribusSingle.TribusId != 0)
            _tribusSingle = await dataService.GetTribusByIdAsync(_subtribusSingle.TribusId) ?? new Tbl57Tribus();

        if (_tribusSingle.SupertribusId != 0)
            _supertribusSingle = await dataService.GetSupertribusByIdAsync(_tribusSingle.SupertribusId) ?? new Tbl54Supertribus();

        if (_supertribusSingle.InfrafamilyId != 0)
            _infrafamilySingle = await dataService.GetInfrafamilyByIdAsync(_supertribusSingle.InfrafamilyId) ?? new Tbl51Infrafamily();

        if (_infrafamilySingle.SubfamilyId != 0)
            _subfamilySingle = await dataService.GetSubfamilyByIdAsync(_infrafamilySingle.SubfamilyId) ?? new Tbl48Subfamily();

        if (_subfamilySingle.FamilyId != 0)
            _familySingle = await dataService.GetFamilyByIdAsync(_subfamilySingle.FamilyId) ?? new Tbl45Family();

        if (_familySingle.SuperfamilyId != 0)
            _superfamilySingle = await dataService.GetSuperfamilyByIdAsync(_familySingle.SuperfamilyId) ?? new Tbl42Superfamily();

        if (_superfamilySingle.InfraordoId != 0)
            _infraordoSingle = await dataService.GetInfraordoByIdAsync(_superfamilySingle.InfraordoId) ?? new Tbl39Infraordo();

        if (_infraordoSingle.SubordoId != 0)
            _subordoSingle = await dataService.GetSubordoByIdAsync(_infraordoSingle.SubordoId) ?? new Tbl36Subordo();

        if (_subordoSingle.OrdoId != 0)
            _ordoSingle = await dataService.GetOrdoByIdAsync(_subordoSingle.OrdoId) ?? new Tbl33Ordo();

        if (_ordoSingle.LegioId != 0)
            _legioSingle = await dataService.GetLegioByIdAsync(_ordoSingle.LegioId) ?? new Tbl30Legio();

        if (_legioSingle.InfraclassId != 0)
            _infraclassSingle = await dataService.GetInfraclassByIdAsync(_legioSingle.InfraclassId) ?? new Tbl27Infraclass();

        if (_infraclassSingle.SubclassId != 0)
            _subclassSingle = await dataService.GetSubclassByIdAsync(_infraclassSingle.SubclassId) ?? new Tbl24Subclass();

        if (_subclassSingle.ClassId != 0)
            _classSingle = await dataService.GetClassByIdAsync(_subclassSingle.ClassId) ?? new Tbl21Class();

        if (_classSingle.SuperclassId != 0)
            _superclassSingle = await dataService.GetSuperclassByIdAsync(_classSingle.SuperclassId) ?? new Tbl18Superclass();

        if (_superclassSingle.SubphylumId != 0)
            _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId) ?? new Tbl12Subphylum();

        if (_subphylumSingle.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId) ?? new Tbl06Phylum();

        if (_phylumSingle.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId) ?? new Tbl03Regnum();

        NamesCollection = new ObservableCollection<Tbl78Name>(
            (await dataService.GetNamesBySpeciesIdAsync(speciesId))
            .Where(x => IsVisibleTaxonName(x.NameName)));

        //ImagesCollection = new ObservableCollection<Tbl81Image>(
        //    (await dataService.GetImagesBySpeciesIdAsync(speciesId))
        //    .Where(x => IsVisibleTaxonName(x.Info)));

        SynonymsCollection = new ObservableCollection<Tbl84Synonym>(
            (await dataService.GetSynonymsBySpeciesIdAsync(speciesId))
            .Where(x => IsVisibleTaxonName(x.SynonymName)));

        //GeographicsCollection = new ObservableCollection<Tbl87Geographic>(
        //    (await dataService.GetGeographicsBySpeciesIdAsync(speciesId))
        //    .Where(x => IsVisibleTaxonName(x.Info)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesBySpeciesIdAsync(speciesId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesBySpeciesIdAsync(speciesId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesBySpeciesIdAsync(speciesId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsBySpeciesIdAsync(speciesId));

        return new Tbl75SpeciesSubsPdfModel
        {
            SpeciesName = _speciesSingle.SpeciesName,
            Subspecies = _speciesSingle.Subspecies,
            Divers = _speciesSingle.Divers,
            GenusId = _speciesSingle.GenusId,
            SpeciesgroupId = _speciesSingle.SpeciesgroupId,
            CountId = _speciesSingle.CountId,
            Valid = _speciesSingle.Valid,
            ValidYear = _speciesSingle.ValidYear,
            MemoSpecies = _speciesSingle.MemoSpecies,
            TradeName = _speciesSingle.TradeName,
            Author = _speciesSingle.Author,
            AuthorYear = _speciesSingle.AuthorYear,
            Importer = _speciesSingle.Importer,
            ImportingYear = _speciesSingle.ImportingYear,
            TypeSpecies = _speciesSingle.TypeSpecies,
            LNumber = _speciesSingle.LNumber,
            LOrigin = _speciesSingle.LOrigin,
            LdaOrigin = _speciesSingle.LdaOrigin,
            LdaNumber = _speciesSingle.LdaNumber,
            BasinLength = _speciesSingle.BasinLength,
            FishLength = _speciesSingle.FishLength,
            Karnivore = _speciesSingle.Karnivore,
            Herbivore = _speciesSingle.Herbivore,
            Limnivore = _speciesSingle.Limnivore,
            Omnivore = _speciesSingle.Omnivore,
            MemoFoods = _speciesSingle.MemoFoods,
            Difficult1 = _speciesSingle.Difficult1,
            Difficult2 = _speciesSingle.Difficult2,
            Difficult3 = _speciesSingle.Difficult3,
            Difficult4 = _speciesSingle.Difficult4,
            RegionTop = _speciesSingle.RegionTop,
            RegionMiddle = _speciesSingle.RegionMiddle,
            RegionBottom = _speciesSingle.RegionBottom,
            MemoRegion = _speciesSingle.MemoRegion,
            MemoTech = _speciesSingle.MemoTech,
            Ph1 = _speciesSingle.Ph1,
            Ph2 = _speciesSingle.Ph2,
            Temp1 = _speciesSingle.Temp1,
            Temp2 = _speciesSingle.Temp2,
            Hardness1 = _speciesSingle.Hardness1,
            Hardness2 = _speciesSingle.Hardness2,
            CarboHardness1 = _speciesSingle.CarboHardness1,
            CarboHardness2 = _speciesSingle.CarboHardness2,
            MemoHusbandry = _speciesSingle.MemoHusbandry,
            MemoBuilt = _speciesSingle.MemoBuilt,
            MemoColor = _speciesSingle.MemoColor,
            MemoSozial = _speciesSingle.MemoSozial,
            MemoDomorphism = _speciesSingle.MemoDomorphism,
            MemoSpecial = _speciesSingle.MemoSpecial,
            WriterDate = _speciesSingle.WriterDate,
            UpdaterDate = _speciesSingle.UpdaterDate,
            MemoBreeding = _speciesSingle.MemoBreeding,
            //plant
            BasinHeight = _speciesSingle.BasinHeight,
            PlantLength = _speciesSingle.PlantLength,
            MemoReproduction = _speciesSingle.MemoReproduction,
            MemoCulture = _speciesSingle.MemoCulture,
            MemoGlobal = _speciesSingle.MemoGlobal,
            Creature = _speciesSingle.Creature,
            //-------------------------------
            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle),
            SubclassTree = GenrateModels.GenSubclasses.GenerateSubclasses(_subclassSingle),
            InfraclassTree = GenrateModels.GenInfraclasses.GenerateInfraclasses(_infraclassSingle),
            LegioTree = GenrateModels.GenLegios.GenerateLegios(_legioSingle),
            OrdoTree = GenrateModels.GenOrdos.GenerateOrdos(_ordoSingle),
            SubordoTree = GenrateModels.GenSubordos.GenerateSubordos(_subordoSingle),
            InfraordoTree = GenrateModels.GenInfraordos.GenerateInfraordos(_infraordoSingle),
            SuperfamilyTree = GenrateModels.GenSuperfamilies.GenerateSuperfamilies(_superfamilySingle),
            FamilyTree = GenrateModels.GenFamilies.GenerateFamilies(_familySingle),
            SubfamilyTree = GenrateModels.GenSubfamilies.GenerateSubfamilies(_subfamilySingle),
            InfrafamilyTree = GenrateModels.GenInfrafamilies.GenerateInfrafamilies(_infrafamilySingle),
            SupertribusTree = GenrateModels.GenSupertribusses.GenerateSupertribusses(_supertribusSingle),
            TribusTree = GenrateModels.GenTribusses.GenerateTribusses(_tribusSingle),
            SubtribusTree = GenrateModels.GenSubtribusses.GenerateSubtribusses(_subtribusSingle),
            InfratribusTree = GenrateModels.GenInfratribusses.GenerateInfratribusses(_infratribusSingle),
            GenusTree = GenrateModels.GenGenusses.GenerateGenusses(_genusSingle),
            SpeciesTree = GenrateModels.GenSpeciesses.GenerateSpeciesses(_speciesSingle),
            SynonymsTree = GenrateModels.GenSynonyms.GenerateSynonyms(_synonymSingle),
            NamesTree = GenrateModels.GenNames.GenerateNames(_nameSingle),
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl27Infraclass> InfraclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl30Legio> LegiosCollection { get; set; } = new();
    public static ObservableCollection<Tbl33Ordo> OrdosCollection { get; set; } = new();
    public static ObservableCollection<Tbl36Subordo> SubordosCollection { get; set; } = new();
    public static ObservableCollection<Tbl39Infraordo> InfraordosCollection { get; set; } = new();
    public static ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl45Family> FamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl48Subfamily> SubfamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl54Supertribus> SupertribussesCollection { get; set; } = new();
    public static ObservableCollection<Tbl57Tribus> TribussesCollection { get; set; } = new();
    public static ObservableCollection<Tbl60Subtribus> SubtribussesCollection { get; set; } = new();
    public static ObservableCollection<Tbl63Infratribus> InfratribussesCollection { get; set; } = new();
    public static ObservableCollection<Tbl66Genus> GenussesCollection { get; set; } = new();
    public static ObservableCollection<Tbl75Species> SpeciessesCollection { get; set; } = new();
    public static ObservableCollection<Tbl75Species> SpeciesSubsCollection { get; set; } = new();
    public static ObservableCollection<Tbl78Name> NamesCollection { get; set; } = new();
    public static ObservableCollection<Tbl81Image> ImagesCollection { get; set; } = new();
    public static ObservableCollection<Tbl84Synonym> SynonymsCollection { get; set; } = new();

    public static ObservableCollection<Tbl87Geographic> GeographicsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _speciesSingle = new Tbl75Species();
        _genusSingle = new Tbl66Genus();
        _infratribusSingle = new Tbl63Infratribus();
        _subtribusSingle = new Tbl60Subtribus();
        _tribusSingle = new Tbl57Tribus();
        _supertribusSingle = new Tbl54Supertribus();
        _infrafamilySingle = new Tbl51Infrafamily();
        _subfamilySingle = new Tbl48Subfamily();
        _familySingle = new Tbl45Family();
        _superfamilySingle = new Tbl42Superfamily();
        _infraordoSingle = new Tbl39Infraordo();
        _subordoSingle = new Tbl36Subordo();
        _ordoSingle = new Tbl33Ordo();
        _legioSingle = new Tbl30Legio();
        _infraclassSingle = new Tbl27Infraclass();
        _subclassSingle = new Tbl24Subclass();
        _classSingle = new Tbl21Class();
        _superclassSingle = new Tbl18Superclass();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        SpeciessesCollection?.Clear();
        GenussesCollection?.Clear();
        InfratribussesCollection?.Clear();
        SubtribussesCollection?.Clear();
        TribussesCollection?.Clear();
        SupertribussesCollection?.Clear();
        InfrafamiliesCollection?.Clear();
        SubfamiliesCollection?.Clear();
        FamiliesCollection?.Clear();
        SuperfamiliesCollection?.Clear();
        InfraordosCollection?.Clear();
        SubordosCollection?.Clear();
        OrdosCollection?.Clear();
        LegiosCollection?.Clear();
        InfraclassesCollection?.Clear();
        SubclassesCollection?.Clear();
        ClassesCollection?.Clear();
        SuperclassesCollection?.Clear();
        SubphylumsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        GeographicsCollection?.Clear();
        SynonymsCollection?.Clear();
        ImagesCollection?.Clear();
        NamesCollection?.Clear();

        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
        CommentsCollection?.Clear();
    }
}
