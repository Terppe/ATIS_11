using Windows.Foundation;

namespace ATIS.DesignSystem.Components;

public sealed class BreadcrumbWrapPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            child.Measure(new Size(
                double.PositiveInfinity,
                double.PositiveInfinity));

            if (child.DesiredSize.Width > availableSize.Width)
            {
                child.Measure(new Size(
                    availableSize.Width,
                    double.PositiveInfinity));
            }
        }

        return Layout(availableSize.Width, arrange: false);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Layout(finalSize.Width, arrange: true);
        return finalSize;
    }

    private Size Layout(double availableWidth, bool arrange)
    {
        double x = 0;
        double y = 0;
        double rowHeight = 0;
        double width = 0;

        foreach (var child in Children)
        {
            var size = child.DesiredSize;

            if (x > 0 && x + size.Width > availableWidth)
            {
                y += rowHeight;
                x = 0;
                rowHeight = 0;
            }

            if (arrange)
                child.Arrange(new Rect(x, y, size.Width, size.Height));

            x += size.Width;
            width = Math.Max(width, x);
            rowHeight = Math.Max(rowHeight, size.Height);
        }

        return new Size(width, y + rowHeight);
    }
}
