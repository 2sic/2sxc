using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Base class to provide data to the Sites DataSource.
    ///
    /// Must be overriden in each platform.
    /// </summary>
    public abstract class SitesDataSourceProvider: ServiceBase<SitesDataSourceProvider.MyServices>
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
                ConnectServices(
                    ZoneMapperLazy = zoneMapperLazy,
                    AppStates = appStates
                );
            }

        }
        
        protected SitesDataSourceProvider(MyServices services, string logName) : base(services, logName)
        {
        }

        /// <summary>
        /// So the core data source doesn't have settings to configure this
        /// </summary>
        /// <returns></returns>
        public abstract List<SiteDataRaw> GetSitesInternal();

        public int GetZoneId(int siteId) => Services.ZoneMapperLazy.Value.GetZoneId(siteId);

        public int GetDefaultAppId(int siteId) => Services.AppStates.DefaultAppId(GetZoneId(siteId));

        public int GetPrimaryAppId(int siteId) => Services.AppStates.PrimaryAppId(GetZoneId(siteId));

        public string GetLanguages(int siteId)
        {
            var languages = Services.AppStates
                .Languages(GetZoneId(siteId), true);
            return string.Join(",", languages.Select(l => l.EnvironmentKey.ToLower()));
            //return Deps.AppStates
            //    .Languages(GetZoneId(siteId), true)
            //    .ToDictionary(d => d.EnvironmentKey.ToLower(), d => d.Name);
        }
    }
}
