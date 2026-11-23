using ATIS.Models;

namespace ATIS.Services;

public static class OfflineTranslationFields
{
    public static async Task TranslateAsync(
        OfflineTranslationService translator,
        Tbl66Genus genus,
        IEnumerable<Tbl90Reference> experts,
        IEnumerable<Tbl90Reference> sources,
        IEnumerable<Tbl90Reference> authors,
        IEnumerable<Tbl93Comment> comments)
    {
        var fields = new List<(string Text, Action<string?> Set)>();

        Add(genus.Info, value => genus.TranslatedInfo = value);
        Add(genus.Memo, value => genus.TranslatedMemo = value);

        foreach (var reference in experts)
        {
            //Add(reference.Info, value => reference.TranslatedInfo = value);
            Add(reference.Memo, value => reference.TranslatedMemo = value);

            if (reference.Tbl90RefExperts is { } expert)
            {
                //Add(expert.Info, value => expert.TranslatedInfo = value);
                Add(expert.Memo, value => expert.TranslatedMemo = value);
            }
        }

        foreach (var reference in sources)
        {
            //Add(reference.Info, value => reference.TranslatedInfo = value);
            Add(reference.Memo, value => reference.TranslatedMemo = value);

            if (reference.Tbl90RefSources is { } source)
            {
                //Add(source.Info, value => source.TranslatedInfo = value);
                Add(source.Memo, value => source.TranslatedMemo = value);
            }
        }

        foreach (var reference in authors)
        {
            //Add(reference.Info, value => reference.TranslatedInfo = value);
            Add(reference.Memo, value => reference.TranslatedMemo = value);

            if (reference.Tbl90RefAuthors is { } author)
            {
                //Add(author.Info, value => author.TranslatedInfo = value);
                Add(author.Memo, value => author.TranslatedMemo = value);
            }
        }

        foreach (var comment in comments)
        {
            //Add(comment.Info, value => comment.TranslatedInfo = value);
            Add(comment.Memo, value => comment.TranslatedMemo = value);
        }

        if (fields.Count == 0)
            return;

        try
        {
            var translated = await Task.Run(
                () => translator.TranslateForDisplay(fields.Select(field => field.Text).ToArray()));

            for (var index = 0; index < fields.Count; index++)
                fields[index].Set(translated[index]);
        }
        catch (Exception exception)
        {
            SimpleLog.Error($"Offline-Übersetzung fehlgeschlagen: {exception.Message}");
            SimpleLog.Log(exception);
        }

        void Add(string? text, Action<string?> set)
        {
            if (!string.IsNullOrWhiteSpace(text))
                fields.Add((text, set));
        }
    }
}
