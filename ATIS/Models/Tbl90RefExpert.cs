using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl90RefExpert : ObservableObject, ICopyable<Tbl90RefExpert>
{
    [Key]
    public int RefExpertId { get; set; }

    [StringLength(300)]
    private string? _refExpertName;
    public string? RefExpertName
    {
        get => _refExpertName;
        set => SetProperty(ref _refExpertName, value);
    }

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


    [StringLength(1000)]
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

    public Tbl90RefExpert Clone()
    {
        return new Tbl90RefExpert
        {
            RefExpertId = RefExpertId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            RefExpertName = RefExpertName,
            Notes = Notes,
            Info = Info,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    public void CopyFrom(Tbl90RefExpert other)
    {
        Debug.WriteLine($"[COPYFROM] {RefExpertName} → {other.RefExpertName}");

        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        RefExpertName = other.RefExpertName;
        Notes = other.Notes;
        Info = other.Info;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }
}
