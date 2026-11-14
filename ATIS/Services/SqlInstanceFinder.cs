using ATIS.Models;

namespace ATIS.Services;


public static class SqlInstanceFinder
{
    // Liefert lokale Instanzen als Data Source, z. B. "localhost" oder "localhost\SQLEXPRESS"

    public static List<string> FindLocalInstances()
    {
        return DbServerSettings.GetInstalledDataSources().ToList();
    }

    public static async Task<List<string>> FindReachableAsync(string database)
    {
        var reachable = new List<string>();

        foreach (var source in FindLocalInstances())
        {
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = source,
                InitialCatalog = database,
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                ConnectTimeout = 10
            };

            try
            {
                await using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
                await connection.OpenAsync();
                reachable.Add(source);
            }
            catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException)
            {
                SimpleLog.Error($"{source} has no reachable database {database}: {ex.Message}");
            }
        }

        return reachable;
    }

    public static async Task<bool> CanConnectAsync(string dataSource, string database)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
        {
            DataSource = dataSource,
            InitialCatalog = database,
            IntegratedSecurity = true,
            TrustServerCertificate = true,
            ConnectTimeout = 10
        };

        try
        {
            await using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException)
        {
            SimpleLog.Error($"{dataSource}: {ex.Message}");
            return false;
        }
    }
}
