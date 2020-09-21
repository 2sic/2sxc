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