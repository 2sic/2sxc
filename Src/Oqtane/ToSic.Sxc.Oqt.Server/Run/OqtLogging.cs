using System;
using System.Collections.Concurrent;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Repository;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtLogging
    {
        /// <summary>
        /// Activate extended logging for a specific duration
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string ActivateForDuration(int duration)
        {
            WipConstants.DontDoAnythingImplementLater();

            //var prop = GlobalConfiguration.Configuration.Properties;
            //prop.GetOrAdd(Constants.AdvancedLoggingEnabledKey, duration > 0);
            var timeout = DateTime.Now.AddMinutes(duration);
            //prop.AddOrUpdate(Constants.AdvancedLoggingTillKey, timeout, (a, b) => timeout);
            return $"test-test Extended logging activated for {duration} minutes to {timeout}";
        }
    }
}
