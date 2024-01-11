using System.IO;
using System.Net;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Dnn.WebApi;

namespace ToSic.Sxc.Dnn.Integration;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnAppFolderUtilities: ServiceBase
{
    private readonly LazySvc<CodeErrorHelpService> _errorHelp;
    private readonly Generator<AppFolder> _appFolder;
    private readonly Generator<DnnGetBlock> _dnnGetBlock;

    public DnnAppFolderUtilities(Generator<AppFolder> appFolder, Generator<DnnGetBlock> dnnGetBlock, LazySvc<CodeErrorHelpService> errorHelp) : base($"{DnnConstants.LogName}.AppFld")
    {
        _errorHelp = errorHelp;
        ConnectServices(
            _appFolder = appFolder,
            _dnnGetBlock = dnnGetBlock
        );
    }

    internal string GetAppFolderVirtualPath(HttpRequestMessage request, ISite site)
    {
        var l = Log.Fn<string>();
        var appFolder = GetAppFolder(request, true);
        var appFolderVirtualPath = Path.Combine(site.AppsRootPhysical, appFolder).ForwardSlash();
        return l.Return(appFolderVirtualPath, $"Ok, AppFolder Virtual Path: {appFolderVirtualPath}");
    }

    internal string GetAppFolder(HttpRequestMessage request, bool errorIfNotFound)
    {
        var l = Log.Fn<string>();
        const string errPrefix = "Api Controller Finder Error: ";
        const string errSuffix = "Check event-log, code and inner exception. ";

        var routeData = request.GetRouteData();

        // Figure out the Path, or show error for that.
        string appFolder;
        try
        {
            appFolder = Route.AppPathOrNull(routeData);

            // only check for app folder if we don't have a context
            if (appFolder == null)
            {
                l.A("no folder found in url, will auto-detect");
                appFolder = _appFolder.New()?
                    .Init(_dnnGetBlock.New().GetCmsBlock(request))
                    .GetAppFolder();
            }

            l.A($"App Folder: {appFolder}");
        }
        catch (Exception getBlockException)
        {
            const string msg = errPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + errSuffix;
            if (errorIfNotFound)
                throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.BadRequest, getBlockException, msg, _errorHelp.Value));
            return l.Return(null, "not found, maybe error");
        }

        if (string.IsNullOrWhiteSpace(appFolder) && errorIfNotFound)
        {
            const string msg = errPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                               "and tried app-detection using url-params/headers pageid/moduleid. " + errSuffix;
            throw l.Done(DnnHttpErrors.LogAndReturnException(request, HttpStatusCode.BadRequest, new(msg), msg, _errorHelp.Value));
        }

        return l.ReturnAsOk(appFolder);
    }

}