using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Search;
using static ATIS.Views.Report.PdfModels;

namespace ATIS.Interfaces;

public interface IDataService
{
    public interface IAuditable
    {
        string Writer { get; set; }
        DateTime WriterDate { get; set; }
        string Updater { get; set; }
        DateTime UpdaterDate { get; set; }
        int CountId { get; set; }
    }



    #region für provisorium

    IQueryable<Tbl03Regnum> Tbl03Regnums { get; }
    IQueryable<Tbl06Phylum> Tbl06Phylums { get; }
    IQueryable<Tbl12Subphylum> Tbl12Subphylums { get; }
    IQueryable<Tbl18Superclass> Tbl18Superclasses { get; }
    IQueryable<Tbl21Class> Tbl21Classes { get; }
    IQueryable<Tbl24Subclass> Tbl24Subclasses { get; }
    IQueryable<Tbl27Infraclass> Tbl27Infraclasses { get; }
    IQueryable<Tbl30Legio> Tbl30Legios { get; }
    IQueryable<Tbl33Ordo> Tbl33Ordos { get; }
    IQueryable<Tbl36Subordo> Tbl36Subordos { get; }
    IQueryable<Tbl39Infraordo> Tbl39Infraordos { get; }
    IQueryable<Tbl42Superfamily> Tbl42Superfamilies { get; }
    IQueryable<Tbl45Family> Tbl45Families { get; }
    IQueryable<Tbl48Subfamily> Tbl48Subfamilies { get; }
    IQueryable<Tbl51Infrafamily> Tbl51Infrafamilies { get; }
    IQueryable<Tbl54Supertribus> Tbl54Supertribusses { get; }
    IQueryable<Tbl57Tribus> Tbl57Tribusses { get; }
    IQueryable<Tbl60Subtribus> Tbl60Subtribusses { get; }
    IQueryable<Tbl63Infratribus> Tbl63Infratribusses { get; }
    IQueryable<Tbl66Genus> Tbl66Genusses { get; }
    IQueryable<Tbl68Speciesgroup> Tbl68Speciesgroups { get; }
    IQueryable<Tbl75Species> Tbl75Speciesses { get; }
    IQueryable<Tbl78Name> Tbl78Names { get; }
    IQueryable<Tbl81Image> Tbl81Images { get; }
    IQueryable<Tbl84Synonym> Tbl84Synonyms { get; }
    IQueryable<Tbl87Geographic> Tbl87Geographics { get; }




    IQueryable<Tbl90Reference> Tbl90References { get; }
    IQueryable<Tbl90RefAuthor> Tbl90RefAuthors { get; }
    IQueryable<Tbl90RefExpert> Tbl90RefExperts { get; }
    IQueryable<Tbl90RefSource> Tbl90RefSources { get; }
    IQueryable<Tbl93Comment> Tbl93Comments { get; }

    IQueryable<TblCountry> TblCountries { get; }
    IQueryable<TblDisease> TblDiseases { get; }
    IQueryable<TblFood> TblFoods { get; }
    IQueryable<TblMiscellaneous> TblMiscellaneousses { get; }
    IQueryable<TblUserProfile> TblUserProfiles { get; }

    #endregion für provisorium

    #region Von App.cs verschoben

    TblFood? SingleFood(int id);
    TblDisease? SingleDisease(int id);
    TblMiscellaneous? SingleMiscellaneous(int id);
    //-----------------------------
    Tbl78Name? SingleName(int id);
    Tbl81Image? SingleImage(int id);
    Tbl84Synonym? SingleSynonym(int id);
    Tbl87Geographic? SingleGeographic(int id);
    //-------------------------------------------------------------------------------
    TblCountry? SingleCountry(int id);
    TblUserProfile? SingleUserProfile(int id);
    //--------------------------------------------------------------------
    Tbl90RefAuthor? SingleRefAuthor(int id);
    Tbl90RefExpert? SingleRefExpert(int id);
    Tbl90RefSource? SingleRefSource(int id);
    Tbl90Reference? SingleReference(int id);
    Tbl93Comment? SingleComment(int id);
    //--------------------------------------------------------------------
    Tbl81Image? SingleImageFood(int imageId, int? foodId);
    Tbl81Image? SingleImageDisease(int imageId, int? diseaseId);
    Tbl81Image? SingleImageMiscellaneous(int imageId, int? miscellaneousId);
    //------------------------------------------------------------------
    Tbl75Species? SingleSpecies(int id);
    Tbl90Reference? SingleReferenceSpecies(int referenceid, int? speciesId);
    Tbl93Comment? SingleCommentSpecies(int commentid, int? speciesid);
    Tbl78Name? SingleNameSpecies(int nameId, int? speciesId);
    Tbl81Image? SingleImageSpecies(int imageId, int? speciesId);
    Tbl84Synonym? SingleSynonymSpecies(int synonymId, int? speciesId);
    Tbl87Geographic? SingleGeographicSpecies(int geographicId, int? speciesId);
    Tbl75Species? SingleSpeciesGenus(int speciesid, int? genusid);
    Tbl78Name? SingleNameSpecies(int nameid, int speciesid);

    //------------------------------------------------------------------
    Tbl68Speciesgroup? SingleSpeciesgroup(int id);
    Tbl75Species? SingleSpeciesSpeciesgroup(int speciesId, int? speciesgroupId);
    //------------------------------------------------------------------

    Tbl66Genus? SingleGenus(int id);
    Tbl90Reference? SingleReferenceGenus(int referenceid, int? genusId);
    Tbl93Comment? SingleCommentGenus(int commentid, int? genusid);
    //---------------------------------------------------
    Tbl63Infratribus? SingleInfratribus(int id);
    Tbl90Reference? SingleReferenceInfratribus(int referenceid, int? infratribusId);
    Tbl93Comment? SingleCommentInfratribus(int commentid, int? infratribusid);

    Tbl66Genus? SingleGenusInfratribus(int genusId, int? infratribusId);
    //------------------------------------------------------------------

    Tbl60Subtribus? SingleSubtribus(int id);
    Tbl90Reference? SingleReferenceSubtribus(int referenceid, int? subtribusId);
    Tbl93Comment? SingleCommentSubtribus(int commentid, int? subtribusid);

