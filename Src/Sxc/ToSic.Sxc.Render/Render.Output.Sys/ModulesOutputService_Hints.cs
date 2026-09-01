namespace ToSic.Sxc.Render.Output.Sys;

internal partial class ModulesOutputService
{
    /// <summary>
    /// Stores ModuleServiceData instances, scoped by ModuleId.
    /// </summary>
    private readonly Dictionary<int, List<ModuleHint>> _moduleHints = new();
    
    /// <inheritdoc/>
    public void AddHint(int moduleId, ModuleHint hint)
    {
        if (_moduleHints.TryGetValue(moduleId, out var hints))
            hints.Add(hint);
        else
            _moduleHints[moduleId] = [hint];
    }

    /// <inheritdoc/>
    public IList<ModuleHint> GetHintsAndFlush(int moduleId = default)
    {
        // If there is nothing to get, exit early
        if (!_moduleHints.TryGetValue(moduleId, out var hints))
            return [];

        // Reset module data to avoid duplicates on subsequent calls
        _moduleHints.Remove(moduleId);

        return hints;
    }

}
