using System.Web;
#if NETFRAMEWORK
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
#endif


namespace ToSic.Sxc.Integration.Paths;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LinkPaths: ILinkPaths
{
#if !NETFRAMEWORK
    // ReSharper disable once ConvertToPrimaryConstructor
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

    public string GetCurrentRequestUrl()
#if NETFRAMEWORK
        => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;
#else
        => _urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl();
#endif

    public string GetCurrentLinkRoot()
#if NETFRAMEWORK
        => HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
#else
        => new Uri(_urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);
#endif

}