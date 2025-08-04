using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATIS.Models;

public class Tbl90Reference
{
    [Key]
    public int ReferenceId { get; set; }
    public int? FiSpeciesId { get; set; }
    public int? PlSpeciesId { get; set; }
    public int? GenusId { get; set; }
    public int? InfratribusId { get; set; }
    public int? SubtribusId { get; set; }
    public int? TribusId { get; set; }
    public int? SupertribusId { get; set; }
    public int? InfrafamilyId { get; set; }
    public int? SubfamilyId { get; set; }
    public int? FamilyId { get; set; }
    public int? SuperfamilyId { get; set; }
    public int? InfraordoId { get; set; }
    public int? SubordoId { get; set; }
    public int? OrdoId { get; set; }
    public int? LegioId { get; set; }
    public int? InfraclassId { get; set; }
    public int? SubclassId { get; set; }
    public int? ClassId { get; set; }
    public int? SuperclassId { get; set; }
    public int? SubdivisionId { get; set; }
    public int? SubphylumId { get; set; }
    public int? DivisionId { get; set; }
    public int? PhylumId { get; set; }
    public int? RegnumId { get; set; }

    //Änderung über Herrn Nolte Microsoft 9.2.2022
    //  Support Escalation Engineer Office: +49 (89) 31764337 
    // martinn @microsoft.com
    public int? RefExpertId { get; set; }
    public void SetRefExpertId(object obj) => RefExpertId = (int)obj;

    public int? RefSourceId { get; set; }

    public void SetRefSourceId(object obj) => RefSourceId = (int)obj;

    public int? RefAuthorId { get; set; }

    public void SetRefAuthorId(object obj) => RefAuthorId = (int)obj;


    public int CountId { get; set; }
    public bool Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Info { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Memo { get; set; }
    //     public byte[] RowVersion { get; set; }

    //public virtual Tbl03Regnum Tbl03Regnums { get; set; }
    //public virtual Tbl06Phylum Tbl06Phylums { get; set; }
    //public virtual Tbl09Division Tbl09Divisions { get; set; }
    //public virtual Tbl12Subphylum Tbl12Subphylums { get; set; }
    //public virtual Tbl15Subdivision Tbl15Subdivisions { get; set; }
    [ForeignKey("FiSpeciesId")]
    public virtual Tbl69FiSpecies Tbl69FiSpeciesses { get; set; }
    [ForeignKey("PlSpeciesId")]
    public virtual Tbl72PlSpecies Tbl72PlSpeciesses { get; set; }

    [ForeignKey("RefAuthorId")]
    public virtual Tbl90RefAuthor Tbl90RefAuthors { get; set; }
    [ForeignKey("RefExpertId")]
    public virtual Tbl90RefExpert Tbl90RefExperts { get; set; }
    [ForeignKey("RefSourceId")]
    public virtual Tbl90RefSource Tbl90RefSources { get; set; }


}
