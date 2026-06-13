using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl75Species;

public class Tbl75SpeciesListViewModel : BaseChildListViewModel<Models.Tbl75Species>
{
    private readonly IDataService _data;

    public Tbl75SpeciesListViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IUserService user,
        IDialogService dialog)
        : base(
            new DataService(factory, user),
            dialog,
            parentId =>
            {
                var data = new DataService(factory, user);
                return data.GetSpeciesForGenusAsync(parentId);
            })
    {
        _data = new DataService(factory, user);

    }

    public Models.Tbl75Species? SelectedSpecies
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

//public class Tbl75SpeciesListViewModel : BaseChildListViewModel<Models.Tbl75Species>
//{
//    private static IDataService CreateDataService(IUserService user)
//        => new DataService(new AtisDbContext(), user);

//    public Tbl75SpeciesListViewModel(IUserService user, IDialogService dialog)
//        : base(CreateDataService(user), dialog, parentId => CreateDataService(user).GetSpeciesForGenusAsync(parentId))
//    {
//    }

//    public Models.Tbl75Species? SelectedSpecies
//    {
//        get => SelectedItem;
//        set => SelectedItem = value;
//    }
//}
