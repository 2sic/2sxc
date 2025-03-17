using ToSic.Lib.Services;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services.Internal;

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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ModuleService() : ServiceBase(SxcLogName + ".ModSvc"), IModuleService
{
    /// <summary>
    /// Stores ModuleServiceData instances, scoped by ModuleId.
    /// </summary>
    private readonly Dictionary<int, ModuleTags> _moduleData = new();

    /// <inheritdoc />
    public void AddTag(IHtmlTag tag, int moduleId, string nameId = null, bool noDuplicates = false)
    {
        if (tag is null)
            return;
        nameId ??= tag.ToString();

#if NETFRAMEWORK
        // DNN implementation must flush the moduleID. It is not used to differentiate the cache, as that is already handled.
        moduleId = default;
#endif
        var moduleServiceData = GetOrCreateModuleData(moduleId);
        if (noDuplicates && moduleServiceData.ExistingKeys.Contains(nameId))
            return;
        moduleServiceData.ExistingKeys.Add(nameId);
        moduleServiceData.MoreTags.Add(tag);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(int moduleId = default)
    {
#if NETFRAMEWORK
        // DNN implementation must flush the moduleID. It is not used to differentiate the cache, as that is already handled.
        moduleId = default;
#endif

        // If there is nothing to get, exit early
        if (!_moduleData.TryGetValue(moduleId, out var moduleServiceData))
            return [];

        // Reset module data to avoid duplicates on subsequent calls
        _moduleData.Remove(moduleId);
        // old code till 2025-03-17, remove ca. Q3 2025
        // _moduleData[moduleId] = new();

        return moduleServiceData.MoreTags;
    }

    private ModuleTags GetOrCreateModuleData(int moduleId)
    {
       if (_moduleData.TryGetValue(moduleId, out var moduleServiceData))
            return moduleServiceData;

        // Handle the case where the moduleId does not exist
        return _moduleData[moduleId] = new();
    }
}