using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;


//    Tbl18SuperclassesDocumentDataSourceSkriptdatum:  16.06.2023  12:32    

namespace ATIS.Views.Report.Pdf.Tbl18Superclasses;

public abstract class Tbl18SuperclassesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl09Division _divisionSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl15Subdivision _subdivisionSingle = new();
    private static Tbl18Superclass _superclassSingle = new();


    #endregion [Private Data Members]      

    //    Part 1    

    public static Tbl18SuperclassesPdfModel GetSuperclassDetails(int superclassId)
    {
        #region [Private Data Members]

        var dataService = new DataService();
        //var context = new AtisDbContext();
        //var fishId = 0;
        //var plantId = 0;
        #endregion [Private Data Members]

        ClearAllCollections();

        //if (context.Tbl15Subdivisions != null)
        //{
        //    var plantaeRegnum = context.Tbl15Subdivisions.FirstOrDefault(e => e.SubdivisionName == "Plantae#Regnum#");
        //    if (plantaeRegnum != null)
        //    {
        //        plantId = plantaeRegnum.SubdivisionId;
        //    }
        //}

        //if (context.Tbl12Subphylums != null)
        //{
        //    var animaliaRegnum = context.Tbl12Subphylums.FirstOrDefault(e => e.SubphylumName == "Animalia#Regnum#");

        //    if (animaliaRegnum != null)
        //    {
        //        fishId = animaliaRegnum.SubphylumId;
        //    }
        //}

        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);

        ////_subdivisionSingle = dataService.GetSubdivisionSingleBySubdivisionIdAndHash(subdivisionId);
        ////_subphylumSingle = dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);

        //if (subphylumId == fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //    _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        //    _divisionSingle = dataService.GetDivisionSingleByDivisionIdAndHash(divisionId);
        //    _subdivisionSingle = dataService.GetSubdivisionSingleBySubdivisionIdAndHash(subdivisionId);

        //}
        //if (subdivisionId == plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

            _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
            _phylumSingle = dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);
            _subphylumSingle = dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);
        }

        //direct children
        ClassesCollection = dataService.CollClassesBySuperclassIdAndHash(superclassId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySuperclassId(superclassId);
        SourcesCollection = dataService.CollSourcesBySuperclassId(superclassId);
        AuthorsCollection = dataService.CollAuthorsBySuperclassId(superclassId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySuperclassId(superclassId);

        _superclassSingle = dataService.GetSuperclassSingleBySuperclassIdAndHash(superclassId);

        return new Tbl18SuperclassesPdfModel
        {
            SuperclassName = _superclassSingle.SuperclassName,
            CountId = _superclassSingle.CountId,
            Synonym = _superclassSingle.Synonym,
            Author = _superclassSingle.Author,
            AuthorYear = _superclassSingle.AuthorYear,
            Info = _superclassSingle.Info,
            Valid = _superclassSingle.Valid,
            ValidYear = _superclassSingle.ValidYear,
            EngName = _superclassSingle.EngName,
            GerName = _superclassSingle.GerName,
            FraName = _superclassSingle.FraName,
            PorName = _superclassSingle.PorName,
            Memo = _superclassSingle.Memo,
            WriterDate = _superclassSingle.WriterDate,
            UpdaterDate = _superclassSingle.UpdaterDate,
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            //DivisionTree = GenrateModels.GenDivisions.GenerateDivisions(_divisionSingle),
            //SubdivisionTree = GenrateModels.GenSubdivisions.GenerateSubdivisions(_subdivisionSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle)
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = null!;



    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;


    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;


    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;


    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _superclassSingle = new Tbl18Superclass();
        _subdivisionSingle = new Tbl15Subdivision();
        _divisionSingle = new Tbl09Division();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

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
