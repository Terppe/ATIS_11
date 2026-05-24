using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl36Subordo;

public class Tbl36SubordoViewModel : BaseListViewModel<Models.Tbl36Subordo>, ITaxonViewModel
{
    public Tbl36SubordoViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl36Subordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl36Subordo CreateNewItem()
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

    public void ResetBelow() { }
    public Task LoadForOrdoAsync(int id) => Task.CompletedTask;
}
