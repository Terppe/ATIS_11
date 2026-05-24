using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl39Infraordo;

public class Tbl39InfraordoViewModel : BaseListViewModel<Models.Tbl39Infraordo>, ITaxonViewModel
{
    public Tbl39InfraordoViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl39Infraordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl39Infraordo CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl39Infraordo?> SaveItemAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl39Infraordo item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForSubordoAsync(int id) => Task.CompletedTask;
}
