﻿using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
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
            ISite site,
            bool showDrafts,
            ILog parentLog)
        {
            var log = new Log("Mvc.Factry", parentLog);
            log.Add($"Create App(z:{zoneId}, a:{appId}, tenantObj:{site != null}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
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
