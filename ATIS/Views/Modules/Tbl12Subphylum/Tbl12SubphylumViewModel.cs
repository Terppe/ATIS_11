using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl12Subphylum;

public class Tbl12SubphylumViewModel : BaseListViewModel<Models.Tbl12Subphylum>, ITaxonViewModel
{
    public Tbl12SubphylumViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl12Subphylum>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl12Subphylum CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl12Subphylum?> SaveItemAsync(Models.Tbl12Subphylum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl12Subphylum item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForPhylumAsync(int id) => Task.CompletedTask;
}
