using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace ATIS.Services;

public static class DbServerSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ATIS", "dbserver.json");

    private sealed record Data(string? DataSource);

    // Gespeicherter Server, z. B. "WLAP02\SQLEXPRESS" oder "WLAP01" oder "localhost,1433"; null = Standard aus App.config
    public static string? DataSource
    {
        get
        {
            if (!File.Exists(FilePath))
            {
                return null; // Noch kein Server gespeichert oder dbserver.json nicht vorhanden.
            }

            var data = JsonSerializer.Deserialize<Data>(
                File.ReadAllText(FilePath));

            if (string.IsNullOrWhiteSpace(data?.DataSource))
            {
                throw new InvalidDataException(
                    $"Kein SQL-Server in {FilePath} gespeichert.");
            }

            return NormalizeDataSource(data.DataSource);
        }
    }

    public static void Save(string? dataSource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataSource);

        var server = NormalizeDataSource(dataSource);

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        File.WriteAllText(
            FilePath,
            JsonSerializer.Serialize(new Data(server)));
    }
    private const string SqlInstancesRegistryPath = @"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL";

    public static IReadOnlyList<string> GetInstalledDataSources()
    {
        var dataSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var machineName = Environment.MachineName;

        foreach (var registryView in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
            using var instancesKey = baseKey.OpenSubKey(SqlInstancesRegistryPath);

            if (instancesKey is null)
                continue;

            foreach (var instanceName in instancesKey.GetValueNames())
            {
                var dataSource = string.Equals(
                    instanceName, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                    ? machineName
                    : $@"{machineName}\{instanceName}";

                dataSources.Add(dataSource);
            }
        }

        return dataSources.OrderBy(server => server, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public static string NormalizeDataSource(string dataSource)
    {
        var parts = dataSource.Trim().Split('\\', 2);
        var host = parts[0];

        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            host.Equals("(local)", StringComparison.OrdinalIgnoreCase) ||
            host == ".")
        {
            host = Environment.MachineName;
        }

        return parts.Length == 2 ? $"{host}\\{parts[1]}" : host;
    }
    public static string GetConnectionString(string? selectedServer = null)
    {
        var server = selectedServer ?? DataSource;

        if (string.IsNullOrWhiteSpace(server))
        {
            throw new InvalidOperationException(
                "Bitte zuerst einen SQL-Server auswählen, testen und speichern.");
        }

        return new SqlConnectionStringBuilder
        {
            DataSource = NormalizeDataSource(server),
            InitialCatalog = "ATIS34",
            IntegratedSecurity = true,
            MultipleActiveResultSets = true,
            TrustServerCertificate = true
        }.ConnectionString;
    }
}
