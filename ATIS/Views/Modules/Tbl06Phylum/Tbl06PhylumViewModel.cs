using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl06Phylum;

public class Tbl06PhylumViewModel : BaseListViewModel<Models.Tbl06Phylum>, ITaxonViewModel
{
    public Tbl06PhylumViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl06Phylum>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl06Phylum CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl06Phylum?> SaveItemAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl06Phylum item)
    {
        throw new NotImplementedException();
    }
    public void ResetBelow() { }
    public Task LoadForRegnumAsync(int id) => Task.CompletedTask;
}
