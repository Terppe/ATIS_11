using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl90RefExpert;

public class RefExpertEditModel : BaseEditModel
{
    private string? _refExpertName;
    public string? RefExpertName
    {
        get => _refExpertName;
        set
        {
            _refExpertName = value;
            OnPropertyChanged(nameof(RefExpertName));
            ValidateRefExpertName();
        }
    }

    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set
        {
            _validYear = value;
            OnPropertyChanged(nameof(ValidYear));
            ValidateValidYear();
        }
    }

    private bool? _valid;
    public bool? Valid
    {
        get => _valid;
        set
        {
            _valid = value;
            OnPropertyChanged(nameof(Valid));
        }
    }

    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set
        {
            _notes = value;
            OnPropertyChanged(nameof(Notes));
        }
    }

    private string? _info;
    public string? Info
    {
        get => _info;
        set
        {
            _info = value;
            OnPropertyChanged(nameof(Info));
        }
    }

    private string? _memo;
    public string? Memo
    {
        get => _memo;
        set
        {
            _memo = value;
            OnPropertyChanged(nameof(Memo));
        }
    }
    //-----------------------------------------


    public bool HasRefExpertNameError => HasPropertyErrors(nameof(RefExpertName));

    public string? RefExpertNameError
    {
        get
        {
            if (_errors.ContainsKey(nameof(RefExpertName)))
                return _errors[nameof(RefExpertName)].FirstOrDefault();

            return null;
        }
    }
    public bool HasValidYearError => HasPropertyErrors(nameof(ValidYear));

    public string? ValidYearError
    {
        get
        {
            if (_errors.ContainsKey(nameof(ValidYear)))
                return _errors[nameof(ValidYear)].FirstOrDefault();

            return null;
        }
    }



    public int CountId { get; set; }

    // -------------------------
    // VALIDIERUNG
    // -------------------------

    private void ValidateRefExpertName()
    {
        ClearErrors(nameof(RefExpertName));

        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefExpertName))
            AddError(nameof(RefExpertName), loc.GetLocalizedString("Error_RefExpertName_Empty"));

        if (RefExpertName?.Length < 3)
            AddError(nameof(RefExpertName), loc.GetLocalizedString("Error_RefExpertName_TooShort"));

        OnPropertyChanged(nameof(RefExpertNameError));
        OnPropertyChanged(nameof(HasRefExpertNameError));
    }
    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ValidYear))
        {
            // Kein Fehler, leeres Feld ist erlaubt
            goto Notify;
        }

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
        {
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Range"));
        }

        Notify:
        OnPropertyChanged(nameof(ValidYearError));
        OnPropertyChanged(nameof(HasValidYearError));
    }
}
