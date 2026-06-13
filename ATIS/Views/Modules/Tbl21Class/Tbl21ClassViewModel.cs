using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl21Class;

public class Tbl21ClassViewModel : BaseBossListViewModel<Models.Tbl21Class>, ITaxonViewModel
{
    public Tbl21ClassViewModel(
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

    protected override Models.Tbl21Class CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl21Class>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl21Class?> SaveItemAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl21Class model)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForSuperclassAsync(int id) => Task.CompletedTask;

    public override async Task LoadParentAsync(int parentId)
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        throw new NotImplementedException();
    }

}
