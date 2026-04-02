using System.ComponentModel.DataAnnotations;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl66Genus : ObservableObject, ICopyable<Tbl66Genus>, IDataService.IAuditable
{
    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int GenusId { get; set; }

    // ---------------------------------------------------------
    // GenusName (Pflicht)
    // ---------------------------------------------------------
    [Required]
    [StringLength(100)]
    private string _genusName = string.Empty;
    public string GenusName
    {
        get => _genusName;
        set => SetProperty(ref _genusName, value);
    }

    // ---------------------------------------------------------
    // InfratribusId (Pflicht)
    // ---------------------------------------------------------
    private int _infratribusId;
    public int InfratribusId
    {
        get => _infratribusId;
        set => SetProperty(ref _infratribusId, value);
    }

    // ---------------------------------------------------------
    // CountId (Pflicht)
    // ---------------------------------------------------------
    private int _countId;
    public int CountId
    {
        get => _countId;
        set => SetProperty(ref _countId, value);
    }

    // ---------------------------------------------------------
    // Valid (Pflicht)
    // ---------------------------------------------------------
    private bool _valid;
    public bool Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    // ---------------------------------------------------------
    // ValidYear (optional)
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set => SetProperty(ref _validYear, value);
    }

    // ---------------------------------------------------------
    // Synonym (optional)
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _synonym;
    public string? Synonym
    {
        get => _synonym;
        set => SetProperty(ref _synonym, value);
    }

    // ---------------------------------------------------------
    // Author (optional)
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    // ---------------------------------------------------------
    // AuthorYear (optional)
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _authorYear;
    public string? AuthorYear
    {
        get => _authorYear;
        set => SetProperty(ref _authorYear, value);
    }

    // ---------------------------------------------------------
    // Info (optional)
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
    }

    // ---------------------------------------------------------
    // EngName (optional)
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _engName;
    public string? EngName
    {
        get => _engName;
        set => SetProperty(ref _engName, value);
    }

    // ---------------------------------------------------------
    // GerName (optional)
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _gerName;
    public string? GerName
    {
        get => _gerName;
        set => SetProperty(ref _gerName, value);
    }

    // ---------------------------------------------------------
    // FraName (optional)
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _fraName;
    public string? FraName
    {
        get => _fraName;
        set => SetProperty(ref _fraName, value);
    }

    // ---------------------------------------------------------
    // PorName (optional)
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _porName;
    public string? PorName
    {
        get => _porName;
        set => SetProperty(ref _porName, value);
    }

    // ---------------------------------------------------------
    // Writer (Pflicht)
    // ---------------------------------------------------------
    [StringLength(50)]
    private string _writer = string.Empty;
    public string Writer
    {
        get => _writer;
        set => SetProperty(ref _writer, value);
    }

    // ---------------------------------------------------------
    // WriterDate (Pflicht)
    // ---------------------------------------------------------
    private DateTime _writerDate;
    public DateTime WriterDate
    {
        get => _writerDate;
        set => SetProperty(ref _writerDate, value);
    }

    // ---------------------------------------------------------
    // Updater (Pflicht)
    // ---------------------------------------------------------
    [StringLength(50)]
    private string _updater = string.Empty;
    public string Updater
    {
        get => _updater;
        set => SetProperty(ref _updater, value);
    }

    // ---------------------------------------------------------
    // UpdaterDate (Pflicht)
    // ---------------------------------------------------------
    private DateTime _updaterDate;
    public DateTime UpdaterDate
    {
        get => _updaterDate;
        set => SetProperty(ref _updaterDate, value);
    }

    // ---------------------------------------------------------
    // Memo (optional)
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
    public Tbl66Genus Clone()
    {
        return new Tbl66Genus
        {
            GenusId = GenusId,
            GenusName = GenusName,
            InfratribusId = InfratribusId,
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
    public void CopyFrom(Tbl66Genus other)
    {
        GenusName = other.GenusName;
        InfratribusId = other.InfratribusId;
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
