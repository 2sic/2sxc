using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Oqt.Server.WebApi;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AppAssetsControllerBase : OqtControllerBase
{
    private string Route { get; }

    #region Dependencies

    public record Dependencies(
        IWebHostEnvironment HostingEnvironment,
        LazySvc<ISxcCurrentContextService> SxcContextSvc,
        SiteState SiteState,
        LazySvc<OqtAssetsFileHelper> FileHelper)
        : DependenciesBase(connect: [HostingEnvironment, SxcContextSvc, SiteState, FileHelper]);

    #endregion


    protected AppAssetsControllerBase(Dependencies services, string route, string logSuffix): base(false, logSuffix)
    {
        Deps = services.ConnectServices(Log);
        Route = route;
    }

    private Dependencies Deps;

    [HttpGet("{*filePath}")]
    public IActionResult GetFile([FromRoute] string appName, [FromRoute] string filePath)
    {
        var l = Log.Fn<IActionResult>($"{nameof(appName)}: {appName}; {nameof(filePath)}: {filePath}");
        try
        {
            // Note 2026-10-18 2dm: #NoLookupSideEffects
            // the next line used to have a side effect of sharing the found app-context
            // ...but it should already be handled in the previous PrepareContextResolverForApiRequest() call
            // This has been removed; it may break something, in which case sharing must be added explicitly
            if (appName == OqtWebApiConstants.Auto)
                appName = Deps.SxcContextSvc.Value.FindAppFolderFromScopeOrThrow();

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