using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl93Comment;

public class Tbl93CommentListViewModel : BaseChildListViewModel<Models.Tbl93Comment>
{
    private readonly IDataService _data;

    public Tbl93CommentListViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IUserService user,
        IDialogService dialog)
        : base(
            new DataService(factory, user),
            dialog,
            parentId =>
            {
                var data = new DataService(factory, user);
                return data.GetCommentsForSubtribusAsync(parentId);
            })
    {
        _data = new DataService(factory, user);
    }

    private Models.Tbl93Comment? _selectedItem;

    public Models.Tbl93Comment? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnSelectedItemChangedAsync();
            }
        }
    }

    public Models.Tbl93Comment? SelectedComment
    {
        get => SelectedItem;
        set => SelectedItem = value;
    }

    protected override Task OnSelectedItemChangedAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }
}
