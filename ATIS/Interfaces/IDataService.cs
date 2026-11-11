using ATIS.Models;

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



    #region CRUD

    #region Regnum

    // ---------------------------------------------------------                  
    // Regnum 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl03Regnum>> GetAllRegnumAsync();
    Task<IEnumerable<Tbl03Regnum>> SearchRegnumAsync(string? search);
    Task<Tbl03Regnum?> GetRegnumByIdAsync(int id);
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

    #endregion Phylum

    #region Subphylum

    // ---------------------------------------------------------                  
    // Subphylum 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl12Subphylum>> GetAllSubphylumAsync();
    Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumWithPhylumAsync(string? search);
    Task<Tbl12Subphylum?> GetSubphylumByIdAsync(int id);
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

    #endregion Subphylum

    #region Superclass

    // ---------------------------------------------------------                  
    // Superclass 
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl18Superclass>> GetAllSuperclassAsync();
    Task<IEnumerable<Tbl18Superclass>> SearchSuperclassWithSubphylumAsync(string? search);
    Task<Tbl18Superclass?> GetSuperclassByIdAsync(int id);
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

    #endregion Superclass

    #region Class
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl21Class>> GetAllClassAsync();
    Task<IEnumerable<Tbl21Class>> SearchClassWithSuperclassAsync(string? search);
    Task<Tbl21Class?> GetClassByIdAsync(int id);
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

    #endregion Class

    #region Subclass
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl24Subclass>> GetAllSubclassAsync();
    Task<IEnumerable<Tbl24Subclass>> SearchSubclassWithClassAsync(string? search);
    Task<Tbl24Subclass?> GetSubclassByIdAsync(int id);
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

    #endregion Subclass

    #region Infraclass                  
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl27Infraclass>> GetAllInfraclassAsync();
    Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassWithSubclassAsync(string? search);
    Task<Tbl27Infraclass?> GetInfraclassByIdAsync(int id);
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
    Task<Tbl42Superfamily> GetSuperfamilyByIdAsync(int? id);
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
    Task<Tbl48Subfamily> GetSubfamilyByIdAsync(int? id);
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

    #endregion Species

    #region Name
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl78Name>> GetAllNameAsync();
    Task<IEnumerable<Tbl78Name>> SearchNameWithSpeciesAndGenusAsync(string? search);
    Task<Tbl78Name?> GetNameByIdAsync(int id);
    Task<Tbl78Name> UpsertNameAsync(Tbl78Name entity);
    Task DeleteNameAsync(int id);
    Task<bool> NameExistsAsync(Tbl78Name entity);

    Task<IEnumerable<Tbl78Name>> GetNameForSpeciesAsync(int speciesId);

    #endregion Name

    #region Image
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl81Image>> GetAllImageAsync();
    Task<IEnumerable<Tbl81Image>> SearchImageWithSpeciesAndGenusAsync(string? search);
    Task<Tbl81Image?> GetImageByIdAsync(int id);
    Task<Tbl81Image> UpsertImageAsync(Tbl81Image entity);
    Task DeleteImageAsync(int id);
    Task<bool> ImageExistsAsync(Tbl81Image entity);

    Task<IEnumerable<Tbl81Image>> GetImageForSpeciesAsync(int speciesId);
    Task<IEnumerable<Tbl81Image>> GetImageForFoodAsync(int foodId);
    Task<IEnumerable<Tbl81Image>> GetImageForDiseaseAsync(int diseaseId);
    Task<IEnumerable<Tbl81Image>> GetImageForMiscellaneousAsync(int miscellaneousId);

    #endregion Image                  

    #region Synonym
    // ---------------------------------------------------------   
    Task<IEnumerable<Tbl84Synonym>> GetAllSynonymAsync();
    Task<IEnumerable<Tbl84Synonym>> SearchSynonymWithSpeciesAndGenusAsync(string? search);
    Task<Tbl84Synonym?> GetSynonymByIdAsync(int id);
    Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym entity);
    Task DeleteSynonymAsync(int id);
    Task<bool> SynonymExistsAsync(Tbl84Synonym entity);

    Task<IEnumerable<Tbl84Synonym>> GetSynonymForSpeciesAsync(int speciesId);

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

    Task<IEnumerable<Tbl87Geographic>> GetGeographicForSpeciesAsync(int speciesId);

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

    #endregion RefExpert

    #region Comment
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

    #endregion Country

    #region UserProfile
    // ---------------------------------------------------------   
    Task<IEnumerable<TblUserProfile>> GetAllUserProfileAsync();
    Task<IEnumerable<TblUserProfile>> SearchUserProfileAsync(string? search);
    Task<TblUserProfile?> GetUserProfileByIdAsync(int id);
    Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile entity);
    Task DeleteUserProfileAsync(int id);
    Task<bool> UserProfileExistsAsync(TblUserProfile entity);

    #endregion UserProfile

    #region Food
    // ---------------------------------------------------------   
    Task<IEnumerable<TblFood>> GetAllFoodAsync();
    Task<IEnumerable<TblFood>> SearchFoodAsync(string? search);
    Task<TblFood?> GetFoodByIdAsync(int id);
    Task<TblFood> UpsertFoodAsync(TblFood entity);
    Task DeleteFoodAsync(int id);
    Task<bool> FoodExistsAsync(TblFood entity);

    #endregion Food

    #region Disease
    // ---------------------------------------------------------   
    Task<IEnumerable<TblDisease>> GetAllDiseaseAsync();
    Task<IEnumerable<TblDisease>> SearchDiseaseAsync(string? search);
    Task<TblDisease?> GetDiseaseByIdAsync(int id);
    Task<TblDisease> UpsertDiseaseAsync(TblDisease entity);
    Task DeleteDiseaseAsync(int id);
    Task<bool> DiseaseExistsAsync(TblDisease entity);

    #endregion Disease

    #region Miscellaneous
    // ---------------------------------------------------------   
    Task<IEnumerable<TblMiscellaneous>> GetAllMiscellaneousAsync();
    Task<IEnumerable<TblMiscellaneous>> SearchMiscellaneousAsync(string? search);
    Task<TblMiscellaneous?> GetMiscellaneousByIdAsync(int id);
    Task<TblMiscellaneous> UpsertMiscellaneousAsync(TblMiscellaneous entity);
    Task DeleteMiscellaneousAsync(int id);
    Task<bool> MiscellaneousExistsAsync(TblMiscellaneous entity);

    #endregion Miscellaneous

    #endregion CRUD

    #region REPORT
    Task<IEnumerable<Tbl03Regnum>> SearchRegnumForReportAsync(string? search);

    Task<IEnumerable<Tbl06Phylum>> SearchPhylumForReportAsync(string? search);
    Task<IEnumerable<Tbl12Subphylum>> SearchSubphylumForReportAsync(string? search);

    Task<IEnumerable<Tbl18Superclass>> SearchSuperclassForReportAsync(string? search);
    Task<IEnumerable<Tbl21Class>> SearchClassForReportAsync(string? search);
    Task<IEnumerable<Tbl24Subclass>> SearchSubclassForReportAsync(string? search);
    Task<IEnumerable<Tbl27Infraclass>> SearchInfraclassForReportAsync(string? search);

    Task<IEnumerable<Tbl30Legio>> SearchLegioForReportAsync(string? search);
    Task<IEnumerable<Tbl33Ordo>> SearchOrdoForReportAsync(string? search);
    Task<IEnumerable<Tbl36Subordo>> SearchSubordoForReportAsync(string? search);
    Task<IEnumerable<Tbl39Infraordo>> SearchInfraordoForReportAsync(string? search);

    Task<IEnumerable<Tbl42Superfamily>> SearchSuperfamilyForReportAsync(string? search);
    Task<IEnumerable<Tbl45Family>> SearchFamilyForReportAsync(string? search);
    Task<IEnumerable<Tbl48Subfamily>> SearchSubfamilyForReportAsync(string? search);
    Task<IEnumerable<Tbl51Infrafamily>> SearchInfrafamilyForReportAsync(string? search);

    Task<IEnumerable<Tbl54Supertribus>> SearchSupertribusForReportAsync(string? search);
    Task<IEnumerable<Tbl57Tribus>> SearchTribusForReportAsync(string? search);
    Task<IEnumerable<Tbl60Subtribus>> SearchSubtribusForReportAsync(string? search);
    Task<IEnumerable<Tbl63Infratribus>> SearchInfratribusForReportAsync(string? search);

    Task<IEnumerable<Tbl66Genus>> SearchGenusForReportAsync(string? search);
    Task<IEnumerable<Tbl75Species>> SearchSpeciesForReportAsync(string? search);

    #endregion REPORT

}
