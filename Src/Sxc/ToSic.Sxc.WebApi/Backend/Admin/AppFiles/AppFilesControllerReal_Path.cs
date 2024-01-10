using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Internal;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private string ResolveAppPath(int appId, bool global) =>
        (
            _appPaths.InitDone
                ? _appPaths
                : _appPaths.Init(_site, _appStates.GetReader(appId))
        )
        .PhysicalPathSwitch(global);
}