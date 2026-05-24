using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl51Infrafamily;

public class Tbl51InfrafamilyViewModel : BaseListViewModel<Models.Tbl51Infrafamily>, ITaxonViewModel
{
    public Tbl51InfrafamilyViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl51Infrafamily>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl51Infrafamily CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl51Infrafamily?> SaveItemAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl51Infrafamily item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForSubfamilyAsync(int id) => Task.CompletedTask;
}
