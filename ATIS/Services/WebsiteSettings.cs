using System.Text.Json;

namespace ATIS.Services;

public static class WebsiteSettings
{
    public static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ATIS",
        "websites.json");

    public static string DefaultFilePath => Path.Combine(
        AppContext.BaseDirectory, "Defaults", "websites.json");

    public sealed record Link(string Name, Uri Uri);
    public sealed record Result(IReadOnlyList<Link> Links, IReadOnlyList<string> InvalidEntries);

    public static async Task<Result> LoadAsync(string filePath, string defaultFilePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("The website settings path must include a directory.", nameof(filePath));

        Directory.CreateDirectory(directory);

        if (!File.Exists(filePath))
        {
            var tempPath = Path.Combine(directory, $".websites-{Guid.NewGuid():N}.tmp");
            try
            {
                File.Copy(defaultFilePath, tempPath, overwrite: false);
                try
                {
                    File.Move(tempPath, filePath, overwrite: false);
                }
                catch (IOException) when (File.Exists(filePath))
                {
                    // Another process created the file first; never replace it.
                }
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }

        using var stream = File.OpenRead(filePath);
        using var document = await JsonDocument.ParseAsync(stream);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new JsonException("The website settings must be a JSON object.");

        var links = new List<Link>();
        var invalidEntries = new List<string>();
        foreach (var entry in document.RootElement.EnumerateObject())
        {
            const string prefix = "Website.";
            if (!entry.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                continue;

            var name = entry.Name[prefix.Length..].Trim();
            if (string.IsNullOrWhiteSpace(name) ||
                entry.Value.ValueKind != JsonValueKind.String ||
                !Uri.TryCreate(entry.Value.GetString(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                invalidEntries.Add(entry.Name);
                continue;
            }

            links.Add(new Link(name, uri));
        }

        return new Result(links, invalidEntries);
    }
}
