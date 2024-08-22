using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.MetadataDecorators;
using ToSic.Eav.Apps.Internal.Specs;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using static ToSic.Eav.Apps.Internal.AppConstants;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AppAssetThumbnail(IAppReader appReader, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    : AppAssetFile
{
    public override string Url => _url.Get(() => GetUrl(appReader, appPaths, globalPaths));
    private readonly GetOnce<string> _url = new();

    public static string GetUrl(IAppReader appReader, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
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