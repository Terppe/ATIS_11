using System;
using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;

public class Tbl57Tribus
{
    [Key]

    public int TribusId { get; set; }
    [Required(ErrorMessageResourceName = "RequiredTribusName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string TribusName { get; set; }
    [Required]
    public int SupertribusId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    public string Synonym { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Author { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string AuthorYear { get; set; }
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
    public string Memo { get; set; }
    //    public byte[] RowVersion { get; set; }

}
