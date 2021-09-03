using System;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Run;

using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public class OqtUiContextBuilder: UiContextBuilderBase
    {
        public OqtUiContextBuilder(ILinkPaths linkPaths, IContextOfSite ctx, SiteState siteState, WipRemoteRouterLink remoteRouterLink, IConfigManager configManager, Dependencies deps) : base(deps)
        {
            _linkPaths = linkPaths;
            _context = ctx;
            _siteState = siteState;
            _remoteRouterLink = remoteRouterLink;
            _configManager = configManager;
        }

        private readonly ILinkPaths _linkPaths;
        private IContextOfSite _context;
        private readonly SiteState _siteState;
        private readonly WipRemoteRouterLink _remoteRouterLink;
        private readonly IConfigManager _configManager;


        //protected override ContextLanguageDto GetLanguage()
        //{
        //    return new ContextLanguageDto
        //    {
        //        Current = _oqtCulture.CurrentCultureCode,
        //        Primary = _oqtCulture.DefaultCultureCode,
        //        //All = new Dictionary<string, string>
        //        //{
        //        //    {WipConstants.DefaultLanguage, WipConstants.DefaultLanguageText}
        //        //}
        //        All = _oqtZoneMapper.CulturesWithState(_context.Site.Id, _context.Site.ZoneId)
        //            .Where(c => c.Active)
        //            .AsEnumerable()
        //            .ToDictionary(l => l.Key, l => l.Text),
        //    };
        //}

        protected override ContextResourceWithApp GetSystem(Ctx flags)
        {
            var result = base.GetSystem(flags);

            //return new WebResourceDto
            //{
            result.Url = _linkPaths.AsSeenFromTheDomainRoot("~/");
            //};
            return result;
        }

        protected override ContextResourceWithApp GetSite(Ctx flags)
        {
            var result = base.GetSite(flags);

            //return new WebResourceDto
            //{
            result.Id = _context.Site.Id;
            result.Url = "//" + _context.Site.Url;
            //};
            return result;
        }

        protected override WebResourceDto GetPage() =>
            new WebResourceDto
            {
                Id = (_context as IContextOfBlock)?.Page.Id ?? Eav.Constants.NullId,
            };

        //protected override ContextEnableDto GetEnable()
        //{
        //    return new()
        //    {
        //        AppPermissions = true,
        //        CodeEditor = true,
        //        Query = true
        //    };
        //}

        protected override ContextAppDto GetApp(Ctx flags)
        {
            var appDto = base.GetApp(flags);
            if (appDto != null) appDto.Api = OqtAssetsAndHeaders.GetSiteRoot(_siteState);
            return appDto;
        }

        protected override string GetGettingStartedUrl()
        {
            var blockCtx = _context as IContextOfBlock; // may be null!
            var x = _siteState.Alias.TenantId;
            
            var gsUrl = _remoteRouterLink.LinkToRemoteRouter(
                RemoteDestinations.GettingStarted,
                "Oqt",
                Version.Parse(Oqtane.Shared.Constants.Version)?.ToString(4), // Assembly.GetAssembly(typeof(SiteState))?.GetName().Version?.ToString(4),
                _configManager.GetInstallationId(),
                Deps.SiteCtx.Site,
                blockCtx?.Module.Id ?? 0, // TODO: V12 - REQUIRED FOR CALLBACK TO WORK
                Deps.AppToLaterInitialize,
                true // TODO: V12 - must be set so installer works properly // Module.DesktopModule.ModuleName == "2sxc"
                );
            return gsUrl;
        }
    }
}
