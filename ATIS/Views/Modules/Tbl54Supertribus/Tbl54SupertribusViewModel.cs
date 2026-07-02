using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl57Tribus;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl54Supertribus;

public class Tbl54SupertribusViewModel : BaseBossListViewModel<Models.Tbl54Supertribus>
{
    public ICommand SortCommand { get; }
    public IRelayCommand BackCommand { get; }

    // ============================================
    // CONSTRUCTOR
    // ============================================
    public Tbl54SupertribusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookup,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        SortCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<string>(SortBy);
        BackCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteBack);

        RegisterHeader("Id", "Supertribus ID");
        RegisterHeader("Name", "Supertribus Name");

    }
    // ============================================
    // CHILD VIEWMODELS
    // ============================================
    //public Tbl57TribusViewModel TribusViewModel
    //{
    //    get => ChildViewModels.TryGetValue("Tribus", out var vm)
    //        ? (Tbl57TribusViewModel)vm
    //        : null!;
    //    set => ChildViewModels["Tribus"] = value;
    //}
    //public Tbl90ReferenceViewModel ReferenceViewModel
    //{
    //    get => ChildViewModels.TryGetValue("Reference", out var vm)
    //        ? (Tbl90ReferenceViewModel)vm
    //        : null!;
    //    set => ChildViewModels["Reference"] = value;
    //}
    //public Tbl93CommentViewModel CommentViewModel
    //{
    //    get => ChildViewModels.TryGetValue("Comment", out var vm)
    //        ? (Tbl93CommentViewModel)vm
    //        : null!;
    //    set => ChildViewModels["Comment"] = value;
    //}
    // ============================================
    // HIERARCHIE-KONFIGURATION
    // ============================================
    //protected override HierarchyLevel CurrentLevel => HierarchyLevel.Supertribus;
    //protected override HierarchyLevel ParentLevel { get; }
    //   protected override HierarchyLevel ParentLevel => HierarchyLevel.Unknown; // Kein Parent für Supertribus


    // ============================================
    // TEMPLATE IMPLEMENTATIONS
    // ============================================

    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl54Supertribus CreateNewItem()
    {
        throw new NotImplementedException();
    }

    protected override async Task<IEnumerable<Models.Tbl54Supertribus>> LoadItemsAsync()
    {
        if (string.IsNullOrWhiteSpace(default))
            return Enumerable.Empty<Models.Tbl54Supertribus>();

        return Enumerable.Empty<Models.Tbl54Supertribus>();
    }

    protected override async Task<Models.Tbl54Supertribus?> SaveItemAsync(Models.Tbl54Supertribus item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl54Supertribus item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmSaveAsync(Models.Tbl54Supertribus item)
    {
        throw new NotImplementedException();
    }

    protected override async Task<bool> ConfirmDeleteAsync(Models.Tbl54Supertribus item)
    {
        throw new NotImplementedException();
    }

    protected override int GetId(Models.Tbl54Supertribus model)
    {
        throw new NotImplementedException();
    }


    public override async Task LoadParentAsync(int parentId)
    {
        throw new NotImplementedException();
    }

    protected override async Task ClearChildCollectionsAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task LoadChildrenAsync(int entityId)
    {
        throw new NotImplementedException();
    }




    //protected override async Task LoadForParentAsync(int parentId)
    //{
    //    throw new NotImplementedException();
    //}

    //protected override async Task ClearChildCollectionsAsync()
    //{
    //    TribusViewModel.Items?.Clear();
    //    await Task.CompletedTask;
    //}

    //protected override async Task LoadChildrenAsync(int entityId)
    //{
    //    await TribusViewModel.LoadForSupertribusAsync(entityId);
    //}

    // ============================================
    // DETAIL VISIBILITY
    // ============================================
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }
    public void ResetBelow()
    {
 //       SubtribusViewModel.Items?.Clear();
        //ReferenceViewModel.Items?.Clear();
        //CommentViewModel.Items?.Clear();
    }

    // ============================================
    // SORTING HEADERS
    // ============================================
    protected override object GetSortKey(Models.Tbl54Supertribus item, string column)
    {
        return column switch
        {
            "Id" => item.SupertribusId,
            "Info" => item.SupertribusName ?? "",
            _ => ""
        };
    }
    // ---------------------------------------------------------
    // BACK
    // ---------------------------------------------------------

    private void ExecuteBack()
    {
        //SelectedItem = null;
        //OnPropertyChanged(nameof(IsDetailVisible));

        //IsInParentMode = true;
        //SelectedDetailPivotIndex = 0;

        //NewCommand.NotifyCanExecuteChanged();
        //RefreshCommand.NotifyCanExecuteChanged();
    }


    //private string _idHeaderText = "ID";
    //public string IdHeaderText
    //{
    //    get => _idHeaderText;
    //    set => SetProperty(ref _idHeaderText, value);
    //}

    //private string _nameHeaderText = "Name";
    //public string NameHeaderText
    //{
    //    get => _nameHeaderText;
    //    set => SetProperty(ref _nameHeaderText, value);
    //}


    ////protected override IEnumerable<Models.Tbl54Supertribus> SortItems(
    ////    IEnumerable<Models.Tbl54Supertribus> items, string column, bool ascending)
    ////{
    ////    return column switch
    ////    {
    ////        "Id" => ascending
    ////            ? items.OrderBy(x => x.SupertribusId)
    ////            : items.OrderByDescending(x => x.SupertribusId),
    ////        "Name" => ascending
    ////            ? items.OrderBy(x => x.SupertribusName)
    ////            : items.OrderByDescending(x => x.SupertribusName),
    ////        _ => items
    ////    };
    ////}

    ////protected override void UpdateHeaderTexts()
    ////{
    ////    const string asc = " ▲";
    ////    const string desc = " ▼";

    ////    IdHeaderText = "ID";
    ////    NameHeaderText = "Name";

    ////    // Optional: Sorting-Pfeile hinzufügen wenn nötig
    ////}
}
