using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl36Subordos;

namespace ATIS.Views.Search.Pdf.Tbl36Subordos;
internal class Tbl36SubordosListDocumentDataSource : ObservableObject
{
    private static readonly Tbl36Subordo SubordoSingle = new();

    public static Tbl36SubordosPdfModel GetSubordoListDetails(ObservableCollection<Tbl36Subordo> subordosCollection)
    {

        SubordosCollection = subordosCollection;

        return new Tbl36SubordosPdfModel
        {
            SubordoName = SubordoSingle.SubordoName,
            CountId = SubordoSingle.CountId,
            Synonym = SubordoSingle.Synonym,
            Author = SubordoSingle.Author,
            AuthorYear = SubordoSingle.AuthorYear,
            Info = SubordoSingle.Info,
            Valid = SubordoSingle.Valid,
            ValidYear = SubordoSingle.ValidYear,
            EngName = SubordoSingle.EngName,
            GerName = SubordoSingle.GerName,
            FraName = SubordoSingle.FraName,
            PorName = SubordoSingle.PorName,
            Memo = SubordoSingle.Memo,
            WriterDate = SubordoSingle.WriterDate,
            UpdaterDate = SubordoSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl36Subordo> SubordosCollection { get; set; } = null!;
    #endregion "Collections"

}
