using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl21Classes;

namespace ATIS.Views.Search.Pdf.Tbl21Classes;
internal class Tbl21ClassesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl21Class ClassSingle = new();

    public static Tbl21ClassesPdfModel GetClassListDetails(ObservableCollection<Tbl21Class> classesCollection)
    {

        ClassesCollection = classesCollection;

        return new Tbl21ClassesPdfModel
        {
            ClassName = ClassSingle.ClassName,
            CountId = ClassSingle.CountId,
            Synonym = ClassSingle.Synonym,
            Author = ClassSingle.Author,
            AuthorYear = ClassSingle.AuthorYear,
            Info = ClassSingle.Info,
            Valid = ClassSingle.Valid,
            ValidYear = ClassSingle.ValidYear,
            EngName = ClassSingle.EngName,
            GerName = ClassSingle.GerName,
            FraName = ClassSingle.FraName,
            PorName = ClassSingle.PorName,
            Memo = ClassSingle.Memo,
            WriterDate = ClassSingle.WriterDate,
            UpdaterDate = ClassSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl21Class> ClassesCollection { get; set; } = null!;
    #endregion "Collections"

}
