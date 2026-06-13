using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl03Regnum;

public class Tbl03RegnumViewModel : BaseBossListViewModel<Models.Tbl03Regnum>, ITaxonViewModel
{
    public Tbl03RegnumViewModel(
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

    protected override Models.Tbl03Regnum CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl03Regnum>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl03Regnum?> SaveItemAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl03Regnum model)
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForRegnumAsync(int id) => Task.CompletedTask;
}
