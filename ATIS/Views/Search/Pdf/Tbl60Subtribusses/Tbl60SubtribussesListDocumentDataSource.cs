using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl60Subtribusses;

namespace ATIS.Views.Search.Pdf.Tbl60Subtribusses;
internal abstract class Tbl60SubtribussesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl60Subtribus SubtribusSingle = new();

    public static Tbl60SubtribussesPdfModel GetSubtribusListDetails(ObservableCollection<Tbl60Subtribus> subtribussesCollection)
    {
        SubtribussesCollection = subtribussesCollection;

        return new Tbl60SubtribussesPdfModel
        {
            SubtribusName = SubtribusSingle.SubtribusName,
            CountId = SubtribusSingle.CountId,
            Synonym = SubtribusSingle.Synonym,
            Author = SubtribusSingle.Author,
            AuthorYear = SubtribusSingle.AuthorYear,
            Info = SubtribusSingle.Info,
            Valid = SubtribusSingle.Valid,
            ValidYear = SubtribusSingle.ValidYear,
            EngName = SubtribusSingle.EngName,
            GerName = SubtribusSingle.GerName,
            FraName = SubtribusSingle.FraName,
            PorName = SubtribusSingle.PorName,
            Memo = SubtribusSingle.Memo,
            WriterDate = SubtribusSingle.WriterDate,
            UpdaterDate = SubtribusSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl60Subtribus> SubtribussesCollection { get; set; } = null!;
    #endregion "Collections"

}
