using System;
using System.Collections.Generic;
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

        [HttpDelete]
        public new void App(int zoneId, int appId)
        {
            
            var sexy = new SexyContent(zoneId, appId, false);

            sexy.RemoveApp(appId, Dnn.User.UserID);
        }

        [HttpPost]
        public new void App(int zoneId, string name)
        {
            SexyContent.AddApp(zoneId, name, new PortalSettings(Dnn.Module.OwnerPortalID));
        }

        #endregion

        #region Dialog Helpers

        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            return new
            {
                gettingStartedUrl = GettingStartedUrl(appId)
            };
        }

        private string GettingStartedUrl(int appId)
        {
            //var appId = SexyContent.GetAppIdFromModule(Dnn.Module);
//             var zoneId = SexyContent.GetZoneID(Dnn.Module.PortalID);
            // var Sexy = new SexyContent(zoneId.Value, appId, true, Dnn.Module.OwnerPortalID);
            var gsUrl = "http://gettingstarted.2sexycontent.org/router.aspx?"

                        // Add version & module infos
                        + "DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                        + "&2SexyContentVersion=" + SexyContent.ModuleVersion
                        + "&ModuleName=" + Dnn.Module.DesktopModule.ModuleName
                        + "&ModuleId=" + Dnn.Module.ModuleID
                        + "&PortalID=" + Dnn.Portal.PortalId
                        + "&ZoneID=" + Sexy.App.ZoneId
                        + "&DefaultLanguage=" + PortalSettings.Current.DefaultLanguage
                        + "&CurrentLanguage=" + PortalSettings.Current.CultureCode;
            ;
            // Add AppStaticName and Version
            if (Dnn.Module.DesktopModule.ModuleName != "2sxc")
            {
                var app = Sexy.App;// SexyContent.GetApp(sexy, appId.Value, Sexy.OwnerPS);

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
        #endregion


    }
}