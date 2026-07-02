using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl42Superfamily;

public class Tbl42SuperfamilyViewModel : BaseBossListViewModel<Models.Tbl42Superfamily>, ITaxonViewModel
{
    public Tbl42SuperfamilyViewModel(
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

    protected override Models.Tbl42Superfamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl42Superfamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl42Superfamily?> SaveItemAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl42Superfamily model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl42Superfamily item, string column)
    {
        return column switch
        {
            "Id" => item.SuperfamilyId,
            "Info" => item.SuperfamilyName ?? "",
            _ => ""
        };
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }


    public void ResetBelow() { }
    public Task LoadForInfraordoAsync(int id) => Task.CompletedTask;
}
