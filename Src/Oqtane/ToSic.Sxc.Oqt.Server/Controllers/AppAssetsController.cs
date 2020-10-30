using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route("{alias}/api/[controller]")]
    //[Route("{alias}/api/sxc/adam")]
    //[Route("{alias}/api/sxc/assets")]
    public class AppAssetsController : Controller
    {
        public virtual string Route => "default";
        private readonly ILogManager _logger;
        private readonly ITenantResolver _tenantResolver;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly List<string> _whiteListExtensions = new List<string> {".js", ".css", ".json", ".map", ".xml", ".csv", ".txt" };
        private const string RiskyExtensionsAll =
            @"^\.\s*(ade|adp|app|bas|bat|chm|class|cmd|com|cpl|crt|dll|exe|fxp|hlp|hta|ins|isp|jse|lnk|mda|mdb|mde|mdt|mdw|mdz|msc|msi|msp|mst|ops|pcd|pif|prf|prg|reg|scf|scr|sct|shb|shs|url|vb|vbe|vbs|wsc|wsf|wsh|cshtml|vbhtml|cs|ps[0-9]|ascx|aspx|asmx|config|inc|html|sql|bin|iso|asp|sh|php([0-9])?|pl|cgi|386|torrent|jar|vbscript|cer|csr|jsp|drv|sys|csh|inf|htaccess|htpasswd|ksh)\s*$";
        private static readonly Regex RiskyDetector = new Regex(RiskyExtensionsAll);

        public AppAssetsController(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ILogManager logger)
        {
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        // GET: 1/api/adam/ping
        [HttpGet("ping")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "<Pending>")]
        public string Ping()
        {
            return $"pong: {Route}";
        }

        // GET /api/appAssets/{appName}/{filePath}
        [HttpGet("{appName}/{*filePath}")]
        public IActionResult GetFile(string appName, string filePath)
        {
            try
            {
                // Blacklist extensions should be denied.
                if (IsKnownRiskyExtension(filePath)) return NotFound();
                if (Eav.Security.Files.FileNames.IsKnownCodeExtension(filePath)) return NotFound();

                // Whitelist extensions like, js, css, json, map, xml, csv should all be fine or
                // any mime-type image/...  should be fine.
                var mimeType = GetMimeType(filePath);
                //if (!_whiteListExtensions.Contains(Path.GetExtension(filePath)) && (!mimeType.StartsWith("image"))) return NotFound();

                // Nothing in a ".xyz" folder or a subfolder of this should be allowed (like .data must be protected).
                if (appName.StartsWith(".") || filePath.StartsWith(".") || filePath.Contains("/.")) return NotFound();

                // Validate for alias.
                var alias = _tenantResolver.GetAlias();
                if (alias == null) return NotFound();

                var folder = "2sxc";
                if (Route == "adam") folder = "adam";
                if (Route == "sxc") folder = "2sxc";

                var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\{folder}";
                var appPath = Path.Combine(_hostingEnvironment.ContentRootPath, aliasPart);
                var fullAppPath = Path.Combine(appPath, appName);

                // Check that folder with appName exists.
                if (Directory.GetDirectories(appPath).All(folder => folder != fullAppPath)) return NotFound();

                // Check that file exist in file system.
                var fullFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, aliasPart, appName, filePath).Replace("/", @"\");
                if (!System.IO.File.Exists(fullFilePath)) return NotFound();

                // Check that file with filePath exists in appPath.
                if (GetAllFiles(appPath).All(file => file != fullFilePath)) return NotFound();

                var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);

                return new FileContentResult(fileBytes, mimeType)
                {
                    FileDownloadName = Path.GetFileName(fullFilePath)
                };
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                return NotFound();
            }
        }

        // Get all files in directory and subdirectories.
        public List<string> GetAllFiles(string folder)
        {
            var files = Directory.GetFiles(folder).ToList();
            var subFolders = Directory.GetDirectories(folder);

            // Recurse into subdirectories of  directory.
            foreach (var subFolder in subFolders)
                files.AddRange(GetAllFiles(subFolder));

            return files;
        }

        public static bool IsKnownRiskyExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension) && RiskyDetector.IsMatch(extension);
        }
    }
}
