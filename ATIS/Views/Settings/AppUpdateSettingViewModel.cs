using Windows.System;

namespace ATIS.Views.Settings;
public partial class AppUpdateSettingViewModel : ObservableObject
{
    //[ObservableProperty]
    //public string currentVersion;

    private string _currentVersion;
    public string CurrentVersion
    {
        get => _currentVersion;
        set => SetProperty(ref _currentVersion, value);
    }

    //[ObservableProperty]
    //public string lastUpdateCheck;

    private string _lastUpdateCheck;
    public string LastUpdateCheck
    {
        get => _lastUpdateCheck;
        set => SetProperty(ref _lastUpdateCheck, value);
    }

    //[ObservableProperty]
    //public bool isUpdateAvailable;

    private bool _isUpdateAvailable;
    public bool IsUpdateAvailable
    {
        get => _isUpdateAvailable;
        set => SetProperty(ref _isUpdateAvailable, value);
    }

    //[ObservableProperty]
    //public bool isLoading;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    //[ObservableProperty]
    //public bool isCheckButtonEnabled = true;

    private bool _isCheckButtonEnabled;
    public bool IsCheckButtonEnabled
    {
        get => _isCheckButtonEnabled;
        set => SetProperty(ref _isCheckButtonEnabled, value);
    }




    //[ObservableProperty]
    //public string loadingStatus = "Status";

    private string _loadingStatus;
    public string LoadingStatus
    {
        get => _loadingStatus;
        set => SetProperty(ref _loadingStatus, value);
    }

    private string ChangeLog = string.Empty;

    //public AppUpdateSettingViewModel()
    //{
    //    CurrentVersion = $"Current Version {ProcessInfoHelper.VersionWithPrefix}";
    //    LastUpdateCheck = AppHelper.Settings.LastUpdateCheck;
    //}

    //[RelayCommand]
    //private async Task CheckForUpdateAsync()
    //{
    //    IsLoading = true;
    //    IsUpdateAvailable = false;
    //    IsCheckButtonEnabled = false;
    //    LoadingStatus = "Checking for new version";
    //    if (NetworkHelper.IsNetworkAvailable())
    //    {
    //        try
    //        {
    //            //Todo: Fix UserName and Repo
    //            string username = "";
    //            string repo = "";
    //            LastUpdateCheck = DateTime.Now.ToShortDateString();
    //            AppHelper.Settings.LastUpdateCheck = DateTime.Now.ToShortDateString();
    //            var update = await UpdateHelper.CheckUpdateAsync(username, repo, new Version(ProcessInfoHelper.Version));
    //            if (update.IsExistNewVersion)
    //            {
    //                IsUpdateAvailable = true;
    //                ChangeLog = update.Changelog;
    //                LoadingStatus = $"We found a new version {update.TagName} Created at {update.CreatedAt} and Published at {update.PublishedAt}";
    //            }
    //            else
    //            {
    //                LoadingStatus = "You are using latest version";
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            LoadingStatus = ex.Message;
    //            IsLoading = false;
    //            IsCheckButtonEnabled = true;
    //        }
    //    }
    //    else
    //    {
    //        LoadingStatus = "Error Connection";
    //    }
    //    IsLoading = false;
    //    IsCheckButtonEnabled = true;
    //}

    //[RelayCommand]
    //private async Task GoToUpdateAsync()
    //{
    //    //Todo: Change Uri
    //    await Launcher.LaunchUriAsync(new Uri("https://github.com/WinUICommunity/WinUICommunity/releases"));
    //}

    //[RelayCommand]
    //private async Task GetReleaseNotesAsync()
    //{
    //    ContentDialog dialog = new ContentDialog()
    //    {
    //        Title = "Release Note",
    //        CloseButtonText = "Close",
    //        Content = new ScrollViewer
    //        {
    //            Content = new TextBlock
    //            {
    //                Text = ChangeLog,
    //                Margin = new Thickness(10)
    //            },
    //            Margin = new Thickness(10)
    //        },
    //        Margin = new Thickness(10),
    //        DefaultButton = ContentDialogButton.Close,
    //        XamlRoot = App.MainWindow.Content.XamlRoot
    //    };

    //    await dialog.ShowAsyncQueue();
    //}
}
