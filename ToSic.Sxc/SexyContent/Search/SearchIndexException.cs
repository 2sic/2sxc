using System;
using System.Runtime.Serialization;
using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent.Search
{
    public class SearchIndexException : Exception
    {
        public SearchIndexException(ModuleInfo moduleInfo, Exception innerException)
            : base("Search: Error while indexing module " + moduleInfo.ModuleID + " on tab " + moduleInfo.TabID + ", portal " + moduleInfo.PortalID, innerException) { }
    }
}