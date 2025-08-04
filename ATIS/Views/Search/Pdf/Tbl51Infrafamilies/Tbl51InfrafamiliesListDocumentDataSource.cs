using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl51Infrafamilies;

namespace ATIS.Views.Search.Pdf.Tbl51Infrafamilies;
internal class Tbl51InfrafamiliesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl51Infrafamily InfrafamilySingle = new();

    public static Tbl51InfrafamiliesPdfModel GetInfrafamilyListDetails(ObservableCollection<Tbl51Infrafamily> infrafamiliesCollection)
    {

        InfrafamiliesCollection = infrafamiliesCollection;

        return new Tbl51InfrafamiliesPdfModel
        {
            InfrafamilyName = InfrafamilySingle.InfrafamilyName,
            CountId = InfrafamilySingle.CountId,
            Synonym = InfrafamilySingle.Synonym,
            Author = InfrafamilySingle.Author,
            AuthorYear = InfrafamilySingle.AuthorYear,
            Info = InfrafamilySingle.Info,
            Valid = InfrafamilySingle.Valid,
            ValidYear = InfrafamilySingle.ValidYear,
            EngName = InfrafamilySingle.EngName,
            GerName = InfrafamilySingle.GerName,
            FraName = InfrafamilySingle.FraName,
            PorName = InfrafamilySingle.PorName,
            Memo = InfrafamilySingle.Memo,
            WriterDate = InfrafamilySingle.WriterDate,
            UpdaterDate = InfrafamilySingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl51Infrafamily> InfrafamiliesCollection { get; set; } = null!;
    #endregion "Collections"

}
