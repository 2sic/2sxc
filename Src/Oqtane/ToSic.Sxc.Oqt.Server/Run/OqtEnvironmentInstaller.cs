using Oqtane.Infrastructure;
using System;
using System.Linq;
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
        private readonly RemoteRouterLink _remoteRouterLink;
        private readonly IAppStates _appStates;
        private readonly IConfigManager _configManager;

        public OqtEnvironmentInstaller(Lazy<CmsRuntime> cmsRuntimeLazy, RemoteRouterLink remoteRouterLink, IAppStates appStates, IConfigManager configManager) : base($"{OqtConstants.OqtLogPrefix}.Instll")
        {
            _cmsRuntimeLazy = cmsRuntimeLazy;
            _remoteRouterLink = remoteRouterLink;
            _appStates = appStates;
            _configManager = configManager;
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
                    var contentAppId = _appStates.IdentityOfDefault(site.ZoneId); //.DefaultAppId(site.ZoneId);
                    // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                    var contentViews = _cmsRuntimeLazy.Value
                        .Init(contentAppId /*_appStates.Identity(null, primaryAppId)*/, false, Log)
                        .Views.GetAll();
                    if (contentViews.Any()) return null;
                }
                catch { /* ignore */ }

            var link = _remoteRouterLink.LinkToRemoteRouter(
                RemoteDestinations.AutoConfigure,
                "Oqt",
                Oqtane.Shared.Constants.Version,
                _configManager.GetInstallationId(),
                site,
                module.Id,
                app: null,
                forContentApp);

            return link;
        }

    }
}
