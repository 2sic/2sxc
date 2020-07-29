using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Security;
using ToSic.Sxc.WebApi.Context;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [SxcWebApiExceptionHandling]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public partial class SystemController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sSysC");
        }

        [HttpGet]
        // todo: deprecate PARAMS find out if / where used
        public dynamic GetLanguages()
	    {
            Log.Add("get languages");
	        var portalId = PortalSettings.PortalId;
	        var zoneId = Env.ZoneMapper.GetZoneId(portalId);
	        // ReSharper disable once PossibleInvalidOperationException
	        var cultures = Env.ZoneMapper.CulturesWithState(portalId, zoneId) 
	            .Select(c => new
	            {
	                Code = c.Key,
	                Culture = c.Text,
	                IsEnabled = c.Active
	            });

            Log.Add("languages - found:" + cultures.Count());
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
            new ZoneManager(zoneId, Log).SaveLanguage(cultureCode, cultureText, enable);
	    }


        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic DialogSettings(int appId)
        {
            IApp app = null;
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appId != 0)
            {
                var appAndPerms = new MultiPermissionsApp(BlockBuilder, appId, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var exp))
                    throw exp;
                app = appAndPerms.App;
            }

            var cb = new ContextBuilder(PortalSettings.Current, 
                Request.FindModuleInfo(),
                UserInfo,
                app?.ZoneId,
                app);

            return new
            {
                // TODO: Deprecate PARAMS these properties as soon as old UI is gone
                IsContent = app?.AppGuid == "Default",
                Language = PortalSettings.Current.CultureCode,
                LanguageDefault = PortalSettings.Current.DefaultLanguage,
                AppPath = app?.Path,
                GettingStartedUrl = cb.GettingStartedUrl(),
                // END TODO
                Context = cb.Get(Ctx.All),
            };
        }

        #region Features
        [HttpGet]
        public IEnumerable<Feature> Features(bool reload = false)
        {
            if(reload)
                Eav.Configuration.Features.Reset();
            return Eav.Configuration.Features.All;
        }

        [HttpGet]
        public string ManageFeaturesUrl()
        {
            if (!UserInfo.IsSuperUser)
            {
                // throw new AccessViolationException("error: user needs host permissions");
                return "error: user needs host permissions";
            }

            return "//gettingstarted.2sxc.org/router.aspx?"
                + $"DnnVersion={DotNetNukeContext.Current.Application.Version.ToString(4)}"
                + $"&2SexyContentVersion={Settings.ModuleVersion}"
                + $"&fp={HttpUtility.UrlEncode(Fingerprint.System)}"
                + $"&DnnGuid={DotNetNuke.Entities.Host.Host.GUID}"
                + $"&ModuleId={Request.FindModuleInfo().ModuleID}" // needed for callback later on
                + "&destination=features";
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool SaveFeatures([FromBody] FeaturesManagementUtils.FeaturesManagementResponse featuresManagementResponse)
        {
            // first do a validity check 
            if (featuresManagementResponse?.Msg?.Features == null) return false;

            // 1. valid json? 
            // - ensure signature is valid
            if (!FeaturesManagementUtils.IsValidJson(featuresManagementResponse.Msg.Features)) return false;

            // then take the newFeatures (it should be a json)
            // and save to /desktopmodules/.data-custom/configurations/features.json
            if (!FeaturesManagementUtils.SaveFeature(featuresManagementResponse.Msg.Features)) return false;

            // when done, reset features
            Eav.Configuration.Features.Reset();

            return true;
        }

        #endregion



        #endregion

        #region Enable extended logging

        [HttpGet]
        public string ExtendedLogging(int duration = 1)
        {
            Log.Add("Extended logging will set for duration:" + duration);
            var msg = DnnLogging.ActivateForDuration(duration);
            Log.Add(msg);
            return msg;
        }

        #endregion
    }
}