namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ILoadSettingsProvider: IHasLog
{
    Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters);
}