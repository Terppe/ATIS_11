using Microsoft.EntityFrameworkCore;

namespace ATIS.Helpers;

public static class EfMetadataHelper
{
    public static int GetMaxLength(DbContext db, Type entityType, string propertyName)
    {
        var entity = db.Model.FindEntityType(entityType);
        var prop = entity?.FindProperty(propertyName);
        return prop?.GetMaxLength() ?? 50;
    }
}
