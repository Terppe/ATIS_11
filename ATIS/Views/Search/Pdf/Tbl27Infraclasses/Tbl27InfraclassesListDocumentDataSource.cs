using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.Tbl27Infraclasses;

namespace ATIS.Views.Search.Pdf.Tbl27Infraclasses;
internal class Tbl27InfraclassesListDocumentDataSource : ObservableObject
{
    private static readonly Tbl27Infraclass InfraclassSingle = new();

    public static Tbl27InfraclassesPdfModel GetInfraclassListDetails(ObservableCollection<Tbl27Infraclass> infraclassesCollection)
    {

        InfraclassesCollection = infraclassesCollection;

        return new Tbl27InfraclassesPdfModel
        {
            InfraclassName = InfraclassSingle.InfraclassName,
            CountId = InfraclassSingle.CountId,
            Synonym = InfraclassSingle.Synonym,
            Author = InfraclassSingle.Author,
            AuthorYear = InfraclassSingle.AuthorYear,
            Info = InfraclassSingle.Info,
            Valid = InfraclassSingle.Valid,
            ValidYear = InfraclassSingle.ValidYear,
            EngName = InfraclassSingle.EngName,
            GerName = InfraclassSingle.GerName,
            FraName = InfraclassSingle.FraName,
            PorName = InfraclassSingle.PorName,
            Memo = InfraclassSingle.Memo,
            WriterDate = InfraclassSingle.WriterDate,
            UpdaterDate = InfraclassSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Tbl27Infraclass> InfraclassesCollection { get; set; } = null!;
    #endregion "Collections"

}
