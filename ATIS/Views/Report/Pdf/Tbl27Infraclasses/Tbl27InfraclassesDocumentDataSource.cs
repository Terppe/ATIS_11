using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl27InfraclassesDocumentDataSourceSkriptdatum:  15.06.2024  18:32    

namespace ATIS.Views.Report.Pdf.Tbl27Infraclasses;

public abstract class Tbl27InfraclassesDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();
    private static Tbl24Subclass _subclassSingle = new();
    private static Tbl27Infraclass _infraclassSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl27InfraclassesPdfModel?> GetInfraclassDetailsAsync(int infraclassId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var infraclassSingle = await dataService.GetInfraclassByIdAsync(infraclassId);
        if (infraclassSingle == null)
            return null;

        _infraclassSingle = infraclassSingle;

        if (_infraclassSingle.SubclassId != 0)
            _subclassSingle = await dataService.GetSubclassByIdAsync(_infraclassSingle.SubclassId) ?? new Tbl24Subclass();

        if (_subclassSingle.ClassId != 0)
            _classSingle = await dataService.GetClassByIdAsync(_subclassSingle.ClassId) ?? new Tbl21Class();

        if (_classSingle.SuperclassId != 0)
            _superclassSingle = await dataService.GetSuperclassByIdAsync(_classSingle.SuperclassId) ?? new Tbl18Superclass();

        if (_superclassSingle.SubphylumId != 0)
            _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId) ?? new Tbl12Subphylum();

        if (_subphylumSingle.PhylumId != 0)
            _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId) ?? new Tbl06Phylum();

        if (_phylumSingle.RegnumId != 0)
            _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId) ?? new Tbl03Regnum();
        LegiosCollection = new ObservableCollection<Tbl30Legio>(
                    (await dataService.GetLegiosByInfraclassIdAsync(infraclassId))
                    .Where(x => IsVisibleTaxonName(x.LegioName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByInfraclassIdAsync(infraclassId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByInfraclassIdAsync(infraclassId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByInfraclassIdAsync(infraclassId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByInfraclassIdAsync(infraclassId));

        return new Tbl27InfraclassesPdfModel
        {
            InfraclassName = _infraclassSingle.InfraclassName,
            CountId = _infraclassSingle.CountId,
            Synonym = _infraclassSingle.Synonym,
            Author = _infraclassSingle.Author,
            AuthorYear = _infraclassSingle.AuthorYear,
            Info = _infraclassSingle.Info,
            Valid = _infraclassSingle.Valid,
            ValidYear = _infraclassSingle.ValidYear,
            EngName = _infraclassSingle.EngName,
            GerName = _infraclassSingle.GerName,
            FraName = _infraclassSingle.FraName,
            PorName = _infraclassSingle.PorName,
            Memo = _infraclassSingle.Memo,
            Writer = _infraclassSingle.Writer,
            WriterDate = _infraclassSingle.WriterDate,
            Updater = _infraclassSingle.Updater,
            UpdaterDate = _infraclassSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle),
            SubclassTree = GenrateModels.GenSubclasses.GenerateSubclasses(_subclassSingle),
            InfraclassTree = GenrateModels.GenInfraclasses.GenerateInfraclasses(_infraclassSingle),
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
    public static ObservableCollection<Tbl30Legio> LegiosCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"               

    public static void ClearAllCollections()
    {
        _infraclassSingle = new Tbl27Infraclass();
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
