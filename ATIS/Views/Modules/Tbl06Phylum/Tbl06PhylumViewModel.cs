using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl06Phylum;

public class Tbl06PhylumViewModel : BaseBossListViewModel<Models.Tbl06Phylum>
{
    private readonly IDataService _data;

    public Tbl06PhylumViewModel(
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

    protected override Models.Tbl06Phylum CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl06Phylum>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl06Phylum?> SaveItemAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl06Phylum model)
    {
        throw new NotImplementedException();
    }

    protected override object GetSortKey(Models.Tbl06Phylum item, string column)
    {
        return column switch
        {
            "Id" => item.PhylumId,
            "Info" => item.PhylumName ?? "",
            _ => ""
        };
    }


    public void ResetBelow() { }
    public Task LoadForRegnumAsync(int id) => Task.CompletedTask;

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
