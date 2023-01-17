using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApiRouting;

namespace ToSic.Sxc.Dnn
{
    public class DnnAppFolderUtilities: ServiceBase
    {
        private readonly Generator<AppFolder> _appFolder;
        private readonly Generator<DnnGetBlock> _dnnGetBlock;

        public DnnAppFolderUtilities(Generator<AppFolder> appFolder, Generator<DnnGetBlock> dnnGetBlock) : base($"{DnnConstants.LogName}.AppFld")
        {
            ConnectServices(
                _appFolder = appFolder,
                _dnnGetBlock = dnnGetBlock
            );
        }

        internal string GetAppFolderVirtualPath(HttpRequestMessage request, ISite site) => Log.Func(() =>
        {
            var appFolder = GetAppFolder(request, true);
            var appFolderVirtualPath = Path.Combine(((DnnSite)site).AppsRootRelative, appFolder).ForwardSlash();
            return (appFolderVirtualPath, $"Ok, AppFolder Virtual Path: {appFolderVirtualPath}");
        });

        internal string GetAppFolder(HttpRequestMessage request, bool errorIfNotFound) => Log.Func(l =>
        {
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
                        .Init(() => _dnnGetBlock.New().GetCmsBlock(request))
                        .GetAppFolder();
                }

                l.A($"App Folder: {appFolder}");
            }
            catch (Exception getBlockException)
            {
                const string msg = errPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + errSuffix;
                if (errorIfNotFound)
                    throw l.Ex(ReportToLogAndThrow(request, HttpStatusCode.BadRequest, getBlockException, msg));
                return (null, "not found, maybe error");
            }

            if (string.IsNullOrWhiteSpace(appFolder) && errorIfNotFound)
            {
                const string msg = errPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                                   "and tried app-detection using url-params/headers pageid/moduleid. " + errSuffix;
                throw l.Ex(ReportToLogAndThrow(request, HttpStatusCode.BadRequest, new Exception(msg), msg));
            }

            return (appFolder, "ok");
        });

        internal static HttpResponseException ReportToLogAndThrow(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg)
        {
            var helpText = ErrorHelp.HelpText(e);
            var exception = new Exception(msg + helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            return new HttpResponseException(request.CreateErrorResponse(code, exception.Message, e));
        }
    }
}