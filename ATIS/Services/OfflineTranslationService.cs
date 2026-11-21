using BergamotTranslatorSharp;

namespace ATIS.Services;

public sealed class OfflineTranslationService : IDisposable
{
    private readonly BlockingService _translator;
    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);
    private readonly object _sync = new();

    public OfflineTranslationService()
    {
        var configPath = Path.Combine(
            AppContext.BaseDirectory,
            "TranslationModels",
            "en-de",
            "config.yml");

        if (!File.Exists(configPath))
            throw new FileNotFoundException(
                "Die Konfiguration des EN-DE-Modells wurde nicht gefunden.",
                configPath);

        _translator = new BlockingService(configPath);
    }

    public IReadOnlyList<string> Translate(IReadOnlyList<string> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        lock (_sync)
        {
            var missing = texts
                .Distinct(StringComparer.Ordinal)
                .Where(text => !_cache.ContainsKey(text))
                .ToArray();

            if (missing.Length > 0)
            {
                var translated = _translator.Translate(missing).ToArray();

                if (translated.Length != missing.Length ||
                    translated.Any(string.IsNullOrWhiteSpace))
                {
                    throw new InvalidOperationException(
                        "Bergamot lieferte ein ungültiges Übersetzungsergebnis.");
                }

                for (var i = 0; i < missing.Length; i++)
                    _cache.Add(missing[i], translated[i]);
            }

            return texts.Select(text => _cache[text]).ToArray();
        }
    }

    public void Dispose() => _translator.Dispose();
}
