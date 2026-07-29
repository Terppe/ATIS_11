

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using ATIS.Services;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Search.DetailPages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl12SubphylumDetailPage : Page
{
    private readonly IDbContextFactory<AtisDbContext> _factory;

    public Tbl12SubphylumDetailPage()
    {
        InitializeComponent();
        _factory = App.GetService<IDbContextFactory<AtisDbContext>>();
    }
    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame?.CanGoBack == true)
            Frame.GoBack();
        else
            Frame?.Navigate(typeof(SearchPage));
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        int id;
        if (e.Parameter is int i) id = i;
        else if (e.Parameter is string s && int.TryParse(s, out var parsed)) id = parsed;
        else if (e.Parameter is SearchResultItem r) id = r.EntityId;
        else return;

        _ = LoadByIdAsync(id);
    }
    private async Task LoadByIdAsync(int id)
    {
        try
        {
            await using var context = _factory.CreateDbContext();

            var entity = await context.Tbl12Subphylums
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SubphylumId == id);

            if (entity is null)
            {
                System.Diagnostics.Debug.WriteLine($"Subphylum {id} nicht gefunden.");
                return;
            }

            // Variante A: direkt ans ViewModel geben
            // ViewModel.SetEntity(entity);

            // Variante B: falls es eine Selected-Property gibt
            // ViewModel.SelectedSubphylum = entity;

            // Variante C: falls du schon Load(id) im VM hast, dann diese Methode oben direkt nutzen.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadByIdAsync Fehler: {ex}");
        }
    }
}
