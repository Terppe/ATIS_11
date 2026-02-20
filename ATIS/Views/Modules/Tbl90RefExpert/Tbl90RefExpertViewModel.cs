using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Modules.Tbl90RefExpert;

public class Tbl90RefExpertViewModel
    : Base1DataGrid1DetailViewModel<Tbl90RefExpert, RefExpertEditModel>
{
    private readonly IDataService _data;
    private readonly IUserService _user;

    public Tbl90RefExpertViewModel(IDataService data, IUserService user)
    {
        _data = data;
        _user = user;
    }

    protected override async Task<IEnumerable<Tbl90RefExpert>> LoadItemsAsync()
        => await _data.GetAllRefExpertAsync();

    protected override Tbl90RefExpert CreateNewDomainModel()
        => new Tbl90RefExpert();

    protected override Tbl90RefExpert CloneDomain(Tbl90RefExpert source)
        => source.Clone();

    protected override RefExpertEditModel ConvertDomainToEdit(Tbl90RefExpert d)
        => new RefExpertEditModel
        {
            RefExpertName = d.RefExpertName,
            ValidYear = d.ValidYear,
            Valid = d.Valid,
            Notes = d.Notes,
            Info = d.Info,
            Memo = d.Memo,
            CountId = d.CountId
        };

    protected override void ConvertEditToDomain(RefExpertEditModel e, Tbl90RefExpert d)
    {
        d.RefExpertName = e.RefExpertName;
        d.ValidYear = e.ValidYear;
        d.Valid = e.Valid;
        d.Notes = e.Notes;
        d.Info = e.Info;
        d.Memo = e.Memo;
        d.CountId = e.CountId;
    }

    protected override async Task<Tbl90RefExpert> SaveDomainAsync(Tbl90RefExpert domain)
        => await _data.UpsertRefExpertAsync(domain);

    protected override async Task DeleteDomainAsync(Tbl90RefExpert domain)
        => await _data.DeleteRefExpertAsync(domain.RefExpertId);

    protected override int GetId(Tbl90RefExpert model)
        => model.RefExpertId;

    protected override void OnBeforeSave(Tbl90RefExpert domain, bool isNew)
    {
        if (isNew)
        {
            domain.Writer = _user.CurrentUser;
            domain.WriterDate = DateTime.Now;
            domain.CountId = RandomHelper.Randomnumber();
        }

        domain.Updater = _user.CurrentUser;
        domain.UpdaterDate = DateTime.Now;
    }
}

