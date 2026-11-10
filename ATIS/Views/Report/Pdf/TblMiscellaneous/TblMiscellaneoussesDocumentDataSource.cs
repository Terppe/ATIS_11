using System.Collections.ObjectModel;
using ATIS.Interfaces;

namespace ATIS.Views.Report.Pdf.TblMiscellaneous;

public abstract class TblMiscellaneoussesDocumentDataSource : ObservableObject
{
    #region [Private Data Members]
    private static Models.TblMiscellaneous _miscellaneousSingle = new();

    #endregion [Private Data Members]      

    //    Part 1
    public static async Task<TblMiscellaneoussesPdfModel?> GetMiscellaneousDetailsAsync(int miscellaneousId)
    {
        #region [Private Data Members]

        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var miscellaneousSingle = await dataService.GetMiscellaneousByIdAsync(miscellaneousId);

        if (miscellaneousSingle == null)
            return null;

        _miscellaneousSingle = miscellaneousSingle;

        #endregion [Private Data Members]


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

    private static ObservableCollection<Models.TblMiscellaneous> MiscellaneoussesCollection { get; set; } = new();

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _miscellaneousSingle = new Models.TblMiscellaneous();

        MiscellaneoussesCollection?.Clear();
    }

}



//public abstract class TblMiscellaneoussesDocumentDataSource : ObservableObject
//{
//    #region [Private Data Members]
//    private static Models.TblMiscellaneous _miscellaneousSingle = new();
//    #endregion [Private Data Members]      

//    //    Part 1
//    public static async Task<TblMiscellaneoussesPdfModel?> GetMiscellaneousDetailsAsync(int miscellaneousId)
//    {
//        #region [Private Data Members]

//        var dataService = App.GetService<IDataService>();

//        ClearAllCollections();

//        var miscellaneousSingle = await dataService.GetMiscellaneousByIdAsync(miscellaneousId);
//        if (miscellaneousSingle == null)
//            return null;

//        _miscellaneousSingle = miscellaneousSingle;

//        #endregion [Private Data Members]

//        ClearAllCollections();


//        return new TblMiscellaneoussesPdfModel
//        {
//            MiscellaneousName = _miscellaneousSingle.MiscellaneousName,
//            CountId = _miscellaneousSingle.CountId,
//            ShortInfo = _miscellaneousSingle.ShortInfo,
//            Thema1 = _miscellaneousSingle.Thema1,
//            Thema2 = _miscellaneousSingle.Thema2,
//            Info = _miscellaneousSingle.Info,
//            Valid = _miscellaneousSingle.Valid,
//            ValidYear = _miscellaneousSingle.ValidYear,
//            Memo = _miscellaneousSingle.Memo,
//            WriterDate = _miscellaneousSingle.WriterDate,
//            UpdaterDate = _miscellaneousSingle.UpdaterDate,
//            //-------------------------------
//            MiscellaneousTree = GenrateModels.GenMiscellaneousses.GenerateMiscellaneoussess(_miscellaneousSingle)
//        };
//    }

//    #region "Collections"

//    private static ObservableCollection<Models.TblMiscellaneous> MiscellaneoussesCollection { get; set; } = new();

//    #endregion "Collections"

//    private static void ClearAllCollections()
//    {
//        _miscellaneousSingle = new Models.TblMiscellaneous();

//        MiscellaneoussesCollection?.Clear();
//    }

//}
