using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;


//    Tbl06PhylumsDocumentDataSourceSkriptdatum:  16.06.2023  12:32    

namespace ATIS.Views.Report.Pdf.Tbl06Phylums;

public abstract class Tbl06PhylumsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();


    #endregion [Private Data Members]      

    //    Part 1    


    public static Tbl06PhylumsPdfModel GetPhylumDetails(int phylumId)

    {
        #region [Private Data Members]

        var dataService = App.GetService<IDataService>();

        #endregion [Private Data Members]

        ClearAllCollections();

        PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
        var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
        RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        _phylumSingle = dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);

        //direct children
        SubphylumsCollection = dataService.CollSubphylumsByPhylumIdAndHash(phylumId);

        //-----------------------------------------------------------------------------
        ExpertsCollection = dataService.CollExpertsByPhylumId(phylumId);
        SourcesCollection = dataService.CollSourcesByPhylumId(phylumId);
        AuthorsCollection = dataService.CollAuthorsByPhylumId(phylumId);
        //------------------------------------------------------------------------------
        CommentsCollection = dataService.CollCommentsByPhylumId(phylumId);

        _phylumSingle = dataService.GetPhylumSingleByPhylumId(phylumId);
        _regnumSingle = dataService.GetRegnumSingleByRegnumId(regnumId);

        return new Tbl06PhylumsPdfModel
        {
            PhylumName = _phylumSingle.PhylumName,
            CountId = _phylumSingle.CountId,
            Synonym = _phylumSingle.Synonym,
            Author = _phylumSingle.Author,
            AuthorYear = _phylumSingle.AuthorYear,
            Info = _phylumSingle.Info,
            Valid = _phylumSingle.Valid,
            ValidYear = _phylumSingle.ValidYear,
            EngName = _phylumSingle.EngName,
            GerName = _phylumSingle.GerName,
            FraName = _phylumSingle.FraName,
            PorName = _phylumSingle.PorName,
            Memo = _phylumSingle.Memo,
            WriterDate = _phylumSingle.WriterDate,
            UpdaterDate = _phylumSingle.UpdaterDate,
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
    #endregion "Collections"          

    public static void ClearAllCollections()
    {
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        SubphylumsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}
