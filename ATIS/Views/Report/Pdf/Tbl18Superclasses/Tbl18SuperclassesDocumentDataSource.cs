using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl18SuperclassesDocumentDataSourceSkriptdatum:  15.06.2024  12:32    

namespace ATIS.Views.Report.Pdf.Tbl18Superclasses;

public abstract class Tbl18SuperclassesDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl18SuperclassesPdfModel?> GetSuperclassDetailsAsync(int superclassId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var superclassSingle = await dataService.GetSuperclassByIdAsync(superclassId);
        if (superclassSingle == null)
            return null;

        _superclassSingle = superclassSingle;

        if (_superclassSingle?.SubphylumId != 0)
            _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId);

        if (_subphylumSingle?.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId);

        if (_phylumSingle?.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId);
        ClassesCollection = new ObservableCollection<Tbl21Class>(
                    (await dataService.GetClassesBySuperclassIdAsync(superclassId))
                    .Where(x => IsVisibleTaxonName(x.ClassName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesBySuperclassIdAsync(superclassId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesBySuperclassIdAsync(superclassId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesBySuperclassIdAsync(superclassId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsBySuperclassIdAsync(superclassId));

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
            Writer = _superclassSingle.Writer,
            WriterDate = _superclassSingle.WriterDate,
            Updater = _superclassSingle.Updater,
            UpdaterDate = _superclassSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
        };
    }

    #region "Collections"
    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"               

    public static void ClearAllCollections()
    {
        _superclassSingle = new Tbl18Superclass();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        ClassesCollection?.Clear();
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
