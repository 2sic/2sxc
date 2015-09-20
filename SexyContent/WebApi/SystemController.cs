using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.UI.WebControls;
using ToSic.Eav;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class SystemController : SxcApiController
	{

	    [HttpGet]
	    public dynamic GetLanguages()
	    {
	        var portalId = Dnn.Portal.PortalId;
            var zoneId = SexyContent.GetZoneID(portalId);
            var cultures = SexyContent.GetCulturesWithActiveState(portalId, zoneId.Value).Select(c => new
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
            Sexy.SetCultureState(cultureCode, enable, Dnn.Portal.PortalId);
	    }


        #region Apps

        [HttpGet]
        public dynamic Apps(int zoneId)
        {
            var list = SexyContent.GetApps(zoneId, true, new PortalSettings(Dnn.Module.OwnerPortalID));
            return list.Select(a => new
            {
                Id = a.AppId,
                IsApp = a.AppGuid != "Default",
                Guid = a.AppGuid,
                a.Name,
                a.Folder,
                IsHidden = a.Hidden,
                //Tokens = a.Settings?.AllowTokenTemplates ?? false,
                //Razor = a.Configuration?.AllowRazorTemplates ?? false,
                ConfigurationId = a.Configuration?.EntityId ?? null
            }).ToList();
        }

        [HttpGet]
        public void DeleteApp(int zoneId, int appId)
        {
            var sexy = new SexyContent(zoneId, appId, false);
            var userId = PortalSettings.Current.UserId;
            sexy.RemoveApp(appId, userId);
        }

        [HttpPost]
        public void App(int zoneId, string name)
        {
            SexyContent.AddApp(zoneId, name, new PortalSettings(Dnn.Module.OwnerPortalID));
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
            var gsUrl = "http://gettingstarted.2sexycontent.org/router.aspx?"

                        // Add version & module infos
                        + "DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                        + "&2SexyContentVersion=" + SexyContent.ModuleVersion
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