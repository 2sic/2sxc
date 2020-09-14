using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using DotNetNuke.Application;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Features;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <inheritdoc cref="ISystemController" />
    [SupportedModules("2sxc,2sxc-app")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public partial class SystemController : ISystemController
    {
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Feature> Features(bool reload = false) => 
            Factory.Resolve<FeaturesBackend>().Init(Log).GetAll(reload);

        // 2020-09-14 2dm disabled this - don't think it's in use
        //[HttpGet]
        //public IEnumerable<Feature> Features(int appId)
        //{
        //    return Factory.Resolve<FeaturesBackend>().Init(Log).Features(GetContext(), PortalSettings.PortalId, appId);

        //    //// some dialogs don't have an app-id yet, because no app has been selected
        //    //// in this case, use the app-id of the content-app for feature-permission check
        //    //if (appId == Eav.Constants.AppIdEmpty)
        //    //{
        //    //    var environment = Factory.Resolve<IAppEnvironment>().Init(Log);
        //    //    var zoneId = environment.ZoneMapper.GetZoneId(PortalSettings.PortalId);
        //    //    appId = new ZoneRuntime(zoneId, Log).DefaultAppId;
        //    //}

        //    //return FeaturesHelpers.FeatureListWithPermissionCheck(new MultiPermissionsApp().Init(GetContext(), GetApp(appId), Log));
        //}

        [HttpGet]
        public string ManageFeaturesUrl()
        {
            if (!UserInfo.IsSuperUser) return "error: user needs SuperUser permissions";

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
        public bool SaveFeatures([FromBody] FeaturesDto featuresManagementResponse) => 
            Factory.Resolve<FeaturesBackend>().Init(Log).SaveFeatures(featuresManagementResponse);
    }
}