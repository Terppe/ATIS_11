
namespace Atis.Helpers;

public static class AtisGridSpacing
{
    public static readonly DependencyProperty ColumnSpacingProperty =
        DependencyProperty.RegisterAttached(
            "ColumnSpacing",
            typeof(double),
            typeof(AtisGridSpacing),
            new PropertyMetadata(0.0, OnSpacingChanged));

    public static readonly DependencyProperty RowSpacingProperty =
        DependencyProperty.RegisterAttached(
            "RowSpacing",
            typeof(double),
            typeof(AtisGridSpacing),
            new PropertyMetadata(0.0, OnSpacingChanged));

    public static double GetColumnSpacing(DependencyObject obj) =>
        (double)obj.GetValue(ColumnSpacingProperty);

    public static void SetColumnSpacing(DependencyObject obj, double value) =>
        obj.SetValue(ColumnSpacingProperty, value);

    public static double GetRowSpacing(DependencyObject obj) =>
        (double)obj.GetValue(RowSpacingProperty);

    public static void SetRowSpacing(DependencyObject obj, double value) =>
        obj.SetValue(RowSpacingProperty, value);

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Grid grid)
        {
            grid.ColumnSpacing = GetColumnSpacing(grid);
            grid.RowSpacing = GetRowSpacing(grid);
        }
    }
}
