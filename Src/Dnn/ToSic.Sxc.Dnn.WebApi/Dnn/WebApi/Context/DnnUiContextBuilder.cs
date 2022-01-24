using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Context;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Dnn.WebApi.Context
{
    public sealed class DnnUiContextBuilder : UiContextBuilderBase
    {
        #region Constructor / DI

        private readonly IContextResolver _ctxResolver;
        private readonly RemoteRouterLink _remoteRouterLink;
        private readonly PortalSettings _portal = PortalSettings.Current;

        private ModuleInfo Module => (_ctxResolver.BlockOrNull()?.Module as DnnModule)?.UnwrappedContents;

        public DnnUiContextBuilder(IContextResolver ctxResolver, RemoteRouterLink remoteRouterLink, Dependencies deps) : base(deps)
        {
            _ctxResolver = ctxResolver;
            _remoteRouterLink = remoteRouterLink;
        }

        #endregion

        protected override ContextResourceWithApp GetSystem(Ctx flags)
        {
            var result = base.GetSystem(flags);
            result.Url = VirtualPathUtility.ToAbsolute("~/");
            return result;
        }

        protected override ContextResourceWithApp GetSite(Ctx flags)
        {
            var result = base.GetSite(flags);
            result.Id = _portal.PortalId;
            result.Url = "//" + _portal.PortalAlias.HTTPAlias + "/";
            return result;
        }

        protected override WebResourceDto GetPage() =>
            Module == null ? null
                : new WebResourceDto
                {
                    Id = Module.TabID,
                    // todo: maybe page url
                    // used to be on ps.ActiveTab.FullUrl;
                    // but we can't get it from there directly
                };

        protected override ContextAppDto GetApp(Ctx flags)
        {
            var appDto = base.GetApp(flags);
            // If no app is selected yet, then there is no information to return
            if (appDto == null) return null;

            try
            {
                var roots = DnnJsApiHeader.GetApiRoots();
                appDto.Api = roots.Item2;
            } catch { /* ignore */ }
            return appDto;
        }

        /// <summary>
        /// build a getting-started url which is used to correctly show the user infos like
        /// warnings related to his dnn or 2sxc version
        /// infos based on his languages
        /// redirects based on the app he's looking at, etc.
        /// </summary>
        /// <returns></returns>
        protected override string GetGettingStartedUrl()
        {
            if (!(App is IApp app)) return "";

            var gsUrl = _remoteRouterLink.LinkToRemoteRouter(
                RemoteDestinations.GettingStarted,
                Deps.SiteCtx.Site,
                Module.ModuleID,
                app,
                Module.DesktopModule.ModuleName == "2sxc");
            return gsUrl;
        }
    }
}
