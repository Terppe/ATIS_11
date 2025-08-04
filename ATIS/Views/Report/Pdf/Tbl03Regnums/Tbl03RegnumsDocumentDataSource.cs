using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;

namespace ATIS.Views.Report.Pdf.Tbl03Regnums;
public class Tbl03RegnumsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static Tbl03Regnum _regnumSingle = new();
    #endregion [Private Data Members]      

    public static Tbl03RegnumsPdfModel GetRegnumDetails(int regnumId)
    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);

        //Children
        PhylumsCollection = dataService.CollPhylumsByRegnumIdAndHash(regnumId);
        DivisionsCollection = dataService.CollDivisionsByRegnumIdAndHash(regnumId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByRegnumId(regnumId);
        SourcesCollection = dataService.CollSourcesByRegnumId(regnumId);
        AuthorsCollection = dataService.CollAuthorsByRegnumId(regnumId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByRegnumId(regnumId);

 
        return new Tbl03RegnumsPdfModel
        {
            RegnumName = _regnumSingle.RegnumName,
            Subregnum = _regnumSingle.Subregnum,
            CountId = _regnumSingle.CountId,
            Synonym = _regnumSingle.Synonym,
            Author = _regnumSingle.Author,
            AuthorYear = _regnumSingle.AuthorYear,
            Info = _regnumSingle.Info,
            Valid = _regnumSingle.Valid,
            ValidYear = _regnumSingle.ValidYear,
            EngName = _regnumSingle.EngName,
            GerName = _regnumSingle.GerName,
            FraName = _regnumSingle.FraName,
            PorName = _regnumSingle.PorName,
            Memo = _regnumSingle.Memo,
            WriterDate = _regnumSingle.WriterDate,
            UpdaterDate = _regnumSingle.UpdaterDate,
            //-------------------------------
            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
        };

    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;


    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;


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
        _regnumSingle = new Tbl03Regnum();

        DivisionsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}

