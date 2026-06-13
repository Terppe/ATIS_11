using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl90Reference;

public class Tbl90ReferenceListViewModel : BaseChildListViewModel<Models.Tbl90Reference>
{
    private readonly IDataService _data;

    public Tbl90ReferenceListViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IUserService user,
        IDialogService dialog)
        : base(
            new DataService(factory, user),
            dialog,
            parentId =>
            {
                var data = new DataService(factory, user);
                return data.GetReferencesForSubtribusAsync(parentId);
            })
    {
        _data = new DataService(factory, user);

        RefAuthorsList = new ObservableCollection<Models.Tbl90RefAuthor>();
        RefSourcesList = new ObservableCollection<Models.Tbl90RefSource>();
        RefExpertsList = new ObservableCollection<Models.Tbl90RefExpert>();
    }

    //    // ComboBox-Listen
    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; }
    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; }
    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; }

    public async Task LoadLookupListsAsync()
    {
        RefAuthorsList.Clear();
        RefSourcesList.Clear();
        RefExpertsList.Clear();

        // 👇 Jetzt korrekt: _user statt User
        var authors = await _data.GetAllRefAuthorsAsync();
        foreach (var a in authors)
            RefAuthorsList.Add(a);

        var sources = await _data.GetAllRefSourcesAsync();
        foreach (var s in sources)
            RefSourcesList.Add(s);

        var experts = await _data.GetAllRefExpertsAsync();
        foreach (var e in experts)
            RefExpertsList.Add(e);
    }

    public Models.Tbl90Reference? SelectedReference
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

//public class Tbl90ReferenceListViewModel : BaseChildListViewModel<Models.Tbl90Reference>
//{
//    private static IDataService CreateDataService(IUserService user)
//        => new DataService(new AtisDbContext(), user);

//    public Tbl90ReferenceListViewModel(IUserService user, IDialogService dialog)
//        : base(CreateDataService(user), dialog, parentId => CreateDataService(user).GetReferencesForSubtribusAsync(parentId))
//    {
//        RefAuthorsList = new ObservableCollection<Models.Tbl90RefAuthor>();
//        RefSourcesList = new ObservableCollection<Models.Tbl90RefSource>();
//        RefExpertsList = new ObservableCollection<Models.Tbl90RefExpert>();
//    }

//    public Models.Tbl90Reference? SelectedReference
//    {
//        get => SelectedItem;
//        set => SelectedItem = value;
//    }

//    // ComboBox-Listen
//    public ObservableCollection<Models.Tbl90RefAuthor> RefAuthorsList { get; }
//    public ObservableCollection<Models.Tbl90RefSource> RefSourcesList { get; }
//    public ObservableCollection<Models.Tbl90RefExpert> RefExpertsList { get; }

//    public async Task LoadLookupListsAsync()
//    {
//        RefAuthorsList.Clear();
//        RefSourcesList.Clear();
//        RefExpertsList.Clear();

//        var authors = await Data.GetAllRefAuthorsAsync();
//        foreach (var a in authors)
//            RefAuthorsList.Add(a);

//        var sources = await Data.GetAllRefSourcesAsync();
//        foreach (var s in sources)
//            RefSourcesList.Add(s);

//        var experts = await Data.GetAllRefExpertsAsync();
//        foreach (var e in experts)
//            RefExpertsList.Add(e);
//    }
//}
