using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl15Subdivisions;

namespace ATIS.Views.Search.Pdf.Tbl15Subdivisions;
public class Tbl15SubdivisionsListDocumentDataSource : ObservableObject
{
    private static readonly Tbl15Subdivision SubdivisionSingle = new();

    public static Tbl15SubdivisionsPdfModel GetSubdivisionListDetails(ObservableCollection<Tbl15Subdivision> subdivisionsCollection)
    {

        SubdivisionsCollection = subdivisionsCollection;

        return new Tbl15SubdivisionsPdfModel
        {
            SubdivisionName = SubdivisionSingle.SubdivisionName,
            CountId = SubdivisionSingle.CountId,
            Synonym = SubdivisionSingle.Synonym,
            Author = SubdivisionSingle.Author,
            AuthorYear = SubdivisionSingle.AuthorYear,
            Info = SubdivisionSingle.Info,
            Valid = SubdivisionSingle.Valid,
            ValidYear = SubdivisionSingle.ValidYear,
            EngName = SubdivisionSingle.EngName,
            GerName = SubdivisionSingle.GerName,
            FraName = SubdivisionSingle.FraName,
            PorName = SubdivisionSingle.PorName,
            Memo = SubdivisionSingle.Memo,
            WriterDate = SubdivisionSingle.WriterDate,
            UpdaterDate = SubdivisionSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl15Subdivision> SubdivisionsCollection { get; set; } = null!;
    #endregion "Collections"

}
