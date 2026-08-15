using System;
using System.Collections.Generic;
using System.Text;

namespace ATIS.Views.Taxonomy;

using System.Linq;

using System.Linq;

public static class TaxonomyDisplayFormatter
{
    public static string BuildPrefixedParts(string prefix, params string?[] parts)
    {
        var text = string.Join(" ",
            parts.Where(x => !string.IsNullOrWhiteSpace(x)));

        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : $"{prefix}{text}";
    }

    public static string BuildPrefixedList(string prefix, params string?[] parts)
    {
        var text = string.Join(", ",
            parts.Where(x => !string.IsNullOrWhiteSpace(x)));

        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : $"{prefix}{text}";
    }
}
