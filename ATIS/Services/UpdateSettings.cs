using System.Text.Json;

namespace ATIS.Services;

public static class UpdateSettings
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ATIS",
        "updates.json");

    public static bool Load()
    {
        return !File.Exists(FilePath) ||
               JsonSerializer.Deserialize<bool>(File.ReadAllText(FilePath));
    }

    public static void Save(bool enabled)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(enabled));
    }
}
