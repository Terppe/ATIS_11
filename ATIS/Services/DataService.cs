using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Search;
using ATIS.Views.Search.DirectChildren;
using Microsoft.EntityFrameworkCore;
using static ATIS.Interfaces.IDataService;
using static ATIS.Views.Report.PdfModels;
using static SkiaSharp.HarfBuzz.SKShaper;
using ATIS.Core.Taxonomy;
//https://www.reddit.com/r/reactjs/comments/m4mhp7/how_to_update_state_in_useeffect_when_getting/

namespace ATIS.Services;

public partial class DataService : ObservableObject, IDataService
{
    private const string Value = "#";

    private readonly IUserService _user;
    private readonly AtisDbContext? _context;
    private readonly IDbContextFactory<AtisDbContext>? _factory;

    //public DataService(AtisDbContext context, IUserService user)
    //{
    //    _context = context;
    //    _user = user;
    //}
    public DataService(IDbContextFactory<AtisDbContext> factory, IUserService user)
    {
        _factory = factory;
        _user = user;
    }
    private void ApplyAuditInfo(IAuditable entity, bool isNew)
    {
        var user = _user.CurrentUser;
        var now = DateTime.UtcNow;

        if (isNew)
        {
            entity.Writer = user;
            entity.WriterDate = now;
            entity.CountId = RandomHelper.Randomnumber();
        }

        entity.Updater = user;
        entity.UpdaterDate = now;
    }

    private static void CopyProperties<T>(T source, T target, params string[] exclude)
    {
        var props = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => !exclude.Contains(p.Name));

