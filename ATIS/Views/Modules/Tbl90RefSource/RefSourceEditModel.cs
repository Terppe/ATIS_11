using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl90RefSource;

public class RefSourceEditModel : BaseEditModel
{
    // ---------------------------------------------------------
    //  Properties
    // ---------------------------------------------------------

    private string? _refSourceName;
    public string? RefSourceName
    {
        get => _refSourceName;
        set
        {
            _refSourceName = value;
            OnPropertyChanged(nameof(RefSourceName));
            ValidateRefSourceName();
        }
    }

    private string? _sourceYear;
    public string? SourceYear
    {
        get => _sourceYear;
        set
        {
            _sourceYear = value;
            OnPropertyChanged(nameof(SourceYear));
            ValidateSourceYear();
        }
    }

    private string? _author;
    public string? Author
    {
        get => _author;
        set
        {
            _author = value;
            OnPropertyChanged(nameof(Author));
            ValidateAuthor();
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

    public int CountId { get; set; }

    // ---------------------------------------------------------
    //  Error Properties
    // ---------------------------------------------------------

    public bool HasRefSourceNameError => HasPropertyErrors(nameof(RefSourceName));
    public string? RefSourceNameError =>
        _errors.ContainsKey(nameof(RefSourceName)) ? _errors[nameof(RefSourceName)].FirstOrDefault() : null;

    public bool HasSourceYearError => HasPropertyErrors(nameof(SourceYear));
    public string? SourceYearError =>
        _errors.ContainsKey(nameof(SourceYear)) ? _errors[nameof(SourceYear)].FirstOrDefault() : null;

    public bool HasAuthorError => HasPropertyErrors(nameof(Author));
    public string? AuthorError =>
        _errors.ContainsKey(nameof(Author)) ? _errors[nameof(Author)].FirstOrDefault() : null;

    public bool HasValidYearError => HasPropertyErrors(nameof(ValidYear));
    public string? ValidYearError =>
        _errors.ContainsKey(nameof(ValidYear)) ? _errors[nameof(ValidYear)].FirstOrDefault() : null;

    // ---------------------------------------------------------
    //  Validation
    // ---------------------------------------------------------

    private void ValidateRefSourceName()
    {
        ClearErrors(nameof(RefSourceName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefSourceName))
            AddError(nameof(RefSourceName), loc.GetLocalizedString("Error_RefSourceName_Empty"));

        if (RefSourceName?.Length < 3)
            AddError(nameof(RefSourceName), loc.GetLocalizedString("Error_RefSourceName_TooShort"));

        OnPropertyChanged(nameof(RefSourceNameError));
        OnPropertyChanged(nameof(HasRefSourceNameError));
    }

    private void ValidateSourceYear()
    {
        ClearErrors(nameof(SourceYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(SourceYear))
            goto Notify;

        if (SourceYear!.Length != 4)
            AddError(nameof(SourceYear), loc.GetLocalizedString("Error_SourceYear_Length"));

        Notify:
        OnPropertyChanged(nameof(SourceYearError));
        OnPropertyChanged(nameof(HasSourceYearError));
    }

    private void ValidateAuthor()
    {
        ClearErrors(nameof(Author));
        var loc = Localizer.Get();

        if (!string.IsNullOrWhiteSpace(Author) && Author!.Length < 3)
            AddError(nameof(Author), loc.GetLocalizedString("Error_Author_TooShort"));

        OnPropertyChanged(nameof(AuthorError));
        OnPropertyChanged(nameof(HasAuthorError));
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ValidYear))
            goto Notify;

        if (ValidYear!.Length != 4)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Length"));
        else if (!int.TryParse(ValidYear, out var year))
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_NotNumber"));
        else if (year < 1500 || year > 2100)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Range"));

        Notify:
        OnPropertyChanged(nameof(ValidYearError));
        OnPropertyChanged(nameof(HasValidYearError));
    }
}
