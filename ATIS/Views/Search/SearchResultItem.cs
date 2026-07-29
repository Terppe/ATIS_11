namespace ATIS.Views.Search;

public class SearchResultItem
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }

    public string IdDisplay => $"{EntityType}-{EntityId}";
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
}
