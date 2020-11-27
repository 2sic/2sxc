using System;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Run.Context;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.Dnn.Install
{
    public partial class InstallationController
    {

        public bool ResumeAbortedUpgrade()
        {
            var callLog = Log.Call<bool>();
            if (IsUpgradeRunning)
            {
                Log.Add("Upgrade is still running");
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 10 minutes, please restart the web application.");
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


        public string GetAutoInstallPackagesUiUrl(ISite site, IModule module, bool isContentApp)
        {
            var moduleInfo = (module as DnnModule)?.UnwrappedContents;
            var portal = (site as DnnSite)?.UnwrappedContents;
            if(moduleInfo == null || portal == null)
                throw new ArgumentException("missing portal/module");

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (isContentApp)
                try
                {
                    var primaryAppId = new ZoneRuntime().Init(site.ZoneId, Log).DefaultAppId;
                    // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                    var contentViews = Eav.Factory.Resolve<CmsRuntime>()
                        .Init(State.Identity(null, primaryAppId), false, Log)
                        .Views.GetAll();
                    if (contentViews.Any()) return null;
                }
                catch { /* ignore */ }
            
            // ReSharper disable StringLiteralTypo
            // Add desired destination
            // Add DNN Version, 2SexyContent Version, module type, module id, Portal ID
            var gettingStartedSrc =
                "//gettingstarted.2sxc.org/router.aspx?"
                + "destination=autoconfigure" + (isContentApp ? Eav.Constants.ContentAppName.ToLower() : "app")
                + "&DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                + "&2SexyContentVersion=" + Settings.ModuleVersion
                + "&ModuleName=" + moduleInfo.DesktopModule.ModuleName
                + "&ModuleId=" + module.Id
                + "&PortalID=" + site.Id
                + "&ZoneID=" + site.ZoneId;
            // ReSharper restore StringLiteralTypo

            // Add DNN Guid
            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            // Add Portal Default Language & current language
            gettingStartedSrc += "&DefaultLanguage="
                                 + site.DefaultLanguage
                                 + "&CurrentLanguage=" + portal.CultureCode;

            // Set src to iframe
            return gettingStartedSrc;
        }
    }
}
