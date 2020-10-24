#if NET451
using System.Web;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Mvc;
using System.IO;
#endif

namespace ToSic.Sxc.Run
{
    public class LinkPaths: ILinkPaths
    {
#if NETSTANDARD
        public LinkPaths(IUrlHelper urlHelper) => _urlHelper = urlHelper;

        private readonly IUrlHelper _urlHelper;
#endif

        public string ToAbsolute(string virtualPath)
        {
#if NETSTANDARD
            return _urlHelper.Content(virtualPath);
#else
            return VirtualPathUtility.ToAbsolute(virtualPath);
#endif
        }
        public string ToAbsolute(string virtualPath, string subPath)
        {
#if NETSTANDARD
            return _urlHelper.Content(Path.Combine(virtualPath, subPath));
#else
            return VirtualPathUtility.Combine(virtualPath, subPath);
#endif
        }

    }
}
