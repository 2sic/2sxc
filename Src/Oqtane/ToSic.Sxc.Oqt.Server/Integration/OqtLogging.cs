using System;
using ToSic.Sxc.Oqt.Shared.Dev;

namespace ToSic.Sxc.Oqt.Server.Integration;

internal static class OqtLogging
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