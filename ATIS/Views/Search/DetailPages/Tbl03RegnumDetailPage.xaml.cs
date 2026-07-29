using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Search.DetailPages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Tbl03RegnumDetailPage : Page
{
    public Tbl03RegnumDetailPage()
    {
        InitializeComponent();
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
