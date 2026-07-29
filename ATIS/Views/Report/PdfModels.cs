using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Models;

namespace ATIS.Views.Report;
public class PdfModels
{
    public class Regnum
    {
        public string RegnumName
        {
            get; set;
        }
        public string Subregnum
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Phylum
    {
        public string PhylumName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public class Subphylum
    {
        public string SubphylumName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    //public class Division
    //{
    //    public string DivisionName
    //    {
    //        get; set;
    //    }
    //    public string Author
    //    {
    //        get; set;
    //    }
    //    public string AuthorYear
    //    {
    //        get; set;
    //    }
    //    public string EngName
    //    {
    //        get; set;
    //    }
    //    public string GerName
    //    {
    //        get; set;
    //    }
    //    public string FraName
    //    {
    //        get; set;
    //    }
    //    public string PorName
    //    {
    //        get; set;
    //    }
    //    public bool Valid
    //    {
    //        get; set;
    //    }
    //    public string ValidYear
    //    {
    //        get; set;
    //    }
    //    public string Info
    //    {
    //        get; set;
    //    }
    //    public string Memo
    //    {
    //        get; set;
    //    }

    //}
    //public class Subdivision
    //{
    //    public string SubdivisionName
    //    {
    //        get; set;
    //    }
    //    public string Author
    //    {
    //        get; set;
    //    }
    //    public string AuthorYear
    //    {
    //        get; set;
    //    }
    //    public string EngName
    //    {
    //        get; set;
    //    }
    //    public string GerName
    //    {
    //        get; set;
    //    }
    //    public string FraName
    //    {
    //        get; set;
    //    }
    //    public string PorName
    //    {
    //        get; set;
    //    }
    //    public bool Valid
    //    {
    //        get; set;
    //    }
    //    public string ValidYear
    //    {
    //        get; set;
    //    }
    //    public string Info
    //    {
    //        get; set;
    //    }
    //    public string Memo
    //    {
    //        get; set;
    //    }

    //}
    public class Superclass
    {
        public string SuperclassName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Class
    {
        public string ClassName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Subclass
    {
        public string SubclassName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Infraclass
    {
        public string InfraclassName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Legio
    {
        public string LegioName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Ordo
    {
        public string OrdoName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Subordo
    {
        public string SubordoName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Infraordo
    {
        public string InfraordoName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Superfamily
    {
        public string SuperfamilyName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Family
    {
        public string FamilyName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Subfamily
    {
        public string SubfamilyName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Infrafamily
    {
        public string InfrafamilyName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
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
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Supertribus
    {
        public string SupertribusName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Tribus
    {
        public string TribusName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Subtribus
    {
        public string SubtribusName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Infratribus
    {
        public string InfratribusName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Genus
    {
        public string GenusName
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
        public string EngName
        {
            get; set;
        }
        public string GerName
        {
            get; set;
        }
        public string FraName
        {
            get; set;
        }
        public string PorName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }
    public class Species
    {
        public int SpeciesId
        {
            get; set;
        }
        public string SpeciesName
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
        public string SpeciesFullName => $"{Tbl66Genusses.GenusName} {SpeciesName} {Subspecies} {Divers}";

        public int GenusId
        {
            get; set;
        }
        public int SpeciesgroupId
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public bool Valid
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
        public bool TypeSpecies
        {
            get; set;
        }
        public string LNumber
        {
            get; set;
        }
        public string LOrigin
        {
            get; set;
        }
        public string LdaNumber
        {
            get; set;
        }
        public string LdaOrigin
        {
            get; set;
        }
        public int? BasinLength
        {
            get; set;
        }
        public decimal? FishLength
        {
            get; set;
        }
        public bool Karnivore
        {
            get; set;
        }
        public bool Herbivore
        {
            get; set;
        }
        public bool Limnivore
        {
            get; set;
        }
        public bool Omnivore
        {
            get; set;
        }
        public string MemoFoods
        {
            get; set;
        }
        public bool Difficult1
        {
            get; set;
        }
        public bool Difficult2
        {
            get; set;
        }
        public bool Difficult3
        {
            get; set;
        }
        public bool Difficult4
        {
            get; set;
        }
        public bool RegionTop
        {
            get; set;
        }
        public bool RegionMiddle
        {
            get; set;
        }
        public bool RegionBottom
        {
            get; set;
        }
        public string MemoRegion
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
        public string MemoHusbandry
        {
            get; set;
        }
        public string MemoBreeding
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
        public string MemoSozial
        {
            get; set;
        }
        public string MemoDomorphism
        {
            get; set;
        }
        public string MemoSpecial
        {
            get; set;
        }
        //Plant
        public int? BasinHeight
        {
            get; set;
        }
        public decimal? PlantLength
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
        public string Creature
        {
            get; set;
        }

        //     public byte[] RowVersion { get; set; }

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


    }
    //public class PlSpecies
    //{
    //    public int PlSpeciesId
    //    {
    //        get; set;
    //    }
    //    public string PlSpeciesName
    //    {
    //        get; set;
    //    }
    //    public string Subspecies
    //    {
    //        get; set;
    //    }
    //    public string Divers
    //    {
    //        get; set;
    //    }
    //    public string PlSpeciesFullName => $"{Tbl66Genusses!.GenusName} {PlSpeciesName} {Subspecies} {Divers}";

    //    public int GenusId
    //    {
    //        get; set;
    //    }
    //    public int SpeciesgroupId
    //    {
    //        get; set;
    //    }
    //    public int CountId
    //    {
    //        get; set;
    //    }
    //    public bool Valid
    //    {
    //        get; set;
    //    }
    //    public string ValidYear
    //    {
    //        get; set;
    //    }
    //    public string MemoSpecies
    //    {
    //        get; set;
    //    }
    //    public string TradeName
    //    {
    //        get; set;
    //    }
    //    public string Author
    //    {
    //        get; set;
    //    }
    //    public string AuthorYear
    //    {
    //        get; set;
    //    }
    //    public string Importer
    //    {
    //        get; set;
    //    }
    //    public string ImportingYear
    //    {
    //        get; set;
    //    }
    //    public int BasinHeight
    //    {
    //        get; set;
    //    }
    //    public decimal PlantLength
    //    {
    //        get; set;
    //    }
    //    public bool Difficult1
    //    {
    //        get; set;
    //    }
    //    public bool Difficult2
    //    {
    //        get; set;
    //    }
    //    public bool Difficult3
    //    {
    //        get; set;
    //    }
    //    public bool Difficult4
    //    {
    //        get; set;
    //    }
    //    public string MemoTech
    //    {
    //        get; set;
    //    }
    //    public decimal Ph1
    //    {
    //        get; set;
    //    }
    //    public decimal Ph2
    //    {
    //        get; set;
    //    }
    //    public int Temp1
    //    {
    //        get; set;
    //    }
    //    public int Temp2
    //    {
    //        get; set;
    //    }
    //    public int Hardness1
    //    {
    //        get; set;
    //    }
    //    public int Hardness2
    //    {
    //        get; set;
    //    }
    //    public int CarboHardness1
    //    {
    //        get; set;
    //    }
    //    public int CarboHardness2
    //    {
    //        get; set;
    //    }
    //    public string Writer
    //    {
    //        get; set;
    //    }
    //    public DateTime WriterDate
    //    {
    //        get; set;
    //    }
    //    public string Updater
    //    {
    //        get; set;
    //    }
    //    public DateTime UpdaterDate
    //    {
    //        get; set;
    //    }
    //    public string MemoBuilt
    //    {
    //        get; set;
    //    }
    //    public string MemoColor
    //    {
    //        get; set;
    //    }
    //    public string MemoReproduction
    //    {
    //        get; set;
    //    }
    //    public string MemoCulture
    //    {
    //        get; set;
    //    }
    //    public string MemoGlobal
    //    {
    //        get; set;
    //    }
    //    //   public byte[] RowVersion { get; set; }

    //    [ForeignKey("GenusId")]
    //    public virtual Tbl66Genus Tbl66Genusses
    //    {
    //        get; set;
    //    }
    //    [ForeignKey("SpeciesgroupId")]
    //    public virtual Tbl68Speciesgroup Tbl68Speciesgroups
    //    {
    //        get; set;
    //    }
    //    //public virtual ICollection<Tbl78Name> Tbl78Names { get; set; }
    //    //public virtual ICollection<Tbl81Image> Tbl81Images { get; set; }
    //    //public virtual ICollection<Tbl84Synonym> Tbl84Synonyms { get; set; }
    //    //public virtual ICollection<Tbl87Geographic> Tbl87Geographics { get; set; }
    //    //public virtual ICollection<Tbl90Reference> Tbl90References { get; set; }
    //    //public virtual ICollection<Tbl93Comment> Tbl93Comments { get; set; }

    //    public void SetSpeciesgroupId(object obj)
    //    {
    //        SpeciesgroupId = (int)obj;
    //    }

    //}
    public class Name
    { 
        public int NameId
        {
            get; set;
        }
        public string NameName
        {
            get; set;
        }
        public int FiSpeciesId
        {
            get; set;
        }
        public int PlSpeciesId
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string Language
        {
            get; set;
        }
        public string Info
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
        public string Memo
        {
            get; set;
        }
        //     public byte[] RowVersion { get; set; }

        [ForeignKey("FiSpeciesId")]
        public virtual Tbl75Species Tbl69FiSpeciesses
        {
            get; set;
        }

        [ForeignKey("PlSpeciesId")]
        public virtual Tbl75Species Tbl72PlSpeciesses
        {
            get; set;
        }


    }
    public class Image
    {
        public int ImageId
        {
            get; set;
        }
        public int FiSpeciesId
        {
            get; set;
        }
        public int PlSpeciesId
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public DateTime ShotDate
        {
            get; set;
        }
        public string Info
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
        public string Memo
        {
            get; set;
        }
        public byte[] ImageData
        {
            get; set;
        }
        public string ImageMimeType
        {
            get; set;
        }
        public byte[] Filestream
        {
            get; set;
        }
        public Guid FilestreamId
        {
            get; set;
        }
        //     public byte[] RowVersion { get; set; }

        [ForeignKey("FiSpeciesId")]
        public virtual Tbl75Species Tbl69FiSpeciesses
        {
            get; set;
        }

        [ForeignKey("PlSpeciesId")]
        public virtual Tbl75Species Tbl72PlSpeciesses
        {
            get; set;
        }


    }
    public class Synonym
    {
        public int SynonymId
        {
            get; set;
        }
        public string SynonymName
        {
            get; set;
        }
        public int FiSpeciesId
        {
            get; set;
        }
        public int PlSpeciesId
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
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
        public string Info
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
        public string Memo
        {
            get; set;
        }
        //   public byte[] RowVersion { get; set; }

        [ForeignKey("FiSpeciesId")]
        public virtual Tbl75Species Tbl69FiSpeciesses
        {
            get; set;
        }

        [ForeignKey("PlSpeciesId")]
        public virtual Tbl75Species Tbl72PlSpeciesses
        {
            get; set;
        }


    }
    public class Geographic
    {

        public int GeographicId
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }
        public string Country
        {
            get; set;
        }
        public int FiSpeciesId
        {
            get; set;
        }
        public int PlSpeciesId
        {
            get; set;
        }
        public int CountId
        {
            get; set;
        }
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double Latitude1
        {
            get; set;
        }
        public double Longitude1
        {
            get; set;
        }
        public double Latitude2
        {
            get; set;
        }
        public double Longitude2
        {
            get; set;
        }
        public double Latitude3
        {
            get; set;
        }
        public double Longitude3
        {
            get; set;
        }
        public double ZoomLevel
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
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
        public string Info
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
        public string Memo
        {
            get; set;
        }
        public string Continent
        {
            get; set;
        }
        public string Http
        {
            get; set;
        }
        //     public byte[] RowVersion { get; set; }

        [ForeignKey("SpeciesId")]
        public virtual Tbl75Species Tbl75Speciesses
        {
            get; set;
        }

        //[ForeignKey("PlSpeciesId")]
        //public virtual Tbl75Species Tbl72PlSpeciesses
        //{
        //    get; set;
        //}


    }

    public class Food
    {
        public string FoodName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string ShortInfo
        {
            get; set;
        }
        public string FoodVolume
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }
        public string Kind
        {
            get; set;
        }
        public string Help
        {
            get; set;
        }
        public string Symptom
        {
            get; set;
        }

    }

    public class Disease
    {
        public string DiseaseName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string ShortInfo
        {
            get; set;
        }
        public string Symptom
        {
            get; set;
        }
        public string Reason
        {
            get; set;
        }
        public string Therapie
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }

    public class Miscellaneous
    {
        public string MiscellaneousName
        {
            get; set;
        }
        public bool Valid
        {
            get; set;
        }
        public string ValidYear
        {
            get; set;
        }
        public string ShortInfo
        {
            get; set;
        }
        public string Thema1
        {
            get; set;
        }
        public string Thema2
        {
            get; set;
        }
        public string Info
        {
            get; set;
        }
        public string Memo
        {
            get; set;
        }

    }

}
