using System.Net;
using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Integration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnWebApiAppFolderUtilities(
    LazySvc<IContextCompleteService> ctxService,
    LazySvc<IContextBuilderService> ctxBuilder,
    LazySvc<CodeErrorHelpService> errorHelp)
    : ServiceWithSetup<HttpRequestMessage>($"{LogName}.AppFld", connect: [errorHelp, ctxService, ctxBuilder])
{
    /// <summary>
    /// Request object - throwing if not properly initialized
    /// </summary>
    private HttpRequestMessage Request => MyOptions ?? throw new("Request not available - call Setup(...) first!");

    /// <summary>
    /// Figure out the App folder based on the Http request
    /// </summary>
    /// <param name="errorIfNotFound"></param>
    /// <param name="blockOrNull"></param>
    /// <returns></returns>
    internal string GetAppFolder(bool errorIfNotFound, IBlock? blockOrNull)
    {
        var l = Log.Fn<string>();
        const string errPrefix = "Api Controller Finder Error: ";
        const string errSuffix = "Check event-log, code and inner exception. ";

        var routeData = Request.GetRouteData();

        // Figure out the Path, or show error for that.
        try
        {
            // 1. Try to get it from the .net Routing Values
            var appFolder = routeData.Values[VarNames.AppPath]?.ToString();

            // only check for app folder if we don't have a context
            if (appFolder == null)
            {
                l.A("no folder found in url, will auto-detect");
                appFolder = ctxService.Value.FindAppFolderFromScopeOrThrow(ctxBuilder.Value);
            }

            l.A($"App Folder: {appFolder}");

            // Found or silent, done.
            if (!string.IsNullOrWhiteSpace(appFolder) || !errorIfNotFound)
                return l.ReturnAsOk(appFolder);

            // Not found, and error requested - will happen outside the try-catch, so it's not re-wrapped in the cache
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

        // No return (so no !errorIfNotFound) and no exception already raised
        // your own - outside the try-catch, so it's not re-wrapped in the cache
        const string errMsg = $"{errPrefix}App name is unknown - tried to check name in url (.../app/[app-name]/...) and tried app-detection using url-params/headers pageid/moduleid. {errSuffix}";
        throw l.Done(DnnHttpErrors.LogAndReturnException(Request, HttpStatusCode.BadRequest, new(errMsg), errMsg, errorHelp.Value));
    }
}