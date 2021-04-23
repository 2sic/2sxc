using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using System.IO;
using ToSic.Oqt.Helpers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/[controller]")]
    [Route(WebApiConstants.ApiRoot2 + "/[controller]")]
    [Route(WebApiConstants.ApiRoot3 + "/[controller]")]

    [Route(WebApiConstants.ApiRoot + "/app-assets")]
    [Route(WebApiConstants.ApiRoot2 + "/app-assets")]
    [Route(WebApiConstants.ApiRoot3 + "/app-assets")]

    // Beta routes
    [Route("{alias:int}/api/[controller]")]
    [Route(WebApiConstants.WebApiStateRoot + "/app-assets")]

    public class AppAssetsController : OqtControllerBase
    {
        public virtual string Route => "default";
        private readonly ILogManager _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AppAssetsController(IWebHostEnvironment hostingEnvironment, ILogManager logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        protected override string HistoryLogName { get; }


        [HttpGet("{appName}/{*filePath}")]
        public IActionResult GetFile(string appName, string filePath)
        {
            try
            {
                var alias = SiteState.Alias;
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
