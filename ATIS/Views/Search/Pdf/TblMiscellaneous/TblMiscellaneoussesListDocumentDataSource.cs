using System.Collections.ObjectModel;
using ATIS.Views.Report.Pdf.TblMiscellaneous;

namespace ATIS.Views.Search.Pdf.TblMiscellaneous;
internal class TblMiscellaneoussesListDocumentDataSource : ObservableObject
{
    private static readonly Models.TblMiscellaneous MiscellaneousSingle = new();

    public static TblMiscellaneoussesPdfModel GetMiscellaneousListDetails(ObservableCollection<Models.TblMiscellaneous> miscellaneoussCollection)
    {

        MiscellaneoussesCollection = miscellaneoussCollection;

        return new TblMiscellaneoussesPdfModel
        {
            MiscellaneousName = MiscellaneousSingle.MiscellaneousName,
            CountId = MiscellaneousSingle.CountId,
            ShortInfo = MiscellaneousSingle.ShortInfo,
            Thema1 = MiscellaneousSingle.Thema1,
            Thema2 = MiscellaneousSingle.Thema2,
            Info = MiscellaneousSingle.Info,
            Valid = MiscellaneousSingle.Valid,
            ValidYear = MiscellaneousSingle.ValidYear,
            Memo = MiscellaneousSingle.Memo,
            WriterDate = MiscellaneousSingle.WriterDate,
            UpdaterDate = MiscellaneousSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<Models.TblMiscellaneous> MiscellaneoussesCollection { get; set; } = null!;
    #endregion "Collections"

}
