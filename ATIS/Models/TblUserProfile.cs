using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;

public class TblUserProfile
{
    [Key]
    public int UserProfileId { get; set; }
    [Required(ErrorMessageResourceName = "RequiredEmail", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Email { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string LastName { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string FirstName { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Title { get; set; }
    [Required(ErrorMessageResourceName = "RequiredPassword", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(250, ErrorMessageResourceName = "StringLength_250", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Password { get; set; }
    [Required(ErrorMessageResourceName = "RequiredRole", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Role { get; set; }
    public bool Flag { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Notes { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Colour { get; set; }
    public int CountId { get; set; }
    public DateTime BirthDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Gender { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Country { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Postcode { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string City { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Street1 { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Street2 { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Tel { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Mobil { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Fax { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string HomePageUrl { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Business { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Company { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Memo { get; set; }
    public byte[] Filestream { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ImageMimeType { get; set; }
    public Guid FilestreamId { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Signature { get; set; }
    public bool MailNewsletter { get; set; }
    public bool MaulHtml { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Known { get; set; }
    //    public byte[] RowVersion { get; set; }
    //   public EntityState EntityState { get; set; }
}
