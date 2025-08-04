using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl42Superfamilies;

namespace ATIS.Views.Search.Pdf.Tbl42Superfamilies;
internal class Tbl42SuperfamiliesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl42Superfamily SuperfamilySingle = new();

    public static Tbl42SuperfamiliesPdfModel GetSuperfamilyListDetails(ObservableCollection<Tbl42Superfamily> superfamiliesCollection)
    {

        SuperfamiliesCollection = superfamiliesCollection;

        return new Tbl42SuperfamiliesPdfModel
        {
            SuperfamilyName = SuperfamilySingle.SuperfamilyName,
            CountId = SuperfamilySingle.CountId,
            Synonym = SuperfamilySingle.Synonym,
            Author = SuperfamilySingle.Author,
            AuthorYear = SuperfamilySingle.AuthorYear,
            Info = SuperfamilySingle.Info,
            Valid = SuperfamilySingle.Valid,
            ValidYear = SuperfamilySingle.ValidYear,
            EngName = SuperfamilySingle.EngName,
            GerName = SuperfamilySingle.GerName,
            FraName = SuperfamilySingle.FraName,
            PorName = SuperfamilySingle.PorName,
            Memo = SuperfamilySingle.Memo,
            WriterDate = SuperfamilySingle.WriterDate,
            UpdaterDate = SuperfamilySingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl42Superfamily> SuperfamiliesCollection { get; set; } = null!;
    #endregion "Collections"

}
