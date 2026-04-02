using System.ComponentModel.DataAnnotations;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl60Subtribus : ObservableObject, ICopyable<Tbl60Subtribus>
{
    [Key]
    public int SubtribusId { get; init; }

    [Required, StringLength(100)]
    public string SubtribusName { get; set; } = string.Empty;

    public int TribusId { get; set; }
    public int CountId { get; set; }
    public bool Valid { get; set; }

    [StringLength(4)]
    public string? ValidYear { get; set; }

    [StringLength(1000)]
    public string? Synonym { get; set; }

    [StringLength(60)]
    public string? Author { get; set; }

    [StringLength(4)]
    public string? AuthorYear { get; set; }

    [StringLength(1000)]
    public string? Info { get; set; }

    [StringLength(200)]
    public string? EngName { get; set; }

    [StringLength(200)]
    public string? GerName { get; set; }

    [StringLength(200)]
    public string? FraName { get; set; }

    [StringLength(200)]
    public string? PorName { get; set; }

    [StringLength(50)]
    public string Writer { get; set; } = string.Empty;

    public DateTime WriterDate { get; set; }

    [StringLength(50)]
    public string Updater { get; set; } = string.Empty;

    public DateTime UpdaterDate { get; set; }

    [StringLength(1000)]
    public string? Memo { get; set; }

    public Tbl60Subtribus Clone() => (Tbl60Subtribus)MemberwiseClone();

    public void CopyFrom(Tbl60Subtribus other)
    {
        SubtribusName = other.SubtribusName;
        TribusId = other.TribusId;
        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        Synonym = other.Synonym;
        Author = other.Author;
        AuthorYear = other.AuthorYear;
        Info = other.Info;
        EngName = other.EngName;
        GerName = other.GerName;
        FraName = other.FraName;
        PorName = other.PorName;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }
}
