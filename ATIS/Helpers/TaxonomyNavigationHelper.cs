using System;
using System.Collections.Generic;
using System.Text;
using ATIS.Views.Search;

namespace ATIS.Helpers;

public static class TaxonomyNavigationHelper
{
    public static async Task NavigateFromTextBlockAsync(
        TextBlock? tb,
        string entityType,
        Func<BreadcrumbItem, Task> navigateBreadcrumbAsync)
    {
        if (tb is null) return;
        if (string.IsNullOrWhiteSpace(entityType)) return;
        if (navigateBreadcrumbAsync is null) return;

        // Id robust aus Tag lesen (int / long / string)
        if (!TryGetIntId(tb.Tag, out var entityId)) return;

        var item = new BreadcrumbItem
        {
            Label = tb.Text ?? string.Empty,
            EntityType = entityType,
            EntityId = entityId
        };

        await navigateBreadcrumbAsync(item);
    }

    private static bool TryGetIntId(object? value, out int id)
    {
        switch (value)
        {
            case int i:
                id = i;
                return true;
            case long l when l <= int.MaxValue && l >= int.MinValue:
                id = (int)l;
                return true;
            case string s when int.TryParse(s, out var parsed):
                id = parsed;
                return true;
            default:
                id = 0;
                return false;
        }
    }
}
