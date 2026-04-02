using ATIS.Views.Base;
using WinUI3Localizer;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusEdit : BaseEditModel
{
    public Tbl63InfratribusEdit(Models.Tbl63Infratribus domain)
    {
        InfratribusName = domain.InfratribusName;
        SubtribusId = (int)domain.SubtribusId;
        Valid = (bool)domain.Valid;
        ValidYear = domain.ValidYear;
        Synonym = domain.Synonym;
        Author = domain.Author;
        AuthorYear = domain.AuthorYear;
        Info = domain.Info;
        EngName = domain.EngName;
        GerName = domain.GerName;
        FraName = domain.FraName;
        PorName = domain.PorName;
        Memo = domain.Memo;
        CountId = domain.CountId;
    }
    public Tbl63InfratribusEdit()
    {
    }
    public bool HasInfratribusNameError { get; set; }
    public string? InfratribusNameError { get; set; }

    public bool HasValidYearError { get; set; }
    public string? ValidYearError { get; set; }

    public void ApplyTo(Models.Tbl63Infratribus domain)
    {
        domain.InfratribusName = InfratribusName;
        domain.SubtribusId = SubtribusId;
        domain.Valid = Valid;
        domain.ValidYear = ValidYear;
        domain.Synonym = Synonym;
        domain.Author = Author;
        domain.AuthorYear = AuthorYear;
        domain.Info = Info;
        domain.EngName = EngName;
        domain.GerName = GerName;
        domain.FraName = FraName;
        domain.PorName = PorName;
        domain.Memo = Memo;
        domain.CountId = CountId;
    }


    // ---------------------------------------------------------
    // 1) InfratribusName
    // ---------------------------------------------------------
    private string? _infratribusName;
    public string? InfratribusName
    {
        get => _infratribusName;
        set
        {
            _infratribusName = value;
            OnPropertyChanged(nameof(InfratribusName));
            ValidateInfratribusName();
        }
    }

    // ---------------------------------------------------------
    // 2) SubtribusId (FK)
    // ---------------------------------------------------------
    private int _subtribusId;
    public int SubtribusId
    {
        get => _subtribusId;
        set
        {
            _subtribusId = value;
            OnPropertyChanged(nameof(SubtribusId));
        }
    }

    // ---------------------------------------------------------
    // 3) Valid + ValidYear
    // ---------------------------------------------------------
    private bool _valid;
    public bool Valid
    {
        get => _valid;
        set
        {
            _valid = value;
            OnPropertyChanged(nameof(Valid));
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

    // ---------------------------------------------------------
    // 4) Synonym
    // ---------------------------------------------------------
    private string? _synonym;
    public string? Synonym
    {
        get => _synonym;
        set
        {
            _synonym = value;
            OnPropertyChanged(nameof(Synonym));
        }
    }

    // ---------------------------------------------------------
    // 5) Author + AuthorYear
    // ---------------------------------------------------------
    private string? _author;
    public string? Author
    {
        get => _author;
        set
        {
            _author = value;
            OnPropertyChanged(nameof(Author));
        }
    }

    private string? _authorYear;
    public string? AuthorYear
    {
        get => _authorYear;
        set
        {
            _authorYear = value;
            OnPropertyChanged(nameof(AuthorYear));
            ValidateAuthorYear();
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
    // 7) EngName / GerName / FraName / PorName
    // ---------------------------------------------------------
    public string? EngName { get; set; }
    public string? GerName { get; set; }
    public string? FraName { get; set; }
    public string? PorName { get; set; }

    // ---------------------------------------------------------
    // 8) Notes / Memo
    // ---------------------------------------------------------
    public string? Notes { get; set; }
    public string? Memo { get; set; }

    // ---------------------------------------------------------
    // CountId
    // ---------------------------------------------------------
    public int CountId { get; set; }

    // ---------------------------------------------------------
    // VALIDIERUNG
    // ---------------------------------------------------------

    private void ValidateInfratribusName()
    {
        ClearErrors(nameof(InfratribusName));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(InfratribusName))
            AddError(nameof(InfratribusName), loc.GetLocalizedString("Error_InfratribusName_Empty"));

        if (InfratribusName?.Length < 2)
            AddError(nameof(InfratribusName), loc.GetLocalizedString("Error_InfratribusName_TooShort"));

        OnPropertyChanged(nameof(InfratribusName));
    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(ValidYear))
            return;

        if (!int.TryParse(ValidYear, out var year) || year < 1500 || year > 2100)
            AddError(nameof(ValidYear), loc.GetLocalizedString("Error_ValidYear_Range"));

        OnPropertyChanged(nameof(ValidYear));
    }

    private void ValidateAuthorYear()
    {
        ClearErrors(nameof(AuthorYear));
        var loc = Localizer.Get();

        if (string.IsNullOrWhiteSpace(AuthorYear))
            return;

        if (!int.TryParse(AuthorYear, out var year) || year < 1500 || year > 2100)
            AddError(nameof(AuthorYear), loc.GetLocalizedString("Error_AuthorYear_Range"));

        OnPropertyChanged(nameof(AuthorYear));
    }
}
