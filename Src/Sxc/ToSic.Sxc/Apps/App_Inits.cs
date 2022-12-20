using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Apps
{
    public partial class App
    {
        public IApp Init(int appId, ILog log, IBlock optionalBlock = null, bool showDrafts = false)
        {
            var appStates = _appStates.New();
            var appIdentity = appStates.IdentityOfApp(appId);
            var confProvider = _appConfigDelegate.New().Init(log);
            var buildConfig = (optionalBlock == null)
                ? confProvider.Build(showDrafts)
                : confProvider.Build(optionalBlock);
            return Init(appIdentity, buildConfig, log);
        }
    }
}
