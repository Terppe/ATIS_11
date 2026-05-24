using System.Configuration;
using System.Net.Sockets;
using ATIS.Core.Taxonomy;
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
using ATIS.Views.Modules.Tbl90Reference;
using ATIS.Views.Modules.Tbl93Comment;
using Microsoft.Data.SqlClient;

namespace ATIS.Views.Main;
public partial class MainViewModel : ObservableObject, ITitleBarAutoSuggestBoxAware
{
    protected readonly IDataService _dataService;
    protected readonly IDialogService _dialogService;
    protected readonly LookupService _lookupService;

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

    public Tbl90ReferenceViewModel ReferenceVM { get; }
    public Tbl93CommentViewModel CommentVM { get; }

    public Tbl78NameViewModel NameVM { get; }
    public Tbl75SpeciesViewModel SpeciesVM { get; }
    public Tbl66GenusViewModel GenusVM { get; }
    public Tbl63InfratribusViewModel InfratribusVM { get; }
    public Tbl60SubtribusViewModel SubtribusVM { get; }
    public Tbl57TribusViewModel TribusVM { get; }
    public Tbl54SupertribusViewModel SupertribusVM { get; }

    public MainViewModel(IDataService dataService, IDialogService dialogService, LookupService lookupService)
    {
        _dataService = dataService;
        _dialogService = dialogService;
        _lookupService = lookupService;

        // Basis-VMs
        ReferenceVM = new Tbl90ReferenceViewModel(
            _dataService,
            _dialogService,
            _lookupService,
            new HierarchyCoordinator());

        CommentVM = new Tbl93CommentViewModel(
            _dataService, 
            _dialogService,
            _lookupService,
            new HierarchyCoordinator());

        // 1. Species
        SpeciesVM = new Tbl75SpeciesViewModel(
            _dataService,
            _dialogService,
            NameVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // 1. Genus
        GenusVM = new Tbl66GenusViewModel(
            _dataService,
            _dialogService,
            SpeciesVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // 2. Infratribus
        InfratribusVM = new Tbl63InfratribusViewModel(
            _dataService,
            _dialogService,
            _lookupService,
            GenusVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // 3. Subtribus
        SubtribusVM = new Tbl60SubtribusViewModel(
            _dataService,
            _dialogService,
            _lookupService,
            InfratribusVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // 4. Tribus
        TribusVM = new Tbl57TribusViewModel(
            _dataService,
            _dialogService,
            _lookupService,
            SubtribusVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // 5. Supertribus
        SupertribusVM = new Tbl54SupertribusViewModel(
            _dataService,
            _dialogService,
            _lookupService,
            TribusVM,
            ReferenceVM,
            CommentVM,
            new HierarchyCoordinator()
        );

        // Taxonomie-Aufbau
        TaxonomyBuilder.Build(
            SupertribusVM,
            TribusVM,
            SubtribusVM,
            InfratribusVM,
            GenusVM
        );
        // -----------------------------
        // Taxonomie einmal aufbauen
        // -----------------------------
        //TaxonomyBuilder.Build(
        //    RegnumVM,
        //    PhylumVM,
        //    SubphylumVM,
        //    SuperclassVM,
        //    ClassVM,
        //    SubclassVM,
        //    InfraclassVM,
        //    LegioVM,
        //    OrdoVM,
        //    SubordoVM,
        //    InfraordoVM,
        //    SuperfamilyVM,
        //    FamilyVM,
        //    SubfamilyVM,
        //    InfrafamilyVM,
        //    SupertribusVM,
        //    TribusVM,
        //    SubtribusVM,
        //    InfratribusVM,
        //    GenusVM,
        //    SpeciesVM
        //);
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
                    _dialogService.ShowErrorAsync("Server_Connect_Title", "Server_Connect_Message");
          //          _ = AllDialogs.WarningNoDatabaseWithSqlServerSelectedMessageDialogAsync(db);
                }
                _dialogService.ShowYesNoAsync("Server_Exist_Title", "Server_Exist_Question");
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
