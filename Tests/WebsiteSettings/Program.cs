using System.Text.Json;
using ATIS.Services;

var root = Path.Combine(Path.GetTempPath(), $"atis-websites-tests-{Guid.NewGuid():N}");
var target = Path.Combine(root, "user", "websites.json");
var defaults = Path.Combine(root, "defaults.json");
Directory.CreateDirectory(root);

try
{
    File.WriteAllText(defaults, """{"Website.A":"https://example.org/a","Other":"ignored"}""");
    var first = await WebsiteSettings.LoadAsync(target, defaults);
    Check(File.Exists(target), "First load must create the user file.");
    Check(first.Links.Count == 1 && first.Links[0].Name == "A", "First load must show default links.");

    var edited = """
    {
      "website.My link": "https://example.org/new",
      "Website.HTTP": "http://example.org/",
      "Other": "https://ignored.example/",
      "Website.Wrong scheme": "file:///test",
      "Website.Relative": "/relative",
      "Website.": "https://example.org/",
      "Website.Number": 123,
      "Website.Null": null
    }
    """;
    File.WriteAllText(target, edited);
    File.WriteAllText(defaults, """{"Website.New default":"https://example.com/"}""");
    var second = await WebsiteSettings.LoadAsync(target, defaults);
    Check(File.ReadAllText(target) == edited, "User file must never be overwritten.");
    Check(second.Links.Select(x => x.Name).SequenceEqual(new[] { "My link", "HTTP" }), "Prefix and valid schemes.");
    Check(second.InvalidEntries.SequenceEqual(new[] { "Website.Wrong scheme", "Website.Relative", "Website.", "Website.Number", "Website.Null" }), "Each invalid entry must be reported.");

    File.WriteAllText(target, "{broken");
    await ExpectJsonExceptionAsync(target, defaults);
    Check(File.ReadAllText(target) == "{broken", "Invalid file must not be overwritten.");
    File.WriteAllText(target, "null");
    await ExpectJsonExceptionAsync(target, defaults);

    File.Delete(target);
    try
    {
        await WebsiteSettings.LoadAsync(target, Path.Combine(root, "missing.json"));
        throw new Exception("Missing default must be reported.");
    }
    catch (FileNotFoundException) { }
    Check(!File.Exists(target), "Missing default must not create user file.");

    var runs = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => WebsiteSettings.LoadAsync(target, defaults)));
    Check(runs.All(x => x.Links.Count == 1 && x.Links[0].Name == "New default"), "Concurrent first launches must load complete settings.");
    Check(!Directory.EnumerateFiles(Path.GetDirectoryName(target)!, "*.tmp").Any(), "Temporary files must be cleaned up.");
    Console.WriteLine("All website settings checks passed.");
}
finally
{
    Directory.Delete(root, recursive: true);
}

static void Check(bool condition, string message)
{
    if (!condition)
        throw new Exception(message);
}

static async Task ExpectJsonExceptionAsync(string target, string defaults)
{
    try
    {
        await WebsiteSettings.LoadAsync(target, defaults);
        throw new Exception("Invalid JSON should fail.");
    }
    catch (JsonException) { }
}
