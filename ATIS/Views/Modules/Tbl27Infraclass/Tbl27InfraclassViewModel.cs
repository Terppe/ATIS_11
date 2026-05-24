using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl27Infraclass;

public class Tbl27InfraclassViewModel : BaseListViewModel<Models.Tbl27Infraclass>, ITaxonViewModel
{
    public Tbl27InfraclassViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl27Infraclass>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl27Infraclass CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl27Infraclass?> SaveItemAsync(Models.Tbl27Infraclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl27Infraclass item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForSubclassAsync(int id) => Task.CompletedTask;
}
