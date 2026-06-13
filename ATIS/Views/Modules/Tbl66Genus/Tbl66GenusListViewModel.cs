using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl66Genus;

public class Tbl66GenusListViewModel : BaseChildListViewModel<Models.Tbl66Genus>
{
    private readonly IDataService _data;

    public Tbl66GenusListViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IUserService user,
        IDialogService dialog)
        : base(
            new DataService(factory, user),
            dialog,
            parentId =>
            {
                var data = new DataService(factory, user);
                return data.GetGenusForInfratribusAsync(parentId);
            })
    {
        _data = new DataService(factory, user);
    }
    public Models.Tbl66Genus? SelectedGenus
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    protected override Task OnSelectedItemChangedAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }
}

//public class Tbl66GenusListViewModel : BaseChildListViewModel<Models.Tbl66Genus>
//{
//    private static IDataService CreateDataService(IUserService user)
//        => new DataService(new AtisDbContext(), user);

//    public Tbl66GenusListViewModel(IUserService user, IDialogService dialog)
//        : base(CreateDataService(user), dialog, parentId => CreateDataService(user).GetGenusForInfratribusAsync(parentId))
//    {
//    }

//    public Models.Tbl66Genus? SelectedGenus
//    {
//        get => SelectedItem;
//        set => SelectedItem = value;
//    }
//}

