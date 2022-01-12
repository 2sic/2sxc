using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Apps;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi
{
    public abstract class AppAssetsControllerBase : OqtControllerBase
    {
        public virtual string Route => "default";
        //private readonly ILogManager _logger;
        private readonly Lazy<OqtAppFolder> _oqtAppFolderLazy;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SiteState _siteState;

        protected AppAssetsControllerBase(AppAssetsDependencies dependencies)
        {
            _hostingEnvironment = dependencies.HostingEnvironment;
            _oqtAppFolderLazy = dependencies.OqtAppFolderLazy;
            _siteState = dependencies.SiteState;
            //_logger = dependencies.Logger;
        }

        [HttpGet("{*filePath}")]
        public IActionResult GetFile([FromRoute] string appName, [FromRoute] string filePath)
        {
            try
            {
                if (appName == WebApiConstants.Auto) appName = _oqtAppFolderLazy.Value.GetAppFolder();

                var alias = _siteState.Alias;
                var fullFilePath = ContentFileHelper.GetFilePath(_hostingEnvironment.ContentRootPath, alias, Route, appName, filePath);
                if (string.IsNullOrEmpty(fullFilePath)) return NotFound();

                var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
                var mimeType = ContentFileHelper.GetMimeType(fullFilePath);

                return mimeType.StartsWith("image") ? File(fileBytes, mimeType) :
                    new FileContentResult(fileBytes, mimeType) { FileDownloadName = Path.GetFileName(fullFilePath) };
            }
            catch
            {
                return NotFound();
            }
        }

        //// Get all files in directory and subdirectories.
        //public List<string> GetAllFiles(string folder)
        //{
        //    var files = Directory.GetFiles(folder).ToList();
        //    var subFolders = Directory.GetDirectories(folder);

        //    // Recurse into subdirectories of  directory.
        //    foreach (var subFolder in subFolders)
        //        files.AddRange(GetAllFiles(subFolder));

        //    return files;
        //}
    }
}
