
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class Tbl90RefAuthor : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int RefAuthorId { get; set; }

    // ---------------------------------------------------------
    // RefAuthorName
    // ---------------------------------------------------------
    [StringLength(200)]
    private string? _refAuthorName;
    public string? RefAuthorName
    {
        get => _refAuthorName;
        set
        {
            SetProperty(ref _refAuthorName, value);
            ValidateRefAuthorName();
        }
    }

    private string? _refAuthorNameError;
    [NotMapped]
    public string? RefAuthorNameError
    {
        get => _refAuthorNameError;
        set => SetProperty(ref _refAuthorNameError, value);
    }

    private bool _hasRefAuthorNameError;
    [NotMapped]
    public bool HasRefAuthorNameError
    {
        get => _hasRefAuthorNameError;
        set
        {
            if (_hasRefAuthorNameError != value)
            {
                _hasRefAuthorNameError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // PublicationYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _publicationYear;
    public string? PublicationYear
    {
        get => _publicationYear;
        set
        {
            if (SetProperty(ref _publicationYear, value))
                ValidatePublicationYear();
        }
    }

    private string? _publicationYearError;
    [NotMapped]
    public string? PublicationYearError
    {
        get => _publicationYearError;
        set => SetProperty(ref _publicationYearError, value);
    }

    private bool _hasPublicationYearError;
    [NotMapped]
    public bool HasPublicationYearError
    {
        get => _hasPublicationYearError;
        set
        {
            if (_hasPublicationYearError != value)
            {
                _hasPublicationYearError = value;
                OnPropertyChanged();
            }
        }
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
    // Page1
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
    private bool _valid;
    public bool Valid
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
        set
        {
            if (SetProperty(ref _validYear, value))
            {
                ValidateValidYear();
            }
        }
    }

    private string? _validYearError;
    [NotMapped]
    public string? ValidYearError
    {
        get => _validYearError;
        set => SetProperty(ref _validYearError, value);
    }

    private bool _hasValidYearError;
    [NotMapped]
    public bool HasValidYearError
    {
        get => _hasValidYearError;
        set => SetProperty(ref _hasValidYearError, value);
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
    // IHierarchicalEntity
    // ---------------------------------------------------------
    public int GetEntityId() => RefAuthorId;
    public string GetDisplayName() => RefAuthorName ?? $"Author {RefAuthorId}";

    // ---------------------------------------------------------
    // VALIDIERUNG
    // ---------------------------------------------------------
    private void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
            _errors[propertyName].Clear();
    }

    private void AddError(string propertyName, string errorMessage)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(errorMessage))
            _errors[propertyName].Add(errorMessage);
    }

    private void UpdateErrorProperty(string propertyName)
    {
        var hasErrors = _errors.ContainsKey(propertyName) && _errors[propertyName].Count > 0;
        var errorMessage = hasErrors ? string.Join(" | ", _errors[propertyName]) : "";

        switch (propertyName)
        {
            case nameof(RefAuthorName):
                RefAuthorNameError = errorMessage;
                HasRefAuthorNameError = hasErrors;
                break;

            case nameof(PublicationYear):
                PublicationYearError = errorMessage;
                HasPublicationYearError = hasErrors;
                break;
            case nameof(ValidYear):
                ValidYearError = errorMessage;
                HasValidYearError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    private void ValidateRefAuthorName()
    {
        ClearErrors(nameof(RefAuthorName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefAuthorName))
            AddError(nameof(RefAuthorName), loc.GetLocalizedString("Error_AuthorName_Empty"));

        if (RefAuthorName?.Length > 200)
            AddError(nameof(RefAuthorName), loc.GetLocalizedString("Error_AuthorName_TooLong"));

        UpdateErrorProperty(nameof(RefAuthorName));
    }

    // ---------------------------------------------------------
    // Validate ValidYear
    // ---------------------------------------------------------

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();  // ← Direkt verwenden

        if (string.IsNullOrWhiteSpace(ValidYear))
            goto Notify;

        if (ValidYear!.Length != 4)
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Length"));
            goto Notify;
        }

        if (!int.TryParse(ValidYear, out var year))
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Range"));

        Notify:
        UpdateErrorProperty(nameof(ValidYear));
    }


    private void ValidatePublicationYear()
    {
        ClearErrors(nameof(PublicationYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(PublicationYear))
            goto Notify;

        if (PublicationYear!.Length != 4)
        {
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_Year_Length"));
            goto Notify;
        }

        if (!int.TryParse(PublicationYear, out var year))
        {
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_Year_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_Year_Range"));

Notify:
        UpdateErrorProperty(nameof(PublicationYear));
    }

    public bool IsValid()
    {
        ValidateRefAuthorName();
        ValidateValidYear();
        ValidatePublicationYear();

        return !HasRefAuthorNameError && !HasValidYearError && !HasPublicationYearError;
    }
}



//public class Tbl90RefAuthor : ObservableObject, ICopyable<Tbl90RefAuthor>
//{
//    [Key]
//    public int RefAuthorId { get; set; }

//    // ---------------------------------------------------------
//    // CountId
//    // ---------------------------------------------------------
//    private int _countId;
//    public int CountId
//    {
//        get => _countId;
//        set => SetProperty(ref _countId, value);
//    }

//    // ---------------------------------------------------------
//    // Valid
//    // ---------------------------------------------------------
//    private bool? _valid;
//    public bool? Valid
//    {
//        get => _valid;
//        set => SetProperty(ref _valid, value);
//    }

//    // ---------------------------------------------------------
//    // ValidYear
//    // ---------------------------------------------------------
//    [StringLength(4)]
//    private string? _validYear;
//    public string? ValidYear
//    {
//        get => _validYear;
//        set => SetProperty(ref _validYear, value);
//    }

//    // ---------------------------------------------------------
//    // RefAuthorName
//    // ---------------------------------------------------------
//    [StringLength(200)]
//    private string? _refAuthorName;
//    public string? RefAuthorName
//    {
//        get => _refAuthorName;
//        set => SetProperty(ref _refAuthorName, value);
//    }

//    // ---------------------------------------------------------
//    // PublicationYear
//    // ---------------------------------------------------------
//    [StringLength(4)]
//    private string? _publicationYear;
//    public string? PublicationYear
//    {
//        get => _publicationYear;
//        set => SetProperty(ref _publicationYear, value);
//    }

//    // ---------------------------------------------------------
//    // ArticelTitle
//    // ---------------------------------------------------------
//    [StringLength(300)]
//    private string? _articelTitle;
//    public string? ArticelTitle
//    {
//        get => _articelTitle;
//        set => SetProperty(ref _articelTitle, value);
//    }

//    // ---------------------------------------------------------
//    // BookName
//    // ---------------------------------------------------------
//    [StringLength(200)]
//    private string? _bookName;
//    public string? BookName
//    {
//        get => _bookName;
//        set => SetProperty(ref _bookName, value);
//    }

//    // ---------------------------------------------------------
//    // Page (wird im Modul nicht genutzt)
//    // ---------------------------------------------------------
//    [StringLength(60)]
//    private string? _page;
//    public string? Page
//    {
//        get => _page;
//        set => SetProperty(ref _page, value);
//    }

//    // ---------------------------------------------------------
//    // Page1 (wird im Modul genutzt)
//    // ---------------------------------------------------------
//    [StringLength(60)]
//    private string? _page1;
//    public string? Page1
//    {
//        get => _page1;
//        set => SetProperty(ref _page1, value);
//    }

//    // ---------------------------------------------------------
//    // Publisher
//    // ---------------------------------------------------------
//    [StringLength(200)]
//    private string? _publisher;
//    public string? Publisher
//    {
//        get => _publisher;
//        set => SetProperty(ref _publisher, value);
//    }

//    // ---------------------------------------------------------
//    // PublicationPlace
//    // ---------------------------------------------------------
//    [StringLength(100)]
//    private string? _publicationPlace;
//    public string? PublicationPlace
//    {
//        get => _publicationPlace;
//        set => SetProperty(ref _publicationPlace, value);
//    }

//    // ---------------------------------------------------------
//    // ISBN
//    // ---------------------------------------------------------
//    [StringLength(60)]
//    private string? _isbn;
//    public string? ISBN
//    {
//        get => _isbn;
//        set => SetProperty(ref _isbn, value);
//    }

//    // ---------------------------------------------------------
//    // Notes
//    // ---------------------------------------------------------
//    [StringLength(1000)]
//    private string? _notes;
//    public string? Notes
//    {
//        get => _notes;
//        set => SetProperty(ref _notes, value);
//    }

//    // ---------------------------------------------------------
//    // Info
//    // ---------------------------------------------------------
//    [StringLength(100)]
//    private string? _info;
//    public string? Info
//    {
//        get => _info;
//        set => SetProperty(ref _info, value);
//    }

//    // ---------------------------------------------------------
//    // Writer
//    // ---------------------------------------------------------
//    [StringLength(50)]
//    private string? _writer;
//    public string? Writer
//    {
//        get => _writer;
//        set => SetProperty(ref _writer, value);
//    }

//    private DateTime? _writerDate;
//    public DateTime? WriterDate
//    {
//        get => _writerDate;
//        set => SetProperty(ref _writerDate, value);
//    }

//    // ---------------------------------------------------------
//    // Updater
//    // ---------------------------------------------------------
//    [StringLength(50)]
//    private string? _updater;
//    public string? Updater
//    {
//        get => _updater;
//        set => SetProperty(ref _updater, value);
//    }

//    private DateTime? _updaterDate;
//    public DateTime? UpdaterDate
//    {
//        get => _updaterDate;
//        set => SetProperty(ref _updaterDate, value);
//    }

//    // ---------------------------------------------------------
//    // Memo
//    // ---------------------------------------------------------
//    [StringLength(1000)]
//    private string? _memo;
//    public string? Memo
//    {
//        get => _memo;
//        set => SetProperty(ref _memo, value);
//    }

//    // ---------------------------------------------------------
//    // Clone
//    // ---------------------------------------------------------
//    public Tbl90RefAuthor Clone()
//    {
//        return new Tbl90RefAuthor
//        {
//            RefAuthorId = RefAuthorId,
//            CountId = CountId,
//            Valid = Valid,
//            ValidYear = ValidYear,
//            RefAuthorName = RefAuthorName,
//            PublicationYear = PublicationYear,
//            ArticelTitle = ArticelTitle,
//            BookName = BookName,
//            Page = Page,
//            Page1 = Page1,
//            Publisher = Publisher,
//            PublicationPlace = PublicationPlace,
//            ISBN = ISBN,
//            Notes = Notes,
//            Info = Info,
//            Writer = Writer,
//            WriterDate = WriterDate,
//            Updater = Updater,
//            UpdaterDate = UpdaterDate,
//            Memo = Memo
//        };
//    }

//    // ---------------------------------------------------------
//    // CopyFrom
//    // ---------------------------------------------------------
//    public void CopyFrom(Tbl90RefAuthor other)
//    {
//        Debug.WriteLine($"[COPYFROM] {RefAuthorName} → {other.RefAuthorName}");

//        CountId = other.CountId;
//        Valid = other.Valid;
//        ValidYear = other.ValidYear;
//        RefAuthorName = other.RefAuthorName;
//        PublicationYear = other.PublicationYear;
//        ArticelTitle = other.ArticelTitle;
//        BookName = other.BookName;
//        Page = other.Page;
//        Page1 = other.Page1;
//        Publisher = other.Publisher;
//        PublicationPlace = other.PublicationPlace;
//        ISBN = other.ISBN;
//        Notes = other.Notes;
//        Info = other.Info;
//        Writer = other.Writer;
//        WriterDate = other.WriterDate;
//        Updater = other.Updater;
//        UpdaterDate = other.UpdaterDate;
//        Memo = other.Memo;
//    }
//}
