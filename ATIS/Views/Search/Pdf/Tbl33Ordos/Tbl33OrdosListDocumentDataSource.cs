using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl33Ordos;

namespace ATIS.Views.Search.Pdf.Tbl33Ordos;
internal class Tbl33OrdosListDocumentDataSource : ObservableObject
{
    private static readonly Tbl33Ordo OrdoSingle = new();

    public static Tbl33OrdosPdfModel GetOrdoListDetails(ObservableCollection<Tbl33Ordo> ordosCollection)
    {

        OrdosCollection = ordosCollection;

        return new Tbl33OrdosPdfModel
        {
            OrdoName = OrdoSingle.OrdoName,
            CountId = OrdoSingle.CountId,
            Synonym = OrdoSingle.Synonym,
            Author = OrdoSingle.Author,
            AuthorYear = OrdoSingle.AuthorYear,
            Info = OrdoSingle.Info,
            Valid = OrdoSingle.Valid,
            ValidYear = OrdoSingle.ValidYear,
            EngName = OrdoSingle.EngName,
            GerName = OrdoSingle.GerName,
            FraName = OrdoSingle.FraName,
            PorName = OrdoSingle.PorName,
            Memo = OrdoSingle.Memo,
            WriterDate = OrdoSingle.WriterDate,
            UpdaterDate = OrdoSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl33Ordo> OrdosCollection { get; set; } = null!;
    #endregion "Collections"

}
