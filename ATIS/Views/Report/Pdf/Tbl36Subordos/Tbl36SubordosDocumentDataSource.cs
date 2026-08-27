using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl36SubordosDocumentDataSourceSkriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Report.Pdf.Tbl36Subordos;

public abstract class Tbl36SubordosDocumentDataSource : ObservableObject
{

    private static Tbl03Regnum _regnumSingle = new();
    private static Tbl06Phylum _phylumSingle = new();
    private static Tbl12Subphylum _subphylumSingle = new();
    private static Tbl18Superclass _superclassSingle = new();
    private static Tbl21Class _classSingle = new();
    private static Tbl24Subclass _subclassSingle = new();
    private static Tbl27Infraclass _infraclassSingle = new();
    private static Tbl30Legio _legioSingle = new();
    private static Tbl33Ordo _ordoSingle = new();
    private static Tbl36Subordo _subordoSingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl36SubordosPdfModel?> GetSubordoDetailsAsync(int subordoId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var subordoSingle = await dataService.GetSubordoByIdAsync(subordoId);
        if (subordoSingle == null)
            return null;

        _subordoSingle = subordoSingle;

        if (_subordoSingle.OrdoId != 0)
            _ordoSingle = await dataService.GetOrdoByIdAsync(_subordoSingle.OrdoId) ?? new Tbl33Ordo();

        if (_ordoSingle.LegioId != 0)
            _legioSingle = await dataService.GetLegioByIdAsync(_ordoSingle.LegioId) ?? new Tbl30Legio();

        if (_legioSingle.InfraclassId != 0)
            _infraclassSingle = await dataService.GetInfraclassByIdAsync(_legioSingle.InfraclassId) ?? new Tbl27Infraclass();

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
        InfraordosCollection = new ObservableCollection<Tbl39Infraordo>(
                    (await dataService.GetInfraordosBySubordoIdAsync(subordoId))
                    .Where(x => IsVisibleTaxonName(x.InfraordoName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesBySubordoIdAsync(subordoId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesBySubordoIdAsync(subordoId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesBySubordoIdAsync(subordoId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsBySubordoIdAsync(subordoId));

        return new Tbl36SubordosPdfModel
        {
            SubordoName = _subordoSingle.SubordoName,
            CountId = _subordoSingle.CountId,
            Synonym = _subordoSingle.Synonym,
            Author = _subordoSingle.Author,
            AuthorYear = _subordoSingle.AuthorYear,
            Info = _subordoSingle.Info,
            Valid = _subordoSingle.Valid,
            ValidYear = _subordoSingle.ValidYear,
            EngName = _subordoSingle.EngName,
            GerName = _subordoSingle.GerName,
            FraName = _subordoSingle.FraName,
            PorName = _subordoSingle.PorName,
            Memo = _subordoSingle.Memo,
            Writer = _subordoSingle.Writer,
            WriterDate = _subordoSingle.WriterDate,
            Updater = _subordoSingle.Updater,
            UpdaterDate = _subordoSingle.UpdaterDate,

            RegnumTree = GenrateModels.GenRegnums.GenerateRegnums(_regnumSingle),
            PhylumTree = GenrateModels.GenPhylums.GeneratePhylums(_phylumSingle),
            SubphylumTree = GenrateModels.GenSubphylums.GenerateSubphylums(_subphylumSingle),
            SuperclassTree = GenrateModels.GenSuperclasses.GenerateSuperclasses(_superclassSingle),
            ClassTree = GenrateModels.GenClasses.GenerateClasses(_classSingle),
            SubclassTree = GenrateModels.GenSubclasses.GenerateSubclasses(_subclassSingle),
            InfraclassTree = GenrateModels.GenInfraclasses.GenerateInfraclasses(_infraclassSingle),
            LegioTree = GenrateModels.GenLegios.GenerateLegios(_legioSingle),
            OrdoTree = GenrateModels.GenOrdos.GenerateOrdos(_ordoSingle),
            SubordoTree = GenrateModels.GenSubordos.GenerateSubordos(_subordoSingle),
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
    public static ObservableCollection<Tbl33Ordo> OrdosCollection { get; set; } = new();
    public static ObservableCollection<Tbl36Subordo> SubordosCollection { get; set; } = new();
    public static ObservableCollection<Tbl39Infraordo> InfraordosCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"               

    public static void ClearAllCollections()
    {
        _subordoSingle = new Tbl36Subordo();
        _ordoSingle = new Tbl33Ordo();
        _legioSingle = new Tbl30Legio();
        _infraclassSingle = new Tbl27Infraclass();
        _subclassSingle = new Tbl24Subclass();
        _classSingle = new Tbl21Class();
        _superclassSingle = new Tbl18Superclass();
        _subphylumSingle = new Tbl12Subphylum();
        _phylumSingle = new Tbl06Phylum();
        _regnumSingle = new Tbl03Regnum();

        SubordosCollection?.Clear();
        OrdosCollection?.Clear();
        LegiosCollection?.Clear();
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
