using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
    public partial class App
    {
        public IApp Init(int appId, IBlock optionalBlock = null)
        {
            var appStates = _appStates.New();
            var appIdentity = appStates.IdentityOfApp(appId);
            var confProvider = _appConfigDelegate.New();
            var buildConfig = optionalBlock == null
                ? confProvider.Build()
                : confProvider.Build(optionalBlock);
            return Init(appIdentity, buildConfig);
        }
    }
}
