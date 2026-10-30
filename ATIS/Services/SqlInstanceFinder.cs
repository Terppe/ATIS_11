using ATIS.Models;
using Microsoft.Win32;

namespace ATIS.Services;


public static class SqlInstanceFinder
{
    // Liefert lokale Instanzen als Data Source, z. B. "localhost" oder "localhost\SQLEXPRESS"
    public static List<string> FindLocalInstances()
    {
        var result = new List<string>();

        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var key = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");

            if (key == null)
            {
                return result;
            }

            foreach (var name in key.GetValueNames())
            {
                result.Add(name.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                    ? "localhost"
                    : $@"localhost\{name}");
            }
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException)
        {
            SimpleLog.Error($"SQL-Instanzsuche fehlgeschlagen: {ex.Message}");
        }

        return result;
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
                ConnectTimeout = 3
            };

            try
            {
                await using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
                await connection.OpenAsync();
                reachable.Add(source);
            }
            catch (Exception ex) when (ex is Microsoft.Data.SqlClient.SqlException or InvalidOperationException)
            {
                SimpleLog.Error($"{source} hat keine erreichbare Datenbank {database}: {ex.Message}");
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
            ConnectTimeout = 3
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
