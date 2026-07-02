using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl33Ordo;

public class Tbl33OrdoViewModel : BaseBossListViewModel<Models.Tbl33Ordo>, ITaxonViewModel
{
    public Tbl33OrdoViewModel(
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

    protected override Models.Tbl33Ordo CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl33Ordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl33Ordo?> SaveItemAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl33Ordo model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl33Ordo item, string column)
    {
        return column switch
        {
            "Id" => item.OrdoId,
            "Info" => item.OrdoName ?? "",
            _ => ""
        };
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForLegioAsync(int id) => Task.CompletedTask;
}
