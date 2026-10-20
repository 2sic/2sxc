using System.Net;
using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Integration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnWebApiAppFolderUtilities(
    Generator<AppFolderLookupForWebApi> folder,
    Generator<DnnGetBlock> dnnGetBlock,
    LazySvc<CodeErrorHelpService> errorHelp)
    : ServiceWithSetup<HttpRequestMessage>($"{LogName}.AppFld", connect: [errorHelp, folder, dnnGetBlock])
{
    /// <summary>
    /// Request object - throwing if not properly initialized
    /// </summary>
    private HttpRequestMessage Request => MyOptions ?? throw new("Request not available - call Setup(...) first!");

    /// <summary>
    /// Figure out the App folder based on the Http request
    /// </summary>
    /// <param name="errorIfNotFound"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    internal string GetAppFolder(bool errorIfNotFound, IBlock block = null)
    {
        var l = Log.Fn<string>();
        const string errPrefix = "Api Controller Finder Error: ";
        const string errSuffix = "Check event-log, code and inner exception. ";

        var routeData = Request.GetRouteData();

        // Figure out the Path, or show error for that.
        try
        {
            var appFolder = routeData.Values[VarNames.AppPath]?.ToString();

            // only check for app folder if we don't have a context
            if (appFolder == null)
            {
                l.A("no folder found in url, will auto-detect");
                var blockToCheck = block ?? dnnGetBlock.New().TryGetBlockSpecsUsingModuleAndContentBlockHeaders(Request);
                appFolder = folder.New().Init(blockToCheck).GetAppFolder();
            }

            l.A($"App Folder: {appFolder}");

            // Found or silent, done.
            if (!string.IsNullOrWhiteSpace(appFolder) || !errorIfNotFound)
                return l.ReturnAsOk(appFolder);
            
            const string errMsg = $"{errPrefix}App name is unknown - tried to check name in url (.../app/[app-name]/...) and tried app-detection using url-params/headers pageid/moduleid. {errSuffix}";
            throw l.Done(DnnHttpErrors.LogAndReturnException(Request, HttpStatusCode.BadRequest, new(errMsg), errMsg, errorHelp.Value));

        }
        catch (Exception getBlockException)
        {
            return errorIfNotFound
                ? throw l.Done(
                    DnnHttpErrors.LogAndReturnException(
                        Request,
                        HttpStatusCode.BadRequest,
                        getBlockException,
                        $"{errPrefix}Trying to find app name, unexpected error - possibly bad/invalid headers. {errSuffix}",
                        errorHelp.Value
                    )
                )
                : l.Return(null, "not found, got exception but caller said to ignore");
        }

    }
}