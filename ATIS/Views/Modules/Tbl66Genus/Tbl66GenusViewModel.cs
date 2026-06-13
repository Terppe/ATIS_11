using System.Collections.ObjectModel;
using System.Windows.Input;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Main;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl66Genus;

public class Tbl66GenusViewModel
    : BaseBossListViewModel<Models.Tbl66Genus>
{
    public Tbl66GenusViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialog,
        HierarchyCoordinator coordinator,
        LookupService lookupService,
        IUserService user)
        : base(new DataService(factory, user), dialog, coordinator)
    {
        SpeciesViewModel = new Tbl75SpeciesListViewModel(factory, user, dialog);
        CommentViewModel = new Tbl93CommentListViewModel(factory, user, dialog);
        ReferenceViewModel = new Tbl90ReferenceListViewModel(factory, user, dialog);

        IsDetailVisible = false;
        IsEditVisible = false;
    }
    // ---------------------------------------------------------
    // CHILD VIEWMODELS (für Genus)
    // ---------------------------------------------------------
    public Tbl75SpeciesListViewModel SpeciesViewModel { get; }
    public Tbl93CommentListViewModel CommentViewModel { get; }
    public Tbl90ReferenceListViewModel ReferenceViewModel { get; }

    // ---------------------------------------------------------
    // UI-STATE
    // ---------------------------------------------------------
    private readonly LookupService _lookupService;
    private bool _isDetailVisible;
    public bool IsDetailVisible
    {
        get => _isDetailVisible;
        set => SetProperty(ref _isDetailVisible, value);
    }

    private bool _isEditVisible;
    public bool IsEditVisible
    {
        get => _isEditVisible;
        set => SetProperty(ref _isEditVisible, value);
    }

    // Parent: Infratribus
    private int? _currentInfratribusId;


    // ---------------------------------------------------------
    // LOAD PARENT (Infratribus → Genus)
    // ---------------------------------------------------------
    protected override int GetId(Models.Tbl66Genus model)
        => model.GenusId;


    public override async Task LoadParentAsync(int parentId)
    {
        _currentInfratribusId = parentId;

        var items = await Data.GetGenusForInfratribusAsync(parentId);
        Items = new ObservableCollection<Models.Tbl66Genus>(items);
    }

    // ---------------------------------------------------------
    // LOAD CHILDREN (Genus → Species, Comments, References)
    // ---------------------------------------------------------
    protected override async Task LoadChildrenAsync(int entityId)
    {
        await SpeciesViewModel.LoadForParentAsync(entityId);
        await CommentViewModel.LoadForParentAsync(entityId);
        await ReferenceViewModel.LoadForParentAsync(entityId);

        IsDetailVisible = true;
    }

    // ---------------------------------------------------------
    // CLEAR CHILDREN
    // ---------------------------------------------------------
    protected override Task ClearChildCollectionsAsync()
    {
        SpeciesViewModel.Items?.Clear();
        CommentViewModel.Items?.Clear();
        ReferenceViewModel.Items?.Clear();

        IsDetailVisible = false;
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------
    // CRUD
    // ---------------------------------------------------------
    protected override async Task NewAsync()
    {
        throw new NotImplementedException();
    }

    protected override Models.Tbl66Genus CreateNewItem()
        => new Models.Tbl66Genus
        {
            InfratribusId = _currentInfratribusId ?? 0,
            Valid = true
        };

    protected override async Task<IEnumerable<Models.Tbl66Genus>> LoadItemsAsync()
    {
        if (_currentInfratribusId is null)
            return Enumerable.Empty<Models.Tbl66Genus>();

        return await Data.GetGenusForInfratribusAsync(_currentInfratribusId.Value);
    }

    protected override async Task<Models.Tbl66Genus?> SaveItemAsync(Models.Tbl66Genus item)
    {
        if (string.IsNullOrWhiteSpace(item.GenusName))
        {
            await Dialog.ShowErrorAsync("Name fehlt", "Bitte einen gültigen Genus-Namen eingeben.");
            return default;
        }

        var saved = await Data.UpsertGenusAsync(item);
        return await Data.GetGenusByIdAsync(saved.GenusId);
    }

    protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    {
        if (item.GenusId == 0)
            return false; // Unsaved

        if (await Data.GenusHasSpeciesAsync(item.GenusId) ||
            await Data.GenusHasReferenceAsync(item.GenusId) ||
            await Data.GenusHasCommentAsync(item.GenusId))
        {
            await Dialog.ShowErrorAsync(
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await Data.DeleteGenusAsync(item.GenusId);
        return true;
    }

    //protected override async Task<bool> DeleteItemAsync(Models.Tbl66Genus item)
    //{
    //    if (item.GenusId == 0)
    //        return false;

    //    if (await Data.GenusHasChildrenAsync(item.GenusId))
    //    {
    //        await Dialog.ShowErrorAsync(
    //            "Löschen nicht möglich",
    //            "Dieses Genus hat abhängige Daten."
    //        );
    //        return false;
    //    }

    //    await Data.DeleteGenusAsync(item.GenusId);
    //    return true;
    //}

    protected override Task<bool> ConfirmSaveAsync(Models.Tbl66Genus item)
        => Dialog.ShowConfirmAsync("Speichern?", "Möchten Sie dieses Genus speichern?");

    protected override Task<bool> ConfirmDeleteAsync(Models.Tbl66Genus item)
        => Dialog.ShowConfirmAsync("Löschen?", "Möchten Sie dieses Genus wirklich löschen?");
}
