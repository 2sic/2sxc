namespace ToSic.Eav.Apps.Internal;

/// <summary>
/// System which provides AppDataConfiguration, so that the client can know if it should show drafts etc.
/// </summary>
public interface IAppDataConfigProvider
{
    public IAppDataConfiguration GetDataConfiguration(EavApp app, AppDataConfigSpecs specs);

}