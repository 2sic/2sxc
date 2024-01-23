using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Lib.Data;
using ToSic.Lib.DI;
using ToSic.Sxc.Integration.Paths;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Internal.Url;
using Page = ToSic.Sxc.Context.Internal.Page;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtPage(
    LazySvc<IHttp> httpBlazor,
    SiteState siteState,
    LazySvc<IAliasRepository> aliasRepository,
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
        Alias = aliasRepository.Value.GetAliases().OrderBy(a => a.Name).FirstOrDefault(a => a.SiteId == siteId); // best guess
        return Alias;
    }
}