    Tbl63Infratribus? SingleInfratribusSubtribus(int infratribusId, int? subtribusId);
    //---------------------------------------------------
    Tbl57Tribus? SingleTribus(int id);
    Tbl90Reference? SingleReferenceTribus(int referenceid, int? tribusId);
    Tbl93Comment? SingleCommentTribus(int commentid, int? tribusid);
    Tbl60Subtribus? SingleSubtribusTribus(int subtribusId, int? tribusId);
    //------------------------------------------------------------------
    Tbl54Supertribus? SingleSupertribus(int id);
    Tbl90Reference? SingleReferenceSupertribus(int referenceid, int? supertribusId);
    Tbl93Comment? SingleCommentSupertribus(int commentid, int? supertribusid);
    Tbl57Tribus? SingleTribusSupertribus(int tribusId, int? supertribusId);
    //------------------------------------------------------------------
    Tbl51Infrafamily? SingleInfrafamily(int id);
    Tbl90Reference? SingleReferenceInfrafamily(int referenceid, int? infrafamilyId);
    Tbl93Comment? SingleCommentInfrafamily(int commentid, int? infrafamilyid);
    Tbl54Supertribus? SingleSupertribusInfrafamily(int supertribusId, int? infrafamilyId);
    //------------------------------------------------------------------
    Tbl48Subfamily? SingleSubfamily(int id);
    Tbl90Reference? SingleReferenceSubfamily(int referenceid, int? subfamilyId);
    Tbl93Comment? SingleCommentSubfamily(int commentid, int? subfamilyid);
    Tbl51Infrafamily? SingleInfrafamilySubfamily(int infrafamilyId, int? subfamilyId);
    //------------------------------------------------------------------
    Tbl45Family? SingleFamily(int id);
    Tbl90Reference? SingleReferenceFamily(int referenceid, int? familyId);
    Tbl93Comment? SingleCommentFamily(int commentid, int? familyid);
    Tbl48Subfamily? SingleSubfamilyFamily(int subfamilyId, int? familyId);
    ////------------------------------------------------------------------
    Tbl42Superfamily? SingleSuperfamily(int id);
    Tbl90Reference? SingleReferenceSuperfamily(int referenceid, int? superfamilyId);
    Tbl93Comment? SingleCommentSuperfamily(int commentid, int? superfamilyid);
    Tbl45Family? SingleFamilySuperfamily(int familyId, int? superfamilyId);
    //------------------------------------------------------------------
    Tbl39Infraordo? SingleInfraordo(int id);
    Tbl90Reference? SingleReferenceInfraordo(int referenceid, int? infraordoId);
    Tbl93Comment? SingleCommentInfraordo(int commentid, int? infraordoid);
    Tbl42Superfamily? SingleSuperfamilyInfraordo(int superfamilyId, int? infraordoId);
    //------------------------------------------------------------------
    Tbl36Subordo? SingleSubordo(int id);
    Tbl90Reference? SingleReferenceSubordo(int referenceid, int? subordoId);
    Tbl93Comment? SingleCommentSubordo(int commentid, int? subordoid);
    Tbl39Infraordo? SingleInfraordoSubordo(int infraordoId, int? subordoId);
    //}
    //------------------------------------------------------------------
    Tbl33Ordo? SingleOrdo(int id);
    Tbl90Reference? SingleReferenceOrdo(int referenceid, int? ordoId);
    Tbl93Comment? SingleCommentOrdo(int commentid, int? ordoid);
    Tbl36Subordo? SingleSubordoOrdo(int subordoId, int? ordoId);
    //------------------------------------------------------------------
    Tbl30Legio? SingleLegio(int id);
    Tbl90Reference? SingleReferenceLegio(int referenceid, int? legioId);
    Tbl93Comment? SingleCommentLegio(int commentid, int? legioid);
    Tbl33Ordo? SingleOrdoLegio(int ordoId, int? legioId);
    //------------------------------------------------------------------
    Tbl27Infraclass? SingleInfraclass(int id);
    Tbl90Reference? SingleReferenceInfraclass(int referenceid, int? infraclassId);
    Tbl93Comment? SingleCommentInfraclass(int commentid, int? infraclassid);
    Tbl30Legio? SingleLegioInfraclass(int legioId, int? infraclassId);
    //------------------------------------------------------------------
    Tbl24Subclass? SingleSubclass(int id);
    Tbl90Reference? SingleReferenceSubclass(int referenceid, int? subclassId);
    Tbl93Comment? SingleCommentSubclass(int commentid, int? subclassid);
    Tbl27Infraclass? SingleInfraclassSubclass(int infraclassId, int? subclassId);
    //------------------------------------------------------------------
    Tbl21Class? SingleClass(int id);
    Tbl90Reference? SingleReferenceClass(int referenceid, int? classId);
    Tbl93Comment? SingleCommentClass(int commentid, int? classid);
    Tbl24Subclass? SingleSubclassClass(int subclassId, int? classId);
    //------------------------------------------------------------------
    Tbl18Superclass? SingleSuperclass(int id);
    Tbl90Reference? SingleReferenceSuperclass(int referenceid, int? superclassId);
    Tbl93Comment? SingleCommentSuperclass(int commentid, int? superclassid);
    Tbl21Class? SingleClassSuperclass(int classeId, int? superclassId);
    //------------------------------------------------------------------
    Tbl12Subphylum? SingleSubphylum(int id);
    Tbl90Reference? SingleReferenceSubphylum(int referenceid, int? subphylumId);
    Tbl93Comment? SingleCommentSubphylum(int commentid, int? subphylumid);
    Tbl18Superclass? SingleSuperclassSubphylum(int superclassId, int? subphylumId);
    //------------------------------------------------------------------
    Tbl06Phylum? SinglePhylum(int id);
    Tbl90Reference? SingleReferencePhylum(int referenceid, int? phylumId);
    Tbl93Comment? SingleCommentPhylum(int commentid, int? phylumid);
    Tbl12Subphylum? SingleSubphylumPhylum(int subphylumId, int? phylumId);
    //------------------------------------------------------------------
    Tbl03Regnum? SingleRegnum(int id);
    Tbl90Reference? SingleReferenceRegnum(int referenceid, int? regnumId);
    Tbl93Comment? SingleCommentRegnum(int commentid, int? regnumid);
    Tbl06Phylum? SinglePhylumRegnum(int phylumId, int? regnumId);



    #endregion Von App.cs verschoben

    #region CRUD

    #region Regnum

