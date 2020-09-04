using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
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
            ITenant tenant,
            bool publishingEnabled,
            bool showDrafts,
            ILog parentLog)
        {
            var log = new Log("Mvc.Factry", parentLog);
            log.Add($"Create App(z:{zoneId}, a:{appId}, tenantObj:{tenant != null}, publishingEnabled: {publishingEnabled}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
            var appStuff = Eav.Factory.Resolve<App>().Init(new AppIdentity(zoneId, appId),
                ConfigurationProvider.Build(showDrafts, publishingEnabled, new LookUpEngine(parentLog)),
                true, parentLog);
            return appStuff;
        }
    }
}
