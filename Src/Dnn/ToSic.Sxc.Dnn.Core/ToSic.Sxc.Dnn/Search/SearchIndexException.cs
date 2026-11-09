using DotNetNuke.Entities.Modules;

namespace ToSic.Sxc.Dnn.Search;

/// <summary>
/// Special search exception, so these exceptions can be handled in a special way if necessary.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
internal class SearchIndexException(ModuleInfo module, Exception innerException, string source, int count, int max)
    : Exception(
        $"Search: Error #{count} of {max} in '{source}' while indexing module {module.ModuleID} on tab {module.TabID}, portal {module.PortalID}",
        innerException);