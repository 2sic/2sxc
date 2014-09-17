using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToSic.SexyContent.Search
{
    public class SearchIndexException : Exception, System.Runtime.Serialization.ISerializable
    {
        public SearchIndexException(ModuleInfo moduleInfo, Exception innerException)
            : base("Search: Error while indexing module " + moduleInfo.ModuleID + " on tab " + moduleInfo.TabID + ", portal " + moduleInfo.PortalID, innerException) { }
    }
}