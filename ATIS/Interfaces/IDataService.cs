using System.Collections.ObjectModel;
using ATIS.Models;

namespace ATIS.Interfaces;

public interface IDataService
{
    #region CRUD

    #region Regnum

    ObservableCollection<Tbl03Regnum> GetRegnumAsync();
    ObservableCollection<Tbl03Regnum> GetRegnumAsync(int regnumid);
    ObservableCollection<Tbl03Regnum> GetRegnumAsync(string search);
    Task<Tbl03Regnum> UpsertRegnumAsync(Tbl03Regnum regnum);
    Task DeleteRegnumAsync(int regnumid);

    #endregion Regnum

    #region Phylum

    ObservableCollection<Tbl06Phylum> GetPhylumAsync();
    ObservableCollection<Tbl06Phylum> GetPhylumAsync(int phylumid);
    ObservableCollection<Tbl06Phylum> GetPhylumAsync(string search);
    ObservableCollection<Tbl06Phylum> GetPhylumFromRegnumIdAsync(int regnumid);
    Task<Tbl06Phylum> UpsertPhylumAsync(Tbl06Phylum phylum);
    Task DeletePhylumAsync(int phylumid);

    #endregion Phylum

    #region Division                  

    ObservableCollection<Tbl09Division> GetDivisionAsync();
    ObservableCollection<Tbl09Division> GetDivisionAsync(int divisionid);
    ObservableCollection<Tbl09Division> GetDivisionAsync(string search);
    ObservableCollection<Tbl09Division> GetDivisionFromRegnumIdAsync(int regnumid);
    Task<Tbl09Division> UpsertDivisionAsync(Tbl09Division division);
    Task DeleteDivisionAsync(int divisionid);

    #endregion Division

    #region Subphylum

    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync();
    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(int subphylumid);
    ObservableCollection<Tbl12Subphylum> GetSubphylumAsync(string search);
    ObservableCollection<Tbl12Subphylum> GetSubphylumFromPhylumIdAsync(int phylumid);
    Task<Tbl12Subphylum> UpsertSubphylumAsync(Tbl12Subphylum subphylum);
    Task DeleteSubphylumAsync(int subphylumid);

    #endregion Subphylum

    #region Subdivision

    ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync();
    ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync(int subdivisionid);
    ObservableCollection<Tbl15Subdivision> GetSubdivisionAsync(string search);
    ObservableCollection<Tbl15Subdivision> GetSubdivisionFromDivisionIdAsync(int divisionid);
    Task<Tbl15Subdivision> UpsertSubdivisionAsync(Tbl15Subdivision subdivision);
    Task DeleteSubdivisionAsync(int subdivisionid);

    #endregion Subdivision

    #region Superclass

    ObservableCollection<Tbl18Superclass> GetSuperclassAsync();
    ObservableCollection<Tbl18Superclass> GetSuperclassAsync(int superclassid);
    ObservableCollection<Tbl18Superclass> GetSuperclassAsync(string search);
    ObservableCollection<Tbl18Superclass> GetSuperclassFromSubphylumIdAsync(int subphylumid);
    Task<Tbl18Superclass> UpsertSuperclassAsync(Tbl18Superclass superclass);
    Task DeleteSuperclassAsync(int superclassid);

    #endregion Superclass

    #region Class                  

    ObservableCollection<Tbl21Class> GetClassAsync();
    ObservableCollection<Tbl21Class> GetClassAsync(int classid);
    ObservableCollection<Tbl21Class> GetClassAsync(string search);
    ObservableCollection<Tbl21Class> GetClassFromSuperclassIdAsync(int superclassid);
    Task<Tbl21Class> UpsertClassAsync(Tbl21Class class1);
    Task DeleteClassAsync(int classid);

    #endregion Class

    #region Subclass                  

