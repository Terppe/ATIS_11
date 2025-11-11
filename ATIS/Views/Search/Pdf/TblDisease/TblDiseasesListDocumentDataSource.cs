using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.TblDisease;

namespace ATIS.Views.Search.Pdf.TblDisease;
internal class TblDiseasesListDocumentDataSource : ObservableObject
{
    private static readonly Models.TblDisease DiseaseSingle = new();

    public static TblDiseasesPdfModel GetDiseaseListDetails(ObservableCollection<Models.TblDisease> diseasesCollection)
    {

        DiseasesCollection = diseasesCollection;

        return new TblDiseasesPdfModel
        {
            DiseaseName = DiseaseSingle.DiseaseName,
            CountId = DiseaseSingle.CountId,
            ShortInfo = DiseaseSingle.ShortInfo,
            Symptom = DiseaseSingle.Symptom,
            Reason = DiseaseSingle.Reason,
            Therapie = DiseaseSingle.Therapie,
            Info = DiseaseSingle.Info,
            Valid = DiseaseSingle.Valid,
            ValidYear = DiseaseSingle.ValidYear,
            Memo = DiseaseSingle.Memo,
            WriterDate = DiseaseSingle.WriterDate,
            UpdaterDate = DiseaseSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Models.TblDisease> DiseasesCollection { get; set; } = null!;
    #endregion "Collections"

}
