using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;


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


    public static async Task<Tbl21ClassesPdfModel> GetClassDetailsAsync(int classId)
    {
        #region [Private Data Members]

        var dataService = App.GetService<IDataService>();
        //var context = new AtisDbContext();
        //var fishId = 0;
        //var plantId = 0;

        #endregion [Private Data Members]

        ClearAllCollections();

        _classSingle = await dataService.GetClassByIdAsync(classId);
        var superclassId = _classSingle.SuperclassId;

        SuperclassesCollection = dataService.CollSuperclassesBySuperclassIdAndHash(superclassId);
        var subphylumId = dataService.SubphylumIdFromSuperclassesCollectionSelect(superclassId);
        //var subdivisionId = dataService.SubdivisionIdFromSuperclassesCollectionSelect(superclassId);
        //if (subphylumId == fishId)  //Basis #Subphylum#
        //{
        //    SubdivisionsCollection = dataService.CollSubdivisionsBySubdivisionIdAndHash(subdivisionId);
        //    var divisionId = dataService.DivisionIdFromSubdivisionsCollectionSelect(subdivisionId);
        //    DivisionsCollection = dataService.CollDivisionsByDivisionIdAndHash(divisionId);
        //    var regnumId = dataService.RegnumIdFromDivisionsCollectionSelect(divisionId);
        //    RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

        //    _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
        //    _divisionSingle = dataService.GetDivisionSingleByDivisionIdAndHash(divisionId);
        //    _subdivisionSingle = dataService.GetSubdivisionSingleBySubdivisionIdAndHash(subdivisionId);

        //}
        //if (subdivisionId == plantId)  //Basis #Subdivision#
        {
            SubphylumsCollection = dataService.CollSubphylumsBySubphylumIdAndHash(subphylumId);
            var phylumId = dataService.PhylumIdFromSubphylumsCollectionSelect(subphylumId);
            PhylumsCollection = dataService.CollPhylumsByPhylumIdAndHash(phylumId);
            var regnumId = dataService.RegnumIdFromPhylumsCollectionSelect(phylumId);
            RegnumsCollection = dataService.CollRegnumsByRegnumIdAndHash(regnumId);

            _regnumSingle = dataService.GetRegnumSingleByRegnumIdAndHash(regnumId);
            _phylumSingle = dataService.GetPhylumSingleByPhylumIdAndHash(phylumId);
            _subphylumSingle = dataService.GetSubphylumSingleBySubphylumIdAndHash(subphylumId);
        }

        //direct children
        SubclassesCollection = new ObservableCollection<Tbl24Subclass>(
            await dataService.GetSubclassesByClassIdAsync(classId));

        //-----------------------------------------------------------------------------
        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByClassIdAsync(classId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByClassIdAsync(classId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByClassIdAsync(classId));
        //------------------------------------------------------------------------------
        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByClassIdAsync(classId));

        _superclassSingle = dataService.GetSuperclassSingleBySuperclassIdAndHash(superclassId);

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
            //------------------------------- 

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle)
        };
    }

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
