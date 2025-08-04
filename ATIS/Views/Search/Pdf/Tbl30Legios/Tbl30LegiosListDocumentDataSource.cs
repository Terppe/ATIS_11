using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl30Legios;

namespace ATIS.Views.Search.Pdf.Tbl30Legios;
internal class Tbl30LegiosListDocumentDataSource : ObservableObject
{
    private static readonly Tbl30Legio LegioSingle = new();

    public static Tbl30LegiosPdfModel GetLegioListDetails(ObservableCollection<Tbl30Legio> legiosCollection)
    {

        LegiosCollection = legiosCollection;

        return new Tbl30LegiosPdfModel
        {
            LegioName = LegioSingle.LegioName,
            CountId = LegioSingle.CountId,
            Synonym = LegioSingle.Synonym,
            Author = LegioSingle.Author,
            AuthorYear = LegioSingle.AuthorYear,
            Info = LegioSingle.Info,
            Valid = LegioSingle.Valid,
            ValidYear = LegioSingle.ValidYear,
            EngName = LegioSingle.EngName,
            GerName = LegioSingle.GerName,
            FraName = LegioSingle.FraName,
            PorName = LegioSingle.PorName,
            Memo = LegioSingle.Memo,
            WriterDate = LegioSingle.WriterDate,
            UpdaterDate = LegioSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl30Legio> LegiosCollection { get; set; } = null!;
    #endregion "Collections"

}
