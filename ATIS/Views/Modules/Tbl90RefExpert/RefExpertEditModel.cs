using ATIS.Views.Base;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public int CountId { get; set; }

    // -------------------------
    // VALIDIERUNG
    // -------------------------

    private void ValidateRefExpertName()
    {
        ClearErrors(nameof(RefExpertName));

        if (string.IsNullOrWhiteSpace(RefExpertName))
            AddError(nameof(RefExpertName), "Name darf nicht leer sein.");

        if (RefExpertName?.Length < 3)
            AddError(nameof(RefExpertName), "Name muss mindestens 3 Zeichen lang sein.");

    }

    private void ValidateValidYear()
    {
        ClearErrors(nameof(ValidYear));

        if (!string.IsNullOrWhiteSpace(ValidYear) && !int.TryParse(ValidYear, out _))
            AddError(nameof(ValidYear), "Jahr muss eine Zahl sein.");
    }
}
