using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public class Tbl51InfrafamilyViewModel : BaseBossListViewModel<Models.Tbl51Infrafamily>, ITaxonViewModel
{
    public Tbl51InfrafamilyViewModel(
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

    protected override Models.Tbl51Infrafamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl51Infrafamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl51Infrafamily?> SaveItemAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl51Infrafamily model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl51Infrafamily item, string column)
    {
        return column switch
        {
            "Id" => item.InfrafamilyId,
            "Info" => item.InfrafamilyName ?? "",
            _ => ""
        };
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForSubfamilyAsync(int id) => Task.CompletedTask;
}
