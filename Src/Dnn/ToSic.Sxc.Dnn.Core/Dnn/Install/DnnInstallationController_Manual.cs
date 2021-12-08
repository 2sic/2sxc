using System;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Run;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnInstallationController
    {

        public bool ResumeAbortedUpgrade()
        {
            var callLog = Log.Call<bool>();
            if (IsUpgradeRunning)
            {
                Log.Add("Upgrade is still running");
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 3-4 minutes, please restart the web application.");
            }

            _installLogger.LogStep("", "FinishAbortedUpgrade starting", false);
            _installLogger.LogStep("", "Will handle " + Settings.Installation.UpgradeVersionList.Length + " versions");
            // Run upgrade again for all versions that do not have a corresponding logfile
            foreach (var upgradeVersion in Settings.Installation.UpgradeVersionList)
            {
                var complete = IsUpgradeComplete(upgradeVersion, "- check for FinishAbortedUpgrade");
                _installLogger.LogStep("", "Status for version " + upgradeVersion + " is " + complete);
                if (!complete)
                    UpgradeModule(upgradeVersion);
            }

            _installLogger.LogStep("", "FinishAbortedUpgrade done", false);

            // Restart application
            HttpRuntime.UnloadAppDomain();
            return callLog("ok", true);
        }


        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool forContentApp)
        {
            var moduleInfo = (module as DnnModule)?.UnwrappedContents;
            var portal = (site as DnnSite)?.UnwrappedContents;
            if(moduleInfo == null || portal == null)
                throw new ArgumentException("missing portal/module");

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (forContentApp)
                try
                {
                    var primaryAppId = DnnStaticDi.StaticBuild<IAppStates>().DefaultAppId(site.ZoneId);
                    // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                    var contentViews = DnnStaticDi.StaticBuild<CmsRuntime>()
                        .Init(new AppIdentity(site.ZoneId, primaryAppId), false, Log)
                        .Views.GetAll();
                    if (contentViews.Any()) return null;
                }
                catch { /* ignore */ }
            
            var gettingStartedSrc = DnnStaticDi.StaticBuild<WipRemoteRouterLink>().LinkToRemoteRouter(
                RemoteDestinations.AutoConfigure, 
                "Dnn",
                Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4),
                DotNetNuke.Entities.Host.Host.GUID, 
                site,
                module.Id,
                app: null,
                forContentApp);

            // Set src to iframe
            return gettingStartedSrc;
        }
    }
}
