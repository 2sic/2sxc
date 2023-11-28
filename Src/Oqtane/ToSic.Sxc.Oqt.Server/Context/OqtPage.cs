using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Lib.Data;
using ToSic.Lib.DI;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using Page = ToSic.Sxc.Context.Page;

namespace ToSic.Sxc.Oqt.Server.Context;

public class OqtPage : Page, IWrapper<Oqtane.Models.Page>
{
    private readonly SiteState _siteState;
    private readonly LazySvc<IAliasRepository> _aliasRepository;
    private readonly LazySvc<IPageRepository> _pages;
    private readonly LazySvc<ILinkPaths> _linkPathsLazy;

    public Alias Alias { get; set; }

    public OqtPage(LazySvc<IHttp> httpBlazor, SiteState siteState, LazySvc<IAliasRepository> aliasRepository, LazySvc<IPageRepository> pages, LazySvc<ILinkPaths> linkPathsLazy) : base(httpBlazor)
    {
        _siteState = siteState;
        _aliasRepository = aliasRepository;
        _pages = pages;
        _linkPathsLazy = linkPathsLazy;
    }

    protected Oqtane.Models.Page UnwrappedPage;
    public Oqtane.Models.Page GetContents() => UnwrappedPage;
    public new OqtPage Init(int id)
    {
        base.Init(id);

        UnwrappedPage = _pages.Value.GetPage(id);

        Url = GetUrl(GetAlias(UnwrappedPage.SiteId));
        return this;
    }

    private ILinkPaths LinkPaths => _linkPathsLazy.Value;

    public string GetUrl(Alias alias)
    {
        // Page url in Oqtane is without protocol, so we need to add it from current request for consistency
        // also without trailing slash
        var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
        return $"{parts.Protocol}{alias.Name}/{UnwrappedPage.Path}".TrimLastSlash();
    }

    private Alias GetAlias(int siteId)
    {
        Alias ??= _siteState.Alias;
        if (Alias != null && Alias.SiteId == siteId) return Alias;
        Alias = _aliasRepository.Value.GetAliases().OrderBy(a => a.Name).FirstOrDefault(a => a.SiteId == siteId); // best guess
        return Alias;
    }
}