        foreach (var prop in props)
        {
            var value = prop.GetValue(source);
            prop.SetValue(target, value);
        }
    }
    //Base-Logik für Upsert-Operationen, die in allen Entitäten verwendet werden kann
    protected async Task<TEntity> UpsertAsync<TEntity, TKey>(
        TEntity uiEntity,
        Func<DbContext, DbSet<TEntity>> setSelector,
        Func<TEntity, TKey> keySelector,
        Func<DbSet<TEntity>, TKey, Task<TEntity?>> findByKeyAsync,
        Action<TEntity, TEntity> mapBusinessFields)
        where TEntity : class, IDataService.IAuditable, new()
    {
        await using var context = _factory.CreateDbContext();
        var set = setSelector(context);

        var key = keySelector(uiEntity);
        var dbEntity = await findByKeyAsync(set, key);

        var isNew = dbEntity is null;
        dbEntity ??= new TEntity();

        if (isNew)
            set.Add(dbEntity);

        mapBusinessFields(uiEntity, dbEntity);
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();
        return dbEntity;
    }

    #region für provisorium

    public IQueryable<Tbl03Regnum> Tbl03Regnums => _context.Tbl03Regnums;
    public IQueryable<Tbl06Phylum> Tbl06Phylums => _context.Tbl06Phylums;
    public IQueryable<Tbl12Subphylum> Tbl12Subphylums => _context.Tbl12Subphylums;
    public IQueryable<Tbl18Superclass> Tbl18Superclasses => _context.Tbl18Superclasses;
    public IQueryable<Tbl21Class> Tbl21Classes => _context.Tbl21Classes;
    public IQueryable<Tbl24Subclass> Tbl24Subclasses => _context.Tbl24Subclasses;
    public IQueryable<Tbl27Infraclass> Tbl27Infraclasses => _context.Tbl27Infraclasses;
    public IQueryable<Tbl30Legio> Tbl30Legios => _context.Tbl30Legios;
    public IQueryable<Tbl33Ordo> Tbl33Ordos => _context.Tbl33Ordos;
    public IQueryable<Tbl36Subordo> Tbl36Subordos => _context.Tbl36Subordos;
    public IQueryable<Tbl39Infraordo> Tbl39Infraordos => _context.Tbl39Infraordos;
    public IQueryable<Tbl42Superfamily> Tbl42Superfamilies => _context.Tbl42Superfamilies;
    public IQueryable<Tbl45Family> Tbl45Families => _context.Tbl45Families;
    public IQueryable<Tbl48Subfamily> Tbl48Subfamilies => _context.Tbl48Subfamilies;
    public IQueryable<Tbl51Infrafamily> Tbl51Infrafamilies => _context.Tbl51Infrafamilies;
    public IQueryable<Tbl54Supertribus> Tbl54Supertribusses => _context.Tbl54Supertribusses;
    public IQueryable<Tbl57Tribus> Tbl57Tribusses => _context.Tbl57Tribusses;
    public IQueryable<Tbl60Subtribus> Tbl60Subtribusses => _context.Tbl60Subtribusses;
    public IQueryable<Tbl63Infratribus> Tbl63Infratribusses => _context.Tbl63Infratribusses;
    public IQueryable<Tbl66Genus> Tbl66Genusses => _context.Tbl66Genusses;
    public IQueryable<Tbl68Speciesgroup> Tbl68Speciesgroups => _context.Tbl68Speciesgroups;
    public IQueryable<Tbl75Species> Tbl75Speciesses => _context.Tbl75Speciesses;
    public IQueryable<Tbl78Name> Tbl78Names => _context.Tbl78Names;
    public IQueryable<Tbl81Image> Tbl81Images => _context.Tbl81Images;
    public IQueryable<Tbl84Synonym> Tbl84Synonyms => _context.Tbl84Synonyms;
    public IQueryable<Tbl87Geographic> Tbl87Geographics => _context.Tbl87Geographics;

    public IQueryable<Tbl90Reference> Tbl90References => _context.Tbl90References;
    public IQueryable<Tbl90RefAuthor> Tbl90RefAuthors => _context.Tbl90RefAuthors;
    public IQueryable<Tbl90RefExpert> Tbl90RefExperts => _context.Tbl90RefExperts;
    public IQueryable<Tbl90RefSource> Tbl90RefSources => _context.Tbl90RefSources;
    public IQueryable<Tbl93Comment> Tbl93Comments => _context.Tbl93Comments;

    public IQueryable<TblCountry> TblCountries => _context.TblCountries;
    public IQueryable<TblDisease> TblDiseases => _context.TblDiseases;
    public IQueryable<TblFood> TblFoods => _context.TblFoods;
    public IQueryable<TblMiscellaneous> TblMiscellaneousses => _context.TblMiscellaneousses;

    public IQueryable<TblUserProfile> TblUserProfiles => _context.TblUserProfiles;


    #endregion für provisorium



    #region Von App.cs verschoben

    public TblFood? SingleFood(int id) => _context.TblFoods.SingleOrDefault(a => a.FoodId == id);
    public TblDisease? SingleDisease(int id) => _context.TblDiseases.SingleOrDefault(a => a.DiseaseId == id);
    public TblMiscellaneous? SingleMiscellaneous(int id) => _context.TblMiscellaneousses.SingleOrDefault(a => a.MiscellaneousId == id);
    //-------------------------------------------
    public Tbl78Name? SingleName(int id) => _context.Tbl78Names.SingleOrDefault(a => a.NameId == id);
    public Tbl81Image? SingleImage(int id) => _context.Tbl81Images.SingleOrDefault(a => a.ImageId == id);
    public Tbl84Synonym? SingleSynonym(int id) => _context.Tbl84Synonyms.SingleOrDefault(a => a.SynonymId == id);
    public Tbl87Geographic? SingleGeographic(int id) => _context.Tbl87Geographics.SingleOrDefault(a => a.GeographicId == id);
    //-------------------------------------------------------------------------------
    public TblCountry? SingleCountry(int id) => _context.TblCountries.SingleOrDefault(a => a.CountryId == id);
    public TblUserProfile? SingleUserProfile(int id) => _context.TblUserProfiles.SingleOrDefault(a => a.UserProfileId == id);
    //--------------------------------------------------------------------
    public Tbl90RefAuthor? SingleRefAuthor(int id) => _context.Tbl90RefAuthors.SingleOrDefault(a => a.RefAuthorId == id);
    public Tbl90RefExpert? SingleRefExpert(int id) => _context.Tbl90RefExperts.SingleOrDefault(a => a.RefExpertId == id);
    public Tbl90RefSource? SingleRefSource(int id) => _context.Tbl90RefSources.SingleOrDefault(a => a.RefSourceId == id);
    public Tbl90Reference? SingleReference(int id) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == id);

    public Tbl93Comment? SingleComment(int id) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == id);

    //--------------------------------------------------------------------
    public Tbl81Image? SingleImageFood(int imageId, int? foodId) => _context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.FoodId == foodId);
    public Tbl81Image? SingleImageDisease(int imageId, int? diseaseId) => _context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.DiseaseId == diseaseId);
    public Tbl81Image? SingleImageMiscellaneous(int imageId, int? miscellaneousId) => _context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.MiscellaneousId == miscellaneousId);
    //------------------------------------------------------------------
    public Tbl75Species? SingleSpecies(int id) => _context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == id);
    public  Tbl90Reference? SingleReferenceSpecies(int referenceid, int? speciesId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SpeciesId == speciesId);
    public Tbl93Comment? SingleCommentSpecies(int commentid, int? speciesid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SpeciesId == speciesid);
    public Tbl78Name? SingleNameSpecies(int nameId, int? speciesId) => _context.Tbl78Names.SingleOrDefault(a => a.NameId == nameId & a.SpeciesId == speciesId);
    public Tbl81Image? SingleImageSpecies(int imageId, int? speciesId) => _context.Tbl81Images.SingleOrDefault(a => a.ImageId == imageId & a.SpeciesId == speciesId);
    public Tbl84Synonym? SingleSynonymSpecies(int synonymId, int? speciesId) => _context.Tbl84Synonyms.SingleOrDefault(a => a.SynonymId == synonymId & a.SpeciesId == speciesId);
    public Tbl87Geographic? SingleGeographicSpecies(int geographicId, int? speciesId) => _context.Tbl87Geographics.SingleOrDefault(a => a.GeographicId == geographicId & a.SpeciesId == speciesId);
    public Tbl75Species? SingleSpeciesGenus(int speciesid, int? genusid) => _context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesid & a.GenusId == genusid);
    public Tbl78Name? SingleNameSpecies(int nameid, int speciesid) => _context.Tbl78Names.SingleOrDefault(a => a.SpeciesId == speciesid & a.NameId == nameid);

    //------------------------------------------------------------------
    public Tbl68Speciesgroup? SingleSpeciesgroup(int id) => _context.Tbl68Speciesgroups.SingleOrDefault(a => a.SpeciesgroupId == id);
    public Tbl75Species? SingleSpeciesSpeciesgroup(int speciesId, int? speciesgroupId) => _context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == speciesId & a.SpeciesgroupId == speciesgroupId);

    //------------------------------------------------------------------

    public Tbl66Genus? SingleGenus(int id) => _context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == id);
    public Tbl90Reference? SingleReferenceGenus(int referenceid, int? genusId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.GenusId == genusId);
    public Tbl93Comment? SingleCommentGenus(int commentid, int? genusid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.GenusId == genusid);
    //---------------------------------------------------
    public Tbl63Infratribus? SingleInfratribus(int id) => _context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == id);
    public Tbl90Reference? SingleReferenceInfratribus(int referenceid, int? infratribusId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfratribusId == infratribusId);
    public Tbl93Comment? SingleCommentInfratribus(int commentid, int? infratribusid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfratribusId == infratribusid);
    public Tbl66Genus? SingleGenusInfratribus(int genusId, int? infratribusId) => _context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == genusId & a.InfratribusId == infratribusId);
    //------------------------------------------------------------------

    public Tbl60Subtribus? SingleSubtribus(int id) => _context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == id);
    public Tbl90Reference? SingleReferenceSubtribus(int referenceid, int? subtribusId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubtribusId == subtribusId);
    public Tbl93Comment? SingleCommentSubtribus(int commentid, int? subtribusid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubtribusId == subtribusid);

    public Tbl63Infratribus? SingleInfratribusSubtribus(int infratribusId, int? subtribusId) => _context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == infratribusId & a.SubtribusId == subtribusId);
    //---------------------------------------------------
    public Tbl57Tribus? SingleTribus(int id) => _context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == id);
    public Tbl90Reference? SingleReferenceTribus(int referenceid, int? tribusId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.TribusId == tribusId);
    public Tbl93Comment? SingleCommentTribus(int commentid, int? tribusid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.TribusId == tribusid);
    public Tbl60Subtribus? SingleSubtribusTribus(int subtribusId, int? tribusId) => _context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == subtribusId & a.TribusId == tribusId);
    //------------------------------------------------------------------
    public Tbl54Supertribus? SingleSupertribus(int id) => _context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == id);
    public Tbl90Reference? SingleReferenceSupertribus(int referenceid, int? supertribusId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SupertribusId == supertribusId);
    public Tbl93Comment? SingleCommentSupertribus(int commentid, int? supertribusid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SupertribusId == supertribusid);
    public Tbl57Tribus? SingleTribusSupertribus(int tribusId, int? supertribusId) => _context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == tribusId & a.SupertribusId == supertribusId);
    //------------------------------------------------------------------
    public Tbl51Infrafamily? SingleInfrafamily(int id) => _context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == id);
    public Tbl90Reference? SingleReferenceInfrafamily(int referenceid, int? infrafamilyId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfrafamilyId == infrafamilyId);
    public Tbl93Comment? SingleCommentInfrafamily(int commentid, int? infrafamilyid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfrafamilyId == infrafamilyid);
    public Tbl54Supertribus? SingleSupertribusInfrafamily(int supertribusId, int? infrafamilyId) => _context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == supertribusId & a.InfrafamilyId == infrafamilyId);
    //------------------------------------------------------------------
    public Tbl48Subfamily? SingleSubfamily(int id) => _context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == id);
    public Tbl90Reference? SingleReferenceSubfamily(int referenceid, int? subfamilyId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubfamilyId == subfamilyId);
    public Tbl93Comment? SingleCommentSubfamily(int commentid, int? subfamilyid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubfamilyId == subfamilyid);
    public Tbl51Infrafamily? SingleInfrafamilySubfamily(int infrafamilyId, int? subfamilyId) => _context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == infrafamilyId & a.SubfamilyId == subfamilyId);
    //------------------------------------------------------------------
    public Tbl45Family? SingleFamily(int id) => _context.Tbl45Families.SingleOrDefault(a => a.FamilyId == id);
    public Tbl90Reference? SingleReferenceFamily(int referenceid, int? familyId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.FamilyId == familyId);
    public Tbl93Comment? SingleCommentFamily(int commentid, int? familyid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.FamilyId == familyid);
    public Tbl48Subfamily? SingleSubfamilyFamily(int subfamilyId, int? familyId) => _context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == subfamilyId & a.FamilyId == familyId);
    ////------------------------------------------------------------------
    public Tbl42Superfamily? SingleSuperfamily(int id) => _context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == id);
    public Tbl90Reference? SingleReferenceSuperfamily(int referenceid, int? superfamilyId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SuperfamilyId == superfamilyId);
    public Tbl93Comment? SingleCommentSuperfamily(int commentid, int? superfamilyid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SuperfamilyId == superfamilyid);
    public Tbl45Family? SingleFamilySuperfamily(int familyId, int? superfamilyId) => _context.Tbl45Families.SingleOrDefault(a => a.FamilyId == familyId & a.SuperfamilyId == superfamilyId);
    //------------------------------------------------------------------
    public Tbl39Infraordo? SingleInfraordo(int id) => _context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == id);
    public Tbl90Reference? SingleReferenceInfraordo(int referenceid, int? infraordoId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfraordoId == infraordoId);
    public Tbl93Comment? SingleCommentInfraordo(int commentid, int? infraordoid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfraordoId == infraordoid);
    public Tbl42Superfamily? SingleSuperfamilyInfraordo(int superfamilyId, int? infraordoId) => _context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == superfamilyId & a.InfraordoId == infraordoId);
    //------------------------------------------------------------------
    public Tbl36Subordo? SingleSubordo(int id) => _context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == id);
    public Tbl90Reference? SingleReferenceSubordo(int referenceid, int? subordoId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubordoId == subordoId);
    public Tbl93Comment? SingleCommentSubordo(int commentid, int? subordoid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubordoId == subordoid);
    public Tbl39Infraordo? SingleInfraordoSubordo(int infraordoId, int? subordoId) => _context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == infraordoId & a.SubordoId == subordoId);
    //}
    //------------------------------------------------------------------
    public Tbl33Ordo? SingleOrdo(int id) => _context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == id);
    public Tbl90Reference? SingleReferenceOrdo(int referenceid, int? ordoId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.OrdoId == ordoId);
    public Tbl93Comment? SingleCommentOrdo(int commentid, int? ordoid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.OrdoId == ordoid);
    public Tbl36Subordo? SingleSubordoOrdo(int subordoId, int? ordoId) => _context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == subordoId & a.OrdoId == ordoId);
    //------------------------------------------------------------------
    public Tbl30Legio? SingleLegio(int id) => _context.Tbl30Legios.SingleOrDefault(a => a.LegioId == id);
    public Tbl90Reference? SingleReferenceLegio(int referenceid, int? legioId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.LegioId == legioId);
    public Tbl93Comment? SingleCommentLegio(int commentid, int? legioid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.LegioId == legioid);
    public Tbl33Ordo? SingleOrdoLegio(int ordoId, int? legioId) => _context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == ordoId & a.LegioId == legioId);
    //------------------------------------------------------------------
    public Tbl27Infraclass? SingleInfraclass(int id) => _context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == id);
    public Tbl90Reference? SingleReferenceInfraclass(int referenceid, int? infraclassId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.InfraclassId == infraclassId);
    public Tbl93Comment? SingleCommentInfraclass(int commentid, int? infraclassid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.InfraclassId == infraclassid);
    public Tbl30Legio? SingleLegioInfraclass(int legioId, int? infraclassId) => _context.Tbl30Legios.SingleOrDefault(a => a.LegioId == legioId & a.InfraclassId == infraclassId);
    //------------------------------------------------------------------
    public Tbl24Subclass? SingleSubclass(int id) => _context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == id);
    public Tbl90Reference? SingleReferenceSubclass(int referenceid, int? subclassId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubclassId == subclassId);
    public Tbl93Comment? SingleCommentSubclass(int commentid, int? subclassid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubclassId == subclassid);
    public Tbl27Infraclass? SingleInfraclassSubclass(int infraclassId, int? subclassId) => _context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == infraclassId & a.SubclassId == subclassId);
    //------------------------------------------------------------------
    public Tbl21Class? SingleClass(int id) => _context.Tbl21Classes.SingleOrDefault(a => a.ClassId == id);
    public Tbl90Reference? SingleReferenceClass(int referenceid, int? classId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.ClassId == classId);
    public Tbl93Comment? SingleCommentClass(int commentid, int? classid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.ClassId == classid);
    public Tbl24Subclass? SingleSubclassClass(int subclassId, int? classId) => _context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == subclassId & a.ClassId == classId);
    //------------------------------------------------------------------
    public Tbl18Superclass? SingleSuperclass(int id) => _context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == id);
    public Tbl90Reference? SingleReferenceSuperclass(int referenceid, int? superclassId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SuperclassId == superclassId);
    public Tbl93Comment? SingleCommentSuperclass(int commentid, int? superclassid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SuperclassId == superclassid);
    public Tbl21Class? SingleClassSuperclass(int classeId, int? superclassId) => _context.Tbl21Classes.SingleOrDefault(a => a.ClassId == classeId & a.SuperclassId == superclassId);
    //------------------------------------------------------------------
    public Tbl12Subphylum? SingleSubphylum(int id) => _context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id);
    public Tbl90Reference? SingleReferenceSubphylum(int referenceid, int? subphylumId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.SubphylumId == subphylumId);
    public Tbl93Comment? SingleCommentSubphylum(int commentid, int? subphylumid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.SubphylumId == subphylumid);
    public Tbl18Superclass? SingleSuperclassSubphylum(int superclassId, int? subphylumId) => _context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == superclassId & a.SubphylumId == subphylumId);
    //------------------------------------------------------------------
    public Tbl06Phylum? SinglePhylum(int id) => _context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id);
    public Tbl90Reference? SingleReferencePhylum(int referenceid, int? phylumId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.PhylumId == phylumId);
    public Tbl93Comment? SingleCommentPhylum(int commentid, int? phylumid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.PhylumId == phylumid);
    public Tbl12Subphylum? SingleSubphylumPhylum(int subphylumId, int? phylumId) => _context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == subphylumId & a.PhylumId == phylumId);
    //------------------------------------------------------------------
    public Tbl03Regnum? SingleRegnum(int id) => _context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id);
    public Tbl90Reference? SingleReferenceRegnum(int referenceid, int? regnumId) => _context.Tbl90References.SingleOrDefault(a => a.ReferenceId == referenceid & a.RegnumId == regnumId);
    public Tbl93Comment? SingleCommentRegnum(int commentid, int? regnumid) => _context.Tbl93Comments.SingleOrDefault(a => a.CommentId == commentid & a.RegnumId == regnumid);
    public Tbl06Phylum? SinglePhylumRegnum(int phylumId, int? regnumId) => _context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == phylumId & a.RegnumId == regnumId);

    #endregion Von App.cs verschoben



    #region CRUD


    #region Regnum
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllRegnumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl03Regnum>> GetAllRegnumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums
            .AsNoTracking()
            .OrderBy(e => e.RegnumName)
            .ThenBy(a => a.Subregnum)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchRegnumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl03Regnum>> SearchRegnumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl03Regnum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl03Regnums
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RegnumId == id || e.RegnumName.StartsWith(search))
                .OrderBy(e => e.RegnumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RegnumName.StartsWith(search))
            .OrderBy(e => e.RegnumName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertRegnumAsync(Tbl03Regnum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum uiEntity)
    {
        return UpsertAsync<Tbl03Regnum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl03Regnum>(),
            x => x.RegnumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RegnumId == id),
            MapRegnumFields
        );
    }
    // ---------------------------------------------------------                  
    // MapRegnumFields(Tbl03Regnum source, Tbl03Regnum target)
    // ---------------------------------------------------------                 
    private static void MapRegnumFields(Tbl03Regnum source, Tbl03Regnum target)
    {
        target.RegnumName = source.RegnumName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetRegnumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl03Regnum?> GetRegnumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RegnumId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteRegnumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRegnumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl03Regnums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl03Regnums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl03Regnum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumExistsAsync(Tbl03Regnum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl03Regnums.AnyAsync(x =>
            x.RegnumName == entity.RegnumName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.RegnumId != entity.RegnumId
        );
    }

    // ---------------------------------------------------------                  
    // HasPhylumAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasPhylumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl06Phylums
            .AnyAsync(x => x.RegnumId == regnumId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasReferenceAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.RegnumId == regnumId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<bool> RegnumHasCommentAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.RegnumId == regnumId);
    }

    // ---------------------------------------------------------                  
    // GetPhylumsByRegnumIdAsync(int regnumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> GetPhylumsByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .Where(x => x.RegnumId == regnumId)
            .OrderBy(x => x.PhylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.RegnumId == regnumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByRegnumIdAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByRegnumIdAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.RegnumId == regnumId)
            .AsNoTracking()
            .ToListAsync();
    }


    //alt----------------------
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync()
    {
        var collection = new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
            .AsNoTracking()
            .OrderBy(a => a.RegnumName)
            .ThenBy(a => a.Subregnum));
        return collection;
    }
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync(int regnumid)
    {
        var collection = new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
            .AsNoTracking()
            .Where(e => e.RegnumId == regnumid)
            .OrderBy(k => k.RegnumName)
            .ThenBy(k => k.Subregnum));

        return collection;
    }
    public ObservableCollection<Tbl03Regnum> GetRegnumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
                .Where(e => e.RegnumId == id))
            : new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
                .Where(e => search != null && e.RegnumName.StartsWith(search))
                .OrderBy(a => a.RegnumName)
                .ThenBy(a => a.Subregnum));
        return collection;
    }

    #endregion Regnum

    #region Phylum
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllPhylumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl06Phylum>> GetAllPhylumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .AsNoTracking()
            .OrderBy(e => e.PhylumName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchPhylumWithRegnumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> SearchPhylumWithRegnumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl06Phylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl06Phylums
            .Include(x => x.Regnum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.PhylumId == id || e.PhylumName.StartsWith(search))
                .OrderBy(e => e.PhylumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.PhylumName.StartsWith(search))
            .OrderBy(e => e.PhylumName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertPhylumAsync(Tbl06Phylum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum uiEntity)
    {
        return UpsertAsync<Tbl06Phylum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl06Phylum>(),
            x => x.PhylumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.PhylumId == id),
            MapPhylumFields
        );
    }
    // ---------------------------------------------------------                  
    // MapPhylumFields(Tbl06Phylum source, Tbl06Phylum target)
    // ---------------------------------------------------------                 
    private static void MapPhylumFields(Tbl06Phylum source, Tbl06Phylum target)
    {
        target.PhylumName = source.PhylumName;
        target.RegnumId = source.RegnumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetPhylumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl06Phylum?> GetPhylumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PhylumId == id);
    }
    // ---------------------------------------------------------                  
    // DeletePhylumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeletePhylumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl06Phylums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl06Phylums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl06Phylum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumExistsAsync(Tbl06Phylum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums.AnyAsync(x =>
            x.PhylumName == entity.PhylumName &&
            x.RegnumId == entity.RegnumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.PhylumId != entity.PhylumId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubphylumAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasSubphylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl12Subphylums
            .AnyAsync(x => x.PhylumId == phylumId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasReferenceAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.PhylumId == phylumId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> PhylumHasCommentAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.PhylumId == phylumId);
    }

    // ---------------------------------------------------------                  
    // GetPhylumForRegnumAsync(int regnumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl06Phylum>> GetPhylumForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl06Phylums
            .Where(e => e.RegnumId == regnumId)
            .Include(e => e.Regnum)
            .OrderBy(e => e.PhylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubphylumsByPhylumIdAsync(int phylumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> GetSubphylumsByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .Where(x => x.PhylumId == phylumId)
            .OrderBy(x => x.SubphylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.PhylumId == phylumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByPhylumIdAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByPhylumIdAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.PhylumId == phylumId)
            .AsNoTracking()
            .ToListAsync();
    }



    //alt--------------------------------------
    public ObservableCollection<Tbl06Phylum> GetPhylumAsync()
    {
        var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
            .AsNoTracking()
            .OrderBy(a => a.PhylumName));
        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumAsync(int phylumid)
    {
        var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
            .AsNoTracking()
            .Where(e => e.PhylumId == phylumid)
            .OrderBy(k => k.PhylumName)
        );

        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
                .Where(e => e.PhylumId == id))
            : new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
                .Where(e => search != null && e.PhylumName.StartsWith(search))
                .OrderBy(a => a.PhylumName));
        return collection;
    }
    public ObservableCollection<Tbl06Phylum> GetPhylumFromRegnumIdAsync(int regnumid)
    {
        var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
            .Where(e => e.RegnumId == regnumid)
            .OrderBy(k => k.PhylumName)
        );

        return collection;
    }

    #endregion Phylum

    #region Subphylum
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSubphylumAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl12Subphylum>> GetAllSubphylumAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .AsNoTracking()
            .OrderBy(e => e.SubphylumName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSubphylumWithPhylumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumWithPhylumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl12Subphylum>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl12Subphylums
            .Include(x => x.Phylum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubphylumId == id || e.SubphylumName.StartsWith(search))
                .OrderBy(e => e.SubphylumName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubphylumName.StartsWith(search))
            .OrderBy(e => e.SubphylumName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSubphylumAsync(Tbl12Subphylum uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum uiEntity)
    {
        return UpsertAsync<Tbl12Subphylum, int>(
            uiEntity,
            ctx => ctx.Set<Tbl12Subphylum>(),
            x => x.SubphylumId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubphylumId == id),
            MapSubphylumFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSubphylumFields(Tbl12Subphylum source, Tbl12Subphylum target)
    // ---------------------------------------------------------                 
    private static void MapSubphylumFields(Tbl12Subphylum source, Tbl12Subphylum target)
    {
        target.SubphylumName = source.SubphylumName;
        target.PhylumId = source.PhylumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSubphylumByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl12Subphylum?> GetSubphylumByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubphylumId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSubphylumAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubphylumAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl12Subphylums.FindAsync(id);
        if (entity != null)
        {
            context.Tbl12Subphylums.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl12Subphylum entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumExistsAsync(Tbl12Subphylum entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums.AnyAsync(x =>
            x.SubphylumName == entity.SubphylumName &&
            x.PhylumId == entity.PhylumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubphylumId != entity.SubphylumId
        );
    }

    // ---------------------------------------------------------                  
    // HasSuperclassAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasSuperclassAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl18Superclasses
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasReferenceAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubphylumHasCommentAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubphylumId == subphylumId);
    }

    // ---------------------------------------------------------                  
    // GetSubphylumForPhylumAsync(int phylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl12Subphylum>> GetSubphylumForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl12Subphylums
            .Where(e => e.PhylumId == phylumId)
            .Include(e => e.Phylum)
            .OrderBy(e => e.SubphylumName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSuperclassesBySubphylumIdAsync(int subphylumId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> GetSuperclassesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .Where(x => x.SubphylumId == subphylumId)
            .OrderBy(x => x.SuperclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubphylumId == subphylumId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubphylumIdAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubphylumIdAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubphylumId == subphylumId)
            .AsNoTracking()
            .ToListAsync();
    }

    //alt-------------------------------
    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync()
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
            .AsNoTracking()
            .OrderBy(a => a.SubphylumName));
        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(int subphylumid)
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
        .AsNoTracking()
            .Where(e => e.SubphylumId == subphylumid)
            .OrderBy(k => k.SubphylumName)
        );

        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id))
            : new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => search != null && e.SubphylumName.StartsWith(search))
                .OrderBy(a => a.SubphylumName));
        return collection;
    }
    public ObservableCollection<Tbl12Subphylum> GetSubphylumFromPhylumIdAsync(int phylumid)
    {
        var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
            .Where(e => e.PhylumId == phylumid)
            .OrderBy(k => k.SubphylumName)
        );

        return collection;
    }

    #endregion Subphylum

    #region Superclass
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSuperclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl18Superclass>> GetAllSuperclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .AsNoTracking()
            .OrderBy(e => e.SuperclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSuperclassWithSubphylumAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> SearchSuperclassWithSubphylumAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl18Superclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl18Superclasses
            .Include(x => x.Subphylum)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SuperclassId == id || e.SuperclassName.StartsWith(search))
                .OrderBy(e => e.SuperclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SuperclassName.StartsWith(search))
            .OrderBy(e => e.SuperclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSuperclassAsync(Tbl18Superclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass uiEntity)
    {
        return UpsertAsync<Tbl18Superclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl18Superclass>(),
            x => x.SuperclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SuperclassId == id),
            MapSuperclassFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSuperclassFields(Tbl18Superclass source, Tbl18Superclass target)
    // ---------------------------------------------------------                 
    private static void MapSuperclassFields(Tbl18Superclass source, Tbl18Superclass target)
    {
        target.SuperclassName = source.SuperclassName;
        target.SubphylumId = source.SubphylumId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSuperclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl18Superclass?> GetSuperclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SuperclassId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSuperclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSuperclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl18Superclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl18Superclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl18Superclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassExistsAsync(Tbl18Superclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses.AnyAsync(x =>
            x.SuperclassName == entity.SuperclassName &&
            x.SubphylumId == entity.SubphylumId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SuperclassId != entity.SuperclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasClassAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasClassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl21Classes
            .AnyAsync(x => x.SuperclassId == superclassId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasReferenceAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SuperclassId == superclassId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperclassHasCommentAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SuperclassId == superclassId);
    }

    // ---------------------------------------------------------                  
    // GetSuperclassForSubphylumAsync(int subphylumId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl18Superclass>> GetSuperclassForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl18Superclasses
            .Where(e => e.SubphylumId == subphylumId)
            .Include(e => e.Subphylum)
            .OrderBy(e => e.SuperclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetClassesBySuperclassIdAsync(int superclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> GetClassesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .Where(x => x.SuperclassId == superclassId)
            .OrderBy(x => x.ClassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SuperclassId == superclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySuperclassIdAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySuperclassIdAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SuperclassId == superclassId)
            .AsNoTracking()
            .ToListAsync();
    }



    //alt-----------------------------------------
    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync()
    {
        var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
            .AsNoTracking()
            .OrderBy(a => a.SuperclassName));
        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync(int superclassid)
    {
        var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
            .AsNoTracking()
            .Where(e => e.SuperclassId == superclassid)
            .OrderBy(k => k.SuperclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id))
            : new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => search != null && e.SuperclassName.StartsWith(search))
                .OrderBy(a => a.SuperclassName));
        return collection;
    }

    public ObservableCollection<Tbl18Superclass> GetSuperclassFromSubphylumIdAsync(int subphylumid)
    {
        var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
            .Where(e => e.SubphylumId == subphylumid)
            .OrderBy(k => k.SuperclassName)
        );

        return collection;
    }


    #endregion Superclass

    #region Class
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllClassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl21Class>> GetAllClassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .AsNoTracking()
            .OrderBy(e => e.ClassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchClassWithSuperclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> SearchClassWithSuperclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl21Class>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl21Classes
            .Include(x => x.Superclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.ClassId == id || e.ClassName.StartsWith(search))
                .OrderBy(e => e.ClassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.ClassName.StartsWith(search))
            .OrderBy(e => e.ClassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertClassAsync(Tbl21Class uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl21Class> UpsertClassAsync(Tbl21Class uiEntity)
    {
        return UpsertAsync<Tbl21Class, int>(
            uiEntity,
            ctx => ctx.Set<Tbl21Class>(),
            x => x.ClassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.ClassId == id),
            MapClassFields
        );
    }
    // ---------------------------------------------------------                  
    // MapClassFields(Tbl21Class source, Tbl21Class target)
    // ---------------------------------------------------------                 
    private static void MapClassFields(Tbl21Class source, Tbl21Class target)
    {
        target.ClassName = source.ClassName;
        target.SuperclassId = source.SuperclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetClassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl21Class?> GetClassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClassId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteClassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteClassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl21Classes.FindAsync(id);
        if (entity != null)
        {
            context.Tbl21Classes.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl21Class entity)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassExistsAsync(Tbl21Class entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes.AnyAsync(x =>
            x.ClassName == entity.ClassName &&
            x.SuperclassId == entity.SuperclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.ClassId != entity.ClassId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubclassAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasSubclassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl24Subclasses
            .AnyAsync(x => x.ClassId == classId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasReferenceAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.ClassId == classId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<bool> ClassHasCommentAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.ClassId == classId);
    }

    // ---------------------------------------------------------                  
    // GetClassForSuperclassAsync(int superclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl21Class>> GetClassForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl21Classes
            .Where(e => e.SuperclassId == superclassId)
            .Include(e => e.Superclass)
            .OrderBy(e => e.ClassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubclassesByClassIdAsync(int classId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> GetSubclassesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .Where(x => x.ClassId == classId)
            .OrderBy(x => x.SubclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.ClassId == classId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.ClassId == classId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.ClassId == classId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByClassIdAsync(int classId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByClassIdAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.ClassId == classId)
            .AsNoTracking()
            .ToListAsync();
    }


    //alt
    public ObservableCollection<Tbl21Class> GetClassAsync()
    {
        var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
            .AsNoTracking()
            .OrderBy(a => a.ClassName));
        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassAsync(int classid)
    {
        var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
            .AsNoTracking()
            .Where(e => e.ClassId == classid)
            .OrderBy(k => k.ClassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => e.ClassId == id))
            : new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => search != null && e.ClassName.StartsWith(search))
                .OrderBy(a => a.ClassName));
        return collection;
    }

    public ObservableCollection<Tbl21Class> GetClassFromSuperclassIdAsync(int superclassid)
    {
        var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
            .Where(e => e.SuperclassId == superclassid)
            .OrderBy(k => k.ClassName)
        );

        return collection;
    }


    #endregion Class                       

    #region Subclass
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSubclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl24Subclass>> GetAllSubclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .AsNoTracking()
            .OrderBy(e => e.SubclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSubclassWithClassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> SearchSubclassWithClassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl24Subclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl24Subclasses
            .Include(x => x.Class)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubclassId == id || e.SubclassName.StartsWith(search))
                .OrderBy(e => e.SubclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubclassName.StartsWith(search))
            .OrderBy(e => e.SubclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSubclassAsync(Tbl24Subclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass uiEntity)
    {
        return UpsertAsync<Tbl24Subclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl24Subclass>(),
            x => x.SubclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubclassId == id),
            MapSubclassFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSubclassFields(Tbl24Subclass source, Tbl24Subclass target)
    // ---------------------------------------------------------                 
    private static void MapSubclassFields(Tbl24Subclass source, Tbl24Subclass target)
    {
        target.SubclassName = source.SubclassName;
        target.ClassId = source.ClassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSubclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl24Subclass?> GetSubclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubclassId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSubclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl24Subclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl24Subclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl24Subclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassExistsAsync(Tbl24Subclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses.AnyAsync(x =>
            x.SubclassName == entity.SubclassName &&
            x.ClassId == entity.ClassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubclassId != entity.SubclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfraclassAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasInfraclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl27Infraclasses
            .AnyAsync(x => x.SubclassId == subclassId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasReferenceAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubclassId == subclassId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubclassHasCommentAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubclassId == subclassId);
    }

    // ---------------------------------------------------------                  
    // GetSubclassForClassAsync(int classeId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl24Subclass>> GetSubclassForClassAsync(int classeId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl24Subclasses
            .Where(e => e.ClassId == classeId)
            .Include(e => e.Class)
            .OrderBy(e => e.SubclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetInfraclassesBySubclassIdAsync(int subclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> GetInfraclassesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .Where(x => x.SubclassId == subclassId)
            .OrderBy(x => x.InfraclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesBySubclassIdAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.SubclassId == subclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsBySubclassIdAsync(int subclassId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsBySubclassIdAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.SubclassId == subclassId)
            .AsNoTracking()
            .ToListAsync();
    }

    //test
    //alt
    public ObservableCollection<Tbl24Subclass> GetSubclassAsync()
    {
        var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
            .AsNoTracking()
            .OrderBy(a => a.SubclassName));
        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassAsync(int subclassid)
    {
        var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
            .AsNoTracking()
            .Where(e => e.SubclassId == subclassid)
            .OrderBy(k => k.SubclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => e.SubclassId == id))
            : new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => search != null && e.SubclassName.StartsWith(search))
                .OrderBy(a => a.SubclassName));
        return collection;
    }

    public ObservableCollection<Tbl24Subclass> GetSubclassFromClassIdAsync(int classeid)
    {
        var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
            .Where(e => e.ClassId == classeid)
            .OrderBy(k => k.SubclassName)
        );

        return collection;
    }


    #endregion Subclass                       

    #region Infraclass                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllInfraclassAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl27Infraclass>> GetAllInfraclassAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .AsNoTracking()
            .OrderBy(e => e.InfraclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchInfraclassWithSubclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassWithSubclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl27Infraclass>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl27Infraclasses
            .Include(x => x.Subclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfraclassId == id || e.InfraclassName.StartsWith(search))
                .OrderBy(e => e.InfraclassName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfraclassName.StartsWith(search))
            .OrderBy(e => e.InfraclassName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertInfraclassAsync(Tbl27Infraclass uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass uiEntity)
    {
        return UpsertAsync<Tbl27Infraclass, int>(
            uiEntity,
            ctx => ctx.Set<Tbl27Infraclass>(),
            x => x.InfraclassId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfraclassId == id),
            MapInfraclassFields
        );
    }
    // ---------------------------------------------------------                  
    // MapInfraclassFields(Tbl27Infraclass source, Tbl27Infraclass target)
    // ---------------------------------------------------------                 
    private static void MapInfraclassFields(Tbl27Infraclass source, Tbl27Infraclass target)
    {
        target.InfraclassName = source.InfraclassName;
        target.SubclassId = source.SubclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetInfraclassByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl27Infraclass?> GetInfraclassByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfraclassId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteInfraclassAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfraclassAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl27Infraclasses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl27Infraclasses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl27Infraclass entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassExistsAsync(Tbl27Infraclass entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses.AnyAsync(x =>
            x.InfraclassName == entity.InfraclassName &&
            x.SubclassId == entity.SubclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfraclassId != entity.InfraclassId
        );
    }

    // ---------------------------------------------------------                  
    // HasLegioAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasLegioAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl30Legios
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasReferenceAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraclassHasCommentAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfraclassId == infraclassId);
    }

    // ---------------------------------------------------------                  
    // GetInfraclassForSubclassAsync(int subclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl27Infraclass>> GetInfraclassForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl27Infraclasses
            .Where(e => e.SubclassId == subclassId)
            .Include(e => e.Subclass)
            .OrderBy(e => e.InfraclassName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetLegiosByInfraclassIdAsync(int infraclassId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> GetLegiosByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .Where(x => x.InfraclassId == infraclassId)
            .OrderBy(x => x.LegioName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByInfraclassIdAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.InfraclassId == infraclassId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByInfraclassIdAsync(int infraclassId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByInfraclassIdAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.InfraclassId == infraclassId)
            .AsNoTracking()
            .ToListAsync();
    }

    //Test
    private static bool IsVisibleTaxonName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && !name.Contains('#');
    }

    public async Task<DirectChildrenResult?> GetFirstDirectChildrenOfSubclassAsync(int subclassId)
    {
        var infraclasses = (await GetInfraclassesBySubclassIdAsync(subclassId))
            .Where(x => IsVisibleTaxonName(x.InfraclassName))
            .Cast<object>()
            .ToList();

        if (infraclasses.Count > 0)
        {
            return new DirectChildrenResult
            {
                Level = TaxonLevel.Infraclass,
                Items = infraclasses
            };
        }

        var legios = (await GetLegiosByInfraclassIdAsync(subclassId))
            .Where(x => IsVisibleTaxonName(x.LegioName))
            .Cast<object>()
            .ToList();

        if (legios.Count > 0)
        {
            return new DirectChildrenResult
            {
                Level = TaxonLevel.Legio,
                Items = legios
            };
        }

        var ordos = (await GetOrdosByLegioIdAsync(subclassId))
            .Where(x => IsVisibleTaxonName(x.OrdoName))
            .Cast<object>()
            .ToList();

        if (ordos.Count > 0)
        {
            return new DirectChildrenResult
            {
                Level = TaxonLevel.Ordo,
                Items = ordos
            };
        }

        return null;
    }
    #endregion Infraclass                       

    #region Legio                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllLegioAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl30Legio>> GetAllLegioAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .AsNoTracking()
            .OrderBy(e => e.LegioName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchLegioWithInfraclassAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> SearchLegioWithInfraclassAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl30Legio>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl30Legios
            .Include(x => x.Infraclass)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.LegioId == id || e.LegioName.StartsWith(search))
                .OrderBy(e => e.LegioName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.LegioName.StartsWith(search))
            .OrderBy(e => e.LegioName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertLegioAsync(Tbl30Legio uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio uiEntity)
    {
        return UpsertAsync<Tbl30Legio, int>(
            uiEntity,
            ctx => ctx.Set<Tbl30Legio>(),
            x => x.LegioId,
            (set, id) => set.FirstOrDefaultAsync(x => x.LegioId == id),
            MapLegioFields
        );
    }
    // ---------------------------------------------------------                  
    // MapLegioFields(Tbl30Legio source, Tbl30Legio target)
    // ---------------------------------------------------------                 
    private static void MapLegioFields(Tbl30Legio source, Tbl30Legio target)
    {
        target.LegioName = source.LegioName;
        target.InfraclassId = source.InfraclassId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetLegioByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl30Legio?> GetLegioByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LegioId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteLegioAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteLegioAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl30Legios.FindAsync(id);
        if (entity != null)
        {
            context.Tbl30Legios.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl30Legio entity)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioExistsAsync(Tbl30Legio entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios.AnyAsync(x =>
            x.LegioName == entity.LegioName &&
            x.InfraclassId == entity.InfraclassId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.LegioId != entity.LegioId
        );
    }

    // ---------------------------------------------------------                  
    // HasOrdoAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasOrdoAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl33Ordos
            .AnyAsync(x => x.LegioId == legioId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasReferenceAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.LegioId == legioId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<bool> LegioHasCommentAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.LegioId == legioId);
    }

    // ---------------------------------------------------------                  
    // GetLegioForInfraclassAsync(int infraclassId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl30Legio>> GetLegioForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl30Legios
            .Where(e => e.InfraclassId == infraclassId)
            .Include(e => e.Infraclass)
            .OrderBy(e => e.LegioName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetOrdosByLegioIdAsync(int legioId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> GetOrdosByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .Where(x => x.LegioId == legioId)
            .OrderBy(x => x.OrdoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.LegioId == legioId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.LegioId == legioId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByLegioIdAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.LegioId == legioId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByLegioIdAsync(int legioId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByLegioIdAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.LegioId == legioId)
            .AsNoTracking()
            .ToListAsync();
    }


    #endregion Legio                       

    #region Ordo                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllOrdoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl33Ordo>> GetAllOrdoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .AsNoTracking()
            .OrderBy(e => e.OrdoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchOrdoWithLegioAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> SearchOrdoWithLegioAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl33Ordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl33Ordos
            .Include(x => x.Legio)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.OrdoId == id || e.OrdoName.StartsWith(search))
                .OrderBy(e => e.OrdoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.OrdoName.StartsWith(search))
            .OrderBy(e => e.OrdoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertOrdoAsync(Tbl33Ordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo uiEntity)
    {
        return UpsertAsync<Tbl33Ordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl33Ordo>(),
            x => x.OrdoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.OrdoId == id),
            MapOrdoFields
        );
    }
    // ---------------------------------------------------------                  
    // MapOrdoFields(Tbl33Ordo source, Tbl33Ordo target)
    // ---------------------------------------------------------                 
    private static void MapOrdoFields(Tbl33Ordo source, Tbl33Ordo target)
    {
        target.OrdoName = source.OrdoName;
        target.LegioId = source.LegioId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetOrdoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl33Ordo?> GetOrdoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrdoId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteOrdoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteOrdoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl33Ordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl33Ordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl33Ordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoExistsAsync(Tbl33Ordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos.AnyAsync(x =>
            x.OrdoName == entity.OrdoName &&
            x.LegioId == entity.LegioId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.OrdoId != entity.OrdoId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubordoAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasSubordoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl36Subordos
            .AnyAsync(x => x.OrdoId == ordoId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasReferenceAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.OrdoId == ordoId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> OrdoHasCommentAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.OrdoId == ordoId);
    }

    // ---------------------------------------------------------                  
    // GetOrdoForLegioAsync(int legioId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl33Ordo>> GetOrdoForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl33Ordos
            .Where(e => e.LegioId == legioId)
            .Include(e => e.Legio)
            .OrderBy(e => e.OrdoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSubordosByOrdoIdAsync(int ordoId) FürReportPDF
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> GetSubordosByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .Where(x => x.OrdoId == ordoId)
            .OrderBy(x => x.SubordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetExpertReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefExperts)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefExpertId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue)
            .OrderBy(x => x.Tbl90RefExperts.RefExpertName)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetSourceReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefSources)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefSourceId.HasValue &&
                        !x.RefAuthorId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefSources.RefSourceName)
            .ThenBy(x => x.Tbl90RefSources.SourceYear)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetAuthorReferencesByOrdoIdAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Include(x => x.Tbl90RefAuthors)
            .Where(x => x.OrdoId == ordoId &&
                        x.RefAuthorId.HasValue &&
                        !x.RefSourceId.HasValue &&
                        !x.RefExpertId.HasValue)
            .OrderBy(x => x.Tbl90RefAuthors.RefAuthorName)
            .ThenBy(x => x.Tbl90RefAuthors.ArticelTitle)
            .ThenBy(x => x.Tbl90RefAuthors.BookName)
            .ThenBy(x => x.Tbl90RefAuthors.Page1)
            .ThenBy(x => x.Tbl90RefAuthors.Publisher)
            .AsNoTracking()
            .ToListAsync();
    }

    // ---------------------------------------------------------                  
    // GetCommentsByOrdoIdAsync(int ordoId)      
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsByOrdoIdAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(x => x.OrdoId == ordoId)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Ordo                       

    #region Subordo                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSubordoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl36Subordo>> GetAllSubordoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .AsNoTracking()
            .OrderBy(e => e.SubordoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSubordoWithOrdoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> SearchSubordoWithOrdoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl36Subordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl36Subordos
            .Include(x => x.Ordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubordoId == id || e.SubordoName.StartsWith(search))
                .OrderBy(e => e.SubordoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubordoName.StartsWith(search))
            .OrderBy(e => e.SubordoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSubordoAsync(Tbl36Subordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo uiEntity)
    {
        return UpsertAsync<Tbl36Subordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl36Subordo>(),
            x => x.SubordoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubordoId == id),
            MapSubordoFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSubordoFields(Tbl36Subordo source, Tbl36Subordo target)
    // ---------------------------------------------------------                 
    private static void MapSubordoFields(Tbl36Subordo source, Tbl36Subordo target)
    {
        target.SubordoName = source.SubordoName;
        target.OrdoId = source.OrdoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSubordoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl36Subordo?> GetSubordoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubordoId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSubordoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubordoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl36Subordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl36Subordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl36Subordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoExistsAsync(Tbl36Subordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos.AnyAsync(x =>
            x.SubordoName == entity.SubordoName &&
            x.OrdoId == entity.OrdoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubordoId != entity.SubordoId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfraordoAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasInfraordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl39Infraordos
            .AnyAsync(x => x.SubordoId == subordoId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasReferenceAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubordoId == subordoId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubordoHasCommentAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubordoId == subordoId);
    }

    // ---------------------------------------------------------                  
    // GetSubordoForOrdoAsync(int ordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl36Subordo>> GetSubordoForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl36Subordos
            .Where(e => e.OrdoId == ordoId)
            .Include(e => e.Ordo)
            .OrderBy(e => e.SubordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subordo                       

    #region Infraordo                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllInfraordoAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl39Infraordo>> GetAllInfraordoAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .AsNoTracking()
            .OrderBy(e => e.InfraordoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchInfraordoWithSubordoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl39Infraordo>> SearchInfraordoWithSubordoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl39Infraordo>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl39Infraordos
            .Include(x => x.Subordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfraordoId == id || e.InfraordoName.StartsWith(search))
                .OrderBy(e => e.InfraordoName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfraordoName.StartsWith(search))
            .OrderBy(e => e.InfraordoName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertInfraordoAsync(Tbl39Infraordo uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo uiEntity)
    {
        return UpsertAsync<Tbl39Infraordo, int>(
            uiEntity,
            ctx => ctx.Set<Tbl39Infraordo>(),
            x => x.InfraordoId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfraordoId == id),
            MapInfraordoFields
        );
    }
    // ---------------------------------------------------------                  
    // MapInfraordoFields(Tbl39Infraordo source, Tbl39Infraordo target)
    // ---------------------------------------------------------                 
    private static void MapInfraordoFields(Tbl39Infraordo source, Tbl39Infraordo target)
    {
        target.InfraordoName = source.InfraordoName;
        target.SubordoId = source.SubordoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetInfraordoByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl39Infraordo?> GetInfraordoByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfraordoId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteInfraordoAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfraordoAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl39Infraordos.FindAsync(id);
        if (entity != null)
        {
            context.Tbl39Infraordos.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl39Infraordo entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoExistsAsync(Tbl39Infraordo entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos.AnyAsync(x =>
            x.InfraordoName == entity.InfraordoName &&
            x.SubordoId == entity.SubordoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfraordoId != entity.InfraordoId
        );
    }

    // ---------------------------------------------------------                  
    // HasSuperfamilyAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasSuperfamilyAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl42Superfamilies
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasReferenceAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfraordoHasCommentAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfraordoId == infraordoId);
    }

    // ---------------------------------------------------------                  
    // GetInfraordoForSubordoAsync(int subordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl39Infraordo>> GetInfraordoForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl39Infraordos
            .Where(e => e.SubordoId == subordoId)
            .Include(e => e.Subordo)
            .OrderBy(e => e.InfraordoName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Infraordo                       

    #region Superfamily                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSuperfamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl42Superfamily>> GetAllSuperfamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .AsNoTracking()
            .OrderBy(e => e.SuperfamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSuperfamilyWithInfraordoAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl42Superfamily>> SearchSuperfamilyWithInfraordoAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl42Superfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl42Superfamilies
            .Include(x => x.Infraordo)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SuperfamilyId == id || e.SuperfamilyName.StartsWith(search))
                .OrderBy(e => e.SuperfamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SuperfamilyName.StartsWith(search))
            .OrderBy(e => e.SuperfamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSuperfamilyAsync(Tbl42Superfamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily uiEntity)
    {
        return UpsertAsync<Tbl42Superfamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl42Superfamily>(),
            x => x.SuperfamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SuperfamilyId == id),
            MapSuperfamilyFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSuperfamilyFields(Tbl42Superfamily source, Tbl42Superfamily target)
    // ---------------------------------------------------------                 
    private static void MapSuperfamilyFields(Tbl42Superfamily source, Tbl42Superfamily target)
    {
        target.SuperfamilyName = source.SuperfamilyName;
        target.InfraordoId = source.InfraordoId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSuperfamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl42Superfamily?> GetSuperfamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SuperfamilyId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSuperfamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSuperfamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl42Superfamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl42Superfamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl42Superfamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyExistsAsync(Tbl42Superfamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies.AnyAsync(x =>
            x.SuperfamilyName == entity.SuperfamilyName &&
            x.InfraordoId == entity.InfraordoId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SuperfamilyId != entity.SuperfamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasFamilyAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasFamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl45Families
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasReferenceAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SuperfamilyHasCommentAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SuperfamilyId == superfamilyId);
    }

    // ---------------------------------------------------------                  
    // GetSuperfamilyForInfraordoAsync(int infraordoId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl42Superfamily>> GetSuperfamilyForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl42Superfamilies
            .Where(e => e.InfraordoId == infraordoId)
            .Include(e => e.Infraordo)
            .OrderBy(e => e.SuperfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Superfamily                       

    #region Family                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllFamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl45Family>> GetAllFamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .AsNoTracking()
            .OrderBy(e => e.FamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchFamilyWithSuperfamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl45Family>> SearchFamilyWithSuperfamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl45Family>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl45Families
            .Include(x => x.Superfamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.FamilyId == id || e.FamilyName.StartsWith(search))
                .OrderBy(e => e.FamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.FamilyName.StartsWith(search))
            .OrderBy(e => e.FamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertFamilyAsync(Tbl45Family uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family uiEntity)
    {
        return UpsertAsync<Tbl45Family, int>(
            uiEntity,
            ctx => ctx.Set<Tbl45Family>(),
            x => x.FamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.FamilyId == id),
            MapFamilyFields
        );
    }
    // ---------------------------------------------------------                  
    // MapFamilyFields(Tbl45Family source, Tbl45Family target)
    // ---------------------------------------------------------                 
    private static void MapFamilyFields(Tbl45Family source, Tbl45Family target)
    {
        target.FamilyName = source.FamilyName;
        target.SuperfamilyId = source.SuperfamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetFamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl45Family?> GetFamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FamilyId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteFamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteFamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl45Families.FindAsync(id);
        if (entity != null)
        {
            context.Tbl45Families.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl45Family entity)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyExistsAsync(Tbl45Family entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families.AnyAsync(x =>
            x.FamilyName == entity.FamilyName &&
            x.SuperfamilyId == entity.SuperfamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.FamilyId != entity.FamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubfamilyAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasSubfamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl48Subfamilies
            .AnyAsync(x => x.FamilyId == familyId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasReferenceAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.FamilyId == familyId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<bool> FamilyHasCommentAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.FamilyId == familyId);
    }

    // ---------------------------------------------------------                  
    // GetFamilyForSuperfamilyAsync(int superfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl45Family>> GetFamilyForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl45Families
            .Where(e => e.SuperfamilyId == superfamilyId)
            .Include(e => e.Superfamily)
            .OrderBy(e => e.FamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Family                       

    #region Subfamily                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSubfamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl48Subfamily>> GetAllSubfamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .AsNoTracking()
            .OrderBy(e => e.SubfamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSubfamilyWithFamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl48Subfamily>> SearchSubfamilyWithFamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl48Subfamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl48Subfamilies
            .Include(x => x.Family)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubfamilyId == id || e.SubfamilyName.StartsWith(search))
                .OrderBy(e => e.SubfamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubfamilyName.StartsWith(search))
            .OrderBy(e => e.SubfamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSubfamilyAsync(Tbl48Subfamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily uiEntity)
    {
        return UpsertAsync<Tbl48Subfamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl48Subfamily>(),
            x => x.SubfamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubfamilyId == id),
            MapSubfamilyFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSubfamilyFields(Tbl48Subfamily source, Tbl48Subfamily target)
    // ---------------------------------------------------------                 
    private static void MapSubfamilyFields(Tbl48Subfamily source, Tbl48Subfamily target)
    {
        target.SubfamilyName = source.SubfamilyName;
        target.FamilyId = source.FamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSubfamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl48Subfamily?> GetSubfamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubfamilyId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSubfamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubfamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl48Subfamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl48Subfamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl48Subfamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyExistsAsync(Tbl48Subfamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies.AnyAsync(x =>
            x.SubfamilyName == entity.SubfamilyName &&
            x.FamilyId == entity.FamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubfamilyId != entity.SubfamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfrafamilyAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasInfrafamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl51Infrafamilies
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasReferenceAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubfamilyHasCommentAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubfamilyId == subfamilyId);
    }

    // ---------------------------------------------------------                  
    // GetSubfamilyForFamilyAsync(int familyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl48Subfamily>> GetSubfamilyForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl48Subfamilies
            .Where(e => e.FamilyId == familyId)
            .Include(e => e.Family)
            .OrderBy(e => e.SubfamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Subfamily                       

    #region Infrafamily                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllInfrafamilyAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl51Infrafamily>> GetAllInfrafamilyAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .AsNoTracking()
            .OrderBy(e => e.InfrafamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchInfrafamilyWithSubfamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl51Infrafamily>> SearchInfrafamilyWithSubfamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl51Infrafamily>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl51Infrafamilies
            .Include(x => x.Subfamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfrafamilyId == id || e.InfrafamilyName.StartsWith(search))
                .OrderBy(e => e.InfrafamilyName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfrafamilyName.StartsWith(search))
            .OrderBy(e => e.InfrafamilyName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertInfrafamilyAsync(Tbl51Infrafamily uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily uiEntity)
    {
        return UpsertAsync<Tbl51Infrafamily, int>(
            uiEntity,
            ctx => ctx.Set<Tbl51Infrafamily>(),
            x => x.InfrafamilyId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfrafamilyId == id),
            MapInfrafamilyFields
        );
    }
    // ---------------------------------------------------------                  
    // MapInfrafamilyFields(Tbl51Infrafamily source, Tbl51Infrafamily target)
    // ---------------------------------------------------------                 
    private static void MapInfrafamilyFields(Tbl51Infrafamily source, Tbl51Infrafamily target)
    {
        target.InfrafamilyName = source.InfrafamilyName;
        target.SubfamilyId = source.SubfamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetInfrafamilyByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl51Infrafamily?> GetInfrafamilyByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfrafamilyId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteInfrafamilyAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfrafamilyAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl51Infrafamilies.FindAsync(id);
        if (entity != null)
        {
            context.Tbl51Infrafamilies.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl51Infrafamily entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyExistsAsync(Tbl51Infrafamily entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies.AnyAsync(x =>
            x.InfrafamilyName == entity.InfrafamilyName &&
            x.SubfamilyId == entity.SubfamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfrafamilyId != entity.InfrafamilyId
        );
    }

    // ---------------------------------------------------------                  
    // HasSupertribusAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasSupertribusAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl54Supertribusses
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasReferenceAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfrafamilyHasCommentAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfrafamilyId == infrafamilyId);
    }

    // ---------------------------------------------------------                  
    // GetInfrafamilyForSubfamilyAsync(int subfamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl51Infrafamily>> GetInfrafamilyForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl51Infrafamilies
            .Where(e => e.SubfamilyId == subfamilyId)
            .Include(e => e.Subfamily)
            .OrderBy(e => e.InfrafamilyName)
            .AsNoTracking()
            .ToListAsync();
    }

    #endregion Infrafamily                       

    #region Supertribus                
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSupertribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl54Supertribus>> GetAllSupertribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .AsNoTracking()
            .OrderBy(e => e.SupertribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSupertribusWithInfrafamilyAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl54Supertribus>> SearchSupertribusWithInfrafamilyAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl54Supertribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl54Supertribusses
            .Include(x => x.Infrafamily)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SupertribusId == id || e.SupertribusName.StartsWith(search))
                .OrderBy(e => e.SupertribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SupertribusName.StartsWith(search))
            .OrderBy(e => e.SupertribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSupertribusAsync(Tbl54Supertribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus uiEntity)
    {
        return UpsertAsync<Tbl54Supertribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl54Supertribus>(),
            x => x.SupertribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SupertribusId == id),
            MapSupertribusFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSupertribusFields(Tbl54Supertribus source, Tbl54Supertribus target)
    // ---------------------------------------------------------                 
    private static void MapSupertribusFields(Tbl54Supertribus source, Tbl54Supertribus target)
    {
        target.SupertribusName = source.SupertribusName;
        target.InfrafamilyId = source.InfrafamilyId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSupertribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl54Supertribus?> GetSupertribusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SupertribusId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSupertribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSupertribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl54Supertribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl54Supertribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl54Supertribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusExistsAsync(Tbl54Supertribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses.AnyAsync(x =>
            x.SupertribusName == entity.SupertribusName &&
            x.InfrafamilyId == entity.InfrafamilyId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SupertribusId != entity.SupertribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasTribusAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasTribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl57Tribusses
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasReferenceAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SupertribusHasCommentAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SupertribusId == supertribusId);
    }

    // ---------------------------------------------------------                  
    // GetSupertribusForInfrafamilyAsync(int infrafamilyId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl54Supertribus>> GetSupertribusForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl54Supertribusses
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .Include(e => e.Infrafamily)
            .OrderBy(e => e.SupertribusName)
            .AsNoTracking()
            .ToListAsync();
    }



    //alt
    public async Task<IEnumerable<Tbl54Supertribus>> GetSupertribusAsync(int dummy)
    {
        return await _context.Tbl54Supertribusses
            .OrderBy(x => x.SupertribusName)
            .ToListAsync();
    }
    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync()
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
            .AsNoTracking()
            .OrderBy(a => a.SupertribusName));
        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id))
            : new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => search != null && e.SupertribusName.StartsWith(search))
                .OrderBy(a => a.SupertribusName));
        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusFromInfrafamilyIdAsync(int infrafamilyid)
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
            .Where(e => e.InfrafamilyId == infrafamilyid)
            .OrderBy(k => k.SupertribusName)
        );

        return collection;
    }

    #endregion Supertribus                       

    #region Tribus                
    //neu

    // ---------------------------------------------------------                  
    // Tribus 
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllTribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl57Tribus>> GetAllTribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .AsNoTracking()
            .OrderBy(e => e.TribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchTribusWithSupertribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl57Tribus>> SearchTribusWithSupertribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl57Tribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl57Tribusses
            .Include(x => x.Supertribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.TribusId == id || e.TribusName.StartsWith(search))
                .OrderBy(e => e.TribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.TribusName.StartsWith(search))
            .OrderBy(e => e.TribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertTribusAsync(Tbl57Tribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus uiEntity)
    {
        return UpsertAsync<Tbl57Tribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl57Tribus>(),
            x => x.TribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.TribusId == id),
            MapTribusFields
        );
    }
    // ---------------------------------------------------------                  
    // MapTribusFields(Tbl57Tribus source, Tbl57Tribus target)
    // ---------------------------------------------------------                 
    private static void MapTribusFields(Tbl57Tribus source, Tbl57Tribus target)
    {
        target.TribusName = source.TribusName;
        target.SupertribusId = source.SupertribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetTribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl57Tribus?> GetTribusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TribusId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteTribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteTribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl57Tribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl57Tribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl57Tribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusExistsAsync(Tbl57Tribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses.AnyAsync(x =>
            x.TribusName == entity.TribusName &&
            x.SupertribusId == entity.SupertribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.TribusId != entity.TribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasSubtribusAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasSubtribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl60Subtribusses
            .AnyAsync(x => x.TribusId == tribusId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasReferenceAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.TribusId == tribusId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> TribusHasCommentAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.TribusId == tribusId);
    }

    // ---------------------------------------------------------                  
    // GetTribusForSupertribusAsync(int supertribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl57Tribus>> GetTribusForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .Where(e => e.SupertribusId == supertribusId)
            .Include(e => e.Supertribus)
            .OrderBy(e => e.TribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    //----------------------------------------------------
    //alt
    public async Task<IEnumerable<Tbl57Tribus>> SearchTribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl57Tribus>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .Where(e =>
                e.TribusName.StartsWith(search) ||
                e.TribusId.ToString() == search)
            .Include(e => e.Supertribus)
            .OrderBy(e => e.TribusName)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Tbl57Tribus> GetTribusByTribusIdAsync(int tribusId)
    {
        return await _context.Tbl57Tribusses
            .FirstOrDefaultAsync(x => x.TribusId == tribusId);
    }
    public async Task<string> GetTribusNameByTribusId(int tribusId)
    {
        var tribus = await GetTribusByTribusIdAsync(tribusId);
        return tribus?.TribusName ?? "Unknown";  // ✓ Null-safe
    }


    public async Task<IEnumerable<Tbl57Tribus>> GetTribusAsync(int dummy)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl57Tribusses
            .Include(e => e.Supertribus)
            .OrderBy(e => e.TribusName)
            .AsNoTracking()
            .ToListAsync();
    }
    public ObservableCollection<Tbl57Tribus> GetTribusAsync()
    {
        var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
            .AsNoTracking()
            .OrderBy(a => a.TribusName));
        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => e.TribusId == id))
            : new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => search != null && e.TribusName.StartsWith(search))
                .OrderBy(a => a.TribusName));
        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusFromSupertribusIdAsync(int supertribusid)
    {
        var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
            .Where(e => e.SupertribusId == supertribusid)
            .OrderBy(k => k.TribusName)
        );

        return collection;
    }



    #endregion Tribus                       

    #region Subtribus                

    // ---------------------------------------------------------                  
    // Subtribus 
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSubtribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl60Subtribus>> GetAllSubtribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .AsNoTracking()
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSubtribusWithTribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusWithTribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl60Subtribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl60Subtribusses
            .Include(x => x.Tribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SubtribusId == id || e.SubtribusName.StartsWith(search))
                .OrderBy(e => e.SubtribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SubtribusName.StartsWith(search))
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSubtribusAsync(Tbl60Subtribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus uiEntity)
    {
        return UpsertAsync<Tbl60Subtribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl60Subtribus>(),
            x => x.SubtribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SubtribusId == id),
            MapSubtribusFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSubtribusFields(Tbl60Subtribus source, Tbl60Subtribus target)
    // ---------------------------------------------------------                 
    private static void MapSubtribusFields(Tbl60Subtribus source, Tbl60Subtribus target)
    {
        target.SubtribusName = source.SubtribusName;
        target.TribusId = source.TribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetSubtribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl60Subtribus?> GetSubtribusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SubtribusId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSubtribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSubtribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl60Subtribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl60Subtribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl60Subtribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusExistsAsync(Tbl60Subtribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses.AnyAsync(x =>
            x.SubtribusName == entity.SubtribusName &&
            x.TribusId == entity.TribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.SubtribusId != entity.SubtribusId
        );
    }

    // ---------------------------------------------------------                  
    // HasInfratribusAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasInfratribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl63Infratribusses
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasReferenceAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> SubtribusHasCommentAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SubtribusId == subtribusId);
    }

    // ---------------------------------------------------------                  
    // GetSubtribusForTribusAsync(int tribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl60Subtribus>> GetSubtribusForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .Where(e => e.TribusId == tribusId)
            .Include(e => e.Tribus)
            .OrderBy(e => e.SubtribusName)
            .AsNoTracking()
            .ToListAsync();
    }

//alt
    ////---------------------------------------------------------
    public async Task<IEnumerable<Tbl60Subtribus>> GetSubtribusAsync(int dummy)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .Include(e => e.Tribus)
            .OrderBy(e => e.SubtribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusAsync(string? search)

    //-------------------------------------------
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl60Subtribus>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl60Subtribusses
            .Where(e =>
                e.SubtribusName.StartsWith(search) ||
                e.SubtribusId.ToString() == search)
            .Include(e => e.Tribus)
            .OrderBy(e => e.SubtribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync()
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
            .AsNoTracking()
            .OrderBy(a => a.SubtribusName));
        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id))
            : new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => search != null && e.SubtribusName.StartsWith(search))
                .OrderBy(a => a.SubtribusName));
        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusFromTribusIdAsync(int tribusid)
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
            .Where(e => e.TribusId == tribusid)
            .OrderBy(k => k.SubtribusName)
        );

        return collection;
    }


    #endregion Subtribus                       

    #region Infratribus
    //neu
    // ---------------------------------------------------------                  
    // Infratribus 
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllInfratribusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl63Infratribus>> GetAllInfratribusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .AsNoTracking()
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchInfratribusWithSubtribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusWithSubtribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl63Infratribus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl63Infratribusses
            .Include(x => x.Subtribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.InfratribusId == id || e.InfratribusName.StartsWith(search))
                .OrderBy(e => e.InfratribusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.InfratribusName.StartsWith(search))
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertInfratribusAsync(Tbl63Infratribus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus uiEntity)
    {
        return UpsertAsync<Tbl63Infratribus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl63Infratribus>(),
            x => x.InfratribusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.InfratribusId == id),
            MapInfratribusFields
        );
    }
    // ---------------------------------------------------------                  
    // MapInfratribusFields(Tbl63Infratribus source, Tbl63Infratribus target)
    // ---------------------------------------------------------                 
    private static void MapInfratribusFields(Tbl63Infratribus source, Tbl63Infratribus target)
    {
        target.InfratribusName = source.InfratribusName;
        target.SubtribusId = source.SubtribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetInfratribusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl63Infratribus?> GetInfratribusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InfratribusId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteInfratribusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteInfratribusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl63Infratribusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl63Infratribusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl63Infratribus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusExistsAsync(Tbl63Infratribus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses.AnyAsync(x =>
            x.InfratribusName == entity.InfratribusName &&
            x.SubtribusId == entity.SubtribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.InfratribusId != entity.InfratribusId
        );
    }


    // ---------------------------------------------------------                  
    // HasGenusAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasGenusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl66Genusses
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasReferenceAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<bool> InfratribusHasCommentAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.InfratribusId == infratribusId);
    }


    // ---------------------------------------------------------                  
    // GetInfratribusForSubtribusAsync(int subtribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl63Infratribus>> GetInfratribusForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .Where(e => e.SubtribusId == subtribusId)
            .Include(e => e.Subtribus)
            .OrderBy(e => e.InfratribusName)
            .AsNoTracking()
            .ToListAsync();
    }

    //-------------------------------------------------------
    //alt

    public async Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl63Infratribus>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .Where(e =>
                e.InfratribusName.StartsWith(search) ||
                e.InfratribusId.ToString() == search)
            .Include(e => e.Subtribus)
            .OrderBy(e => e.InfratribusName)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl63Infratribus>> GetInfratribusAsync(int dummy)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl63Infratribusses
            .OrderBy(x => x.InfratribusName)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<bool> HasGenusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .AnyAsync(g => g.InfratribusId == infratribusId);
    }

    public async Task<ObservableCollection<Tbl63Infratribus>> GetInfratribusFromSubtribusIdAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        var list = await context.Tbl63Infratribusses
            .Where(e => e.SubtribusId == subtribusId)
            .Include(e => e.Subtribus)
            .OrderBy(e => e.InfratribusName)
            .AsNoTracking()
            .ToListAsync();

        return new ObservableCollection<Tbl63Infratribus>(list);
    }

    public ObservableCollection<Tbl63Infratribus> GetInfratribusAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
            .OrderBy(k => k.InfratribusName));
        return collection;
    }


    #endregion Infratribus                       

    #region Genus                
    //Neu

    // ---------------------------------------------------------                  
    // Genus 
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllGenusAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl66Genus>> GetAllGenusAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchGenusWithInfratribusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl66Genus>> SearchGenusWithInfratribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl66Genus>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl66Genusses
            .Include(x => x.Infratribus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.GenusId == id || e.GenusName.StartsWith(search))
                .OrderBy(e => e.GenusName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.GenusName.StartsWith(search))
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertGenusAsync(Tbl66Genus uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus uiEntity)
    {
        return UpsertAsync<Tbl66Genus, int>(
            uiEntity,
            ctx => ctx.Set<Tbl66Genus>(),
            x => x.GenusId,
            (set, id) => set.FirstOrDefaultAsync(x => x.GenusId == id),
            MapGenusFields
        );
    }
    // ---------------------------------------------------------                  
    // MapGenusFields(Tbl66Genus source, Tbl66Genus target)
    // ---------------------------------------------------------                 
    private static void MapGenusFields(Tbl66Genus source, Tbl66Genus target)
    {
        target.GenusName = source.GenusName;
        target.InfratribusId = source.InfratribusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Synonym = source.Synonym;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
        target.Info = source.Info;
        target.EngName = source.EngName;
        target.GerName = source.GerName;
        target.FraName = source.FraName;
        target.PorName = source.PorName;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetGenusByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl66Genus?> GetGenusByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GenusId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteGenusAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteGenusAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl66Genusses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl66Genusses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl66Genus entity)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusExistsAsync(Tbl66Genus entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses.AnyAsync(x =>
            x.GenusName == entity.GenusName &&
            x.InfratribusId == entity.InfratribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Synonym == entity.Synonym &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.Info == entity.Info &&
            x.EngName == entity.EngName &&
            x.GerName == entity.GerName &&
            x.FraName == entity.FraName &&
            x.PorName == entity.PorName &&
            x.Memo == entity.Memo &&
            x.GenusId != entity.GenusId
        );
    }

    // ---------------------------------------------------------                  
    // HasSpeciesAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasSpeciesAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl75Speciesses
            .AnyAsync(x => x.GenusId == genusId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasReferenceAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.GenusId == genusId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<bool> GenusHasCommentAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.GenusId == genusId);
    }

    // ---------------------------------------------------------                  
    // GetGenusForInfratribusAsync(int infratribusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl66Genus>> GetGenusForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .Where(e => e.InfratribusId == infratribusId)
            .Include(e => e.Infratribus)
            .OrderBy(e => e.GenusName)
            .AsNoTracking()
            .ToListAsync();
    }

    //---------------------------------------
    //Alt
    public async Task<IEnumerable<Tbl66Genus>> GetGenusAsync(int dummy)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .OrderBy(x => x.GenusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl66Genus>> SearchGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl66Genus>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl66Genusses
            .Where(e =>
                e.GenusName.StartsWith(search) ||
                e.GenusId.ToString() == search)
            .Include(e => e.Infratribus)
            .OrderBy(e => e.GenusName)
            .AsNoTracking()
            .ToListAsync();
    }
    public ObservableCollection<Tbl66Genus> GetGenusAsync()
    {
        var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(a => a.GenusName));
        return collection;
    }

    //public ObservableCollection<Tbl66Genus> GetGenusAsync(int genusid)
    //{
    //    var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
    //        .AsNoTracking()
    //        .Where(e => e.GenusId == genusid)
    //        .OrderBy(k => k.GenusName)
    //    );

    //    return collection;
    //}

    public ObservableCollection<Tbl66Genus> GetGenusAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => e.GenusId == id))
            : new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => search != null && e.GenusName.StartsWith(search))
                .OrderBy(a => a.GenusName));
        return collection;
    }

    public ObservableCollection<Tbl66Genus> GetGenusFromInfratribusIdAsync(int infratribusid)
    {
        var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
            .Where(e => e.InfratribusId == infratribusid)
            .OrderBy(k => k.GenusName)
        );

        return collection;
    }

    //public async Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus genus)
    //{
    //    var current = await _context.Tbl66Genusses.FirstOrDefaultAsync(c => c.GenusId == genus.GenusId);
    //    if (null == current)
    //    {
    //        genus.CountId = RandomHelper.Randomnumber();
    //        genus.Writer = Environment.UserName;
    //        genus.WriterDate = DateTime.Now;
    //        genus.Updater = Environment.UserName;
    //        genus.UpdaterDate = DateTime.Now;
    //        _context.Tbl66Genusses.Add(genus);
    //    }
    //    else
    //    {
    //        genus.Updater = Environment.UserName;
    //        genus.UpdaterDate = DateTime.Now;
    //        _context.Entry(current).CurrentValues.SetValues(genus);
    //    }
    //    await _context.SaveChangesAsync();
    //    return genus;
    //}

    //public async Task DeleteGenusAsync(int genusid)
    //{
    //    var genus = await _context.Tbl66Genusses.FirstOrDefaultAsync(b => b.GenusId == genusid);
    //    if (genus != null)
    //    {
    //        //necessary to delete before
    //        //------ Species -------------
    //        var speciess = await _context.Tbl75Speciesses.Where(a => a.GenusId == genusid).ToListAsync();
    //        foreach (var species in speciess)
    //        {
    //            for (var i = 0; i <= speciess.Count; i++)
    //            {
    //                var speciesid = species.SpeciesId;
    //                var singleref = _context.Tbl90References.SingleOrDefault(a => a.SpeciesId == speciesid);
    //                if (singleref != null)
    //                {
    //                    _context.Tbl90References.Remove(singleref);
    //                    await _context.SaveChangesAsync();
    //                }
    //                var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SpeciesId == speciesid);
    //                if (singlecom != null)
    //                {
    //                    _context.Tbl93Comments.Remove(singlecom);
    //                    await _context.SaveChangesAsync();
    //                }
    //            }
    //        }
    //        _context.Tbl75Speciesses.RemoveRange(speciess);
    //        await _context.SaveChangesAsync();
    //        //---------------------------------------------------------
    //        var references = await _context.Tbl90References.Where(reference => reference.GenusId == genusid).ToListAsync();
    //        _context.Tbl90References.RemoveRange(references);
    //        await _context.SaveChangesAsync();

    //        var comments = await _context.Tbl93Comments.Where(comment => comment.GenusId == genusid).ToListAsync();
    //        _context.Tbl93Comments.RemoveRange(comments);
    //        await _context.SaveChangesAsync();

    //        _context.Tbl66Genusses.Remove(genus);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    #endregion Genus

    #region Speciesgroup                

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync()
    {
        var collection = new ObservableCollection<Tbl68Speciesgroup>(_context.Tbl68Speciesgroups
            .AsNoTracking()
            .OrderBy(a => a.SpeciesgroupName)
            .ThenBy(a => a.Subspeciesgroup));
        return collection;
    }

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(int speciesgroupid)
    {
        var collection = new ObservableCollection<Tbl68Speciesgroup>(_context.Tbl68Speciesgroups
            .AsNoTracking()
            .Where(e => e.SpeciesgroupId == speciesgroupid)
            .OrderBy(k => k.SpeciesgroupName)
            .ThenBy(a => a.Subspeciesgroup));

        return collection;
    }

    public ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl68Speciesgroup>(_context.Tbl68Speciesgroups
                .Where(e => e.SpeciesgroupId == id))
            : new ObservableCollection<Tbl68Speciesgroup>(_context.Tbl68Speciesgroups
                .Where(e => search != null && e.SpeciesgroupName.StartsWith(search))
                .OrderBy(a => a.SpeciesgroupName)
                .ThenBy(a => a.Subspeciesgroup));
        return collection;
    }

    public async Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup speciesgroup)
    {
        var current = await _context.Tbl68Speciesgroups.FirstOrDefaultAsync(c => c.SpeciesgroupId == speciesgroup.SpeciesgroupId);
        if (null == current)
        {
            speciesgroup.CountId = RandomHelper.Randomnumber();
            speciesgroup.Writer = Environment.UserName;
            speciesgroup.WriterDate = DateTime.Now;
            speciesgroup.Updater = Environment.UserName;
            speciesgroup.UpdaterDate = DateTime.Now;
            _context.Tbl68Speciesgroups.Add(speciesgroup);
        }
        else
        {
            speciesgroup.Updater = Environment.UserName;
            speciesgroup.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(speciesgroup);
        }
        await _context.SaveChangesAsync();
        return speciesgroup;
    }

    public async Task DeleteSpeciesgroupAsync(int speciesgroupid)
    {
        var speciesgroup = await _context.Tbl68Speciesgroups.FirstOrDefaultAsync(b => b.SpeciesgroupId == speciesgroupid);
        if (speciesgroup != null)
        {
            //necessary to delete before
            //------ FiSpecies -------------
            //var fispeciess = await _context.Tbl69FiSpeciesses.Where(a => a.SpeciesgroupId == speciesgroupid).ToListAsync();
            //foreach (var fispecies in fispeciess)
            //{
            //    for (var i = 0; i <= fispeciess.Count; i++)
            //    {
            //        var fispeciesid = fispecies.FiSpeciesId;
            //        var singleref = _context.Tbl90References.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
            //        if (singleref != null)
            //        {
            //            _context.Tbl90References.Remove(singleref);
            //            await _context.SaveChangesAsync();
            //        }
            //        var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.FiSpeciesId == fispeciesid);
            //        if (singlecom != null)
            //        {
            //            _context.Tbl93Comments.Remove(singlecom);
            //            await _context.SaveChangesAsync();
            //        }
            //    }
            //}
            //_context.Tbl69FiSpeciesses.RemoveRange(fispeciess);
            //await _context.SaveChangesAsync();

            _context.Tbl68Speciesgroups.Remove(speciesgroup);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Speciesgroup

    #region Species

    // ---------------------------------------------------------                  
    // Species 
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllSpeciesAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl75Species>> GetAllSpeciesAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .AsNoTracking()
            .OrderBy(e => e.SpeciesName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchSpeciesWithGenusAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl75Species>> SearchSpeciesWithGenusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl75Species>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl75Speciesses
            .Include(x => x.Genus)
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.SpeciesId == id || e.SpeciesName.StartsWith(search))
                .OrderBy(e => e.SpeciesName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.SpeciesName.StartsWith(search))
            .OrderBy(e => e.SpeciesName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertSpeciesAsync(Tbl75Species uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species uiEntity)
    {
        return UpsertAsync<Tbl75Species, int>(
            uiEntity,
            ctx => ctx.Set<Tbl75Species>(),
            x => x.SpeciesId,
            (set, id) => set.FirstOrDefaultAsync(x => x.SpeciesId == id),
            MapSpeciesFields
        );
    }
    // ---------------------------------------------------------                  
    // MapSpeciesFields(Tbl75Species source, Tbl75Species target)
    // ---------------------------------------------------------                 
    private static void MapSpeciesFields(Tbl75Species source, Tbl75Species target)
    {
        target.SpeciesName = source.SpeciesName;
        target.GenusId = source.GenusId;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Author = source.Author;
        target.AuthorYear = source.AuthorYear;
    }
    // ---------------------------------------------------------                  
    // GetSpeciesByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl75Species?> GetSpeciesByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SpeciesId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteSpeciesAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteSpeciesAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl75Speciesses.FindAsync(id);
        if (entity != null)
        {
            context.Tbl75Speciesses.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl75Species entity)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesExistsAsync(Tbl75Species entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses.AnyAsync(x =>
            x.SpeciesName == entity.SpeciesName &&
            x.GenusId == entity.GenusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Author == entity.Author &&
            x.AuthorYear == entity.AuthorYear &&
            x.SpeciesId != entity.SpeciesId
        );
    }

    // ---------------------------------------------------------                  
    // HasNameAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasNameAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl78Names
            .AnyAsync(x => x.SpeciesId == speciesId);
    }
    // ---------------------------------------------------------                  
    // HasReferenceAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasReferenceAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .AnyAsync(x => x.SpeciesId == speciesId);
    }
    // ---------------------------------------------------------                  
    // HasCommentAsync(int speciesId)
    // ---------------------------------------------------------                 
    public async Task<bool> SpeciesHasCommentAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl93Comments
            .AnyAsync(x => x.SpeciesId == speciesId);
    }

    // ---------------------------------------------------------                  
    // GetSpeciesForGenusAsync(int genusId)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl75Species>> GetSpeciesForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl75Speciesses
            .Where(e => e.GenusId == genusId)
            .Include(e => e.Genus)
            .OrderBy(e => e.SpeciesName)
            .AsNoTracking()
            .ToListAsync();
    }



    //Neu
    //public async Task<IEnumerable<Tbl75Species>> GetAllSpeciesAsync()
    //{
    //    await using var context = _factory.CreateDbContext();

    //    return await context.Tbl75Speciesses
    //        .Include(e => e.Genus)
    //        .OrderBy(a => a.Genus.GenusName)
    //        .ThenBy(a => a.SpeciesName)
    //        .ThenBy(a => a.Subspecies)
    //        .ThenBy(a => a.Divers)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}
    //public async Task<IEnumerable<Tbl75Species>> GetSpeciesForGenusAsync(int genusId)
    //{
    //    await using var context = _factory.CreateDbContext();

    //    return await context.Tbl75Speciesses
    //        .Where(e => e.GenusId == genusId)
    //        .OrderBy(a => a.Genus.GenusName)
    //        .ThenBy(a => a.SpeciesName)
    //        .ThenBy(a => a.Subspecies)
    //        .ThenBy(a => a.Divers)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}
    //public async Task<IEnumerable<Tbl75Species>> SearchSpeciesAsync(string? search)
    //{
    //    if (string.IsNullOrWhiteSpace(search))
    //        return Enumerable.Empty<Tbl75Species>();

    //    await using var context = _factory.CreateDbContext();

    //    return await context.Tbl75Speciesses
    //        .Where(e =>
    //            e.SpeciesName.StartsWith(search) ||
    //            e.SpeciesId.ToString() == search)
    //        .Include(e => e.Genus)
    //        .OrderBy(a => a.Genus.GenusName)
    //        .ThenBy(a => a.SpeciesName)
    //        .ThenBy(a => a.Subspecies)
    //        .ThenBy(a => a.Divers)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}
    //public async Task<IEnumerable<Tbl75Species>> SearchSpeciesWithGenusAsync(string? search)
    //{
    //    if (string.IsNullOrWhiteSpace(search))
    //        return Enumerable.Empty<Tbl75Species>();

    //    await using var context = _factory.CreateDbContext();

    //    return await context.Tbl75Speciesses
    //        .Where(e =>
    //            e.SpeciesName.StartsWith(search) ||
    //            e.SpeciesId.ToString() == search)
    //        .Include(e => e.Genus)
    //        .OrderBy(a => a.Genus.GenusName)
    //        .ThenBy(a => a.SpeciesName)
    //        .ThenBy(a => a.Subspecies)
    //        .ThenBy(a => a.Divers)
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    //public async Task<IEnumerable<Tbl75Species>> GetSpeciesAsync(int dummy)
    //{
    //    await using var context = _factory.CreateDbContext();

    //    return await context.Tbl75Speciesses
    //        .OrderBy(x => x.SpeciesName)
    //        .ToListAsync();
    //}
    //public async Task<bool> SpeciesHasReferenceAsync(int speciesId)
    //{
    //    await using var context = _factory.CreateDbContext();
    //    return await context.Tbl90References.AnyAsync(x => x.SpeciesId == speciesId);
    //}
    //public async Task<bool> SpeciesHasCommentAsync(int speciesId)
    //{
    //    await using var context = _factory.CreateDbContext();
    //    return await context.Tbl93Comments.AnyAsync(x => x.SpeciesId == speciesId);
    //}

    //-----------------------------
    //alt
    public ObservableCollection<Tbl75Species> GetSpeciesAsync()
    {
        var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
            .OrderBy(a => a.Genus.GenusName)
            .ThenBy(a => a.SpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));
        return collection;
    }
    public ObservableCollection<Tbl75Species> GetSpeciesAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => e.SpeciesId == id))
            : new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => search != null && e.SpeciesName.StartsWith(search))
                .OrderBy(a => a.Genus.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
        return collection;
    }
    //public ObservableCollection<Tbl75Species> SearchSpeciesWithGenusAsync(string search)
    //{
    //    //search about GenusName
    //    var collection = int.TryParse(search, out var id)
    //        ? new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
    //            .Where(e => e.SpeciesId == id))
    //        : new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
    //            .Where(e => e.Tbl66Genusses.GenusName.StartsWith(search))
    //            .OrderBy(a => a.Tbl66Genusses.GenusName)
    //            .ThenBy(a => a.SpeciesName)
    //            .ThenBy(a => a.Subspecies)
    //            .ThenBy(a => a.Divers)
    //        );
    //    return collection;
    //}

    public ObservableCollection<Tbl75Species> GetSpeciesFromGenusIdAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.SpeciesName)
        );

        return collection;
    }
    //public async Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species species)
    //{
    //    var current = await _context.Tbl75Speciesses.FirstOrDefaultAsync(c => c.SpeciesId == species.SpeciesId);
    //    if (null == current)
    //    {
    //        species.CountId = RandomHelper.Randomnumber();
    //        species.Writer = Environment.UserName;
    //        species.WriterDate = DateTime.Now;
    //        species.Updater = Environment.UserName;
    //        species.UpdaterDate = DateTime.Now;
    //        _context.Tbl75Speciesses.Add(species);
    //    }
    //    else
    //    {
    //        species.Updater = Environment.UserName;
    //        species.UpdaterDate = DateTime.Now;
    //        _context.Entry(current).CurrentValues.SetValues(species);
    //    }
    //    await _context.SaveChangesAsync();
    //    return species;
    //}
    //public async Task DeleteSpeciesAsync(int speciesid)
    //{
    //    var species = await _context.Tbl75Speciesses.FirstOrDefaultAsync(b => b.SpeciesId == speciesid);
    //    if (species != null)
    //    {
    //        //necessary to delete before
    //        var references = await _context.Tbl90References.Where(reference => reference.SpeciesId == speciesid).ToListAsync();
    //        _context.Tbl90References.RemoveRange(references);
    //        await _context.SaveChangesAsync();

    //        var comments = await _context.Tbl93Comments.Where(comment => comment.SpeciesId == speciesid).ToListAsync();
    //        _context.Tbl93Comments.RemoveRange(comments);
    //        await _context.SaveChangesAsync();

    //        _context.Tbl75Speciesses.Remove(species);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    #endregion Species                       

    #region Name                  

    public ObservableCollection<Tbl78Name> GetNameAsync()
    {
        var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
            .AsNoTracking()
            .OrderBy(a => a.NameName));
        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameAsync(int nameid)
    {
        var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
            .AsNoTracking()
            .Where(e => e.NameId == nameid)
            .OrderBy(k => k.NameName)
        );

        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl78Name>(_context.Tbl78Names
                .Where(e => e.NameId == id))
            : new ObservableCollection<Tbl78Name>(_context.Tbl78Names
                .Where(e => search != null && e.NameName.StartsWith(search))
                .OrderBy(a => a.NameName));
        return collection;
    }

    public ObservableCollection<Tbl78Name> GetNameFromSpeciesIdAsync(int speciesid)
    {
        var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
            .Where(e => e.SpeciesId == speciesid)
            .OrderBy(k => k.NameName)
        );

        return collection;
    }

    //public ObservableCollection<Tbl78Name> GetNameFromPlSpeciesIdAsync(int plspeciesid)
    //{
    //    var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
    //        .Where(e => e.PlSpeciesId == plspeciesid)
    //        .OrderBy(k => k.NameName)
    //    );

    //    return collection;
    //}

    public async Task<Tbl78Name> UpsertNameAsync(Tbl78Name name)
    {
        var current = await _context.Tbl78Names.FirstOrDefaultAsync(c => c.NameId == name.NameId);
        if (null == current)
        {
            name.CountId = RandomHelper.Randomnumber();
            name.Writer = Environment.UserName;
            name.WriterDate = DateTime.Now;
            name.Updater = Environment.UserName;
            name.UpdaterDate = DateTime.Now;
            _context.Tbl78Names.Add(name);
        }
        else
        {
            name.Updater = Environment.UserName;
            name.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(name);
        }
        await _context.SaveChangesAsync();
        return name;
    }

    public async Task DeleteNameAsync(int nameid)
    {
        var names = await _context.Tbl78Names.FirstOrDefaultAsync(b => b.NameId == nameid);
        if (names != null)
        {
            _context.Tbl78Names.Remove(names);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Name

    #region Image                  

    public ObservableCollection<Tbl81Image> GetImageAsync()
    {
        var collection = new ObservableCollection<Tbl81Image>(_context.Tbl81Images
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageAsync(int imageid)
    {
        var collection = new ObservableCollection<Tbl81Image>(_context.Tbl81Images
            .AsNoTracking()
            .Where(e => e.ImageId == imageid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl81Image> GetImageAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl81Image>(_context.Tbl81Images
                .Where(e => e.ImageId == id))
            : new ObservableCollection<Tbl81Image>(_context.Tbl81Images
                .Where(e => search != null && e.Info.StartsWith(search))
                .OrderBy(a => a.Info));
        return collection;
    }

    //public ObservableCollection<Tbl81Image> GetImageFromFiSpeciesIdAsync(int fispeciesid)
    //{
    //    var collection = new ObservableCollection<Tbl81Image>(_context.Tbl81Images
    //        .Where(e => e.FiSpeciesId == fispeciesid)
    //        .OrderBy(k => k.Info)
    //    );

    //    return collection;
    //}

    public ObservableCollection<Tbl81Image> GetImageFromSpeciesIdAsync(int speciesid)
    {
        var collection = new ObservableCollection<Tbl81Image>(_context.Tbl81Images
            .Where(e => e.SpeciesId == speciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<Tbl81Image> UpsertImageAsync(Tbl81Image image, string selectedpath)
    {
        var current = await _context.Tbl81Images.FirstOrDefaultAsync(c => c.ImageId == image.ImageId);
        if (null == current)
        {
            image.CountId = RandomHelper.Randomnumber();
            image.Writer = Environment.UserName;
            image.WriterDate = DateTime.Now;
            image.Updater = Environment.UserName;
            image.UpdaterDate = DateTime.Now;
            image.FilestreamId = Guid.NewGuid();
            image.Filestream = LoadImageData(selectedpath);

            _context.Tbl81Images.Add(image);
        }
        else
        {
            image.Updater = Environment.UserName;
            image.UpdaterDate = DateTime.Now;
            image.Filestream = LoadImageData(selectedpath);
            _context.Entry(current).CurrentValues.SetValues(image);
        }
        await _context.SaveChangesAsync();
        return image;
    }
    private static byte[] LoadImageData(string filePath)
    {
        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var br = new BinaryReader(fs);
        var imageBytes = br.ReadBytes((int)fs.Length);
        br.Close();
        fs.Close();
        return imageBytes;
    }


    public async Task DeleteImageAsync(int imageid)
    {
        var images = await _context.Tbl81Images.FirstOrDefaultAsync(b => b.ImageId == imageid);
        if (images != null)
        {
            _context.Tbl81Images.Remove(images);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Image                  

    #region Synonym                  

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync()
    {
        var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
            .AsNoTracking()
            .OrderBy(a => a.SynonymName));
        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync(int synonymid)
    {
        var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
            .AsNoTracking()
            .Where(e => e.SynonymId == synonymid)
            .OrderBy(k => k.SynonymName)
        );

        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
                .Where(e => e.SynonymId == id))
            : new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
                .Where(e => search != null && e.SynonymName.StartsWith(search))
                .OrderBy(a => a.SynonymName));
        return collection;
    }

    public ObservableCollection<Tbl84Synonym> GetSynonymFromSpeciesIdAsync(int speciesid)
    {
        var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
            .Where(e => e.SpeciesId == speciesid)
            .OrderBy(k => k.SynonymName)
        );

        return collection;
    }

    //public ObservableCollection<Tbl84Synonym> GetSynonymFromPlSpeciesIdAsync(int plspeciesid)
    //{
    //    var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
    //        .Where(e => e.PlSpeciesId == plspeciesid)
    //        .OrderBy(k => k.SynonymName)
    //    );

    //    return collection;
    //}

    public async Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym synonym)
    {
        var current = await _context.Tbl84Synonyms.FirstOrDefaultAsync(c => c.SynonymId == synonym.SynonymId);
        if (null == current)
        {
            synonym.CountId = RandomHelper.Randomnumber();
            synonym.Writer = Environment.UserName;
            synonym.WriterDate = DateTime.Now;
            synonym.Updater = Environment.UserName;
            synonym.UpdaterDate = DateTime.Now;
            _context.Tbl84Synonyms.Add(synonym);
        }
        else
        {
            synonym.Updater = Environment.UserName;
            synonym.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(synonym);
        }
        await _context.SaveChangesAsync();
        return synonym;
    }

    public async Task DeleteSynonymAsync(int synonymid)
    {
        var synonyms = await _context.Tbl84Synonyms.FirstOrDefaultAsync(b => b.SynonymId == synonymid);
        if (synonyms != null)
        {
            _context.Tbl84Synonyms.Remove(synonyms);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Name                  

    #region Geographic                  

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync()
    {
        var collection = new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync(int geographicid)
    {
        var collection = new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
            .AsNoTracking()
            .Where(e => e.GeographicId == geographicid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
                .Where(e => e.GeographicId == id))
            : new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
                .Where(e => search != null && e.Info.StartsWith(search))
                .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl87Geographic> GetGeographicFromSpeciesIdAsync(int speciesid)
    {
        var collection = new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
            .Where(e => e.SpeciesId == speciesid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    //public ObservableCollection<Tbl87Geographic> GetGeographicFromPlSpeciesIdAsync(int plspeciesid)
    //{
    //    var collection = new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
    //        .Where(e => e.PlSpeciesId == plspeciesid)
    //        .OrderBy(k => k.Info)
    //    );

    //    return collection;
    //}

    public async Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic geographic)
    {
        var current = await _context.Tbl87Geographics.FirstOrDefaultAsync(c => c.GeographicId == geographic.GeographicId);
        if (null == current)
        {
            geographic.CountId = RandomHelper.Randomnumber();
            geographic.Writer = Environment.UserName;
            geographic.WriterDate = DateTime.Now;
            geographic.Updater = Environment.UserName;
            geographic.UpdaterDate = DateTime.Now;
            _context.Tbl87Geographics.Add(geographic);
        }
        else
        {
            geographic.Updater = Environment.UserName;
            geographic.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(geographic);
        }
        await _context.SaveChangesAsync();
        return geographic;
    }

    public async Task DeleteGeographicAsync(int geographicid)
    {
        var geographics = await _context.Tbl87Geographics.FirstOrDefaultAsync(b => b.GeographicId == geographicid);
        if (geographics != null)
        {
            _context.Tbl87Geographics.Remove(geographics);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Image                  


    #region Reference
    //---------------------------------------------------------------
    public async Task<IEnumerable<Tbl90Reference>> GetAllReferenceAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> SearchReferenceAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90Reference>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .Where(e =>
                e.Info!.StartsWith(search) ||
                e.ReferenceId.ToString() == search)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<Tbl90Reference> GetReferenceByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References
            .FirstOrDefaultAsync(x => x.ReferenceId == id);
    }
    public async Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference uiEntity)
    {
        await using var context = _factory.CreateDbContext();

        Tbl90Reference dbEntity;

        if (uiEntity.ReferenceId == 0)
        {
            dbEntity = new Tbl90Reference();
            context.Tbl90References.Add(dbEntity);
        }
        else
        {
            dbEntity = await context.Tbl90References
                           .FirstOrDefaultAsync(x => x.ReferenceId == uiEntity.ReferenceId)
                       ?? throw new InvalidOperationException("Entity not found in database.");
        }

        CopyProperties(uiEntity, dbEntity,
            nameof(Tbl90Reference.ReferenceId),
            nameof(Tbl90Reference.Writer),
            nameof(Tbl90Reference.WriterDate),
            nameof(Tbl90Reference.Updater),
            nameof(Tbl90Reference.UpdaterDate),
            nameof(Tbl90Reference.CountId)
        );

        var isNew = dbEntity.ReferenceId == 0;
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();

        return dbEntity;
    }
    public async Task DeleteReferenceAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90References.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90References.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    public async Task<bool> ReferenceExistsAsync(Tbl90Reference entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90References.AnyAsync(x =>
            x.Info == entity.Info &&
            x.SubtribusId == entity.SubtribusId &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Memo == entity.Memo &&
            x.ReferenceId != entity.ReferenceId
        );
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.RegnumId == regnumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.PhylumId == phylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubphylumId == subphylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SuperclassId == superclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForClassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.ClassId == classId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubclassId == subclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfraclassId == infraclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.LegioId == legioId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.OrdoId == ordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubordoId == subordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfraordoId == infraordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SuperfamilyId == superfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.FamilyId == familyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubfamilyId == subfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SupertribusId == supertribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SubtribusId == subtribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.TribusId == tribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.InfratribusId == infratribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.GenusId == genusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();
        return await context.Tbl90References
            .Where(e => e.SpeciesId == speciesId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task DeleteReferencesAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        var refs = context.Tbl90References
            .Where(r => r.InfratribusId == infratribusId);

        context.Tbl90References.RemoveRange(refs);
        await context.SaveChangesAsync();
    }

    //----------------------------------------------
    //alt
    public ObservableCollection<Tbl90Reference> GetReferenceAsync()
    {
        var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }
    public ObservableCollection<Tbl90Reference> GetReferenceAsync(int referenceid)
    {
        var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
            .AsNoTracking()
            .Where(e => e.ReferenceId == referenceid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }
    public Task<ObservableCollection<Tbl90Reference>> GetReferenceAsync(string search)
    {
        ObservableCollection<Tbl90Reference> collection = null!;
        collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
            .AsNoTracking()
            .Where(e => search != null && e.Info.StartsWith(search))
            .OrderBy(a => a.Info));

        return Task.FromResult(collection);
    }
    //public async Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference reference)
    //{
    //    var current = await _context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == reference.ReferenceId);
    //    if (null == current)
    //    {
    //        reference.CountId = RandomHelper.Randomnumber();
    //        reference.Writer = Environment.UserName;
    //        reference.WriterDate = DateTime.Now;
    //        reference.Updater = Environment.UserName;
    //        reference.UpdaterDate = DateTime.Now;
    //        _context.Tbl90References.Add(reference);
    //    }
    //    else
    //    {
    //        reference.Updater = Environment.UserName;
    //        reference.UpdaterDate = DateTime.Now;
    //        _context.Entry(current).CurrentValues.SetValues(reference);
    //    }
    //    await _context.SaveChangesAsync();
    //    return reference;
    //}
    //public async Task DeleteReferenceAsync(int referenceid)
    //{
    //    var reference = await _context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == referenceid);
    //    if (null != reference)
    //    {
    //        _context.Tbl90References.Remove(reference);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    #endregion Reference


    #region RefAuthor
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllRefAuthorAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefAuthor>> GetAllRefAuthorAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors
            .AsNoTracking()
            .OrderBy(e => e.RefAuthorName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchRefAuthorWithNULLAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefAuthor>> SearchRefAuthorAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefAuthor>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefAuthors
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefAuthorId == id || e.RefAuthorName.StartsWith(search))
                .OrderBy(e => e.RefAuthorName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefAuthorName.StartsWith(search))
            .OrderBy(e => e.RefAuthorName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertRefAuthorAsync(Tbl90RefAuthor uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor uiEntity)
    {
        return UpsertAsync<Tbl90RefAuthor, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefAuthor>(),
            x => x.RefAuthorId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefAuthorId == id),
            MapRefAuthorFields
        );
    }
    // ---------------------------------------------------------                  
    // MapRefAuthorFields(Tbl90RefAuthor source, Tbl90RefAuthor target)
    // ---------------------------------------------------------                 
    private static void MapRefAuthorFields(Tbl90RefAuthor source, Tbl90RefAuthor target)
    {
        target.RefAuthorName = source.RefAuthorName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetRefAuthorByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefAuthor?> GetRefAuthorByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefAuthorId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteRefAuthorAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefAuthorAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefAuthors.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefAuthors.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefAuthor entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefAuthorExistsAsync(Tbl90RefAuthor entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefAuthors.AnyAsync(x =>
            x.RefAuthorName == entity.RefAuthorName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefAuthorId != entity.RefAuthorId
        );
    }


    public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
            .OrderBy(k => k.RefAuthorName));
        return collection;
    }

    #endregion RefAuthor

    #region RefSource
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllRefSourceAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefSource>> GetAllRefSourceAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources
            .AsNoTracking()
            .OrderBy(e => e.RefSourceName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchRefSourceWithNULLAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefSource>> SearchRefSourceAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefSource>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefSources
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefSourceId == id || e.RefSourceName.StartsWith(search))
                .OrderBy(e => e.RefSourceName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefSourceName.StartsWith(search))
            .OrderBy(e => e.RefSourceName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertRefSourceAsync(Tbl90RefSource uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource uiEntity)
    {
        return UpsertAsync<Tbl90RefSource, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefSource>(),
            x => x.RefSourceId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefSourceId == id),
            MapRefSourceFields
        );
    }
    // ---------------------------------------------------------                  
    // MapRefSourceFields(Tbl90RefSource source, Tbl90RefSource target)
    // ---------------------------------------------------------                 
    private static void MapRefSourceFields(Tbl90RefSource source, Tbl90RefSource target)
    {
        target.RefSourceName = source.RefSourceName;
        target.Valid = source.Valid;
        target.Author = source.Author;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetRefSourceByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefSource?> GetRefSourceByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefSourceId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteRefSourceAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefSourceAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefSources.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefSources.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefSource entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefSourceExistsAsync(Tbl90RefSource entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefSources.AnyAsync(x =>
            x.RefSourceName == entity.RefSourceName &&
            x.Valid == entity.Valid &&
            x.Author == entity.Author &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefSourceId != entity.RefSourceId
        );
    }
    //alt----------------------
    public ObservableCollection<Tbl90RefSource> GetRefSourceAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefSource>(_context.Tbl90RefSources
            .OrderBy(k => k.RefSourceName));
        return collection;
    }

    #endregion

    #region RefExpert
    // ---------------------------------------------------------   
    // ---------------------------------------------------------                  
    // GetAllRefExpertAsync()
    // ---------------------------------------------------------                           
    public async Task<IEnumerable<Tbl90RefExpert>> GetAllRefExpertAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts
            .AsNoTracking()
            .OrderBy(e => e.RefExpertName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // SearchRefExpertWithNULLAsync(string? search)
    // ---------------------------------------------------------                 
    public async Task<IEnumerable<Tbl90RefExpert>> SearchRefExpertAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefExpert>();

        search = search.Trim();

        await using var context = _factory.CreateDbContext();

        var query = context.Tbl90RefExperts
            .AsNoTracking();

        if (int.TryParse(search, out var id))
        {
            return await query
                .Where(e => e.RefExpertId == id || e.RefExpertName.StartsWith(search))
                .OrderBy(e => e.RefExpertName)
                .ToListAsync();
        }

        return await query
            .Where(e => e.RefExpertName.StartsWith(search))
            .OrderBy(e => e.RefExpertName)
            .ToListAsync();
    }
    // ---------------------------------------------------------                  
    // UpsertRefExpertAsync(Tbl90RefExpert uiEntity)
    // ---------------------------------------------------------                 
    public Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert uiEntity)
    {
        return UpsertAsync<Tbl90RefExpert, int>(
            uiEntity,
            ctx => ctx.Set<Tbl90RefExpert>(),
            x => x.RefExpertId,
            (set, id) => set.FirstOrDefaultAsync(x => x.RefExpertId == id),
            MapRefExpertFields
        );
    }
    // ---------------------------------------------------------                  
    // MapRefExpertFields(Tbl90RefExpert source, Tbl90RefExpert target)
    // ---------------------------------------------------------                 
    private static void MapRefExpertFields(Tbl90RefExpert source, Tbl90RefExpert target)
    {
        target.RefExpertName = source.RefExpertName;
        target.Valid = source.Valid;
        target.ValidYear = source.ValidYear;
        target.Info = source.Info;
        target.Memo = source.Memo;
    }
    // ---------------------------------------------------------                  
    // GetRefExpertByIdAsync(int id)
    // ---------------------------------------------------------                 
    public async Task<Tbl90RefExpert?> GetRefExpertByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefExpertId == id);
    }
    // ---------------------------------------------------------                  
    // DeleteRefExpertAsync(int id)
    // ---------------------------------------------------------                 
    public async Task DeleteRefExpertAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl90RefExperts.FindAsync(id);
        if (entity != null)
        {
            context.Tbl90RefExperts.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    // ---------------------------------------------------------                  
    // ExistsAsync(Tbl90RefExpert entity)
    // ---------------------------------------------------------                 
    public async Task<bool> RefExpertExistsAsync(Tbl90RefExpert entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl90RefExperts.AnyAsync(x =>
            x.RefExpertName == entity.RefExpertName &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.RefExpertId != entity.RefExpertId
        );
    }



    //-----alt----------------------


    //public async Task<List<Tbl90RefExpert>> GetAllRefExpertsAsync()
    //{
    //    return await _context.Tbl90RefExperts
    //        .AsNoTracking()
    //        .OrderBy(a => a.RefExpertName)
    //        .ToListAsync();
    //}

    //public async Task<List<Tbl90RefExpert>> GetRefExpertsByNameOrIdAsync(string? query)
    //{
    //    if (string.IsNullOrWhiteSpace(query))
    //        return [];

    //    var isId = int.TryParse(query, out int idValue);

    //    var queryable = _context.Tbl90RefExperts.AsNoTracking();

    //    if (isId)
    //    {
    //        return await queryable
    //            .Where(f => f.RefExpertId == idValue)
    //            .OrderBy(a => a.RefExpertName)
    //            .ToListAsync();
    //    }
    //    else
    //    {
    //        return await queryable
    //            .Where(f => EF.Functions.Like(f.RefExpertName, $"{query}%"))
    //            .OrderBy(a => a.RefExpertName)
    //            .ToListAsync();
    //    }
    //}

    //public async Task<List<Tbl90RefExpert>> GetRefExpertsByNameAsync(string? query)
    //{
    //    if (string.IsNullOrWhiteSpace(query))
    //        return [];

    //    var queryable = _context.Tbl90RefExperts.AsNoTracking();
    //    {
    //        return await queryable
    //            .Where(f => EF.Functions.Like(f.RefExpertName, $"{query}%"))
    //            .OrderBy(a => a.RefExpertName)
    //            .ToListAsync();
    //    }
    //}

    //public async Task<bool> RefExpertExistsAsync(Tbl90RefExpert model)
    //{
    //    return await _context.Tbl90RefExperts.AnyAsync(c =>
    //        c.RefExpertId != model.RefExpertId &&
    //        c.RefExpertName == model.RefExpertName &&
    //        c.Valid == model.Valid &&
    //        c.ValidYear == model.ValidYear &&
    //        c.Info == model.Info &&
    //        c.Notes == model.Notes &&
    //        c.Memo == model.Memo
    //    );
    //}

    //public async Task<List<Tbl90RefExpert>> GetRefExpertAsync()
    //{
    //    return await _context.Tbl90RefExperts
    //        .AsNoTracking()
    //        .OrderBy(a => a.RefExpertName)
    //        .ToListAsync();
    //}

    //public async Task<List<Tbl90RefExpert>> GetRefExpertAsync(int refexpertid)
    //{
    //    return await _context.Tbl90RefExperts
    //        .AsNoTracking()
    //        .Where(e => e.RefExpertId == refexpertid)
    //        .OrderBy(k => k.RefExpertName)
    //        .ToListAsync();
    //}

    //public async Task<Tbl90RefExpert?> GetRefExpertByIdAsync(int? refexpertid)
    //{
    //    return await _context.Tbl90RefExperts
    //        .AsNoTracking()
    //        .FirstOrDefaultAsync(e => e.RefExpertId == refexpertid);
    //}

    //public async Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert refexpert)
    //{
    //    if (refexpert.RefExpertId == 0)
    //    {
    //        if (refexpert.Valid != null)
    //        {
    //        }
    //        else
    //        {
    //            refexpert.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
    //        }

    //        refexpert.CountId = RandomHelper.Randomnumber();
    //        refexpert.Writer = Environment.UserName;
    //        refexpert.WriterDate = DateTime.Now;
    //        refexpert.Updater = Environment.UserName;
    //        refexpert.UpdaterDate = DateTime.Now;
    //        await _context.Tbl90RefExperts.AddAsync(refexpert);
    //    }
    //    else
    //    {
    //        refexpert.Updater = Environment.UserName;
    //        refexpert.UpdaterDate = DateTime.Now;
    //        _context.Tbl90RefExperts.Update(refexpert);
    //    }

    //    await _context.SaveChangesAsync();
    //    return refexpert;
    //}

    //public async Task DeleteRefExpertAsync(int? refexpertid)
    //{
    //    if (refexpertid is null)
    //        return;

    //    var refexpert = await _context.Tbl90RefExperts
    //        .FirstOrDefaultAsync(c => c.RefExpertId == refexpertid);

    //    if (refexpert != null)
    //    {
    //        _context.Tbl90RefExperts.Remove(refexpert);
    //        await _context.SaveChangesAsync();
    //    }
    //}
    //-----------------------------------------------------
    public ObservableCollection<Tbl90RefExpert> GetRefExpertAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefExpert>(_context.Tbl90RefExperts
            .OrderBy(k => k.RefExpertName));
        return collection;
    }

    public ObservableCollection<Tbl90RefExpert> GetRefExpertAsyncAlt(int refexpertid)
    {
        var collection = new ObservableCollection<Tbl90RefExpert>(_context.Tbl90RefExperts
            .Where(e => e.RefExpertId == refexpertid)
            .OrderBy(k => k.RefExpertName)
        );
        return collection;
    }

    public async Task<ObservableCollection<Tbl90RefExpert>> GetRefExpertAsyncAlt(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl90RefExpert>(_context.Tbl90RefExperts
                .Where(e => e.RefExpertId == id))
            : new ObservableCollection<Tbl90RefExpert>(_context.Tbl90RefExperts
                .Where(e => e.RefExpertName.StartsWith(search))
                .OrderBy(a => a.RefExpertName)
            );
        await Task.CompletedTask;
        return collection;
    }

    public async Task<Tbl90RefExpert> UpsertRefExpertAsyncAlt(Tbl90RefExpert refexpert)
    {
        var current = await _context.Tbl90RefExperts.FirstOrDefaultAsync(_comment => _comment.RefExpertId == refexpert.RefExpertId);
        if (null == current)
        {
            if (refexpert.Valid != null)
            {
            }
            else
            {
                refexpert.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
            }

            refexpert.CountId = RandomHelper.Randomnumber();
            refexpert.Writer = Environment.UserName;
            refexpert.WriterDate = DateTime.Now;
            refexpert.Updater = Environment.UserName;
            refexpert.UpdaterDate = DateTime.Now;
            _context.Tbl90RefExperts.Add(refexpert);
        }
        else
        {
            _context.Entry(current).CurrentValues.SetValues(refexpert);
        }
        await _context.SaveChangesAsync();
        return refexpert;
    }

    public async Task DeleteRefExpertAsyncAlt(int refexpertid)
    {
        var reference = await _context.Tbl90RefExperts.FirstOrDefaultAsync(_reference => _reference.RefExpertId == refexpertid);
        if (null != reference)
        {
            _context.Tbl90RefExperts.Remove(reference);
            await _context.SaveChangesAsync();
        }
    }
    //--------------------------------------------------------
    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceAuthorsCollectionOrderByInfoFromGenusIdAndRefSourceIdIsNullAndRefExpertIdIsNull(int? genusId)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefSourceId == null && e.RefExpertId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceSourcesCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefExpertIdIsNull(int? genusId)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefAuthorId == null && e.RefExpertId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl90Reference> GetTbl90ReferenceExpertsCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefSourceIdIsNull(int? genusId)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Where(e => e.GenusId == genusId && e.RefAuthorId == null && e.RefSourceId == null)
                .OrderBy(k => k.Info));
            return collection;
        }
        return null!;
    }
    //-----------------------------------------------------------

    #endregion RefExpert

    #region Comment
    //--------------------------------------------------------
    public async Task<IEnumerable<Tbl93Comment>> GetAllCommentAsync()
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .AsNoTracking()
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> SearchCommentAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl93Comment>();

        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .AsNoTracking()
            .Where(e =>
                e.Info.StartsWith(search) ||
                e.CommentId.ToString() == search)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<Tbl93Comment> GetCommentByIdAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments

            .FirstOrDefaultAsync(x => x.CommentId == id);
    }
    public async Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment uiEntity)
    {
        await using var context = _factory.CreateDbContext();

        Tbl93Comment dbEntity;

        if (uiEntity.CommentId == 0)
        {
            dbEntity = new Tbl93Comment();
            context.Tbl93Comments.Add(dbEntity);
        }
        else
        {
            dbEntity = await context.Tbl93Comments
                           .FirstOrDefaultAsync(x => x.CommentId == uiEntity.CommentId)
                       ?? throw new InvalidOperationException("Entity not found.");
        }

        CopyProperties(uiEntity, dbEntity,
            nameof(Tbl93Comment.CommentId),
            nameof(Tbl93Comment.Writer),
            nameof(Tbl93Comment.WriterDate),
            nameof(Tbl93Comment.Updater),
            nameof(Tbl93Comment.UpdaterDate)
        );

        var isNew = dbEntity.CommentId == 0;
        ApplyAuditInfo(dbEntity, isNew);

        await context.SaveChangesAsync();

        return dbEntity;
    }
    public async Task DeleteCommentAsync(int id)
    {
        await using var context = _factory.CreateDbContext();

        var entity = await context.Tbl93Comments.FindAsync(id);
        if (entity != null)
        {
            context.Tbl93Comments.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
    public async Task<bool> CommentExistsAsync(Tbl93Comment entity)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments.AnyAsync(x =>
            x.SpeciesId == entity.SpeciesId &&
            x.GenusId == entity.GenusId &&
            x.InfratribusId == entity.InfratribusId &&
            x.SubtribusId == entity.SubtribusId &&
            x.TribusId == entity.TribusId &&
            x.SupertribusId == entity.SupertribusId &&
            x.InfrafamilyId == entity.InfrafamilyId &&
            x.SubfamilyId == entity.SubfamilyId &&
            x.FamilyId == entity.FamilyId &&
            x.SuperfamilyId == entity.SuperfamilyId &&
            x.InfraordoId == entity.InfraordoId &&
            x.SubordoId == entity.SubordoId &&
            x.OrdoId == entity.OrdoId &&
            x.LegioId == entity.LegioId &&
            x.InfraclassId == entity.InfraclassId &&
            x.SubclassId == entity.SubclassId &&
            x.ClassId == entity.ClassId &&
            x.SuperclassId == entity.SuperclassId &&
            x.SubdivisionId == entity.SubdivisionId &&
            x.SubphylumId == entity.SubphylumId &&
            x.DivisionId == entity.DivisionId &&
            x.PhylumId == entity.PhylumId &&
            x.RegnumId == entity.RegnumId &&
            x.Info == entity.Info &&
            x.Memo == entity.Memo &&
            x.Valid == entity.Valid &&
            x.ValidYear == entity.ValidYear &&

            x.CommentId != entity.CommentId
        );
    }

    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForRegnumAsync(int regnumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.RegnumId == regnumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForPhylumAsync(int phylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.PhylumId == phylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubphylumAsync(int subphylumId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubphylumId == subphylumId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperclassAsync(int superclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SuperclassId == superclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForClassAsync(int classId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.ClassId == classId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubclassAsync(int subclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubclassId == subclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraclassAsync(int infraclassId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfraclassId == infraclassId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForLegioAsync(int legioId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.LegioId == legioId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForOrdoAsync(int ordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.OrdoId == ordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubordoAsync(int subordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubordoId == subordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraordoAsync(int infraordoId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfraordoId == infraordoId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperfamilyAsync(int superfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SuperfamilyId == superfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForFamilyAsync(int familyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.FamilyId == familyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubfamilyAsync(int subfamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SubfamilyId == subfamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfrafamilyAsync(int infrafamilyId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.InfrafamilyId == infrafamilyId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSupertribusAsync(int supertribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(e => e.SupertribusId == supertribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForTribusAsync(int tribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.TribusId == tribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSubtribusAsync(int subtribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.SubtribusId == subtribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfratribusAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.InfratribusId == infratribusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForGenusAsync(int genusId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.GenusId == genusId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForSpeciesAsync(int speciesId)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Tbl93Comments
            .Where(c => c.SpeciesId == speciesId)
            .OrderBy(c => c.Info)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteCommentsAsync(int infratribusId)
    {
        await using var context = _factory.CreateDbContext();

        var comments = context.Tbl93Comments
            .Where(c => c.InfratribusId == infratribusId);

        context.Tbl93Comments.RemoveRange(comments);
        await context.SaveChangesAsync();
    }

    //-----------------------------------------------------
    //alt

    public ObservableCollection<Tbl93Comment> GetCommentAsync()
    {
        var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
            .AsNoTracking()
            .OrderBy(a => a.Info));
        return collection;
    }

    public ObservableCollection<Tbl93Comment> GetCommentAsync(int commentid)
    {
        var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
            .AsNoTracking()
            .Where(e => e.CommentId == commentid)
            .OrderBy(k => k.Info)
        );

        return collection;
    }

    public async Task<ObservableCollection<Tbl93Comment>> GetCommentAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.CommentId == id))
            : new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.Info.StartsWith(search))
                .OrderBy(a => a.Info));

        await Task.CompletedTask;
        return collection;
    }

    //public async Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment comment)
    //{
    //    var current = await _context.Tbl93Comments.FirstOrDefaultAsync(_comment => _comment.CommentId == comment.CommentId);
    //    if (null == current)
    //    {
    //        comment.CountId = RandomHelper.Randomnumber();
    //        comment.Writer = Environment.UserName;
    //        comment.WriterDate = DateTime.Now;
    //        comment.Updater = Environment.UserName;
    //        comment.UpdaterDate = DateTime.Now;
    //        _context.Tbl93Comments.Add(comment);
    //    }
    //    else
    //    {
    //        comment.Updater = Environment.UserName;
    //        comment.UpdaterDate = DateTime.Now;
    //        _context.Entry(current).CurrentValues.SetValues(comment);
    //    }
    //    await _context.SaveChangesAsync();
    //    return comment;
    //}

    //public async Task DeleteCommentAsync(int commentid)
    //{
    //    var comment = await _context.Tbl93Comments.FirstOrDefaultAsync(_comment => _comment.CommentId == commentid);
    //    if (null != comment)
    //    {
    //        _context.Tbl93Comments.Remove(comment);
    //        await _context.SaveChangesAsync();
    //    }
    //}


    #endregion Comment

    #region Country
    public async Task<List<TblCountry>> GetAllCountriesAsync()
    {
        return await _context.TblCountries
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<List<TblCountry>> GetCountriesByNameOrIdAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var isId = int.TryParse(query, out int idValue);

        var queryable = _context.TblCountries.AsNoTracking();

        if (isId)
        {
            return await queryable
                .Where(f => f.CountryId == idValue)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
        else
        {
            return await queryable
                .Where(f => EF.Functions.Like(f.Name, $"{query}%"))
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
    }
    public async Task<List<TblCountry>> GetCountriesByNameAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var queryable = _context.TblCountries.AsNoTracking();
        {
            return await queryable
                .Where(f => EF.Functions.Like(f.Name, $"{query}%"))
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
    }
    public async Task<bool> CountryExistsAsync(TblCountry model)
    {
        return await _context.TblCountries.AnyAsync(c =>
            c.CountryId != model.CountryId &&
            c.Name == model.Name &&
            c.Regex == model.Regex &&
            c.ZoomLevel == model.ZoomLevel &&
            c.Latitude == model.Latitude &&
            c.Longitude == model.Longitude
        );
    }

    public async Task<List<TblCountry>> GetCountryAsync()
    {
        return await _context.TblCountries
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .ToListAsync();
    }
    public async Task<List<TblCountry>> GetCountryAsync(int countryid)
    {
        return await _context.TblCountries
            .AsNoTracking()
            .Where(e => e.CountryId == countryid)
            .OrderBy(k => k.Name)
            .ToListAsync();
    }

    public async Task<TblCountry?> GetCountryByIdAsync(int? countryid)
    {
        return await _context.TblCountries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CountryId == countryid);
    }

    public async Task<TblCountry> UpsertCountryAsync(TblCountry country)
    {
        if (country.CountryId == 0)
        {
            await _context.TblCountries.AddAsync(country);
        }
        else
        {
            _context.TblCountries.Update(country);
        }

        await _context.SaveChangesAsync();
        return country;
    }

    public async Task DeleteCountryAsync(int? countryid)
    {
        if (countryid is null)
            return;

        var country = await _context.TblCountries
            .FirstOrDefaultAsync(c => c.CountryId == countryid);

        if (country != null)
        {
            _context.TblCountries.Remove(country);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Country

    #region UserProfile
    public ObservableCollection<TblUserProfile> GetUserProfileAsync()
    {
        var collection = new ObservableCollection<TblUserProfile>(_context.TblUserProfiles
            .AsNoTracking()
            .OrderBy(a => a.Email));
        return collection;
    }

    public ObservableCollection<TblUserProfile> GetUserProfileAsync(int userprofileid)
    {
        var collection = new ObservableCollection<TblUserProfile>(_context.TblUserProfiles
        .AsNoTracking()
            .Where(e => e.UserProfileId == userprofileid)
            .OrderBy(k => k.Email)
        );

        return collection;
    }

    public ObservableCollection<TblUserProfile> GetEmailFromUserProfileAsync(string email)
    {
        var collection = new ObservableCollection<TblUserProfile>(_context.TblUserProfiles
            .Where(e => e.Email == email)
            .OrderBy(k => k.Email)
        );
        return collection;

    }


    public async Task<ObservableCollection<TblUserProfile>> GetUserProfileAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblUserProfile>(_context.TblUserProfiles
                .Where(e => e.UserProfileId == id))
            : new ObservableCollection<TblUserProfile>(_context.TblUserProfiles
                .Where(e => e.Email.StartsWith(search))
                .OrderBy(a => a.Email));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile userprofile)
    {
        var current = await _context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofile.UserProfileId);
        if (null == current)
        {
            userprofile.CountId = RandomHelper.Randomnumber();
            //  userprofile.Role = "User";
            ////  userprofile.Password = Crypt.CalculateHash(Password, Email);
            //  userprofile.Password = "Test";
            userprofile.Writer = Environment.UserName;
            userprofile.WriterDate = DateTime.Now;
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            _context.TblUserProfiles.Add(userprofile);
        }
        else
        {
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(userprofile);
        }
        await _context.SaveChangesAsync();
        return userprofile;
    }

    public async Task<TblUserProfile> UpsertEmailFromUserProfileAsync(TblUserProfile userprofile)
    {
        var current = await _context.TblUserProfiles.FirstOrDefaultAsync(b => b.UserProfileId == userprofile.UserProfileId);
        if (null == current)
        {
            //  userprofile.CountId = RandomHelper.Randomnumber();
            //  userprofile.Role = "User";
            ////  userprofile.Password = Crypt.CalculateHash(Password, Email);
            //  userprofile.Password = Crypt.CalculateHash(clearTextPasswordNew, Email);

            userprofile.Writer = Environment.UserName;
            userprofile.WriterDate = DateTime.Now;
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            _context.TblUserProfiles.Add(userprofile);
        }
        else
        {
            userprofile.Updater = Environment.UserName;
            userprofile.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(userprofile);
        }
        await _context.SaveChangesAsync();
        return userprofile;
    }

    public async Task DeleteUserProfileAsync(int userprofileid)
    {
        var userprofile = await _context.TblUserProfiles.FirstOrDefaultAsync(c => c.UserProfileId == userprofileid);
        if (null != userprofile)
        {
            _context.TblUserProfiles.Remove(userprofile);
            await _context.SaveChangesAsync();
        }
    }


    #endregion UserProfile

    #region Food
    public ObservableCollection<TblFood> GetFoodAsync()
    {
        var collection = new ObservableCollection<TblFood>(_context.TblFoods
            .AsNoTracking()
            .OrderBy(a => a.FoodName));
        return collection;
    }

    public ObservableCollection<TblFood> GetFoodAsync(int foodid)
    {
        var collection = new ObservableCollection<TblFood>(_context.TblFoods
        .AsNoTracking()
            .Where(e => e.FoodId == foodid)
            .OrderBy(k => k.FoodName)
        );

        return collection;
    }

    public async Task<ObservableCollection<TblFood>> GetFoodAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblFood>(_context.TblFoods
                .Where(e => e.FoodId == id))
            : new ObservableCollection<TblFood>(_context.TblFoods
                .Where(e => e.FoodName.StartsWith(search))
                .OrderBy(a => a.FoodName));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblFood> UpsertFoodAsync(TblFood food)
    {
        var current = await _context.TblFoods.FirstOrDefaultAsync(c => c.FoodId == food.FoodId);
        if (null == current)
        {
            food.CountId = RandomHelper.Randomnumber();
            food.Writer = Environment.UserName;
            food.WriterDate = DateTime.Now;
            food.Updater = Environment.UserName;
            food.UpdaterDate = DateTime.Now;

            _context.TblFoods.Add(food);
        }
        else
        {
            food.Updater = Environment.UserName;
            food.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(food);
        }
        await _context.SaveChangesAsync();
        return food;
    }

    public async Task DeleteFoodAsync(int foodid)
    {
        var food = await _context.TblFoods.FirstOrDefaultAsync(c => c.FoodId == foodid);
        if (null != food)
        {
            _context.TblFoods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Food

    #region Disease
    public ObservableCollection<TblDisease> GetDiseaseAsync()
    {
        var collection = new ObservableCollection<TblDisease>(_context.TblDiseases
            .AsNoTracking()
            .OrderBy(a => a.DiseaseName));
        return collection;
    }

    public ObservableCollection<TblDisease> GetDiseaseAsync(int diseaseid)
    {
        var collection = new ObservableCollection<TblDisease>(_context.TblDiseases
        .AsNoTracking()
            .Where(e => e.DiseaseId == diseaseid)
            .OrderBy(k => k.DiseaseName)
        );

        return collection;
    }

    public async Task<ObservableCollection<TblDisease>> GetDiseaseAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblDisease>(_context.TblDiseases
                .Where(e => e.DiseaseId == id))
            : new ObservableCollection<TblDisease>(_context.TblDiseases
                .Where(e => e.DiseaseName.StartsWith(search))
                .OrderBy(a => a.DiseaseName));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblDisease> UpsertDiseaseAsync(TblDisease disease)
    {
        var current = await _context.TblDiseases.FirstOrDefaultAsync(c => c.DiseaseId == disease.DiseaseId);
        if (null == current)
        {
            disease.CountId = RandomHelper.Randomnumber();
            disease.Writer = Environment.UserName;
            disease.WriterDate = DateTime.Now;
            disease.Updater = Environment.UserName;
            disease.UpdaterDate = DateTime.Now;

            _context.TblDiseases.Add(disease);
        }
        else
        {
            disease.Updater = Environment.UserName;
            disease.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(disease);
        }
        await _context.SaveChangesAsync();
        return disease;
    }

    public async Task DeleteDiseaseAsync(int diseaseid)
    {
        var disease = await _context.TblDiseases.FirstOrDefaultAsync(c => c.DiseaseId == diseaseid);
        if (null != disease)
        {
            _context.TblDiseases.Remove(disease);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Disease

    #region Miscellaneous
    public ObservableCollection<TblMiscellaneous> GetMiscellaneousAsync()
    {
        var collection = new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
            .AsNoTracking()
            .OrderBy(a => a.MiscellaneousName));
        return collection;
    }

    public ObservableCollection<TblMiscellaneous> GetMiscellaneousAsync(int miscellaneousid)
    {
        var collection = new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
        .AsNoTracking()
            .Where(e => e.MiscellaneousId == miscellaneousid)
            .OrderBy(k => k.MiscellaneousName)
        );

        return collection;
    }

    public async Task<ObservableCollection<TblMiscellaneous>> GetMiscellaneousAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
                .Where(e => e.MiscellaneousId == id))
            : new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
                .Where(e => e.MiscellaneousName.StartsWith(search))
                .OrderBy(a => a.MiscellaneousName));

        await Task.CompletedTask;
        return collection;
    }

    public async Task<TblMiscellaneous> UpsertMiscellaneousAsync(TblMiscellaneous miscellaneous)
    {
        var current = await _context.TblMiscellaneousses.FirstOrDefaultAsync(c => c.MiscellaneousId == miscellaneous.MiscellaneousId);
        if (null == current)
        {
            miscellaneous.CountId = RandomHelper.Randomnumber();
            miscellaneous.Writer = Environment.UserName;
            miscellaneous.WriterDate = DateTime.Now;
            miscellaneous.Updater = Environment.UserName;
            miscellaneous.UpdaterDate = DateTime.Now;

            _context.TblMiscellaneousses.Add(miscellaneous);
        }
        else
        {
            miscellaneous.Updater = Environment.UserName;
            miscellaneous.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(miscellaneous);
        }
        await _context.SaveChangesAsync();
        return miscellaneous;
    }

    public async Task DeleteMiscellaneousAsync(int miscellaneousid)
    {
        var miscellaneous = await _context.TblMiscellaneousses.FirstOrDefaultAsync(c => c.MiscellaneousId == miscellaneousid);
        if (null != miscellaneous)
        {
            _context.TblMiscellaneousses.Remove(miscellaneous);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Miscellaneous

    #endregion CRUD

    #region Search

    public async Task<IEnumerable<SearchResultItem>> SearchAcrossTablesAsync(string? search, string? entityType = "All")
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<SearchResultItem>();

        search = search.Trim();
        //if (search.Length < 1)   // <- statt < 2
        //    return Enumerable.Empty<SearchResultItem>();

        var type = (entityType ?? "All").Trim();
        var s = search.Trim(); // kein ToLower

        await using var context = _factory.CreateDbContext();

        var results = new List<SearchResultItem>();

        if (type is "All" or "Regnum")
            results.AddRange(await SearchRegnumAsync(context, s, take: 30));

        if (type is "All" or "Phylum")
            results.AddRange(await SearchPhylumAsync(context, s, take: 30));

        if (type is "All" or "Subphylum")
            results.AddRange(await SearchSubphylumAsync(context, s, take: 30));

        if (type is "All" or "Class")
            results.AddRange(await SearchClassAsync(context, s, take: 30));

        return results
            .OrderBy(x => x.Title)
            .Take(200)
            .ToList();
    }
    private static async Task<List<SearchResultItem>> SearchRegnumAsync(AtisDbContext context, string s, int take)
    {
        var q = context.Tbl03Regnums.AsNoTracking();

        if (int.TryParse(s, out var id))
        {
            return await q
                .Where(e =>
                    e.RegnumId == id ||
                    (e.RegnumName != null && e.RegnumName.StartsWith(s)))
                .OrderBy(e => e.RegnumName)
                .Take(take)
                .Select(e => new SearchResultItem
                {
                    EntityType = "Regnum",
                    EntityId = e.RegnumId,
                    Title = e.RegnumName ?? string.Empty,
                    Subtitle = e.Subregnum ?? string.Empty
                })
                .ToListAsync();
        }

        return await q
            .Where(e => e.RegnumName != null && e.RegnumName.StartsWith(s))
            .OrderBy(e => e.RegnumName)
            .Take(take)
            .Select(e => new SearchResultItem
            {
                EntityType = "Regnum",
                EntityId = e.RegnumId,
                Title = e.RegnumName ?? string.Empty,
                Subtitle = e.Subregnum ?? string.Empty
            })
            .ToListAsync();
    }

    private static async Task<List<SearchResultItem>> SearchPhylumAsync(AtisDbContext context, string s, int take)
    {
        var q = context.Tbl06Phylums
            .Include(x => x.Regnum)
            .AsNoTracking();

        if (int.TryParse(s, out var id))
        {
            return await q
                .Where(e =>
                    e.PhylumId == id ||
                    (e.PhylumName != null && e.PhylumName.StartsWith(s)))
                .OrderBy(e => e.PhylumName)
                .Take(take)
                .Select(e => new SearchResultItem
                {
                    EntityType = "Phylum",
                    EntityId = e.PhylumId,
                    Title = e.PhylumName ?? string.Empty,
                    Subtitle = e.Regnum != null ? e.Regnum.RegnumName ?? string.Empty : string.Empty
                })
                .ToListAsync();
        }

        return await q
            .Where(e => e.PhylumName != null && e.PhylumName.StartsWith(s))
            .OrderBy(e => e.PhylumName)
            .Take(take)
            .Select(e => new SearchResultItem
            {
                EntityType = "Phylum",
                EntityId = e.PhylumId,
                Title = e.PhylumName ?? string.Empty,
                Subtitle = e.Regnum != null ? e.Regnum.RegnumName ?? string.Empty : string.Empty
            })
            .ToListAsync();
    }

    private static async Task<List<SearchResultItem>> SearchSubphylumAsync(AtisDbContext context, string s, int take)
    {
        var q = context.Tbl12Subphylums
            .Include(x => x.Phylum)
            .AsNoTracking();

        if (int.TryParse(s, out var id))
        {
            return await q
                .Where(e =>
                    e.SubphylumId == id ||
                    (e.SubphylumName != null && e.SubphylumName.StartsWith(s)))
                .OrderBy(e => e.SubphylumName)
                .Take(take)
                .Select(e => new SearchResultItem
                {
                    EntityType = "Subphylum",
                    EntityId = e.SubphylumId,
                    Title = e.SubphylumName ?? string.Empty,
                    Subtitle = e.Phylum != null ? e.Phylum.PhylumName ?? string.Empty : string.Empty
                })
                .ToListAsync();
        }

        return await q
            .Where(e => e.SubphylumName != null && e.SubphylumName.StartsWith(s))
            .OrderBy(e => e.SubphylumName)
            .Take(take)
            .Select(e => new SearchResultItem
            {
                EntityType = "Subphylum",
                EntityId = e.SubphylumId,
                Title = e.SubphylumName ?? string.Empty,
                Subtitle = e.Author ?? string.Empty
            })
            .ToListAsync();
    }

    private static async Task<List<SearchResultItem>> SearchClassAsync(AtisDbContext context, string s, int take)
    {
        var q = context.Tbl21Classes.AsNoTracking();

        if (int.TryParse(s, out var id))
        {
            return await q
                .Where(e =>
                    e.ClassId == id ||
                    (e.ClassName != null && e.ClassName.StartsWith(s)))
                .OrderBy(e => e.ClassName)
                .Take(take)
                .Select(e => new SearchResultItem
                {
                    EntityType = "Class",
                    EntityId = e.ClassId,
                    Title = e.ClassName ?? string.Empty,
                    Subtitle = e.Author ?? string.Empty
                })
                .ToListAsync();
        }

        return await q
            .Where(e => e.ClassName != null && e.ClassName.StartsWith(s))
            .OrderBy(e => e.ClassName)
            .Take(take)
            .Select(e => new SearchResultItem
            {
                EntityType = "Class",
                EntityId = e.ClassId,
                Title = e.ClassName ?? string.Empty,
                Subtitle = e.Author ?? string.Empty
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<object>> SearchAsync(string tableName, string searchText)
    {
        await Task.Delay(50);

        return new List<object>
        {
            $"{tableName} – Ergebnis 1 für '{searchText}'",
            $"{tableName} – Ergebnis 2 für '{searchText}'",
            $"{tableName} – Ergebnis 3 für '{searchText}'"
        };
    }

    public async Task<object?> GetByIdAsync(string tableName, int id)
    {
        await Task.Delay(50);
        return $"{tableName} – Detail für ID {id}";
    }
    #endregion

    #region Search_Gets

    public ObservableCollection<Tbl03Regnum> GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(string filterText)
    {
        if (_context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
                    .Where(e => e.RegnumName.StartsWith(filterText) &&
                                e.RegnumName.Contains(Value) == false ||
                                e.Subregnum.Contains(filterText) ||
                                e.EngName.Contains(filterText) ||
                                e.GerName.Contains(filterText) ||
                                e.FraName.Contains(filterText) ||
                                e.PorName.Contains(filterText)
                    )
                    .OrderBy(e => e.RegnumName)
                    .ThenBy(e => e.Subregnum)
                );
            return collection;

        }
        return null!;
    }

    public ObservableCollection<Tbl06Phylum> GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(string filterText)
    {
        if (_context.Tbl06Phylums != null)
        {

            var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
                .Where(e => e.PhylumName.StartsWith(filterText) &&
                            e.PhylumName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.PhylumName)
            );
            return collection;

        }
        return null!;
    }

    public ObservableCollection<Tbl09Division> GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(string filterText)
    {
        if (_context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(_context.Tbl09Divisions
                .Where(e => e.DivisionName.StartsWith(filterText) &&
                            e.DivisionName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.DivisionName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl12Subphylum> GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(string filterText)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => e.SubphylumName.StartsWith(filterText) &&
                            e.SubphylumName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubphylumName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl15Subdivision> GetTbl15SubdivisionsCollectionOrderBySubdivisionNameFromFilterText(string filterText)
    {
        if (_context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(_context.Tbl15Subdivisions
                .Where(e => e.SubdivisionName.StartsWith(filterText) &&
                            e.SubdivisionName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubdivisionName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl18Superclass> GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(string filterText)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => e.SuperclassName.StartsWith(filterText) &&
                            e.SuperclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SuperclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl21Class> GetTbl21ClassesCollectionOrderByClassNameFromFilterText(string filterText)
    {
        if (_context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => e.ClassName.StartsWith(filterText) &&
                            e.ClassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.ClassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl24Subclass> GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(string filterText)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => e.SubclassName.StartsWith(filterText) &&
                            e.SubclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl27Infraclass> GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(string filterText)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => e.InfraclassName.StartsWith(filterText) &&
                            e.InfraclassName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfraclassName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl30Legio> GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(string filterText)
    {
        if (_context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => e.LegioName.StartsWith(filterText) &&
                            e.LegioName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.LegioName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl33Ordo> GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(string filterText)
    {
        if (_context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => e.OrdoName.StartsWith(filterText) &&
                            e.OrdoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.OrdoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl36Subordo> GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(string filterText)
    {
        if (_context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => e.SubordoName.StartsWith(filterText) &&
                            e.SubordoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubordoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl39Infraordo> GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(string filterText)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => e.InfraordoName.StartsWith(filterText) &&
                            e.InfraordoName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfraordoName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl42Superfamily> GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(string filterText)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyName.StartsWith(filterText) &&
                            e.SuperfamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SuperfamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl45Family> GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(string filterText)
    {
        if (_context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => e.FamilyName.StartsWith(filterText) &&
                            e.FamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.FamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl48Subfamily> GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(string filterText)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => e.SubfamilyName.StartsWith(filterText) &&
                            e.SubfamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubfamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl51Infrafamily> GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(string filterText)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyName.StartsWith(filterText) &&
                            e.InfrafamilyName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfrafamilyName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl54Supertribus> GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(string filterText)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => e.SupertribusName.StartsWith(filterText) &&
                            e.SupertribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SupertribusName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl57Tribus> GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(string filterText)
    {
        if (_context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => e.TribusName.StartsWith(filterText) &&
                            e.TribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.TribusName));

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl60Subtribus> GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(string filterText)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => e.SubtribusName.StartsWith(filterText) &&
                            e.SubtribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.SubtribusName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl63Infratribus> GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(string filterText)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
                .Where(e => e.InfratribusName.StartsWith(filterText) &&
                            e.InfratribusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.InfratribusName));
            return collection;
        }
        return null!;
    }


    public ObservableCollection<Tbl66Genus> GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(string filterText)
    {
        if (_context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => e.GenusName.StartsWith(filterText) &&
                            e.GenusName.Contains(Value) == false ||
                            e.EngName.Contains(filterText) ||
                            e.GerName.Contains(filterText) ||
                            e.FraName.Contains(filterText) ||
                            e.PorName.Contains(filterText)
                )
                .OrderBy(a => a.GenusName));

            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl75Species>
        GetTbl75SpeciessesCollectionOrderByGenusNameAndSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Include(a => a.Genus)
                .Where(e => e.SpeciesName.StartsWith(filterText) &&
                            e.SpeciesName.Contains(Value) == false ||
                            e.Genus.GenusName.Contains(filterText) ||
                            e.Subspecies.Contains(filterText) ||
                            e.Divers.Contains(filterText) ||
                            e.TradeName.Contains(filterText) ||
                            e.Author.Contains(filterText)
                )
                .OrderBy(a => a.Genus.GenusName)        
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );

            return collection;
        }
        return null!;
    }

    //public ObservableCollection<Tbl69FiSpecies>
    //    GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText)
    //{
    //    if (_context.Tbl69FiSpeciesses != null)
    //    {
    //        var collection = new ObservableCollection<Tbl69FiSpecies>(_context.Tbl69FiSpeciesses
    //            .Include(a => a.Tbl66Genusses)
    //            .Where(e => e.FiSpeciesName.StartsWith(filterText) &&
    //                        e.FiSpeciesName.Contains(Value) == false ||
    //                        e.Tbl66Genusses.GenusName.Contains(filterText) ||
    //                        e.Subspecies.Contains(filterText) ||
    //                        e.Divers.Contains(filterText) ||
    //                        e.TradeName.Contains(filterText) ||
    //                        e.Author.Contains(filterText)
    //            )
    //            .OrderBy(a => a.Tbl66Genusses.GenusName)
    //            .ThenBy(a => a.FiSpeciesName)
    //            .ThenBy(a => a.Subspecies)
    //            .ThenBy(a => a.Divers)
    //        );

    //        return collection;
    //    }
    //    return null!;
    //}

    //public ObservableCollection<Tbl72PlSpecies> GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(
    //    string filterText)
    //{
    //    if (_context.Tbl72PlSpeciesses != null)
    //    {
    //        var collection = new ObservableCollection<Tbl72PlSpecies>(_context.Tbl72PlSpeciesses
    //            .Include(a => a.Tbl66Genusses)
    //            .Where(e => e.PlSpeciesName.StartsWith(filterText) &&
    //                        e.PlSpeciesName.Contains(Value) == false ||
    //                        e.Tbl66Genusses.GenusName.Contains(filterText) ||
    //                        e.Subspecies.Contains(filterText) ||
    //                        e.Divers.Contains(filterText) ||
    //                        e.TradeName.Contains(filterText) ||
    //                        e.Author.Contains(filterText)
    //            )
    //            .OrderBy(a => a.Tbl66Genusses.GenusName)
    //            .ThenBy(a => a.PlSpeciesName)
    //            .ThenBy(a => a.Subspecies)
    //            .ThenBy(a => a.Divers)
    //        );

    //        return collection;
    //    }
    //    return null!;
    //}

    public ObservableCollection<Tbl78Name> GetTbl78NamesCollectionOrderByNameNameFromFilterText(string filterText)
    {
        if (_context.Tbl78Names != null)
        {
            var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
                .Include(a => a.Species)
                .Where(e => e.NameName.StartsWith(filterText) &&
                            e.Species.SpeciesId == e.SpeciesId
                )
                .OrderBy(a => a.NameName));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl84Synonym> GetTbl84SynonymsCollectionOrderBySynonymNameFromFilterText(string filterText)
    {
        if (_context.Tbl84Synonyms != null)
        {
            var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
                .Include(a => a.Species)
                .Where(e => e.SynonymName.StartsWith(filterText) &&
                            e.Species.SpeciesId == e.SpeciesId
                )
                .OrderBy(a => a.SynonymName));
            return collection;
        }
        return null!;
    }



    
    public ObservableCollection<TblFood> GetTblFoodsCollectionOrderByFoodNameFromFilterText(string filterText)
    {
        if (_context.TblFoods != null)
        {
            var collection = new ObservableCollection<TblFood>(_context.TblFoods
                .Where(e => e.FoodName.StartsWith(filterText))
                .OrderBy(e => e.FoodName)
            );
            return collection;

        }
        return null!;
    }

    public async Task<List<TblFood>> GetFoodsByNameOrIdAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        // Prüfen, ob Query eine ID ist
        var isId = int.TryParse(query, out var idValue);

        using var context = new AtisDbContext();
        var queryable = _context.TblFoods.AsQueryable();

        if (isId)
        {
            // Nur ID-Suche
            return await queryable
                .Where(f => f.FoodId == idValue)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            // Nur Namenssuche (beginnt mit)
            return await queryable
                .Where(f => EF.Functions.Like(f.FoodName, $"{query}%"))
                .Distinct()
                .ToListAsync();
        }
    }

    public ObservableCollection<TblDisease> GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(string filterText)
    {
        if (_context.TblDiseases != null)
        {
            var collection = new ObservableCollection<TblDisease>(_context.TblDiseases
                .Where(e => e.DiseaseName.StartsWith(filterText))
                .OrderBy(e => e.DiseaseName)
            );
            return collection;

        }
        return null!;
    }
    public async Task<List<TblDisease>> GetDiseasesByNameOrIdAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        // Prüfen, ob Query eine ID ist
        var isId = int.TryParse(query, out int idValue);

        using var context = new AtisDbContext();
        var queryable = _context.TblDiseases.AsQueryable();

        if (isId)
        {
            // Nur ID-Suche
            return await queryable
                .Where(f => f.DiseaseId == idValue)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            // Nur Namenssuche (beginnt mit)
            return await queryable
                .Where(f => EF.Functions.Like(f.DiseaseName, $"{query}%"))
                .Distinct()
                .ToListAsync();
        }
    }

    public ObservableCollection<TblMiscellaneous> GetTblMiscellaneoussesCollectionOrderByMiscellaneousNameFromFilterText(string filterText)
    {
        if (_context.TblMiscellaneousses != null)
        {
            var collection = new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
                .Where(e => e.MiscellaneousName.StartsWith(filterText))
                .OrderBy(e => e.MiscellaneousName)
            );
            return collection;

        }
        return null!;
    }

    public async Task<List<TblMiscellaneous>> GetMiscellaneoussesByNameOrIdAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        // Prüfen, ob Query eine ID ist
        var isId = int.TryParse(query, out int idValue);

        using var context = new AtisDbContext();
        var queryable = _context.TblMiscellaneousses.AsQueryable();

        if (isId)
        {
            // Nur ID-Suche
            return await queryable
                .Where(f => f.MiscellaneousId == idValue)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            // Nur Namenssuche (beginnt mit)
            return await queryable
                .Where(f => EF.Functions.Like(f.MiscellaneousName, $"{query}%"))
                .Distinct()
                .ToListAsync();
        }
    }

    #endregion Search_Gets

    #region Report_Gets

    #region Regnum
    public Tbl03Regnum GetRegnumSingleByRegnumIdAndHash(int id)
    {
        if (_context.Tbl03Regnums != null)
        {
            var single = _context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id &&
                                                                   a.RegnumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl03Regnum GetRegnumSingleByRegnumId(int id)
    {
        if (_context.Tbl03Regnums != null)
        {
            var single = _context.Tbl03Regnums.SingleOrDefault(a => a.RegnumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumId(int id)
    {
        if (_context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
                .Where(e => e.RegnumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl06Phylum> CollPhylumsByRegnumIdAndHash(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
            .Where(e => e.RegnumId == id &&
                        e.PhylumName.Contains(Value) == false)
            .OrderBy(a => a.PhylumName));
            return collection;
        }
        return null!;
        ;
    }
    public ObservableCollection<Tbl09Division> CollDivisionsByRegnumIdAndHash(int id)
    {
        if (_context.Tbl09Divisions != null)
        {
            var collection = new ObservableCollection<Tbl09Division>(_context.Tbl09Divisions
                .Where(e => e.RegnumId == id &&
                            e.DivisionName.Contains(Value) == false)
                .OrderBy(a => a.DivisionName));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByRegnumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.RegnumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByRegnumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.RegnumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByRegnumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.RegnumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByRegnumId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.RegnumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Regnum

    #region Phylum
    public Tbl06Phylum GetPhylumSingleByPhylumIdAndHash(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var single = _context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id &&
                                                                  a.PhylumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl06Phylum GetPhylumSingleByPhylumId(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var single = _context.Tbl06Phylums.SingleOrDefault(a => a.PhylumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumId(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
                .Where(e => e.PhylumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl12Subphylum> CollSubphylumsByPhylumIdAndHash(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => e.PhylumId == id &&
                            e.SubphylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int RegnumIdFromPhylumsCollectionSelect(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var dbset = _context.Tbl06Phylums.SingleOrDefault(p => p.PhylumId == id);

            if (dbset != null)
            {
                return (int)dbset.RegnumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumIdAndHash(int id)
    {
        if (_context.Tbl03Regnums != null)
        {
            var collection = new ObservableCollection<Tbl03Regnum>(_context.Tbl03Regnums
                .Where(e => e.RegnumId == id &&
                            e.RegnumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByPhylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.PhylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByPhylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.PhylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByPhylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.PhylumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByPhylumId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.PhylumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Phylum

    #region Subphylum
    public Tbl12Subphylum GetSubphylumSingleBySubphylumIdAndHash(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var single = _context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id &&
                                                                     a.SubphylumName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public Tbl12Subphylum GetSubphylumSingleBySubphylumId(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var single = _context.Tbl12Subphylums.SingleOrDefault(a => a.SubphylumId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumId(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySubphylumIdAndHash(int id)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => e.SubphylumId == id &&
                            e.SuperclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int PhylumIdFromSubphylumsCollectionSelect(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var dbset = _context.Tbl12Subphylums
                .SingleOrDefault(p => p.SubphylumId == id);

            if (dbset != null)
            {
                return dbset.PhylumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumIdAndHash(int id)
    {
        if (_context.Tbl06Phylums != null)
        {
            var collection = new ObservableCollection<Tbl06Phylum>(_context.Tbl06Phylums
                .Where(e => e.PhylumId == id &&
                            e.PhylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsBySubphylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubphylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubphylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubphylumId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubphylumId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubphylumId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubphylumId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SubphylumId == id));
            return collection;
        }
        return null!;
    }
    #endregion Subphylum

    #region Superclass
    public Tbl18Superclass GetSuperclassSingleBySuperclassIdAndHash(int id)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var single = _context.Tbl18Superclasses.SingleOrDefault(a => a.SuperclassId == id &&
                                                                      a.SuperclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }

        return null!;
    }

    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassId(int id)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl21Class> CollClassesBySuperclassIdAndHash(int id)
    {
        if (_context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => e.SuperclassId == id &&
                            e.ClassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    //public int SubdivisionIdFromSuperclassesCollectionSelect(int id)
    //{
    //    if (_context.Tbl18Superclasses != null)
    //    {
    //        var dbset = _context.Tbl18Superclasses
    //            .SingleOrDefault(p => p.SuperclassId == id);

    //        if (dbset != null)
    //        {
    //            return dbset.SubdivisionId;
    //        }
    //    }

    //    return 0;
    //}
    public int SubphylumIdFromSuperclassesCollectionSelect(int id)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var dbset = _context.Tbl18Superclasses
                .SingleOrDefault(p => p.SuperclassId == id);

            if (dbset != null)
            {
                return dbset.SubphylumId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumIdAndHash(int id)
    {
        if (_context.Tbl12Subphylums != null)
        {
            var collection = new ObservableCollection<Tbl12Subphylum>(_context.Tbl12Subphylums
                .Where(e => e.SubphylumId == id &&
                            e.SubphylumName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl15Subdivision> CollSubdivisionsBySubdivisionIdAndHash(int id)
    {
        if (_context.Tbl15Subdivisions != null)
        {
            var collection = new ObservableCollection<Tbl15Subdivision>(_context.Tbl15Subdivisions
                .Where(e => e.SubdivisionId == id &&
                            e.SubdivisionName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsBySuperclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SuperclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySuperclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SuperclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySuperclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SuperclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySuperclassId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SuperclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion Superclass

    #region Class
    public Tbl21Class GetClassSingleByClassIdAndHash(int id)
    {
        if (_context.Tbl21Classes != null)
        {
            var single = _context.Tbl21Classes.SingleOrDefault(a => a.ClassId == id &&
                                                                 a.ClassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }

        return null!;
    }

    public ObservableCollection<Tbl21Class> CollClassesByClassId(int id)
    {
        if (_context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => e.ClassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl24Subclass> CollSubclassesByClassIdAndHash(int id)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => e.ClassId == id &&
                            e.SubclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SuperclassIdFromClassesCollectionSelect(int id)
    {
        if (_context.Tbl21Classes != null)
        {
            var dbset = _context.Tbl21Classes
                .SingleOrDefault(p => p.ClassId == id);

            if (dbset != null)
            {
                return dbset.SuperclassId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassIdAndHash(int id)
    {
        if (_context.Tbl18Superclasses != null)
        {
            var collection = new ObservableCollection<Tbl18Superclass>(_context.Tbl18Superclasses
                .Where(e => e.SuperclassId == id &&
                            e.SuperclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByClassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.ClassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByClassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.ClassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByClassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.ClassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByClassId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.ClassId == id));
            return collection;
        }
        return null!;
    }
    #endregion Class

    #region Subclass
    public Tbl24Subclass GetSubclassSingleBySubclassIdAndHash(int id)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var single = _context.Tbl24Subclasses.SingleOrDefault(a => a.SubclassId == id &&
                                                                      a.SubclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassId(int id)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => e.SubclassId == id));
            //var collection = new ObservableCollection<Tbl24Subclass>(_uow.Tbl24Subclasses
            //    .Find(e => e.SubclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl27Infraclass> CollInfraclassesBySubclassIdAndHash(int id)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => e.SubclassId == id &&
                            e.InfraclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.SubclassId == id &&
            //               e.InfraclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int ClassIdFromSubclassesCollectionSelect(int id)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var dbset = _context.Tbl24Subclasses
                .SingleOrDefault(p => p.SubclassId == id);

            if (dbset != null)
            {
                return dbset.ClassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl21Class> CollClassesByClassIdAndHash(int id)
    {
        if (_context.Tbl21Classes != null)
        {
            var collection = new ObservableCollection<Tbl21Class>(_context.Tbl21Classes
                .Where(e => e.ClassId == id &&
                            e.ClassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl21Class>(_uow.Tbl21Classes
            //    .Find(e => e.ClassId == id &&
            //               e.ClassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    // References
    public ObservableCollection<Tbl90Reference> CollExpertsBySubclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubclassId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SubclassId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infraclass
    public Tbl27Infraclass GetInfraclassSingleByInfraclassIdAndHash(int id)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var single = _context.Tbl27Infraclasses.SingleOrDefault(a => a.InfraclassId == id &&
                                                                      a.InfraclassName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassId(int id)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.InfraclassId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl30Legio> CollLegiosByInfraclassIdAndHash(int id)
    {
        if (_context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => e.InfraclassId == id &&
                            e.LegioName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.InfraclassId == id &&
            //               e.LegioName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubclassIdFromInfraclassesCollectionSelect(int id)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var dbset = _context.Tbl27Infraclasses
                .SingleOrDefault(p => p.InfraclassId == id);

            if (dbset != null)
            {
                return dbset.SubclassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassIdAndHash(int id)
    {
        if (_context.Tbl24Subclasses != null)
        {
            var collection = new ObservableCollection<Tbl24Subclass>(_context.Tbl24Subclasses
                .Where(e => e.SubclassId == id &&
                            e.SubclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl24Subclass>(_uow.Tbl24Subclasses
            //    .Find(e => e.SubclassId == id &&
            //               e.SubclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollExpertsByInfraclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfraclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfraclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfraclassId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfraclassId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfraclassId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfraclassId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.InfraclassId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfraclassId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Legio
    public Tbl30Legio GetLegioSingleByLegioIdAndHash(int id)
    {
        if (_context.Tbl30Legios != null)
        {
            var single = _context.Tbl30Legios.SingleOrDefault(a => a.LegioId == id &&
                                                                 a.LegioName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl30Legio> CollLegiosByLegioId(int id)
    {
        if (_context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => e.LegioId == id));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.LegioId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl33Ordo> CollOrdosByLegioIdAndHash(int id)
    {
        if (_context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => e.LegioId == id &&
                            e.OrdoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.LegioId == id &&
            //               e.OrdoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfraclassIdFromLegiosCollectionSelect(int id)
    {
        if (_context.Tbl30Legios != null)
        {
            var dbset = _context.Tbl30Legios
                .SingleOrDefault(p => p.LegioId == id);

            if (dbset != null)
            {
                return dbset.InfraclassId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassIdAndHash(int id)
    {
        if (_context.Tbl27Infraclasses != null)
        {
            var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id &&
                            e.InfraclassName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl27Infraclass>(_uow.Tbl27Infraclasses
            //    .Find(e => e.InfraclassId == id &&
            //               e.InfraclassName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollExpertsByLegioId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.LegioId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByLegioId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.LegioId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByLegioId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.LegioId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByLegioId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.LegioId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.LegioId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Ordo
    public Tbl33Ordo GetOrdoSingleByOrdoIdAndHash(int id)
    {
        if (_context.Tbl33Ordos != null)
        {
            var single = _context.Tbl33Ordos.SingleOrDefault(a => a.OrdoId == id &&
                                                                a.OrdoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl33Ordo> CollOrdosByOrdoId(int id)
    {
        if (_context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => e.OrdoId == id));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.OrdoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl36Subordo> CollSubordosByOrdoIdAndHash(int id)
    {
        if (_context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => e.OrdoId == id &&
                            e.SubordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.OrdoId == id &&
            //               e.SubordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int LegioIdFromOrdosCollectionSelect(int id)
    {
        if (_context.Tbl33Ordos != null)
        {
            var dbset = _context.Tbl33Ordos
                .SingleOrDefault(p => p.OrdoId == id);

            if (dbset != null)
            {
                return dbset.LegioId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl30Legio> CollLegiosByLegioIdAndHash(int id)
    {
        if (_context.Tbl30Legios != null)
        {
            var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => e.LegioId == id &&
                            e.LegioName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl30Legio>(_uow.Tbl30Legios
            //    .Find(e => e.LegioId == id &&
            //               e.LegioName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByOrdoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.OrdoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByOrdoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.OrdoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByOrdoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.OrdoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByOrdoId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.OrdoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.OrdoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subordo
    public Tbl36Subordo GetSubordoSingleBySubordoIdAndHash(int id)
    {
        if (_context.Tbl36Subordos != null)
        {
            var single = _context.Tbl36Subordos.SingleOrDefault(a => a.SubordoId == id &&
                                                                   a.SubordoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl36Subordo> CollSubordosBySubordoId(int id)
    {
        if (_context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => e.SubordoId == id));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.SubordoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl39Infraordo> CollInfraordosBySubordoIdAndHash(int id)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => e.SubordoId == id &&
                            e.InfraordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.SubordoId == id &&
            //               e.InfraordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int OrdoIdFromSubordosCollectionSelect(int id)
    {
        if (_context.Tbl36Subordos != null)
        {
            var dbset = _context.Tbl36Subordos
                .SingleOrDefault(p => p.SubordoId == id);

            if (dbset != null)
            {
                return dbset.OrdoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl33Ordo> CollOrdosByOrdoIdAndHash(int id)
    {
        if (_context.Tbl33Ordos != null)
        {
            var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => e.OrdoId == id &&
                            e.OrdoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl33Ordo>(_uow.Tbl33Ordos
            //    .Find(e => e.OrdoId == id &&
            //               e.OrdoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubordoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubordoId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SubordoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubordoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infraordo
    public Tbl39Infraordo GetInfraordoSingleByInfraordoIdAndHash(int id)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var single = _context.Tbl39Infraordos.SingleOrDefault(a => a.InfraordoId == id &&
                                                                     a.InfraordoName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoId(int id)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.InfraordoId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesByInfraordoIdAndHash(int id)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => e.InfraordoId == id &&
                            e.SuperfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.InfraordoId == id &&
            //               e.SuperfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubordoIdFromInfraordosCollectionSelect(int id)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var dbset = _context.Tbl39Infraordos
                .SingleOrDefault(p => p.InfraordoId == id);

            if (dbset != null)
            {
                return dbset.SubordoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl36Subordo> CollSubordosBySubordoIdAndHash(int id)
    {
        if (_context.Tbl36Subordos != null)
        {
            var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => e.SubordoId == id &&
                            e.SubordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl36Subordo>(_uow.Tbl36Subordos
            //    .Find(e => e.SubordoId == id &&
            //               e.SubordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfraordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfraordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfraordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfraordoId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfraordoId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfraordoId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfraordoId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.InfraordoId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfraordoId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Superfamily
    public Tbl42Superfamily GetSuperfamilySingleBySuperfamilyIdAndHash(int id)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var single = _context.Tbl42Superfamilies.SingleOrDefault(a => a.SuperfamilyId == id &&
                                                                       a.SuperfamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyId(int id)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.SuperfamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl45Family> CollFamiliesBySuperfamilyIdAndHash(int id)
    {
        if (_context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => e.SuperfamilyId == id &&
                            e.FamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.SuperfamilyId == id &&
            //               e.FamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfraordoIdFromSuperfamiliesCollectionSelect(int id)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var dbset = _context.Tbl42Superfamilies
                .SingleOrDefault(p => p.SuperfamilyId == id);

            if (dbset != null)
            {
                return dbset.InfraordoId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoIdAndHash(int id)
    {
        if (_context.Tbl39Infraordos != null)
        {
            var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id &&
                            e.InfraordoName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl39Infraordo>(_uow.Tbl39Infraordos
            //    .Find(e => e.InfraordoId == id &&
            //               e.InfraordoName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySuperfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SuperfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySuperfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SuperfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySuperfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SuperfamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySuperfamilyId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SuperfamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SuperfamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Family
    public Tbl45Family GetFamilySingleByFamilyIdAndHash(int id)
    {
        if (_context.Tbl45Families != null)
        {
            var single = _context.Tbl45Families.SingleOrDefault(a => a.FamilyId == id &&
                                                                  a.FamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl45Family> CollFamiliesByFamilyId(int id)
    {
        if (_context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => e.FamilyId == id));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.FamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesByFamilyIdAndHash(int id)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => e.FamilyId == id &&
                            e.SubfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.FamilyId == id &&
            //               e.SubfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SuperfamilyIdFromFamiliesCollectionSelect(int id)
    {
        if (_context.Tbl45Families != null)
        {
            var dbset = _context.Tbl45Families
                .SingleOrDefault(p => p.FamilyId == id);

            if (dbset != null)
            {
                return dbset.SuperfamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyIdAndHash(int id)
    {
        if (_context.Tbl42Superfamilies != null)
        {
            var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id &&
                            e.SuperfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl42Superfamily>(_uow.Tbl42Superfamilies
            //    .Find(e => e.SuperfamilyId == id &&
            //               e.SuperfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByFamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.FamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByFamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.FamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByFamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.FamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByFamilyId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.FamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.FamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subfamily
    public Tbl48Subfamily GetSubfamilySingleBySubfamilyIdAndHash(int id)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var single = _context.Tbl48Subfamilies.SingleOrDefault(a => a.SubfamilyId == id &&
                                                                     a.SubfamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyId(int id)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.SubfamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesBySubfamilyIdAndHash(int id)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => e.SubfamilyId == id &&
                            e.InfrafamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.SubfamilyId == id &&
            //               e.InfrafamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int FamilyIdFromSubfamiliesCollectionSelect(int id)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var dbset = _context.Tbl48Subfamilies
                .SingleOrDefault(p => p.SubfamilyId == id);

            if (dbset != null)
            {
                return dbset.FamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl45Family> CollFamiliesByFamilyIdAndHash(int id)
    {
        if (_context.Tbl45Families != null)
        {
            var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => e.FamilyId == id &&
                            e.FamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl45Family>(_uow.Tbl45Families
            //    .Find(e => e.FamilyId == id &&
            //               e.FamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubfamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubfamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubfamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubfamilyId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SubfamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubfamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infrafamily
    public Tbl51Infrafamily GetInfrafamilySingleByInfrafamilyIdAndHash(int id)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var single = _context.Tbl51Infrafamilies.SingleOrDefault(a => a.InfrafamilyId == id &&
                                                                       a.InfrafamilyName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyId(int id)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.InfrafamilyId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl54Supertribus> CollSupertribussesByInfrafamilyIdAndHash(int id)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => e.InfrafamilyId == id &&
                            e.SupertribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.InfrafamilyId == id &&
            //               e.SupertribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubfamilyIdFromInfrafamiliesCollectionSelect(int id)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var dbset = _context.Tbl51Infrafamilies
                .SingleOrDefault(p => p.InfrafamilyId == id);

            if (dbset != null)
            {
                return dbset.SubfamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyIdAndHash(int id)
    {
        if (_context.Tbl48Subfamilies != null)
        {
            var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id &&
                            e.SubfamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl48Subfamily>(_uow.Tbl48Subfamilies
            //    .Find(e => e.SubfamilyId == id &&
            //               e.SubfamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfrafamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfrafamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfrafamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfrafamilyId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfrafamilyId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfrafamilyId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfrafamilyId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.InfrafamilyId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfrafamilyId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Supertribus
    public Tbl54Supertribus GetSupertribusSingleBySupertribusIdAndHash(int id)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var single = _context.Tbl54Supertribusses.SingleOrDefault(a => a.SupertribusId == id &&
                                                                       a.SupertribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusId(int id)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.SupertribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl57Tribus> CollTribussesBySupertribusIdAndHash(int id)
    {
        if (_context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => e.SupertribusId == id &&
                            e.TribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.SupertribusId == id &&
            //               e.TribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int InfrafamilyIdFromSupertribussesCollectionSelect(int id)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var dbset = _context.Tbl54Supertribusses
                .SingleOrDefault(p => p.SupertribusId == id);

            if (dbset != null)
            {
                return dbset.InfrafamilyId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyIdAndHash(int id)
    {
        if (_context.Tbl51Infrafamilies != null)
        {
            var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id &&
                            e.InfrafamilyName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl51Infrafamily>(_uow.Tbl51Infrafamilies
            //    .Find(e => e.InfrafamilyId == id &&
            //               e.InfrafamilyName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySupertribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SupertribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySupertribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SupertribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySupertribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SupertribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySupertribusId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SupertribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SupertribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Tribus
    public Tbl57Tribus GetTribusSingleByTribusIdAndHash(int id)
    {
        if (_context.Tbl57Tribusses != null)
        {
            var single = _context.Tbl57Tribusses.SingleOrDefault(a => a.TribusId == id &&
                                                                  a.TribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl57Tribus> CollTribussesByTribusId(int id)
    {
        if (_context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => e.TribusId == id));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.TribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl60Subtribus> CollSubtribussesByTribusIdAndHash(int id)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => e.TribusId == id &&
                            e.SubtribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.TribusId == id &&
            //               e.SubtribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SupertribusIdFromTribussesCollectionSelect(int id)
    {
        //if (_context.Tbl57Tribusses != null)
        //{
        //    var dbset = _context.Tbl57Tribusses
        //        .SingleOrDefault(p => p.TribusId == id);

        //    if (dbset != null)
        //    {
        //        return dbset.SupertribusId;
        //    }
        //}

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusIdAndHash(int id)
    {
        if (_context.Tbl54Supertribusses != null)
        {
            var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
                .Where(e => e.SupertribusId == id &&
                            e.SupertribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl54Supertribus>(_uow.Tbl54Supertribusses
            //    .Find(e => e.SupertribusId == id &&
            //               e.SupertribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByTribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.TribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByTribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.TribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByTribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.TribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByTribusId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.TribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.TribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Subtribus
    public Tbl60Subtribus GetSubtribusSingleBySubtribusIdAndHash(int id)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var single = _context.Tbl60Subtribusses.SingleOrDefault(a => a.SubtribusId == id &&
                                                                     a.SubtribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusId(int id)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.SubtribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl63Infratribus> CollInfratribussesBySubtribusIdAndHash(int id)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
                .Where(e => e.SubtribusId == id &&
                            e.InfratribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl63Infratribus>(_uow.Tbl63Infratribusses
            //    .Find(e => e.SubtribusId == id &&
            //               e.InfratribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int TribusIdFromSubtribussesCollectionSelect(int id)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var dbset = _context.Tbl60Subtribusses
                .SingleOrDefault(p => p.SubtribusId == id);

            if (dbset != null)
            {
              //  return dbset.TribusId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl57Tribus> CollTribussesByTribusIdAndHash(int id)
    {
        if (_context.Tbl57Tribusses != null)
        {
            var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
                .Where(e => e.TribusId == id &&
                            e.TribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl57Tribus>(_uow.Tbl57Tribusses
            //    .Find(e => e.TribusId == id &&
            //               e.TribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySubtribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SubtribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySubtribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SubtribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySubtribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SubtribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySubtribusId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SubtribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.SubtribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Infratribus
    public Tbl63Infratribus GetInfratribusSingleByInfratribusIdAndHash(int id)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var single = _context.Tbl63Infratribusses.SingleOrDefault(a => a.InfratribusId == id &&
                                                                       a.InfratribusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusId(int id)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
                .Where(e => e.InfratribusId == id));
            //var collection = new ObservableCollection<Tbl63Infratribus>(_uow.Tbl63Infratribusses
            //    .Find(e => e.InfratribusId == id));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl66Genus> CollGenussesByInfratribusIdAndHash(int id)
    {
        if (_context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => e.InfratribusId == id &&
                            e.GenusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl66Genus>(_uow.Tbl66Genusses
            //    .Find(e => e.InfratribusId == id &&
            //               e.GenusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Function
    public int SubtribusIdFromInfratribussesCollectionSelect(int id)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var dbset = _context.Tbl63Infratribusses
                .SingleOrDefault(p => p.InfratribusId == id);

            if (dbset != null)
            {
     //           return dbset.SubtribusId;
            }
        }

        return 0;
    }
    // ForeignKey
    public ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusIdAndHash(int id)
    {
        if (_context.Tbl60Subtribusses != null)
        {
            var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
                .Where(e => e.SubtribusId == id &&
                            e.SubtribusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl60Subtribus>(_uow.Tbl60Subtribusses
            //    .Find(e => e.SubtribusId == id &&
            //               e.SubtribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsByInfratribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.InfratribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByInfratribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.InfratribusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByInfratribusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.InfratribusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByInfratribusId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.InfratribusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_uow.Tbl93Comments
            //    .Find(e => e.InfratribusId == id));
            return collection;
        }
        return null!;
    }
    #endregion

    #region Genus
    public Tbl66Genus GetGenusSingleByGenusIdAndHash(int id)
    {
        if (_context.Tbl66Genusses != null)
        {
            var single = _context.Tbl66Genusses.SingleOrDefault(a => a.GenusId == id &&
                                                                 a.GenusName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public ObservableCollection<Tbl66Genus> CollGenussesByGenusId(int id)
    {
        if (_context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => e.GenusId == id));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndHash(int id)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => e.GenusId == id &&
                            e.SpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    public ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                //        .Include(a => a.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.Subspecies == null &&
                            e.SpeciesName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }

    //public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndHash(int id)
    //{
    //    if (_context.Tbl72PlSpeciesses != null)
    //    {
    //        var collection = new ObservableCollection<Tbl72PlSpecies>(_context.Tbl72PlSpeciesses
    //            .Where(e => e.GenusId == id &&
    //                        e.PlSpeciesName.Contains(Value) == false));
    //        return collection;
    //    }
    //    return null!;
    //}

    //public ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id)
    //{
    //    if (_context.Tbl72PlSpeciesses != null)
    //    {
    //        var collection = new ObservableCollection<Tbl72PlSpecies>(_context.Tbl72PlSpeciesses
    //            .Include(a => a.Tbl66Genusses)
    //            .Where(e => e.GenusId == id &&
    //                        e.Subspecies == null &&
    //                        e.PlSpeciesName.Contains(Value) == false));
    //        return collection;
    //    }
    //    return null!;
    //}

    //Function
    public int InfratribusIdFromGenussesCollectionSelect(int id)
    {
        if (_context.Tbl66Genusses != null)
        {
            var dbset = _context.Tbl66Genusses
                .SingleOrDefault(p => p.GenusId == id);

            if (dbset != null)
            {
                return dbset.InfratribusId ?? 0;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusIdAndHash(int id)
    {
        if (_context.Tbl63Infratribusses != null)
        {
            var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
                .Where(e => e.InfratribusId == id &&
                            e.InfratribusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //References
    public ObservableCollection<Tbl90Reference> CollExpertsByGenusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.GenusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesByGenusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.GenusId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsByGenusId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.GenusId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    //Comments
    public ObservableCollection<Tbl93Comment> CollCommentsByGenusId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.GenusId == id));
            //var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
            //    .Where(e => e.GenusId == id));
            return collection;
        }
        return null!;
    }
    #endregion Genus

    #region Species
    public Tbl75Species GetSpeciesSingleBySpeciesIdAndHash(int id)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var single = _context.Tbl75Speciesses.SingleOrDefault(a => a.SpeciesId == id &&
                                                                     a.SpeciesName.Contains(Value) == false);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }

    public Tbl75Species GetSpeciesSingleBySpeciesId(int speciesid)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = _context.Tbl75Speciesses.FirstOrDefault(a => a.SpeciesId == speciesid);
        return single;
    }
    public Tbl75Species GetSpeciesSingleModelBySpeciesId(int speciesid)
    {
        // var single = _uow.Tbl69FiSpeciesses.GetById(id);
        var single = _context.Tbl75Speciesses.FirstOrDefault(a => a.SpeciesId == speciesid);
        return single;
    }

    public ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesId(int speciesid)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => e.SpeciesId == speciesid));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl75Species> CollSpeciesSubsBySpeciesNameandGenusId(string name, int genusId)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Include(d => d.Genus)
                .Where(e => e.GenusId == genusId &&
                            e.SpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Genus.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesNameAndNotEmptySubspecies(int id, string name)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Include(d => d.Genus)
                .Where(e => e.GenusId == id &&
                            e.SpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Genus.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }

    //FiSpeciessesCollection = new ObservableCollection<Tbl69FiSpecies>(__context.Tbl69FiSpeciesses
    //.Include(a => a.Tbl66Genusses)
    //.Where(e => e.GenusId == id &&
    //e.Subspecies == null &&
    //e.FiSpeciesName.Contains(Value) == false));

    public ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndSpeciesNameAndEmptySubspecies(int id, string name)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Include(d => d.Genus)
                .Where(e => e.GenusId == id &&
                            e.SpeciesName == name &&
                            (e.Subspecies == null || e.Subspecies == string.Empty) &&
                            (e.Divers == null || e.Divers == string.Empty))
                .OrderBy(a => a.Genus.GenusName)
                .ThenBy(a => a.SpeciesName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesNameAndSubspeciesAndDivers(string speciesName, string subspecies,
        string divers)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Include(d => d.Genus)
                .Where(e => e.SpeciesName == speciesName && e.Subspecies == subspecies && e.Divers == divers)
                .OrderBy(a => a.Genus.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
            return collection;
        }
        return null!;
    }
    //direct children
    public ObservableCollection<Tbl78Name> CollNamesBySpeciesIdAndHash(int id)
    {
        if (_context.Tbl78Names != null)
        {
            var collection = new ObservableCollection<Tbl78Name>(_context.Tbl78Names
                .Where(e => e.SpeciesId == id &&
                            e.NameName.Contains(Value) == false)
                .OrderBy(r => r.NameName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl84Synonym> CollSynonymsBySpeciesIdAndHash(int id)
    {
        if (_context.Tbl84Synonyms != null)
        {
            var collection = new ObservableCollection<Tbl84Synonym>(_context.Tbl84Synonyms
                .Where(e => e.SpeciesId == id &&
                            e.SynonymName.Contains(Value) == false)
                .OrderBy(r => r.SynonymName));
            return collection;
        }
        return null!;
    }
    //Function
    public int GenusIdFromSpeciessesCollectionSelect(int id)
    {
        if (_context.Tbl75Speciesses != null)
        {
            var dbset = _context.Tbl75Speciesses
                .SingleOrDefault(p => p.SpeciesId == id);

            if (dbset != null)
            {
                return dbset.GenusId;
            }
        }

        return 0;
    }
    //ForeignKey
    public ObservableCollection<Tbl66Genus> CollGenussesByGenusIdAndHash(int id)
    {
        if (_context.Tbl66Genusses != null)
        {
            var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
                .Where(e => e.GenusId == id &&
                            e.GenusName.Contains(Value) == false));
            //var collection = new ObservableCollection<Tbl66Genus>(_uow.Tbl66Genusses
            //    .Find(e => e.GenusId == id &&
            //               e.GenusName.Contains(Value) == false));
            return collection;
        }
        return null!;
    }
    //Reference
    public ObservableCollection<Tbl90Reference> CollExpertsBySpeciesId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefExperts)
                .Where(e => e.RefExpertId == e.Tbl90RefExperts.RefExpertId &&
                            e.SpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefSourceId.HasValue == false)
                .OrderBy(a => a.Tbl90RefExperts.RefExpertName));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollSourcesBySpeciesId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefSources)
                .Where(e => e.RefSourceId == e.Tbl90RefSources.RefSourceId &&
                            e.SpeciesId == id &&
                            e.RefAuthorId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(a => a.Tbl90RefSources.RefSourceName)
                .ThenBy(a => a.Tbl90RefSources.SourceYear));
            return collection;
        }
        return null!;
    }
    public ObservableCollection<Tbl90Reference> CollAuthorsBySpeciesId(int id)
    {
        if (_context.Tbl90References != null)
        {
            var collection = new ObservableCollection<Tbl90Reference>(_context.Tbl90References
                .Include(a => a.Tbl90RefAuthors)
                .Where(e => e.RefAuthorId == e.Tbl90RefAuthors.RefAuthorId &&
                            e.SpeciesId == id &&
                            e.RefSourceId.HasValue == false &&
                            e.RefExpertId.HasValue == false)
                .OrderBy(e => e.Tbl90RefAuthors.RefAuthorName)
                .ThenBy(e => e.Tbl90RefAuthors.ArticelTitle)
                .ThenBy(e => e.Tbl90RefAuthors.BookName)
                .ThenBy(e => e.Tbl90RefAuthors.Page1)
                .ThenBy(e => e.Tbl90RefAuthors.Publisher));
            return collection;
        }
        return null!;
    }
    // Comments
    public ObservableCollection<Tbl93Comment> CollCommentsBySpeciesId(int id)
    {
        if (_context.Tbl93Comments != null)
        {
            var collection = new ObservableCollection<Tbl93Comment>(_context.Tbl93Comments
                .Where(e => e.SpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Images
    public ObservableCollection<Tbl81Image> CollImagesBySpeciesId(int id)
    {
        if (_context.Tbl81Images != null)
        {
            var collection = new ObservableCollection<Tbl81Image>(_context.Tbl81Images
                .Where(e => e.SpeciesId == id));
            return collection;
        }
        return null!;
    }
    //Geographics
    public ObservableCollection<Tbl87Geographic> CollGeographicsBySpeciesId(int id)
    {
        if (_context.Tbl87Geographics != null)
        {
            var collection = new ObservableCollection<Tbl87Geographic>(_context.Tbl87Geographics
                .Where(e => e.SpeciesId == id));
            return collection;
        }
        return null!;
    }


    #endregion

    #region Disease

    public TblDisease GetDiseaseSingleByDiseaseId(int id)
    {
        if (_context.TblDiseases != null)
        {
            var single = _context.TblDiseases.SingleOrDefault(a => a.DiseaseId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }
    public ObservableCollection<TblDisease> CollDiseasesByDiseaseId(int id)
    {
        if (_context.TblDiseases != null)
        {
            var collection = new ObservableCollection<TblDisease>(_context.TblDiseases
                .Where(e => e.DiseaseId == id));
            return collection;
        }
        return null!;
    }


    #endregion Disease

    #region Food

    public ObservableCollection<TblFood> CollFoodsByFoodId(int id)
    {
        if (_context.TblFoods != null)
        {
            var collection = new ObservableCollection<TblFood>(_context.TblFoods
                .Where(e => e.FoodId == id));
            return collection;
        }
        return null!;
    }


    public TblFood GetFoodSingleByFoodId(int id)
    {
        if (_context.TblFoods != null)
        {
            var single = _context.TblFoods.SingleOrDefault(a => a.FoodId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }


    #endregion Food

    #region Miscellaneous

    public ObservableCollection<TblMiscellaneous> CollMiscellaneoussesByMiscellaneousId(int id)
    {
        if (_context.TblMiscellaneousses != null)
        {
            var collection = new ObservableCollection<TblMiscellaneous>(_context.TblMiscellaneousses
                .Where(e => e.MiscellaneousId == id));
            return collection;
        }
        return null!;
    }

    public TblMiscellaneous GetMiscellaneousSingleByMiscellaneousId(int id)
    {
        if (_context.TblMiscellaneousses != null)
        {
            var single = _context.TblMiscellaneousses.SingleOrDefault(a => a.MiscellaneousId == id);
            if (single != null)
            {
                return single;
            }
        }
        return null!;
    }


    #endregion Miscellaneous

    #endregion

    #region "Public ObservableCollections"

    public string SearchRegnumName
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> Tbl03RegnumsAllList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl06Phylum> Tbl06PhylumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl12Subphylum> Tbl12SubphylumsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl09Division> Tbl09DivisionsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl15Subdivision> Tbl15SubdivisionsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl18Superclass> Tbl18SuperclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl21Class> Tbl21ClassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl24Subclass> Tbl24SubclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl27Infraclass> Tbl27InfraclassesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl30Legio> Tbl30LegiosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl33Ordo> Tbl33OrdosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl36Subordo> Tbl36SubordosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl39Infraordo> Tbl39InfraordosList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl42Superfamily> Tbl42SuperfamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl45Family> Tbl45FamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl48Subfamily> Tbl48SubfamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl51Infrafamily> Tbl51InfrafamiliesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl54Supertribus> Tbl54SupertribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl57Tribus> Tbl57TribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl60Subtribus> Tbl60SubtribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl63Infratribus> Tbl63InfratribussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl66Genus> Tbl66GenussesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl75Species> Tbl75SpeciessesList
    {
        get; set;
    } = null!;

    //public ObservableCollection<Tbl72PlSpecies> Tbl72PlSpeciessesList
    //{
    //    get; set;
    //} = null!;

    public ObservableCollection<Tbl78Name> Tbl78NamesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl81Image> Tbl81ImagesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl84Synonym> Tbl84SynonymsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl87Geographic> Tbl87GeographicsList
    {
        get; set;
    } = null!;


    public ObservableCollection<Tbl90Reference> Tbl90ReferencesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl93Comment> Tbl93CommentsList
    {
        get; set;
    } = null!;

    #endregion "Public ObservableCollections"     

    #region "Collections"

    public string FilterText
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl03Regnum> RegnumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl06Phylum> PhylumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl12Subphylum> SubphylumsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl09Division> DivisionsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl15Subdivision> SubdivisionsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl18Superclass> SuperclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl21Class> ClassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl24Subclass> SubclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl27Infraclass> InfraclassesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl30Legio> LegiosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl33Ordo> OrdosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl36Subordo> SubordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl39Infraordo> InfraordosCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl45Family> FamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl48Subfamily> SubfamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl54Supertribus> SupertribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl57Tribus> TribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl60Subtribus> SubtribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl63Infratribus> InfratribussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl66Genus> GenussesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl75Species> SpeciessesCollection
    {
        get; set;
    } = null!;
    //public ObservableCollection<Tbl69FiSpecies> FiSpeciessesCollection
    //{
    //    get; set;
    //} = null!;

    //public ObservableCollection<Tbl72PlSpecies> PlSpeciessesCollection
    //{
    //    get; set;
    //} = null!;

    public ObservableCollection<Tbl78Name> NamesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl84Synonym> SynonymsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> AuthorsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefAuthorsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> SourcesCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefSourcesList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> ExpertsCollection
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl90Reference> Tbl90RefExpertsList
    {
        get; set;
    } = null!;

    public ObservableCollection<Tbl93Comment> CommentsCollection
    {
        get; set;
    } = null!;

    #endregion "Collections"


}
