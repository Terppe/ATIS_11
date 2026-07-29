namespace ATIS.Views.Search;

public sealed class SearchTabViewModel
{
    public string TableName { get; set; } = string.Empty;
    public IEnumerable<object> Items { get; set; } = Enumerable.Empty<object>();
}
