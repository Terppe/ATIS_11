using System.Text.Json;

namespace ATIS.Services;

public static class DbServerSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ATIS", "dbserver.json");

    private sealed record Data(string? DataSource);

    // Gespeicherter Server, z. B. "WLAP02\SQLEXPRESS" oder "localhost,1433"; null = Standard aus App.config
    public static string? DataSource
    {
        get
        {
            try
            {
                if (!File.Exists(FilePath)) return null;
                var value = JsonSerializer.Deserialize<Data>(File.ReadAllText(FilePath))?.DataSource;
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                return null;
            }
        }
    }

    public static void Save(string? dataSource)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(new Data(dataSource)));
    }
}
