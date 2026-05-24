// Views/Base/IHierarchicalEntity.cs
namespace ATIS.Views.Base;

/// <summary>
/// Marker Interface für Entities die in der Hierarchie Parent sein können
/// </summary>
public interface IHierarchicalEntity
{
    int GetEntityId();
    string GetDisplayName();
}
