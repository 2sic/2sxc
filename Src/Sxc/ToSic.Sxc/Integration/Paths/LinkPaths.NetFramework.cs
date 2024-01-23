#if NETFRAMEWORK
using System.Web;


namespace ToSic.Sxc.Integration.Paths;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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