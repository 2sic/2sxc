using System;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Search
{
    /// <summary>
    /// Special search exception, so these exceptions can be handled in a special way if necessary.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class SearchIndexException : Exception
    {
        public SearchIndexException(ModuleInfo moduleInfo, Exception innerException)
            : base("Search: Error while indexing module " + moduleInfo.ModuleID + " on tab " + moduleInfo.TabID + ", portal " + moduleInfo.PortalID, innerException) { }
    }
}