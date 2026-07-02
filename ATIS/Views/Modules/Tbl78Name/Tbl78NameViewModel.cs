using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl78Name;

public class Tbl78NameViewModel : BaseBossListViewModel<Models.Tbl78Name>
{
    public Tbl78NameViewModel(
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

    protected override async Task<IEnumerable<Models.Tbl78Name>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }


    protected override int GetId(Models.Tbl78Name model)
        => model.NameId;

    protected override object GetSortKey(Models.Tbl78Name item, string column)
    {
        return column switch
        {
            "Id" => item.NameId,
            "Name" => item.Info ?? "",
            _ => ""
        };
    }

    public override async Task LoadParentAsync(int parentId)
    {
        throw new NotImplementedException();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl78Name CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl78Name item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl78Name?> SaveItemAsync(Models.Tbl78Name item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl78Name item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl78Name item)
    {
        throw new NotImplementedException();
    }
}
