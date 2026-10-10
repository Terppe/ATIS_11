using System.Configuration;
using System.Net.Sockets;
using ATIS.Helpers;
using ATIS.Interfaces;
using ATIS.Models;
using ATIS.Services;
using ATIS.Strings;
using ATIS.Views.Modules.Hierarchy;
using ATIS.Views.Modules.Tbl03Regnum;
using ATIS.Views.Modules.Tbl06Phylum;
using ATIS.Views.Modules.Tbl12Subphylum;
using ATIS.Views.Modules.Tbl18Superclass;
using ATIS.Views.Modules.Tbl21Class;
using ATIS.Views.Modules.Tbl24Subclass;
using ATIS.Views.Modules.Tbl27Infraclass;
using ATIS.Views.Modules.Tbl30Legio;
using ATIS.Views.Modules.Tbl33Ordo;
using ATIS.Views.Modules.Tbl36Subordo;
using ATIS.Views.Modules.Tbl39Infraordo;
using ATIS.Views.Modules.Tbl42Superfamily;
using ATIS.Views.Modules.Tbl45Family;
using ATIS.Views.Modules.Tbl48Subfamily;
using ATIS.Views.Modules.Tbl51Infrafamily;
using ATIS.Views.Modules.Tbl54Supertribus;
using ATIS.Views.Modules.Tbl57Tribus;
using ATIS.Views.Modules.Tbl60Subtribus;
using ATIS.Views.Modules.Tbl63Infratribus;
using ATIS.Views.Modules.Tbl66Genus;
using ATIS.Views.Modules.Tbl75Species;
using ATIS.Views.Modules.Tbl78Name;
using ATIS.Views.Modules.Tbl81Image;
using ATIS.Views.Modules.Tbl84Synonym;
using ATIS.Views.Modules.Tbl87Geographic;
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using WinUI3Localizer;

namespace ATIS.Views.Main;

public partial class MainViewModel : ObservableObject, ITitleBarAutoSuggestBoxAware
{
    private readonly IDialogService _dialogService;
    private readonly LookupService _lookupService;
    private readonly HierarchyCoordinator _coordinator;

    public string DbExist_SQLActiv { get; set; }
    // -----------------------------
    // Taxonomie-ViewModels
    // -----------------------------
    public Tbl03RegnumViewModel RegnumVM { get; }
    public Tbl06PhylumViewModel PhylumVM { get; }
    public Tbl12SubphylumViewModel SubphylumVM { get; }
    public Tbl18SuperclassViewModel SuperclassVM { get; }
    public Tbl21ClassViewModel ClassVM { get; }
    public Tbl24SubclassViewModel SubclassVM { get; }
    public Tbl27InfraclassViewModel InfraclassVM { get; }
    public Tbl30LegioViewModel LegioVM { get; }
    public Tbl33OrdoViewModel OrdoVM { get; }
    public Tbl36SubordoViewModel SubordoVM { get; }
    public Tbl39InfraordoViewModel InfraordoVM { get; }
    public Tbl42SuperfamilyViewModel SuperfamilyVM { get; }
    public Tbl45FamilyViewModel FamilyVM { get; }
    public Tbl48SubfamilyViewModel SubfamilyVM { get; }
    public Tbl51InfrafamilyViewModel InfrafamilyVM { get; }
    public Tbl54SupertribusViewModel SupertribusVM { get; }
    public Tbl57TribusViewModel TribusVM { get; }
    public Tbl60SubtribusViewModel SubtribusVM { get; }
    public Tbl63InfratribusViewModel InfratribusVM { get; }
    public Tbl66GenusViewModel GenusVM { get; }
    public Tbl75SpeciesViewModel SpeciesVM { get; }

    public Tbl78NameViewModel NameVM { get; }
    public Tbl81ImageViewModel ImageVM { get; }
    public Tbl84SynonymViewModel SynonymVM { get; }
    public Tbl87GeographicViewModel GeographicVM { get; }
    public Tbl90ReferenceViewModel ReferenceVM { get; }
    public Tbl93CommentViewModel CommentVM { get; }

