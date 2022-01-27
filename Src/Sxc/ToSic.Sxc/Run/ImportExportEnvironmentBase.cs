using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps.Paths;
using App = ToSic.Sxc.Apps.App;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Run
{
    public abstract class ImportExportEnvironmentBase: Eav.Apps.Run.ImportExportEnvironmentBase
    {
        #region constructor / DI

        public class Dependencies
        {
            internal readonly AppPaths AppPaths;
            internal readonly IAppStates AppStates;
            internal readonly ISite Site;
            internal readonly App NewApp;

            public Dependencies(ISite site, App newApp, IAppStates appStates, AppPaths appPaths)
            {
                AppPaths = appPaths;
                AppStates = appStates;
                Site = site;
                NewApp = newApp;
            }
        }


        /// <summary>
        /// DI Constructor
        /// </summary>
        protected ImportExportEnvironmentBase(Dependencies dependencies, string logName) : base(dependencies.Site, dependencies.AppStates, logName) 
            => _dependencies = dependencies;

        private readonly Dependencies _dependencies;

        #endregion

        public override string FallbackContentTypeScope => Scopes.Default;

        public override string TemplatesRoot(int zoneId, int appId)
        {
            //var app = _dependencies.NewApp.InitNoData(new AppIdentity(zoneId, appId), Log);

            //// Copy all files in 2sexy folder to (portal file system) 2sexy folder
            //var templateRoot = app.PhysicalPath; //.PhysicalPathSwitch(false);
            //return templateRoot;
            return AppPaths(zoneId, appId).PhysicalPath; 
        }

        public override string GlobalTemplatesRoot(int zoneId, int appId)
        {
            //var app = _dependencies.NewApp.InitNoData(new AppIdentity(zoneId, appId), Log);
            //return app.PhysicalPathShared;
            return AppPaths(zoneId, appId).PhysicalPathShared;
        }

        private AppPaths AppPaths(int zoneId, int appId)
        {
            // only build once
            return _appPaths ?? (_appPaths =_dependencies.AppPaths.Init(_dependencies.Site,
                _dependencies.AppStates.Get(new AppIdentity(zoneId, appId)), Log));
        }

        private AppPaths _appPaths;


    }
}
