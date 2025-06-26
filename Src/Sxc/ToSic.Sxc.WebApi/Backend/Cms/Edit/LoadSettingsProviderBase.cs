﻿using ToSic.Eav.Data.Sys.PropertyStack;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class LoadSettingsProviderBase(string logName) : ServiceBase(logName)
{
    protected Dictionary<string, object> SettingsByKeys(PropertyStack appSettings, List<string> keys)
    {
        var l = Log.Fn<Dictionary<string, object>>();
        // Try to find each setting
        var settings = keys.ToDictionary(
            key => key,
            key => appSettings.InternalGetPath(key).Result!
        );

        return l.Return(settings, $"{settings.Count}");
    }

}