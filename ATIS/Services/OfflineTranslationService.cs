using BergamotTranslatorSharp;
using NTextCat;
using System.Globalization;

namespace ATIS.Services;

public sealed class OfflineTranslationService : IDisposable
{
    private const int MaxLoadedTranslators = 2;
    private const float MinimumLanguageScoreGap = 10;

    private readonly RankedLanguageIdentifier _languageIdentifier;
    private readonly Dictionary<string, (BlockingService Service, long LastUsed)> _translators = new();
    private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);
    private readonly object _sync = new();
    private long _useSequence;

    public OfflineTranslationService()
    {
        var profilePath = Path.Combine(
            AppContext.BaseDirectory,
            "TranslationModels",
            "languageid",
            "Core14.profile.xml");

        if (!File.Exists(profilePath))
            throw new FileNotFoundException(
                "Das Offline-Sprachprofil wurde nicht gefunden.",
                profilePath);

        _languageIdentifier = new RankedLanguageIdentifierFactory().Load(profilePath);
    }

    public IReadOnlyList<string> Translate(IReadOnlyList<string> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        lock (_sync)
        {
            var targetLanguage = GetTargetLanguage();
            return texts.Select(text => Translate(text, targetLanguage)).ToArray();
        }
    }

    private string Translate(string text, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var detectedLanguage = DetectLanguage(text);
        if (detectedLanguage is null || detectedLanguage == targetLanguage)
            return text;

        var cacheKey = $"{targetLanguage}\0{text}";
        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        var translation = detectedLanguage == "en" || targetLanguage == "en"
            ? TranslateWithModel($"{detectedLanguage}-{targetLanguage}", text)
            : TranslateWithEnglishPivot(detectedLanguage, targetLanguage, text);

        if (string.IsNullOrWhiteSpace(translation))
            throw new InvalidOperationException("Bergamot lieferte eine leere Übersetzung.");

        _cache.Add(cacheKey, translation);
        return translation;
    }

    private string TranslateWithEnglishPivot(
        string sourceLanguage,
        string targetLanguage,
        string text)
    {
        var englishText = TranslateWithModel($"{sourceLanguage}-en", text);
        return TranslateWithModel($"en-{targetLanguage}", englishText);
    }

    private string TranslateWithModel(string direction, string text)
    {
        var configPath = Path.Combine(
            AppContext.BaseDirectory,
            "TranslationModels",
            direction,
            "config.yml");

        if (!File.Exists(configPath))
            throw new FileNotFoundException(
                $"Die Offline-Übersetzung für {direction} wurde nicht gefunden.",
                configPath);

        if (!_translators.TryGetValue(direction, out var entry))
        {
            if (_translators.Count >= MaxLoadedTranslators)
            {
                var leastRecentlyUsed = _translators
                    .MinBy(item => item.Value.LastUsed);
                leastRecentlyUsed.Value.Service.Dispose();
                _translators.Remove(leastRecentlyUsed.Key);
            }

            entry = (new BlockingService(configPath), 0);
        }

        _translators[direction] = (entry.Service, ++_useSequence);
        return entry.Service.Translate(text);
    }

    private string? DetectLanguage(string text)
    {
        var matches = _languageIdentifier.Identify(text).Take(2).ToArray();

        // Short or ambiguous text is left unchanged rather than guessed.
        if (matches.Length < 2 ||
            matches[1].Item2 - matches[0].Item2 < MinimumLanguageScoreGap)
        {
            return null;
        }

        return matches[0].Item1.Iso639_3 switch
        {
            "deu" => "de",
            "eng" => "en",
            "fra" => "fr",
            "spa" => "es",
            _ => null
        };
    }

    private static string GetTargetLanguage()
    {
        if (string.IsNullOrWhiteSpace(App.Cult))
            throw new InvalidOperationException("Die ATIS-Anmeldesprache ist nicht gesetzt.");

        return CultureInfo.GetCultureInfo(App.Cult).TwoLetterISOLanguageName switch
        {
            "de" => "de",
            "en" => "en",
            "fr" => "fr",
            "es" => "es",
            var language => throw new NotSupportedException(
                $"Die Zielsprache '{language}' wird offline nicht unterstützt.")
        };
    }

    public IReadOnlyList<string> TranslateForDisplay(IReadOnlyList<string> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        lock (_sync)
        {
            var targetLanguage = GetTargetLanguage();

            var notice = targetLanguage switch
            {
                "de" => "Nicht übersetzt",
                "en" => "Not translated",
                "fr" => "Non traduit",
                "es" => "Sin traducir",
                _ => throw new NotSupportedException(targetLanguage)
            };

            return texts.Select(text =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return text;

                if (DetectLanguage(text) == null)
                    return $"{text} [{notice}]";

                return Translate(text, targetLanguage);
            }).ToArray();
        }
    }
    public void Dispose()
    {
        lock (_sync)
        {
            foreach (var translator in _translators.Values)
                translator.Service.Dispose();

            _translators.Clear();
        }
    }
}
