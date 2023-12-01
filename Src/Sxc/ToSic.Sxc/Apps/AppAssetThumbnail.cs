using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Decorators;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using static ToSic.Eav.Apps.AppConstants;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AppAssetThumbnail : AppAssetFile
{
    private readonly IAppPaths _appPaths;
    private readonly IAppState _appState;
    private readonly LazySvc<GlobalPaths> _globalPaths;

    public AppAssetThumbnail(IAppState appState, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    {
        _appPaths = appPaths;
        _appState = appState;
        _globalPaths = globalPaths;
    }

    public override string Url => _url.Get(() => GetUrl(_appState, _appPaths, _globalPaths));
    private readonly GetOnce<string> _url = new();

    public static string GetUrl(IAppState appState, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    {
        // Primary app - we only PiggyBack cache the icon in this case
        // Because otherwise the icon could get moved, and people would have a hard time seeing the effect
        if (appState.NameId == Eav.Constants.PrimaryAppGuid)
            return appState.Internal().GetPiggyBack("app-thumbnail-primary",
                () => globalPaths.Value.GlobalPathTo(AppPrimaryIconFile, PathTypes.Link));

        // standard app (not global) try to find app-icon in its (portal) app folder
        if (!appState.IsShared() && File.Exists($"{appPaths.PhysicalPath}/{AppIconFile}"))
            return $"{appPaths.Path}/{AppIconFile}";

        // global app (and standard app without app-icon in its portal folder) looks for app-icon in global shared location 
        if (File.Exists($"{appPaths.PhysicalPathShared}/{AppIconFile}"))
            return $"{appPaths.PathShared}/{AppIconFile}";

        return null;
    }
}