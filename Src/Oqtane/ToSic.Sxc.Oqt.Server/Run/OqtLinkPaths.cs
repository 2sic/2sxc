using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Run;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;


namespace ToSic.Sxc.Oqt.Server.Run;

public class OqtLinkPaths: ILinkPaths
{
    public OqtLinkPaths(IHttpContextAccessor contextAccessor, SiteStateInitializer siteStateInitializer)
    {
        _contextAccessor = contextAccessor;
        _siteStateInitializer = siteStateInitializer;
    }

    private readonly IHttpContextAccessor _contextAccessor;
    private readonly SiteStateInitializer _siteStateInitializer;
    public HttpContext Current => _contextAccessor.HttpContext;

    #region Paths


    private string toWebAbsolute(string virtualPath)
    {
        virtualPath = virtualPath.TrimStart('~');
        return virtualPath.PrefixSlash().ForwardSlash();
    }

    public string AsSeenFromTheDomainRoot(string virtualPath) => toWebAbsolute(virtualPath);

    public string ApiFromSiteRoot(string appFolder, string apiPath) => $"/app/{appFolder}/{apiPath}";

    public string AppFromTheDomainRoot(string appFolder, string pagePath)
    {
        var siteRoot = OqtPageOutput.GetSiteRoot(_siteStateInitializer.InitializedState).TrimLastSlash();
        return AppFromTheDomainRoot(siteRoot, appFolder, pagePath);
    }

    public string AppFromTheDomainRoot(string siteRoot, string appFolder, string pagePath) => $"{siteRoot}/app/{appFolder}/{pagePath}";

    #endregion

    public string GetCurrentRequestUrl() => _contextAccessor.HttpContext?.Request?.GetEncodedUrl() ?? string.Empty;

    public string GetCurrentLinkRoot()
    {
        var scheme = _contextAccessor?.HttpContext?.Request?.Scheme ?? "http";
        var alias = _siteStateInitializer.InitializedState.Alias;
        var domainName = string.IsNullOrEmpty(alias.Path)
            ? alias.Name
            : alias.Name.Substring(0, alias.Name.Length - alias.Path.Length - 1);
        return $"{scheme}://{domainName}";
    }
}