    ObservableCollection<Tbl24Subclass> GetSubclassAsync();
    ObservableCollection<Tbl24Subclass> GetSubclassAsync(int subclassid);
    ObservableCollection<Tbl24Subclass> GetSubclassAsync(string search);
    ObservableCollection<Tbl24Subclass> GetSubclassFromClassIdAsync(int classeid);
    Task<Tbl24Subclass> UpsertSubclassAsync(Tbl24Subclass subclass);
    Task DeleteSubclassAsync(int subclassid);

    #endregion Subclass

    #region Infraclass                  

    ObservableCollection<Tbl27Infraclass> GetInfraclassAsync();
    ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(int infraclassid);
    ObservableCollection<Tbl27Infraclass> GetInfraclassAsync(string search);
    ObservableCollection<Tbl27Infraclass> GetInfraclassFromSubclassIdAsync(int subclassid);
    Task<Tbl27Infraclass> UpsertInfraclassAsync(Tbl27Infraclass infraclass);
    Task DeleteInfraclassAsync(int infraclassid);

    #endregion Infraclass

    #region Legio                  

    ObservableCollection<Tbl30Legio> GetLegioAsync();
    ObservableCollection<Tbl30Legio> GetLegioAsync(int legioid);
    ObservableCollection<Tbl30Legio> GetLegioAsync(string search);
    ObservableCollection<Tbl30Legio> GetLegioFromInfraclassIdAsync(int infraclassid);
    Task<Tbl30Legio> UpsertLegioAsync(Tbl30Legio legio);
    Task DeleteLegioAsync(int legioid);

    #endregion Legio

    #region Ordo                  

    ObservableCollection<Tbl33Ordo> GetOrdoAsync();
    ObservableCollection<Tbl33Ordo> GetOrdoAsync(int ordoid);
    ObservableCollection<Tbl33Ordo> GetOrdoAsync(string search);
    ObservableCollection<Tbl33Ordo> GetOrdoFromLegioIdAsync(int legioid);
    Task<Tbl33Ordo> UpsertOrdoAsync(Tbl33Ordo ordo);
    Task DeleteOrdoAsync(int ordoid);

    #endregion Ordo

    #region Subordo                  

    ObservableCollection<Tbl36Subordo> GetSubordoAsync();
    ObservableCollection<Tbl36Subordo> GetSubordoAsync(int subordoid);
    ObservableCollection<Tbl36Subordo> GetSubordoAsync(string search);
    ObservableCollection<Tbl36Subordo> GetSubordoFromOrdoIdAsync(int ordoid);
    Task<Tbl36Subordo> UpsertSubordoAsync(Tbl36Subordo subordo);
    Task DeleteSubordoAsync(int subordoid);

    #endregion Subordo

    #region Infraordo                  

    ObservableCollection<Tbl39Infraordo> GetInfraordoAsync();
    ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(int infraordoid);
    ObservableCollection<Tbl39Infraordo> GetInfraordoAsync(string search);
    ObservableCollection<Tbl39Infraordo> GetInfraordoFromSubordoIdAsync(int subordoid);
    Task<Tbl39Infraordo> UpsertInfraordoAsync(Tbl39Infraordo infraordo);
    Task DeleteInfraordoAsync(int infraordoid);

    #endregion Infraordo

    #region Superfamily                  

    ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync();
    ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(int superfamilyid);
    ObservableCollection<Tbl42Superfamily> GetSuperfamilyAsync(string search);
    ObservableCollection<Tbl42Superfamily> GetSuperfamilyFromInfraordoIdAsync(int infraordoid);
    Task<Tbl42Superfamily> UpsertSuperfamilyAsync(Tbl42Superfamily superfamily);
    Task DeleteSuperfamilyAsync(int superfamilyid);

    #endregion Superfamily

    #region Family                  

    ObservableCollection<Tbl45Family> GetFamilyAsync();
    ObservableCollection<Tbl45Family> GetFamilyAsync(int familyid);
    ObservableCollection<Tbl45Family> GetFamilyAsync(string search);
    ObservableCollection<Tbl45Family> GetFamilyFromSuperfamilyIdAsync(int superfamilyid);
    Task<Tbl45Family> UpsertFamilyAsync(Tbl45Family family);
    Task DeleteFamilyAsync(int familyid);

