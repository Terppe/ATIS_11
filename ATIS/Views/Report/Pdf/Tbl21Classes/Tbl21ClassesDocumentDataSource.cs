using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl21ClassesDocumentDataSourceSkriptdatum:  14.06.2023  18:32    

namespace ATIS.Views.Report.Pdf.Tbl21Classes;

public abstract class Tbl21ClassesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]   

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();

    #endregion [Private Data Members]      


    //    Part 1
    //
    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl21ClassesPdfModel?> GetClassDetailsAsync(int classId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var _classSingle = await dataService.GetClassByIdAsync(classId);

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

        System.Diagnostics.Debug.WriteLine($"Class: {_classSingle?.ClassName}");
        System.Diagnostics.Debug.WriteLine($"Superclass: {_superclassSingle?.SuperclassName}");
        System.Diagnostics.Debug.WriteLine($"Subphylum: {_subphylumSingle?.SubphylumName}");
        System.Diagnostics.Debug.WriteLine($"Phylum: {_phylumSingle?.PhylumName}");
        System.Diagnostics.Debug.WriteLine($"Regnum: {_regnumSingle?.RegnumName}");
        System.Diagnostics.Debug.WriteLine($"Subclasses count: {SubclassesCollection?.Count}");


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
            WriterDate = _classSingle.WriterDate,
            UpdaterDate = _classSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle)

        };
    }

    //public static async Task<Tbl21ClassesPdfModel> GetClassDetailsAsync(int classId)
    //{
    //    #region [Private Data Members]

    //    var dataService = App.GetService<IDataService>();

    //    #endregion [Private Data Members]

    //    ClearAllCollections();

    //    _classSingle = await dataService.GetClassByIdAsync(classId);

    //    if (_classSingle.SuperclassId != 0)
    //        _superclassSingle = await dataService.GetSuperclassByIdAsync(_classSingle.SuperclassId);

    //    if (_superclassSingle?.SubphylumId != 0)
    //        _subphylumSingle = await dataService.GetSubphylumByIdAsync(_superclassSingle.SubphylumId);

    //    if (_subphylumSingle?.PhylumId != 0)
    //        _phylumSingle = await dataService.GetPhylumByIdAsync(_subphylumSingle.PhylumId);

    //    if (_phylumSingle?.RegnumId != 0)
    //        _regnumSingle = await dataService.GetRegnumByIdAsync(_phylumSingle.RegnumId);

    //    //direct children
    //    SubclassesCollection = new ObservableCollection<Tbl24Subclass>(
    //        await dataService.GetSubclassesByClassIdAsync(classId));

    //    //-----------------------------------------------------------------------------
    //    ExpertsCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetExpertReferencesByClassIdAsync(classId));

    //    SourcesCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetSourceReferencesByClassIdAsync(classId));

    //    AuthorsCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetAuthorReferencesByClassIdAsync(classId));
    //    //------------------------------------------------------------------------------
    //    CommentsCollection = new ObservableCollection<Tbl93Comment>(
    //        await dataService.GetCommentsByClassIdAsync(classId));

    //    return new Tbl21ClassesPdfModel
    //    {
    //        ClassName = _classSingle.ClassName,
    //        CountId = _classSingle.CountId,
    //        Synonym = _classSingle.Synonym,
    //        Author = _classSingle.Author,
    //        AuthorYear = _classSingle.AuthorYear,
    //        Info = _classSingle.Info,
    //        Valid = _classSingle.Valid,
    //        ValidYear = _classSingle.ValidYear,
    //        EngName = _classSingle.EngName,
    //        GerName = _classSingle.GerName,
    //        FraName = _classSingle.FraName,
    //        PorName = _classSingle.PorName,
    //        Memo = _classSingle.Memo,
    //        WriterDate = _classSingle.WriterDate,
    //        UpdaterDate = _classSingle.UpdaterDate,
    //        //------------------------------- 

    //        RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
    //        PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
    //        SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
    //        SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
    //        ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle)
    //    };
    //}

    //public static async Task<Tbl21ClassesPdfModel> GetClassDetailsAsync(int classId)
    //{
    //    #region [Private Data Members]

    //    var dataService = App.GetService<IDataService>();

    //    #endregion [Private Data Members]

    //    ClearAllCollections();

    //    _classSingle = await dataService.GetClassByIdAsync(classId);
    //    var superclassId = _classSingle.SuperclassId;
    //    SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
    //    var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
    //    SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
    //    var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
    //    PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
    //    var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
    //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

    //    _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
    //    _phylumSingle = dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);
    //    _subphylumSingle = dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);

    //    //direct children
    //    SubclassesCollection = new ObservableCollection<Tbl24Subclass>(
    //        await dataService.GetSubclassesByClassIdAsync(classId));

    //    //-----------------------------------------------------------------------------
    //    ExpertsCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetExpertReferencesByClassIdAsync(classId));

    //    SourcesCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetSourceReferencesByClassIdAsync(classId));

    //    AuthorsCollection = new ObservableCollection<Tbl90Reference>(
    //        await dataService.GetAuthorReferencesByClassIdAsync(classId));
    //    //------------------------------------------------------------------------------
    //    CommentsCollection = new ObservableCollection<Tbl93Comment>(
    //        await dataService.GetCommentsByClassIdAsync(classId));

    //    _superclassSingle = dataService.GetSuperclassSingleBySuperclassIdAndHash(superclassId);

    //    return new Tbl21ClassesPdfModel
    //    {
    //        ClassName = _classSingle.ClassName,
    //        CountId = _classSingle.CountId,
    //        Synonym = _classSingle.Synonym,
    //        Author = _classSingle.Author,
    //        AuthorYear = _classSingle.AuthorYear,
    //        Info = _classSingle.Info,
    //        Valid = _classSingle.Valid,
    //        ValidYear = _classSingle.ValidYear,
    //        EngName = _classSingle.EngName,
    //        GerName = _classSingle.GerName,
    //        FraName = _classSingle.FraName,
    //        PorName = _classSingle.PorName,
    //        Memo = _classSingle.Memo,
    //        WriterDate = _classSingle.WriterDate,
    //        UpdaterDate = _classSingle.UpdaterDate,
    //        //------------------------------- 

    //        RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
    //        PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
    //        SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
    //        SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
    //        ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle)
    //    };
    //}

    #region "Collections"
    public static ObservableCollection<Tbl03Regnum> RegnumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = null!;

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = null!;

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = null!;
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
        SubdivisionsCollection?.Clear();
        SubphylumsCollection?.Clear();
        DivisionsCollection?.Clear();
        PhylumsCollection?.Clear();
        RegnumsCollection?.Clear();

        CommentsCollection?.Clear();
        ExpertsCollection?.Clear();
        SourcesCollection?.Clear();
        AuthorsCollection?.Clear();
    }

}
