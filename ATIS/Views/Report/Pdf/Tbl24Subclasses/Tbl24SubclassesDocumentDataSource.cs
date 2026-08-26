using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl24SubclassesDocumentDataSourceSkriptdatum:  15.08.2026  18:32    

namespace ATIS.Views.Report.Pdf.Tbl24Subclasses;

public abstract class Tbl24SubclassesDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();
    private static Tbl24Subclass _subclassSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl24SubclassesPdfModel?> GetSubclassDetailsAsync(int subclassId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var subclassSingle = await dataService.GetSubclassByIdAsync(subclassId);
        if (subclassSingle == null)
            return null;

        _subclassSingle = subclassSingle;

        if (_subclassSingle.ClassId != 0)
            _classSingle = await dataService.GetClassByIdAsync(_subclassSingle.ClassId) ?? new Tbl21Class();

        if (_classSingle.SuperclassId != 0)
            _superclassSingle = await dataService.GetSuperclassByIdAsync(_classSingle.SuperclassId) ??
                                new Tbl18Superclass();

        if (_superclassSingle.SubphylumId != 0)
            _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId) ??
                               new Tbl12Subphylum();

        if (_subphylumSingle.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId) ?? new Tbl06Phylum();

        if (_phylumSingle.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId) ?? new Tbl03Regnum();
        InfraclassesCollection = new ObservableCollection<Tbl27Infraclass>(
            (await dataService.GetInfraclassesBySubclassIdAsync(subclassId))
            .Where(x => IsVisibleTaxonName(x.InfraclassName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesBySubclassIdAsync(subclassId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesBySubclassIdAsync(subclassId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesBySubclassIdAsync(subclassId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsBySubclassIdAsync(subclassId));

        return new Tbl24SubclassesPdfModel
        {
            SubclassName = _subclassSingle.SubclassName,
            CountId = _subclassSingle.CountId,
            Synonym = _subclassSingle.Synonym,
            Author = _subclassSingle.Author,
            AuthorYear = _subclassSingle.AuthorYear,
            Info = _subclassSingle.Info,
            Valid = _subclassSingle.Valid,
            ValidYear = _subclassSingle.ValidYear,
            EngName = _subclassSingle.EngName,
            GerName = _subclassSingle.GerName,
            FraName = _subclassSingle.FraName,
            PorName = _subclassSingle.PorName,
            Memo = _subclassSingle.Memo,
            Writer = _subclassSingle.Writer,
            WriterDate = _subclassSingle.WriterDate,
            Updater = _subclassSingle.Updater,
            UpdaterDate = _subclassSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle),
            SubclassTree = GenrateModels.GenSubclasses.GenerateSubclasses(_subclassSingle),
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = new();
    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = new();
    public static ObservableCollection<Tbl27Infraclass> InfraclassesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();

    #endregion "Collections"

    public static void ClearAllCollections()
    {
        _subclassSingle = new Tbl24Subclass();
        _classSingle = new Tbl21Class();
        _superclassSingle = new Tbl18Superclass();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        InfraclassesCollection?.Clear();
        SubclassesCollection?.Clear();
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
