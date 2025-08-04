using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl09Divisions;

namespace ATIS.Views.Search.Pdf.Tbl09Divisions;
public class Tbl09DivisionsListDocumentDataSource : ObservableObject
{
    private static readonly Tbl09Division DivisionSingle = new();

    public static Tbl09DivisionsPdfModel GetDivisionListDetails(ObservableCollection<Tbl09Division> divisionsCollection)
    {

        DivisionsCollection = divisionsCollection;

        return new Tbl09DivisionsPdfModel
        {
            DivisionName = DivisionSingle.DivisionName,
            CountId = DivisionSingle.CountId,
            Synonym = DivisionSingle.Synonym,
            Author = DivisionSingle.Author,
            AuthorYear = DivisionSingle.AuthorYear,
            Info = DivisionSingle.Info,
            Valid = DivisionSingle.Valid,
            ValidYear = DivisionSingle.ValidYear,
            EngName = DivisionSingle.EngName,
            GerName = DivisionSingle.GerName,
            FraName = DivisionSingle.FraName,
            PorName = DivisionSingle.PorName,
            Memo = DivisionSingle.Memo,
            WriterDate = DivisionSingle.WriterDate,
            UpdaterDate = DivisionSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl09Division> DivisionsCollection { get; set; } = null!;
    #endregion "Collections"

}
