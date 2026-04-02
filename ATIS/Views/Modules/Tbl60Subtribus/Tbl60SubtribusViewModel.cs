using ATIS.Services;
using ATIS.Views.Base;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Modules.Tbl60Subtribus;

public class Tbl60SubtribusViewModel : BaseListViewModel<Models.Tbl60Subtribus>
{
    private readonly AtisDbContext _db;

    public Tbl60SubtribusViewModel(AtisDbContext db, Tbl93CommentViewModel commentViewModel)
    {
        _db = db;
        CommentViewModel = commentViewModel;
    }

    // ---------------------------------------------------------
    // PROPERTIES
    // ---------------------------------------------------------
    public Tbl93CommentViewModel CommentViewModel { get; }
    public string? SearchText { get; set; }

    public int DataSetCount => Items.Count;

    // ---------------------------------------------------------
    // LOAD + FILTER
    // ---------------------------------------------------------

    public async Task LoadAsync()
    {
        await ReloadAsync();
    }

    public async Task ApplyFilterAsync(string? search)
    {
        SearchText = search ?? "";

        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Items.Clear();
            return;
        }

        var result = await _db.Tbl60Subtribusses
            .AsNoTracking()
            .Where(x =>
                x.SubtribusName.StartsWith(SearchText))
            .OrderBy(x => x.SubtribusName)
            .ToListAsync();

        Items.Clear();
        foreach (var item in result)
            Items.Add(item);
    }

    protected override async Task<IEnumerable<Models.Tbl60Subtribus>> LoadItemsAsync()
    {
        var query = _db.Tbl60Subtribusses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var t = SearchText.Trim();
            query = query.Where(x =>
                x.SubtribusName.Contains(t) ||
                x.Author!.Contains(t) ||
                x.Info!.Contains(t));
        }

        return await query
            .OrderBy(x => x.SubtribusName)
            .ToListAsync();
    }

    // ---------------------------------------------------------
    // SELECTION API (für Page)
    // ---------------------------------------------------------

    public void SelectItem(Models.Tbl60Subtribus? item)
    {
        SelectedItem = item;
    }

    // SaveAsync / DeleteAsync wie gehabt ...
}
