using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.Context;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Call;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApiRouting;

namespace ToSic.Sxc.Dnn
{
    internal static class AppFolderUtilities
    {
        internal static string GetAppFolderVirtualPath(IServiceProvider sp, HttpRequestMessage request, ISite site, ILog log)
        {
            var wrapLog = log.Fn<string>();

            var appFolder = GetAppFolder(sp, request, log, wrapLog);

            var appFolderVirtualPath = Path.Combine(((DnnSite)site).AppsRootRelative, appFolder).ForwardSlash();

            return wrapLog.Return(appFolderVirtualPath, $"Ok, AppFolder Virtual Path: {appFolderVirtualPath}");
        }

        internal static string GetAppFolder<T>(IServiceProvider sp, HttpRequestMessage request, ILog log, LogCall<T> wrapLog)
        {
            const string errPrefix = "Api Controller Finder Error: ";
            const string errSuffix = "Check event-log, code and inner exception. ";

            var routeData = request.GetRouteData();

            // Figure out the Path, or show error for that.
            string appFolder = null;
            try
            {
                appFolder = Route.AppPathOrNull(routeData);

                // only check for app folder if we don't have a context
                if (appFolder == null)
                {
                    log.A("no folder found in url, will auto-detect");
                    appFolder = sp.Build<AppFolder>()?.GetAppFolder();
                }

                log.A($"App Folder: {appFolder}");
            }
            catch (Exception getBlockException)
            {
                const string msg = errPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + errSuffix;
                throw ReportToLogAndThrow(request, HttpStatusCode.BadRequest, getBlockException, msg, wrapLog);
            }

            if (string.IsNullOrWhiteSpace(appFolder))
            {
                const string msg = errPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                                   "and tried app-detection using url-params/headers pageid/moduleid. " + errSuffix;
                throw ReportToLogAndThrow<T>(request, HttpStatusCode.BadRequest, new Exception(msg), msg, wrapLog);
            }

            return appFolder;
        }

        internal static HttpResponseException ReportToLogAndThrow<T>(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg, LogCall<T> wrapLog)
        {
            var helpText = ErrorHelp.HelpText(e);
            var exception = new Exception(msg + helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            wrapLog.Return(default, "error");
            return new HttpResponseException(request.CreateErrorResponse(code, exception.Message, e));
        }
    }
}