using System;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Run
{
    internal class LinkPathsUnknown : ILinkPaths, IIsUnknown
    {
        public LinkPathsUnknown(WarnUseOfUnknown<LinkHelperUnknown> warn)
        {
            
        }
        
        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
            throw new NotImplementedException();
        }

        // Stub CurrentPage
        public string GetCurrentRequestUrl() => LinkHelperUnknown.NiceCurrentUrl;

        // Stub DomainName
        public string GetCurrentLinkRoot() => LinkHelperUnknown.DefRoot;
    }
}
