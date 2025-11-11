using System.Collections.ObjectModel;
using ATIS.Services;

namespace ATIS.Views.Report.Pdf.TblDisease;
public abstract class TblDiseasesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static Models.TblDisease _diseaseSingle = new();
    #endregion [Private Data Members]      

    //    Part 1 
    public static TblDiseasesPdfModel GetDiseaseDetails(int diseaseId)
    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        DiseasesCollection = dataService.CollDiseasesByDiseaseId(diseaseId);

        _diseaseSingle = dataService.GetDiseaseSingleByDiseaseId(diseaseId);

        return new TblDiseasesPdfModel
        {
            DiseaseName = _diseaseSingle.DiseaseName,
            CountId = _diseaseSingle.CountId,
            ShortInfo = _diseaseSingle.ShortInfo,
            Symptom = _diseaseSingle.Symptom,
            Reason = _diseaseSingle.Reason,
            Therapie = _diseaseSingle.Therapie,
            Info = _diseaseSingle.Info,
            Valid = _diseaseSingle.Valid,
            ValidYear = _diseaseSingle.ValidYear,
            Memo = _diseaseSingle.Memo,
            WriterDate = _diseaseSingle.WriterDate,
            UpdaterDate = _diseaseSingle.UpdaterDate,
            //-------------------------------
            DiseaseTree = GenrateModels.GenDiseases.GenerateDiseases(_diseaseSingle)
        };
    }

    #region "Collections"

    private static ObservableCollection<Models.TblDisease> DiseasesCollection { get; set; } = null!;

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _diseaseSingle = new Models.TblDisease();

        DiseasesCollection?.Clear();
    }

}
