
namespace Atis.Helpers;

public static class AtisRowExtensions
{
    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.RegisterAttached(
            "Rows",
            typeof(int),
            typeof(AtisRowExtensions),
            new PropertyMetadata(0, OnRowsChanged));

    public static int GetRows(DependencyObject obj) =>
        (int)obj.GetValue(RowsProperty);

    public static void SetRows(DependencyObject obj, int value) =>
        obj.SetValue(RowsProperty, value);

    private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Grid grid)
            return;

        int count = (int)e.NewValue;

        grid.RowDefinitions.Clear();

        for (int i = 0; i < count; i++)
        {
            grid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            );
        }
    }
}
