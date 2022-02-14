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
        public string GetCurrentRequestUrl() => _urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl();
#else
        public string GetCurrentRequestUrl() => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;
#endif

#if NETSTANDARD
        public string GetCurrentLinkRoot() => new Uri(_urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
#else
        public string GetCurrentLinkRoot() => HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
#endif

        }
    }
