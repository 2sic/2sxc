using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Mvc
{
    public class Factory
    {
        [InternalApi_DoNotUse_MayChangeWithoutNotice]
        public static IApp App(
            int zoneId,
            int appId,
            ISite site,
            bool showDrafts,
            ILog parentLog)
        {
            var log = new Log("Mvc.Factry", parentLog);
            log.A($"Create App(z:{zoneId}, a:{appId}, tenantObj:{site != null}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
            var app = Eav.Factory.StaticBuild<App>();
            if (site != null) app.PreInit(site);

            // debugging
            var appIdentity = new AppIdentity(zoneId, appId);
            var configDelegate = Eav.Factory.StaticBuild<AppConfigDelegate>().Init(parentLog).Build(showDrafts);

            var appStuff = app.Init(appIdentity, configDelegate, parentLog);

            return appStuff;
        }
    }
}
