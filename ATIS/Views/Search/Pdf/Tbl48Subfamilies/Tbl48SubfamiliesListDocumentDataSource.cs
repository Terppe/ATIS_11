using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl48Subfamilies;

namespace ATIS.Views.Search.Pdf.Tbl48Subfamilies;
internal class Tbl48SubfamiliesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl48Subfamily SubfamilySingle = new();

    public static Tbl48SubfamiliesPdfModel GetSubfamilyListDetails(ObservableCollection<Tbl48Subfamily> subfamiliesCollection)
    {

        SubfamiliesCollection = subfamiliesCollection;

        return new Tbl48SubfamiliesPdfModel
        {
            SubfamilyName = SubfamilySingle.SubfamilyName,
            CountId = SubfamilySingle.CountId,
            Synonym = SubfamilySingle.Synonym,
            Author = SubfamilySingle.Author,
            AuthorYear = SubfamilySingle.AuthorYear,
            Info = SubfamilySingle.Info,
            Valid = SubfamilySingle.Valid,
            ValidYear = SubfamilySingle.ValidYear,
            EngName = SubfamilySingle.EngName,
            GerName = SubfamilySingle.GerName,
            FraName = SubfamilySingle.FraName,
            PorName = SubfamilySingle.PorName,
            Memo = SubfamilySingle.Memo,
            WriterDate = SubfamilySingle.WriterDate,
            UpdaterDate = SubfamilySingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl48Subfamily> SubfamiliesCollection { get; set; } = null!;
    #endregion "Collections"

}
