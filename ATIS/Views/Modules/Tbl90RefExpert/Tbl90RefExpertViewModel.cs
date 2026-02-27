using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Views.Modules.Tbl90RefExpert;

public class Tbl90RefExpertViewModel
    : Base1List1DetailViewModel<Tbl90RefExpert, RefExpertEditModel>
{
    private readonly IDataService _data;
    private readonly IUserService _user;

    public Tbl90RefExpertViewModel(IDataService data, IUserService user, IDialogService dialogService)
        : base(dialogService)
    {
        _data = data;
        _user = user;
    }
    protected override async Task<IEnumerable<Tbl90RefExpert>> SearchItemsAsync(string text)
    {
        IsLoading = true;
        try
        {
            return await _data.SearchRefExpertAsync(text);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected override async Task<IEnumerable<Tbl90RefExpert>> LoadItemsAsync()
        => await _data.GetAllRefExpertAsync();

    protected override Tbl90RefExpert CreateNewDomainModel()
        => new Tbl90RefExpert();

    protected override Tbl90RefExpert CloneDomain(Tbl90RefExpert source)
        => source.Clone();

    //protected override Task<bool> ExistsAsync(RefExpertEditModel edit)
    //{
    //    return _data.RefExpertExistsAsync(edit.RefExpertName);
    //}
    protected override Task<bool> ExistsAsync(RefExpertEditModel edit)
    {
        var domain = new Tbl90RefExpert
        {
            RefExpertName = edit.RefExpertName,
            Valid = edit.Valid,
            ValidYear = edit.ValidYear,
            Info = edit.Info,
            Notes = edit.Notes,
            Memo = edit.Memo
        };

        return _data.RefExpertExistsAsync(domain);
    }

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

