namespace ATIS.Views.Taxonomy;

public sealed class TaxonomyDisplayItem<T>
{
    public T Item { get; set; } = default!;
    public Thickness LeftMargin { get; set; }
}
