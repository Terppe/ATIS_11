using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Models;


//    Tbl51InfrafamiliesDocumentDataSourceSkriptdatum:  18.08.2026  10:32    

namespace ATIS.Views.Report.Pdf.Tbl51Infrafamilies;

public abstract class Tbl51InfrafamiliesDocumentDataSource : ObservableObject
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
    private static Tbl39Infraordo _infraordoSingle = new();
    private static Tbl42Superfamily _superfamilySingle = new();
    private static Tbl45Family _familySingle = new();
    private static Tbl48Subfamily _subfamilySingle = new();
    private static Tbl51Infrafamily _infrafamilySingle = new();


    //    Part 1    


    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public static async Task<Tbl51InfrafamiliesPdfModel?> GetInfrafamilyDetailsAsync(int infrafamilyId)
    {
        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var infrafamilySingle = await dataService.GetInfrafamilyByIdAsync(infrafamilyId);
        if (infrafamilySingle == null)
            return null;

        _infrafamilySingle = infrafamilySingle;

        if (_infrafamilySingle.SubfamilyId != 0)
            _subfamilySingle = await dataService.GetSubfamilyByIdAsync(_infrafamilySingle.SubfamilyId) ?? new Tbl48Subfamily();

        if (_subfamilySingle.FamilyId != 0)
            _familySingle = await dataService.GetFamilyByIdAsync(_subfamilySingle.FamilyId) ?? new Tbl45Family();

        if (_familySingle.SuperfamilyId != 0)
            _superfamilySingle = await dataService.GetSuperfamilyByIdAsync(_familySingle.SuperfamilyId) ?? new Tbl42Superfamily();

        if (_superfamilySingle.InfraordoId != 0)
            _infraordoSingle = await dataService.GetInfraordoByIdAsync(_superfamilySingle.InfraordoId) ?? new Tbl39Infraordo();

        if (_infraordoSingle.SubordoId != 0)
            _subordoSingle = await dataService.GetSubordoByIdAsync(_infraordoSingle.SubordoId) ?? new Tbl36Subordo();

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
        SupertribussesCollection = new ObservableCollection<Tbl54Supertribus>(
                    (await dataService.GetSupertribussesByInfrafamilyIdAsync(infrafamilyId))
                    .Where(x => IsVisibleTaxonName(x.SupertribusName)));

        ExpertsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetExpertReferencesByInfrafamilyIdAsync(infrafamilyId));

        SourcesCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetSourceReferencesByInfrafamilyIdAsync(infrafamilyId));

        AuthorsCollection = new ObservableCollection<Tbl90Reference>(
            await dataService.GetAuthorReferencesByInfrafamilyIdAsync(infrafamilyId));

        CommentsCollection = new ObservableCollection<Tbl93Comment>(
            await dataService.GetCommentsByInfrafamilyIdAsync(infrafamilyId));

        return new Tbl51InfrafamiliesPdfModel
        {
            InfrafamilyName = _infrafamilySingle.InfrafamilyName,
            CountId = _infrafamilySingle.CountId,
            Synonym = _infrafamilySingle.Synonym,
            Author = _infrafamilySingle.Author,
            AuthorYear = _infrafamilySingle.AuthorYear,
            Info = _infrafamilySingle.Info,
            Valid = _infrafamilySingle.Valid,
            ValidYear = _infrafamilySingle.ValidYear,
            EngName = _infrafamilySingle.EngName,
            GerName = _infrafamilySingle.GerName,
            FraName = _infrafamilySingle.FraName,
            PorName = _infrafamilySingle.PorName,
            Memo = _infrafamilySingle.Memo,
            Writer = _infrafamilySingle.Writer,
            WriterDate = _infrafamilySingle.WriterDate,
            Updater = _infrafamilySingle.Updater,
            UpdaterDate = _infrafamilySingle.UpdaterDate,

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
            InfraordoTree = GenrateModels.GenInfraordos.GenerateInfraordos(_infraordoSingle),
            SuperfamilyTree = GenrateModels.GenSuperfamilies.GenerateSuperfamilies(_superfamilySingle),
            FamilyTree = GenrateModels.GenFamilies.GenerateFamilies(_familySingle),
            SubfamilyTree = GenrateModels.GenSubfamilies.GenerateSubfamilies(_subfamilySingle),
            InfrafamilyTree = GenrateModels.GenInfrafamilies.GenerateInfrafamilies(_infrafamilySingle),
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
    public static ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl45Family> FamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl48Subfamily> SubfamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection { get; set; } = new();
    public static ObservableCollection<Tbl54Supertribus> SupertribussesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> AuthorsCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> SourcesCollection { get; set; } = new();

    public static ObservableCollection<Tbl90Reference> ExpertsCollection { get; set; } = new();

    public static ObservableCollection<Tbl93Comment> CommentsCollection { get; set; } = new();
    #endregion "Collections"               

    public static void ClearAllCollections()
    {
        _infrafamilySingle = new Tbl51Infrafamily();
        _subfamilySingle = new Tbl48Subfamily();
        _familySingle = new Tbl45Family();
        _superfamilySingle = new Tbl42Superfamily();
        _infraordoSingle = new Tbl39Infraordo();
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

        InfrafamiliesCollection?.Clear();
        SubfamiliesCollection?.Clear();
        FamiliesCollection?.Clear();
        SuperfamiliesCollection?.Clear();
        InfraordosCollection?.Clear();
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
