using ToSic.Eav.Apps;
using ToSic.Eav.Integration;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the Sites DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class SitesDataSourceProvider(SitesDataSourceProvider.MyServices services, string logName)
    : ServiceBase<SitesDataSourceProvider.MyServices>(services, logName)
{
    public class MyServices : MyServicesBase
    {
        public LazySvc<IZoneMapper> ZoneMapperLazy { get; }
        public IAppStates AppStates { get; }

        public MyServices(
            LazySvc<IZoneMapper> zoneMapperLazy,
            IAppStates appStates
        )
        {
            ConnectLogs([
                ZoneMapperLazy = zoneMapperLazy,
                AppStates = appStates
            ]);
        }

    }

    /// <summary>
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public abstract List<SiteDataRaw> GetSitesInternal();

    public int GetZoneId(int siteId) => Services.ZoneMapperLazy.Value.GetZoneId(siteId);

    public int GetDefaultAppId(int siteId) => Services.AppStates.AppsCatalog.DefaultAppIdentity(GetZoneId(siteId)).AppId;

    public int GetPrimaryAppId(int siteId) => Services.AppStates.AppsCatalog.PrimaryAppIdentity(GetZoneId(siteId)).AppId;

    public string GetLanguages(int siteId)
    {
        var languages = Services.AppStates.AppsCatalog.Zone(GetZoneId(siteId)).Languages;
        return string.Join(",", languages.Select(l => l.EnvironmentKey.ToLower()));
        //return Deps.AppStates
        //    .Languages(GetZoneId(siteId), true)
        //    .ToDictionary(d => d.EnvironmentKey.ToLower(), d => d.Name);
    }
}