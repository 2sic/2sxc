using ToSic.Razor.Blade;

namespace ToSic.Sxc.Render.Output.Sys;

internal partial class ModulesOutputService
{
    /// <summary>
    /// Stores ModuleServiceData instances, scoped by ModuleId.
    /// </summary>
    private readonly Dictionary<int, ModuleAddOnTags> _moduleTags = new();

    /// <summary>
    /// Record to store list of HTML tags and a set of distinct tag identifiers for each module.
    /// </summary>
    private record ModuleAddOnTags
    {
        public List<IHtmlTag> HtmlTags { get; } = [];
        public HashSet<string> DistinctTags { get; } = [];
    }

    /// <inheritdoc />
    public IHtmlTag? AddTag(int moduleId, IHtmlTag tag, bool noDuplicates = false)
    {
        var nameId = tag.ToString();

#if NETFRAMEWORK
        // DNN implementation must flush the moduleID. It is not used to differentiate the cache, as that is already handled.
        moduleId = default;
#endif
        var moduleServiceData = GetOrCreateModuleData(moduleId);
        if (noDuplicates && moduleServiceData.DistinctTags.Contains(nameId))
            return null;
        moduleServiceData.DistinctTags.Add(nameId);
        moduleServiceData.HtmlTags.Add(tag);
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

        return moduleServiceData.HtmlTags;
    }

    private ModuleAddOnTags GetOrCreateModuleData(int moduleId)
    {
       if (_moduleTags.TryGetValue(moduleId, out var moduleServiceData))
            return moduleServiceData;

        // Handle the case where the moduleId does not exist
        return _moduleTags[moduleId] = new();
    }
}
