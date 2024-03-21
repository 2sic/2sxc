using System.IO;
using System.Net;
using System.Web.Http.Routing;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.WebApi.Routing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Dnn.Integration;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnAppFolderUtilities(
    Generator<AppFolder> folder,
    Generator<DnnGetBlock> dnnGetBlock,
    LazySvc<CodeErrorHelpService> errorHelp)
    : ServiceBase($"{DnnConstants.LogName}.AppFld", connect: [errorHelp, folder, dnnGetBlock])
{
    private HttpRequestMessage _request;

    private HttpRequestMessage Request => _request ?? throw new Exception("Request not available - call Setup(...) first!");

    public DnnAppFolderUtilities Setup(HttpRequestMessage request)
    {
        _request = request;
        return this;
    }

    internal string GetAppFolderVirtualPath(ISite site)
    {
        var l = Log.Fn<string>();
        var appFolder = GetAppFolder(true);
        var appFolderVirtualPath = Path.Combine(site.AppsRootPhysical, appFolder).ForwardSlash();
        return l.Return(appFolderVirtualPath, $"Ok, AppFolder Virtual Path: {appFolderVirtualPath}");
    }

    internal string GetAppFolder(bool errorIfNotFound, IBlock block = null)
    {
        var l = Log.Fn<string>();
        const string errPrefix = "Api Controller Finder Error: ";
        const string errSuffix = "Check event-log, code and inner exception. ";

        var routeData = Request.GetRouteData();

        // Figure out the Path, or show error for that.
        string appFolder;
        try
        {
            appFolder = AppPathOrNull(routeData);

            // only check for app folder if we don't have a context
            if (appFolder == null)
            {
                l.A("no folder found in url, will auto-detect");
                appFolder = folder.New()
                    .Init(block ?? dnnGetBlock.New().GetCmsBlock(Request))
                    .GetAppFolder();
            }

            l.A($"App Folder: {appFolder}");
        }
        catch (Exception getBlockException)
        {
            const string msg = errPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + errSuffix;
            if (errorIfNotFound)
                throw l.Done(DnnHttpErrors.LogAndReturnException(Request, HttpStatusCode.BadRequest, getBlockException, msg, errorHelp.Value));
            return l.Return(null, "not found, maybe error");
        }

        if (string.IsNullOrWhiteSpace(appFolder) && errorIfNotFound)
        {
            const string msg = errPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                               "and tried app-detection using url-params/headers pageid/moduleid. " + errSuffix;
            throw l.Done(DnnHttpErrors.LogAndReturnException(Request, HttpStatusCode.BadRequest, new(msg), msg, errorHelp.Value));
        }

        return l.ReturnAsOk(appFolder);
    }

    public static string AppPathOrNull(IHttpRouteData route) => route.Values[VarNames.AppPath]?.ToString();

}