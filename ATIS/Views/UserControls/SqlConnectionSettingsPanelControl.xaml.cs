using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ATIS.Views.UserControls;
public sealed partial class SqlConnectionSettingsPanelControl : UserControl, INotifyPropertyChanged
{
    private bool _isFetchingDatabases;
    private bool _isConnecting;

    private List<string> _databases = new();

    private readonly SqlConnectionStringBuilder _builder = new();

    public event PropertyChangedEventHandler PropertyChanged;

    public SqlConnectionSettingsPanelControl()
    {
        InitializeComponent();
    }
    //Ziel Data Source=.\SQLEXPRESS;Initial Catalog=ATIS34;Integrated Security=True;TrustServerCertificate=True
    //aus App.config und dort speichern

    /// <summary>
    /// Gets or sets a value indicating whether this instance is busy getting the list of databases.
    /// </summary>
    public bool IsFetchingDatabases
    {
        get { return _isFetchingDatabases; }
        set
        {
            _isFetchingDatabases = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is busy connecting.
    /// </summary>
    public bool IsConnecting
    {
        get { return _isConnecting; }
        set
        {
            _isConnecting = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current server name.
    /// </summary>
    public string Server
    {
        get { return _builder.DataSource; }
        set
        {
            _builder.DataSource = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public string UserId
    {
        get { return _builder.UserID; }
        set
        {
            _builder.UserID = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the user password.
    /// </summary>
    public string Password
    {
        get { return _builder.Password; }
        set
        {
            _builder.Password = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the list of databases.
    /// </summary>
    public List<string> Databases
    {
        get { return _databases; }
        set
        {
            _databases = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current database.
    /// </summary>
    public string Database
    {
        get { return _builder.InitialCatalog; }
        set
        {
            _builder.InitialCatalog = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current connection string.
    /// </summary>
    public string ConnectionString
    {
        get { return _builder.ConnectionString; }
        set
        {
            _builder.ConnectionString = value;
            CboAuthenticationProtocols.SelectedIndex = (int)_builder.Authentication;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged()
    {
        // Broadcast all properties.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    private IEnumerable<string> AuthenticationProtocols
    {
        get
        {
            foreach (string str in Enum.GetNames(typeof(SqlAuthenticationMethod)))
            {
                yield return str.SplitCamelCase();
            }
        }
    }

    private async void DatabaseComboBox_DropDownOpened(object sender, object e)
    {
        await RefreshDatabaseList();
    }

    private void ClearDatabaseList()
    {
        var databases = new List<string>();

        if (!string.IsNullOrEmpty(Database))
        {
            databases.Add(Database);
        }

        Databases = databases;
    }

    private async Task RefreshDatabaseList()
    {
        if (string.IsNullOrWhiteSpace(Server))
        {
            return;
        }

        IsFetchingDatabases = true;

        try
        {
            // Remove initial catalog
            var connector = new SqlConnectionStringBuilder(_builder.ConnectionString);
            connector.InitialCatalog = String.Empty;

            var databases = new List<string>();

            await using (var connection = new SqlConnection(connector.ConnectionString))
            {
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT [name] FROM sys.databases ORDER BY [name]";

                await using SqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    databases.Add(reader.GetString(0));
                }
            }

            if (!string.IsNullOrEmpty(Database) || !databases.Contains(Database))
            {
                databases.Insert(0, Database);
            }

            Databases = databases;
        }
        catch (Exception)
        {
            ClearDatabaseList();
        }
        finally
        {
            IsFetchingDatabases = false;
        }
    }

    private void Database_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Any())
        {
            Database = e.AddedItems.First().ToString();
        }
    }

    private async void ConnectionButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        IsConnecting = true;
        try
        {
            using (var connection = new SqlConnection(_builder.ConnectionString))
            {
                await connection.OpenAsync();
            }

            RedIcon.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            GreenIcon.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            GreenIcon.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            RedIcon.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        finally
        {
            IsConnecting = false;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException("in App.config speichern");
    }

    private void ConnectionFlyout_Opened(object sender, object e)
    {
        RedIcon.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        GreenIcon.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    }

    private void Authentication_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _builder.Authentication = (SqlAuthenticationMethod)(sender as ComboBox).SelectedIndex;

        // You may not set both !!!
        // builder.IntegratedSecurity = builder.Authentication == SqlAuthenticationMethod.ActiveDirectoryIntegrated;

        // Process UserId and Password boxes.
        // Based on the descriptions and sample connection strings here:
        // https://docs.microsoft.com/en-us/sql/connect/ado-net/sql/azure-active-directory-authentication?view=sql-server-ver16
        // Not all scenarios could be tested.
        switch (_builder.Authentication)
        {
            case SqlAuthenticationMethod.ActiveDirectoryIntegrated:
                ActivateUserId(); // User Id is optional.
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryInteractive:
                ActivateUserId(); // User Id is optional.
                DeactivatePassword(); // Password provided via authentication prompt.
                break;
            case SqlAuthenticationMethod.ActiveDirectoryPassword:
                ActivateUserId();
                ActivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryServicePrincipal:
                ActivateUserId();
                ActivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow:
                DeactivateUserId();
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryManagedIdentity:
                ActivateUserId();
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryMSI:
                ActivateUserId();
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.ActiveDirectoryDefault:
                DeactivateUserId();
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.NotSpecified:
                DeactivateUserId();
                DeactivatePassword();
                break;
            case SqlAuthenticationMethod.SqlPassword:
                ActivateUserId();
                ActivatePassword();
                break;
        }
    }

    private void ActivateUserId()
    {
        CrdUserId.IsEnabled = true;
    }

    private void DeactivateUserId()
    {
        UserId = string.Empty;
        _builder.Remove(nameof(UserId));
        CrdUserId.IsEnabled = false;
    }

    private void ActivatePassword()
    {
        CrdPassword.IsEnabled = true;
    }

    private void DeactivatePassword()
    {
        Password = string.Empty;
        _builder.Remove(nameof(Password));
        CrdPassword.IsEnabled = false;
    }

    private void Database_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        throw new NotImplementedException();
    }
}

internal static class StringExtensions
{
    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }
}
