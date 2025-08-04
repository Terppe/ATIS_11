using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;

namespace ATIS.Views.Report.Pdf.Tbl63Infratribusses;
public class Tbl63InfratribussesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl09Division _divisionSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl15Subdivision _subdivisionSingle = new();
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
    #endregion [Private Data Members]      

    public static Tbl63InfratribussesPdfModel GetInfratribusDetails(int infratribusId)
    {
        #region [Private Data Members]

        var _dataService = new DataService();
        var _context = new AtisDbContext();
        int _mainId;
        var _fishId = 0;
        var _plantId = 0;

        #endregion [Private Data Members]

        ClearAllCollections();

        if (_context.Tbl15Subdivisions != null)
        {
            var plantaeRegnum = _context.Tbl15Subdivisions.FirstOrDefault(e => e.SubdivisionName == "Plantae#Regnum#");
            if (plantaeRegnum != null)
            {
                _plantId = plantaeRegnum.SubdivisionId;
            }
        }

        if (_context.Tbl12Subphylums != null)
        {
            var animaliaRegnum = _context.Tbl12Subphylums.FirstOrDefault(e => e.SubphylumName == "Animalia#Regnum#");

            if (animaliaRegnum != null)
            {
                _fishId = animaliaRegnum.SubphylumId;
            }
        }

        _mainId = infratribusId;

        InfratribussesCollection = _dataService.CollInfratribussesByInfratribusIdAndHash(infratribusId);
        var subtribusId = _dataService.SubtribusIdFromInfratribussesCollectionSelect(infratribusId);
        SubtribussesCollection = _dataService.CollSubtribussesBySubtribusIdAndHash(subtribusId);
        var tribusId = _dataService.TribusIdFromSubtribussesCollectionSelect(subtribusId);
        TribussesCollection = _dataService.CollTribussesByTribusIdAndHash(tribusId);
        var supertribusId = _dataService.SupertribusIdFromTribussesCollectionSelect(tribusId);
        SupertribussesCollection = _dataService.CollSupertribussesBySupertribusIdAndHash(supertribusId);
        var infrafamilyId = _dataService.InfrafamilyIdFromSupertribussesCollectionSelect(supertribusId);
        InfrafamiliesCollection = _dataService.CollInfrafamiliesByInfrafamilyIdAndHash(infrafamilyId);
        var subfamilyId = _dataService.SubfamilyIdFromInfrafamiliesCollectionSelect(infrafamilyId);
        SubfamiliesCollection = _dataService.CollSubfamiliesBySubfamilyIdAndHash(subfamilyId);
        var familyId = _dataService.FamilyIdFromSubfamiliesCollectionSelect(subfamilyId);
        FamiliesCollection = _dataService.CollFamiliesByFamilyIdAndHash(familyId);
        var superfamilyId = _dataService.SuperfamilyIdFromFamiliesCollectionSelect(familyId);
        SuperfamiliesCollection = _dataService.CollSuperfamiliesBySuperfamilyIdAndHash(superfamilyId);
        var infraordoId = _dataService.InfraordoIdFromSuperfamiliesCollectionSelect(superfamilyId);
        InfraordosCollection = _dataService.CollInfraordosByInfraordoIdAndHash(infraordoId);
        var subordoId = _dataService.SubordoIdFromInfraordosCollectionSelect(infraordoId);
        SubordosCollection = _dataService.CollSubordosBySubordoIdAndHash(subordoId);
        var ordoId = _dataService.OrdoIdFromSubordosCollectionSelect(subordoId);
        OrdosCollection = _dataService.CollOrdosByOrdoIdAndHash(ordoId);
        var legioId = _dataService.LegioIdFromOrdosCollectionSelect(ordoId);
        LegiosCollection = _dataService.CollLegiosByLegioIdAndHash(legioId);
        var infraclassId = _dataService.InfraclassIdFromLegiosCollectionSelect(legioId);
        InfraclassesCollection = _dataService.CollInfraclassesByInfraclassIdAndHash(infraclassId);
        var subclassId = _dataService.SubclassIdFromInfraclassesCollectionSelect(infraclassId);
        SubclassesCollection = _dataService.CollSubclassesBySubclassIdAndHash(subclassId);
        var classId = _dataService.ClassIdFromSubclassesCollectionSelect(subclassId);
        ClassesCollection = _dataService.CollClassesByClassIdAndHash(classId);
        var superclassId = _dataService.SuperclassIdFromClassesCollectionSelect(classId);
        SuperclassesCollection = _dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = _dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        var subdivisionId = _dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        if (subphylumId == _fishId)  //Basis #Subphylum#
        {
            SubdivisionsCollection = _dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
            var divisionId = _dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
            DivisionsCollection = _dataService.CollDivisionsByDivisionIdAndHash(divisionId);
            var regnumId = _dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

            _regnumSingle = _dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
            _divisionSingle = _dataService.GetDivisionSingleByDivisionIdAndHash(divisionId);
            _subdivisionSingle = _dataService.GetSubdivisionSingleBySubdivisionIdAndHash(subdivisionId);

        }
        if (subdivisionId == _plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = _dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = _dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = _dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = _dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = _dataService.CollRegnumsByRegnumIdAndHash(regnumId);

            _regnumSingle = _dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
            _phylumSingle = _dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);
            _subphylumSingle = _dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);
        }


        //direct children
        GenussesCollection = _dataService.CollGenussesByInfratribusIdAndHash(infratribusId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = _dataService.CollExpertsByGenusId(infratribusId);
        SourcesCollection = _dataService.CollSourcesByGenusId(infratribusId);
        AuthorsCollection = _dataService.CollAuthorsByGenusId(infratribusId);
        //------------------------------------------------------------------------------
        CommentsCollection = _dataService.CollCommentsByGenusId(infratribusId);


        _infratribusSingle = _dataService.GetInfratribusSingleByInfratribusIdAndHash(infratribusId);
        _subtribusSingle = _dataService.GetSubtribusSingleBySubtribusIdAndHash(subtribusId);
        _tribusSingle = _dataService.GetTribusSingleByTribusIdAndHash(tribusId);
        _supertribusSingle = _dataService.GetSupertribusSingleBySupertribusIdAndHash(supertribusId);
        _infrafamilySingle = _dataService.GetInfrafamilySingleByInfrafamilyIdAndHash(infrafamilyId);
        _subfamilySingle = _dataService.GetSubfamilySingleBySubfamilyIdAndHash(subfamilyId);
        _familySingle = _dataService.GetFamilySingleByFamilyIdAndHash(familyId);
        _superfamilySingle = _dataService.GetSuperfamilySingleBySuperfamilyIdAndHash(superfamilyId);
        _infraordoSingle = _dataService.GetInfraordoSingleByInfraordoIdAndHash(infraordoId);
        _subordoSingle = _dataService.GetSubordoSingleBySubordoIdAndHash(subordoId);
        _ordoSingle = _dataService.GetOrdoSingleByOrdoIdAndHash(ordoId);
        _legioSingle = _dataService.GetLegioSingleByLegioIdAndHash(legioId);
        _infraclassSingle = _dataService.GetInfraclassSingleByInfraclassIdAndHash(infraclassId);
        _subclassSingle = _dataService.GetSubclassSingleBySubclassIdAndHash(subclassId);
        _classSingle = _dataService.GetClassSingleByClassIdAndHash(classId);
        _superclassSingle = _dataService.GetSuperclassSingleBySuperclassIdAndHash(superclassId);


        return new Tbl63InfratribussesPdfModel
        {
            InfratribusName = _infratribusSingle.InfratribusName,
            CountId = _infratribusSingle.CountId,
            Synonym = _infratribusSingle.Synonym,
            Author = _infratribusSingle.Author,
            AuthorYear = _infratribusSingle.AuthorYear,
            Info = _infratribusSingle.Info,
            Valid = _infratribusSingle.Valid,
            ValidYear = _infratribusSingle.ValidYear,
            EngName = _infratribusSingle.EngName,
            GerName = _infratribusSingle.GerName,
            FraName = _infratribusSingle.FraName,
            PorName = _infratribusSingle.PorName,
            Memo = _infratribusSingle.Memo,
            WriterDate = _infratribusSingle.WriterDate,
            UpdaterDate = _infratribusSingle.UpdaterDate,
            //-------------------------------
            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            DivisionTree = GenrateModels.GenDivisions.GenerateDivisions(_divisionSingle),
            SubdivisionTree = GenrateModels.GenSubdivisions.GenerateSubdivisions(_subdivisionSingle),
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
        };
    }


    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl09Division> DivisionsCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl21Class> ClassesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl24Subclass> SubclassesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl27Infraclass> InfraclassesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl30Legio> LegiosCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl33Ordo> OrdosCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl36Subordo> SubordosCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl39Infraordo> InfraordosCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl45Family> FamiliesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl48Subfamily> SubfamiliesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl54Supertribus> SupertribussesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl57Tribus> TribussesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl60Subtribus> SubtribussesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl63Infratribus> InfratribussesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl66Genus> GenussesCollection
    {
        get; set;
    } = null!;




    public static ObservableCollection<Tbl90Reference> AuthorsCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection
    {
        get; set;
    } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection
    {
        get; set;
    } = null!;

    #endregion "Collections"

    public static void ClearAllCollections()
    {
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
        _subdivisionSingle = new Tbl15Subdivision();
        _divisionSingle = new Tbl09Division();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

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
        SubdivisionsCollection?.Clear();
        SubphylumsCollection?.Clear();
        DivisionsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}

