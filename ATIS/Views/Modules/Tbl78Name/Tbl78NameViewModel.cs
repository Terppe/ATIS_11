using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl78Name;

public class Tbl78NameViewModel : BaseListViewModel<Models.Tbl90Reference>
{
    public Tbl78NameViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl90Reference>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl90Reference CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl90Reference?> SaveItemAsync(Models.Tbl90Reference item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl90Reference item)
    {
        throw new NotImplementedException();
    }
}
