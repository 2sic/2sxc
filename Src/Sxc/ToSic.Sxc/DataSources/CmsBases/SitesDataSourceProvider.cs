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
    public abstract class SitesDataSourceProvider: ServiceBase<SitesDataSourceProvider.Dependencies>
    {
        public class Dependencies : ServiceDependencies
        {
            public LazySvc<IZoneMapper> ZoneMapperLazy { get; }
            public IAppStates AppStates { get; }

            /// <summary>
            /// Note that we will use Generators for safety, because in rare cases the dependencies could be re-used to create a sub-data-source
            /// </summary>
            public Dependencies(
                LazySvc<IZoneMapper> zoneMapperLazy,
                IAppStates appStates
            )
            {
                AddToLogQueue(
                    ZoneMapperLazy = zoneMapperLazy,
                    AppStates = appStates
                );
            }

        }
        
        private readonly Dependencies _dependencies;

        protected SitesDataSourceProvider(Dependencies dependencies, string logName) : base(dependencies, logName)
        {
            //ConnectServices(
            //    dependencies.ZoneMapperLazy, 
            //    dependencies.AppStates
            //);
            _dependencies = dependencies;
        }

        /// <summary>
        /// So the core data source doesn't have settings to configure this
        /// </summary>
        /// <returns></returns>
        public abstract List<CmsSiteInfo> GetSitesInternal();

        public int GetZoneId(int siteId) => _dependencies.ZoneMapperLazy.Value.GetZoneId(siteId);

        public int GetDefaultAppId(int siteId) => _dependencies.AppStates.DefaultAppId(GetZoneId(siteId));

        //public int GetContentAppId(int siteId) => _dependencies.AppStates.IdentityOfDefault(GetZoneId(siteId)).AppId;

        public int GetPrimaryAppId(int siteId) => _dependencies.AppStates.PrimaryAppId(GetZoneId(siteId));

        public Dictionary<string, string> GetLanguages(int siteId) => _dependencies.AppStates
            .Languages(GetZoneId(siteId), true).ToDictionary(d => d.EnvironmentKey.ToLower(), d => d.Name);
    }
}
