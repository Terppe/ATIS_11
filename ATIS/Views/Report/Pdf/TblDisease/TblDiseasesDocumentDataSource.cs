using System.Collections.ObjectModel;
using ATIS.Interfaces;

namespace ATIS.Views.Report.Pdf.TblDisease;


public abstract class TblDiseasesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]
    private static Models.TblDisease _diseaseSingle = new();

    #endregion [Private Data Members]      

    //    Part 1
    public static async Task<TblDiseasesPdfModel?> GetDiseaseDetailsAsync(int diseaseId)
    {
        #region [Private Data Members]

        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var diseaseSingle = await dataService.GetDiseaseByIdAsync(diseaseId);

        if (diseaseSingle == null)
            return null;

        _diseaseSingle = diseaseSingle;

        #endregion [Private Data Members]


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

    private static ObservableCollection<Models.TblDisease> DiseasesCollection { get; set; } = new();

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _diseaseSingle = new Models.TblDisease();

        DiseasesCollection?.Clear();
    }

}
