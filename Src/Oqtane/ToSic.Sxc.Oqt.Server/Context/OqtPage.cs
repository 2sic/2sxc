using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Lib.Data;
using ToSic.Lib.DI;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Internal.Url;
using ToSic.Sxc.Web.Parameters;
using Page = ToSic.Sxc.Context.Internal.Page;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtPage(
    LazySvc<IHttp> httpBlazor,
    SiteState siteState,
    LazySvc<AliasResolver> aliasResolver,
    LazySvc<IPageRepository> pages,
    LazySvc<ILinkPaths> linkPathsLazy)
    : Page(httpBlazor), IWrapper<Oqtane.Models.Page>
{
    public Alias Alias { get; set; }

    protected Oqtane.Models.Page UnwrappedPage;
    public Oqtane.Models.Page GetContents() => UnwrappedPage;
    public new OqtPage Init(int id)
    {
        base.Init(id);

        UnwrappedPage = pages.Value.GetPage(id);

        Url = GetUrl(GetAlias(UnwrappedPage.SiteId));
        return this;
    }

    private ILinkPaths LinkPaths => linkPathsLazy.Value;

    public string GetUrl(Alias alias)
    {
        // Page url in Oqtane is without protocol, so we need to add it from current request for consistency
        // also without trailing slash
        var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
        return $"{parts.Protocol}{alias.Name}/{UnwrappedPage.Path}".TrimLastSlash();
    }

    private Alias GetAlias(int siteId)
    {
        Alias ??= siteState.Alias;
        if (Alias != null && Alias.SiteId == siteId) return Alias;
        if (aliasResolver.Value.InitIfEmpty(siteId)) Alias = aliasResolver.Value.Alias;
        return Alias;
    }

    // caching is disabled because in Blazor Interactive the query string parameters are changed after the page is created
    public override IParameters Parameters => new Parameters
    {
        Nvc = OriginalParameters.GetOverrideParams(httpBlazor.Value?.QueryStringParams)
    };

}