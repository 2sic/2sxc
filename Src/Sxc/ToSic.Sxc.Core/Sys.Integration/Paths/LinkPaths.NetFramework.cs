#if NETFRAMEWORK
using System.Web;

namespace ToSic.Sxc.Sys.Integration.Paths;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class LinkPaths: ILinkPaths
{

    public string AsSeenFromTheDomainRoot(string virtualPath)
        => VirtualPathUtility.ToAbsolute(virtualPath);

    public string GetCurrentRequestUrl()
        => HttpContext.Current?.Request?.Url?.AbsoluteUri ?? string.Empty;

    public string GetCurrentLinkRoot()
        => HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
}
#endif