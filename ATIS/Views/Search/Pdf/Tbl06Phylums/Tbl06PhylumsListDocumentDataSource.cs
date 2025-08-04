using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl06Phylums;

namespace ATIS.Views.Search.Pdf.Tbl06Phylums;
public class Tbl06PhylumsListDocumentDataSource : ObservableObject
{
    private static readonly Tbl06Phylum PhylumSingle = new();

    public static Tbl06PhylumsPdfModel GetPhylumListDetails(ObservableCollection<Tbl06Phylum> phylumsCollection)
    {

        PhylumsCollection = phylumsCollection;

        return new Tbl06PhylumsPdfModel
        {
            PhylumName = PhylumSingle.PhylumName,
            CountId = PhylumSingle.CountId,
            Synonym = PhylumSingle.Synonym,
            Author = PhylumSingle.Author,
            AuthorYear = PhylumSingle.AuthorYear,
            Info = PhylumSingle.Info,
            Valid = PhylumSingle.Valid,
            ValidYear = PhylumSingle.ValidYear,
            EngName = PhylumSingle.EngName,
            GerName = PhylumSingle.GerName,
            FraName = PhylumSingle.FraName,
            PorName = PhylumSingle.PorName,
            Memo = PhylumSingle.Memo,
            WriterDate = PhylumSingle.WriterDate,
            UpdaterDate = PhylumSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl06Phylum> PhylumsCollection { get; set; } = null!;
    #endregion "Collections"
}
