using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl90RefAuthor : ObservableObject, ICopyable<Tbl90RefAuthor>
{
    [Key]
    public int RefAuthorId { get; set; }

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
    // RefAuthorName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _refAuthorName;
    public string? RefAuthorName
    {
        get => _refAuthorName;
        set => SetProperty(ref _refAuthorName, value);
    }

    // ---------------------------------------------------------
    // PublicationYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _publicationYear;
    public string? PublicationYear
    {
        get => _publicationYear;
        set => SetProperty(ref _publicationYear, value);
    }

    // ---------------------------------------------------------
    // ArticelTitle
    // ---------------------------------------------------------
    [StringLength(300)]
    private string? _articelTitle;
    public string? ArticelTitle
    {
        get => _articelTitle;
        set => SetProperty(ref _articelTitle, value);
    }

    // ---------------------------------------------------------
    // BookName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _bookName;
    public string? BookName
    {
        get => _bookName;
        set => SetProperty(ref _bookName, value);
    }

    // ---------------------------------------------------------
    // Page (wird im Modul nicht genutzt)
    // ---------------------------------------------------------
    [StringLength(60)]
    private string? _page;
    public string? Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }

    // ---------------------------------------------------------
    // Page1 (wird im Modul genutzt)
    // ---------------------------------------------------------
    [StringLength(60)]
    private string? _page1;
    public string? Page1
    {
        get => _page1;
        set => SetProperty(ref _page1, value);
    }

    // ---------------------------------------------------------
    // Publisher
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _publisher;
    public string? Publisher
    {
        get => _publisher;
        set => SetProperty(ref _publisher, value);
    }

    // ---------------------------------------------------------
    // PublicationPlace
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _publicationPlace;
    public string? PublicationPlace
    {
        get => _publicationPlace;
        set => SetProperty(ref _publicationPlace, value);
    }

    // ---------------------------------------------------------
    // ISBN
    // ---------------------------------------------------------
    [StringLength(60)]
    private string? _isbn;
    public string? ISBN
    {
        get => _isbn;
        set => SetProperty(ref _isbn, value);
    }

    // ---------------------------------------------------------
    // Notes
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    // ---------------------------------------------------------
    // Info
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
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

    private DateTime? _writerDate;
    public DateTime? WriterDate
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

    private DateTime? _updaterDate;
    public DateTime? UpdaterDate
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
    // Clone
    // ---------------------------------------------------------
    public Tbl90RefAuthor Clone()
    {
        return new Tbl90RefAuthor
        {
            RefAuthorId = RefAuthorId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            RefAuthorName = RefAuthorName,
            PublicationYear = PublicationYear,
            ArticelTitle = ArticelTitle,
            BookName = BookName,
            Page = Page,
            Page1 = Page1,
            Publisher = Publisher,
            PublicationPlace = PublicationPlace,
            ISBN = ISBN,
            Notes = Notes,
            Info = Info,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    // ---------------------------------------------------------
    // CopyFrom
    // ---------------------------------------------------------
    public void CopyFrom(Tbl90RefAuthor other)
    {
        Debug.WriteLine($"[COPYFROM] {RefAuthorName} → {other.RefAuthorName}");

        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        RefAuthorName = other.RefAuthorName;
        PublicationYear = other.PublicationYear;
        ArticelTitle = other.ArticelTitle;
        BookName = other.BookName;
        Page = other.Page;
        Page1 = other.Page1;
        Publisher = other.Publisher;
        PublicationPlace = other.PublicationPlace;
        ISBN = other.ISBN;
        Notes = other.Notes;
        Info = other.Info;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }
}
