using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl90RefAuthor;

public class RefAuthorEditModel : BaseEditModel
{
    // ---------------------------------------------------------
    // 0) RefAuthorName
    // ---------------------------------------------------------
    private string? _refAuthorName;
    public string? RefAuthorName
    {
        get => _refAuthorName;
        set
        {
            _refAuthorName = value;
            OnPropertyChanged(nameof(RefAuthorName));
            ValidateRefAuthorName();
        }
    }

    // ---------------------------------------------------------
    // 1) ValidYear, Valid, ISBN
    // ---------------------------------------------------------
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

    private string? _isbn;
    public string? ISBN
    {
        get => _isbn;
        set
        {
            _isbn = value;
            OnPropertyChanged(nameof(ISBN));
            ValidateISBN();
        }
    }

    // ---------------------------------------------------------
    // 2) ArticelTitle
    // ---------------------------------------------------------
    private string? _articelTitle;
    public string? ArticelTitle
    {
        get => _articelTitle;
        set
        {
            _articelTitle = value;
            OnPropertyChanged(nameof(ArticelTitle));
        }
    }

    // ---------------------------------------------------------
    // 3) BookName + Page1
    // ---------------------------------------------------------
    private string? _bookName;
    public string? BookName
    {
        get => _bookName;
        set
        {
            _bookName = value;
            OnPropertyChanged(nameof(BookName));
        }
    }

    private string? _page;
    public string? Page
    {
        get => _page;
        set
        {
            _page = value;
            OnPropertyChanged(nameof(Page));
        }
    }
    private string? _page1;
    public string? Page1
    {
        get => _page1;
        set
        {
            _page1 = value;
            OnPropertyChanged(nameof(Page1));
        }
    }

    // ---------------------------------------------------------
    // 4) Publisher
    // ---------------------------------------------------------
    private string? _publisher;
    public string? Publisher
    {
        get => _publisher;
        set
        {
            _publisher = value;
            OnPropertyChanged(nameof(Publisher));
        }
    }

    // ---------------------------------------------------------
    // 5) PublicationPlace + PublicationYear
    // ---------------------------------------------------------
    private string? _publicationPlace;
    public string? PublicationPlace
    {
        get => _publicationPlace;
        set
        {
            _publicationPlace = value;
            OnPropertyChanged(nameof(PublicationPlace));
        }
    }

    private string? _publicationYear;
    public string? PublicationYear
    {
        get => _publicationYear;
        set
        {
            _publicationYear = value;
            OnPropertyChanged(nameof(PublicationYear));
            ValidatePublicationYear();
        }
    }

    // ---------------------------------------------------------
    // 6) Info
    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    // 7) Notes
    // ---------------------------------------------------------
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

    // ---------------------------------------------------------
    // 8) Memo
    // ---------------------------------------------------------
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
    // VALIDIERUNG
    // ---------------------------------------------------------

    // --- RefAuthorName ---
    public bool HasRefAuthorNameError => HasPropertyErrors(nameof(RefAuthorName));
    public string? RefAuthorNameError =>
        _errors.ContainsKey(nameof(RefAuthorName)) ? _errors[nameof(RefAuthorName)].FirstOrDefault() : null;

    private void ValidateRefAuthorName()
    {
        ClearErrors(nameof(RefAuthorName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(RefAuthorName))
            AddError(nameof(RefAuthorName), loc.GetLocalizedString("Error_RefAuthorName_Empty"));

        if (RefAuthorName?.Length < 3)
            AddError(nameof(RefAuthorName), loc.GetLocalizedString("Error_RefAuthorName_TooShort"));

        OnPropertyChanged(nameof(RefAuthorNameError));
        OnPropertyChanged(nameof(HasRefAuthorNameError));
    }

    // --- ValidYear ---
    public bool HasValidYearError => HasPropertyErrors(nameof(ValidYear));
    public string? ValidYearError =>
        _errors.ContainsKey(nameof(ValidYear)) ? _errors[nameof(ValidYear)].FirstOrDefault() : null;

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();

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
        OnPropertyChanged(nameof(ValidYearError));
        OnPropertyChanged(nameof(HasValidYearError));
    }

    // --- ISBN ---
    public bool HasISBNError => HasPropertyErrors(nameof(ISBN));
    public string? ISBNError =>
        _errors.ContainsKey(nameof(ISBN)) ? _errors[nameof(ISBN)].FirstOrDefault() : null;

    private void ValidateISBN()
    {
        ClearErrors(nameof(ISBN));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ISBN))
            goto Notify;

        if (ISBN!.Length < 5)
            AddError(nameof(ISBN), loc.GetLocalizedString("Error_ISBN_TooShort"));

        Notify:
        OnPropertyChanged(nameof(ISBNError));
        OnPropertyChanged(nameof(HasISBNError));
    }

    // --- PublicationYear ---
    public bool HasPublicationYearError => HasPropertyErrors(nameof(PublicationYear));
    public string? PublicationYearError =>
        _errors.ContainsKey(nameof(PublicationYear)) ? _errors[nameof(PublicationYear)].FirstOrDefault() : null;

    private void ValidatePublicationYear()
    {
        ClearErrors(nameof(PublicationYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(PublicationYear))
            goto Notify;

        if (PublicationYear!.Length != 4)
        {
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_PublicationYear_Length"));
            goto Notify;
        }

        if (!int.TryParse(PublicationYear, out var year))
        {
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_PublicationYear_NotNumber"));
            goto Notify;
        }

        if (year < 1500 || year > 2100)
            AddError(nameof(PublicationYear), loc.GetLocalizedString("Error_PublicationYear_Range"));

        Notify:
        OnPropertyChanged(nameof(PublicationYearError));
        OnPropertyChanged(nameof(HasPublicationYearError));
    }
}
