using System.Collections.ObjectModel;
using ATIS.Interfaces;

//    Tbl45FamiliesDocumentDataSourceSkriptdatum:  13.06.2023  10:32    

namespace ATIS.Views.Report.Pdf.TblFoods;


public abstract class TblFoodsDocumentDataSource : ObservableObject
{
    #region [Private Data Members]
    private static Models.TblFood _foodSingle = new();

    #endregion [Private Data Members]      

    //    Part 1
    public static async Task<TblFoodsPdfModel?> GetFoodDetailsAsync(int foodId)
    {
        #region [Private Data Members]

        var dataService = App.GetService<IDataService>();

        ClearAllCollections();

        var foodSingle = await dataService.GetFoodByIdAsync(foodId);

        if (foodSingle == null)
            return null;

        _foodSingle = foodSingle;

        #endregion [Private Data Members]


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

    private static ObservableCollection<Models.TblFood> FoodsCollection { get; set; } = new();

    #endregion "Collections"

    private static void ClearAllCollections()
    {
        _foodSingle = new Models.TblFood();

        FoodsCollection?.Clear();
    }

}


//public abstract class TblFoodsDocumentDataSource : ObservableObject
//{
//    #region [Private Data Members]
//    private static Models.TblFood _foodSingle = new();
//    #endregion [Private Data Members]      

//    //    Part 1
//    public static async Task<TblFoodsPdfModel?> GetFoodDetails(int foodId)
//    {
//        #region [Private Data Members]

//        var dataService = App.GetService<IDataService>();

//        ClearAllCollections();

//        var foodSingle = await dataService.GetFoodByIdAsync(foodId);
//        if (foodSingle == null)
//            return null;

//        _foodSingle = foodSingle;

//        #endregion [Private Data Members]


//        return new TblFoodsPdfModel
//        {
//            FoodName = _foodSingle.FoodName,
//            CountId = _foodSingle.CountId,
//            ShortInfo = _foodSingle.ShortInfo,
//            Symptom = _foodSingle.Symptom,
//            FoodVolume = _foodSingle.FoodVolume,
//            Info = _foodSingle.Info,
//            Valid = _foodSingle.Valid,
//            ValidYear = _foodSingle.ValidYear,
//            Memo = _foodSingle.Memo,
//            WriterDate = _foodSingle.WriterDate,
//            UpdaterDate = _foodSingle.UpdaterDate,
//            //-------------------------------
//            FoodTree = GenrateModels.GenFoods.GenerateFoods(_foodSingle)
//        };
//    }

//    #region "Collections"

//    private static ObservableCollection<Models.TblFood> FoodsCollection { get; set; } = new();

//    #endregion "Collections"

//    private static void ClearAllCollections()
//    {
//        _foodSingle = new Models.TblFood();

//        FoodsCollection?.Clear();
//    }

//}



