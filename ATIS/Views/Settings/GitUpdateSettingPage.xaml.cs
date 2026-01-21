

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.Settings;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GitUpdateSettingPage : Page
{
    public GitUpdateSettingViewModel ViewModel { get; }

    public GitUpdateSettingPage()
    {
        ViewModel = App.GetService<GitUpdateSettingViewModel>();
        InitializeComponent();
    }


 //   [Obsolete]
    //private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(TxtUser.Text) || string.IsNullOrEmpty(TxtRepo.Text))
    //    {
    //        TxtUser.Text = "Terppe";
    //        TxtRepo.Text = "ATIS";
    //    }
    //    var ver = await Update.UpdateHelper.CheckUpdateAsync(TxtUser.Text, TxtRepo.Text);
    //    if (ver.IsExistNewVersion)
    //    {
    //        TxtReleaseUrl.Text = $"Release Url: {ver.HtmlUrl}";
    //        TxtCreatedAt.Text = $"Created At: {ver.CreatedAt}";
    //        TxtPublishedAt.Text = $"Published At {ver.PublishedAt}";
    //        TxtIsPreRelease.Text = $"Is PreRelease: {ver.IsPreRelease}";
    //        TxtTagName.Text = $"Tag Name: {ver.TagName}";
    //        TxtChangelog.Text = $"Changelog: {ver.Changelog}";
    //        foreach (var item in ver.Assets!)
    //        {
    //            ListView.Items.Add($"{item.Url}{Environment.NewLine}Size: {item.Size}");
    //        }
    //    }
    //}

}
