using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using ATIS.Interfaces;
using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Models;

public class Tbl93Comment : ObservableObject,  IDataService.IAuditable, IHierarchicalEntity
{
    private readonly Dictionary<string, List<string>> _errors = new();

    // ---------------------------------------------------------
    // PRIMARY KEY
    // ---------------------------------------------------------
    [Key]
    public int CommentId { get; set; }

    // ---------------------------------------------------------
    // FK-IDs (alle optional)
    // ---------------------------------------------------------
    public int? SpeciesId { get; set; }
    public int? GenusId { get; set; }
    public int? InfratribusId { get; set; }
    public int? SubtribusId { get; set; }
    public int? TribusId { get; set; }
    public int? SupertribusId { get; set; }
    public int? InfrafamilyId { get; set; }
    public int? SubfamilyId { get; set; }
    public int? FamilyId { get; set; }
    public int? SuperfamilyId { get; set; }
    public int? InfraordoId { get; set; }
    public int? SubordoId { get; set; }
    public int? OrdoId { get; set; }
    public int? LegioId { get; set; }
    public int? InfraclassId { get; set; }
    public int? SubclassId { get; set; }
    public int? ClassId { get; set; }
    public int? SuperclassId { get; set; }
    public int? SubdivisionId { get; set; }
    public int? SubphylumId { get; set; }
    public int? DivisionId { get; set; }
    public int? PhylumId { get; set; }
    public int? RegnumId { get; set; }

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
    // Valid (optional)
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
    // Info (optional)
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set
        {
            SetProperty(ref _info, value);
            ValidateInfo();
        }
    }
    private string? _infoError;
    [NotMapped]
    public string? InfoError
    {
        get => _infoError;
        set => SetProperty(ref _infoError, value);
    }

    private bool _hasInfoError;
    [NotMapped]
    public bool HasInfoError
    {
        get => _hasInfoError;
        set => SetProperty(ref _hasInfoError, value);
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
    // IHierarchicalEntity IMPLEMENTATION
    // ---------------------------------------------------------
    public int GetEntityId() => CommentId;
    public string GetDisplayName() => Info ?? $"Comment {CommentId}";

    // ---------------------------------------------------------
    // VALIDIERUNGSMETHODEN MIT LOCALIZATION
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
            case nameof(ValidYear):
                ValidYearError = errorMessage;
                HasValidYearError = hasErrors;
                break;
            case nameof(Info):
                InfoError = errorMessage;
                HasInfoError = hasErrors;
                break;
        }

        OnPropertyChanged($"{propertyName}Error");
        OnPropertyChanged($"Has{propertyName}Error");
    }

    private void ValidateValidYear()
    {
        Debug.WriteLine($"ValidateValidYear() called with '{ValidYear}'");

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
    private void ValidateInfo()
    {
        ClearErrors(nameof(Info));
        var loc = Localizer.Get();  // ← Direkt verwenden wie in deinem Code

        if (_info?.Length > 100)
            AddError(nameof(Info), loc.GetLocalizedString("Error_Info_TooLong"));

        UpdateErrorProperty(nameof(Info));
    }

    public bool IsValid()
    {
        ValidateValidYear();
        ValidateInfo();

        return !HasValidYearError && !HasInfoError;
    }
}
