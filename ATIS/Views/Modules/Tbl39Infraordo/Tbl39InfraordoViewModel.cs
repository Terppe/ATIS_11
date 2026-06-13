using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl39Infraordo;

public class Tbl39InfraordoViewModel : BaseBossListViewModel<Models.Tbl39Infraordo>, ITaxonViewModel
{
    public Tbl39InfraordoViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookup,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        //Child1ViewModel = new TblYYListViewModel(factory, user, dialog);
        //Child2ViewModel = new TblZZListViewModel(factory, user, dialog);

        //IsDetailVisible = false;
        //IsEditVisible = false;
    }

    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl39Infraordo CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl39Infraordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl39Infraordo?> SaveItemAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl39Infraordo model)
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForSubordoAsync(int id) => Task.CompletedTask;
}
