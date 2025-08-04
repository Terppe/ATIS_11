using System.Configuration;
using ATIS.Strings;
using System.Diagnostics;

namespace ATIS.Views.Main;

public sealed partial class HomeLandingPage : Page
{
    public string AppInfo { get; set; }
    public HomeLandingViewModel ViewModel { get; }
    public string WinAppSdkVersion { get; set; } = ConfigurationManager.AppSettings.Get("WinAppSDK");

    public HomeLandingPage()
    {
        ViewModel = App.GetService<HomeLandingViewModel>();
        InitializeComponent();


        AppInfo = $"{App.Current.AppName} v{App.Current.AppVersion}";
        //LbVersion.Text = $"{App.Current.Copyright} | .NET Core Version: " + Environment.Version +
        //                 " + OS-Version " + Environment.OSVersion +
        //                 " | ATIS-Version " + $"{App.Current.AppVersion}";
        LbVersion.Text = $"{App.Current.Copyright} " +
                         " | SDK Version: " + WinAppSdkVersion +
                         " | .NET Core Version: " + Environment.Version +
                         " + OS-Version " + Environment.OSVersion +
                         " | ATIS-Version " + $"{App.Current.AppVersion}";
     //   MainLandingsPage.HeaderSubtitleText = StringsRes.MainLandingsPageHeaderSubtitleText;
      //  MainLandingsPage.HeaderSubtitleText = WinAppSdkVersion;  //from App.config
        HeaderTileDocCenter.Title = StringsRes.HeaderTileDocCenterTitle;
        HeaderTileDocuments.Title = StringsRes.HeaderTileDocumentsTitle;
        HeaderTileContextMenuExt.Title = StringsRes.HeaderTileContextMenuExtTitle;
        HeaderTileItisUsa.Title = StringsRes.HeaderTileItisUsaTitle;
        HeaderTileItisCanada.Title = StringsRes.HeaderTileItisCanadaTitle;
        HeaderTileCatalogOfLife.Title = StringsRes.HeaderTileCatalogOfLifeTitle;
        HeaderTileMexico.Title = StringsRes.HeaderTileMexicoTitle;
        HeaderTileGlobalBio.Title = StringsRes.HeaderTileGlobalBioTitle;
        HeaderTileEol.Title = StringsRes.HeaderTileEolTitle;

        Height = Convert.ToDouble(ConfigurationManager.AppSettings.Get("HomeHeight"));
    }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        MainLandingsPage.GetData(ViewModel.JsonNavigationViewService.DataSource);
        MainLandingsPage.OrderBy(i => i.Title);
    }

    private void MainLandingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        MainLandingsPage.GetData(ViewModel.JsonNavigationViewService.DataSource);
    }

    private void MainLandingsPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs)e;
        var item = (DataItem)args.ClickedItem;
        ViewModel.JsonNavigationViewService.NavigateTo(item.UniqueId);
    }

    private void HyperlinkButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("mailto:rudolf@terppe.de") { UseShellExecute = true });
    }
}

