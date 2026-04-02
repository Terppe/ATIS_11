using System.ComponentModel.DataAnnotations;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl90RefSource : ObservableObject, ICopyable<Tbl90RefSource>
{
    [Key]
    public int RefSourceId { get; set; }

    private int _countId;
    public int CountId
    {
        get => _countId;
        set => SetProperty(ref _countId, value);
    }

    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    [StringLength(4)]
    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set => SetProperty(ref _validYear, value);
    }

    [StringLength(300)]
    private string? _refSourceName;
    public string? RefSourceName
    {
        get => _refSourceName;
        set => SetProperty(ref _refSourceName, value);
    }

    [StringLength(4)]
    private string? _sourceYear;
    public string? SourceYear
    {
        get => _sourceYear;
        set => SetProperty(ref _sourceYear, value);
    }

    public string FullName => $"{RefSourceName} {SourceYear}";
    public string RefSourceFullName => $"{RefSourceName} {SourceYear}";

    [StringLength(400)]
    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    [StringLength(100)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
    }

    [StringLength(50)]
    private string? _writer;
    public string? Writer
    {
        get => _writer;
        set => SetProperty(ref _writer, value);
    }

    private DateTime? _writerDate;
    public DateTime? WriterDate
    {
        get => _writerDate;
        set => SetProperty(ref _writerDate, value);
    }

    [StringLength(50)]
    private string? _updater;
    public string? Updater
    {
        get => _updater;
        set => SetProperty(ref _updater, value);
    }

    private DateTime? _updaterDate;
    public DateTime? UpdaterDate
    {
        get => _updaterDate;
        set => SetProperty(ref _updaterDate, value);
    }

    private string? _memo;
    public string? Memo
    {
        get => _memo;
        set => SetProperty(ref _memo, value);
    }

    // ---------------------------------------------------------
    // Clone + CopyFrom (wie bei Tbl90RefExpert)
    // ---------------------------------------------------------

    public Tbl90RefSource Clone()
    {
        return new Tbl90RefSource
        {
            RefSourceId = RefSourceId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            RefSourceName = RefSourceName,
            SourceYear = SourceYear,
            Author = Author,
            Notes = Notes,
            Info = Info,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    public void CopyFrom(Tbl90RefSource other)
    {
        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        RefSourceName = other.RefSourceName;
        SourceYear = other.SourceYear;
        Author = other.Author;
        Notes = other.Notes;
        Info = other.Info;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }
}
