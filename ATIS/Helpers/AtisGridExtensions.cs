
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Atis.Helpers;

public static class AtisGridExtensions
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.RegisterAttached(
            "Columns",
            typeof(int),
            typeof(AtisGridExtensions),
            new PropertyMetadata(0, OnColumnsChanged));

    public static int GetColumns(DependencyObject obj) =>
        (int)obj.GetValue(ColumnsProperty);

    public static void SetColumns(DependencyObject obj, int value) =>
        obj.SetValue(ColumnsProperty, value);

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Grid grid)
            return;

        int count = (int)e.NewValue;

        grid.ColumnDefinitions.Clear();

        for (int i = 0; i < count; i++)
        {
            grid.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            );
        }
    }
}
