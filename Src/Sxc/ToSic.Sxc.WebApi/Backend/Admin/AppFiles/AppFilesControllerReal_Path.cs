using ToSic.Sxc.Apps.Internal;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private string ResolveAppPath(int appId, bool global) =>
        (
            _appPaths ??= _appPathsFactoryTemp.Get(_appReaders.Get(appId), _site)
        )
        .PhysicalPathSwitch(global);
}