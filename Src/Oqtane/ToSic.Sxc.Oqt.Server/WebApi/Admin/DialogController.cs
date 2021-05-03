using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //[DnnLogExceptions]
    [Authorize(Roles = RoleNames.Admin)]
    //[AutoValidateAntiforgeryToken]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/dialog/[action]")]

    [ApiController]

    public class DialogController : OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.SysCnt";
        public DialogController(StatefulControllerDependencies dependencies) : base()
        {
        }

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// Used to be GET System/DialogSettings
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public DialogContextStandalone Settings(int appId)
        {
            return HttpContext.RequestServices.Build<AdminBackend>().Init(Log)
                .DialogSettings(appId);
        }

        #endregion

    }
}