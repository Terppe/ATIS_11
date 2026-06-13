using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl48Subfamily;

public class Tbl48SubfamilyViewModel : BaseBossListViewModel<Models.Tbl48Subfamily>, ITaxonViewModel
{
    public Tbl48SubfamilyViewModel(
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

    protected override Models.Tbl48Subfamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl48Subfamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl48Subfamily?> SaveItemAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl48Subfamily model)
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForFamilyAsync(int id) => Task.CompletedTask;
}
