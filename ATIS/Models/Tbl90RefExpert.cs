using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl90RefExpert : ICopyable<Tbl90RefExpert>
{
    // ------------------ Primary Key ------------------
    [Key]
    public int RefExpertId { get; set; }

    // ------------------ Expert Name ------------------
    [StringLength(300)]
    public string? RefExpertName { get; set; }

    // ------------------ Counter ------------------
    public int CountId { get; set; }

    // ------------------ Valid Flag ------------------
    public bool? Valid { get; set; }

    // ------------------ Valid Year ------------------
    [StringLength(4)]
    public string? ValidYear { get; set; }

    // ------------------ Notes ------------------
    [StringLength(1000)]
    public string? Notes { get; set; }

    // ------------------ Info ------------------
    [StringLength(100)]
    public string? Info { get; set; }

    // ------------------ Writer ------------------
    [StringLength(50)]
    public string? Writer { get; set; }

    public DateTime? WriterDate { get; set; }

    // ------------------ Updater ------------------
    [StringLength(50)]
    public string? Updater { get; set; }

    public DateTime? UpdaterDate { get; set; }

    // ------------------ Memo ------------------
    public string? Memo { get; set; }

    // ------------------ Clone ------------------
    public Tbl90RefExpert Clone()
    {
        return new Tbl90RefExpert
        {
            RefExpertId = RefExpertId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            RefExpertName = RefExpertName,
            Notes = Notes,
            Info = Info,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    // ------------------ CopyFrom ------------------
    public void CopyFrom(Tbl90RefExpert other)
    {
        Debug.WriteLine($"[COPYFROM] {RefExpertName} → {other.RefExpertName}");

        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        RefExpertName = other.RefExpertName;
        Notes = other.Notes;
        Info = other.Info;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }

    // ------------------ Alternate RowColor in DataGrid ------------------
    [NotMapped]
    public bool IsAlternate { get; set; }
}
