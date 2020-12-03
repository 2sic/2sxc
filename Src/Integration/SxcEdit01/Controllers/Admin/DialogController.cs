using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.WebApi.Admin;

namespace IntegrationSamples.SxcEdit01.Controllers.Admin
{
    [Route(Int01Constants.DefaultRouteSchema)]
    [ApiController]
    public class DialogController : Int01StatelessControllerBase
    {
        protected override string HistoryLogName => "Api.SysCnt";

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// Used to be GET System/DialogSettings
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public DialogContextStandalone Settings(int appId)
            => HttpContext.RequestServices.Build<AdminBackend>().Init(Log).DialogSettings(appId);

        #endregion
    }
}
