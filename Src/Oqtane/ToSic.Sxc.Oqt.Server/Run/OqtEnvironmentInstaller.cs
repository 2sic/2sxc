using System;
using System.Linq;
using System.Reflection;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEnvironmentInstaller: HasLog<IEnvironmentInstaller>, IEnvironmentInstaller
    {
        private readonly Lazy<CmsRuntime> _cmsRuntimeLazy;
        private readonly WipRemoteRouterLink _remoteRouterLink;
        private readonly IAppStates _appStates;

        public OqtEnvironmentInstaller(Lazy<CmsRuntime> cmsRuntimeLazy, WipRemoteRouterLink remoteRouterLink, IAppStates appStates): base($"{OqtConstants.OqtLogPrefix}.Instll")
        {
            _cmsRuntimeLazy = cmsRuntimeLazy;
            _remoteRouterLink = remoteRouterLink;
            _appStates = appStates;
        }


        public string UpgradeMessages()
        {
            // for now, always assume installation worked
            return null;
        }

        private bool IsUpgradeRunning => false;

        public bool ResumeAbortedUpgrade()
        {
            // don't do anything for now
            return true;
        }

        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp)
        {

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (forContentApp)
                try
                {
                    var primaryAppId = _appStates.DefaultAppId(site.ZoneId); // new ZoneRuntime().Init(site.ZoneId, Log).DefaultAppId;
                    // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                    var contentViews = _cmsRuntimeLazy.Value //  Eav.Factory.Resolve<CmsRuntime>()
                        .Init(_appStates.Identity(null, primaryAppId), false, Log)
                        .Views.GetAll();
                    if (contentViews.Any()) return null;
                }
                catch { /* ignore */ }

            var link = _remoteRouterLink.LinkToRemoteRouter(
                RemoteDestinations.AutoConfigure,
                "Oqt",
                Assembly.GetAssembly(typeof(SiteState))?.GetName().Version?.ToString(4),
                Guid.Empty.ToString(), // TODO
                site,
                module.Id,
                app: null,
                forContentApp);

            return link;
        }

    }
}
