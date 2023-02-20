using Oqtane.Shared;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Context;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public class OqtUiContextBuilder: UiContextBuilderBase
    {
        public OqtUiContextBuilder(ILinkPaths linkPaths, IContextOfSite ctx, SiteState siteState, RemoteRouterLink remoteRouterLink, MyServices deps) : base(deps)
        {
            _linkPaths = linkPaths;
            _context = ctx;
            _siteState = siteState;
            _remoteRouterLink = remoteRouterLink;
        }

        private readonly ILinkPaths _linkPaths;
        private readonly IContextOfSite _context;
        private readonly SiteState _siteState;
        private readonly RemoteRouterLink _remoteRouterLink;


        protected override ContextResourceWithApp GetSystem(Ctx flags)
        {
            var result = base.GetSystem(flags);

            result.Url = _linkPaths.AsSeenFromTheDomainRoot("~/");
            return result;
        }

        protected override ContextResourceWithApp GetSite(Ctx flags)
        {
            var result = base.GetSite(flags);

            result.Id = _context.Site.Id;
            result.Url = "//" + _context.Site.UrlRoot;
            return result;
        }

        protected override WebResourceDto GetPage() =>
            new()
            {
                Id = (_context as IContextOfBlock)?.Page.Id ?? Eav.Constants.NullId,
            };

        protected override ContextAppDto GetApp(Ctx flags)
        {
            var appDto = base.GetApp(flags);
            if (appDto != null) appDto.Api = OqtPageOutput.GetSiteRoot(_siteState);
            return appDto;
        }

        protected override string GetGettingStartedUrl()
        {
            var blockCtx = _context as IContextOfBlock; // may be null!

            var gsUrl = _remoteRouterLink.LinkToRemoteRouter(
                RemoteDestinations.GettingStarted,
                Services.SiteCtx.Site,
                blockCtx?.Module.Id ?? 0,
                Services.AppToLaterInitialize,
                true
                );
            return gsUrl;
        }
    }
}
