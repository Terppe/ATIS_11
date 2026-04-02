using System.Collections.ObjectModel;
using System.Diagnostics;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using Microsoft.EntityFrameworkCore;
using static ATIS.Interfaces.IDataService;

//https://www.reddit.com/r/reactjs/comments/m4mhp7/how_to_update_state_in_useeffect_when_getting/

namespace ATIS.Services;

public partial class DataService : ObservableObject, IDataService
{
    private const string Value = "#";

    private readonly IUserService _user;
    private readonly AtisDbContext _context;
    public DataService(AtisDbContext context, IUserService user)
    {
        _context = context;
        _user = user;
    }

    private void ApplyAuditInfo(IAuditable entity, bool isNew)
    {
        if (isNew)
        {
            entity.Writer = _user.CurrentUser;
            entity.WriterDate = DateTime.Now;
            entity.CountId = RandomHelper.Randomnumber();
        }

        entity.Updater = _user.CurrentUser;
        entity.UpdaterDate = DateTime.Now;
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
    public async Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum regnum)
    {
        var current = await _context.Tbl03Regnums.FirstOrDefaultAsync(_regnum => _regnum.RegnumId == regnum.RegnumId);
        if (null == current)
        {
            regnum.CountId = RandomHelper.Randomnumber();
            regnum.Writer = Environment.UserName;
            regnum.WriterDate = DateTime.Now;
            regnum.Updater = Environment.UserName;
            regnum.UpdaterDate = DateTime.Now;
            _context.Tbl03Regnums.Add(regnum);
        }
        else
        {
            regnum.Updater = Environment.UserName;
            regnum.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(regnum);
        }
        await _context.SaveChangesAsync();
        return regnum;
    }
    public async Task DeleteRegnumAsync(int regnumid)
    {
        var regnum = await _context.Tbl03Regnums.FirstOrDefaultAsync(reg => reg.RegnumId == regnumid);
        if (regnum != null)
        {
            var references = await _context.Tbl90References.Where(reference => reference.RegnumId == regnumid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.RegnumId == regnumid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl03Regnums.Remove(regnum);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Regnum

    #region Phylum

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
    public async Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum phylum)
    {
        var current = await _context.Tbl06Phylums.FirstOrDefaultAsync(_phy => _phy.PhylumId == phylum.PhylumId);
        if (null == current)
        {
            phylum.CountId = RandomHelper.Randomnumber();
            phylum.Writer = Environment.UserName;
            phylum.WriterDate = DateTime.Now;
            phylum.Updater = Environment.UserName;
            phylum.UpdaterDate = DateTime.Now;
            _context.Tbl06Phylums.Add(phylum);
        }
        else
        {
            phylum.Updater = Environment.UserName;
            phylum.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(phylum);
        }
        await _context.SaveChangesAsync();
        return phylum;
    }
    public async Task DeletePhylumAsync(int phylumid)
    {
        var phylum = await _context.Tbl06Phylums.FirstOrDefaultAsync(b => b.PhylumId == phylumid);
        if (phylum != null)
        {
            //necessary to delete before
            var subphylums = await _context.Tbl12Subphylums.Where(a => a.PhylumId == phylumid).ToListAsync();
            foreach (var subphylum in subphylums)
            {
                for (var i = 0; i <= subphylums.Count; i++)
                {
                    var subphylumid = subphylum.SubphylumId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SubphylumId == subphylumid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SubphylumId == subphylumid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl12Subphylums.RemoveRange(subphylums);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(reference => reference.PhylumId == phylumid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.PhylumId == phylumid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl06Phylums.Remove(phylum);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Phylum

    #region Subphylum

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
    public async Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum subphylum)
    {
        var current = await _context.Tbl12Subphylums.FirstOrDefaultAsync(a => a.SubphylumId == subphylum.SubphylumId);
        if (null == current)
        {
            subphylum.CountId = RandomHelper.Randomnumber();
            subphylum.Writer = Environment.UserName;
            subphylum.WriterDate = DateTime.Now;
            subphylum.Updater = Environment.UserName;
            subphylum.UpdaterDate = DateTime.Now;
            _context.Tbl12Subphylums.Add(subphylum);
        }
        else
        {
            subphylum.Updater = Environment.UserName;
            subphylum.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(subphylum);
        }
        await _context.SaveChangesAsync();
        return subphylum;
    }
    public async Task DeleteSubphylumAsync(int subphylumid)
    {
        var subphylum = await _context.Tbl12Subphylums.FirstOrDefaultAsync(reg => reg.SubphylumId == subphylumid);
        if (subphylum != null)
        {
            //necessary to delete before
            var superclasss = await _context.Tbl18Superclasses.Where(a => a.SubphylumId == subphylumid).ToListAsync();
            foreach (var superclass in superclasss)
            {
                for (var i = 0; i <= superclasss.Count; i++)
                {
                    var superclassid = superclass.SuperclassId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SuperclassId == superclassid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            _context.Tbl18Superclasses.RemoveRange(superclasss);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(r => r.SubphylumId == subphylumid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(c => c.SubphylumId == subphylumid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl12Subphylums.Remove(subphylum);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Subphylum

    #region Superclass

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

    public async Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass superclass)
    {
        var current = await _context.Tbl18Superclasses.FirstOrDefaultAsync(a => a.SuperclassId == superclass.SuperclassId);
        if (null == current)
        {
            superclass.CountId = RandomHelper.Randomnumber();
            superclass.Writer = Environment.UserName;
            superclass.WriterDate = DateTime.Now;
            superclass.Updater = Environment.UserName;
            superclass.UpdaterDate = DateTime.Now;
            _context.Tbl18Superclasses.Add(superclass);
        }
        else
        {
            superclass.Updater = Environment.UserName;
            superclass.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(superclass);
        }
        await _context.SaveChangesAsync();
        return superclass;
    }

    public async Task DeleteSuperclassAsync(int superclassid)
    {
        var superclass = await _context.Tbl18Superclasses.FirstOrDefaultAsync(reg => reg.SuperclassId == superclassid);
        if (superclass != null)
        {
            //necessary to delete before evtl noch weiter nach oben bis FiSpecies 
            var classes = await _context.Tbl21Classes.Where(a => a.SuperclassId == superclassid).ToListAsync();
            foreach (var class1 in classes)
            {
                for (var i = 0; i <= classes.Count; i++)
                {
                    var classid = class1.ClassId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.ClassId == classid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.ClassId == classid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            _context.Tbl21Classes.RemoveRange(classes);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(r => r.SuperclassId == superclassid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(c => c.SuperclassId == superclassid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl18Superclasses.Remove(superclass);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Superclass

    #region Class                

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

    public async Task<Tbl21Class> UpsertClassAsync(Tbl21Class class1)
    {
        var current = await _context.Tbl21Classes.FirstOrDefaultAsync(c => c.ClassId == class1.ClassId);
        if (null == current)
        {
            class1.CountId = RandomHelper.Randomnumber();
            class1.Writer = Environment.UserName;
            class1.WriterDate = DateTime.Now;
            class1.Updater = Environment.UserName;
            class1.UpdaterDate = DateTime.Now;
            _context.Tbl21Classes.Add(class1);
        }
        else
        {
            class1.Updater = Environment.UserName;
            class1.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(class1);
        }
        await _context.SaveChangesAsync();
        return class1;
    }

    public async Task DeleteClassAsync(int classid)
    {
        var class1 = await _context.Tbl21Classes.FirstOrDefaultAsync(b => b.ClassId == classid);
        if (class1 != null)
        {
            //necessary to delete before
            var subclasss = await _context.Tbl24Subclasses.Where(a => a.ClassId == classid).ToListAsync();
            foreach (var subclass in subclasss)
            {
                for (var i = 0; i <= subclasss.Count; i++)
                {
                    var subclassid = subclass.SubclassId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SubclassId == subclassid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SubclassId == subclassid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl24Subclasses.RemoveRange(subclasss);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(reference => reference.ClassId == classid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.ClassId == classid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl21Classes.Remove(class1);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Class                       

    #region Subclass                

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

    public async Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass subclass)
    {
        var current = await _context.Tbl24Subclasses.FirstOrDefaultAsync(c => c.SubclassId == subclass.SubclassId);
        if (null == current)
        {
            subclass.CountId = RandomHelper.Randomnumber();
            subclass.Writer = Environment.UserName;
            subclass.WriterDate = DateTime.Now;
            subclass.Updater = Environment.UserName;
            subclass.UpdaterDate = DateTime.Now;
            _context.Tbl24Subclasses.Add(subclass);
        }
        else
        {
            subclass.Updater = Environment.UserName;
            subclass.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(subclass);
        }
        await _context.SaveChangesAsync();
        return subclass;
    }

    public async Task DeleteSubclassAsync(int subclassid)
    {
        var subclass = await _context.Tbl24Subclasses.FirstOrDefaultAsync(b => b.SubclassId == subclassid);
        if (subclass != null)
        {
            //necessary to delete before
            var infraclasss = await _context.Tbl27Infraclasses.Where(a => a.SubclassId == subclassid).ToListAsync();
            foreach (var infraclass in infraclasss)
            {
                for (var i = 0; i <= infraclasss.Count; i++)
                {
                    var infraclassid = infraclass.InfraclassId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.InfraclassId == infraclassid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.InfraclassId == infraclassid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl27Infraclasses.RemoveRange(infraclasss);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(reference => reference.SubclassId == subclassid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SubclassId == subclassid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl24Subclasses.Remove(subclass);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Subclass                       

    #region Infraclass                

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync()
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
            .AsNoTracking()
            .OrderBy(a => a.InfraclassName));
        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(int infraclassid)
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
            .AsNoTracking()
            .Where(e => e.InfraclassId == infraclassid)
            .OrderBy(k => k.InfraclassName)
        );

        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => e.InfraclassId == id))
            : new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
                .Where(e => search != null && e.InfraclassName.StartsWith(search))
                .OrderBy(a => a.InfraclassName));
        return collection;
    }

    public ObservableCollection<Tbl27Infraclass> GetInfraclassFromSubclassIdAsync(int subclassid)
    {
        var collection = new ObservableCollection<Tbl27Infraclass>(_context.Tbl27Infraclasses
            .Where(e => e.SubclassId == subclassid)
            .OrderBy(k => k.InfraclassName)
        );

        return collection;
    }

    public async Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass infraclass)
    {
        var current = await _context.Tbl27Infraclasses.FirstOrDefaultAsync(c => c.InfraclassId == infraclass.InfraclassId);
        if (null == current)
        {
            infraclass.CountId = RandomHelper.Randomnumber();
            infraclass.Writer = Environment.UserName;
            infraclass.WriterDate = DateTime.Now;
            infraclass.Updater = Environment.UserName;
            infraclass.UpdaterDate = DateTime.Now;
            _context.Tbl27Infraclasses.Add(infraclass);
        }
        else
        {
            infraclass.Updater = Environment.UserName;
            infraclass.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(infraclass);
        }
        await _context.SaveChangesAsync();
        return infraclass;
    }

    public async Task DeleteInfraclassAsync(int infraclassid)
    {
        var infraclass = await _context.Tbl27Infraclasses.FirstOrDefaultAsync(b => b.InfraclassId == infraclassid);
        if (infraclass != null)
        {
            //necessary to delete before
            var legios = await _context.Tbl30Legios.Where(a => a.InfraclassId == infraclassid).ToListAsync();
            foreach (var legio in legios)
            {
                for (var i = 0; i <= legios.Count; i++)
                {
                    var legioid = legio.LegioId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.LegioId == legioid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.LegioId == legioid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl30Legios.RemoveRange(legios);
            await _context.SaveChangesAsync();

            var references = await _context.Tbl90References.Where(reference => reference.InfraclassId == infraclassid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.InfraclassId == infraclassid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl27Infraclasses.Remove(infraclass);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Infraclass                       

    #region Legio                

    public ObservableCollection<Tbl30Legio> GetLegioAsync()
    {
        var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
            .AsNoTracking()
            .OrderBy(a => a.LegioName));
        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioAsync(int legioid)
    {
        var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
            .AsNoTracking()
            .Where(e => e.LegioId == legioid)
            .OrderBy(k => k.LegioName)
        );

        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => e.LegioId == id))
            : new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
                .Where(e => search != null && e.LegioName.StartsWith(search))
                .OrderBy(a => a.LegioName));
        return collection;
    }

    public ObservableCollection<Tbl30Legio> GetLegioFromInfraclassIdAsync(int infraclassid)
    {
        var collection = new ObservableCollection<Tbl30Legio>(_context.Tbl30Legios
            .Where(e => e.InfraclassId == infraclassid)
            .OrderBy(k => k.LegioName)
        );

        return collection;
    }

    public async Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio legio)
    {
        var current = await _context.Tbl30Legios.FirstOrDefaultAsync(c => c.LegioId == legio.LegioId);
        if (null == current)
        {
            legio.CountId = RandomHelper.Randomnumber();
            legio.Writer = Environment.UserName;
            legio.WriterDate = DateTime.Now;
            legio.Updater = Environment.UserName;
            legio.UpdaterDate = DateTime.Now;
            _context.Tbl30Legios.Add(legio);
        }
        else
        {
            legio.Updater = Environment.UserName;
            legio.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(legio);
        }
        await _context.SaveChangesAsync();
        return legio;
    }

    public async Task DeleteLegioAsync(int legioid)
    {
        var legio = await _context.Tbl30Legios.FirstOrDefaultAsync(b => b.LegioId == legioid);
        if (legio != null)
        {
            //necessary to delete before
            //------ Ordo -------------
            var ordos = await _context.Tbl33Ordos.Where(a => a.LegioId == legioid).ToListAsync();
            foreach (var ordo in ordos)
            {
                for (var i = 0; i <= ordos.Count; i++)
                {
                    var ordoid = ordo.OrdoId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.OrdoId == ordoid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.OrdoId == ordoid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl33Ordos.RemoveRange(ordos);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.LegioId == legioid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.LegioId == legioid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl30Legios.Remove(legio);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Legio                       

    #region Ordo                

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync()
    {
        var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
            .AsNoTracking()
            .OrderBy(a => a.OrdoName));
        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync(int ordoid)
    {
        var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
            .AsNoTracking()
            .Where(e => e.OrdoId == ordoid)
            .OrderBy(k => k.OrdoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => e.OrdoId == id))
            : new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
                .Where(e => search != null && e.OrdoName.StartsWith(search))
                .OrderBy(a => a.OrdoName));
        return collection;
    }

    public ObservableCollection<Tbl33Ordo> GetOrdoFromLegioIdAsync(int legioid)
    {
        var collection = new ObservableCollection<Tbl33Ordo>(_context.Tbl33Ordos
            .Where(e => e.LegioId == legioid)
            .OrderBy(k => k.OrdoName)
        );

        return collection;
    }

    public async Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo ordo)
    {
        var current = await _context.Tbl33Ordos.FirstOrDefaultAsync(c => c.OrdoId == ordo.OrdoId);
        if (null == current)
        {
            ordo.CountId = RandomHelper.Randomnumber();
            ordo.Writer = Environment.UserName;
            ordo.WriterDate = DateTime.Now;
            ordo.Updater = Environment.UserName;
            ordo.UpdaterDate = DateTime.Now;
            _context.Tbl33Ordos.Add(ordo);
        }
        else
        {
            ordo.Updater = Environment.UserName;
            ordo.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(ordo);
        }
        await _context.SaveChangesAsync();
        return ordo;
    }

    public async Task DeleteOrdoAsync(int ordoid)
    {
        var ordo = await _context.Tbl33Ordos.FirstOrDefaultAsync(b => b.OrdoId == ordoid);
        if (ordo != null)
        {
            //necessary to delete before
            //------ Subordo -------------
            var subordos = await _context.Tbl36Subordos.Where(a => a.OrdoId == ordoid).ToListAsync();
            foreach (var subordo in subordos)
            {
                for (var i = 0; i <= subordos.Count; i++)
                {
                    var subordoid = subordo.SubordoId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SubordoId == subordoid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SubordoId == subordoid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl36Subordos.RemoveRange(subordos);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.OrdoId == ordoid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.OrdoId == ordoid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl33Ordos.Remove(ordo);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Ordo                       

    #region Subordo                

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync()
    {
        var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
            .AsNoTracking()
            .OrderBy(a => a.SubordoName));
        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync(int subordoid)
    {
        var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
            .AsNoTracking()
            .Where(e => e.SubordoId == subordoid)
            .OrderBy(k => k.SubordoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => e.SubordoId == id))
            : new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
                .Where(e => search != null && e.SubordoName.StartsWith(search))
                .OrderBy(a => a.SubordoName));
        return collection;
    }

    public ObservableCollection<Tbl36Subordo> GetSubordoFromOrdoIdAsync(int ordoid)
    {
        var collection = new ObservableCollection<Tbl36Subordo>(_context.Tbl36Subordos
            .Where(e => e.OrdoId == ordoid)
            .OrderBy(k => k.SubordoName)
        );

        return collection;
    }

    public async Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo subordo)
    {
        var current = await _context.Tbl36Subordos.FirstOrDefaultAsync(c => c.SubordoId == subordo.SubordoId);
        if (null == current)
        {
            subordo.CountId = RandomHelper.Randomnumber();
            subordo.Writer = Environment.UserName;
            subordo.WriterDate = DateTime.Now;
            subordo.Updater = Environment.UserName;
            subordo.UpdaterDate = DateTime.Now;
            _context.Tbl36Subordos.Add(subordo);
        }
        else
        {
            subordo.Updater = Environment.UserName;
            subordo.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(subordo);
        }
        await _context.SaveChangesAsync();
        return subordo;
    }

    public async Task DeleteSubordoAsync(int subordoid)
    {
        var subordo = await _context.Tbl36Subordos.FirstOrDefaultAsync(b => b.SubordoId == subordoid);
        if (subordo != null)
        {
            //necessary to delete before
            //------ Infraordo -------------
            var infraordos = await _context.Tbl39Infraordos.Where(a => a.SubordoId == subordoid).ToListAsync();
            foreach (var infraordo in infraordos)
            {
                for (var i = 0; i <= infraordos.Count; i++)
                {
                    var infraordoid = infraordo.InfraordoId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.InfraordoId == infraordoid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.InfraordoId == infraordoid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl39Infraordos.RemoveRange(infraordos);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.SubordoId == subordoid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SubordoId == subordoid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl36Subordos.Remove(subordo);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Subordo                       

    #region Infraordo                

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync()
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
            .AsNoTracking()
            .OrderBy(a => a.InfraordoName));
        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(int infraordoid)
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
            .AsNoTracking()
            .Where(e => e.InfraordoId == infraordoid)
            .OrderBy(k => k.InfraordoName)
        );

        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => e.InfraordoId == id))
            : new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
                .Where(e => search != null && e.InfraordoName.StartsWith(search))
                .OrderBy(a => a.InfraordoName));
        return collection;
    }

    public ObservableCollection<Tbl39Infraordo> GetInfraordoFromSubordoIdAsync(int subordoid)
    {
        var collection = new ObservableCollection<Tbl39Infraordo>(_context.Tbl39Infraordos
            .Where(e => e.SubordoId == subordoid)
            .OrderBy(k => k.InfraordoName)
        );

        return collection;
    }

    public async Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo infraordo)
    {
        var current = await _context.Tbl39Infraordos.FirstOrDefaultAsync(c => c.InfraordoId == infraordo.InfraordoId);
        if (null == current)
        {
            infraordo.CountId = RandomHelper.Randomnumber();
            infraordo.Writer = Environment.UserName;
            infraordo.WriterDate = DateTime.Now;
            infraordo.Updater = Environment.UserName;
            infraordo.UpdaterDate = DateTime.Now;
            _context.Tbl39Infraordos.Add(infraordo);
        }
        else
        {
            infraordo.Updater = Environment.UserName;
            infraordo.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(infraordo);
        }
        await _context.SaveChangesAsync();
        return infraordo;
    }

    public async Task DeleteInfraordoAsync(int infraordoid)
    {
        var infraordo = await _context.Tbl39Infraordos.FirstOrDefaultAsync(b => b.InfraordoId == infraordoid);
        if (infraordo != null)
        {
            //necessary to delete before
            //------ Superfamily -------------
            var superfamilys = await _context.Tbl42Superfamilies.Where(a => a.InfraordoId == infraordoid).ToListAsync();
            foreach (var superfamily in superfamilys)
            {
                for (var i = 0; i <= superfamilys.Count; i++)
                {
                    var superfamilyid = superfamily.SuperfamilyId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SuperfamilyId == superfamilyid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl42Superfamilies.RemoveRange(superfamilys);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.InfraordoId == infraordoid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.InfraordoId == infraordoid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl39Infraordos.Remove(infraordo);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Infraordo                       

    #region Superfamily                

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync()
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
            .AsNoTracking()
            .OrderBy(a => a.SuperfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(int superfamilyid)
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
            .AsNoTracking()
            .Where(e => e.SuperfamilyId == superfamilyid)
            .OrderBy(k => k.SuperfamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => e.SuperfamilyId == id))
            : new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
                .Where(e => search != null && e.SuperfamilyName.StartsWith(search))
                .OrderBy(a => a.SuperfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl42Superfamily> GetSuperfamilyFromInfraordoIdAsync(int infraordoid)
    {
        var collection = new ObservableCollection<Tbl42Superfamily>(_context.Tbl42Superfamilies
            .Where(e => e.InfraordoId == infraordoid)
            .OrderBy(k => k.SuperfamilyName)
        );

        return collection;
    }

    public async Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily superfamily)
    {
        var current = await _context.Tbl42Superfamilies.FirstOrDefaultAsync(c => c.SuperfamilyId == superfamily.SuperfamilyId);
        if (null == current)
        {
            superfamily.CountId = RandomHelper.Randomnumber();
            superfamily.Writer = Environment.UserName;
            superfamily.WriterDate = DateTime.Now;
            superfamily.Updater = Environment.UserName;
            superfamily.UpdaterDate = DateTime.Now;
            _context.Tbl42Superfamilies.Add(superfamily);
        }
        else
        {
            superfamily.Updater = Environment.UserName;
            superfamily.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(superfamily);
        }
        await _context.SaveChangesAsync();
        return superfamily;
    }

    public async Task DeleteSuperfamilyAsync(int superfamilyid)
    {
        var superfamily = await _context.Tbl42Superfamilies.FirstOrDefaultAsync(b => b.SuperfamilyId == superfamilyid);
        if (superfamily != null)
        {
            //necessary to delete before
            //------ Family -------------
            var familys = await _context.Tbl45Families.Where(a => a.SuperfamilyId == superfamilyid).ToListAsync();
            foreach (var family in familys)
            {
                for (var i = 0; i <= familys.Count; i++)
                {
                    var familyid = family.FamilyId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.FamilyId == familyid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.FamilyId == familyid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl45Families.RemoveRange(familys);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.SuperfamilyId == superfamilyid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SuperfamilyId == superfamilyid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl42Superfamilies.Remove(superfamily);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Superfamily                       

    #region Family                

    public ObservableCollection<Tbl45Family> GetFamilyAsync()
    {
        var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
            .AsNoTracking()
            .OrderBy(a => a.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyAsync(int familyid)
    {
        var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
            .AsNoTracking()
            .Where(e => e.FamilyId == familyid)
            .OrderBy(k => k.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => e.FamilyId == id))
            : new ObservableCollection<Tbl45Family>(_context.Tbl45Families
                .Where(e => search != null && e.FamilyName.StartsWith(search))
                .OrderBy(a => a.FamilyName));
        return collection;
    }

    public ObservableCollection<Tbl45Family> GetFamilyFromSuperfamilyIdAsync(int superfamilyid)
    {
        var collection = new ObservableCollection<Tbl45Family>(_context.Tbl45Families
            .Where(e => e.SuperfamilyId == superfamilyid)
            .OrderBy(k => k.FamilyName)
        );

        return collection;
    }

    public async Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family family)
    {
        var current = await _context.Tbl45Families.FirstOrDefaultAsync(c => c.FamilyId == family.FamilyId);
        if (null == current)
        {
            family.CountId = RandomHelper.Randomnumber();
            family.Writer = Environment.UserName;
            family.WriterDate = DateTime.Now;
            family.Updater = Environment.UserName;
            family.UpdaterDate = DateTime.Now;
            _context.Tbl45Families.Add(family);
        }
        else
        {
            family.Updater = Environment.UserName;
            family.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(family);
        }
        await _context.SaveChangesAsync();
        return family;
    }

    public async Task DeleteFamilyAsync(int familyid)
    {
        var family = await _context.Tbl45Families.FirstOrDefaultAsync(b => b.FamilyId == familyid);
        if (family != null)
        {
            //necessary to delete before
            //------ Subfamily -------------
            var subfamilys = await _context.Tbl48Subfamilies.Where(a => a.FamilyId == familyid).ToListAsync();
            foreach (var subfamily in subfamilys)
            {
                for (var i = 0; i <= subfamilys.Count; i++)
                {
                    var subfamilyid = subfamily.SubfamilyId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SubfamilyId == subfamilyid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl48Subfamilies.RemoveRange(subfamilys);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.FamilyId == familyid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.FamilyId == familyid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl45Families.Remove(family);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Family                       

    #region Subfamily                

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync()
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
            .AsNoTracking()
            .OrderBy(a => a.SubfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(int subfamilyid)
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
            .AsNoTracking()
            .Where(e => e.SubfamilyId == subfamilyid)
            .OrderBy(k => k.SubfamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => e.SubfamilyId == id))
            : new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
                .Where(e => search != null && e.SubfamilyName.StartsWith(search))
                .OrderBy(a => a.SubfamilyName));
        return collection;
    }

    public ObservableCollection<Tbl48Subfamily> GetSubfamilyFromFamilyIdAsync(int familyid)
    {
        var collection = new ObservableCollection<Tbl48Subfamily>(_context.Tbl48Subfamilies
            .Where(e => e.FamilyId == familyid)
            .OrderBy(k => k.SubfamilyName)
        );

        return collection;
    }

    public async Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily subfamily)
    {
        var current = await _context.Tbl48Subfamilies.FirstOrDefaultAsync(c => c.SubfamilyId == subfamily.SubfamilyId);
        if (null == current)
        {
            subfamily.CountId = RandomHelper.Randomnumber();
            subfamily.Writer = Environment.UserName;
            subfamily.WriterDate = DateTime.Now;
            subfamily.Updater = Environment.UserName;
            subfamily.UpdaterDate = DateTime.Now;
            _context.Tbl48Subfamilies.Add(subfamily);
        }
        else
        {
            subfamily.Updater = Environment.UserName;
            subfamily.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(subfamily);
        }
        await _context.SaveChangesAsync();
        return subfamily;
    }

    public async Task DeleteSubfamilyAsync(int subfamilyid)
    {
        var subfamily = await _context.Tbl48Subfamilies.FirstOrDefaultAsync(b => b.SubfamilyId == subfamilyid);
        if (subfamily != null)
        {
            //necessary to delete before
            //------ Infrafamily -------------
            var infrafamilys = await _context.Tbl51Infrafamilies.Where(a => a.SubfamilyId == subfamilyid).ToListAsync();
            foreach (var infrafamily in infrafamilys)
            {
                for (var i = 0; i <= infrafamilys.Count; i++)
                {
                    var infrafamilyid = infrafamily.InfrafamilyId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.InfrafamilyId == infrafamilyid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl51Infrafamilies.RemoveRange(infrafamilys);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.SubfamilyId == subfamilyid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SubfamilyId == subfamilyid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl48Subfamilies.Remove(subfamily);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Subfamily                       

    #region Infrafamily                

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync()
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
            .AsNoTracking()
            .OrderBy(a => a.InfrafamilyName));
        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(int infrafamilyid)
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
            .AsNoTracking()
            .Where(e => e.InfrafamilyId == infrafamilyid)
            .OrderBy(k => k.InfrafamilyName)
        );

        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(string search)
    {
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => e.InfrafamilyId == id))
            : new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
                .Where(e => search != null && e.InfrafamilyName.StartsWith(search))
                .OrderBy(a => a.InfrafamilyName));
        return collection;
    }

    public ObservableCollection<Tbl51Infrafamily> GetInfrafamilyFromSubfamilyIdAsync(int subfamilyid)
    {
        var collection = new ObservableCollection<Tbl51Infrafamily>(_context.Tbl51Infrafamilies
            .Where(e => e.SubfamilyId == subfamilyid)
            .OrderBy(k => k.InfrafamilyName)
        );

        return collection;
    }

    public async Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily infrafamily)
    {
        var current = await _context.Tbl51Infrafamilies.FirstOrDefaultAsync(c => c.InfrafamilyId == infrafamily.InfrafamilyId);
        if (null == current)
        {
            infrafamily.CountId = RandomHelper.Randomnumber();
            infrafamily.Writer = Environment.UserName;
            infrafamily.WriterDate = DateTime.Now;
            infrafamily.Updater = Environment.UserName;
            infrafamily.UpdaterDate = DateTime.Now;
            _context.Tbl51Infrafamilies.Add(infrafamily);
        }
        else
        {
            infrafamily.Updater = Environment.UserName;
            infrafamily.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(infrafamily);
        }
        await _context.SaveChangesAsync();
        return infrafamily;
    }

    public async Task DeleteInfrafamilyAsync(int infrafamilyid)
    {
        var infrafamily = await _context.Tbl51Infrafamilies.FirstOrDefaultAsync(b => b.InfrafamilyId == infrafamilyid);
        if (infrafamily != null)
        {
            //necessary to delete before
            //------ Supertribus -------------
            var supertribuss = await _context.Tbl54Supertribusses.Where(a => a.InfrafamilyId == infrafamilyid).ToListAsync();
            foreach (var supertribus in supertribuss)
            {
                for (var i = 0; i <= supertribuss.Count; i++)
                {
                    var supertribusid = supertribus.SupertribusId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SupertribusId == supertribusid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SupertribusId == supertribusid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl54Supertribusses.RemoveRange(supertribuss);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.InfrafamilyId == infrafamilyid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.InfrafamilyId == infrafamilyid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl51Infrafamilies.Remove(infrafamily);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Infrafamily                       

    #region Supertribus                

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync()
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
            .AsNoTracking()
            .OrderBy(a => a.SupertribusName));
        return collection;
    }

    public ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(int supertribusid)
    {
        var collection = new ObservableCollection<Tbl54Supertribus>(_context.Tbl54Supertribusses
            .AsNoTracking()
            .Where(e => e.SupertribusId == supertribusid)
            .OrderBy(k => k.SupertribusName)
        );

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

    public async Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus supertribus)
    {
        var current = await _context.Tbl54Supertribusses.FirstOrDefaultAsync(c => c.SupertribusId == supertribus.SupertribusId);
        if (null == current)
        {
            supertribus.CountId = RandomHelper.Randomnumber();
            supertribus.Writer = Environment.UserName;
            supertribus.WriterDate = DateTime.Now;
            supertribus.Updater = Environment.UserName;
            supertribus.UpdaterDate = DateTime.Now;
            _context.Tbl54Supertribusses.Add(supertribus);
        }
        else
        {
            supertribus.Updater = Environment.UserName;
            supertribus.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(supertribus);
        }
        await _context.SaveChangesAsync();
        return supertribus;
    }

    public async Task DeleteSupertribusAsync(int supertribusid)
    {
        var supertribus = await _context.Tbl54Supertribusses.FirstOrDefaultAsync(b => b.SupertribusId == supertribusid);
        if (supertribus != null)
        {
            //necessary to delete before
            //------ Tribus -------------
            var tribuss = await _context.Tbl57Tribusses.Where(a => a.SupertribusId == supertribusid).ToListAsync();
            foreach (var tribus in tribuss)
            {
                for (var i = 0; i <= tribuss.Count; i++)
                {
                    var tribusid = tribus.TribusId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.TribusId == tribusid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.TribusId == tribusid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl57Tribusses.RemoveRange(tribuss);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.SupertribusId == supertribusid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SupertribusId == supertribusid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl54Supertribusses.Remove(supertribus);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Supertribus                       

    #region Tribus                

    public ObservableCollection<Tbl57Tribus> GetTribusAsync()
    {
        var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
            .AsNoTracking()
            .OrderBy(a => a.TribusName));
        return collection;
    }

    public ObservableCollection<Tbl57Tribus> GetTribusAsync(int tribusid)
    {
        var collection = new ObservableCollection<Tbl57Tribus>(_context.Tbl57Tribusses
            .AsNoTracking()
            .Where(e => e.TribusId == tribusid)
            .OrderBy(k => k.TribusName)
        );

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

    public async Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus tribus)
    {
        var current = await _context.Tbl57Tribusses.FirstOrDefaultAsync(c => c.TribusId == tribus.TribusId);
        if (null == current)
        {
            tribus.CountId = RandomHelper.Randomnumber();
            tribus.Writer = Environment.UserName;
            tribus.WriterDate = DateTime.Now;
            tribus.Updater = Environment.UserName;
            tribus.UpdaterDate = DateTime.Now;
            _context.Tbl57Tribusses.Add(tribus);
        }
        else
        {
            tribus.Updater = Environment.UserName;
            tribus.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(tribus);
        }
        await _context.SaveChangesAsync();
        return tribus;
    }

    public async Task DeleteTribusAsync(int tribusid)
    {
        var tribus = await _context.Tbl57Tribusses.FirstOrDefaultAsync(b => b.TribusId == tribusid);
        if (tribus != null)
        {
            //necessary to delete before
            //------ Subtribus -------------
            var subtribuss = await _context.Tbl60Subtribusses.Where(a => a.TribusId == tribusid).ToListAsync();
            foreach (var subtribus in subtribuss)
            {
                for (var i = 0; i <= subtribuss.Count; i++)
                {
                    var subtribusid = subtribus.SubtribusId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SubtribusId == subtribusid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SubtribusId == subtribusid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl60Subtribusses.RemoveRange(subtribuss);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.TribusId == tribusid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.TribusId == tribusid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl57Tribusses.Remove(tribus);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Tribus                       

    #region Subtribus                
    //neu
    public async Task<IEnumerable<Tbl60Subtribus>> GetAllSubtribusAsync()
    {
        return await _context.Tbl60Subtribusses
            .OrderBy(e => e.SubtribusName)
            .ToListAsync();
    }

    //----------------
    //alt
    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync()
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
            .AsNoTracking()
            .OrderBy(a => a.SubtribusName));
        return collection;
    }

    public ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(int subtribusid)
    {
        var collection = new ObservableCollection<Tbl60Subtribus>(_context.Tbl60Subtribusses
            .AsNoTracking()
            .Where(e => e.SubtribusId == subtribusid)
            .OrderBy(k => k.SubtribusName)
        );

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

    public async Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus subtribus)
    {
        var current = await _context.Tbl60Subtribusses.FirstOrDefaultAsync(c => c.SubtribusId == subtribus.SubtribusId);
        if (null == current)
        {
            subtribus.CountId = RandomHelper.Randomnumber();
            subtribus.Writer = Environment.UserName;
            subtribus.WriterDate = DateTime.Now;
            subtribus.Updater = Environment.UserName;
            subtribus.UpdaterDate = DateTime.Now;
            _context.Tbl60Subtribusses.Add(subtribus);
        }
        else
        {
            subtribus.Updater = Environment.UserName;
            subtribus.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(subtribus);
        }
        await _context.SaveChangesAsync();
        return subtribus;
    }

    public async Task DeleteSubtribusAsync(int subtribusid)
    {
        var subtribus = await _context.Tbl60Subtribusses.FirstOrDefaultAsync(b => b.SubtribusId == subtribusid);
        if (subtribus != null)
        {
            //necessary to delete before
            //------ Infratribus -------------
            var infratribuss = await _context.Tbl63Infratribusses.Where(a => a.SubtribusId == subtribusid).ToListAsync();
            foreach (var infratribus in infratribuss)
            {
                for (var i = 0; i <= infratribuss.Count; i++)
                {
                    var infratribusid = infratribus.InfratribusId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.InfratribusId == infratribusid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.InfratribusId == infratribusid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl63Infratribusses.RemoveRange(infratribuss);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.SubtribusId == subtribusid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SubtribusId == subtribusid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl60Subtribusses.Remove(subtribus);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Subtribus                       

    #region Infratribus
    public async Task<IEnumerable<Tbl63Infratribus>> GetAllInfratribusAsync()
    {
        return await _context.Tbl63Infratribusses
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }

    public async Task<Tbl63Infratribus?> GetInfratribusByIdAsync(int id)
    {
        return await _context.Tbl63Infratribusses
            .FirstOrDefaultAsync(x => x.InfratribusId == id);
    }


    public async Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl63Infratribus>();

        return await _context.Tbl63Infratribusses
            .Where(e =>
                e.InfratribusName.StartsWith(search) ||
                e.InfratribusId.ToString() == search)
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();
    }


    public async Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus model)
    {
        bool isNew = model.InfratribusId == 0;

        ApplyAuditInfo(model, isNew);

        if (isNew)
            _context.Tbl63Infratribusses.Add(model);
        else
            _context.Tbl63Infratribusses.Update(model);

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task DeleteInfratribusAsync(int id)
    {
        var entity = await _context.Tbl63Infratribusses.FindAsync(id);
        if (entity != null)
        {
            _context.Tbl63Infratribusses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> InfratribusExistsAsync(Tbl63Infratribus model)
    {
        return await _context.Tbl63Infratribusses.AnyAsync(c =>
            c.InfratribusName == model.InfratribusName &&
            c.Synonym == model.Synonym &&
            c.Author == model.Author &&
            c.AuthorYear == model.AuthorYear &&
            c.EngName == model.EngName &&
            c.GerName == model.GerName &&
            c.FraName == model.FraName &&
            c.PorName == model.PorName &&
            c.Valid == model.Valid &&
            c.ValidYear == model.ValidYear &&
            c.Info == model.Info &&
            c.Memo == model.Memo
        );
    }

    public async Task<ObservableCollection<Tbl63Infratribus>> GetInfratribusFromSubtribusIdAsync(int subtribusId)
    {
        var list = await _context.Tbl63Infratribusses
            .Where(e => e.SubtribusId == subtribusId)
            .OrderBy(e => e.InfratribusName)
            .ToListAsync();

        return new ObservableCollection<Tbl63Infratribus>(list);
    }

    public async Task<IEnumerable<Tbl66Genus>> GetGenusForInfratribusAsync(int infratribusId)
    {
        return await _context.Tbl66Genusses
            .Where(e => e.InfratribusId == infratribusId)
            .OrderBy(e => e.GenusName)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl90Reference>> GetReferencesForInfratribusAsync(int infratribusId)
    {
        return await _context.Tbl90References
            .Where(e => e.InfratribusId == infratribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<IEnumerable<Tbl93Comment>> GetCommentsForInfratribusAsync(int infratribusId)
    {
        return await _context.Tbl93Comments
            .Where(e => e.InfratribusId == infratribusId)
            .OrderBy(e => e.Info)
            .ToListAsync();
    }
    public async Task<bool> HasGenusAsync(int infratribusId)
    {
        return await _context.Tbl66Genusses
            .AnyAsync(g => g.InfratribusId == infratribusId);
    }

    public async Task DeleteCommentsAsync(int infratribusId)
    {
        var comments = _context.Tbl93Comments
            .Where(c => c.InfratribusId == infratribusId);

        _context.Tbl93Comments.RemoveRange(comments);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReferencesAsync(int infratribusId)
    {
        var refs = _context.Tbl90References
            .Where(r => r.InfratribusId == infratribusId);

        _context.Tbl90References.RemoveRange(refs);
        await _context.SaveChangesAsync();
    }

    //-------------------------------------------------------
    public ObservableCollection<Tbl63Infratribus> GetInfratribusAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
            .OrderBy(k => k.InfratribusName));
        return collection;
    }


    //public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync()
    //{
    //    var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
    //        .AsNoTracking()
    //        .OrderBy(a => a.InfratribusName));
    //    return collection;
    //}

    //public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(int infratribusid)
    //{
    //    var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
    //        .AsNoTracking()
    //        .Where(e => e.InfratribusId == infratribusid)
    //        .OrderBy(k => k.InfratribusName)
    //    );

    //    return collection;
    //}

    //public ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(string search)
    //{
    //    var collection = int.TryParse(search, out var id)
    //        ? new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
    //            .Where(e => e.InfratribusId == id))
    //        : new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
    //            .Where(e => search != null && e.InfratribusName.StartsWith(search))
    //            .OrderBy(a => a.InfratribusName));
    //    return collection;
    //}

    //public ObservableCollection<Tbl63Infratribus> GetInfratribusFromSubtribusIdAsync(int subtribusid)
    //{
    //    var collection = new ObservableCollection<Tbl63Infratribus>(_context.Tbl63Infratribusses
    //        .Where(e => e.SubtribusId == subtribusid)
    //        .OrderBy(k => k.InfratribusName)
    //    );

    //    return collection;
    //}

    //public async Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus infratribus)
    //{
    //    var current = await _context.Tbl63Infratribusses.FirstOrDefaultAsync(c => c.InfratribusId == infratribus.InfratribusId);
    //    if (null == current)
    //    {
    //        infratribus.CountId = RandomHelper.Randomnumber();
    //        infratribus.Writer = Environment.UserName;
    //        infratribus.WriterDate = DateTime.Now;
    //        infratribus.Updater = Environment.UserName;
    //        infratribus.UpdaterDate = DateTime.Now;
    //        _context.Tbl63Infratribusses.Add(infratribus);
    //    }
    //    else
    //    {
    //        infratribus.Updater = Environment.UserName;
    //        infratribus.UpdaterDate = DateTime.Now;
    //        _context.Entry(current).CurrentValues.SetValues(infratribus);
    //    }
    //    await _context.SaveChangesAsync();
    //    return infratribus;
    //}

    //public async Task DeleteInfratribusAsync(int infratribusid)
    //{
    //    var infratribus = await _context.Tbl63Infratribusses.FirstOrDefaultAsync(b => b.InfratribusId == infratribusid);
    //    if (infratribus != null)
    //    {
    //        //necessary to delete before
    //        //------ Genus -------------
    //        var genuss = await _context.Tbl66Genusses.Where(a => a.InfratribusId == infratribusid).ToListAsync();
    //        foreach (var genus in genuss)
    //        {
    //            for (var i = 0; i <= genuss.Count; i++)
    //            {
    //                var genusid = genus.GenusId;
    //                var singleref = _context.Tbl90References.SingleOrDefault(a => a.GenusId == genusid);
    //                if (singleref != null)
    //                {
    //                    _context.Tbl90References.Remove(singleref);
    //                    await _context.SaveChangesAsync();
    //                }
    //                var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.GenusId == genusid);
    //                if (singlecom != null)
    //                {
    //                    _context.Tbl93Comments.Remove(singlecom);
    //                    await _context.SaveChangesAsync();
    //                }
    //            }
    //        }
    //        _context.Tbl66Genusses.RemoveRange(genuss);
    //        await _context.SaveChangesAsync();
    //        //---------------------------------------------------------
    //        var references = await _context.Tbl90References.Where(reference => reference.InfratribusId == infratribusid).ToListAsync();
    //        _context.Tbl90References.RemoveRange(references);
    //        await _context.SaveChangesAsync();

    //        var comments = await _context.Tbl93Comments.Where(comment => comment.InfratribusId == infratribusid).ToListAsync();
    //        _context.Tbl93Comments.RemoveRange(comments);
    //        await _context.SaveChangesAsync();

    //        _context.Tbl63Infratribusses.Remove(infratribus);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    #endregion Infratribus                       

    #region Genus                

    public ObservableCollection<Tbl66Genus> GetGenusAsync()
    {
        var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
            .AsNoTracking()
            .OrderBy(a => a.GenusName));
        return collection;
    }

    public ObservableCollection<Tbl66Genus> GetGenusAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl66Genus>(_context.Tbl66Genusses
            .AsNoTracking()
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.GenusName)
        );

        return collection;
    }

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

    public async Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus genus)
    {
        var current = await _context.Tbl66Genusses.FirstOrDefaultAsync(c => c.GenusId == genus.GenusId);
        if (null == current)
        {
            genus.CountId = RandomHelper.Randomnumber();
            genus.Writer = Environment.UserName;
            genus.WriterDate = DateTime.Now;
            genus.Updater = Environment.UserName;
            genus.UpdaterDate = DateTime.Now;
            _context.Tbl66Genusses.Add(genus);
        }
        else
        {
            genus.Updater = Environment.UserName;
            genus.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(genus);
        }
        await _context.SaveChangesAsync();
        return genus;
    }

    public async Task DeleteGenusAsync(int genusid)
    {
        var genus = await _context.Tbl66Genusses.FirstOrDefaultAsync(b => b.GenusId == genusid);
        if (genus != null)
        {
            //necessary to delete before
            //------ Species -------------
            var speciess = await _context.Tbl75Speciesses.Where(a => a.GenusId == genusid).ToListAsync();
            foreach (var species in speciess)
            {
                for (var i = 0; i <= speciess.Count; i++)
                {
                    var speciesid = species.SpeciesId;
                    var singleref = _context.Tbl90References.SingleOrDefault(a => a.SpeciesId == speciesid);
                    if (singleref != null)
                    {
                        _context.Tbl90References.Remove(singleref);
                        await _context.SaveChangesAsync();
                    }
                    var singlecom = _context.Tbl93Comments.SingleOrDefault(a => a.SpeciesId == speciesid);
                    if (singlecom != null)
                    {
                        _context.Tbl93Comments.Remove(singlecom);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            _context.Tbl75Speciesses.RemoveRange(speciess);
            await _context.SaveChangesAsync();
            //---------------------------------------------------------
            var references = await _context.Tbl90References.Where(reference => reference.GenusId == genusid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.GenusId == genusid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl66Genusses.Remove(genus);
            await _context.SaveChangesAsync();
        }
    }

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

    public ObservableCollection<Tbl75Species> GetSpeciesAsync()
    {
        //var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
        //    .AsNoTracking()
        //    .OrderBy(a => a.SpeciesName));
        //return collection;

        var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
            .OrderBy(a => a.Tbl66Genusses.GenusName)
            .ThenBy(a => a.SpeciesName)
            .ThenBy(a => a.Subspecies)
            .ThenBy(a => a.Divers));
        return collection;
    }
    public ObservableCollection<Tbl75Species> GetSpeciesAsync(int speciesid)
    {
        var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
            .AsNoTracking()
            .Where(e => e.SpeciesId == speciesid)
            .OrderBy(a => a.Tbl66Genusses.GenusName)
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
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers));
        return collection;
    }
    public ObservableCollection<Tbl75Species> GetSpeciesFromGenusNameAsync(string search)
    {
        //search about GenusName
        var collection = int.TryParse(search, out var id)
            ? new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => e.SpeciesId == id))
            : new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
                .Where(e => e.Tbl66Genusses.GenusName.StartsWith(search))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
                .ThenBy(a => a.SpeciesName)
                .ThenBy(a => a.Subspecies)
                .ThenBy(a => a.Divers)
            );
        return collection;
    }
    public ObservableCollection<Tbl75Species> GetSpeciesFromGenusIdAsync(int genusid)
    {
        var collection = new ObservableCollection<Tbl75Species>(_context.Tbl75Speciesses
            .Where(e => e.GenusId == genusid)
            .OrderBy(k => k.SpeciesName)
        );

        return collection;
    }
    public async Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species species)
    {
        var current = await _context.Tbl75Speciesses.FirstOrDefaultAsync(c => c.SpeciesId == species.SpeciesId);
        if (null == current)
        {
            species.CountId = RandomHelper.Randomnumber();
            species.Writer = Environment.UserName;
            species.WriterDate = DateTime.Now;
            species.Updater = Environment.UserName;
            species.UpdaterDate = DateTime.Now;
            _context.Tbl75Speciesses.Add(species);
        }
        else
        {
            species.Updater = Environment.UserName;
            species.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(species);
        }
        await _context.SaveChangesAsync();
        return species;
    }
    public async Task DeleteSpeciesAsync(int speciesid)
    {
        var species = await _context.Tbl75Speciesses.FirstOrDefaultAsync(b => b.SpeciesId == speciesid);
        if (species != null)
        {
            //necessary to delete before
            var references = await _context.Tbl90References.Where(reference => reference.SpeciesId == speciesid).ToListAsync();
            _context.Tbl90References.RemoveRange(references);
            await _context.SaveChangesAsync();

            var comments = await _context.Tbl93Comments.Where(comment => comment.SpeciesId == speciesid).ToListAsync();
            _context.Tbl93Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            _context.Tbl75Speciesses.Remove(species);
            await _context.SaveChangesAsync();
        }
    }

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
    public async Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference reference)
    {
        var current = await _context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == reference.ReferenceId);
        if (null == current)
        {
            reference.CountId = RandomHelper.Randomnumber();
            reference.Writer = Environment.UserName;
            reference.WriterDate = DateTime.Now;
            reference.Updater = Environment.UserName;
            reference.UpdaterDate = DateTime.Now;
            _context.Tbl90References.Add(reference);
        }
        else
        {
            reference.Updater = Environment.UserName;
            reference.UpdaterDate = DateTime.Now;
            _context.Entry(current).CurrentValues.SetValues(reference);
        }
        await _context.SaveChangesAsync();
        return reference;
    }
    public async Task DeleteReferenceAsync(int referenceid)
    {
        var reference = await _context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == referenceid);
        if (null != reference)
        {
            _context.Tbl90References.Remove(reference);
            await _context.SaveChangesAsync();
        }
    }

    #endregion Reference

    #region RefAuthor

    public async Task<IEnumerable<Tbl90RefAuthor>> SearchRefAuthorAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefAuthor>();

        return await _context.Tbl90RefAuthors
            .Where(e =>
                e.RefAuthorName.StartsWith(search) ||
                e.RefAuthorId.ToString() == search)
            .OrderBy(e => e.RefAuthorName)
            .ToListAsync();
    }

    public async Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor model)
    {
        if (model.RefAuthorId == 0)
        {
            _context.Tbl90RefAuthors.Add(model);
        }
        else
        {
            _context.Tbl90RefAuthors.Update(model);
        }

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task DeleteRefAuthorAsync(int id)
    {
        var entity = await _context.Tbl90RefAuthors.FindAsync(id);
        if (entity != null)
        {
            _context.Tbl90RefAuthors.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> RefAuthorExistsAsync(Tbl90RefAuthor model)
    {
        return await _context.Tbl90RefAuthors.AnyAsync(c =>
            c.RefAuthorName == model.RefAuthorName &&
            c.ArticelTitle == model.ArticelTitle &&
            c.BookName == model.BookName &&
            c.Publisher == model.Publisher &&
            c.PublicationPlace == model.PublicationPlace &&
            c.Valid == model.Valid &&
            c.ValidYear == model.ValidYear &&
            c.Page == model.Page &&
            c.Page1 == model.Page1 &&
            c.Info == model.Info &&
            c.Notes == model.Notes &&
            c.Memo == model.Memo
        );
    }

    public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
            .OrderBy(k => k.RefAuthorName));
        return collection;
    }


    //public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync()
    //{
    //    var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
    //        .AsNoTracking()
    //        .OrderBy(k => k.RefAuthorName)
    //        .ThenBy(a => a.BookName)
    //        .ThenBy(a => a.Page1));
    //    //var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
    //    //    .AsNoTracking()
    //    //    .OrderBy(x => x.RefAuthorName)
    //    //    .ThenBy(y => y.ArticelTitle)
    //    //    .ThenBy(y => y.BookName)
    //    //    .ThenBy(y => y.Page1)
    //    //    .ThenBy(y => y.Publisher)
    //    //    .ThenBy(y => y.PublicationPlace));

    //    return collection;
    //}

    //public ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync(int refauthorid)
    //{
    //    var collection = new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
    //        .Where(e => e.RefAuthorId == refauthorid));

    //    return collection;
    //}

    //public async Task<ObservableCollection<Tbl90RefAuthor>> GetRefAuthorAsync(string search)
    //{
    //    var collection = int.TryParse(search, out var id)
    //        ? new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
    //            .Where(e => e.RefAuthorId == id))
    //        : new ObservableCollection<Tbl90RefAuthor>(_context.Tbl90RefAuthors
    //            .Where(e => e.RefAuthorName.StartsWith(search))
    //            .OrderBy(x => x.RefAuthorName)
    //            .ThenBy(y => y.ArticelTitle)
    //            .ThenBy(y => y.BookName)
    //            .ThenBy(y => y.Page1)
    //            .ThenBy(y => y.Publisher)
    //            .ThenBy(y => y.PublicationPlace));

    //    await Task.CompletedTask;
    //    return collection;
    //}

    //public async Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor refauthor)
    //{
    //    var current = await _context.Tbl90RefAuthors.FirstOrDefaultAsync(_comment => _comment.RefAuthorId == refauthor.RefAuthorId);
    //    if (null == current)
    //    {
    //        if (refauthor.Valid != null)
    //        {
    //        }
    //        else
    //        {
    //            refauthor.Valid = false; //Valid cannot be null. eingefügt 06.01.2025 Terppe
    //        }

    //        refauthor.CountId = RandomHelper.Randomnumber();
    //        refauthor.Writer = Environment.UserName;
    //        refauthor.WriterDate = DateTime.Now;
    //        refauthor.Updater = Environment.UserName;
    //        refauthor.UpdaterDate = DateTime.Now;
    //        _context.Tbl90RefAuthors.Add(refauthor);
    //    }
    //    else
    //    {
    //        _context.Entry(current).CurrentValues.SetValues(refauthor);
    //    }
    //    await _context.SaveChangesAsync();
    //    return refauthor;
    //}


    //public async Task DeleteRefAuthorAsync(int refauthorid)
    //{
    //    var reference = await _context.Tbl90References.FirstOrDefaultAsync(_reference => _reference.ReferenceId == refauthorid);
    //    if (null != reference)
    //    {
    //        _context.Tbl90References.Remove(reference);
    //        await _context.SaveChangesAsync();
    //    }
    //}

    #endregion RefAuthor

    #region RefSource

    public async Task<IEnumerable<Tbl90RefSource>> SearchRefSourceAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefSource>();

        return await _context.Tbl90RefSources
            .Where(e =>
                e.RefSourceName.StartsWith(search) ||
                e.RefSourceId.ToString() == search)
            .OrderBy(e => e.RefSourceName)
            .ToListAsync();
    }

    public async Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource model)
    {
        if (model.RefSourceId == 0)
        {
            _context.Tbl90RefSources.Add(model);
        }
        else
        {
            _context.Tbl90RefSources.Update(model);
        }

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task DeleteRefSourceAsync(int id)
    {
        var entity = await _context.Tbl90RefSources.FindAsync(id);
        if (entity != null)
        {
            _context.Tbl90RefSources.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> RefSourceExistsAsync(Tbl90RefSource model)
    {
        return await _context.Tbl90RefSources.AnyAsync(c =>
            c.RefSourceName == model.RefSourceName &&
            c.SourceYear == model.SourceYear &&
            c.Valid == model.Valid &&
            c.ValidYear == model.ValidYear &&
            c.Info == model.Info &&
            c.Notes == model.Notes &&
            c.Memo == model.Memo &&
            c.Author == model.Author
        );
    }

    public ObservableCollection<Tbl90RefSource> GetRefSourceAsyncAlt()
    {
        var collection = new ObservableCollection<Tbl90RefSource>(_context.Tbl90RefSources
            .OrderBy(k => k.RefSourceName));
        return collection;
    }

    #endregion

    #region RefExpert

    public async Task<IEnumerable<Tbl90RefExpert>> SearchRefExpertAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return Enumerable.Empty<Tbl90RefExpert>();

        return await _context.Tbl90RefExperts
            .Where(e => e.RefExpertName.StartsWith(search) || e.RefExpertId.ToString() == search)
            .OrderBy(e => e.RefExpertName)
            .ToListAsync();
    }
    public async Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert model)
    {
        if (model.RefExpertId == 0)
        {
            _context.Tbl90RefExperts.Add(model);
        }
        else
        {
            _context.Tbl90RefExperts.Update(model);
        }

        await _context.SaveChangesAsync();
        return model;
    }
    public async Task DeleteRefExpertAsync(int id)
    {
        var entity = await _context.Tbl90RefExperts.FindAsync(id);
        if (entity != null)
        {
            _context.Tbl90RefExperts.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> RefExpertExistsAsync(Tbl90RefExpert model)
    {
        return await _context.Tbl90RefExperts.AnyAsync(c =>
            c.RefExpertName == model.RefExpertName &&
            c.Valid == model.Valid &&
            c.ValidYear == model.ValidYear &&
            c.Info == model.Info &&
            c.Notes == model.Notes &&
            c.Memo == model.Memo
        );
    }

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
    public async Task<IEnumerable<Tbl93Comment>> GetAllCommentAsync()
    {
        return await _context.Tbl93Comments
            .OrderBy(e => e.Info)
            .ToListAsync();
    }

    public async Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment model)
    {
        bool isNew = model.CommentId == 0;

        ApplyAuditInfo(model, isNew);

        if (isNew)
            _context.Tbl93Comments.Add(model);
        else
            _context.Tbl93Comments.Update(model);

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task DeleteCommentAsync(int id)
    {
        var entity = await _context.Tbl93Comments.FindAsync(id);
        if (entity != null)
        {
            _context.Tbl93Comments.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

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
                .Include(a => a.Tbl66Genusses)
                .Where(e => e.SpeciesName.StartsWith(filterText) &&
                            e.SpeciesName.Contains(Value) == false ||
                            e.Tbl66Genusses.GenusName.Contains(filterText) ||
                            e.Subspecies.Contains(filterText) ||
                            e.Divers.Contains(filterText) ||
                            e.TradeName.Contains(filterText) ||
                            e.Author.Contains(filterText)
                )
                .OrderBy(a => a.Tbl66Genusses.GenusName)
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
                .Include(a => a.Tbl75Speciesses)
                .Where(e => e.NameName.StartsWith(filterText) &&
                            e.Tbl75Speciesses.SpeciesId == e.SpeciesId
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
                .Include(a => a.Tbl75Speciesses)
                .Where(e => e.SynonymName.StartsWith(filterText) &&
                            e.Tbl75Speciesses.SpeciesId == e.SpeciesId
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
        if (_context.Tbl57Tribusses != null)
        {
            var dbset = _context.Tbl57Tribusses
                .SingleOrDefault(p => p.TribusId == id);

            if (dbset != null)
            {
                return dbset.SupertribusId;
            }
        }

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
                return dbset.TribusId;
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
                return dbset.InfratribusId;
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
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == genusId &&
                            e.SpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
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
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.SpeciesName == name &&
                            e.Subspecies != null &&
                            e.Subspecies != string.Empty)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
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
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.GenusId == id &&
                            e.SpeciesName == name &&
                            (e.Subspecies == null || e.Subspecies == string.Empty) &&
                            (e.Divers == null || e.Divers == string.Empty))
                .OrderBy(a => a.Tbl66Genusses.GenusName)
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
                .Include(d => d.Tbl66Genusses)
                .Where(e => e.SpeciesName == speciesName && e.Subspecies == subspecies && e.Divers == divers)
                .OrderBy(a => a.Tbl66Genusses.GenusName)
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
