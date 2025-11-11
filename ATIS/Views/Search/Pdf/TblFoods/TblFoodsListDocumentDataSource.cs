using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Views.Report.Pdf.TblFoods;

namespace ATIS.Views.Search.Pdf.TblFoods;

internal class TblFoodsListDocumentDataSource : ObservableObject
{
    private static readonly TblFood FoodSingle = new();

    public static TblFoodsPdfModel GetFoodListDetails(ObservableCollection<TblFood> foodsCollection)
    {

        FoodsCollection = foodsCollection;

        return new TblFoodsPdfModel
        {
            FoodName = FoodSingle.FoodName,
            CountId = FoodSingle.CountId,
            ShortInfo = FoodSingle.ShortInfo,
            FoodVolume = FoodSingle.FoodVolume,
            Info = FoodSingle.Info,
            Valid = FoodSingle.Valid,
            ValidYear = FoodSingle.ValidYear,
            Memo = FoodSingle.Memo,
            WriterDate = FoodSingle.WriterDate,
            UpdaterDate = FoodSingle.UpdaterDate,
        };
    }

    #region "Collections"

    public static ObservableCollection<TblFood> FoodsCollection { get; set; } = null!;
    #endregion "Collections"

}
