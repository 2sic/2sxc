using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Web.Http;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Eav.WebApi.Admin.AppInternalsControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the AppInternalsController (Web API Controller)
    /// </summary>
    [DnnLogExceptions]
	public class AppInternalsController : SxcApiControllerBase, IAppInternalsController
	{
        public AppInternalsController(): base(RealController.LogSuffix) { }

        private RealController Real => SysHlp.GetService<RealController>();

        /// <inheritdoc/>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public AppInternalsDto Get(int appId, int targetType, string keyType, string key) 
            => Real.Get(appId, targetType, keyType, key);
    }
}
