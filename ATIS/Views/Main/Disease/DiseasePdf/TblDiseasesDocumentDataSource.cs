using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;
using ATIS.Views.Report;

namespace ATIS.Views.Main.Disease.DiseasePdf;
public abstract class TblDiseasesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static TblDisease _diseaseSingle = new();

    #endregion [Private Data Members]      

    public static TblDiseasesPdfModel GetDiseaseDetails(int diseaseId)
    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        _diseaseSingle = dataService.GetDiseaseSingleByDiseaseId(diseaseId);

        return new Disease.DiseasePdf.TblDiseasesPdfModel
        {
            DiseaseName = _diseaseSingle.DiseaseName,
            CountId = _diseaseSingle.CountId,
            Valid = _diseaseSingle.Valid,
            ValidYear = _diseaseSingle.ValidYear,
            ShortInfo = _diseaseSingle.ShortInfo,
            Symptom = _diseaseSingle.Symptom,
            Reason = _diseaseSingle.Reason,
            Therapie = _diseaseSingle.Therapie,
            Info = _diseaseSingle.Info,
            Memo = _diseaseSingle.Memo,
            WriterDate = _diseaseSingle.WriterDate,
            UpdaterDate = _diseaseSingle.UpdaterDate,
            //-------------------------------
            DiseaseTree = GenrateModels.GenDiseases.GenerateDiseases(_diseaseSingle),
        };

    }

    #region "Collections"




    public static ObservableCollection<Tbl81Image> ImagesCollection
    {
        get; set;
    } = null!;

    #endregion "Collections"

    public static void ClearAllCollections()
    {
        _diseaseSingle = new TblDisease();

    }

}
