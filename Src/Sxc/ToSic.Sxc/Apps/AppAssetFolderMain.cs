using ToSic.Eav.Apps.Integration;

namespace ToSic.Sxc.Apps;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AppAssetFolderMain(IAppPaths appPaths, string folder, bool shared) : AppAssetFolder
{
    internal const string LocationSite = "site";
    internal const string LocationShared = "shared";
    internal const string LocationAuto = "auto";

    /// <summary>
    /// Return true/false or null to allow upstream to do auto-detect
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal static bool? DetermineShared(string location)
    {
        switch (location?.ToLowerInvariant() ?? LocationAuto)
        {
            case LocationAuto: return null;
            case LocationShared: return true;
            case LocationSite: return false;
            default: throw new ArgumentException($@"should be null, {LocationAuto}, {LocationSite} or {LocationShared}", nameof(location));
        }
    }


    internal readonly IAppPaths AppPaths = appPaths;

    public override string Name { get; } = folder;

    public override string Path => shared ? AppPaths.RelativePathShared : AppPaths.RelativePath;

    public override string PhysicalPath => (shared ? AppPaths.PhysicalPathShared : AppPaths.PhysicalPath);

    public override string Url => shared ? AppPaths.PathShared : AppPaths.Path;


}