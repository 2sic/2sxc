namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class LoadSettingsProviderBase(string logName) : ServiceBase(logName)
{
    protected Dictionary<string, object> SettingsByKeys(PropertyStack appSettings, List<string> keys)
    {
        var l = Log.Fn<Dictionary<string, object>>();
        // Try to find each setting
        var settings = keys.ToDictionary(
            key => key,
            key => appSettings.InternalGetPath(key).Result
        );

        return l.Return(settings, $"{settings.Count}");
    }

}