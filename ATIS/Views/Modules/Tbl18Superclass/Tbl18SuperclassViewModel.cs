using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Interfaces;
using ATIS.Views.Base;

namespace ATIS.Views.Modules.Tbl18Superclass;

public class Tbl18SuperclassViewModel : BaseListViewModel<Models.Tbl18Superclass>, ITaxonViewModel
{
    public Tbl18SuperclassViewModel(IDataService data) : base(data)
    {
    }

    protected override async Task<IEnumerable<Models.Tbl18Superclass>> LoadItemsAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl18Superclass CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<Models.Tbl18Superclass?> SaveItemAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl18Superclass item)
    {
        throw new NotImplementedException();
    }

    public void ResetBelow() { }
    public Task LoadForSubphylumAsync(int id) => Task.CompletedTask;
}
