using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sxc.Web.Sys.Parameters;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using OqtanePage = Oqtane.Models.Page;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtPage(
    LazySvc<IHttp> httpBlazor,
    SiteState siteState,
    LazySvc<AliasResolver> aliasResolver,
    LazySvc<IPageRepository> pages,
    LazySvc<ILinkPaths> linkPathsLazy)
    : IPage, IWrapper<OqtanePage>
{
    public int Id { get; private set; } = EavConstants.NullId;

    public string Url { get; private set; } = EavConstants.UrlNotInitialized;

    public Alias Alias { get; set; }

    protected OqtanePage UnwrappedPage;

    public OqtanePage GetContents() => UnwrappedPage;

    public OqtPage OqtInit(int id)
    {
        Id = id;

        UnwrappedPage = pages.Value.GetPage(id);

        Url = GetUrl(GetAlias(UnwrappedPage.SiteId));
        return this;
    }
    
    private string GetUrl(Alias alias)
    {
        // Page url in Oqtane is without protocol, so we need to add it from current request for consistency
        // also without trailing slash
        var parts = new UrlParts(linkPathsLazy.Value.GetCurrentRequestUrl());
        return $"{parts.Protocol}{alias.Name}/{UnwrappedPage.Path}".TrimLastSlash();
    }

    private Alias GetAlias(int siteId)
    {
        Alias ??= siteState.Alias;
        if (Alias != null && Alias.SiteId == siteId)
            return Alias;
        if (aliasResolver.Value.InitIfEmpty(siteId))
            Alias = aliasResolver.Value.Alias;
        return Alias;
    }

    /// <summary>
    /// Parameters.
    /// </summary>
    /// <remarks>
    /// Caching must be disabled (so no field ??=)
    /// because in Blazor Interactive the query string parameters are changed after the page is created
    /// </remarks>
    public IParameters Parameters => new Parameters
    {
        Nvc = OriginalParameters.GetOverrideParams(httpBlazor.Value?.QueryStringParams)
    };

}