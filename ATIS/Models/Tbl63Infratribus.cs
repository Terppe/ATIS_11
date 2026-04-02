using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl63Infratribus : ObservableObject, ICopyable<Tbl63Infratribus>, IDataService.IAuditable
{
    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int InfratribusId { get; set; }

    // ---------------------------------------------------------
    // InfratribusName
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _infratribusName;
    public string? InfratribusName
    {
        get => _infratribusName;
        set => SetProperty(ref _infratribusName, value);
    }

    // ---------------------------------------------------------
    // SubtribusId (FK)
    // ---------------------------------------------------------
    private int? _subtribusId;
    public int? SubtribusId
    {
        get => _subtribusId;
        set => SetProperty(ref _subtribusId, value);
    }

    // ---------------------------------------------------------
    // Navigation: Subtribus
    // ---------------------------------------------------------
    public Tbl60Subtribus? Subtribus { get; set; }

    [NotMapped]
    public string? SubtribusName => Subtribus?.SubtribusName;

    // ---------------------------------------------------------
    // CountId
    // ---------------------------------------------------------
    private int _countId;
    public int CountId
    {
        get => _countId;
        set => SetProperty(ref _countId, value);
    }

    // ---------------------------------------------------------
    // Valid
    // ---------------------------------------------------------
    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    // ---------------------------------------------------------
    // ValidYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set => SetProperty(ref _validYear, value);
    }

    // ---------------------------------------------------------
    // Synonym
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _synonym;
    public string? Synonym
    {
        get => _synonym;
        set => SetProperty(ref _synonym, value);
    }

    // ---------------------------------------------------------
    // Author
    // ---------------------------------------------------------
    [StringLength(60)]
    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    // ---------------------------------------------------------
    // AuthorYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _authorYear;
    public string? AuthorYear
    {
        get => _authorYear;
        set => SetProperty(ref _authorYear, value);
    }

    // ---------------------------------------------------------
    // Info
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
    }

    // ---------------------------------------------------------
    // EngName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _engName;
    public string? EngName
    {
        get => _engName;
        set => SetProperty(ref _engName, value);
    }

    // ---------------------------------------------------------
    // GerName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _gerName;
    public string? GerName
    {
        get => _gerName;
        set => SetProperty(ref _gerName, value);
    }

    // ---------------------------------------------------------
    // FraName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _fraName;
    public string? FraName
    {
        get => _fraName;
        set => SetProperty(ref _fraName, value);
    }

    // ---------------------------------------------------------
    // PorName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _porName;
    public string? PorName
    {
        get => _porName;
        set => SetProperty(ref _porName, value);
    }

    // ---------------------------------------------------------
    // Writer
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _writer;
    public string? Writer
    {
        get => _writer;
        set => SetProperty(ref _writer, value);
    }

    private DateTime _writerDate;
    public DateTime WriterDate
    {
        get => _writerDate;
        set => SetProperty(ref _writerDate, value);
    }

    // ---------------------------------------------------------
    // Updater
    // ---------------------------------------------------------
    [StringLength(50)]
    private string? _updater;
    public string? Updater
    {
        get => _updater;
        set => SetProperty(ref _updater, value);
    }

    private DateTime _updaterDate;
    public DateTime UpdaterDate
    {
        get => _updaterDate;
        set => SetProperty(ref _updaterDate, value);
    }

    // ---------------------------------------------------------
    // Memo
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _memo;
    public string? Memo
    {
        get => _memo;
        set => SetProperty(ref _memo, value);
    }

    // ---------------------------------------------------------
    // CLONE
    // ---------------------------------------------------------
    public Tbl63Infratribus Clone()
    {
        return new Tbl63Infratribus
        {
            InfratribusId = InfratribusId,
            InfratribusName = InfratribusName,
            SubtribusId = SubtribusId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            Synonym = Synonym,
            Author = Author,
            AuthorYear = AuthorYear,
            Info = Info,
            EngName = EngName,
            GerName = GerName,
            FraName = FraName,
            PorName = PorName,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    // ---------------------------------------------------------
    // COPYFROM
    // ---------------------------------------------------------
    public void CopyFrom(Tbl63Infratribus other)
    {
        Debug.WriteLine($"[COPYFROM] {InfratribusName} → {other.InfratribusName}");

        InfratribusName = other.InfratribusName;
        SubtribusId = other.SubtribusId;
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

    [NotMapped]
    public List<Tbl66Genus>? GenusPreview { get; set; }

    [NotMapped]
    public List<Tbl90Reference>? ReferencePreview { get; set; }

    [NotMapped]
    public List<Tbl93Comment>? CommentPreview { get; set; }

}
