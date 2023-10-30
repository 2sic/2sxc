using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Dnn.Install
{
    public class DnnPlatformAppInstaller: ServiceBase, IPlatformAppInstaller
    {
        private readonly LazySvc<AppWorkSxc> _appSysSxc;
        private readonly LazySvc<IAppStates> _appStatesLazy;
        private readonly LazySvc<RemoteRouterLink> _remoteRouterLazy;

        public DnnPlatformAppInstaller(
            LazySvc<IAppStates> appStatesLazy,
            LazySvc<AppWorkSxc> appSysSxc,
            LazySvc<RemoteRouterLink> remoteRouterLazy
        ) : base("Dnn.AppIns")
        {
            ConnectServices(
                _appSysSxc = appSysSxc,
                _appStatesLazy = appStatesLazy,
                _remoteRouterLazy = remoteRouterLazy
            );
        }

        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp)
        {
            var l = Log.Fn<string>();
            var moduleInfo = (module as DnnModule)?.GetContents();
            var portal = (site as DnnSite)?.GetContents();
            if (moduleInfo == null || portal == null)
                throw l.Done(new ArgumentException("missing portal/module"));

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (forContentApp)
                try
                {
                    var primaryAppId = _appStatesLazy.Value.IdentityOfDefault(site.ZoneId);
                    // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                    var contentViews = _appSysSxc.Value.AppViews(identity: primaryAppId)
                        .GetAll();
                    if (contentViews.Any()) return null;
                }
                catch
                {
                    /* ignore */
                }

            var gettingStartedSrc = _remoteRouterLazy.Value.LinkToRemoteRouter(
                RemoteDestinations.AutoConfigure,
                site,
                module.Id,
                app: null,
                forContentApp);

            // Set src to iframe
            return l.ReturnAsOk(gettingStartedSrc);
        }
    }
}
