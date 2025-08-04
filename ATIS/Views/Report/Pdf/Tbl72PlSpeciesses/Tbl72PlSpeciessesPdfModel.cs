using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Models;

namespace ATIS.Views.Report.Pdf.Tbl72PlSpeciesses;
public class Tbl72PlSpeciessesPdfModel
{
    public int PlSpeciesId
    {
        get; set;
    }
    public string PlSpeciesName
    {
        get; set;
    }
    public string Subspecies
    {
        get; set;
    }
    public string Divers
    {
        get; set;
    }
    public string PlSpeciesFullName => $"{Tbl66Genusses!.GenusName} {PlSpeciesName} {Subspecies} {Divers}";

    public int GenusId
    {
        get; set;
    }
    public int? SpeciesgroupId
    {
        get; set;
    }
    public int CountId
    {
        get; set;
    }
    public bool? Valid
    {
        get; set;
    }
    public string ValidYear
    {
        get; set;
    }
    public string MemoSpecies
    {
        get; set;
    }
    public string TradeName
    {
        get; set;
    }
    public string Author
    {
        get; set;
    }
    public string AuthorYear
    {
        get; set;
    }
    public string Importer
    {
        get; set;
    }
    public string ImportingYear
    {
        get; set;
    }
    public int? BasinHeight
    {
        get; set;
    }
    public decimal? PlantLength
    {
        get; set;
    }
    public bool? Difficult1
    {
        get; set;
    }
    public bool? Difficult2
    {
        get; set;
    }
    public bool? Difficult3
    {
        get; set;
    }
    public bool? Difficult4
    {
        get; set;
    }
    public string MemoTech
    {
        get; set;
    }
    public decimal? Ph1
    {
        get; set;
    }
    public decimal? Ph2
    {
        get; set;
    }
    public int? Temp1
    {
        get; set;
    }
    public int? Temp2
    {
        get; set;
    }
    public int? Hardness1
    {
        get; set;
    }
    public int? Hardness2
    {
        get; set;
    }
    public int? CarboHardness1
    {
        get; set;
    }
    public int? CarboHardness2
    {
        get; set;
    }
    public string Writer
    {
        get; set;
    }
    public DateTime WriterDate
    {
        get; set;
    }
    public string Updater
    {
        get; set;
    }
    public DateTime UpdaterDate
    {
        get; set;
    }
    public string MemoBuilt
    {
        get; set;
    }
    public string MemoColor
    {
        get; set;
    }
    public string MemoReproduction
    {
        get; set;
    }
    public string MemoCulture
    {
        get; set;
    }
    public string MemoGlobal
    {
        get; set;
    }
    //   public byte[] RowVersion { get; set; }

    [ForeignKey("GenusId")]
    public virtual Tbl66Genus Tbl66Genusses
    {
        get; set;
    }
    [ForeignKey("SpeciesgroupId")]
    public virtual Tbl68Speciesgroup Tbl68Speciesgroups
    {
        get; set;
    }
    //public virtual ICollection<Tbl78Name> Tbl78Names { get; set; }
    //public virtual ICollection<Tbl81Image> Tbl81Images { get; set; }
    //public virtual ICollection<Tbl84Synonym> Tbl84Synonyms { get; set; }
    //public virtual ICollection<Tbl87Geographic> Tbl87Geographics { get; set; }
    //public virtual ICollection<Tbl90Reference> Tbl90References { get; set; }
    //public virtual ICollection<Tbl93Comment> Tbl93Comments { get; set; }

    public void SetSpeciesgroupId(object obj)
    {
        SpeciesgroupId = (int)obj;
    }

    public PdfModels.Regnum RegnumTree
    {
        get; set;
    }
    public PdfModels.Phylum PhylumTree
    {
        get; set;
    }
    public PdfModels.Division DivisionTree
    {
        get; set;
    }
    public PdfModels.Subphylum SubphylumTree
    {
        get; set;
    }
    public PdfModels.Subdivision SubdivisionTree
    {
        get; set;
    }
    public PdfModels.Superclass SuperclassTree
    {
        get; set;
    }
    public PdfModels.Class ClassTree
    {
        get; set;
    }
    public PdfModels.Subclass SubclassTree
    {
        get; set;
    }
    public PdfModels.Infraclass InfraclassTree
    {
        get; set;
    }
    public PdfModels.Legio LegioTree
    {
        get; set;
    }
    public PdfModels.Ordo OrdoTree
    {
        get; set;
    }
    public PdfModels.Subordo SubordoTree
    {
        get; set;
    }
    public PdfModels.Infraordo InfraordoTree
    {
        get; set;
    }
    public PdfModels.Superfamily SuperfamilyTree
    {
        get; set;
    }
    public PdfModels.Family FamilyTree
    {
        get; set;
    }
    public PdfModels.Subfamily SubfamilyTree
    {
        get; set;
    }
    public PdfModels.Infrafamily InfrafamilyTree
    {
        get; set;
    }
    public PdfModels.Supertribus SupertribusTree
    {
        get; set;
    }
    public PdfModels.Tribus TribusTree
    {
        get; set;
    }
    public PdfModels.Subtribus SubtribusTree
    {
        get; set;
    }
    public PdfModels.Infratribus InfratribusTree
    {
        get; set;
    }
    public PdfModels.Genus GenusTree
    {
        get; set;
    }
    public PdfModels.FiSpecies FiSpeciesTree
    {
        get; set;
    }
    public PdfModels.PlSpecies PlSpeciesTree
    {
        get; set;
    }
    public PdfModels.Name NamesTree
    {
        get; set;
    }
    public PdfModels.Synonym SynonymsTree
    {
        get; set;
    }


}
