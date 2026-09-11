
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class Tbl90RefSource : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int RefSourceId { get; set; }

    // ---------------------------------------------------------
    // RefSourceName
    // ---------------------------------------------------------
    [StringLength(300)]
    private string? _refSourceName;
    public string? RefSourceName
    {
        get => _refSourceName;
        set
        {
            SetProperty(ref _refSourceName, value);
            ValidateRefSourceName();
        }
    }

    private string? _refSourceNameError;
    [NotMapped]
    public string? RefSourceNameError
    {
        get => _refSourceNameError;
        set => SetProperty(ref _refSourceNameError, value);
    }

    private bool _hasRefSourceNameError;
    [NotMapped]
    public bool HasRefSourceNameError
    {
        get => _hasRefSourceNameError;
        set
        {
            if (_hasRefSourceNameError != value)
            {
                _hasRefSourceNameError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // SourceYear
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _sourceYear;
    public string? SourceYear
    {
        get => _sourceYear;
        set
        {
            if (SetProperty(ref _sourceYear, value))
                ValidateSourceYear();
        }
    }

    private string? _sourceYearError;
    [NotMapped]
    public string? SourceYearError
    {
        get => _sourceYearError;
        set => SetProperty(ref _sourceYearError, value);
    }

    private bool _hasSourceYearError;
    [NotMapped]
    public bool HasSourceYearError
    {
        get => _hasSourceYearError;
        set
        {
            if (_hasSourceYearError != value)
            {
                _hasSourceYearError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // Author
    // ---------------------------------------------------------
    [StringLength(400)]
    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
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
    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set => SetProperty(ref _valid, value);
    }

    // ---------------------------------------------------------
    // Audit
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
    public int GetEntityId() => RefSourceId;
    public string GetDisplayName() => RefSourceName ?? $"Source {RefSourceId}";

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
            case nameof(RefSourceName):
                RefSourceNameError = errorMessage;
                HasRefSourceNameError = hasErrors;
                break;

            case nameof(SourceYear):
                SourceYearError = errorMessage;
                HasSourceYearError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }


    private void ValidateRefSourceName()
    {
        ClearErrors(nameof(RefSourceName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefSourceName))
            AddError(nameof(RefSourceName), loc.GetLocalizedString("Error_SourceName_Empty"));

        if (RefSourceName?.Length > 300)
            AddError(nameof(RefSourceName), loc.GetLocalizedString("Error_SourceName_TooLong"));

        UpdateErrorProperty(nameof(RefSourceName));
    }

    private void ValidateSourceYear()
    {
        ClearErrors(nameof(SourceYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(SourceYear))
            goto Notify;

        if (SourceYear!.Length != 4)
        {
            AddError(nameof(SourceYear), loc.GetLocalizedString("Error_Year_Length"));
            goto Notify;
        }

        if (!int.TryParse(SourceYear, out var year))
        {
            AddError(nameof(SourceYear), loc.GetLocalizedString("Error_Year_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(SourceYear), loc.GetLocalizedString("Error_Year_Range"));

Notify:
        UpdateErrorProperty(nameof(SourceYear));
    }

    public bool IsValid()
    {
        ValidateRefSourceName();
        ValidateSourceYear();

        return !HasRefSourceNameError && !HasSourceYearError;
    }
}


//public class Tbl90RefSource : ObservableObject, ICopyable<Tbl90RefSource>
//{
//    [Key]
//    public int RefSourceId { get; set; }

//    private int _countId;
//    public int CountId
//    {
//        get => _countId;
//        set => SetProperty(ref _countId, value);
//    }

//    private bool? _valid;
//    public bool? Valid
//    {
//        get => _valid;
//        set => SetProperty(ref _valid, value);
//    }

//    [StringLength(4)]
//    private string? _validYear;
//    public string? ValidYear
//    {
//        get => _validYear;
//        set => SetProperty(ref _validYear, value);
//    }

//    [StringLength(300)]
//    private string? _refSourceName;
//    public string? RefSourceName
//    {
//        get => _refSourceName;
//        set => SetProperty(ref _refSourceName, value);
//    }

//    [StringLength(4)]
//    private string? _sourceYear;
//    public string? SourceYear
//    {
//        get => _sourceYear;
//        set => SetProperty(ref _sourceYear, value);
//    }

//    public string FullName => $"{RefSourceName} {SourceYear}";
//    public string RefSourceFullName => $"{RefSourceName} {SourceYear}";

//    [StringLength(400)]
//    private string? _author;
//    public string? Author
//    {
//        get => _author;
//        set => SetProperty(ref _author, value);
//    }

//    private string? _notes;
//    public string? Notes
//    {
//        get => _notes;
//        set => SetProperty(ref _notes, value);
//    }

//    [StringLength(100)]
//    private string? _info;
//    public string? Info
//    {
//        get => _info;
//        set => SetProperty(ref _info, value);
//    }

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

//    private string? _memo;
//    public string? Memo
//    {
//        get => _memo;
//        set => SetProperty(ref _memo, value);
//    }

//    // ---------------------------------------------------------
//    // Clone + CopyFrom (wie bei Tbl90RefExpert)
//    // ---------------------------------------------------------

//    public Tbl90RefSource Clone()
//    {
//        return new Tbl90RefSource
//        {
//            RefSourceId = RefSourceId,
//            CountId = CountId,
//            Valid = Valid,
//            ValidYear = ValidYear,
//            RefSourceName = RefSourceName,
//            SourceYear = SourceYear,
//            Author = Author,
//            Notes = Notes,
//            Info = Info,
//            Writer = Writer,
//            WriterDate = WriterDate,
//            Updater = Updater,
//            UpdaterDate = UpdaterDate,
//            Memo = Memo
//        };
//    }

//    public void CopyFrom(Tbl90RefSource other)
//    {
//        CountId = other.CountId;
//        Valid = other.Valid;
//        ValidYear = other.ValidYear;
//        RefSourceName = other.RefSourceName;
//        SourceYear = other.SourceYear;
//        Author = other.Author;
//        Notes = other.Notes;
//        Info = other.Info;
//        Writer = other.Writer;
//        WriterDate = other.WriterDate;
//        Updater = other.Updater;
//        UpdaterDate = other.UpdaterDate;
//        Memo = other.Memo;
//    }
//}
