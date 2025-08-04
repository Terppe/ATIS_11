using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;

public class Tbl90RefSource
{
    [Key]
    public int RefSourceId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(300, ErrorMessageResourceName = "StringLength_300", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string RefSourceName { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string SourceYear { get; set; }

    public string FullName => $"{RefSourceName} {SourceYear}";
    public string RefSourceFullName => $"{RefSourceName} {SourceYear}";

    [StringLength(400, ErrorMessageResourceName = "StringLength_400", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Author { get; set; }
    public string Notes { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Info { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public System.DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public System.DateTime UpdaterDate { get; set; }
    public string Memo { get; set; }
    //   public byte[] RowVersion { get; set; }

    //    public virtual Tbl90Reference Tbl90Reference { get; set; }

}
