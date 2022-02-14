#if NETFRAMEWORK
using System.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
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

#if NETSTANDARD
        public string GetCurrentRequestUrl() => _urlHelper.ActionContext.HttpContext?.Request?.GetEncodedUrl() ?? string.Empty;
#else
        public string GetCurrentRequestUrl() => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;
#endif

    }
}
