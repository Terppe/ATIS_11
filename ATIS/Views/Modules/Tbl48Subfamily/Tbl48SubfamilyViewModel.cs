using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl48Subfamily;

public class Tbl48SubfamilyViewModel : BaseListViewModel<Models.Tbl48Subfamily>, ITaxonViewModel
{
    public Tbl48SubfamilyViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl48Subfamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl48Subfamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl48Subfamily?> SaveItemAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl48Subfamily item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForFamilyAsync(int id) => Task.CompletedTask;
}
