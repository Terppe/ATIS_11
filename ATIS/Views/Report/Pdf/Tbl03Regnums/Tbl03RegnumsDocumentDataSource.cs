using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl66Genusses;


//    Tbl03RegnumsDocumentDataSourceSkriptdatum:  30.12.2024  12:32      

namespace ATIS.Views.Report.Pdf.Tbl03Regnums;

public abstract class Tbl03RegnumsDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }


    public static async Task<Tbl03RegnumsPdfModel?> GetRegnumDetailsAsync(int regnumId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var regnumSingle = await dataService.GetRegnumByIdAsync(regnumId);
        if (regnumSingle == null)
            return null;

        _regnumSingle = regnumSingle;


        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByRegnumIdAsync(regnumId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByRegnumIdAsync(regnumId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByRegnumIdAsync(regnumId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByRegnumIdAsync(regnumId));


        return new Tbl03RegnumsPdfModel
        {
            RegnumName = _regnumSingle.RegnumName,
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
            Writer = _regnumSingle.Writer,
            WriterDate = _regnumSingle.WriterDate,
            Updater = _regnumSingle.Updater,
            UpdaterDate = _regnumSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
        };
    }


    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = new();

    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = new();

    public static ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = new();


    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();

    #endregion "Collections"

    public static void ClearAllCollections()
    {
        _regnumSingle = new Tbl03Regnum();

        SubclassesCollection?.Clear();
        ClassesCollection?.Clear();
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
