using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;


//    Tbl09DivisionsDocumentDataSourceSkriptdatum:  16.06.2023   12:32    

namespace ATIS.Views.Report.Pdf.Tbl09Divisions;

public abstract class Tbl09DivisionsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl09Division _divisionSingle = new();


    #endregion [Private Data Members]      


    //    Part 1    


    public static Tbl09DivisionsPdfModel GetDivisionDetails(int divisionId)

    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        _divisionSingle = dataService.GetDivisionSingleByDivisionIdAndHash(divisionId);

        //direct children
        SubdivisionsCollection = dataService.CollSubdivisionsByDivisionIdAndHash(divisionId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByDivisionId(divisionId);
        SourcesCollection = dataService.CollSourcesByDivisionId(divisionId);
        AuthorsCollection = dataService.CollAuthorsByDivisionId(divisionId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByDivisionId(divisionId);

        _divisionSingle = dataService.GetDivisionSingleByDivisionId(divisionId);
        _regnumSingle = dataService.GetRegnumSingleByRegnumId(regnumId);

        return new Tbl09DivisionsPdfModel
        {
            DivisionName = _divisionSingle.DivisionName,
            CountId = _divisionSingle.CountId,
            Synonym = _divisionSingle.Synonym,
            Author = _divisionSingle.Author,
            AuthorYear = _divisionSingle.AuthorYear,
            Info = _divisionSingle.Info,
            Valid = _divisionSingle.Valid,
            ValidYear = _divisionSingle.ValidYear,
            EngName = _divisionSingle.EngName,
            GerName = _divisionSingle.GerName,
            FraName = _divisionSingle.FraName,
            PorName = _divisionSingle.PorName,
            Memo = _divisionSingle.Memo,
            WriterDate = _divisionSingle.WriterDate,
            UpdaterDate = _divisionSingle.UpdaterDate,
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            DivisionTree = GenrateModels.GenDivisions.GenerateDivisions(_divisionSingle)
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _divisionSingle = new Tbl09Division();
        _regnumSingle = new Tbl03Regnum();

        SubdivisionsCollection?.Clear();
        DivisionsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}
