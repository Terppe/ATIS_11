using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;

public class Tbl90RefAuthor
{

    [Key]
    public int RefAuthorId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string RefAuthorName { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string PublicationYear { get; set; }
    [StringLength(300, ErrorMessageResourceName = "StringLength_300", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ArticelTitle { get; set; }
    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string BookName { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Page { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Page1 { get; set; }

    public string FullName => $"{RefAuthorName} {BookName} {Page1}";
    public string RefAuthorFullName => $"{RefAuthorName} {BookName} {Page1}";

    [StringLength(200, ErrorMessageResourceName = "StringLength_200", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Publisher { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string PublicationPlace { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ISBN { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Notes { get; set; }
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
    //    public byte[] RowVersion { get; set; }

    //   public virtual ICollection<Tbl90Reference> Tbl90References { get; set; }

    //    public virtual Tbl90Reference Tbl90Reference { get; set; }


}
