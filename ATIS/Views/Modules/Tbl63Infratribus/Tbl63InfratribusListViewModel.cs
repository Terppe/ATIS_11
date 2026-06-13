using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl63Infratribus;

public class Tbl63InfratribusListViewModel : BaseChildListViewModel<Models.Tbl63Infratribus>
{
    private readonly IUserService _user;
    private readonly IDataService _data;

    public Tbl63InfratribusListViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IUserService user,
        IDialogService dialog)
        : base(
            new DataService(factory, user),
            dialog,
            parentId =>
            {
                var data = new DataService(factory, user);
                return data.GetInfratribusForSubtribusAsync(parentId);
            })
    {
        _user = user;
        _data = new DataService(factory, user);
    }


    public Models.Tbl63Infratribus? SelectedInfratribus
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    protected override Task OnSelectedItemChangedAsync()
    {
        // Falls später Detail- oder Edit-UI sichtbar werden soll
        return Task.CompletedTask;
    }

    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }
}

//public class Tbl63InfratribusListViewModel : BaseChildListViewModel<Models.Tbl63Infratribus>
//{
//    private static IDataService CreateDataService(IUserService user)
//        => new DataService(new AtisDbContext(), user);

//    public Tbl63InfratribusListViewModel(IUserService user, IDialogService dialog)
//        : base(CreateDataService(user), dialog, parentId => CreateDataService(user).GetInfratribusForSubtribusAsync(parentId))
//    {
//    }

//    private bool _isEditVisible;
//    public bool IsEditVisible
//    {
//        get => _isEditVisible;
//        set => SetProperty(ref _isEditVisible, value);
//    }


//    protected override Task OnSelectedItemChangedAsync()
//    {
//        IsEditVisible = SelectedItem != null;
//        return Task.CompletedTask;
//    }
//}
