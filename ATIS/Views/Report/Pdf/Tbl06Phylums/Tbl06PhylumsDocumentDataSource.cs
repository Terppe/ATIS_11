using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl06PhylumsDocumentDataSourceSkriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Report.Pdf.Tbl06Phylums;

public abstract class Tbl06PhylumsDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl06PhylumsPdfModel?> GetPhylumDetailsAsync(int phylumId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var phylumSingle = await dataService.GetPhylumByIdAsync(phylumId);
        if (phylumSingle == null)
            return null;

        _phylumSingle = phylumSingle;

        if (_phylumSingle?.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId);
        SubphylumsCollection = new ObservableCollection<Tbl12Subphylum>(
                    (await dataService.GetSubphylumsByPhylumIdAsync(phylumId))
                    .Where(x => IsVisibleTaxonName(x.SubphylumName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByPhylumIdAsync(phylumId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByPhylumIdAsync(phylumId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByPhylumIdAsync(phylumId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByPhylumIdAsync(phylumId));

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
            Writer = _phylumSingle.Writer,
            WriterDate = _phylumSingle.WriterDate,
            Updater = _phylumSingle.Updater,
            UpdaterDate = _phylumSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
        };
    }

    #region "Collections"
    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"               

    public static void ClearAllCollections()
    {
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        SubphylumsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
        CommentsCollection?.Clear();
    }
}
