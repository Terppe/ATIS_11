using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ATIS.Models;

public class TblCountry
{
    // ------------------ Primary Key ------------------
    // Beim Erstellen eines neuen Datensatzes darf CountryId null sein.
    // EF setzt den Wert beim Speichern automatisch.
    [Key]
    public int CountryId { get; set; }

    // ------------------ Required Name ------------------
    // Name ist Pflichtfeld → non-nullable
    // Default = "" verhindert SqlNullValueException
    [Required(ErrorMessageResourceName = "RequiredNameName", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    [StringLength(100, ErrorMessageResourceName = "StringLength_100", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string Name { get; set; } = string.Empty;

    // ------------------ Optional Regex ------------------
    // Regex ist optional → nullable
    // Default = "" verhindert NullReference-Probleme im UI
    [StringLength(250, ErrorMessageResourceName = "StringLength_250", ErrorMessageResourceType = typeof(Strings.StringsRes))]
    public string? Regex { get; set; } = string.Empty;

    // ------------------ Coordinates ------------------
    // Diese Werte sind immer gesetzt → non-nullable
    public int? ZoomLevel { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }


    // ------------------ Clone ------------------
    public TblCountry Clone()
    {
        return new TblCountry
        {
            CountryId = CountryId,
            Name = Name,          
            Regex = Regex,
            Latitude = Latitude,
            Longitude = Longitude,
            ZoomLevel = ZoomLevel
        };
    }

    // ------------------ CopyFrom ------------------
    public void CopyFrom(TblCountry other)
    {
        Debug.WriteLine($"[COPYFROM] {Name} → {other.Name}");
        CountryId = other.CountryId;
        Name = other.Name;
        Regex = other.Regex;
        ZoomLevel = other.ZoomLevel;
        Latitude = other.Latitude;
        Longitude = other.Longitude;
    }
    // ------------------ Alternate RowColor in DataGrid ------------------
    [NotMapped]
    public bool IsAlternate { get; set; }


}
