//    PdfModelTbl24SubclassesSkriptdatum:  15.08.2026  18:32    

namespace ATIS.Views.Report.Pdf.Tbl24Subclasses;

public class Tbl24SubclassesPdfModel
{
    public int SubclassId { get; set; }
    public string? SubclassName { get; set; }
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    public string? ValidYear { get; set; }
    public string? Synonym { get; set; }
    public string? Author { get; set; }
    public string? AuthorYear { get; set; }
    public string? Info { get; set; }
    public string? EngName { get; set; }
    public string? GerName { get; set; }
    public string? FraName { get; set; }
    public string? PorName { get; set; }
    public string? Writer { get; set; }
    public DateTime WriterDate { get; set; }
    public string? Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    public string? Memo { get; set; }


    public PdfModels.Regnum? RegnumTree { get; set; }
    public PdfModels.Phylum? PhylumTree { get; set; }
    public PdfModels.Subphylum? SubphylumTree { get; set; }
    public PdfModels.Superclass? SuperclassTree { get; set; }
    public PdfModels.Class? ClassTree { get; set; }
    public PdfModels.Subclass? SubclassTree { get; set; }


}
