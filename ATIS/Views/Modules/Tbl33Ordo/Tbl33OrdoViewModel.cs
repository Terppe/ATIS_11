using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl33Ordo;

public class Tbl33OrdoViewModel : BaseListViewModel<Models.Tbl33Ordo>, ITaxonViewModel
{
    public Tbl33OrdoViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl33Ordo>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl33Ordo CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl33Ordo?> SaveItemAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl33Ordo item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForLegioAsync(int id) => Task.CompletedTask;
}