    public MainViewModel(
        IDbContextFactory<AtisDbContext> factory,
        IDialogService dialogService,
        LookupService lookupService,
        HierarchyCoordinator coordinator,
        IUserService user)
    {
        _dialogService = dialogService;
        _lookupService = lookupService;
        _coordinator = coordinator;

        // HIER werden ALLE ViewModels korrekt erzeugt
        RegnumVM = new Tbl03RegnumViewModel(factory, dialogService, coordinator, lookupService, user);
        PhylumVM = new Tbl06PhylumViewModel(factory, dialogService, coordinator, lookupService, user);
        SubphylumVM = new Tbl12SubphylumViewModel(factory, dialogService, coordinator, lookupService, user);
        SuperclassVM = new Tbl18SuperclassViewModel(factory, dialogService, coordinator, lookupService, user);
        ClassVM = new Tbl21ClassViewModel(factory, dialogService, coordinator, lookupService, user);
        SubclassVM = new Tbl24SubclassViewModel(factory, dialogService, coordinator, lookupService, user);
        InfraclassVM = new Tbl27InfraclassViewModel(factory, dialogService, coordinator, lookupService, user);
        LegioVM = new Tbl30LegioViewModel(factory, dialogService, coordinator, lookupService, user);
        OrdoVM = new Tbl33OrdoViewModel(factory, dialogService, coordinator, lookupService, user);
        SubordoVM = new Tbl36SubordoViewModel(factory, dialogService, coordinator, lookupService, user);
        InfraordoVM = new Tbl39InfraordoViewModel(factory, dialogService, coordinator, lookupService, user);
        SuperfamilyVM = new Tbl42SuperfamilyViewModel(factory, dialogService, coordinator, lookupService, user);
        FamilyVM = new Tbl45FamilyViewModel(factory, dialogService, coordinator, lookupService, user);
        SubfamilyVM = new Tbl48SubfamilyViewModel(factory, dialogService, coordinator, lookupService, user);
        InfrafamilyVM = new Tbl51InfrafamilyViewModel(factory, dialogService, coordinator, lookupService, user);
        SupertribusVM = new Tbl54SupertribusViewModel(factory, dialogService, coordinator, lookupService, user);
        TribusVM = new Tbl57TribusViewModel(factory, dialogService, coordinator, lookupService, user);
        SubtribusVM = new Tbl60SubtribusViewModel(factory, dialogService, coordinator, lookupService, user);
        InfratribusVM = new Tbl63InfratribusViewModel(factory, dialogService, coordinator, lookupService, user);
        GenusVM = new Tbl66GenusViewModel(factory, dialogService, coordinator, lookupService, user);
        SpeciesVM = new Tbl75SpeciesViewModel(factory, dialogService, coordinator, lookupService, user);

        ReferenceVM = new Tbl90ReferenceViewModel(factory, dialogService, coordinator, lookupService, user);
        CommentVM = new Tbl93CommentViewModel(factory, dialogService, coordinator, lookupService, user);

        NameVM = new Tbl78NameViewModel(factory, dialogService, coordinator, lookupService, user);
        ImageVM = new Tbl81ImageViewModel(factory, dialogService, coordinator, lookupService, user);
        SynonymVM = new Tbl84SynonymViewModel(factory, dialogService, coordinator, lookupService, user);
        GeographicVM = new Tbl87GeographicViewModel(factory, dialogService, coordinator, lookupService, user);
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

        await CheckSql_DbAsync();
        // Logging etc.

        SimpleLog.Info("MainWindow logging started.");
        IsLoading = false;
    }