    #endregion Family

    #region Subfamily                  

    ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync();
    ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(int subfamilyid);
    ObservableCollection<Tbl48Subfamily> GetSubfamilyAsync(string search);
    ObservableCollection<Tbl48Subfamily> GetSubfamilyFromFamilyIdAsync(int familyid);
    Task<Tbl48Subfamily> UpsertSubfamilyAsync(Tbl48Subfamily subfamily);
    Task DeleteSubfamilyAsync(int subfamilyid);

    #endregion Subfamily

    #region Infrafamily                  

    ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync();
    ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(int infrafamilyid);
    ObservableCollection<Tbl51Infrafamily> GetInfrafamilyAsync(string search);
    ObservableCollection<Tbl51Infrafamily> GetInfrafamilyFromSubfamilyIdAsync(int subfamilyid);
    Task<Tbl51Infrafamily> UpsertInfrafamilyAsync(Tbl51Infrafamily infrafamily);
    Task DeleteInfrafamilyAsync(int infrafamilyid);

    #endregion Infrafamily

    #region Supertribus                  

    ObservableCollection<Tbl54Supertribus> GetSupertribusAsync();
    ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(int supertribusid);
    ObservableCollection<Tbl54Supertribus> GetSupertribusAsync(string search);
    ObservableCollection<Tbl54Supertribus> GetSupertribusFromInfrafamilyIdAsync(int infrafamilyid);
    Task<Tbl54Supertribus> UpsertSupertribusAsync(Tbl54Supertribus supertribus);
    Task DeleteSupertribusAsync(int supertribusid);

    #endregion Supertribus

    #region Tribus                  

    ObservableCollection<Tbl57Tribus> GetTribusAsync();
    ObservableCollection<Tbl57Tribus> GetTribusAsync(int tribusid);
    ObservableCollection<Tbl57Tribus> GetTribusAsync(string search);
    ObservableCollection<Tbl57Tribus> GetTribusFromSupertribusIdAsync(int supertribusid);
    Task<Tbl57Tribus> UpsertTribusAsync(Tbl57Tribus tribus);
    Task DeleteTribusAsync(int tribusid);

    #endregion Tribus

    #region Subtribus                  

    ObservableCollection<Tbl60Subtribus> GetSubtribusAsync();
    ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(int subtribusid);
    ObservableCollection<Tbl60Subtribus> GetSubtribusAsync(string search);
    ObservableCollection<Tbl60Subtribus> GetSubtribusFromTribusIdAsync(int tribusid);
    Task<Tbl60Subtribus> UpsertSubtribusAsync(Tbl60Subtribus subtribus);
    Task DeleteSubtribusAsync(int subtribusid);

    #endregion Subtribus

    #region Infratribus                  

    ObservableCollection<Tbl63Infratribus> GetInfratribusAsync();
    ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(int infratribusid);
    ObservableCollection<Tbl63Infratribus> GetInfratribusAsync(string search);
    ObservableCollection<Tbl63Infratribus> GetInfratribusFromSubtribusIdAsync(int subtribusid);
    Task<Tbl63Infratribus> UpsertInfratribusAsync(Tbl63Infratribus infratribus);
    Task DeleteInfratribusAsync(int infratribusid);

    #endregion Infratribus

    #region Genus                  

    ObservableCollection<Tbl66Genus> GetGenusAsync();
    ObservableCollection<Tbl66Genus> GetGenusAsync(int genusid);
    ObservableCollection<Tbl66Genus> GetGenusAsync(string search);
    ObservableCollection<Tbl66Genus> GetGenusFromInfratribusIdAsync(int infratribusid);
    Task<Tbl66Genus> UpsertGenusAsync(Tbl66Genus genus);
    Task DeleteGenusAsync(int genusid);

