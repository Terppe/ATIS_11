namespace ATIS.Views.Taxonomy;

public sealed class TaxonomyLine<T>
{
    public T Item { get; set; } = default!;
    public Thickness HeaderMargin { get; set; }
    public bool ShowAsHyperlink { get; set; } = true;
}
