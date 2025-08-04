using System;
using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;


public class Tbl90RefExpert
{
    [Key]
    public int RefExpertId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(300, ErrorMessageResourceName = "StringLength_300", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string RefExpertName { get; set; }
    public string Notes { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Info { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    public string Memo { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public virtual Tbl90Reference Tbl90Reference { get; set; }

}
