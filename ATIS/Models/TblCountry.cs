using System.ComponentModel.DataAnnotations;

namespace ATIS.Models;

public class TblCountry
{
    [Key]

    public int CountryId { get; set; }
    [Required(ErrorMessageResourceName = "RequiredNameName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Name { get; set; }
    [StringLength(250, ErrorMessageResourceName = "StringLength_250", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Regex { get; set; }

}
