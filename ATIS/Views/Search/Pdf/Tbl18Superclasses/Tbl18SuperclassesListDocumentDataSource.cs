using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl18Superclasses;

namespace ATIS.Views.Search.Pdf.Tbl18Superclasses;
internal class Tbl18SuperclassesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl18Superclass SuperclassSingle = new();

    public static Tbl18SuperclassesPdfModel GetSuperclassListDetails(ObservableCollection<Tbl18Superclass> superclassesCollection)
    {

        SuperclassesCollection = superclassesCollection;

        return new Tbl18SuperclassesPdfModel
        {
            SuperclassName = SuperclassSingle.SuperclassName,
            CountId = SuperclassSingle.CountId,
            Synonym = SuperclassSingle.Synonym,
            Author = SuperclassSingle.Author,
            AuthorYear = SuperclassSingle.AuthorYear,
            Info = SuperclassSingle.Info,
            Valid = SuperclassSingle.Valid,
            ValidYear = SuperclassSingle.ValidYear,
            EngName = SuperclassSingle.EngName,
            GerName = SuperclassSingle.GerName,
            FraName = SuperclassSingle.FraName,
            PorName = SuperclassSingle.PorName,
            Memo = SuperclassSingle.Memo,
            WriterDate = SuperclassSingle.WriterDate,
            UpdaterDate = SuperclassSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl18Superclass> SuperclassesCollection { get; set; } = null!;
    #endregion "Collections"

}
