using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl57Tribusses;

namespace ATIS.Views.Search.Pdf.Tbl57Tribusses;
internal class Tbl57TribussesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl57Tribus TribusSingle = new();

    public static Tbl57TribussesPdfModel GetTribusListDetails(ObservableCollection<Tbl57Tribus> tribussesCollection)
    {

        TribussesCollection = tribussesCollection;

        return new Tbl57TribussesPdfModel
        {
            TribusName = TribusSingle.TribusName,
            CountId = TribusSingle.CountId,
            Synonym = TribusSingle.Synonym,
            Author = TribusSingle.Author,
            AuthorYear = TribusSingle.AuthorYear,
            Info = TribusSingle.Info,
            Valid = TribusSingle.Valid,
            ValidYear = TribusSingle.ValidYear,
            EngName = TribusSingle.EngName,
            GerName = TribusSingle.GerName,
            FraName = TribusSingle.FraName,
            PorName = TribusSingle.PorName,
            Memo = TribusSingle.Memo,
            WriterDate = TribusSingle.WriterDate,
            UpdaterDate = TribusSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl57Tribus> TribussesCollection { get; set; } = null!;
    #endregion "Collections"

}
