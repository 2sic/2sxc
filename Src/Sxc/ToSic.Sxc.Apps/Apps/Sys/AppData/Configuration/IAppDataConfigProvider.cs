namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// System which provides AppDataConfiguration, so that the client can know if it should show drafts etc.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(SxcAppBase app, AppDataConfigSpecs specs);

}