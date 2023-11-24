using System;
using DotNetNuke.Entities.Modules;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Search;

/// <summary>
/// Special search exception, so these exceptions can be handled in a special way if necessary.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
internal class SearchIndexException : Exception
{
    public SearchIndexException(ModuleInfo module, Exception innerException, string source, int count, int max)
        : base(
            $"Search: Error #{count} of {max} in '{source}' while indexing module {module.ModuleID} on tab {module.TabID}, portal {module.PortalID}", innerException) { }
}