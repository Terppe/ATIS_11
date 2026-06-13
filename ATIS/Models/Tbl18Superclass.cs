using System;
using System.ComponentModel.DataAnnotations;
using ATIS.Views.Base;

namespace ATIS.Models;

public class Tbl18Superclass : IHierarchicalEntity
{
    [Key]
    public int SuperclassId { get; set; }
    [Required(ErrorMessageResourceName = "RequiredSuperclassName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string SuperclassName { get; set; }
    [Required]
    public int SubphylumId { get; set; }
    //[Required]
    //public int SubdivisionId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Synonym { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Author { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string AuthorYear { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Info { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string EngName { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string GerName { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string FraName { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string PorName { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Memo { get; set; }
    //     public byte[] RowVersion { get; set; }

    public int GetEntityId()
    {
        throw new NotImplementedException();
    }

    public string GetDisplayName()
    {
        throw new NotImplementedException();
    }
}
