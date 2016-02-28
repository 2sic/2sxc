using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent.Statics;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class SystemController : DnnApiController
	{

	    [HttpGet]
	    public dynamic GetLanguages()
	    {
	        var portalId = PortalSettings.PortalId;
            var zoneId = ZoneHelpers.GetZoneID(portalId);
            var cultures = ZoneHelpers.GetCulturesWithActiveState(portalId, zoneId.Value).Select(c => new
            {
                c.Code,
                Culture = c.Text,
                IsEnabled = c.Active
            });
	        return cultures;
	    }

	    /// <summary>
	    /// Helper to prepare a quick-info about 1 content type
	    /// </summary>
	    /// <param name="allCTs"></param>
	    /// <param name="staticName"></param>
	    /// <param name="maybeEntity"></param>
	    /// <returns></returns>
	    [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable)
	    {
            // Activate or Deactivate the Culture
            var portalId = PortalSettings.PortalId;
            var zoneId = ZoneHelpers.GetZoneID(portalId);
	        var cache = DataSource.GetCache(zoneId.Value);
            var sexy = new InstanceContext(zoneId.Value, cache.AppId);
            var cultureText = LocaleController.Instance.GetLocale(cultureCode).Text;

            sexy.ContentContext.Dimensions.AddOrUpdateLanguage(cultureCode, cultureText, enable, PortalSettings.PortalId);
	    }


        #region Apps

        [HttpGet]
        public dynamic Apps(int zoneId)
        {
            var list = AppHelpers.GetApps(zoneId, true, new PortalSettings(ActiveModule.OwnerPortalID));
            return list.Select(a => new
            {
                Id = a.AppId,
                IsApp = a.AppGuid != Constants.DefaultAppName,
                Guid = a.AppGuid,
                a.Name,
                a.Folder,
                AppRoot = GetPath(zoneId, a.AppId),
                IsHidden = a.Hidden,
                //Tokens = a.Settings?.AllowTokenTemplates ?? false,
                //Razor = a.Configuration?.AllowRazorTemplates ?? false,
                ConfigurationId = a.Configuration?.EntityId ?? null
            }).ToList();
        }

        private string GetPath(int zoneId, int appId)
        {
            var sexy = new InstanceContext(zoneId, appId);
            return sexy.App.Path;
        }

        [HttpGet]
        public void DeleteApp(int zoneId, int appId)
        {
            var userId = PortalSettings.Current.UserId;
            //var portalId = this.PortalSettings.PortalId;
            AppHelpers.RemoveApp(zoneId, appId, this.PortalSettings, userId);
        }

        [HttpPost]
        public void App(int zoneId, string name)
        {
            AppHelpers.AddApp(zoneId, name, new PortalSettings(ActiveModule.OwnerPortalID));
        }

        #endregion

        #region Dialog Helpers

        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            var sxc = Request.GetSxcOfModuleContext(appId);

            return new
            {
                IsContent = sxc.App.AppGuid == "Default",
                Language = PortalSettings.Current.CultureCode,
                LanguageDefault = PortalSettings.Current.DefaultLanguage,
                GettingStartedUrl = GettingStartedUrl(sxc.App)
            };
        }

        // build a getting-started url which is used to correctly show the user infos like
        // warnings related to his dnn or 2sxc version
        // infos based on his languages
        // redirects based on the app he's looking at, etc.
        private string GettingStartedUrl(App app)
        {
            var dnn = PortalSettings.Current;
            var mod = Request.FindModuleInfo();
            //int appId = sxc.AppId.Value;
            var gsUrl = "//gettingstarted.2sxc.org/router.aspx?" // change to use protocoll neutral base URL, also change to 2sxc

                        // Add version & module infos
                        + "DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                        + "&2SexyContentVersion=" + Settings.ModuleVersion
                        + "&ModuleName=" + mod.DesktopModule.ModuleName
                        + "&ModuleId=" + mod.ModuleID
                        + "&PortalID=" + dnn.PortalId
                        + "&ZoneID=" + app.ZoneId
                        + "&DefaultLanguage=" + dnn.DefaultLanguage
                        + "&CurrentLanguage=" + dnn.CultureCode;
            ;
            // Add AppStaticName and Version
            if (mod.DesktopModule.ModuleName != "2sxc")
            {
                //var app = sxc.App;// SexyContent.GetApp(sexy, appId.Value, Sexy.OwnerPS);

                gsUrl += "&AppGuid=" + app.AppGuid;
                if (app.Configuration != null)
                {
                    gsUrl += "&AppVersion=" + app.Configuration.Version;
                    gsUrl += "&AppOriginalId=" + app.Configuration.OriginalId;
                }
            }

            var HostSettings = HostController.Instance.GetSettingsDictionary();
            gsUrl += HostSettings.ContainsKey("GUID") ? "&DnnGUID=" + HostSettings["GUID"] : "";
            return gsUrl;
        }

        [HttpGet]
        public List<string> WebAPiFiles(int appId)
        {
            var sxc = Request.GetSxcOfModuleContext(appId);
            var path = Path.Combine(sxc.App.PhysicalPath, "Api");
            if (Directory.Exists(path))
                return Directory.GetFiles(path, "*.cs")
                    .Select(Path.GetFileName)
                    .ToList<string>();

            return new List<string>();
        } 
        #endregion


    }
}