using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using Page = ToSic.Sxc.Context.Page;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtPage : Page, IWrapper<Oqtane.Models.Page>
    {
        private readonly SiteState _siteState;
        private readonly LazyInit<IAliasRepository> _aliasRepository;
        private readonly LazyInit<IPageRepository> _pages;
        private readonly LazyInit<ILinkPaths> _linkPathsLazy;

        public Alias Alias { get; set; }

        public OqtPage(LazyInit<IHttp> httpBlazor, SiteState siteState, LazyInit<IAliasRepository> aliasRepository, LazyInit<IPageRepository> pages, LazyInit<ILinkPaths> linkPathsLazy) : base(httpBlazor)
        {
            _siteState = siteState;
            _aliasRepository = aliasRepository;
            _pages = pages;
            _linkPathsLazy = linkPathsLazy;
        }

        public Oqtane.Models.Page UnwrappedContents { get; set; }
        public Oqtane.Models.Page GetContents() => UnwrappedContents;
        public new OqtPage Init(int id)
        {
            base.Init(id);

            UnwrappedContents = _pages.Value.GetPage(id);

            Url = GetUrl(GetAlias(UnwrappedContents.SiteId));
            return this;
        }

        private ILinkPaths LinkPaths => _linkPathsLazy.Value;

        public string GetUrl(Alias alias)
        {
            // Page url in Oqtane is without protocol, so we need to add it from current request for consistency
            // also without trailing slash
            var parts = new UrlParts(LinkPaths.GetCurrentRequestUrl());
            return $"{parts.Protocol}{alias.Name}/{UnwrappedContents.Path}".TrimLastSlash();
        }

        private Alias GetAlias(int siteId)
        {
            Alias ??= _siteState.Alias;
            if (Alias != null && Alias.SiteId == siteId) return Alias;
            Alias = _aliasRepository.Value.GetAliases().OrderBy(a => a.Name).FirstOrDefault(a => a.SiteId == siteId); // best guess
            return Alias;
        }
    }
}
