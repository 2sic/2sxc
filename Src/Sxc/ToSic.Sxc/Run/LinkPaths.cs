#if NETFRAMEWORK
using System.Web;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace ToSic.Sxc.Run
{
    public class LinkPaths: ILinkPaths
    {
#if NETSTANDARD
        public LinkPaths(IUrlHelper urlHelper) => _urlHelper = urlHelper;
        private readonly IUrlHelper _urlHelper;
#endif

        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
#if NETSTANDARD
            return _urlHelper.Content(virtualPath);
#else
            return VirtualPathUtility.ToAbsolute(virtualPath);
#endif
        }
    }
}
