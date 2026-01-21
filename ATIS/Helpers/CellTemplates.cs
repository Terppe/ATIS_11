using ATIS.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ATIS.Helpers;

public static class CellTemplates
{
    public static Func<object, FrameworkElement> FoodId =>
        row => new TextBlock
        {
            Text = ((TblFood)row).FoodId.ToString(),
            Padding = new Thickness(8, 0, 8, 0)
        };

    public static Func<object, FrameworkElement> FoodName =>
        row => new TextBlock
        {
            Text = ((TblFood)row).FoodName,
            Padding = new Thickness(8, 0, 8, 0)
        };

    public static Func<object, FrameworkElement> ShortInfo =>
        row => new TextBlock
        {
            Text = ((TblFood)row).ShortInfo,
            Padding = new Thickness(8, 0, 8, 0)
        };
}
