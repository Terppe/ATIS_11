//    PdfModelTbl36SubordosSkriptdatum:  15.06.2024  10:32    

namespace ATIS.Views.Report.Pdf.Tbl36Subordos;

public class Tbl36SubordosPdfModel
{
    public int SubordoId { get; set; }
    public string? SubordoName { get; set; }
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
    public PdfModels.Infraclass? InfraclassTree { get; set; }
    public PdfModels.Legio? LegioTree { get; set; }
    public PdfModels.Ordo? OrdoTree { get; set; }
    public PdfModels.Subordo? SubordoTree { get; set; }


}
