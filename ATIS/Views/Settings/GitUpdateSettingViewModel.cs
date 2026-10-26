using System.Collections.ObjectModel;

namespace ATIS.Views.Settings;

public partial class GitUpdateSettingViewModel : ObservableObject
{
    public IAsyncRelayCommand UpdateCommand { get; }

    public GitUpdateSettingViewModel()
    {
       // UpdateCommand = new AsyncRelayCommand(OnUpdateAsync);
    }

    //[ObservableProperty]
    //private string user;

    //[ObservableProperty]
    //private string repo;
        public string User { get; set; }
        public string Repo { get; set; }
    public string ReleaseUrl { get; set; }
    public string CreatedAt { get; set; }
    public string PublishedAt { get; set; }
    public string IsPreRelease { get; set; }
    public string TagName { get; set; }
    public string Changelog { get; set; }

        public ObservableCollection<string> Assets { get; } = new();

    //[ObservableProperty]
    //private string releaseUrl;

    //[ObservableProperty]
    //private string createdAt;

    //[ObservableProperty]
    //private string publishedAt;

    //[ObservableProperty]
    //private string isPreRelease;

    //[ObservableProperty]
    //private string tagName;

    //[ObservableProperty]
    //private string changelog;

 //   public ObservableCollection<string> Assets { get; } = new();

    //private async Task OnUpdateAsync()
    //{
    //    //if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Repo))
        //{
        //    User = "Terppe";
        //    Repo = "ATIS";
        //}

// //       var ver = await Update.UpdateHelper.CheckUpdateAsync(User, Repo);

//        if (ver.IsExistNewVersion)
//        {
//            ReleaseUrl = $"Release Url: {ver.HtmlUrl}";
//            CreatedAt = $"Created At: {ver.CreatedAt}";
//            PublishedAt = $"Published At: {ver.PublishedAt}";
//            IsPreRelease = $"Is PreRelease: {ver.IsPreRelease}";
//            TagName = $"Tag Name: {ver.TagName}";
//            Changelog = $"Changelog: {ver.Changelog}";

//            Assets.Clear();
//            foreach (var item in ver.Assets!)
//            {
//                Assets.Add($"{item.Url}\nSize: {item.Size}");
//            }
//        }
//    }
//}

//public partial class GitUpdateSettingViewModel : ObservableObject
//{
//    public string User { get; set; }
//    public string Repo { get; set; }
//    public string ReleaseUrl { get; set; }
//    public string CreatedAt { get; set; }
//    public string PublishedAt { get; set; }
//    public string IsPreRelease { get; set; }
//    public string TagName { get; set; }
//    public string Changelog { get; set; }

//    public ObservableCollection<string> Assets { get; } = new();

//    public IAsyncRelayCommand UpdateCommand { get; }
//    public GitUpdateSettingViewModel() { UpdateCommand = new AsyncRelayCommand(OnUpdateAsync); }
//    private async Task OnUpdateAsync()
//    {
//        if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Repo))
//        {
//            User = "Terppe";
//            Repo = "ATIS";
//        }

//        var ver = await Update.UpdateHelper.CheckUpdateAsync(User, Repo);

//        if (ver.IsExistNewVersion)
//        {
//            ReleaseUrl = $"Release Url: {ver.HtmlUrl}";
//            CreatedAt = $"Created At: {ver.CreatedAt}";
//            PublishedAt = $"Published At {ver.PublishedAt}";
//            IsPreRelease = $"Is PreRelease: {ver.IsPreRelease}";
//            TagName = $"Tag Name: {ver.TagName}";
//            Changelog = $"Changelog: {ver.Changelog}";

//            Assets.Clear();
//            foreach (var item in ver.Assets!)
//            {
//                Assets.Add($"{item.Url}\nSize: {item.Size}");
//            }
//        }
//    }
}