    #endregion Genus

    #region Speciesgroup                  

    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync();
    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(int speciesgroupid);
    ObservableCollection<Tbl68Speciesgroup> GetSpeciesgroupAsync(string search);
    Task<Tbl68Speciesgroup> UpsertSpeciesgroupAsync(Tbl68Speciesgroup speciesgroup);
    Task DeleteSpeciesgroupAsync(int speciesgroupid);

    #endregion Speciesgroup

    #region FiSpecies                  

    ObservableCollection<Tbl69FiSpecies> GetSpeciesAsync();
    ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync();
    ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync(int fispeciesid);
    ObservableCollection<Tbl69FiSpecies> GetFiSpeciesAsync(string search);
    ObservableCollection<Tbl69FiSpecies> GetFiSpeciesFromGenusNameAsync(string search);
    ObservableCollection<Tbl69FiSpecies> GetFiSpeciesFromGenusIdAsync(int genusid);
    Task<Tbl69FiSpecies> UpsertFiSpeciesAsync(Tbl69FiSpecies fispecies);
    Task DeleteFiSpeciesAsync(int fispeciesid);

    #endregion FiSpecies

    #region PlSpecies                  

    ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync();
    ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync(int plspeciesid);
    ObservableCollection<Tbl72PlSpecies> GetPlSpeciesAsync(string search);
    ObservableCollection<Tbl72PlSpecies> GetPlSpeciesFromGenusNameAsync(string search);
    ObservableCollection<Tbl72PlSpecies> GetPlSpeciesFromGenusIdAsync(int genusid);
    Task<Tbl72PlSpecies> UpsertPlSpeciesAsync(Tbl72PlSpecies plspecies);
    Task DeletePlSpeciesAsync(int plspeciesid);

    #endregion PlSpecies

    #region Name                  

    ObservableCollection<Tbl78Name> GetNameAsync();
    ObservableCollection<Tbl78Name> GetNameAsync(int nameid);
    ObservableCollection<Tbl78Name> GetNameAsync(string search);
    ObservableCollection<Tbl78Name> GetNameFromFiSpeciesIdAsync(int fispeciesid);
    ObservableCollection<Tbl78Name> GetNameFromPlSpeciesIdAsync(int plspeciesid);
    Task<Tbl78Name> UpsertNameAsync(Tbl78Name name);
    Task DeleteNameAsync(int nameid);

    #endregion Name

    #region Image                  

    ObservableCollection<Tbl81Image> GetImageAsync();
    ObservableCollection<Tbl81Image> GetImageAsync(int imageid);
    ObservableCollection<Tbl81Image> GetImageAsync(string search);
    ObservableCollection<Tbl81Image> GetImageFromFiSpeciesIdAsync(int fispeciesid);
    ObservableCollection<Tbl81Image> GetImageFromPlSpeciesIdAsync(int plspeciesid);
    Task<Tbl81Image> UpsertImageAsync(Tbl81Image image, string selectedpath);
    Task DeleteImageAsync(int imageid);

    #endregion Image                  

    #region Synonym                  

    ObservableCollection<Tbl84Synonym> GetSynonymAsync();
    ObservableCollection<Tbl84Synonym> GetSynonymAsync(int synonymid);
    ObservableCollection<Tbl84Synonym> GetSynonymAsync(string search);
    ObservableCollection<Tbl84Synonym> GetSynonymFromFiSpeciesIdAsync(int fispeciesid);
    ObservableCollection<Tbl84Synonym> GetSynonymFromPlSpeciesIdAsync(int plspeciesid);
    Task<Tbl84Synonym> UpsertSynonymAsync(Tbl84Synonym synonym);
    Task DeleteSynonymAsync(int synonymid);

    #endregion Synonym

    #region Geographic                  

