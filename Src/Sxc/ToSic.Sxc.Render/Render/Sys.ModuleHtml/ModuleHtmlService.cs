using ToSic.Razor.Blade;
using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Render.Sys.ModuleHtml;

/// <summary>
/// Provides functionality to manage module rendering and HTML tag collections,
/// ensuring proper scoping of services for each module instance (sometimes difficult in Oqtane applications).
/// </summary>
/// <remarks>
/// IModuleService is registered as a scoped service.
/// In the Oqtane Interactive Server, the Dependency Injection (DI) session scope is bound to the first HTTP request
/// of the user's browser session and remains unchanged during subsequent SignalR communications (until a full page reload).
/// Consequently, scoped services share the same instance for all 2sxc module instances across all pages during a user's session.
/// To prevent conflicts, the `ModuleId` is used to scope the `ModuleService` functionality to each module rendering.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleHtmlService() : ServiceBase(SxcLogName + ".ModSvc"), IModuleHtmlService
{
    /// <summary>
    /// Stores ModuleServiceData instances, scoped by ModuleId.
    /// </summary>
    private readonly Dictionary<int, ModuleTags> _moduleTags = new();

    /// <inheritdoc />
    public IHtmlTag? AddTag(IHtmlTag tag, int moduleId, string? nameId = null, bool noDuplicates = false)
    {
        //if (tag is null)
        //    return;
        nameId ??= tag.ToString();

#if NETFRAMEWORK
        // DNN implementation must flush the moduleID. It is not used to differentiate the cache, as that is already handled.
        moduleId = default;
#endif
        var moduleServiceData = GetOrCreateModuleData(moduleId);
        if (noDuplicates && moduleServiceData.ExistingKeys.Contains(nameId))
            return null;
        moduleServiceData.ExistingKeys.Add(nameId);
        moduleServiceData.MoreTags.Add(tag);
        return tag;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(int moduleId = default)
    {
#if NETFRAMEWORK
        // DNN implementation must flush the moduleID. It is not used to differentiate the cache, as that is already handled.
        moduleId = default;
#endif

        // If there is nothing to get, exit early
        if (!_moduleTags.TryGetValue(moduleId, out var moduleServiceData))
            return [];

        // Reset module data to avoid duplicates on subsequent calls
        _moduleTags.Remove(moduleId);
        // old code till 2025-03-17, remove ca. Q3 2025
        // _moduleData[moduleId] = new();

        return moduleServiceData.MoreTags;
    }

    private ModuleTags GetOrCreateModuleData(int moduleId)
    {
       if (_moduleTags.TryGetValue(moduleId, out var moduleServiceData))
            return moduleServiceData;

        // Handle the case where the moduleId does not exist
        return _moduleTags[moduleId] = new();
    }

    #region Output Caching

    // Output-cache settings and DependOn(...) calls can happen in any order during one render,
    // so we keep a small per-module buffer until the final render result is assembled.
    public void ConfigureOutputCache(int moduleId, OutputCacheSettings settings)
    {
        var cacheState = GetOrCreateOutputCacheData(moduleId);
        // Dependencies are accumulated separately because multiple DependOn(...) calls may follow.
        cacheState.Settings = settings with { ExternalDependencyKeys = null };

        foreach (var dependency in settings.ExternalDependencyKeys ?? [])
            cacheState.ExternalDependencyKeys.Add(dependency.Trim());
    }

    public void AddOutputCacheDependency(int moduleId, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        // DependOn(...) is additive for the current render, so just normalize and union the key.
        GetOrCreateOutputCacheData(moduleId).ExternalDependencyKeys.Add(key.Trim());
    }

    public OutputCacheSettings? GetOutputCache(int moduleId)
    {
        if (!_moduleOutputCache.TryGetValue(moduleId, out var cacheState))
            return null;

        // Flush once consumed so the next render starts with a clean state buffer.
        _moduleOutputCache.Remove(moduleId);

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

    private ModuleOutputCacheState GetOrCreateOutputCacheData(int moduleId)
    {
        if (_moduleOutputCache.TryGetValue(moduleId, out var cacheState))
            return cacheState;

        return _moduleOutputCache[moduleId] = new();
    }

    // Separate from _moduleTags because tags and output-cache state are flushed at different points in the pipeline.
    private readonly Dictionary<int, ModuleOutputCacheState> _moduleOutputCache = new();


    #endregion
}
