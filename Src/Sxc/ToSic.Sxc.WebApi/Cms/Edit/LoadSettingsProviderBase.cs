using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.WebApi.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class LoadSettingsProviderBase: ServiceBase
{
    protected LoadSettingsProviderBase(string logName) : base(logName)
    {
    }

    protected Dictionary<string, object> SettingsByKeys(PropertyStack appSettings, List<string> keys) => Log.Func(l =>
    {
        // Try to find each setting
        var settings = keys.ToDictionary(
            key => key,
            key => appSettings.InternalGetPath(key).Result
        );

        return (settings, $"{settings.Count}");
    });

}