using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Sys
{
    [Route(WebApiConstants.WebApiStateRoot + "/sys/[controller]/[action]")]
    public class LogController: OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Log";

        public LogController(StatefulControllerDependencies dependencies) : base(dependencies)
        { }

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
