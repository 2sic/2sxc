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

        public class MyServices: MyServicesBase
        {
            internal readonly AppPaths AppPaths;
            internal readonly IAppStates AppStates;
            internal readonly ISite Site;
            internal readonly App NewApp;

            public MyServices(ISite site, App newApp, IAppStates appStates, AppPaths appPaths)
            {
                ConnectServices(
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
        protected ImportExportEnvironmentBase(MyServices services, string logName) : base(services.Site, services.AppStates, logName)
        {
            _services = services.SetLog(Log);
        }

        private readonly MyServices _services;

        #endregion

        public override string FallbackContentTypeScope => Scopes.Default;

        public override string TemplatesRoot(int zoneId, int appId) 
            => AppPaths(zoneId, appId).PhysicalPath;

        public override string GlobalTemplatesRoot(int zoneId, int appId) 
            => AppPaths(zoneId, appId).PhysicalPathShared;

        private AppPaths AppPaths(int zoneId, int appId) =>
            _appPaths ?? (_appPaths =_services.AppPaths.Init(_services.Site,
                _services.AppStates.Get(new AppIdentity(zoneId, appId))));
        private AppPaths _appPaths;


    }
}