    ObservableCollection<Tbl87Geographic> GetGeographicAsync();
    ObservableCollection<Tbl87Geographic> GetGeographicAsync(int geographicid);
    ObservableCollection<Tbl87Geographic> GetGeographicAsync(string search);
    ObservableCollection<Tbl87Geographic> GetGeographicFromFiSpeciesIdAsync(int fispeciesid);
    ObservableCollection<Tbl87Geographic> GetGeographicFromPlSpeciesIdAsync(int plspeciesid);
    Task<Tbl87Geographic> UpsertGeographicAsync(Tbl87Geographic geographic);
    Task DeleteGeographicAsync(int geographicid);

    #endregion Geographic                  


    #region Reference
    ObservableCollection<Tbl90Reference> GetReferenceAsync();
    ObservableCollection<Tbl90Reference> GetReferenceAsync(int referenceid);
    Task<ObservableCollection<Tbl90Reference>> GetReferenceAsync(string search);
    Task<Tbl90Reference> UpsertReferenceAsync(Tbl90Reference reference);
    Task DeleteReferenceAsync(int referenceid);

    //-----------------------------------------------------------
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceAuthorsCollectionOrderByInfoFromGenusIdAndRefSourceIdIsNullAndRefExpertIdIsNull(int? genusId);
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceSourcesCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefExpertIdIsNull(int? genusId);
    ObservableCollection<Tbl90Reference> GetTbl90ReferenceExpertsCollectionOrderByInfoFromGenusIdAndRefAuthorIdIsNullAndRefSourceIdIsNull(int? genusId);

    #endregion Reference

    #region RefAuthor

    ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync();
    ObservableCollection<Tbl90RefAuthor> GetRefAuthorAsync(int refauthorid);
    Task<ObservableCollection<Tbl90RefAuthor>> GetRefAuthorAsync(string search);
    Task<Tbl90RefAuthor> UpsertRefAuthorAsync(Tbl90RefAuthor refauthor);
    Task DeleteRefAuthorAsync(int refauthorid);

    #endregion RefAuthor

    #region RefSource

    ObservableCollection<Tbl90RefSource> GetRefSourceAsync();
    ObservableCollection<Tbl90RefSource> GetRefSourceAsync(int refsourceid);
    Task<ObservableCollection<Tbl90RefSource>> GetRefSourceAsync(string search);
    Task<Tbl90RefSource> UpsertRefSourceAsync(Tbl90RefSource refsource);
    Task DeleteRefSourceAsync(int refsourceid);


    #endregion RefSource

    #region RefExpert
    ObservableCollection<Tbl90RefExpert> GetRefExpertAsync();
    ObservableCollection<Tbl90RefExpert> GetRefExpertAsync(int refexpertid);
    Task<ObservableCollection<Tbl90RefExpert>> GetRefExpertAsync(string search);
    Task<Tbl90RefExpert> UpsertRefExpertAsync(Tbl90RefExpert refexpert);
    Task DeleteRefExpertAsync(int refexpertid);

    #endregion RefExpert

    #region Comment
    ObservableCollection<Tbl93Comment> GetCommentAsync();
    ObservableCollection<Tbl93Comment> GetCommentAsync(int commentid);
    Task<ObservableCollection<Tbl93Comment>> GetCommentAsync(string search);
    Task<Tbl93Comment> UpsertCommentAsync(Tbl93Comment comment);
    Task DeleteCommentAsync(int commentid);

    #endregion Comment

    #region Country

    ObservableCollection<TblCountry> GetCountryAsync();
    ObservableCollection<TblCountry> GetCountryAsync(int countryid);
    Task<ObservableCollection<TblCountry>> GetCountryAsync(string search);
    Task<TblCountry> UpsertCountryAsync(TblCountry country);
    Task DeleteCountryAsync(int countryid);

    #endregion Country

    #region UserProfile

