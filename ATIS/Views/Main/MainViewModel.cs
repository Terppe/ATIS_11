
using System.Configuration;
using System.Net.Sockets;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using Microsoft.Data.SqlClient;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace ATIS.Views.Main;
public partial class MainViewModel : ObservableObject, ITitleBarAutoSuggestBoxAware
{
    private readonly IDataService _dataService;

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    public async Task InitAsync()
    {
        IsLoading = true;

        await CheckSql_DbAsync(); // Asynchrone Initialisierung!
        // await CloseAppAsync();    // Falls gewünscht
        // Logging etc.
        SimpleLog.Info("MainWindow logging started.");
        IsLoading = false;
    }

    private Task CheckSql_DbAsync()
    {
        if (!CheckForServer(ConfigurationManager.AppSettings.Get("ServerName"),
                Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"))))
        {
            //           TbDataBase.Text = StringsRes.SqlServerNotRunning;
        }
        else
        {
            if (!CheckDatabaseExist(ConfigurationManager.AppSettings.Get("ServerName")))
            {
                //              TbDataBase.Text = StringsRes.DatabaseNotFound;
                ;
            }
        }

        return Task.CompletedTask;
    }


    //------------------------Check Server and Database-------------------------------
    private static bool CheckForServer(string address, int port)
    {
        var timeout = 500;
        if (ConfigurationManager.AppSettings.Get("RemoteTestTimeout") != null)
        {
            timeout = int.Parse(ConfigurationManager.AppSettings.Get("RemoteTestTimeout") ?? string.Empty);
        }

        try
        {
            var result = false;
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (address != null)
            {
                var asyncResult = socket.BeginConnect(address, port, null, null);
                result = asyncResult.AsyncWaitHandle.WaitOne(timeout, true);
            }

            socket.Close();

            return result;
        }
        catch (SqlException e)
        {
            _ = AllDialogs.ErrorMessageDialogAsync(e.ToString());

            _ = AllDialogs.NoServerConnectWarnMessageDialogAsync();
            //MessageBox.Show(CultRes.StringsRes.NoConnectServer, e.Message,
            //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            SimpleLog.Error("Server not connected" + e.Message);

            return false;
        }
    }

    //Check if database exist or SQLServer not running
    private bool CheckDatabaseExist(string db)
    {
        var ret = false;

        try
        {
            using var context = new AtisDbContext();
            var dbExists = context.Database.CanConnect();

            if (!dbExists)
            {
                // MessageBox.Show(CultRes.StringsRes.NoConnectServer + " " + db, $"CultRes.StringsRes.NoConnectServer: {db}");
                if (db != null)
                {
                    _ = AllDialogs.WarningNoDatabaseWithSqlServerSelectedMessageDialogAsync(db);
                }

                //MessageBox.Show($"Database does not exist", "SQL-Server ok " + db,
                //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (true)
                {
                    ret = false;
                }

                SimpleLog.Error("Database does not exist - SQL - Server ok");
            }
            else
            {
 //               TbDataBase.Foreground = new SolidColorBrush(Colors.Green);
                ret = true;
            }
        }
        catch (SqlException e)
        {
            _ = AllDialogs.ErrorMessageDialogAsync(e.ToString());

            _ = AllDialogs.NoServerConnectWarnMessageDialogAsync();
            //MessageBox.Show($"CultRes.StringsRes.NoConnectServer: {db}", e.Message,
            //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

            SimpleLog.Error($"Server not connected: {db}");
            ret = false;
        }

        return ret;
    }

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {

    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }

}
