using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Paths;

namespace ToSic.Sxc.WebApi.Assets
{
    public partial class AppAssetsBackend
    {
        private string ResolveAppPath(int appId, bool global) =>
            (_appPaths.InitDone
                ? _appPaths
                : _appPaths.Init(_site, _appStates.Get(appId), Log))
            .PhysicalPathSwitch(global);
    }
}
