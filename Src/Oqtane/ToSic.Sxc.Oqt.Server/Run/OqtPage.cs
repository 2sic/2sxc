using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtPage : Context.Page, IWrapper<Oqtane.Models.Page>
    {
        private readonly Lazy<SiteState> _siteState;
        private readonly Lazy<IAliasRepository> _aliasRepository;
        private readonly Lazy<IPageRepository> _pages;
        public Alias Alias { get; set; }

        public OqtPage(Lazy<IHttp> httpBlazor, Lazy<SiteState> siteState, Lazy<IAliasRepository> aliasRepository, Lazy<IPageRepository> pages) : base(httpBlazor)
        {
            _siteState = siteState;
            _aliasRepository = aliasRepository;
            _pages = pages;
        }

        public Oqtane.Models.Page UnwrappedContents { get; set; }

        public new OqtPage Init(int id)
        {
            base.Init(id);

            UnwrappedContents = _pages.Value.GetPage(id);

            Url = GetUrl(GetAlias(UnwrappedContents.SiteId));
            return this;
        }

        public string GetUrl(Alias alias) => (alias != null) ? $"//{alias?.Path}/{UnwrappedContents.Path}" : string.Empty;

        private Alias GetAlias(int siteId)
        {
            Alias ??= _siteState.Value.Alias;
            if (Alias != null && Alias.SiteId == siteId) return Alias;
            Alias = _aliasRepository.Value.GetAliases().OrderBy(a => a.Name).FirstOrDefault(a => a.SiteId == siteId); // best guess
            return Alias;
        }
    }
}
