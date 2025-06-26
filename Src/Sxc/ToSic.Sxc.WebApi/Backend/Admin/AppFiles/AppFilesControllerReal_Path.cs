using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Backend.Admin.AppFiles;

partial class AppFilesControllerReal
{
    private string ResolveAppPath(int appId, bool global) =>
        (
            _appPaths ??= appPathsFactoryTemp.Get(appReaders.Get(appId), site)
        )
        .PhysicalPathSwitch(global);
    private IAppPaths? _appPaths;

}