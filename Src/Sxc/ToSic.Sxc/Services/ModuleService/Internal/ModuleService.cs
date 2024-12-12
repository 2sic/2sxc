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
    // Stores ModuleServiceData instances, scoped by ModuleId.
    private readonly Dictionary<int, ModuleServiceData> _moduleData = new();

    /// <summary>
    /// Adds an HTML tag to the collection of tags to be rendered at the end of the module,
    /// optionally preventing duplicates and scoping to a specific module ID.
    /// </summary>
    /// <param name="tag">The HTML tag to add.</param>
    /// <param name="nameId">
    /// An optional identifier for the tag; if not provided, the string representation of the tag is used.
    /// This identifier helps prevent duplicate tags if <paramref name="noDuplicates"/> is set to true.
    /// </param>
    /// <param name="noDuplicates">
    /// If true, the tag will only be added if it does not already exist in the collection.
    /// </param>
    /// <param name="moduleId">
    /// The ID of the module to which the tag should be scoped; defaults to the current module.
    /// </param>
    public void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false, int moduleId = default)
    {
        if (tag is null) return;
        nameId ??= tag.ToString();
        var moduleServiceData = GetOrCreateModuleServiceData(moduleId);
        if (noDuplicates && moduleServiceData.ExistingKeys.Contains(nameId)) return;
        moduleServiceData.ExistingKeys.Add(nameId);
        moduleServiceData.MoreTags.Add(tag);
    }

    /// <summary>
    /// Retrieves and clears stored tags for the specified module to prevent duplicate data accumulation.
    /// </summary>
    /// <param name="moduleId">The ID of the module.</param>
    /// <returns>A read-only collection of HTML tags associated with the module.</returns>
    public IReadOnlyCollection<IHtmlTag> GetMoreTagsAndFlush(int moduleId = default)
    {
        var moduleServiceData = GetOrCreateModuleServiceData(moduleId);
        var tags = moduleServiceData.MoreTags;
        _moduleData[moduleId] = new(); // Reset module data to avoid duplicates on subsequent calls
        return tags;
    }

    private ModuleServiceData GetOrCreateModuleServiceData(int moduleId = default)
    {
        if (_moduleData.TryGetValue(moduleId, out var moduleServiceData)) 
            return moduleServiceData;

        // Handle the case where the moduleId does not exist
        return _moduleData[moduleId] = new ModuleServiceData();
    }
}