#if !NETFRAMEWORK
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace ToSic.Sxc.Integration.Paths;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
