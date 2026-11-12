using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Install.Sys;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Sxc.Apps.Sys.Work;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Sys.Integration.Installation;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtEnvironmentInstaller(
    IContextCompleteService context,
    AppWorkQuick<WorkViewsGet> workViews,
    ExternalLinksService externalLinksService,
    IAppsCatalog appsCatalog)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Instll", connect: [context, workViews, appsCatalog, externalLinksService]),
        IEnvironmentInstaller, IPlatformAppInstaller
{
    public string UpgradeMessages()
    {
        // for now, always assume installation worked
        return null;
    }

    //private bool IsUpgradeRunning => false;

    public bool ResumeAbortedUpgrade()
    {
        // don't do anything for now
        return true;
    }

    public string GetAutoInstallPackagesUiUrl(bool forContentApp)
    {
        var site = context.BlockContextRequired().Site;
        var module = context.BlockContextRequired().Module;

        // new: check if it should allow this
        // it should only be allowed, if the current situation is either
        // Content - and no views exist (even invisible ones)
        // App - and no apps exist - this is already checked on client side, so I won't include a check here
        if (forContentApp)
            try
            {
                var contentAppId = appsCatalog.DefaultAppIdentity(site.ZoneId);
                // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                var contentViews = workViews.New(contentAppId).AllViews;
                if (contentViews.Any())
                    return null;
            }
            catch { /* ignore */ }

        var link = externalLinksService.LinkToDestination(
            ExternalSxcDestinations.AutoConfigure,
            site,
            module.Id,
            appSpecsOrNull: null,
            forContentApp);

        return link;
    }

}