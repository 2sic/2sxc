using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sys.Caching.PiggyBack;
using static ToSic.Eav.Apps.Sys.AppConstants;

namespace ToSic.Sxc.Apps.Sys.Assets;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AppAssetThumbnail(IAppReader appReader, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    : AppAssetFile
{
    public override string? Url => _url.Get(() => GetUrl(appReader, appPaths, globalPaths));
    private readonly GetOnce<string?> _url = new();

    public static string? GetUrl(IAppReader appReader, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    {
        // Primary app - we only PiggyBack cache the icon in this case
        // Because otherwise the icon could get moved, and people would have a hard time seeing the effect
        if (appReader.Specs.IsSiteSettingsApp())
            return appReader.GetCache().GetPiggyBack("app-thumbnail-primary",
                () => globalPaths.Value.GlobalPathTo(AppPrimaryIconFile, PathTypes.Link));

        // standard app (not global) try to find app-icon in its (portal) app folder
        if (!appReader.IsShared() && File.Exists($"{appPaths.PhysicalPath}/{AppIconFile}"))
            return $"{appPaths.Path}/{AppIconFile}";

        // global app (and standard app without app-icon in its portal folder) looks for app-icon in global shared location 
        if (File.Exists($"{appPaths.PhysicalPathShared}/{AppIconFile}"))
            return $"{appPaths.PathShared}/{AppIconFile}";

        return null;
    }
}