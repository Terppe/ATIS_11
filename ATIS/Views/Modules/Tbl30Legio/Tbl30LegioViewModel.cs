using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl30Legio;

public class Tbl30LegioViewModel : BaseListViewModel<Models.Tbl30Legio>, ITaxonViewModel
{
    public Tbl30LegioViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl30Legio>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl30Legio CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl30Legio?> SaveItemAsync(Models.Tbl30Legio item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl30Legio item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForInfraclassAsync(int id) => Task.CompletedTask;
}
