using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Sxc.Engines;
using App = ToSic.Sxc.Apps.App;

namespace ToSic.Sxc.Run
{
    public abstract class ImportExportEnvironmentBase: Eav.Apps.Run.ImportExportEnvironmentBase
    {

        #region constructor / DI

        public class Dependencies
        {
            internal readonly IAppStates AppStates;
            internal readonly ISite Site;
            internal readonly App NewApp;
            internal readonly AppPathHelpers AppPathHelpers;

            public Dependencies(ISite site, App newApp, AppPathHelpers appPathHelpers, IAppStates appStates)
            {
                AppStates = appStates;
                Site = site;
                NewApp = newApp;
                AppPathHelpers = appPathHelpers;
            }
        }

        private readonly Dependencies _dependencies;

        /// <summary>
        /// DI Constructor
        /// </summary>
        protected ImportExportEnvironmentBase(Dependencies dependencies, string logName) : base(dependencies.Site, dependencies.AppStates, logName)
        {
            _dependencies = dependencies;
        }

        #endregion

        public override string FallbackContentTypeScope => Scopes.Default;

        public override string TemplatesRoot(int zoneId, int appId)
        {
            var app = _dependencies.NewApp.InitNoData(new AppIdentity(zoneId, appId), Log);

            // Copy all files in 2sexy folder to (portal file system) 2sexy folder
            var templateRoot = _dependencies.AppPathHelpers.Init(app, Log)
                .AppPathRoot(false, PathTypes.PhysFull);
            return templateRoot;
        }

        public override string GlobalTemplatesRoot(int zoneId, int appId)
        {
            var app = _dependencies.NewApp.InitNoData(new AppIdentity(zoneId, appId), Log);
            return app.PhysicalPathShared;
        }

    }
}
