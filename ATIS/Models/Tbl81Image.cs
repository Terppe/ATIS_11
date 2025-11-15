using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATIS.Models;

public class Tbl81Image
{
    [Key]

    public int ImageId { get; set; }
    public int SpeciesId { get; set; }
    //public int FiSpeciesId { get; set; }
    //public int PlSpeciesId { get; set; }
    public int FoodId { get; set; }
    public int DiseaseId { get; set; }
    public int MiscellaneousId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    public DateTime? ShotDate { get; set; }
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
    public byte[] ImageData { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ImageMimeType { get; set; }
    public byte[] Filestream { get; set; }
    public Guid FilestreamId { get; set; }
    //     public byte[] RowVersion { get; set; }

    //[ForeignKey("FiSpeciesId")]
    //public virtual Tbl69FiSpecies Tbl69FiSpeciesses { get; set; }

    //[ForeignKey("PlSpeciesId")]
    //public virtual Tbl72PlSpecies Tbl72PlSpeciesses { get; set; }

    //[ForeignKey("FoodId")]
    //public virtual TblFood TblFoods { get; set; }

    //[ForeignKey("DiseaseId")]
    //public virtual TblDisease TblDiseases { get; set; }

    //[ForeignKey("MiscellaneousId")]
    //public virtual TblMiscellaneous TblMiscellaneousses { get; set; }

    //[ForeignKey("SpeciesId")]
    //public virtual Tbl75Species Tbl75Speciesses { get; set; }


}
