using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATIS.Interfaces;

namespace ATIS.Models;

public class Tbl93Comment : ObservableObject, ICopyable<Tbl93Comment>, IDataService.IAuditable
{
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
    // ValidYear (optional)
    // ---------------------------------------------------------
    [StringLength(4)]
    private string? _validYear;
    public string? ValidYear
    {
        get => _validYear;
        set => SetProperty(ref _validYear, value);
    }

    // ---------------------------------------------------------
    // Info (optional)
    // ---------------------------------------------------------
    [StringLength(100)]
    private string? _info;
    public string? Info
    {
        get => _info;
        set => SetProperty(ref _info, value);
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
    // Navigation (optional)
    // ---------------------------------------------------------
    [ForeignKey("SpeciesId")]
    public virtual Tbl75Species? Tbl75Speciesses { get; set; }

    // ---------------------------------------------------------
    // CLONE
    // ---------------------------------------------------------
    public Tbl93Comment Clone()
    {
        return new Tbl93Comment
        {
            CommentId = CommentId,
            SpeciesId = SpeciesId,
            GenusId = GenusId,
            InfratribusId = InfratribusId,
            SubtribusId = SubtribusId,
            TribusId = TribusId,
            SupertribusId = SupertribusId,
            InfrafamilyId = InfrafamilyId,
            SubfamilyId = SubfamilyId,
            FamilyId = FamilyId,
            SuperfamilyId = SuperfamilyId,
            InfraordoId = InfraordoId,
            SubordoId = SubordoId,
            OrdoId = OrdoId,
            LegioId = LegioId,
            InfraclassId = InfraclassId,
            SubclassId = SubclassId,
            ClassId = ClassId,
            SuperclassId = SuperclassId,
            SubdivisionId = SubdivisionId,
            SubphylumId = SubphylumId,
            DivisionId = DivisionId,
            PhylumId = PhylumId,
            RegnumId = RegnumId,
            CountId = CountId,
            Valid = Valid,
            ValidYear = ValidYear,
            Info = Info,
            Writer = Writer,
            WriterDate = WriterDate,
            Updater = Updater,
            UpdaterDate = UpdaterDate,
            Memo = Memo
        };
    }

    // ---------------------------------------------------------
    // COPYFROM
    // ---------------------------------------------------------
    public void CopyFrom(Tbl93Comment other)
    {
        SpeciesId = other.SpeciesId;
        GenusId = other.GenusId;
        InfratribusId = other.InfratribusId;
        SubtribusId = other.SubtribusId;
        TribusId = other.TribusId;
        SupertribusId = other.SupertribusId;
        InfrafamilyId = other.InfrafamilyId;
        SubfamilyId = other.SubfamilyId;
        FamilyId = other.FamilyId;
        SuperfamilyId = other.SuperfamilyId;
        InfraordoId = other.InfraordoId;
        SubordoId = other.SubordoId;
        OrdoId = other.OrdoId;
        LegioId = other.LegioId;
        InfraclassId = other.InfraclassId;
        SubclassId = other.SubclassId;
        ClassId = other.ClassId;
        SuperclassId = other.SuperclassId;
        SubdivisionId = other.SubdivisionId;
        SubphylumId = other.SubphylumId;
        DivisionId = other.DivisionId;
        PhylumId = other.PhylumId;
        RegnumId = other.RegnumId;
        CountId = other.CountId;
        Valid = other.Valid;
        ValidYear = other.ValidYear;
        Info = other.Info;
        Writer = other.Writer;
        WriterDate = other.WriterDate;
        Updater = other.Updater;
        UpdaterDate = other.UpdaterDate;
        Memo = other.Memo;
    }
}
