
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class Tbl90RefExpert : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int RefExpertId { get; set; }

    // ---------------------------------------------------------
    // RefExpertName
    // ---------------------------------------------------------
    [StringLength(300)]
    private string? _refExpertName;
    public string? RefExpertName
    {
        get => _refExpertName;
        set
        {
            SetProperty(ref _refExpertName, value);
            ValidateRefExpertName();
        }
    }

    private string? _refExpertNameError;
    [NotMapped]
    public string? RefExpertNameError
    {
        get => _refExpertNameError;
        set => SetProperty(ref _refExpertNameError, value);
    }

    private bool _hasRefExpertNameError;
    [NotMapped]
    public bool HasRefExpertNameError
    {
        get => _hasRefExpertNameError;
        set
        {
            if (_hasRefExpertNameError != value)
            {
                _hasRefExpertNameError = value;
                OnPropertyChanged();
            }
        }
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
                ValidateValidYear();
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
        set
        {
            if (_hasValidYearError != value)
            {
                _hasValidYearError = value;
                OnPropertyChanged();
            }
        }
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
    public int GetEntityId() => RefExpertId;
    public string GetDisplayName() => RefExpertName ?? $"Expert {RefExpertId}";

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
            case nameof(RefExpertName):
                RefExpertNameError = errorMessage;
                HasRefExpertNameError = hasErrors;
                break;

            case nameof(ValidYear):
                ValidYearError = errorMessage;
                HasValidYearError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    private void ValidateRefExpertName()
    {
        ClearErrors(nameof(RefExpertName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefExpertName))
            AddError(nameof(RefExpertName), loc.GetLocalizedString("Error_ExpertName_Empty"));

        if (RefExpertName?.Length > 300)
            AddError(nameof(RefExpertName), loc.GetLocalizedString("Error_ExpertName_TooLong"));

        UpdateErrorProperty(nameof(RefExpertName));
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ValidYear))
            goto Notify;

        if (ValidYear!.Length != 4)
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_Year_Length"));
            goto Notify;
        }

        if (!int.TryParse(ValidYear, out var year))
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_Year_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_Year_Range"));

Notify:
        UpdateErrorProperty(nameof(ValidYear));
    }

    public bool IsValid()
    {
        ValidateRefExpertName();
        ValidateValidYear();

        return !HasRefExpertNameError && !HasValidYearError;
    }
}



//public class Tbl90RefExpert : ObservableObject, ICopyable<Tbl90RefExpert>
//{
//    [Key]
//    public int RefExpertId { get; set; }

//    [StringLength(300)]
//    private string? _refExpertName;
//    public string? RefExpertName
//    {
//        get => _refExpertName;
//        set => SetProperty(ref _refExpertName, value);
//    }

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


//    [StringLength(1000)]
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

//    public Tbl90RefExpert Clone()
//    {
//        return new Tbl90RefExpert
//        {
//            RefExpertId = RefExpertId,
//            CountId = CountId,
//            Valid = Valid,
//            ValidYear = ValidYear,
//            RefExpertName = RefExpertName,
//            Notes = Notes,
//            Info = Info,
//            Writer = Writer,
//            WriterDate = WriterDate,
//            Updater = Updater,
//            UpdaterDate = UpdaterDate,
//            Memo = Memo
//        };
//    }

//    public void CopyFrom(Tbl90RefExpert other)
//    {
//        Debug.WriteLine($"[COPYFROM] {RefExpertName} → {other.RefExpertName}");

//        CountId = other.CountId;
//        Valid = other.Valid;
//        ValidYear = other.ValidYear;
//        RefExpertName = other.RefExpertName;
//        Notes = other.Notes;
//        Info = other.Info;
//        Writer = other.Writer;
//        WriterDate = other.WriterDate;
//        Updater = other.Updater;
//        UpdaterDate = other.UpdaterDate;
//        Memo = other.Memo;
//    }
//}
