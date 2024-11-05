using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Oqtane.Shared;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class AppAssetsControllerBase : OqtControllerBase
{
    private string Route { get; }

    #region Dependencies

    public class MyServices : MyServicesBase
    {
        internal LazySvc<OqtAssetsFileHelper> FileHelper { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public LazySvc<AppFolder> AppFolder { get; }
        public SiteState SiteState { get; }

        public MyServices(
            IWebHostEnvironment hostingEnvironment,
            LazySvc<AppFolder> appFolder,
            SiteState siteState,
            LazySvc<OqtAssetsFileHelper> fileHelper
        )
        {
            ConnectLogs([
                HostingEnvironment = hostingEnvironment,
                AppFolder = appFolder,
                SiteState = siteState,
                FileHelper = fileHelper
            ]);
        }
    }

    #endregion


    protected AppAssetsControllerBase(MyServices services, string route, string logSuffix): base(false, logSuffix)
    {
        Deps = services.ConnectServices(Log);
        Route = route;
    }

    private MyServices Deps;

    [HttpGet("{*filePath}")]
    public IActionResult GetFile([FromRoute] string appName, [FromRoute] string filePath)
    {
        var l = Log.Fn<IActionResult>($"{nameof(appName)}: {appName}; {nameof(filePath)}: {filePath}");
        try
        {
            if (appName == OqtWebApiConstants.Auto) appName = Deps.AppFolder.Value.GetAppFolder();

            var alias = Deps.SiteState.Alias;
            var fullFilePath = Deps.FileHelper.Value.GetFilePath(Deps.HostingEnvironment.ContentRootPath, alias, Route, appName, filePath);
            if (string.IsNullOrEmpty(fullFilePath))
                return l.Return(NotFound(), "empty path");

            var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
            var mimeType = OqtAssetsFileHelper.GetMimeType(fullFilePath);

            var result = mimeType.StartsWith("image") ? File(fileBytes, mimeType) :
                new(fileBytes, mimeType) { FileDownloadName = Path.GetFileName(fullFilePath) };

            return l.Return(result, "found");
        }
        catch
        {
            return l.Return(NotFound(), "error");
        }
    }
}