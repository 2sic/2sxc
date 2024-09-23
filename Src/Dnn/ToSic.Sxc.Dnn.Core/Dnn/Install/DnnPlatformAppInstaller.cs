using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Integration.Installation;

namespace ToSic.Sxc.Dnn.Install;

internal class DnnPlatformAppInstaller(
    LazySvc<IAppsCatalog> appsCatalog,
    GenWorkPlus<WorkViews> workViews,
    LazySvc<RemoteRouterLink> remoteRouterLazy)
    : ServiceBase("Dnn.AppIns", connect: [workViews, appsCatalog, remoteRouterLazy]), IPlatformAppInstaller
{
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
                var primaryAppId = appsCatalog.Value.DefaultAppIdentity(site.ZoneId);
                // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                var contentViews = workViews.New(primaryAppId).GetAll();
                if (contentViews.Any()) return null;
            }
            catch
            {
                /* ignore */
            }

        var gettingStartedSrc = remoteRouterLazy.Value.LinkToRemoteRouter(
            RemoteDestinations.AutoConfigure,
            site,
            module.Id,
            appSpecsOrNull: null,
            forContentApp);

        // Set src to iframe
        return l.ReturnAsOk(gettingStartedSrc);
    }
}