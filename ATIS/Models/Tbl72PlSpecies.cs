using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATIS.Models;

public class Tbl72PlSpecies
{
    [Key]
    public int PlSpeciesId { get; set; }
    [Required(ErrorMessageResourceName = "RequiredPlSpeciesName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string PlSpeciesName { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Subspecies { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Divers { get; set; }
    public string PlSpeciesFullName => $"{PlSpeciesName} {Subspecies} {Divers}";
  //  public string PlSpeciesFullName => $"{Tbl66Genusses.GenusName} {PlSpeciesName} {Subspecies} {Divers}";
    [Required]
    public int GenusId { get; set; }
    public int SpeciesgroupId { get; set; }
    [Required]
    public int CountId { get; set; }
    public bool Valid { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ValidYear { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoSpecies { get; set; }
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string TradeName { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Author { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string AuthorYear { get; set; }
    [StringLength(60, ErrorMessageResourceName = "StringLength_60", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Importer { get; set; }
    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string ImportingYear { get; set; }
    public int BasinHeight { get; set; }
    public decimal PlantLength { get; set; }
    public bool Difficult1 { get; set; }
    public bool Difficult2 { get; set; }
    public bool Difficult3 { get; set; }
    public bool Difficult4 { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoTech { get; set; }
    public decimal Ph1 { get; set; }
    public decimal Ph2 { get; set; }
    public int Temp1 { get; set; }
    public int Temp2 { get; set; }
    public int Hardness1 { get; set; }
    public int Hardness2 { get; set; }
    public int CarboHardness1 { get; set; }
    public int CarboHardness2 { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoBuilt { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoColor { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoReproduction { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoCulture { get; set; }
    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string MemoGlobal { get; set; }
    //   public byte[] RowVersion { get; set; }

    [ForeignKey("GenusId")]
    public virtual Tbl66Genus Tbl66Genusses { get; set; }
    //  public virtual ICollection<Tbl66Genus> Tbl66Genusses { get; set; }
    [ForeignKey("SpeciesgroupId")]
    public virtual Tbl68Speciesgroup Tbl68Speciesgroups { get; set; }

    //public virtual ICollection<Tbl78Name> Tbl78Names { get; set; }
    //public virtual ICollection<Tbl81Image> Tbl81Images { get; set; }
    //public virtual ICollection<Tbl84Synonym> Tbl84Synonyms { get; set; }
    //public virtual ICollection<Tbl87Geographic> Tbl87Geographics { get; set; }
    //public virtual ICollection<Tbl90Reference> Tbl90References { get; set; }
    //public virtual ICollection<Tbl93Comment> Tbl93Comments { get; set; }

    public void SetSpeciesgroupId(Object obj)
    {
        SpeciesgroupId = (int)obj;
    }

}
