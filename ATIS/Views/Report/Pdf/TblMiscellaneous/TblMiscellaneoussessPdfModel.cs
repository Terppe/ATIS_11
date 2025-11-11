
namespace ATIS.Views.Report.Pdf.TblMiscellaneous;
public class TblMiscellaneoussesPdfModel
{
    public string MiscellaneousName { get; set; }
    public int CountId { get; set; }
    public bool? Valid { get; set; }
    public string ValidYear { get; set; }
    public string ShortInfo { get; set; }
    public string Thema1 { get; set; }
    public string Thema2 { get; set; }
    public string Info { get; set; }
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    public string Memo { get; set; }
    public PdfModels.Miscellaneous MiscellaneousTree { get; set; }
}
