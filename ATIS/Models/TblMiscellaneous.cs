using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class TblMiscellaneous : ObservableObject, IDataService.IAuditable, IHierarchicalEntity
{
    private Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int MiscellaneousId { get; set; }

    // ---------------------------------------------------------
    // MiscellaneousName
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _miscellaneousName;
    public string? MiscellaneousName        
    {
        get => _miscellaneousName;
        set
        {
            SetProperty(ref _miscellaneousName, value);
            ValidateMiscellaneousName();
        }
    }

    private string? _miscellaneousNameError;
    [NotMapped]
    public string? MiscellaneousNameError
    {
        get => _miscellaneousNameError;
        set => SetProperty(ref _miscellaneousNameError, value);
    }

    private bool _hasMiscellaneousNameError;
    [NotMapped]
    public bool HasMiscellaneousNameError
    {
        get => _hasMiscellaneousNameError;
        set
        {
            if (_hasMiscellaneousNameError != value)
            {
                _hasMiscellaneousNameError = value;
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
    // ShortInfo
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _shortInfo;
    public string? ShortInfo
    {
        get => _shortInfo;
        set
        {
            SetProperty(ref _shortInfo, value);
            ValidateShortInfo();
        }
    }

    private string? _shortInfoError;
    [NotMapped]
    public string? ShortInfoError
    {
        get => _shortInfoError;
        set => SetProperty(ref _shortInfoError, value);
    }

    private bool _hasShortInfoError;
    [NotMapped]
    public bool HasShortInfoError
    {
        get => _hasShortInfoError;
        set
        {
            if (_hasShortInfoError != value)
            {
                _hasShortInfoError = value;
                OnPropertyChanged();
            }
        }
    }

    // ---------------------------------------------------------
    // Thema1
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _thema1;
    public string? Thema1       
    {
        get => _thema1;
        set => SetProperty(ref _thema1, value);
    }

    // ---------------------------------------------------------
    // Thema2
    // ---------------------------------------------------------
    [StringLength(1000)]
    private string? _thema2;
    public string? Thema2
    {
        get => _thema2;
        set => SetProperty(ref _thema2, value);
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
    private string _writer = string.Empty;
    public string Writer
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
    private string _updater = string.Empty;
    public string Updater
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
    public int GetEntityId() => MiscellaneousId;
    public string GetDisplayName() => MiscellaneousName ?? $"Miscellaneous {MiscellaneousId}";

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
            case nameof(MiscellaneousName):
                MiscellaneousNameError = errorMessage;
                HasMiscellaneousNameError = hasErrors;
                break;

            case nameof(ValidYear):
                ValidYearError = errorMessage;
                HasValidYearError = hasErrors;
                break;

            case nameof(ShortInfo):
                ShortInfoError = errorMessage;
                HasShortInfoError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    private void ValidateMiscellaneousName()
    {
        ClearErrors(nameof(MiscellaneousName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(MiscellaneousName))
            AddError(nameof(MiscellaneousName), loc.GetLocalizedString("Error_MiscellaneousName_Empty"));

        if (MiscellaneousName?.Length > 100)
            AddError(nameof(MiscellaneousName), loc.GetLocalizedString("Error_MiscellaneousName_TooLong"));

        UpdateErrorProperty(nameof(MiscellaneousName));
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
    private void ValidateShortInfo()
    {
        ClearErrors(nameof(ShortInfo));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ShortInfo))
            AddError(nameof(ShortInfo), loc.GetLocalizedString("Error_ShortInfo_Empty"));

        if (ShortInfo?.Length > 100)
            AddError(nameof(ShortInfo), loc.GetLocalizedString("Error_ShortInfo_TooLong"));

        UpdateErrorProperty(nameof(ShortInfo));
    }

    public bool IsValid()
    {
        ValidateMiscellaneousName();
        ValidateValidYear();
        ValidateShortInfo();

        return !HasMiscellaneousNameError && !HasValidYearError && !HasShortInfoError;
    }
}



//public class TblMiscellaneous
//{
//    [Key]

//    public int MiscellaneousId { get; set; }
//    [Required(ErrorMessageResourceName = "RequiredMiscellaneousName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    //  [MinLength(2, ErrorMessageResourceName = "MinLength_1", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    // [MaxLength(100, ErrorMessageResourceName = "MaxLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string MiscellaneousName { get; set; }
//    [Required]
//    public int CountId { get; set; }
//    public bool? Valid { get; set; }
//    [StringLength(4, ErrorMessageResourceName = "StringLengthYear_4", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ValidYear { get; set; }
//    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string ShortInfo { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Thema1 { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Thema2 { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Info { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Writer { get; set; }
//    public DateTime WriterDate { get; set; }
//    [StringLength(50, ErrorMessageResourceName = "StringLength_50", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Updater { get; set; }
//    public DateTime UpdaterDate { get; set; }
//    [StringLength(1000, ErrorMessageResourceName = "StringLength_Max", ErrorMessageResourceType = typeof(Strings.StringsRes))]
//    public string Memo { get; set; }
//    //     public byte[] RowVersion { get; set; }


//}
