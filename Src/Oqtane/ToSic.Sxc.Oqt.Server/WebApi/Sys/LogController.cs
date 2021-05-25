using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/sys/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/sys/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/sys/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/sys/[controller]/[action]")]

    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    public class LogController: OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Log";

        #region Enable extended logging

        /// <summary>
        /// Used to be GET System/ExtendedLogging
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        [HttpGet]
        public string EnableDebug([FromQuery] int duration = 1)
        {
            Log.Add("Extended logging will set for duration:" + duration);
            var msg = OqtLogging.ActivateForDuration(duration);
            Log.Add(msg);
            return msg;
        }

        #endregion
    }
}
