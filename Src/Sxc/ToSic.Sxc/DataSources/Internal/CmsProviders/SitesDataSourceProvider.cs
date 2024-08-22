using ToSic.Eav.Apps;
using ToSic.Eav.Integration;
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
    public class MyServices(LazySvc<IZoneMapper> zoneMapperLazy, IAppsCatalog appsCatalog)
        : MyServicesBase(connect: [zoneMapperLazy, appsCatalog])
    {
        public LazySvc<IZoneMapper> ZoneMapperLazy { get; } = zoneMapperLazy;
        public IAppsCatalog AppsCatalog { get; } = appsCatalog;
    }

    /// <summary>
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public abstract List<SiteDataRaw> GetSitesInternal();

    public int GetZoneId(int siteId) => Services.ZoneMapperLazy.Value.GetZoneId(siteId);

    public int GetDefaultAppId(int siteId) => Services.AppsCatalog.DefaultAppIdentity(GetZoneId(siteId)).AppId;

    public int GetPrimaryAppId(int siteId) => Services.AppsCatalog.PrimaryAppIdentity(GetZoneId(siteId)).AppId;

    public string GetLanguages(int siteId)
    {
        var languages = Services.AppsCatalog.Zone(GetZoneId(siteId)).Languages;
        return string.Join(",", languages.Select(l => l.EnvironmentKey.ToLower()));
        //return Deps.AppStates
        //    .Languages(GetZoneId(siteId), true)
        //    .ToDictionary(d => d.EnvironmentKey.ToLower(), d => d.Name);
    }
}