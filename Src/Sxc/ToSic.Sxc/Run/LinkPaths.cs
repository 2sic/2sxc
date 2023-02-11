#if NETFRAMEWORK
using System.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
#endif
using System;


namespace ToSic.Sxc.Run
{
    public class LinkPaths: ILinkPaths
    {
#if !NETFRAMEWORK
        public LinkPaths(IUrlHelper urlHelper) => _urlHelper = urlHelper;
        private readonly IUrlHelper _urlHelper;
#endif

        public string AsSeenFromTheDomainRoot(string virtualPath)
        {
#if NETFRAMEWORK
            return VirtualPathUtility.ToAbsolute(virtualPath);
#else
            return _urlHelper.Content(virtualPath);
#endif
        }

#if NETFRAMEWORK
        public string GetCurrentRequestUrl() => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;
#else
        public string GetCurrentRequestUrl() => _urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl();
#endif

#if NETFRAMEWORK
        public string GetCurrentLinkRoot() => HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
#else
        public string GetCurrentLinkRoot() => new Uri(_urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
#endif

        }
    }
