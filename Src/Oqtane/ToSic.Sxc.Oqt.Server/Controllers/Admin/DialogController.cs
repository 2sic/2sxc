using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //   [DnnLogExceptions]
    //   [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/dialog/[action]")]
    [ApiController]

    [ValidateAntiForgeryToken]
    public class DialogController : SxcStatefulControllerBase
    {
        private readonly OqtaneContextBuilder _contextBuilder;
        protected override string HistoryLogName => "Api.SysCnt";
        public DialogController(StatefulControllerDependencies dependencies, OqtaneContextBuilder contextBuilder) : base(dependencies)
        {
            _contextBuilder = contextBuilder;
        }

        #region Dialog Helpers
        /// <summary>
        /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
        /// Used to be GET System/DialogSettings
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic Settings(int appId)
        {
            var block = GetBlock();
            return new AdminBackend().Init(Log).DialogSettings(GetContext(), _contextBuilder.Init(block), appId);
        }

        #endregion

    }
}