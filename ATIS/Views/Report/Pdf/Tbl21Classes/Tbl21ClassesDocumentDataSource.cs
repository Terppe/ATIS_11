using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl21ClassesDocumentDataSourceSkriptdatum:  26.08.2026   18:32    

namespace ATIS.Views.Report.Pdf.Tbl21Classes;

public abstract class Tbl21ClassesDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl21ClassesPdfModel?> GetClassDetailsAsync(int classId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var classSingle = await dataService.GetClassByIdAsync(classId);
        if (classSingle == null)
            return null;

        _classSingle = classSingle;

        if (_classSingle.SuperclassId != 0)
            _superclassSingle = await dataService.GetSuperclassByIdAsync(_classSingle.SuperclassId);

        if (_superclassSingle?.SubphylumId != 0)
            _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId);

        if (_subphylumSingle?.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId);

        if (_phylumSingle?.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId);
        SubclassesCollection = new ObservableCollection<Tbl24Subclass>(
                    (await dataService.GetSubclassesByClassIdAsync(classId))
                    .Where(x => IsVisibleTaxonName(x.SubclassName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByClassIdAsync(classId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByClassIdAsync(classId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByClassIdAsync(classId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByClassIdAsync(classId));

        return new Tbl21ClassesPdfModel
        {
            ClassName = _classSingle.ClassName,
            CountId = _classSingle.CountId,
            Synonym = _classSingle.Synonym,
            Author = _classSingle.Author,
            AuthorYear = _classSingle.AuthorYear,
            Info = _classSingle.Info,
            Valid = _classSingle.Valid,
            ValidYear = _classSingle.ValidYear,
            EngName = _classSingle.EngName,
            GerName = _classSingle.GerName,
            FraName = _classSingle.FraName,
            PorName = _classSingle.PorName,
            Memo = _classSingle.Memo,
            Writer = _classSingle.Writer,
            WriterDate = _classSingle.WriterDate,
            Updater = _classSingle.Updater,
            UpdaterDate = _classSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle)
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
        _classSingle = new Tbl21Class();
        _superclassSingle = new Tbl18Superclass();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

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
