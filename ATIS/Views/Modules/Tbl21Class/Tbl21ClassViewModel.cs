using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl21Class;

public class Tbl21ClassViewModel : BaseListViewModel<Models.Tbl21Class>, ITaxonViewModel
{
    public Tbl21ClassViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl21Class>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl21Class CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl21Class?> SaveItemAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl21Class item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForSuperclassAsync(int id) => Task.CompletedTask;
}
