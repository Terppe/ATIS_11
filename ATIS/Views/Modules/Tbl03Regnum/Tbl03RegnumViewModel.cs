using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl03Regnum;

public class Tbl03RegnumViewModel : BaseListViewModel<Models.Tbl03Regnum>, ITaxonViewModel
{
    public Tbl03RegnumViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl03Regnum>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl03Regnum CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl03Regnum?> SaveItemAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl03Regnum item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForRegnumAsync(int id) => Task.CompletedTask;
}
