using System.ComponentModel;
using System.Runtime.CompilerServices;
using ATIS.Interfaces;
using ATIS.Services;
using Microsoft.EntityFrameworkCore;

namespace ATIS.Views.Search.DetailPages;

public sealed partial class Tbl06PhylumDetailPage : Page, INotifyPropertyChanged
{
    private readonly IDataService _dataService;

    public Tbl06PhylumDetailPage()
    {
        InitializeComponent();
        DataContext = this;

        var factory = App.Services.GetRequiredService<IDbContextFactory<AtisDbContext>>();
        var user = App.Services.GetRequiredService<IUserService>();
        _dataService = new DataService(factory, user);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    //protected override async void OnNavigatedTo(NavigationEventArgs e)
    //{
    //    base.OnNavigatedTo(e);

    //    if (e.Parameter is not string idText || !int.TryParse(idText, out var id))
    //    {
    //        DetailIdTextBlock.Text = "(ungültige Id)";
    //        return;
    //    }

    //    try
    //    {
    //        IsBusy = true;

    //        var item = await _dataService.GetPhylumByIdAsync(id);

    //        if (item is null)
    //        {
    //            DetailIdTextBlock.Text = $"Nicht gefunden: {id}";
    //            DetailTitleTextBlock.Text = string.Empty;
    //            DetailSubtitleTextBlock.Text = string.Empty;
    //            return;
    //        }

    //        DetailIdTextBlock.Text = item.PhylumId.ToString();
    //        DetailTitleTextBlock.Text = item.PhylumName ?? string.Empty;
    //        DetailSubtitleTextBlock.Text = item.Author ?? string.Empty;
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

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

        int? id = e.Parameter switch
        {
            int i => i,
            string s when int.TryParse(s, out var p) => p,
            SearchResultItem r => r.EntityId,
            _ => null
        };

        if (id is null)
        {
            System.Diagnostics.Debug.WriteLine($"Invalid nav parameter: {e.Parameter?.GetType().FullName ?? "null"}");
            return;
        }

        LoadById(id.Value);
    }
    private async void LoadById(int id)
    {
        // Daten laden...
    }
}
