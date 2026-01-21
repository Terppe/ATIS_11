using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Data;

namespace ATIS.Helpers.Converters;

public class RowIndexToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DataGridRow row)
        {
            int index = row.GetIndex();
            return index % 2 == 1
                ? Application.Current.Resources["DataGridAlternatingRowBackgroundBrush"]
                : Application.Current.Resources["DataGridRowBackgroundBrush"];
        }

        return Application.Current.Resources["DataGridRowBackgroundBrush"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
