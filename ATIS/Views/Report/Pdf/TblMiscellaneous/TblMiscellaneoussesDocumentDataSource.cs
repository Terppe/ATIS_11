using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;

namespace ATIS.Views.Report.Pdf.TblMiscellaneous;
public abstract class TblMiscellaneoussesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static Models.TblMiscellaneous _miscellaneousSingle = new();
    #endregion [Private Data Members]      

    //    Part 1 
    public static TblMiscellaneoussesPdfModel GetMiscellaneousDetails(int foodId)
    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();



        MiscellaneoussesCollection = dataService.CollMiscellaneoussesByMiscellaneousId(foodId);

        _miscellaneousSingle = dataService.GetMiscellaneousSingleByMiscellaneousId(foodId);

        return new TblMiscellaneoussesPdfModel
        {
            MiscellaneousName = _miscellaneousSingle.MiscellaneousName,
            CountId = _miscellaneousSingle.CountId,
            ShortInfo = _miscellaneousSingle.ShortInfo,
            Thema1 = _miscellaneousSingle.Thema1,
            Thema2 = _miscellaneousSingle.Thema2,
            Info = _miscellaneousSingle.Info,
            Valid = _miscellaneousSingle.Valid,
            ValidYear = _miscellaneousSingle.ValidYear,
            Memo = _miscellaneousSingle.Memo,
            WriterDate = _miscellaneousSingle.WriterDate,
            UpdaterDate = _miscellaneousSingle.UpdaterDate,
            //-------------------------------
            MiscellaneousTree = GenrateModels.GenMiscellaneousses.GenerateMiscellaneoussess(_miscellaneousSingle)
        };
    }

    #region "Collections"

    private static ObservableCollection<Models.TblMiscellaneous> MiscellaneoussesCollection { get; set; } = null!;

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _miscellaneousSingle = new Models.TblMiscellaneous();

        MiscellaneoussesCollection?.Clear();
    }

}
