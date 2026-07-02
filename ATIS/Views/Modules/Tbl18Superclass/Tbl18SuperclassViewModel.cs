using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl18Superclass;

public class Tbl18SuperclassViewModel : BaseBossListViewModel<Models.Tbl18Superclass>
{
    public Tbl18SuperclassViewModel(
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

    protected override Models.Tbl18Superclass CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl18Superclass>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl18Superclass?> SaveItemAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl18Superclass model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl18Superclass item, string column)
    {
        return column switch
        {
            "Id" => item.SuperclassId,
            "Info" => item.SuperclassName ?? "",
            _ => ""
        };
    }


    public void ResetBelow() { }

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
