namespace ATIS.Views.Modules.Hierarchy;

/// <summary>
/// Zentrale Koordination für beliebig tiefe Hierarchien
/// Alle Level-Änderungen gehen durch diesen Coordinator
/// → Verhindert State-Chaos und Race Conditions
/// </summary>
public class HierarchyCoordinator : ObservableObject
{
    // ============================================
    // STATE: Dictionary für beliebige Tiefe
    // ============================================
    private readonly Dictionary<HierarchyLevel, int?> _selectedIds = new();

    // ============================================
    // EVENT: Bei jeder Level-Änderung
    // ============================================
    public event EventHandler<HierarchyLevelChangedArgs>? LevelChanged;

    // ============================================
    // PUBLIC API
    // ============================================

    /// <summary>
    /// Setzt die ID für eine Hierarchie-Ebene
    /// Triggert automatisch Cascade-Reset von Ebenen darunter
    /// </summary>
    public void SetSelectedId(HierarchyLevel level, int? id)
    {
        if (_selectedIds.TryGetValue(level, out var current) && current == id)
            return; // Keine Änderung

        _selectedIds[level] = id;

        // CASCADE: Alle Ebenen DARUNTER werden null gesetzt
        var resetFrom = (int)level + 1;
        foreach (var kvp in _selectedIds.ToList())
        {
            if ((int)kvp.Key > resetFrom)
                _selectedIds[kvp.Key] = null;
        }

        // Event triggern
        LevelChanged?.Invoke(this, new HierarchyLevelChangedArgs
        {
            Level = level,
            SelectedId = id
        });
    }

    /// <summary>
    /// Holt die ID für eine Hierarchie-Ebene
    /// </summary>
    public int? GetSelectedId(HierarchyLevel level)
        => _selectedIds.TryGetValue(level, out var id) ? id : null;

    /// <summary>
    /// Setzt eine Ebene UND alle darunter auf null
    /// </summary>
    public void ResetFrom(HierarchyLevel level)
    {
        var resetFrom = (int)level;
        foreach (var kvp in _selectedIds.ToList())
        {
            if ((int)kvp.Key >= resetFrom)
                _selectedIds[kvp.Key] = null;
        }

        LevelChanged?.Invoke(this, new HierarchyLevelChangedArgs
        {
            Level = level,
            SelectedId = null
        });
    }

    /// <summary>
    /// Setzt ALLES zurück
    /// </summary>
    public void ResetAll()
    {
        _selectedIds.Clear();
    }

    /// <summary>
    /// Debug-Info
    /// </summary>
    public override string ToString()
    {
        var items = string.Join(", ", _selectedIds
            .Where(x => x.Value.HasValue)
            .Select(x => $"{x.Key}={x.Value}"));
        return $"Hierarchy: [{items}]";
    }
}
