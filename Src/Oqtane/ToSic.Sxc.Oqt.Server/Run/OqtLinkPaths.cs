using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Oqt.Server.Plumbing;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtLinkPaths(IHttpContextAccessor contextAccessor, AliasResolver aliasResolver)
    : ILinkPaths
{
    #region Paths

    private string ToWebAbsolute(string virtualPath)
    {
        virtualPath = virtualPath.TrimStart('~');
        return virtualPath.PrefixSlash().ForwardSlash();
    }

    public string AsSeenFromTheDomainRoot(string virtualPath) => ToWebAbsolute(virtualPath);

    public string ApiFromSiteRoot(string appFolder, string apiPath) => $"/app/{appFolder}/{apiPath}";

    public string AppFromTheDomainRoot(string appFolder, string pagePath)
    {
        var siteRoot = OqtPageOutput.GetSiteRoot(aliasResolver.Alias).TrimLastSlash();
        return AppFromTheDomainRoot(siteRoot, appFolder, pagePath);
    }

    public string AppFromTheDomainRoot(string siteRoot, string appFolder, string pagePath) => $"{siteRoot}/app/{appFolder}/{pagePath}";

    #endregion

    public string GetCurrentRequestUrl() => contextAccessor.HttpContext?.Request?.GetEncodedUrl() ?? string.Empty;

    public string GetCurrentLinkRoot()
    {
        var scheme = contextAccessor?.HttpContext?.Request?.Scheme ?? "http";
        var alias = aliasResolver.Alias;
        var domainName = string.IsNullOrEmpty(alias.Path)
            ? alias.Name
            : alias.Name.Substring(0, alias.Name.Length - alias.Path.Length - 1);
        return $"{scheme}://{domainName}";
    }
}