    ObservableCollection<TblUserProfile> GetUserProfileAsync();
    ObservableCollection<TblUserProfile> GetUserProfileAsync(int userprofileid);
    ObservableCollection<TblUserProfile> GetEmailFromUserProfileAsync(string email);
    Task<ObservableCollection<TblUserProfile>> GetUserProfileAsync(string search);
    Task<TblUserProfile> UpsertUserProfileAsync(TblUserProfile userprofile);
    Task<TblUserProfile> UpsertEmailFromUserProfileAsync(TblUserProfile userprofile);
    Task DeleteUserProfileAsync(int userprofileid);

    #endregion UserProfile


    #endregion CRUD

    #region Search_Gets

    ObservableCollection<Tbl03Regnum> GetTbl03RegnumsCollectionOrderByRegnumNameAndSubregnumFromFilterText(string filterText);
    ObservableCollection<Tbl06Phylum> GetTbl06PhylumsCollectionOrderByPhylumNameFromFilterText(string filterText);
    ObservableCollection<Tbl09Division> GetTbl09DivisionsCollectionOrderByDivisionNameFromFilterText(string filterText);
    ObservableCollection<Tbl12Subphylum> GetTbl12SubphylumsCollectionOrderBySubphylumNameFromFilterText(string filterText);
    ObservableCollection<Tbl15Subdivision> GetTbl15SubdivisionsCollectionOrderBySubdivisionNameFromFilterText(string filterText);
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
    ObservableCollection<Tbl69FiSpecies> GetTbl69FiSpeciessesCollectionOrderByGenusNameAndFiSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText);
    ObservableCollection<Tbl72PlSpecies> GetTbl72PlSpeciessesCollectionOrderByGenusNameAndPlSpeciesNameAndSubspeciesAndDiversFromFilterText(string filterText);
    ObservableCollection<Tbl78Name> GetTbl78NamesCollectionOrderByNameNameFromFilterText(string filterText);
    ObservableCollection<Tbl84Synonym> GetTbl84SynonymsCollectionOrderBySynonymNameFromFilterText(string filterText);


    #endregion Search_Gets

    #region Report_Gets

    #region Regnum
    Tbl03Regnum GetRegnumSingleByRegnumIdAndHash(int id);
    Tbl03Regnum GetRegnumSingleByRegnumId(int id);

    ObservableCollection<Tbl03Regnum> CollRegnumsByRegnumId(int id);
    //direct children
    ObservableCollection<Tbl06Phylum> CollPhylumsByRegnumIdAndHash(int id);
    ObservableCollection<Tbl09Division> CollDivisionsByRegnumIdAndHash(int id);
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

    #region Division
    Tbl09Division GetDivisionSingleByDivisionIdAndHash(int id);
    Tbl09Division GetDivisionSingleByDivisionId(int id);

    ObservableCollection<Tbl09Division> CollDivisionsByDivisionId(int id);
    //direct children
    ObservableCollection<Tbl15Subdivision> CollSubdivisionsByDivisionIdAndHash(int id);
    //Function
    int RegnumIdFromDivisionsCollectionSelect(int id);
    //ForeignKey
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByDivisionId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByDivisionId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByDivisionId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByDivisionId(int id);
    #endregion Division

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

    #region Subdivision
    Tbl15Subdivision GetSubdivisionSingleBySubdivisionIdAndHash(int id);
    Tbl15Subdivision GetSubdivisionSingleBySubdivisionId(int id);

    ObservableCollection<Tbl15Subdivision> CollSubdivisionsBySubdivisionId(int id);
    //direct children
    ObservableCollection<Tbl18Superclass> CollSuperclassesBySubdivisionIdAndHash(int id);
    //Function
    int DivisionIdFromSubdivisionsCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl09Division> CollDivisionsByDivisionIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsBySubdivisionId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesBySubdivisionId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsBySubdivisionId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsBySubdivisionId(int id);
    #endregion Subdivision

    #region Superclass
    Tbl18Superclass GetSuperclassSingleBySuperclassIdAndHash(int id);
    ObservableCollection<Tbl18Superclass> CollSuperclassesBySuperclassId(int id);
    //direct children
    ObservableCollection<Tbl21Class> CollClassesBySuperclassIdAndHash(int id);
    //Function
    int SubdivisionIdFromSuperclassesCollectionSelect(int id);
    int SubphylumIdFromSuperclassesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl12Subphylum> CollSubphylumsBySubphylumIdAndHash(int id);
    ObservableCollection<Tbl15Subdivision> CollSubdivisionsBySubdivisionIdAndHash(int id);
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
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndHash(int id);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndHash(int id);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndSubspeciesIsNullAndHash(int id);
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

    #region FiSpecies
    Tbl69FiSpecies GetFiSpeciesSingleByFiSpeciesIdAndHash(int id);
    Tbl69FiSpecies GetFiSpeciesSingleByFiSpeciesId(int fispeciesId);
    Tbl69FiSpecies GetFiSpeciesSingleModelByFiSpeciesId(int fispeciesId);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesId(int id);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciesSubsByFiSpeciesNameandGenusId(string name, int genusId);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesNameAndNotEmptySubspecies(int id, string name);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByGenusIdAndFiSpeciesNameAndEmptySubspecies(int id, string name);
    ObservableCollection<Tbl69FiSpecies> CollFiSpeciessesByFiSpeciesNameAndSubspeciesAndDivers(string fiSpeciesName, string subspecies,
        string divers);
    //direct children
    ObservableCollection<Tbl78Name> CollNamesByFiSpeciesIdAndHash(int id);
    ObservableCollection<Tbl84Synonym> CollSynonymsByFiSpeciesIdAndHash(int id);
    //Function
    int GenusIdFromFiSpeciessesCollectionSelect(int id);
    //ForeignKey
    ObservableCollection<Tbl66Genus> CollGenussesByGenusIdAndHash(int id);
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByFiSpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByFiSpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByFiSpeciesId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByFiSpeciesId(int id);
    //Images
    ObservableCollection<Tbl81Image> CollImagesByFiSpeciesId(int id);
    //Geographics
    ObservableCollection<Tbl87Geographic> CollGeographicsByFiSpeciesId(int id);
    #endregion FiSpecies

    #region PlSpecies
    Tbl72PlSpecies GetPlSpeciesSingleByPlSpeciesIdAndHash(int id);
    Tbl72PlSpecies GetPlSpeciesSingleByPlSpeciesId(int plspeciesId);
    Tbl72PlSpecies GetPlSpeciesSingleModelByPlSpeciesId(int plspeciesId);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesId(int id);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciesSubsByPlSpeciesNameandGenusId(string name, int genusId);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesNameAndNotEmptySubspecies(int id, string name);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByGenusIdAndPlSpeciesNameAndEmptySubspecies(int id, string name);
    ObservableCollection<Tbl72PlSpecies> CollPlSpeciessesByPlSpeciesNameAndSubspeciesAndDivers(string plSpeciesName, string subspecies,
        string divers);

    //direct children
    ObservableCollection<Tbl78Name> CollNamesByPlSpeciesIdAndHash(int id);
    ObservableCollection<Tbl84Synonym> CollSynonymsByPlSpeciesIdAndHash(int id);
    //Function
    int GenusIdFromPlSpeciessesCollectionSelect(int id);
    //ForeignKey
    //References
    ObservableCollection<Tbl90Reference> CollExpertsByPlSpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollSourcesByPlSpeciesId(int id);
    ObservableCollection<Tbl90Reference> CollAuthorsByPlSpeciesId(int id);
    //Comments
    ObservableCollection<Tbl93Comment> CollCommentsByPlSpeciesId(int id);
    //Images
    ObservableCollection<Tbl81Image> CollImagesByPlSpeciesId(int id);
    //Geographics
    ObservableCollection<Tbl87Geographic> CollGeographicsByPlSpeciesId(int id);
    #endregion PlSpecies

    #endregion Report_Gets

}
