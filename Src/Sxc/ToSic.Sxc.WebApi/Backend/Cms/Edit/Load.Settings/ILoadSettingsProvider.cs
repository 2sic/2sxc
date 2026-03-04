namespace ToSic.Sxc.Backend.Cms.Load.Settings;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ILoadSettingsProvider: IHasLog
{
    Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters);
}