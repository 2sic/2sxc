using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Render.Output.Sys;

internal partial class ModulesOutputService
{
    #region Output Caching

    /// <summary>
    /// Stores caching settings per module.
    /// </summary>
    private readonly Dictionary<int, ModuleOutputCacheSettings> _moduleOutputCacheSpecs = new();

    /// <summary>
    /// Temporary per-module buffer for output-cache configuration collected during a single render.
    /// It exists because OutputCache.Configure(...) and OutputCache.DependOn(...) may happen at different times,
    /// but the final merged settings are only needed when the render result is finalized.
    /// </summary>
    private sealed class ModuleOutputCacheSettings
    {
        // Last explicit OutputCache.Configure(...) settings captured for this module render.
        public OutputCacheSettings? Settings { get; set; }

        // Union of all DependOn(...) keys collected while the module is rendering.
        public HashSet<string> ExternalDependencyKeys { get; } = new(StringComparer.OrdinalIgnoreCase);
    }


    /// <inheritdoc/>
    public void ConfigureOutputCache(int moduleId, OutputCacheSettings settings)
    {
        var cacheState = GetOrCreateOutputCacheData(moduleId);
        // Dependencies are accumulated separately because multiple DependOn(...) calls may follow.
        cacheState.Settings = settings with { ExternalDependencyKeys = null };

        foreach (var dependency in settings.ExternalDependencyKeys ?? [])
            cacheState.ExternalDependencyKeys.Add(dependency.Trim());
    }

    /// <inheritdoc/>
    public void AddOutputCacheDependency(int moduleId, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        // DependOn(...) is additive for the current render, so just normalize and union the key.
        GetOrCreateOutputCacheData(moduleId).ExternalDependencyKeys.Add(key.Trim());
    }

    /// <inheritdoc/>
    public OutputCacheSettings? GetOutputCache(int moduleId)
    {
        if (!_moduleOutputCacheSpecs.TryGetValue(moduleId, out var cacheState))
            return null;

        // Flush once consumed so the next render starts with a clean state buffer.
        _moduleOutputCacheSpecs.Remove(moduleId);

        var hasDependencies = cacheState.ExternalDependencyKeys.Count > 0;
        if (cacheState.Settings == null && !hasDependencies)
            return null;

        // Rehydrate the final settings object with the merged dependency keys collected during rendering.
        return (cacheState.Settings ?? new()) with
        {
            ExternalDependencyKeys = hasDependencies
                ? cacheState.ExternalDependencyKeys.OrderBy(key => key, StringComparer.Ordinal).ToArray()
                : null
        };
    }

    private ModuleOutputCacheSettings GetOrCreateOutputCacheData(int moduleId)
    {
        if (_moduleOutputCacheSpecs.TryGetValue(moduleId, out var cacheState))
            return cacheState;

        return _moduleOutputCacheSpecs[moduleId] = new();
    }

    #endregion
}
