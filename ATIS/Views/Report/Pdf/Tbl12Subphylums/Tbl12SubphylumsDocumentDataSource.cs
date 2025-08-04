using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;


//    Tbl12SubphylumsDocumentDataSourceSkriptdatum:  16.06.2023  12:32    

namespace ATIS.Views.Report.Pdf.Tbl12Subphylums;

public abstract class Tbl12SubphylumsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();


    #endregion [Private Data Members]      


    //    Part 1    


    public static Tbl12SubphylumsPdfModel GetSubphylumDetails(int subphylumId)

    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
        var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
        PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
        var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        _phylumSingle = dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);
        _subphylumSingle = dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);

        //direct children
        SuperclassesCollection = dataService.CollSuperclassesBySubphylumIdAndHash(subphylumId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsBySubphylumId(subphylumId);
        SourcesCollection = dataService.CollSourcesBySubphylumId(subphylumId);
        AuthorsCollection = dataService.CollAuthorsBySubphylumId(subphylumId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsBySubphylumId(subphylumId);

        _subphylumSingle = dataService.GetSubphylumSingleBySubphylumId(subphylumId);
        _phylumSingle = dataService.GetPhylumSingleByPhylumId(phylumId);
        _regnumSingle = dataService.GetRegnumSingleByRegnumId(regnumId);

        return new Tbl12SubphylumsPdfModel
        {
            SubphylumName = _subphylumSingle.SubphylumName,
            CountId = _subphylumSingle.CountId,
            Synonym = _subphylumSingle.Synonym,
            Author = _subphylumSingle.Author,
            AuthorYear = _subphylumSingle.AuthorYear,
            Info = _subphylumSingle.Info,
            Valid = _subphylumSingle.Valid,
            ValidYear = _subphylumSingle.ValidYear,
            EngName = _subphylumSingle.EngName,
            GerName = _subphylumSingle.GerName,
            FraName = _subphylumSingle.FraName,
            PorName = _subphylumSingle.PorName,
            Memo = _subphylumSingle.Memo,
            WriterDate = _subphylumSingle.WriterDate,
            UpdaterDate = _subphylumSingle.UpdaterDate,
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        SuperclassesCollection?.Clear();
        SubphylumsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}
