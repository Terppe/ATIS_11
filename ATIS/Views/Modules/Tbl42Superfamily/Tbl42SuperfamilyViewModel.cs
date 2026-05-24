using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl42Superfamily;

public class Tbl42SuperfamilyViewModel : BaseListViewModel<Models.Tbl42Superfamily>, ITaxonViewModel
{
    public Tbl42SuperfamilyViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl42Superfamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl42Superfamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl42Superfamily?> SaveItemAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl42Superfamily item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForInfraordoAsync(int id) => Task.CompletedTask;
}
