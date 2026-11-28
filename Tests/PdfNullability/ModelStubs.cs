global using CommunityToolkit.Mvvm.ComponentModel;

namespace ATIS.Models;

public class Tbl66Genus;
public class Tbl68Speciesgroup;
public class Tbl75Species;

public class Tbl63Infratribus
{
    public string? InfratribusName { get; set; }
    public int CountId { get; set; }
    public string? Synonym { get; set; }
    public string? Author { get; set; }
    public string? AuthorYear { get; set; }
    public string? Info { get; set; }
    public bool? Valid { get; set; }
    public string? ValidYear { get; set; }
    public string? EngName { get; set; }
    public string? GerName { get; set; }
    public string? FraName { get; set; }
    public string? PorName { get; set; }
    public string? Memo { get; set; }
    public DateTime WriterDate { get; set; }
    public DateTime UpdaterDate { get; set; }
}
