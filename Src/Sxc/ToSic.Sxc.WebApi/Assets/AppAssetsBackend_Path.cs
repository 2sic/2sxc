using ToSic.Sxc.Apps;

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
