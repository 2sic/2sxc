using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtAssetsFileHelper() : ServiceBase(OqtConstants.OqtLogPrefix + ".FilHlp")
{
    public const string RouteAdam = "adam";
    public const string RouteAssets = "assets";
    public const string RouteShared = "shared";

    public static readonly Regex RiskyDetector = Eav.Security.Files.FileNames.RiskyDownloadDetector;

    public const string FallbackMimeType = MimeHelper.FallbackType;


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
        if (appName.StartsWith(".") || filePath.StartsWith(".") || Path.GetDirectoryName(filePath).Backslash().Contains(@"\.")) 
            return l.Return(string.Empty, "folders or subfolder that start with . are not allowed");

        var fullFilePath = route switch
        {
            "" => AdamPathWithoutAppName(contentRootPath, alias, filePath),
            RouteAdam => AdamPath(contentRootPath, alias, appName, filePath),
            RouteAssets => SxcPath(contentRootPath, alias, appName, filePath),
            RouteShared => SharedPath(contentRootPath, appName, filePath),
            _ => SxcPath(contentRootPath, alias, appName, filePath),
        };

        // Check that file exist in file system.
        var exists = System.IO.File.Exists(fullFilePath);
        return l.Return(exists ? fullFilePath : string.Empty, exists? "found" : "file not found");
    }

    private static bool IsKnownRiskyExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(extension) && RiskyDetector.IsMatch(extension);
    }

    private static string AdamPathWithoutAppName(string contentRootPath, Alias alias, string filePath)
        => Path.Combine(contentRootPath, string.Format(OqtConstants.ContentRootPublicBase, alias.TenantId, alias.SiteId), filePath).Backslash();

    private static string AdamPath(string contentRootPath, Alias alias, string appName, string filePath)
        => Path.Combine(contentRootPath, string.Format(OqtConstants.ContentRootPublicBase, alias.TenantId, alias.SiteId), "adam", appName, filePath).Backslash();

    private static string SxcPath(string contentRootPath, Alias alias, string appName, string filePath)
        => Path.Combine(contentRootPath, string.Format(OqtConstants.AppRootPublicBase, alias.SiteId), appName, filePath).Backslash();

    private static string SharedPath(string contentRootPath, string appName, string filePath)
        => Path.Combine(contentRootPath, string.Format(OqtConstants.AppRootPublicBase, "Shared"), appName, filePath).Backslash();

}