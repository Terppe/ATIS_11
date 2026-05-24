using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl24Subclass;

public class Tbl24SubclassViewModel : BaseListViewModel<Models.Tbl24Subclass>, ITaxonViewModel
{
    public Tbl24SubclassViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl24Subclass>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl24Subclass CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl24Subclass?> SaveItemAsync(Models.Tbl24Subclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl24Subclass item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForClassAsync(int id) => Task.CompletedTask;
}
