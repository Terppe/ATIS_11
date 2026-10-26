using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace ATIS.Update;
public static class UpdateHelper
{
    private const string GITHUB_API = "https://api.github.com/repos/{0}/{1}/releases/latest";
    //  private const string GITHUB_API = "https://github.com/{0}/{1}/releases/latest";

    //[Obsolete("Obsolete")]
    //public static async Task<WinUICommunity.UpdateInfo> CheckUpdateAsync(string username, string repository)
    //{
    //    var ver = await UpdateHelper.CheckUpdateAsync("WinUICommunity", "WinUICommunity");
    //    if (ver.IsExistNewVersion)
    //    {
    //        Debug.WriteLine(ver.Url);
    //        //  Debug.WriteLine(ver.ReleaseUrl);
    //        Debug.WriteLine(ver.CreatedAt.ToString());
    //        Debug.WriteLine(ver.PublishedAt.ToString());

    //        //Asset is List so maybe there is more than one file you can use forech or increase index
    //        Debug.WriteLine(ver.Assets[0].Url);
    //        Debug.WriteLine(ver.IsPreRelease.ToString());
    //        Debug.WriteLine(ver.Assets[0].Size.ToString());
    //        //  Debug.WriteLine(ver.Version);
    //        Debug.WriteLine(ver.Changelog);
    //    }

    //    return null!;
    //}

    //public static async Task<WinUICommunity.UpdateInfo> CheckUpdateAsync(
    //string username,
    //string repository,
    //Version currentVersion = null)
    //{
    //    if (string.IsNullOrWhiteSpace(username))
    //        throw new ArgumentNullException(nameof(username));

    //    if (string.IsNullOrWhiteSpace(repository))
    //        throw new ArgumentNullException(nameof(repository));

    //    using var client = new HttpClient();
    //    client.DefaultRequestHeaders.UserAgent.ParseAdd(username);

    //    var url = string.Format(GITHUB_API, username, repository);
    //    var response = await client.GetAsync(url);
    //    response.EnsureSuccessStatusCode();

    //    var responseBody = await response.Content.ReadAsStringAsync();

    //    var result = JsonSerializer.Deserialize<WinUICommunity.UpdateInfo>(responseBody);

    //    if (result is null)
    //        return null;

    //    // Version ermitteln
    //    currentVersion ??= new Version(
    //        Windows.ApplicationModel.Package.Current.Id.Version.Major,
    //        Windows.ApplicationModel.Package.Current.Id.Version.Minor,
    //        Windows.ApplicationModel.Package.Current.Id.Version.Build,
    //        Windows.ApplicationModel.Package.Current.Id.Version.Revision);

    //    var newVersionInfo = GetAsVersionInfo(result.TagName);

    //    var currentVersionInfo = new WinUICommunity.SystemVersionInfo(
    //        currentVersion.Major,
    //        currentVersion.Minor,
    //        currentVersion.Build,
    //        currentVersion.Revision);

    //    return new WinUICommunity.UpdateInfo
    //    {
    //        Changelog = result.Changelog,
    //        CreatedAt = result.CreatedAt,
    //        Assets = result.Assets,
    //        IsPreRelease = result.IsPreRelease,
    //        PublishedAt = result.PublishedAt,
    //        TagName = result.TagName,
    //        AssetsUrl = result.AssetsUrl,
    //        Author = result.Author,
    //        HtmlUrl = result.HtmlUrl,
    //        Name = result.Name,
    //        TarballUrl = result.TarballUrl,
    //        TargetCommitish = result.TargetCommitish,
    //        UploadUrl = result.UploadUrl,
    //        Url = result.Url,
    //        ZipballUrl = result.ZipballUrl,
    //        IsExistNewVersion = newVersionInfo > currentVersionInfo
    //    };
    //}

    //  [Obsolete("Obsolete")]
    //public static async Task<WinUICommunity.UpdateInfo> CheckUpdateAsync(string username, string repository, Version currentVersion = null!)
    //{
    //    if (string.IsNullOrEmpty(username))
    //    {
    //        throw new ArgumentNullException(nameof(username));
    //    }

    //    if (string.IsNullOrEmpty(repository))
    //    {
    //        throw new ArgumentNullException(nameof(repository));
    //    }

    //    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
    //    var client = new HttpClient();
    //    client.DefaultRequestHeaders.Add("User-Agent", username);
    //    var url = string.Format(GITHUB_API, username, repository);
    //    var response = await client.GetAsync(url);
    //    response.EnsureSuccessStatusCode();
    //    var responseBody = await response.Content.ReadAsStringAsync();
    //    var result = JsonSerializer.Deserialize<WinUICommunity.UpdateInfo>(responseBody);

    //    if (result != null)
    //    {
    //        if (currentVersion == null)
    //        {
    //            currentVersion = Assembly.GetEntryAssembly()!.GetName().Version;
    //        }

    //        var newVersionInfo = GetAsVersionInfo(result.TagName);
    //        var major = currentVersion!.Major == -1 ? 0 : currentVersion.Major;
    //        var minor = currentVersion.Minor == -1 ? 0 : currentVersion.Minor;
    //        var build = currentVersion.Build == -1 ? 0 : currentVersion.Build;
    //        var revision = currentVersion.Revision == -1 ? 0 : currentVersion.Revision;

    //        var currentVersionInfo = new WinUICommunity.SystemVersionInfo(major, minor, build, revision);

    //        return new WinUICommunity.UpdateInfo
    //        {
    //            Changelog = result?.Changelog,
    //            CreatedAt = Convert.ToDateTime(result?.CreatedAt),
    //            Assets = result?.Assets,
    //            IsPreRelease = result!.IsPreRelease,
    //            PublishedAt = Convert.ToDateTime(result?.PublishedAt),
    //            TagName = result?.TagName,
    //            AssetsUrl = result?.AssetsUrl,
    //            Author = result?.Author,
    //            HtmlUrl = result?.HtmlUrl,
    //            Name = result?.Name,
    //            TarballUrl = result?.TarballUrl,
    //            TargetCommitish = result?.TargetCommitish,
    //            UploadUrl = result?.UploadUrl,
    //            Url = result?.Url,
    //            ZipballUrl = result?.ZipballUrl,
    //            IsExistNewVersion = newVersionInfo > currentVersionInfo
    //        };
    //    }

    //    return null!;
    //}

    //private static WinUICommunity.SystemVersionInfo GetAsVersionInfo(string version)
    //{
    //    var nums = GetVersionNumbers(version).Split('.').Select(int.Parse).ToList();

    //    if (nums.Count <= 1)
    //    {
    //        return new WinUICommunity.SystemVersionInfo(nums[0], 0, 0, 0);
    //    }
    //    else if (nums.Count <= 2)
    //    {
    //        return new WinUICommunity.SystemVersionInfo(nums[0], nums[1], 0, 0);
    //    }
    //    else if (nums.Count <= 3)
    //    {
    //        return new WinUICommunity.SystemVersionInfo(nums[0], nums[1], nums[2], 0);
    //    }
    //    else
    //    {
    //        return new WinUICommunity.SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
    //    }
    //}

    private static string GetVersionNumbers(string version)
    {
        const string allowedChars = "01234567890.";
        return new string(version!.Where(c => allowedChars.Contains(c)).ToArray());
    }
}
