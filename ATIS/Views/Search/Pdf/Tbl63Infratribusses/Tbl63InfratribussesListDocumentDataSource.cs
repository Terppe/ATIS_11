using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl63Infratribusses;

namespace ATIS.Views.Search.Pdf.Tbl63Infratribusses;
internal class Tbl63InfratribussesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl63Infratribus InfratribusSingle = new();

    public static Tbl63InfratribussesPdfModel GetInfratribusListDetails(ObservableCollection<Tbl63Infratribus> infratribussesCollection)
    {

        InfratribussesCollection = infratribussesCollection;

        return new Tbl63InfratribussesPdfModel
        {
            InfratribusName = InfratribusSingle.InfratribusName,
            CountId = InfratribusSingle.CountId,
            Synonym = InfratribusSingle.Synonym,
            Author = InfratribusSingle.Author,
            AuthorYear = InfratribusSingle.AuthorYear,
            Info = InfratribusSingle.Info,
            Valid = (bool)InfratribusSingle.Valid,
            ValidYear = InfratribusSingle.ValidYear,
            EngName = InfratribusSingle.EngName,
            GerName = InfratribusSingle.GerName,
            FraName = InfratribusSingle.FraName,
            PorName = InfratribusSingle.PorName,
            Memo = InfratribusSingle.Memo,
            WriterDate = InfratribusSingle.WriterDate,
            UpdaterDate = InfratribusSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl63Infratribus> InfratribussesCollection { get; set; } = null!;
    #endregion "Collections"

}