    // ---------------------------------------------------------                  
    // Regnum 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl03Regnum>> GetAllRegnumAsync();
    Task<IEnumerable<Tbl03Regnum>> SearchRegnumAsync(string? search);
    Task<Tbl03Regnum> GetRegnumByIdAsync(int id);
    Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum entity);
    Task DeleteRegnumAsync(int id);
    Task<bool> RegnumExistsAsync(Tbl03Regnum entity);

    Task<bool> RegnumHasPhylumAsync(int regnumId);
    Task<bool> RegnumHasReferenceAsync(int regnumId);
    Task<bool> RegnumHasCommentAsync(int sregnumId);

    //Für ReportPDF
    Task<IEnumerable<Tbl06Phylum>> GetPhylumsByRegnumIdAsync(int regnumId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByRegnumIdAsync(int regnumId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByRegnumIdAsync(int regnumId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByRegnumIdAsync(int regnumId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByRegnumIdAsync(int regnumId);

    //alt-------------------------------------
    ObservableCollection<Tbl03Regnum> GetRegnumAsync();
    ObservableCollection<Tbl03Regnum> GetRegnumAsync(int regnumid);
    ObservableCollection<Tbl03Regnum> GetRegnumAsync(string search);

    #endregion Regnum

    #region Phylum
    // ---------------------------------------------------------                  
    // Phylum 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl06Phylum>> GetAllPhylumAsync();
    Task<IEnumerable<Tbl06Phylum>> SearchPhylumWithRegnumAsync(string? search);
    Task<Tbl06Phylum?> GetPhylumByIdAsync(int id);
    Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum entity);
    Task DeletePhylumAsync(int id);
    Task<bool> PhylumExistsAsync(Tbl06Phylum entity);

    Task<bool> PhylumHasSubphylumAsync(int phylumId);
    Task<bool> PhylumHasReferenceAsync(int phylumId);
    Task<bool> PhylumHasCommentAsync(int sphylumId);

    Task<IEnumerable<Tbl06Phylum>> GetPhylumForRegnumAsync(int regnumId);

    //Für ReportPDF
    Task<IEnumerable<Tbl12Subphylum>> GetSubphylumsByPhylumIdAsync(int phylumId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByPhylumIdAsync(int phylumId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByPhylumIdAsync(int phylumId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByPhylumIdAsync(int phylumId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByPhylumIdAsync(int phylumId);


    //alt----------------------------------
    ObservableCollection<Tbl06Phylum> GetPhylumAsync();
    ObservableCollection<Tbl06Phylum> GetPhylumAsync(int phylumid);
    ObservableCollection<Tbl06Phylum> GetPhylumAsync(string search);
    ObservableCollection<Tbl06Phylum> GetPhylumFromRegnumIdAsync(int regnumid);

    #endregion Phylum

    #region Subphylum

    // ---------------------------------------------------------                  
    // Subphylum 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl12Subphylum>> GetAllSubphylumAsync();
    Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumWithPhylumAsync(string? search);
    Task<Tbl12Subphylum> GetSubphylumByIdAsync(int id);
    Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum entity);
    Task DeleteSubphylumAsync(int id);
    Task<bool> SubphylumExistsAsync(Tbl12Subphylum entity);

    Task<bool> SubphylumHasSuperclassAsync(int subphylumId);
    Task<bool> SubphylumHasReferenceAsync(int subphylumId);
    Task<bool> SubphylumHasCommentAsync(int ssubphylumId);

    Task<IEnumerable<Tbl12Subphylum>> GetSubphylumForPhylumAsync(int phylumId);

    //Für ReportPDF
    Task<IEnumerable<Tbl18Superclass>> GetSuperclassesBySubphylumIdAsync(int subphylumId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubphylumIdAsync(int subphylumId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubphylumIdAsync(int subphylumId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubphylumIdAsync(int subphylumId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySubphylumIdAsync(int subphylumId);


    //alt
    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync();
    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(int subphylumid);
    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(string search);
    ObservableCollection<Tbl12Subphylum> GetSubphylumFromPhylumIdAsync(int phylumid);

    #endregion Subphylum

    #region Superclass

    // ---------------------------------------------------------                  
    // Superclass 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl18Superclass>> GetAllSuperclassAsync();
    Task<IEnumerable<Tbl18Superclass>> SearchSuperclassWithSubphylumAsync(string? search);
    Task<Tbl18Superclass> GetSuperclassByIdAsync(int id);
    Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass entity);
    Task DeleteSuperclassAsync(int id);
    Task<bool> SuperclassExistsAsync(Tbl18Superclass entity);

    Task<bool> SuperclassHasClassAsync(int superclassId);
    Task<bool> SuperclassHasReferenceAsync(int superclassId);
    Task<bool> SuperclassHasCommentAsync(int ssuperclassId);

    Task<IEnumerable<Tbl18Superclass>> GetSuperclassForSubphylumAsync(int subphylumId);

    //Für ReportPDF
    Task<IEnumerable<Tbl21Class>> GetClassesBySuperclassIdAsync(int superclassId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySuperclassIdAsync(int superclassId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySuperclassIdAsync(int superclassId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySuperclassIdAsync(int superclassId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySuperclassIdAsync(int superclassId);


    //alt
    ObservableCollection<Tbl18Superclass> GetSuperclassAsync();
    ObservableCollection<Tbl18Superclass> GetSuperclassAsync(int superclassid);
    ObservableCollection<Tbl18Superclass> GetSuperclassAsync(string search);
    ObservableCollection<Tbl18Superclass> GetSuperclassFromSubphylumIdAsync(int subphylumid);

    #endregion Superclass

    #region Class
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl21Class>> GetAllClassAsync();
    Task<IEnumerable<Tbl21Class>> SearchClassWithSuperclassAsync(string? search);
    Task<Tbl21Class> GetClassByIdAsync(int id);
    Task<Tbl21Class> UpsertClassAsync(Tbl21Class entity);
    Task DeleteClassAsync(int id);
    Task<bool> ClassExistsAsync(Tbl21Class entity);

    Task<bool> ClassHasSubclassAsync(int classId);
    Task<bool> ClassHasReferenceAsync(int classId);
    Task<bool> ClassHasCommentAsync(int sclassId);

    Task<IEnumerable<Tbl21Class>> GetClassForSuperclassAsync(int superclassId);

    //Für ReportPDF
    Task<IEnumerable<Tbl24Subclass>> GetSubclassesByClassIdAsync(int classId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByClassIdAsync(int classId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByClassIdAsync(int classId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByClassIdAsync(int classId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByClassIdAsync(int classId);


    //alt
    ObservableCollection<Tbl21Class> GetClassAsync();
    ObservableCollection<Tbl21Class> GetClassAsync(int classid);
    ObservableCollection<Tbl21Class> GetClassAsync(string search);
    ObservableCollection<Tbl21Class> GetClassFromSuperclassIdAsync(int superclassid);

    #endregion Class

    #region Subclass
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl24Subclass>> GetAllSubclassAsync();
    Task<IEnumerable<Tbl24Subclass>> SearchSubclassWithClassAsync(string? search);
    Task<Tbl24Subclass> GetSubclassByIdAsync(int id);
    Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass entity);
    Task DeleteSubclassAsync(int id);
    Task<bool> SubclassExistsAsync(Tbl24Subclass entity);

    Task<bool> SubclassHasInfraclassAsync(int subclassId);
    Task<bool> SubclassHasReferenceAsync(int subclassId);
    Task<bool> SubclassHasCommentAsync(int ssubclassId);

    Task<IEnumerable<Tbl24Subclass>> GetSubclassForClassAsync(int classeId);

    //Für ReportPDF
    Task<IEnumerable<Tbl27Infraclass>> GetInfraclassesBySubclassIdAsync(int subclassId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubclassIdAsync(int subclassId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubclassIdAsync(int subclassId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubclassIdAsync(int subclassId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySubclassIdAsync(int subclassId);

    //alt
    ObservableCollection<Tbl24Subclass> GetSubclassAsync();
    ObservableCollection<Tbl24Subclass> GetSubclassAsync(int subclassid);
    ObservableCollection<Tbl24Subclass> GetSubclassAsync(string search);
    ObservableCollection<Tbl24Subclass> GetSubclassFromClassIdAsync(int classeid);

    #endregion Subclass

    #region Infraclass                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl27Infraclass>> GetAllInfraclassAsync();
    Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassWithSubclassAsync(string? search);
    Task<Tbl27Infraclass> GetInfraclassByIdAsync(int id);
    Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass entity);
    Task DeleteInfraclassAsync(int id);
    Task<bool> InfraclassExistsAsync(Tbl27Infraclass entity);

    Task<bool> InfraclassHasLegioAsync(int infraclassId);
    Task<bool> InfraclassHasReferenceAsync(int infraclassId);
    Task<bool> InfraclassHasCommentAsync(int sinfraclassId);

    Task<IEnumerable<Tbl27Infraclass>> GetInfraclassForSubclassAsync(int subclassId);

    //Für ReportPDF
    Task<IEnumerable<Tbl30Legio>> GetLegiosByInfraclassIdAsync(int infraclassId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfraclassIdAsync(int infraclassId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfraclassIdAsync(int infraclassId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfraclassIdAsync(int infraclassId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByInfraclassIdAsync(int infraclassId);


    #endregion Infraclass

    #region Legio                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl30Legio>> GetAllLegioAsync();
    Task<IEnumerable<Tbl30Legio>> SearchLegioWithInfraclassAsync(string? search);
    Task<Tbl30Legio> GetLegioByIdAsync(int id);
    Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio entity);
    Task DeleteLegioAsync(int id);
    Task<bool> LegioExistsAsync(Tbl30Legio entity);

    Task<bool> LegioHasOrdoAsync(int legioId);
    Task<bool> LegioHasReferenceAsync(int legioId);
    Task<bool> LegioHasCommentAsync(int slegioId);

    Task<IEnumerable<Tbl30Legio>> GetLegioForInfraclassAsync(int infraclassId);

    //Für ReportPDF
    Task<IEnumerable<Tbl33Ordo>> GetOrdosByLegioIdAsync(int legioId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByLegioIdAsync(int legioId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByLegioIdAsync(int legioId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByLegioIdAsync(int legioId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByLegioIdAsync(int legioId);


    #endregion Legio

    #region Ordo                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl33Ordo>> GetAllOrdoAsync();
    Task<IEnumerable<Tbl33Ordo>> SearchOrdoWithLegioAsync(string? search);
    Task<Tbl33Ordo> GetOrdoByIdAsync(int id);
    Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo entity);
    Task DeleteOrdoAsync(int id);
    Task<bool> OrdoExistsAsync(Tbl33Ordo entity);

    Task<bool> OrdoHasSubordoAsync(int ordoId);
    Task<bool> OrdoHasReferenceAsync(int ordoId);
    Task<bool> OrdoHasCommentAsync(int sordoId);

    Task<IEnumerable<Tbl33Ordo>> GetOrdoForLegioAsync(int legioId);

    //Für ReportPDF
    Task<IEnumerable<Tbl36Subordo>> GetSubordosByOrdoIdAsync(int ordoId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByOrdoIdAsync(int ordoId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByOrdoIdAsync(int ordoId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByOrdoIdAsync(int ordoId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByOrdoIdAsync(int ordoId);


    #endregion Ordo

    #region Subordo                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl36Subordo>> GetAllSubordoAsync();
    Task<IEnumerable<Tbl36Subordo>> SearchSubordoWithOrdoAsync(string? search);
    Task<Tbl36Subordo> GetSubordoByIdAsync(int id);
    Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo entity);
    Task DeleteSubordoAsync(int id);
    Task<bool> SubordoExistsAsync(Tbl36Subordo entity);

    Task<bool> SubordoHasInfraordoAsync(int subordoId);
    Task<bool> SubordoHasReferenceAsync(int subordoId);
    Task<bool> SubordoHasCommentAsync(int ssubordoId);

    Task<IEnumerable<Tbl36Subordo>> GetSubordoForOrdoAsync(int ordoId);

    //Für ReportPDF
    Task<IEnumerable<Tbl39Infraordo>> GetInfraordosBySubordoIdAsync(int subordoId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubordoIdAsync(int subordoId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubordoIdAsync(int subordoId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubordoIdAsync(int subordoId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySubordoIdAsync(int subordoId);


    #endregion Subordo

    #region Infraordo                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl39Infraordo>> GetAllInfraordoAsync();
    Task<IEnumerable<Tbl39Infraordo>> SearchInfraordoWithSubordoAsync(string? search);
    Task<Tbl39Infraordo> GetInfraordoByIdAsync(int id);
    Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo entity);
    Task DeleteInfraordoAsync(int id);
    Task<bool> InfraordoExistsAsync(Tbl39Infraordo entity);

    Task<bool> InfraordoHasSuperfamilyAsync(int infraordoId);
    Task<bool> InfraordoHasReferenceAsync(int infraordoId);
    Task<bool> InfraordoHasCommentAsync(int sinfraordoId);

    Task<IEnumerable<Tbl39Infraordo>> GetInfraordoForSubordoAsync(int subordoId);

    //Für ReportPDF
    Task<IEnumerable<Tbl42Superfamily>> GetSuperfamiliesByInfraordoIdAsync(int infraordoId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfraordoIdAsync(int infraordoId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfraordoIdAsync(int infraordoId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfraordoIdAsync(int infraordoId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByInfraordoIdAsync(int infraordoId);


    #endregion Infraordo

    #region Superfamily                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl42Superfamily>> GetAllSuperfamilyAsync();
    Task<IEnumerable<Tbl42Superfamily>> SearchSuperfamilyWithInfraordoAsync(string? search);
    Task<Tbl42Superfamily> GetSuperfamilyByIdAsync(int id);
    Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily entity);
    Task DeleteSuperfamilyAsync(int id);
    Task<bool> SuperfamilyExistsAsync(Tbl42Superfamily entity);

    Task<bool> SuperfamilyHasFamilyAsync(int superfamilyId);
    Task<bool> SuperfamilyHasReferenceAsync(int superfamilyId);
    Task<bool> SuperfamilyHasCommentAsync(int ssuperfamilyId);

    Task<IEnumerable<Tbl42Superfamily>> GetSuperfamilyForInfraordoAsync(int infraordoId);

    //Für ReportPDF
    Task<IEnumerable<Tbl45Family>> GetFamiliesBySuperfamilyIdAsync(int superfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySuperfamilyIdAsync(int superfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySuperfamilyIdAsync(int superfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySuperfamilyIdAsync(int superfamilyId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySuperfamilyIdAsync(int superfamilyId);


    #endregion Superfamily

    #region Family                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl45Family>> GetAllFamilyAsync();
    Task<IEnumerable<Tbl45Family>> SearchFamilyWithSuperfamilyAsync(string? search);
    Task<Tbl45Family> GetFamilyByIdAsync(int id);
    Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family entity);
    Task DeleteFamilyAsync(int id);
    Task<bool> FamilyExistsAsync(Tbl45Family entity);

    Task<bool> FamilyHasSubfamilyAsync(int familyId);
    Task<bool> FamilyHasReferenceAsync(int familyId);
    Task<bool> FamilyHasCommentAsync(int sfamilyId);

    Task<IEnumerable<Tbl45Family>> GetFamilyForSuperfamilyAsync(int superfamilyId);

    //Für ReportPDF
    Task<IEnumerable<Tbl48Subfamily>> GetSubfamiliesByFamilyIdAsync(int familyId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByFamilyIdAsync(int familyId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByFamilyIdAsync(int familyId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByFamilyIdAsync(int familyId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByFamilyIdAsync(int familyId);


    #endregion Family

    #region Subfamily                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl48Subfamily>> GetAllSubfamilyAsync();
    Task<IEnumerable<Tbl48Subfamily>> SearchSubfamilyWithFamilyAsync(string? search);
    Task<Tbl48Subfamily> GetSubfamilyByIdAsync(int id);
    Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily entity);
    Task DeleteSubfamilyAsync(int id);
    Task<bool> SubfamilyExistsAsync(Tbl48Subfamily entity);

    Task<bool> SubfamilyHasInfrafamilyAsync(int subfamilyId);
    Task<bool> SubfamilyHasReferenceAsync(int subfamilyId);
    Task<bool> SubfamilyHasCommentAsync(int ssubfamilyId);

    Task<IEnumerable<Tbl48Subfamily>> GetSubfamilyForFamilyAsync(int familyId);

    //Für ReportPDF
    Task<IEnumerable<Tbl51Infrafamily>> GetInfrafamiliesBySubfamilyIdAsync(int subfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubfamilyIdAsync(int subfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubfamilyIdAsync(int subfamilyId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubfamilyIdAsync(int subfamilyId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySubfamilyIdAsync(int subfamilyId);


    #endregion Subfamily

    #region Infrafamily                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl51Infrafamily>> GetAllInfrafamilyAsync();
    Task<IEnumerable<Tbl51Infrafamily>> SearchInfrafamilyWithSubfamilyAsync(string? search);
    Task<Tbl51Infrafamily> GetInfrafamilyByIdAsync(int id);
    Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily entity);
    Task DeleteInfrafamilyAsync(int id);
    Task<bool> InfrafamilyExistsAsync(Tbl51Infrafamily entity);

    Task<bool> InfrafamilyHasSupertribusAsync(int infrafamilyId);
    Task<bool> InfrafamilyHasReferenceAsync(int infrafamilyId);
    Task<bool> InfrafamilyHasCommentAsync(int sinfrafamilyId);

    Task<IEnumerable<Tbl51Infrafamily>> GetInfrafamilyForSubfamilyAsync(int subfamilyId);

    //Für ReportPDF
    Task<IEnumerable<Tbl54Supertribus>> GetSupertribussesByInfrafamilyIdAsync(int infrafamilyId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfrafamilyIdAsync(int infrafamilyId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfrafamilyIdAsync(int infrafamilyId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfrafamilyIdAsync(int infrafamilyId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByInfrafamilyIdAsync(int infrafamilyId);


    #endregion Infrafamily

    #region Supertribus                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl54Supertribus>> GetAllSupertribusAsync();
    Task<IEnumerable<Tbl54Supertribus>> SearchSupertribusWithInfrafamilyAsync(string? search);
    Task<Tbl54Supertribus> GetSupertribusByIdAsync(int? id);
    Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus entity);
    Task DeleteSupertribusAsync(int id);
    Task<bool> SupertribusExistsAsync(Tbl54Supertribus entity);

    Task<bool> SupertribusHasTribusAsync(int supertribusId);
    Task<bool> SupertribusHasReferenceAsync(int supertribusId);
    Task<bool> SupertribusHasCommentAsync(int ssupertribusId);

    Task<IEnumerable<Tbl54Supertribus>> GetSupertribusForInfrafamilyAsync(int infrafamilyId);

    //Für ReportPDF
    Task<IEnumerable<Tbl57Tribus>> GetTribussesBySupertribusIdAsync(int supertribusId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySupertribusIdAsync(int supertribusId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySupertribusIdAsync(int supertribusId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySupertribusIdAsync(int supertribusId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySupertribusIdAsync(int supertribusId);

    //----------------------------------------------------
    //alt
    ObservableCollection<Tbl54Supertribus> GetSupertribusAsync();
 //   ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(int supertribusid);
    ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(string search);
    ObservableCollection<Tbl54Supertribus> GetSupertribusFromInfrafamilyIdAsync(int infrafamilyid);

    #endregion Supertribus

    #region Tribus
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl57Tribus>> GetAllTribusAsync();
    Task<IEnumerable<Tbl57Tribus>> SearchTribusWithSupertribusAsync(string? search);
    Task<Tbl57Tribus> GetTribusByIdAsync(int? id);
    Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus entity);
    Task DeleteTribusAsync(int id);
    Task<bool> TribusExistsAsync(Tbl57Tribus entity);

    Task<bool> TribusHasSubtribusAsync(int tribusId);
    Task<bool> TribusHasReferenceAsync(int tribusId);
    Task<bool> TribusHasCommentAsync(int tribusId);

    Task<IEnumerable<Tbl57Tribus>> GetTribusForSupertribusAsync(int supertribusId);

    //Für ReportPDF
    Task<IEnumerable<Tbl60Subtribus>> GetSubtribussesByTribusIdAsync(int tribusId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByTribusIdAsync(int tribusId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByTribusIdAsync(int tribusId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByTribusIdAsync(int tribusId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByTribusIdAsync(int tribusId);


    //---------------------------
    //Alt
    Task<IEnumerable<Tbl57Tribus>> SearchTribusAsync(string? search);
    Task<Tbl57Tribus> GetTribusByTribusIdAsync(int tribusId);
    Task<string> GetTribusNameByTribusId(int tribusId);
    Task<IEnumerable<Tbl57Tribus>> GetTribusAsync(int dummy);
    ObservableCollection<Tbl57Tribus> GetTribusAsync();
    ObservableCollection<Tbl57Tribus> GetTribusAsync(string search);
    ObservableCollection<Tbl57Tribus> GetTribusFromSupertribusIdAsync(int supertribusid);

    #endregion Tribus

    #region Subtribus                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl60Subtribus>> GetAllSubtribusAsync();
    Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusWithTribusAsync(string? search);
    Task<Tbl60Subtribus> GetSubtribusByIdAsync(int? id);
    Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus entity);
    Task DeleteSubtribusAsync(int id);
    Task<bool> SubtribusExistsAsync(Tbl60Subtribus entity);

    Task<bool> SubtribusHasInfratribusAsync(int subtribusId);
    Task<bool> SubtribusHasReferenceAsync(int subtribusId);
    Task<bool> SubtribusHasCommentAsync(int subtribusId);

    Task<IEnumerable<Tbl60Subtribus>> GetSubtribusForTribusAsync(int tribusId);

    //Für ReportPDF
    Task<IEnumerable<Tbl63Infratribus>> GetInfratribussesBySubtribusIdAsync(int subtribusId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySubtribusIdAsync(int subtribusId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySubtribusIdAsync(int subtribusId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySubtribusIdAsync(int subtribusId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySubtribusIdAsync(int subtribusId);

    //alt
    //--------------------------------------------------------
    Task<IEnumerable<Tbl60Subtribus>> GetSubtribusAsync(int dummy);
    Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusAsync(string? search);
    ObservableCollection<Tbl60Subtribus> GetSubtribusAsync();
    ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(string search);
    ObservableCollection<Tbl60Subtribus> GetSubtribusFromTribusIdAsync(int tribusid);

    #endregion Subtribus

    #region Infratribus
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl63Infratribus>> GetAllInfratribusAsync();
    Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusWithSubtribusAsync(string? search);
    Task<Tbl63Infratribus> GetInfratribusByIdAsync(int? id);
    Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus entity);
    Task DeleteInfratribusAsync(int id);
    Task<bool> InfratribusExistsAsync(Tbl63Infratribus entity);

    Task<bool> InfratribusHasGenusAsync(int infratribusId);
    Task<bool> InfratribusHasReferenceAsync(int infratribusId);
    Task<bool> InfratribusHasCommentAsync(int infratribusId);

    Task<IEnumerable<Tbl63Infratribus>> GetInfratribusForSubtribusAsync(int subtribusId);

    //Für ReportPDF
    Task<IEnumerable<Tbl66Genus>> GetGenussesByInfratribusIdAsync(int infratribusId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByInfratribusIdAsync(int infratribusId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByInfratribusIdAsync(int infratribusId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByInfratribusIdAsync(int infratribusId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByInfratribusIdAsync(int infratribusId);


    //alt
    //----------------------------
    Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusAsync(string? search);
    Task<IEnumerable<Tbl63Infratribus>> GetInfratribusAsync(int dummy);
    Task<ObservableCollection<Tbl63Infratribus>> GetInfratribusFromSubtribusIdAsync(int subtribusId);

    //-------------------------------------------------------------------------------
    ObservableCollection<Tbl63Infratribus> GetInfratribusAsyncAlt();

    #endregion Infratribus

    #region Genus                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl66Genus>> GetAllGenusAsync();
    Task<IEnumerable<Tbl66Genus>> SearchGenusWithInfratribusAsync(string? search);
    Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus entity);

    Task<Tbl66Genus> GetGenusByIdAsync(int id);
    Task DeleteGenusAsync(int id);
    Task<bool> GenusExistsAsync(Tbl66Genus entity);

    Task<bool> GenusHasSpeciesAsync(int genusId);
    Task<bool> GenusHasReferenceAsync(int genusId);
    Task<bool> GenusHasCommentAsync(int genusId);

    Task<IEnumerable<Tbl66Genus>> GetGenusForInfratribusAsync(int infratribusId);

    //Für ReportPDF
    Task<IEnumerable<Tbl75Species>> GetSpeciessesByGenusIdAsync(int genusId);
    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesByGenusIdAsync(int genusId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesByGenusIdAsync(int genusId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesByGenusIdAsync(int genusId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsByGenusIdAsync(int genusId);


    //--------------------------
    //alt
    Task<IEnumerable<Tbl66Genus>> GetGenusAsync(int dummy);
    Task<IEnumerable<Tbl66Genus>> SearchGenusAsync(string? search);
    ObservableCollection<Tbl66Genus> GetGenusAsync();
  //  ObservableCollection<Tbl66Genus> GetGenusAsync(int genusid);
    ObservableCollection<Tbl66Genus> GetGenusAsync(string search);
    ObservableCollection<Tbl66Genus> GetGenusFromInfratribusIdAsync(int infratribusid);
    //   Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus genus);
    //   Task DeleteGenusAsync(int genusid);

    #endregion Genus

    #region Speciesgroup                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl68Speciesgroup>> GetAllSpeciesgroupAsync();
    Task<IEnumerable<Tbl68Speciesgroup>> SearchSpeciesgroupAsync(string? search);
    Task<Tbl68Speciesgroup?> GetSpeciesgroupByIdAsync(int id);
    Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup entity);
    Task DeleteSpeciesgroupAsync(int id);
    Task<bool> SpeciesgroupExistsAsync(Tbl68Speciesgroup entity);

    Task<bool> SpeciesgroupHasSpeciesAsync(int speciesgroupId);

    //alt
    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync();
    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(int speciesgroupid);
    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(string search);
    //Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup speciesgroup);
    //Task DeleteSpeciesgroupAsync(int speciesgroupid);

    #endregion Speciesgroup

    #region Species
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl75Species>> GetAllSpeciesAsync();
    Task<IEnumerable<Tbl75Species>> GetSpeciesWithGenusAndSpeciesgroupByIdAsync(string? search);
    Task<Tbl75Species> GetSpeciesByIdAsync(int id);
    Task<Tbl75Species> GetSpeciesWithGenusAndSpeciesgroupByIdAsync(int id);
    Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species entity);
    Task DeleteSpeciesAsync(int id);
    Task<bool> SpeciesExistsAsync(Tbl75Species entity);

    Task<bool> SpeciesHasNameAsync(int speciesId);
    Task<bool> SpeciesHasImageAsync(int speciesId);
    Task<bool> SpeciesHasSynonymAsync(int speciesId);
    Task<bool> SpeciesHasGeographicAsync(int speciesId);
    Task<bool> SpeciesHasReferenceAsync(int speciesId);
    Task<bool> SpeciesHasCommentAsync(int speciesId);

    Task<IEnumerable<Tbl75Species>> GetSpeciesForGenusAsync(int genusId);
    Task<IEnumerable<Tbl75Species>> GetSpeciesForSpeciesgroupAsync(int speciesgroupId);

    //Für ReportPDF
    Task<IEnumerable<Tbl78Name>> GetNamesBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl81Image>> GetImagesBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl84Synonym>> GetSynonymsBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl87Geographic>> GetGeographicsBySpeciesIdAsync(int speciesId);

    Task<IEnumerable<Tbl90Reference>> GetExpertReferencesBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl90Reference>> GetSourceReferencesBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl90Reference>> GetAuthorReferencesBySpeciesIdAsync(int speciesId);
    Task<IEnumerable<Tbl93Comment>> GetCommentsBySpeciesIdAsync(int speciesId);

    //neu
    //Task<IEnumerable<Tbl75Species>> GetAllSpeciesAsync();
    //Task<IEnumerable<Tbl75Species>> SearchSpeciesWithGenusAsync(string? search);
    //Task<Tbl75Species> UpsertSpeciesAsync(Tbl75Species species);
    //Task DeleteSpeciesAsync(int speciesid);
    //Task<bool> SpeciesExistsAsync(Tbl75Species entity);

    //Task<bool> SpeciesHasReferenceAsync(int speciesId);
    //Task<bool> SpeciesHasCommentAsync(int speciesId);
    //Task<bool> SpeciesHasNameAsync(int speciesId);

    //Task<IEnumerable<Tbl75Species>> GetSpeciesForGenusAsync(int genusId);

    //--------------------------------
    //Alt
    //   Task<IEnumerable<Tbl75Species>> GetSpeciesAsync(int dummy);
    //  Task<IEnumerable<Tbl75Species>> SearchSpeciesAsync(string? search);
    ObservableCollection<Tbl75Species> GetSpeciesAsync();

    //  ObservableCollection<Tbl75Species> GetSpeciesAsync(int speciesid);
  //  ObservableCollection<Tbl75Species> GetSpeciesAsync(string search);
    //ObservableCollection<Tbl75Species> GetSpeciesFromGenusNameAsync(string search);
    ObservableCollection<Tbl75Species> GetSpeciesFromGenusIdAsync(int genusid);

    #endregion Species

    #region Name
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl78Name>> GetAllNameAsync();
    Task<IEnumerable<Tbl78Name>> SearchNameWithSpeciesAndGenusAsync(string? search);
    Task<Tbl78Name?> GetNameByIdAsync(int id);
    Task<Tbl78Name> UpsertNameAsync(Tbl78Name entity);
    Task DeleteNameAsync(int id);
    Task<bool> NameExistsAsync(Tbl78Name entity);

    //Task<bool> NameHasImageAsync(int nameId);
    //Task<bool> NameHasReferenceAsync(int nameId);
    //Task<bool> NameHasCommentAsync(int snameId);

    Task<IEnumerable<Tbl78Name>> GetNameForSpeciesAsync(int speciesId);


    //alt------------------------
    ObservableCollection<Tbl78Name> GetNameAsync();
    ObservableCollection<Tbl78Name> GetNameAsync(int nameid);
    ObservableCollection<Tbl78Name> GetNameAsync(string search);
    ObservableCollection<Tbl78Name> GetNameFromSpeciesIdAsync(int speciesid);
    //ObservableCollection<Tbl78Name> GetNameFromPlSpeciesIdAsync(int plspeciesid);
    //Task<Tbl78Name> UpsertNameAsync(Tbl78Name name);
    //Task DeleteNameAsync(int nameid);

    #endregion Name

    #region Image
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl81Image>> GetAllImageAsync();
    Task<IEnumerable<Tbl81Image>> SearchImageWithSpeciesAndGenusAsync(string? search);
    Task<Tbl81Image?> GetImageByIdAsync(int id);
    Task<Tbl81Image> UpsertImageAsync(Tbl81Image entity);
    Task DeleteImageAsync(int id);
    Task<bool> ImageExistsAsync(Tbl81Image entity);

    //Task<bool> ImageHasNameAsync(int imageId);
    //Task<bool> ImageHasReferenceAsync(int imageId);
    //Task<bool> ImageHasCommentAsync(int simageId);

    Task<IEnumerable<Tbl81Image>> GetImageForSpeciesAsync(int speciesId);

    //alr------------------------   
    ObservableCollection<Tbl81Image> GetImageAsync();
    ObservableCollection<Tbl81Image> GetImageAsync(int imageid);
    ObservableCollection<Tbl81Image> GetImageAsync(string search);
    ObservableCollection<Tbl81Image> GetImageFromSpeciesIdAsync(int speciesid);
    //ObservableCollection<Tbl81Image> GetImageFromPlSpeciesIdAsync(int plspeciesid);
    Task<Tbl81Image> UpsertImageAsync(Tbl81Image image, string selectedpath);
    //Task DeleteImageAsync(int imageid);

    #endregion Image                  

    #region Synonym
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl84Synonym>> GetAllSynonymAsync();
    Task<IEnumerable<Tbl84Synonym>> SearchSynonymWithSpeciesAndGenusAsync(string? search);
    Task<Tbl84Synonym?> GetSynonymByIdAsync(int id);
    Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym entity);
    Task DeleteSynonymAsync(int id);
    Task<bool> SynonymExistsAsync(Tbl84Synonym entity);

    //Task<bool> SynonymHasNameAsync(int synonymId);
    //Task<bool> SynonymHasReferenceAsync(int synonymId);
    //Task<bool> SynonymHasCommentAsync(int ssynonymId);

    Task<IEnumerable<Tbl84Synonym>> GetSynonymForSpeciesAsync(int speciesId);

    //alt-----------------------------------
    ObservableCollection<Tbl84Synonym> GetSynonymAsync();
    ObservableCollection<Tbl84Synonym> GetSynonymAsync(int synonymid);
    ObservableCollection<Tbl84Synonym> GetSynonymAsync(string search);
    ObservableCollection<Tbl84Synonym> GetSynonymFromSpeciesIdAsync(int speciesid);
    //ObservableCollection<Tbl84Synonym> GetSynonymFromPlSpeciesIdAsync(int plspeciesid);
    //Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym synonym);
    //Task DeleteSynonymAsync(int synonymid);

    #endregion Synonym

    #region Geographic
    // ---------------------------------------------------------                  
    // Geographic 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl87Geographic>> GetAllGeographicAsync();
    Task<IEnumerable<Tbl87Geographic>> SearchGeographicWithSpeciesAndGenusAsync(string? search);
    Task<Tbl87Geographic?> GetGeographicByIdAsync(int id);
    Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic entity);
    Task DeleteGeographicAsync(int id);
    Task<bool> GeographicExistsAsync(Tbl87Geographic entity);

    //Task<bool> GeographicHasNameAsync(int geographicId);
    //Task<bool> GeographicHasReferenceAsync(int geographicId);
    //Task<bool> GeographicHasCommentAsync(int sgeographicId);

    Task<IEnumerable<Tbl87Geographic>> GetGeographicForSpeciesAsync(int speciesId);

    //alt-----------------------------------
    ObservableCollection<Tbl87Geographic> GetGeographicAsync();
    ObservableCollection<Tbl87Geographic> GetGeographicAsync(int geographicid);
    ObservableCollection<Tbl87Geographic> GetGeographicAsync(string search);
    ObservableCollection<Tbl87Geographic> GetGeographicFromSpeciesIdAsync(int speciesid);
    //ObservableCollection<Tbl87Geographic> GetGeographicFromPlSpeciesIdAsync(int plspeciesid);
    //Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic geographic);
    //Task DeleteGeographicAsync(int geographicid);

    #endregion Geographic                  


    #region Reference
    //------------------------------------------------------

    Task<IEnumerable<Tbl90Reference>> GetAllReferenceAsync();
    Task<IEnumerable<Tbl90Reference>> SearchReferenceAsync(string? search);
    Task<bool> ReferenceExistsAsync(Tbl90Reference entity);
    Task<Tbl90Reference> GetReferenceByIdAsync(int id);
    Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference model);
    Task DeleteReferenceAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForRegnumAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForPhylumAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSubphylumAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperclassAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForClassAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSubclassAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraclassAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForLegioAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForOrdoAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSubordoAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForInfraordoAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSuperfamilyAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForFamilyAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSubfamilyAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForInfrafamilyAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSupertribusAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForTribusAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSubtribusAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForInfratribusAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForGenusAsync(int id);
    Task<IEnumerable<Tbl90Reference>> GetReferencesForSpeciesAsync(int id);
    //----------------------------------------
    Task DeleteReferencesAsync(int infratribusId);
    ObservableCollection<Tbl90Reference> GetReferenceAsync();
    ObservableCollection<Tbl90Reference> GetReferenceAsync(int referenceid);
    Task<ObservableCollection<Tbl90Reference>> GetReferenceAsync(string search);
    //Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference reference);
    //Task DeleteReferenceAsync(int referenceid);

    //-----------------------------------------------------------
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceAuthorsCollectionOrderByInfoFromGenusIdAndRefSourceIdIsNullAndRefExpertIdIsNull(int? genusId);
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceSourcesCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefExpertIdIsNull(int? genusId);
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceExpertsCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefSourceIdIsNull(int? genusId);

    #endregion Reference

    #region RefAuthor
    // ---------------------------------------------------------                  
    // RefAuthor 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl90RefAuthor>> GetAllRefAuthorAsync();
    Task<IEnumerable<Tbl90RefAuthor>> SearchRefAuthorAsync(string? search);
    Task<Tbl90RefAuthor> GetRefAuthorByIdAsync(int id);
    Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor entity);
    Task DeleteRefAuthorAsync(int id);
    Task<bool> RefAuthorExistsAsync(Tbl90RefAuthor entity);

    //Task<bool> RefAuthorHasSpeciesAsync(int refAuthorId);
    //Task<bool> RefAuthorHasReferenceAsync(int refAuthorId);
    //Task<bool> RefAuthorHasCommentAsync(int srefAuthorId);

    //Task<IEnumerable<Tbl90RefAuthor>> GetRefAuthorForNULLAsync(int NULLId);

    //Task<IEnumerable<Tbl90RefAuthor>> GetAllRefAuthorsAsync();
    //Task<IEnumerable<Tbl90RefAuthor>> SearchRefAuthorAsync(string? search);
    //Task<Tbl90RefAuthor> GetRefAuthorByIdAsync(int id);

    //Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor model);
    //Task DeleteRefAuthorAsync(int id);
    //Task<bool> RefAuthorExistsAsync(Tbl90RefAuthor model);

    //----------------------------------------------
    //alt
    ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsyncAlt();

    //ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync();
    //ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync(int refauthorid);
    //Task<ObservableCollection<Tbl90RefAuthor>> GetRefAuthorAsync(string search);
    //Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor refauthor);
    //Task DeleteRefAuthorAsync(int refauthorid);

    #endregion RefAuthor

    #region RefSource
    // ---------------------------------------------------------                  
    // RefSource 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl90RefSource>> GetAllRefSourceAsync();
    Task<IEnumerable<Tbl90RefSource>> SearchRefSourceAsync(string? search);
    Task<Tbl90RefSource> GetRefSourceByIdAsync(int id);
    Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource entity);
    Task DeleteRefSourceAsync(int id);
    Task<bool> RefSourceExistsAsync(Tbl90RefSource entity);
    //alt----------------------
    ObservableCollection<Tbl90RefSource> GetRefSourceAsyncAlt();

    #endregion

    #region RefExpert
    // ---------------------------------------------------------                  
    // RefExpert 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl90RefExpert>> GetAllRefExpertAsync();
    Task<IEnumerable<Tbl90RefExpert>> SearchRefExpertAsync(string? search);
    Task<Tbl90RefExpert> GetRefExpertByIdAsync(int id);
    Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert entity);
    Task DeleteRefExpertAsync(int id);
    Task<bool> RefExpertExistsAsync(Tbl90RefExpert entity);

    //--------------------------------------------------------------

    ObservableCollection<Tbl90RefExpert> GetRefExpertAsyncAlt();
    ObservableCollection<Tbl90RefExpert> GetRefExpertAsyncAlt(int refexpertid);
    Task<ObservableCollection<Tbl90RefExpert>> GetRefExpertAsyncAlt(string search);
    Task<Tbl90RefExpert> UpsertRefExpertAsyncAlt(Tbl90RefExpert refexpert);
    Task DeleteRefExpertAsyncAlt(int refexpertid);

    #endregion RefExpert

    #region Comment
    //Neu
    Task<IEnumerable<Tbl93Comment>> GetAllCommentAsync();
    Task<IEnumerable<Tbl93Comment>> SearchCommentAsync(string? search);
    Task<bool> CommentExistsAsync(Tbl93Comment entity);
    Task<Tbl93Comment> GetCommentByIdAsync(int id);
    Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment model);
    Task DeleteCommentAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForRegnumAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForPhylumAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSubphylumAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperclassAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForClassAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSubclassAsync(int id);
Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraclassAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForLegioAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForOrdoAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSubordoAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForInfraordoAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSuperfamilyAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForFamilyAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSubfamilyAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForInfrafamilyAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSupertribusAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForTribusAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSubtribusAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForInfratribusAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForGenusAsync(int id);
    Task<IEnumerable<Tbl93Comment>> GetCommentsForSpeciesAsync(int id);
    Task DeleteCommentsAsync(int infratribusId);

    //------------------------------------------
    // alt
    ObservableCollection<Tbl93Comment> GetCommentAsync();
    ObservableCollection<Tbl93Comment> GetCommentAsync(int commentid);
    Task<ObservableCollection<Tbl93Comment>> GetCommentAsync(string search);

    #endregion Comment

    #region Country
    // ---------------------------------------------------------                  
    // Country 
    // ---------------------------------------------------------   
    Task<IEnumerable<TblCountry>> GetAllCountryAsync();
    Task<IEnumerable<TblCountry>> SearchCountryAsync(string? search);
    Task<TblCountry?> GetCountryByIdAsync(int id);
    Task<TblCountry> UpsertCountryAsync(TblCountry entity);
    Task DeleteCountryAsync(int id);
    Task<bool> CountryExistsAsync(TblCountry entity);


    //alt
    Task<List<TblCountry>> GetAllCountriesAsync();
    Task<List<TblCountry>> GetCountriesByNameOrIdAsync(string? query);
    Task<List<TblCountry>> GetCountriesByNameAsync(string? query);
 //   Task<bool> CountryExistsAsync(TblCountry model);
    Task<List<TblCountry>> GetCountryAsync();
    Task<List<TblCountry>> GetCountryAsync(int countryid);
    Task<TblCountry?> GetCountryByIdAsync(int? countryid);
 //   Task<TblCountry> UpsertCountryAsync(TblCountry country);
    Task DeleteCountryAsync(int? countryId);

    #endregion Country

    #region UserProfile
    // ---------------------------------------------------------   
    Task<IEnumerable<TblUserProfile>> GetAllUserProfileAsync();
    Task<IEnumerable<TblUserProfile>> SearchUserProfileWithNULLAsync(string? search);
    Task<TblUserProfile?> GetUserProfileByIdAsync(int id);
    Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile entity);
    Task DeleteUserProfileAsync(int id);
    Task<bool> UserProfileExistsAsync(TblUserProfile entity);

    //alt
    ObservableCollection<TblUserProfile> GetUserProfileAsync();
    ObservableCollection<TblUserProfile> GetUserProfileAsync(int userprofileid);
    ObservableCollection<TblUserProfile> GetEmailFromUserProfileAsync(string email);
    Task<ObservableCollection<TblUserProfile>> GetUserProfileAsync(string search);
    //Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile userprofile);
    Task<TblUserProfile> UpsertEmailFromUserProfileAsync(TblUserProfile userprofile);
    //Task DeleteUserProfileAsync(int userprofileid);

    #endregion UserProfile

    #region Food

    ObservableCollection<TblFood> GetFoodAsync();
    ObservableCollection<TblFood> GetFoodAsync(int foodid);
    Task<ObservableCollection<TblFood>> GetFoodAsync(string search);
    Task<TblFood> UpsertFoodAsync(TblFood food);
    Task DeleteFoodAsync(int foodid);

    #endregion Food

    #region Disease

    ObservableCollection<TblDisease> GetDiseaseAsync();
    ObservableCollection<TblDisease> GetDiseaseAsync(int diseaseid);
    Task<ObservableCollection<TblDisease>> GetDiseaseAsync(string search);
    Task<TblDisease> UpsertDiseaseAsync(TblDisease disease);
    Task DeleteDiseaseAsync(int diseaseid);

    #endregion Disease

    #region Miscellaneous

    ObservableCollection<TblMiscellaneous> GetMiscellaneousAsync();
    ObservableCollection<TblMiscellaneous> GetMiscellaneousAsync(int miscellaneousid);
    Task<ObservableCollection<TblMiscellaneous>> GetMiscellaneousAsync(string search);
    Task<TblMiscellaneous> UpsertMiscellaneousAsync(TblMiscellaneous miscellaneous);
    Task DeleteMiscellaneousAsync(int miscellaneousid);

    #endregion Miscellaneous

    #endregion CRUD

    #region Search

    Task<IEnumerable<SearchResultItem>> SearchAcrossTablesAsync(string? search, string? entityType = "All");
    Task<IEnumerable<object>> SearchAsync(string tableName, string searchText);
    Task<object?> GetByIdAsync(string tableName, int id);
    #endregion

    #region Search_Gets

    ObservableCollection<Tbl03Regnum> GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(string filterText);
    ObservableCollection<Tbl06Phylum> GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(string filterText);
    ObservableCollection<Tbl12Subphylum> GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(string filterText);
    ObservableCollection<Tbl18Superclass> GetTbl18SuperclassesCollectionOrderBySuperclassNameFromFilterText(string filterText);
    ObservableCollection<Tbl21Class> GetTbl21ClassesCollectionOrderByClassNameFromFilterText(string filterText);
    ObservableCollection<Tbl24Subclass> GetTbl24SubclassesCollectionOrderBySubclassNameFromFilterText(string filterText);
    ObservableCollection<Tbl27Infraclass> GetTbl27InfraclassesCollectionOrderByInfraclassNameFromFilterText(string filterText);
    ObservableCollection<Tbl30Legio> GetTbl30LegiosCollectionOrderByLegioNameFromFilterText(string filterText);
    ObservableCollection<Tbl33Ordo> GetTbl33OrdosCollectionOrderByOrdoNameFromFilterText(string filterText);
    ObservableCollection<Tbl36Subordo> GetTbl36SubordosCollectionOrderBySubordoNameFromFilterText(string filterText);
    ObservableCollection<Tbl39Infraordo> GetTbl39InfraordosCollectionOrderByInfraordoNameFromFilterText(string filterText);
    ObservableCollection<Tbl42Superfamily> GetTbl42SuperfamiliesCollectionOrderBySuperfamilyNameFromFilterText(string filterText);
    ObservableCollection<Tbl45Family> GetTbl45FamiliesCollectionOrderByFamilyNameFromFilterText(string filterText);
    ObservableCollection<Tbl48Subfamily> GetTbl48SubfamiliesCollectionOrderBySubfamilyNameFromFilterText(string filterText);
    ObservableCollection<Tbl51Infrafamily> GetTbl51InfrafamiliesCollectionOrderByInfrafamilyNameFromFilterText(string filterText);
    ObservableCollection<Tbl54Supertribus> GetTbl54SupertribussesCollectionOrderBySupertribusNameFromFilterText(string filterText);
    ObservableCollection<Tbl57Tribus> GetTbl57TribussesCollectionOrderByTribusNameFromFilterText(string filterText);
    ObservableCollection<Tbl60Subtribus> GetTbl60SubtribussesCollectionOrderBySubtribusNameFromFilterText(string filterText);
    ObservableCollection<Tbl63Infratribus> GetTbl63InfratribussesCollectionOrderByInfratribusNameFromFilterText(string filterText);
    ObservableCollection<Tbl66Genus> GetTbl66GenussesCollectionOrderByGenusNameFromFilterText(string filterText);
    ObservableCollection<Tbl75Species> GetTbl75SpeciessesCollectionOrderByGenusNameAndSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText);
    //ObservableCollection<Tbl72PlSpecies> GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText);
    ObservableCollection<Tbl78Name> GetTbl78NamesCollectionOrderByNameNameFromFilterText(string filterText);
    ObservableCollection<Tbl84Synonym> GetTbl84SynonymsCollectionOrderBySynonymNameFromFilterText(string filterText);

    
    ObservableCollection<TblFood> GetTblFoodsCollectionOrderByFoodNameFromFilterText(string filterText);
    Task<List<TblDisease>> GetDiseasesByNameOrIdAsync(string query);
    Task<List<TblFood>> GetFoodsByNameOrIdAsync(string query);
    Task<List<TblMiscellaneous>> GetMiscellaneoussesByNameOrIdAsync(string query);
    ObservableCollection<TblDisease> GetTblDiseasesCollectionOrderByDiseaseNameFromFilterText(string filterText);
    ObservableCollection<TblMiscellaneous> GetTblMiscellaneoussesCollectionOrderByMiscellaneousNameFromFilterText(string filterText);


    #endregion Search_Gets

    #region Report_Gets

    #region Regnum
    Tbl03Regnum GetRegnumSingleByRegnumIdAndHash(int id);
    Tbl03Regnum GetRegnumSingleByRegnumId(int id);

    ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumId(int id);
    //direct children
    ObservableCollection<Tbl06Phylum> CollPhylumsByRegnumIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByRegnumId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByRegnumId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByRegnumId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByRegnumId(int id);
    #endregion Regnum

    #region Phylum
    Tbl06Phylum GetPhylumSingleByPhylumIdAndHash(int id);
    Tbl06Phylum GetPhylumSingleByPhylumId(int id);

    ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumId(int id);
    //direct children
    ObservableCollection<Tbl12Subphylum> CollSubphylumsByPhylumIdAndHash(int id);
    //Function
    int RegnumIdFromPhylumsCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByPhylumId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByPhylumId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByPhylumId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByPhylumId(int id);
    #endregion Phylum

    #region Subphylum
    Tbl12Subphylum GetSubphylumSingleBySubphylumIdAndHash(int id);
    Tbl12Subphylum GetSubphylumSingleBySubphylumId(int id);

    ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumId(int id);
    //direct children
    ObservableCollection<Tbl18Superclass> CollSuperclassesBySubphylumIdAndHash(int id);
    //Function
    int PhylumIdFromSubphylumsCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl06Phylum> CollPhylumsByPhylumIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubphylumId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubphylumId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubphylumId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubphylumId(int id);
    #endregion Subphylum

    #region Superclass
    Tbl18Superclass GetSuperclassSingleBySuperclassIdAndHash(int id);
    ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassId(int id);
    //direct children
    ObservableCollection<Tbl21Class> CollClassesBySuperclassIdAndHash(int id);
    //Function
    //   int SubdivisionIdFromSuperclassesCollectionSelect(int id);
    int SubphylumIdFromSuperclassesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySuperclassId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySuperclassId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySuperclassId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySuperclassId(int id);
    #endregion Superclass

    #region Class
    Tbl21Class GetClassSingleByClassIdAndHash(int id);

    ObservableCollection<Tbl21Class> CollClassesByClassId(int id);
    //direct children
    ObservableCollection<Tbl24Subclass> CollSubclassesByClassIdAndHash(int id);
    //Function
    int SuperclassIdFromClassesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByClassId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByClassId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByClassId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByClassId(int id);
    #endregion Class

    #region Subclass
    Tbl24Subclass GetSubclassSingleBySubclassIdAndHash(int id);

    ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassId(int id);
    //direct children
    ObservableCollection<Tbl27Infraclass> CollInfraclassesBySubclassIdAndHash(int id);
    //Function
    int ClassIdFromSubclassesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl21Class> CollClassesByClassIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubclassId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubclassId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubclassId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubclassId(int id);
    #endregion Subclass

    #region Infraclass
    Tbl27Infraclass GetInfraclassSingleByInfraclassIdAndHash(int id);

    ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassId(int id);
    //direct children
    ObservableCollection<Tbl30Legio> CollLegiosByInfraclassIdAndHash(int id);
    //Function
    int SubclassIdFromInfraclassesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl24Subclass> CollSubclassesBySubclassIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByInfraclassId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByInfraclassId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByInfraclassId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByInfraclassId(int id);
    #endregion Infraclass

    #region Legio
    Tbl30Legio GetLegioSingleByLegioIdAndHash(int id);

    ObservableCollection<Tbl30Legio> CollLegiosByLegioId(int id);
    //direct children
    ObservableCollection<Tbl33Ordo> CollOrdosByLegioIdAndHash(int id);
    //Function
    int InfraclassIdFromLegiosCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl27Infraclass> CollInfraclassesByInfraclassIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByLegioId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByLegioId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByLegioId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByLegioId(int id);
    #endregion Legio

    #region Ordo
    Tbl33Ordo GetOrdoSingleByOrdoIdAndHash(int id);

    ObservableCollection<Tbl33Ordo> CollOrdosByOrdoId(int id);
    //direct children
    ObservableCollection<Tbl36Subordo> CollSubordosByOrdoIdAndHash(int id);
    //Function
    int LegioIdFromOrdosCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl30Legio> CollLegiosByLegioIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByOrdoId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByOrdoId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByOrdoId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByOrdoId(int id);
    #endregion Ordo

    #region Subordo
    Tbl36Subordo GetSubordoSingleBySubordoIdAndHash(int id);

    ObservableCollection<Tbl36Subordo> CollSubordosBySubordoId(int id);
    //direct children
    ObservableCollection<Tbl39Infraordo> CollInfraordosBySubordoIdAndHash(int id);
    //Function
    int OrdoIdFromSubordosCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl33Ordo> CollOrdosByOrdoIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubordoId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubordoId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubordoId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubordoId(int id);
    #endregion Subordo

    #region Infraordo
    Tbl39Infraordo GetInfraordoSingleByInfraordoIdAndHash(int id);

    ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoId(int id);
    //direct children
    ObservableCollection<Tbl42Superfamily> CollSuperfamiliesByInfraordoIdAndHash(int id);
    //Function
    int SubordoIdFromInfraordosCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl36Subordo> CollSubordosBySubordoIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByInfraordoId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByInfraordoId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByInfraordoId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByInfraordoId(int id);
    #endregion Infraordo

    #region Superfamily
    Tbl42Superfamily GetSuperfamilySingleBySuperfamilyIdAndHash(int id);

    ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyId(int id);
    //direct children
    ObservableCollection<Tbl45Family> CollFamiliesBySuperfamilyIdAndHash(int id);
    //Function
    int InfraordoIdFromSuperfamiliesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl39Infraordo> CollInfraordosByInfraordoIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySuperfamilyId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySuperfamilyId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySuperfamilyId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySuperfamilyId(int id);
    #endregion Superfamily

    #region Family
    Tbl45Family GetFamilySingleByFamilyIdAndHash(int id);

    ObservableCollection<Tbl45Family> CollFamiliesByFamilyId(int id);
    //direct children
    ObservableCollection<Tbl48Subfamily> CollSubfamiliesByFamilyIdAndHash(int id);
    //Function
    int SuperfamilyIdFromFamiliesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl42Superfamily> CollSuperfamiliesBySuperfamilyIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByFamilyId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByFamilyId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByFamilyId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByFamilyId(int id);
    #endregion Family

    #region Subfamily
    Tbl48Subfamily GetSubfamilySingleBySubfamilyIdAndHash(int id);

    ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyId(int id);
    //direct children
    ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesBySubfamilyIdAndHash(int id);
    //Function
    int FamilyIdFromSubfamiliesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl45Family> CollFamiliesByFamilyIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubfamilyId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubfamilyId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubfamilyId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubfamilyId(int id);
    #endregion Subfamily

    #region Infrafamily
    Tbl51Infrafamily GetInfrafamilySingleByInfrafamilyIdAndHash(int id);

    ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyId(int id);
    //direct children
    ObservableCollection<Tbl54Supertribus> CollSupertribussesByInfrafamilyIdAndHash(int id);
    //Function
    int SubfamilyIdFromInfrafamiliesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl48Subfamily> CollSubfamiliesBySubfamilyIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByInfrafamilyId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByInfrafamilyId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByInfrafamilyId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByInfrafamilyId(int id);
    #endregion Infrafamily

    #region Supertribus
    Tbl54Supertribus GetSupertribusSingleBySupertribusIdAndHash(int id);

    ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusId(int id);
    //direct children
    ObservableCollection<Tbl57Tribus> CollTribussesBySupertribusIdAndHash(int id);
    //Function
    int InfrafamilyIdFromSupertribussesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl51Infrafamily> CollInfrafamiliesByInfrafamilyIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySupertribusId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySupertribusId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySupertribusId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySupertribusId(int id);
    #endregion Supertribus

    #region Tribus
    Tbl57Tribus GetTribusSingleByTribusIdAndHash(int id);

    ObservableCollection<Tbl57Tribus> CollTribussesByTribusId(int id);
    //direct children
    ObservableCollection<Tbl60Subtribus> CollSubtribussesByTribusIdAndHash(int id);
    //Function
    int SupertribusIdFromTribussesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl54Supertribus> CollSupertribussesBySupertribusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByTribusId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByTribusId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByTribusId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByTribusId(int id);
    #endregion Tribus

    #region Subtribus
    Tbl60Subtribus GetSubtribusSingleBySubtribusIdAndHash(int id);

    ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusId(int id);
    //direct children
    ObservableCollection<Tbl63Infratribus> CollInfratribussesBySubtribusIdAndHash(int id);
    //Function
    int TribusIdFromSubtribussesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl57Tribus> CollTribussesByTribusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubtribusId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubtribusId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubtribusId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubtribusId(int id);
    #endregion Subtribus

    #region Infratribus
    Tbl63Infratribus GetInfratribusSingleByInfratribusIdAndHash(int id);

    ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusId(int id);
    //direct children
    ObservableCollection<Tbl66Genus> CollGenussesByInfratribusIdAndHash(int id);
    //Function
    int SubtribusIdFromInfratribussesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl60Subtribus> CollSubtribussesBySubtribusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByInfratribusId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByInfratribusId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByInfratribusId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByInfratribusId(int id);
    #endregion Infratribus

    #region Genus
    Tbl66Genus GetGenusSingleByGenusIdAndHash(int id);

    ObservableCollection<Tbl66Genus> CollGenussesByGenusId(int id);
    //direct children
    ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndHash(int id);
    ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id);
    //ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndHash(int id);
    //ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id);
    //Function
    int InfratribusIdFromGenussesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl63Infratribus> CollInfratribussesByInfratribusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByGenusId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByGenusId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByGenusId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByGenusId(int id);
    #endregion Genus

    #region Species

    Tbl75Species GetSpeciesSingleBySpeciesIdAndHash(int id);
    Tbl75Species GetSpeciesSingleBySpeciesId(int speciesId);
    Tbl75Species GetSpeciesSingleModelBySpeciesId(int speciesId);
    ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesId(int speciesid);
    ObservableCollection<Tbl75Species> CollSpeciesSubsBySpeciesNameandGenusId(string name, int genusId);
    ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesNameAndNotEmptySubspecies(int id, string name);
    ObservableCollection<Tbl75Species> CollSpeciessesByGenusIdAndSpeciesNameAndEmptySubspecies(int id, string name);
    ObservableCollection<Tbl75Species> CollSpeciessesBySpeciesNameAndSubspeciesAndDivers(string speciesName, string subspecies,
        string divers);
    //direct children
    ObservableCollection<Tbl78Name> CollNamesBySpeciesIdAndHash(int id);
    ObservableCollection<Tbl84Synonym> CollSynonymsBySpeciesIdAndHash(int id);
    //Function
    int GenusIdFromSpeciessesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl66Genus> CollGenussesByGenusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySpeciesId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySpeciesId(int id);
    //Images
    ObservableCollection<Tbl81Image> CollImagesBySpeciesId(int id);
    //Geographics
    ObservableCollection<Tbl87Geographic> CollGeographicsBySpeciesId(int id);
    #endregion Species

    #region Disease

    TblDisease GetDiseaseSingleByDiseaseId(int id);

    ObservableCollection<TblDisease> CollDiseasesByDiseaseId(int id);

    #endregion Disease

    #region Food

    TblFood GetFoodSingleByFoodId(int id);
    ObservableCollection<TblFood> CollFoodsByFoodId(int id);


    #endregion Food

    #region Miscellaneou

    TblMiscellaneous GetMiscellaneousSingleByMiscellaneousId(int id);
    ObservableCollection<TblMiscellaneous> CollMiscellaneoussesByMiscellaneousId(int id);


    #endregion Miscellaneou

    #endregion Report_Gets

}
