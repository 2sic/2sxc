using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Lib.Services;
using App = ToSic.Sxc.Apps.App;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Run
{
    public abstract class ImportExportEnvironmentBase: Eav.Apps.Run.ImportExportEnvironmentBase
    {
        #region constructor / DI

        public class Dependencies: ServiceDependencies
        {
            internal readonly AppPaths AppPaths;
            internal readonly IAppStates AppStates;
            internal readonly ISite Site;
            internal readonly App NewApp;

            public Dependencies(ISite site, App newApp, IAppStates appStates, AppPaths appPaths)
            {
                AddToLogQueue(
                    AppPaths = appPaths,
                    AppStates = appStates,
                    Site = site,
                    NewApp = newApp
                );
            }
        }


        /// <summary>
        /// DI Constructor
        /// </summary>
        protected ImportExportEnvironmentBase(Dependencies dependencies, string logName) : base(dependencies.Site, dependencies.AppStates, logName)
        {
            _dependencies = dependencies.SetLog(Log);
        }

        private readonly Dependencies _dependencies;

        #endregion

        public override string FallbackContentTypeScope => Scopes.Default;

        public override string TemplatesRoot(int zoneId, int appId) 
            => AppPaths(zoneId, appId).PhysicalPath;

        public override string GlobalTemplatesRoot(int zoneId, int appId) 
            => AppPaths(zoneId, appId).PhysicalPathShared;

        private AppPaths AppPaths(int zoneId, int appId) =>
            _appPaths ?? (_appPaths =_dependencies.AppPaths.Init(_dependencies.Site,
                _dependencies.AppStates.Get(new AppIdentity(zoneId, appId))));
        private AppPaths _appPaths;


    }
}
