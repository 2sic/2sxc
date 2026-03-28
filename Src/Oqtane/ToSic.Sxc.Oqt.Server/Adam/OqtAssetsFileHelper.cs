using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Models;
using Oqtane.Shared;
using System.Text.RegularExpressions;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;
using File = System.IO.File;

namespace ToSic.Sxc.Oqt.Server.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class OqtAssetsFileHelper(OqtSiteGroup siteGroup = null) : ServiceBase(OqtConstants.OqtLogPrefix + ".FilHlp")
{
    public const string RouteAdam = "adam";
    public const string RouteAssets = "assets";
    public const string RouteShared = "shared";

    public static readonly Regex RiskyDetector = Eav.Security.Files.FileNames.RiskyDownloadDetector;

    public const string FallbackMimeType = MimeTypeConstants.FallbackType;


    public static string GetMimeType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return FallbackMimeType;
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType)) 
            contentType = FallbackMimeType;
        return contentType;
    }

    public string GetFilePath(string contentRootPath, Alias alias, string filePath) 
        => GetFilePath(contentRootPath, alias, string.Empty,  string.Empty, filePath);

    public string GetFilePath(string contentRootPath, Alias alias, string route, string appName, string filePath)
    {
        var l = Log.Fn<string>(
            $"{nameof(contentRootPath)}: '{contentRootPath}'; {nameof(route)}: {route}; {nameof(appName)}: '{appName}'; {nameof(filePath)}: '{filePath}'");
            
        // Validate for alias.
        if (alias == null) 
            return l.Return(string.Empty, "no site alias");

        // Oqtane path and file name validation.
        // Partly commented because Path validation is not working as expected.
        if (!appName.IsPathOrFileValid()) 
            return l.Return(string.Empty, "not valid");

        // Blacklist extensions should be denied.
        if (IsKnownRiskyExtension(filePath))
            return l.Return(string.Empty, "risky extension");

        if (Eav.Security.Files.FileNames.IsKnownCodeExtension(filePath))
            return l.Return(string.Empty, "code extension");

        // Nothing in a ".xyz" folder or a subfolder of this should be allowed (like .data must be protected).
        if (StartsWithDot(appName) || StartsWithDot(filePath) || HasHiddenFolderSegment(filePath))
            return l.Return(string.Empty, "folders or subfolder that start with . are not allowed");

        var siteId = ContentSiteId(alias);
        var fullFilePath = route switch
        {
            "" => AdamPathWithoutAppName(contentRootPath, alias, siteId, filePath),
            RouteAdam => AdamPath(contentRootPath, alias, siteId, appName, filePath),
            RouteAssets => SxcPath(contentRootPath, alias, siteId, appName, filePath),
            RouteShared => SharedPath(contentRootPath, alias, appName, filePath),

            _ => SxcPath(contentRootPath, alias, siteId, appName, filePath),
        };

        if (File.Exists(fullFilePath))
            return l.Return(fullFilePath, "found");

        return l.Return(string.Empty, "file not found");
    }

    private static bool IsKnownRiskyExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension) && RiskyDetector.IsMatch(extension);
    }

    // Path.GetDirectoryName gives the parent path; split it to catch nested
    // protected folders such as .data regardless of slash direction.
    private static bool HasHiddenFolderSegment(string filePath)
        => (Path.GetDirectoryName(filePath) ?? string.Empty)
            .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
            .Any(StartsWithDot);

    private static bool StartsWithDot(string value)
        => value != null && value.StartsWith(".", StringComparison.Ordinal);

    private int ContentSiteId(Alias alias)
        => siteGroup?.GetPrimaryLocalizationSiteId(alias.SiteId) ?? alias.SiteId;

    private static string AdamPathWithoutAppName(string contentRootPath, Alias alias, int siteId, string filePath)
        => BuildPhysicalPath(contentRootPath, string.Format(OqtConstants.ContentRootPublicBase, alias.TenantId, siteId), filePath);

    private static string AdamPath(string contentRootPath, Alias alias, int siteId, string appName, string filePath)
        => BuildPhysicalPath(contentRootPath, string.Format(OqtConstants.ContentRootPublicBase, alias.TenantId, siteId), RouteAdam, appName, filePath);

    private static string SxcPath(string contentRootPath, Alias alias, int siteId, string appName, string filePath)
        => BuildPhysicalPath(contentRootPath, string.Format(OqtConstants.AppRootTenantSiteBase, alias.TenantId, siteId), appName, filePath);

    private static string SharedPath(string contentRootPath, Alias alias, string appName, string filePath)
        => BuildPhysicalPath(contentRootPath, string.Format(OqtConstants.AppRootTenantSiteBase, alias.TenantId, OqtConstants.SharedAppFolder), appName, filePath);

    private static string BuildPhysicalPath(params string[] segments)
        => Path.GetFullPath(Path.Combine(segments));

}
