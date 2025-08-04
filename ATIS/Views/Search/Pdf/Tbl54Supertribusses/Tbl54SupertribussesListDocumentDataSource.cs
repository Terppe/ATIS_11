using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl54Supertribusses;

namespace ATIS.Views.Search.Pdf.Tbl54Supertribusses;
internal class Tbl54SupertribussesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl54Supertribus SupertribusSingle = new();

    public static Tbl54SupertribussesPdfModel GetSupertribusListDetails(ObservableCollection<Tbl54Supertribus> supertribussesCollection)
    {

        SupertribussesCollection = supertribussesCollection;

        return new Tbl54SupertribussesPdfModel
        {
            SupertribusName = SupertribusSingle.SupertribusName,
            CountId = SupertribusSingle.CountId,
            Synonym = SupertribusSingle.Synonym,
            Author = SupertribusSingle.Author,
            AuthorYear = SupertribusSingle.AuthorYear,
            Info = SupertribusSingle.Info,
            Valid = SupertribusSingle.Valid,
            ValidYear = SupertribusSingle.ValidYear,
            EngName = SupertribusSingle.EngName,
            GerName = SupertribusSingle.GerName,
            FraName = SupertribusSingle.FraName,
            PorName = SupertribusSingle.PorName,
            Memo = SupertribusSingle.Memo,
            WriterDate = SupertribusSingle.WriterDate,
            UpdaterDate = SupertribusSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl54Supertribus> SupertribussesCollection { get; set; } = null!;
    #endregion "Collections"

}
