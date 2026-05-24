using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl45Family;

public class Tbl45FamilyViewModel : BaseListViewModel<Models.Tbl45Family>, ITaxonViewModel
{
    public Tbl45FamilyViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl45Family>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl45Family CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl45Family?> SaveItemAsync(Models.Tbl45Family item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl45Family item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForSuperfamilyAsync(int id) => Task.CompletedTask;
}
