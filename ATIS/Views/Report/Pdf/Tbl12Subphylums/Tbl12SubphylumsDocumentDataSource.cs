using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl12SubphylumsDocumentDataSourceSkriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Report.Pdf.Tbl12Subphylums;

public abstract class Tbl12SubphylumsDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl12SubphylumsPdfModel?> GetSubphylumDetailsAsync(int subphylumId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var subphylumSingle = await dataService.GetSubphylumByIdAsync(subphylumId);
        if (subphylumSingle == null)
            return null;

        _subphylumSingle = subphylumSingle;

        if (_subphylumSingle?.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId);

        if (_phylumSingle?.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId);
        SuperclassesCollection = new ObservableCollection<Tbl18Superclass>(
                    (await dataService.GetSuperclassesBySubphylumIdAsync(subphylumId))
                    .Where(x => IsVisibleTaxonName(x.SuperclassName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesBySubphylumIdAsync(subphylumId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesBySubphylumIdAsync(subphylumId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesBySubphylumIdAsync(subphylumId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsBySubphylumIdAsync(subphylumId));

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
            Writer = _subphylumSingle.Writer,
            WriterDate = _subphylumSingle.WriterDate,
            Updater = _subphylumSingle.Updater,
            UpdaterDate = _subphylumSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
        };
    }

    #region "Collections"
    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
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

        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
        CommentsCollection?.Clear();
    }
}
