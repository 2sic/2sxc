using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi.Dnn;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class SystemController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sSysC");
        }


        [HttpGet]
	    public dynamic GetLanguages()
	    {
            Log.Add("get langs");
	        var portalId = PortalSettings.PortalId;
	        var zoneId = Env.ZoneMapper.GetZoneId(portalId);
	        var env = new Environment.DnnEnvironment();
	        // ReSharper disable once PossibleInvalidOperationException
	        var cultures = env.ZoneMapper.CulturesWithState(portalId, zoneId) 
	            .Select(c => new
	            {
	                Code = c.Key,
	                Culture = c.Text,
	                IsEnabled = c.Active
	            });

            Log.Add("langs - found:" + cultures.Count());
	        return cultures;
	    }

        /// <summary>
        /// Helper to prepare a quick-info about 1 content type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable)
	    {
            Log.Add($"switch language:{cultureCode}, to:{enable}");
            // Activate or Deactivate the Culture
	        var zoneId = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);

            var cultureText = LocaleController.Instance.GetLocale(cultureCode).Text;
            new ZoneManager(zoneId).SaveLanguage(cultureCode, cultureText, enable);
	    }


        #region Apps

        [HttpGet]
        public dynamic Apps(int zoneId)
        {
            var list = AppManagement.GetApps(zoneId, true, new PortalSettings(ActiveModule.OwnerPortalID));
            return list.Select(a => new
            {
                Id = a.AppId,
                IsApp = a.AppGuid != Eav.Constants.DefaultAppName,
                Guid = a.AppGuid,
                a.Name,
                a.Folder,
                AppRoot = GetPath(zoneId, a.AppId),
                IsHidden = a.Hidden,
                ConfigurationId = a.Configuration?.EntityId
            }).ToList();
        }

        private string GetPath(int zoneId, int appId)
        {
            var app = new App(zoneId, appId , PortalSettings);
            return app.Path;
        }

        [HttpGet]
        public void DeleteApp(int zoneId, int appId)
        {
            var userId = PortalSettings.Current.UserId;
            AppManagement.RemoveAppInDnnAndEav(zoneId, appId, PortalSettings, userId);
        }

        [HttpPost]
        public void App(int zoneId, string name)
        {
            AppManagement.AddBrandNewApp(zoneId, name, new PortalSettings(ActiveModule.OwnerPortalID));
        }

        #endregion

        #region Dialog Helpers
        /// <summary>
        /// This seems to be the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            App app = null;
            try
            {
                app = new App(PortalSettings.Current, appId);
            }
            catch (KeyNotFoundException) {}

            return new
            {
                IsContent = app?.AppGuid == "Default",
                Language = PortalSettings.Current.CultureCode,
                LanguageDefault = PortalSettings.Current.DefaultLanguage,
                GettingStartedUrl = app == null ? "" : IntroductionToAppUrl(app)
            };
        }

        // build a getting-started url which is used to correctly show the user infos like
        // warnings related to his dnn or 2sxc version
        // infos based on his languages
        // redirects based on the app he's looking at, etc.
        private string IntroductionToAppUrl(App app)
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

            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gsUrl += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            return gsUrl;
        }

        #endregion

        #region advanced logging

        [HttpGet]
        public string ExtendedLogging(int duration = 1)
        {
            Log.Add("Extended logging will set for duration:" + duration);
            var msg = Logging.ActivateForDuration(duration);
            Log.Add(msg);
            return msg;
        }

        #endregion
    }
}