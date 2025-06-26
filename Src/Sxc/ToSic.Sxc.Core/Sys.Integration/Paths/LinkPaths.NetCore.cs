#if !NETFRAMEWORK
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ToSic.Sxc.Sys.Integration.Paths;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class LinkPaths(IUrlHelper urlHelper) : ILinkPaths
{
    public string AsSeenFromTheDomainRoot(string virtualPath)
        => urlHelper.Content(virtualPath);

    public string GetCurrentRequestUrl()
        => urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl();

    public string GetCurrentLinkRoot()
        => new Uri(urlHelper.ActionContext.HttpContext.Request.GetEncodedUrl())
            .GetLeftPart(UriPartial.Authority);
}
#endif
