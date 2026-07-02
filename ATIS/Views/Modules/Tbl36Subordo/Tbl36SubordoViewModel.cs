using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl36Subordo;

public class Tbl36SubordoViewModel : BaseBossListViewModel<Models.Tbl36Subordo>, ITaxonViewModel
{
    public Tbl36SubordoViewModel(
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

    protected override Models.Tbl36Subordo CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl36Subordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl36Subordo?> SaveItemAsync(Models.Tbl36Subordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl36Subordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl36Subordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl36Subordo item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl36Subordo model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl36Subordo item, string column)
    {
        return column switch
        {
            "Id" => item.SubordoId,
            "Info" => item.SubordoName ?? "",
            _ => ""
        };
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForOrdoAsync(int id) => Task.CompletedTask;
}
