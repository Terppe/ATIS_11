using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.UI.Xaml.Data;

namespace ATIS.DesignSystem.Converters;

public class CommentToEditControlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Models.Tbl93Comment comment)
        {
            return new Tbl93CommentEditControl { Item = comment };
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
