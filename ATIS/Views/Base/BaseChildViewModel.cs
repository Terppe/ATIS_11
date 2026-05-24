using System.Collections.ObjectModel;
using ATIS.Interfaces;
using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Hierarchy;

namespace ATIS.Views.Base;

public abstract class BaseChildViewModel<TModel> : BaseBossListViewModel<TModel>
    where TModel : class, new()
{
    private new readonly IDataService _data;

    // Parent-IDs für hierarchische Beziehungen
    public int? CurrentParentId { get; set; }

    protected BaseChildViewModel(IDataService data, IDialogService dialog, HierarchyCoordinator coordinator)
        : base(data, dialog, coordinator) // Konstruktor der Basisklasse aufrufen
    {
        _data = data;
    }

    // Generische Logik: Lädt Items basierend auf der Parent-ID
    protected override async Task<IEnumerable<TModel>> LoadForParentAsync(int parentId)
    {
        return await LoadItemsByParentIdAsync(parentId); // Kind-Logik definieren
    }

    // Methode für spezifische Kind-Implementierung (Subtribus, Comment, etc.)
    protected abstract Task<IEnumerable<TModel>> LoadItemsByParentIdAsync(int parentId);

    // Generisches Löschen, basierend auf Kind-Logik
    protected override async Task<bool> DeleteItemAsync(TModel item)
    {
        if (!await CanDeleteItemAsync(item))
        {
            await DialogService.ShowErrorDialogAsync( // Fehler anzeigen, falls nicht löschbar
                "Error_DeleteNotAllowed_Title",
                "Error_DeleteNotAllowed_Message"
            );
            return false;
        }

        await DeleteItemFromDatabaseAsync(item);
        return true;
    }

    // Abstrakte Methoden für delete-spezifische Kind-Logik
    protected abstract Task<bool> CanDeleteItemAsync(TModel item);
    protected abstract Task DeleteItemFromDatabaseAsync(TModel item);
}
