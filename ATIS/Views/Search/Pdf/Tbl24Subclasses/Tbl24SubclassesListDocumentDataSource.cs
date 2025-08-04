using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl24Subclasses;

namespace ATIS.Views.Search.Pdf.Tbl24Subclasses;
internal class Tbl24SubclassesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl24Subclass SubclassSingle = new();

    public static Tbl24SubclassesPdfModel GetSubclassListDetails(ObservableCollection<Tbl24Subclass> subclassesCollection)
    {

        SubclassesCollection = subclassesCollection;

        return new Tbl24SubclassesPdfModel
        {
            SubclassName = SubclassSingle.SubclassName,
            CountId = SubclassSingle.CountId,
            Synonym = SubclassSingle.Synonym,
            Author = SubclassSingle.Author,
            AuthorYear = SubclassSingle.AuthorYear,
            Info = SubclassSingle.Info,
            Valid = SubclassSingle.Valid,
            ValidYear = SubclassSingle.ValidYear,
            EngName = SubclassSingle.EngName,
            GerName = SubclassSingle.GerName,
            FraName = SubclassSingle.FraName,
            PorName = SubclassSingle.PorName,
            Memo = SubclassSingle.Memo,
            WriterDate = SubclassSingle.WriterDate,
            UpdaterDate = SubclassSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl24Subclass> SubclassesCollection { get; set; } = null!;
    #endregion "Collections"

}
