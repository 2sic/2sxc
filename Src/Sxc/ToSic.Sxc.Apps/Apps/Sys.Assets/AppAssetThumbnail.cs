using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;

namespace ToSic.Sxc.Apps.Sys.Assets;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class AppAssetThumbnail(IAppReader appReader, IAppPaths appPaths, LazySvc<GlobalPaths> globalPaths)
    : AppAssetFile
{
    public override string? Url => _url.Get(() => AppThumbnailHelper.GetUrl(appReader, appPaths, globalPaths));
    private readonly LazyGet<string?> _url = new();

}