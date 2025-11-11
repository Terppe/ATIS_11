using System.Collections.ObjectModel;
using ATIS.Models;
using ATIS.Services;

//    Tbl45FamiliesDocumentDataSourceSkriptdatum:  13.06.2023  10:32    

namespace ATIS.Views.Report.Pdf.TblFoods;
public abstract class TblFoodsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]

    private static TblFood _foodSingle = new();
    #endregion [Private Data Members]      

    //    Part 1 
    public static TblFoodsPdfModel GetFoodDetails(int foodId)
    {
        #region [Private Data Members]

        var dataService = new DataService();

        #endregion [Private Data Members]

        ClearAllCollections();

        FoodsCollection = dataService.CollFoodsByFoodId(foodId);

        _foodSingle = dataService.GetFoodSingleByFoodId(foodId);

        return new TblFoodsPdfModel
        {
            FoodName = _foodSingle.FoodName,
            CountId = _foodSingle.CountId,
            ShortInfo = _foodSingle.ShortInfo,
            Symptom = _foodSingle.Symptom,
            FoodVolume = _foodSingle.FoodVolume,
            Info = _foodSingle.Info,
            Valid = _foodSingle.Valid,
            ValidYear = _foodSingle.ValidYear,
            Memo = _foodSingle.Memo,
            WriterDate = _foodSingle.WriterDate,
            UpdaterDate = _foodSingle.UpdaterDate,
            //-------------------------------
            FoodTree = GenrateModels.GenFoods.GenerateFoods(_foodSingle)
        };
    }

    #region "Collections"

    private static ObservableCollection<TblFood> FoodsCollection { get; set; } = null!;

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _foodSingle = new TblFood();

        FoodsCollection?.Clear();
    }

}
