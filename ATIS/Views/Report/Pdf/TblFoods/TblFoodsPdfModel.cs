

//  Interface Skriptdatum:  13.06.2023  10:32      

namespace ATIS.Views.Report.Pdf.TblFoods;
public class TblFoodsPdfModel
{
    public string FoodName { get; set; }
    public int CountId { get; set; }
    public bool Valid { get; set; }
    public string ValidYear { get; set; }
    public string ShortInfo { get; set; }
    public string Symptom { get; set; }
    public string FoodVolume { get; set; }
    public string Info { get; set; }
    public string Writer { get; set; }
    public DateTime WriterDate { get; set; }
    public string Updater { get; set; }
    public DateTime UpdaterDate { get; set; }
    public string Memo { get; set; }
    public PdfModels.Food FoodTree { get; set; }
}
