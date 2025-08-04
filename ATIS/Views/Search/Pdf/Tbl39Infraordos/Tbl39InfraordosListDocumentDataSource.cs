using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl39Infraordos;

namespace ATIS.Views.Search.Pdf.Tbl39Infraordos;
internal class Tbl39InfraordosListDocumentDataSource : ObservableObject
{
    private static readonly Tbl39Infraordo InfraordoSingle = new();

    public static Tbl39InfraordosPdfModel GetInfraordoListDetails(ObservableCollection<Tbl39Infraordo> infraordosCollection)
    {

        InfraordosCollection = infraordosCollection;

        return new Tbl39InfraordosPdfModel
        {
            InfraordoName = InfraordoSingle.InfraordoName,
            CountId = InfraordoSingle.CountId,
            Synonym = InfraordoSingle.Synonym,
            Author = InfraordoSingle.Author,
            AuthorYear = InfraordoSingle.AuthorYear,
            Info = InfraordoSingle.Info,
            Valid = InfraordoSingle.Valid,
            ValidYear = InfraordoSingle.ValidYear,
            EngName = InfraordoSingle.EngName,
            GerName = InfraordoSingle.GerName,
            FraName = InfraordoSingle.FraName,
            PorName = InfraordoSingle.PorName,
            Memo = InfraordoSingle.Memo,
            WriterDate = InfraordoSingle.WriterDate,
            UpdaterDate = InfraordoSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl39Infraordo> InfraordosCollection { get; set; } = null!;
    #endregion "Collections"

}
