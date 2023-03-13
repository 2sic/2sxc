using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    public partial class ContextResolver
    {

        //public IContextOfApp App(int appId)
        //{
        //    var appCtx = _contextOfApp.New();
        //    appCtx.ResetApp(appId);
        //    LatestAppContext = appCtx;
        //    return appCtx;
        //}

        //public IContextOfApp App(string nameOrPath) => App(AppIdResolver.Value.GetAppIdFromPath(Site().Site.ZoneId, nameOrPath, true));

        public IContextOfApp SetAppOrNull(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
            var zoneId = Site().Site.ZoneId;
            var id = AppIdResolver.Value.GetAppIdFromPath(zoneId, nameOrPath, false);
            return id <= Eav.Constants.AppIdEmpty ? null : SetApp(new AppIdentity(zoneId, id));
        }
    }
}
