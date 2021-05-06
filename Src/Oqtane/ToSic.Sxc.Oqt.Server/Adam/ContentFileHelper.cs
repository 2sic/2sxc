using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam
{
    public static class ContentFileHelper
    {
        public static readonly Regex RiskyDetector = ToSic.Eav.Security.Files.FileNames.RiskyDownloadDetector;// new Regex(RiskyExtensionsAll);

        //public const string RiskyExtensionsAll =
        //    @"^\.\s*(ade|adp|app|bas|bat|chm|class|cmd|com|cpl|crt|dll|exe|fxp|hlp|hta|ins|isp|jse|lnk|mda|mdb|mde|mdt|mdw|mdz|msc|msi|msp|mst|ops|pcd|pif|prf|prg|reg|scf|scr|sct|shb|shs|url|vb|vbe|vbs|wsc|wsf|wsh|cshtml|vbhtml|cs|ps[0-9]|ascx|aspx|asmx|config|inc|html|sql|bin|iso|asp|sh|php([0-9])?|pl|cgi|386|torrent|jar|vbscript|cer|csr|jsp|drv|sys|csh|inf|htaccess|htpasswd|ksh)\s*$";

        public const string FallbackMimeType = "application/octet-stream";
        
        public static string GetMimeType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return FallbackMimeType;
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType)) 
                contentType = FallbackMimeType;
            return contentType;
        }

        public static string GetFilePath(string contentRootPath, Alias alias, string filePath)
        {
            return GetFilePath(contentRootPath, alias, string.Empty,  string.Empty, filePath);
        }

        public static string GetFilePath(string contentRootPath, Alias alias, string route, string appName, string filePath)
        {
            // Oqtane path and file name validation.
            // Partly commented because Path validation is not working as expected.
            if (!appName.IsPathOrFileValid() /*|| !filePath.Backslash().IsPathOrFileValid()*/) return string.Empty;

            // Blacklist extensions should be denied.
            if (IsKnownRiskyExtension(filePath)) return string.Empty;

            if (Eav.Security.Files.FileNames.IsKnownCodeExtension(filePath)) return string.Empty;

            // Nothing in a ".xyz" folder or a subfolder of this should be allowed (like .data must be protected).
            if (appName.StartsWith(".") || filePath.StartsWith(".") || filePath.Backslash().Contains(@"\.")) return string.Empty;

            // Validate for alias.
            if (alias == null) return string.Empty;

            var fullFilePath = route switch
            {
                "" => AdamPathWithoutAppName(contentRootPath, alias, filePath),
                "adam" => AdamPath(contentRootPath, alias, appName, filePath),
                "sxc" => SxcPath(contentRootPath, alias, appName, filePath),
                _ => SxcPath(contentRootPath, alias, appName, filePath),
            };

            // Check that file exist in file system.
            return System.IO.File.Exists(fullFilePath) ? fullFilePath : string.Empty;
        }

        public static bool IsKnownRiskyExtension(string fileName)
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

    }
}