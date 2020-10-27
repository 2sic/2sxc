using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Infrastructure;
using System.IO;
using System.Linq;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route("{alias}/api/[controller]")]
    public class AppAssetsController : Controller
    {
        private readonly ILogManager _logger;
        private readonly ITenantResolver _tenantResolver;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AppAssetsController(ITenantResolver tenantResolver , IHostingEnvironment hostingEnvironment, ILogManager logger)
        {
            _tenantResolver = tenantResolver;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
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
            return "pong";
        }

        // GET /api/appAssets/{appName}/{filePath}
        [HttpGet("{appName}/{*filePath}")]
        public IActionResult GetFile(string appName, string filePath)
        {
            try
            {
                // TODO: stv
                // 1. any mime-image/... type should be fine
	            // 2. extensions like, js, css, json, .map, xml, csv. should all be fine.
	            // 3. nothing in a ".xyz" folder or a subfolder of this should be allowed (like .data must be protected)
	            // 4. anything in the ToSic.Eav.Security.Files.FileNames.IsKnownRisyExtension(...) should be denied

                var alias = _tenantResolver.GetAlias();
                if (alias == null) return NotFound();

                var aliasPart = $@"Content\Tenants\{alias.TenantId}\Sites\{alias.SiteId}\2sxc";
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
                var mimeType = GetMimeType(fullFilePath);

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
    }
}
