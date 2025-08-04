using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;


//    Tbl15SubdivisionsDocumentDataSourceSkriptdatum:  16.06.2023  12:32    

namespace ATIS.Views.Report.Pdf.Tbl15Subdivisions;

public abstract class Tbl15SubdivisionsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl09Division _divisionSingle = new();
    private static Tbl15Subdivision _subdivisionSingle = new();


    #endregion [Private Data Members]      


    //    Part 1    


    public static Tbl15SubdivisionsPdfModel GetSubdivisionDetails(int subdivisionId)

    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        _divisionSingle = dataService.GetDivisionSingleByDivisionIdAndHash(divisionId);
        _subdivisionSingle = dataService.GetSubdivisionSingleBySubdivisionIdAndHash(subdivisionId);

        //direct children
        SuperclassesCollection = dataService.CollSuperclassesBySubdivisionIdAndHash(subdivisionId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubdivisionId(subdivisionId);
        SourcesCollection = dataService.CollSourcesBySubdivisionId(subdivisionId);
        AuthorsCollection = dataService.CollAuthorsBySubdivisionId(subdivisionId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubdivisionId(subdivisionId);

        _subdivisionSingle = dataService.GetSubdivisionSingleBySubdivisionId(subdivisionId);
        _divisionSingle = dataService.GetDivisionSingleByDivisionId(divisionId);
        _regnumSingle = dataService.GetRegnumSingleByRegnumId(regnumId);

        return new Tbl15SubdivisionsPdfModel
        {
            SubdivisionName = _subdivisionSingle.SubdivisionName,
            CountId = _subdivisionSingle.CountId,
            Synonym = _subdivisionSingle.Synonym,
            Author = _subdivisionSingle.Author,
            AuthorYear = _subdivisionSingle.AuthorYear,
            Info = _subdivisionSingle.Info,
            Valid = _subdivisionSingle.Valid,
            ValidYear = _subdivisionSingle.ValidYear,
            EngName = _subdivisionSingle.EngName,
            GerName = _subdivisionSingle.GerName,
            FraName = _subdivisionSingle.FraName,
            PorName = _subdivisionSingle.PorName,
            Memo = _subdivisionSingle.Memo,
            WriterDate = _subdivisionSingle.WriterDate,
            UpdaterDate = _subdivisionSingle.UpdaterDate,
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            DivisionTree = GenrateModels.GenDivisions.GenerateDivisions(_divisionSingle),
            SubdivisionTree = GenrateModels.GenSubdivisions.GenerateSubdivisions(_subdivisionSingle)
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;


    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _subdivisionSingle = new Tbl15Subdivision();
        _divisionSingle = new Tbl09Division();
        _regnumSingle = new Tbl03Regnum();

        SuperclassesCollection?.Clear();
        SubdivisionsCollection?.Clear();
        DivisionsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}
