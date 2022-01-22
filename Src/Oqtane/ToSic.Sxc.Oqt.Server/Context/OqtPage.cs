using System;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Web;
using Page = ToSic.Sxc.Context.Page;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtPage : Page, IWrapper<Oqtane.Models.Page>
    {
        private readonly SiteState _siteState;
        private readonly Lazy<IAliasRepository> _aliasRepository;
        private readonly Lazy<IPageRepository> _pages;
        private readonly Lazy<ILinkHelper> _linkHelperLazy;
        public Alias Alias { get; set; }

        public OqtPage(Lazy<IHttp> httpBlazor, SiteState siteState, Lazy<IAliasRepository> aliasRepository, Lazy<IPageRepository> pages, Lazy<ILinkHelper> linkHelperLazy) : base(httpBlazor)
        {
            _siteState = siteState;
            _aliasRepository = aliasRepository;
            _pages = pages;
            _linkHelperLazy = linkHelperLazy;
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

        public string GetUrl(Alias alias)
        {
            // Page url in Oqtane is without protocol, so we need to add it from current request for consistency
            // also without trailing slash
            var parts = new UrlParts(_linkHelperLazy.Value.GetCurrentRequestUrl());
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
