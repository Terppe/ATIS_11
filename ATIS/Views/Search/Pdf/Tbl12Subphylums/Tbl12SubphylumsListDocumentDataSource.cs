using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl12Subphylums;

namespace ATIS.Views.Search.Pdf.Tbl12Subphylums;
public class Tbl12SubphylumsListDocumentDataSource : ObservableObject
{
    private static readonly Tbl12Subphylum SubphylumSingle = new();

    public static Tbl12SubphylumsPdfModel GetSubphylumListDetails(ObservableCollection<Tbl12Subphylum> subphylumsCollection)
    {

        SubphylumsCollection = subphylumsCollection;

        return new Tbl12SubphylumsPdfModel
        {
            SubphylumName = SubphylumSingle.SubphylumName,
            CountId = SubphylumSingle.CountId,
            Synonym = SubphylumSingle.Synonym,
            Author = SubphylumSingle.Author,
            AuthorYear = SubphylumSingle.AuthorYear,
            Info = SubphylumSingle.Info,
            Valid = SubphylumSingle.Valid,
            ValidYear = SubphylumSingle.ValidYear,
            EngName = SubphylumSingle.EngName,
            GerName = SubphylumSingle.GerName,
            FraName = SubphylumSingle.FraName,
            PorName = SubphylumSingle.PorName,
            Memo = SubphylumSingle.Memo,
            WriterDate = SubphylumSingle.WriterDate,
            UpdaterDate = SubphylumSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl12Subphylum> SubphylumsCollection { get; set; } = null!;
    #endregion "Collections"

}