    public async Task RequestCloseAsync()
    {
        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Title = StringsRes.WantToCloseApp,
            PrimaryButtonText = StringsRes.Yes,
            CloseButtonText = StringsRes.No,
            DefaultButton = ContentDialogButton.Close
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            SimpleLog.Info(StringsRes.AppClosed);
            App.MainWindow.Close();
        }
    }

    private async Task CheckSql_DbAsync()
    {
        var serverName = ConfigurationManager.AppSettings.Get("ServerName");

        if (!int.TryParse(
                ConfigurationManager.AppSettings.Get("Port"),
                out var port))
        {
            SimpleLog.Error("Ungültiger Port in AppSettings.");

            return;
        }

        if (!CheckForServer(serverName, port))
        {
            return;
        }

        await CheckDatabaseExistAsync(serverName);
    }

    //private Task CheckSql_DbAsync()
    //{
    //    if (!CheckForServer(ConfigurationManager.AppSettings.Get("ServerName"),
    //            Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"))))
    //    {
    //        //           TbDataBase.Text = StringsRes.SqlServerNotRunning;
    //    }
    //    else
    //    {
    //        if (!CheckDatabaseExist(ConfigurationManager.AppSettings.Get("ServerName")))
    //        {
    //            //              TbDataBase.Text = StringsRes.DatabaseNotFound;
    //            ;
    //        }
    //    }

    //    return Task.CompletedTask;
    //}


    //------------------------Check Server and Database-------------------------------

    private async Task<bool> CheckDatabaseExistAsync(string? db)
    {
        try
        {
            await using var context = new AtisDbContext();

            var dbExists = await context.Database.CanConnectAsync();

            if (!dbExists)
            {
                if (!string.IsNullOrWhiteSpace(db))
                {
                    await _dialogService.ShowErrorAsync(
                        "Server_Connect_Title",
                        "Server_Connect_Message");
                }

                await _dialogService.ShowYesNoAsync(
                    "Server_Exist_Title",
                    "Server_Exist_Question");

                SimpleLog.Error($"Datenbank nicht erreichbar: {db}");

                DbExist_SQLActiv = $"Datenbank nicht erreichbar: {db}";

                return false;
            }
            //noch bearbeiten
            var loc1 = Localizer.Get();
            DbExist_SQLActiv = loc1.GetLocalizedString("SQL_Server_Ok");
        //    DbExist_SQLActiv = $"SQL-Server ok: {db}";

            return true;
        }
        catch (SqlException ex)
        {
            SimpleLog.Error($"SQL-Server nicht verbunden: {db} - {ex}");

            DbExist_SQLActiv = $"SQL-Server nicht verbunden: {db}";

            await _dialogService.ShowYesNoAsync(
                "Server_Exist_Title",
                "Server_Exist_Question");

            return false;
        }
    }

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

    // Check if database exist or SQLServer not running
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
                    _dialogService.ShowErrorAsync("Server_Connect_Title", "Server_Connect_Message");
                    //          _ = AllDialogs.WarningNoDatabaseWithSqlServerSelectedMessageDialogAsync(db);
                }
                _dialogService.ShowYesNoAsync("Server_Exist_Title", "Server_Exist_Question");
                //MessageBox.Show($"Database does not exist", "SQL-Server ok " + db,
                //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (true)
                {
                    DbExist_SQLActiv = $"Database does not exist - SQL - Server ok: {db}";

                    ret = false;
                }

                SimpleLog.Error("Database does not exist - SQL - Server ok");
            }
            else
            {
                DbExist_SQLActiv = $"SQL-Server ok: {db}";
               // TbDataBase.Foreground = new SolidColorBrush(Colors.Green);
                ret = true;
            }
        }
        catch (SqlException e)
        {
            _ = AllDialogs.ErrorMessageDialogAsync(e.ToString());

            //  _ = AllDialogs.NoServerConnectWarnMessageDialogAsync();
            //MessageBox.Show($"CultRes.StringsRes.NoConnectServer: {db}", e.Message,
            //    MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            _dialogService.ShowYesNoAsync("Server_Exist_Title", "Server_Exist_Question");

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
