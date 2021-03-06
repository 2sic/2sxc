﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApiRouting;

namespace ToSic.Sxc.Dnn
{
    internal static class AppFolderUtilities
    {
        internal static string GetAppFolderVirtualPath(HttpRequestMessage request, ILog log)
        {
            var wrapLog = log.Call<string>();

            var appFolder = GetAppFolder(request, log, wrapLog);

            var tenant = Eav.Factory.StaticBuild<DnnSite>();

            var appFolderVirtualPath = Path.Combine(tenant.AppsRootRelative, appFolder).ForwardSlash();

            return wrapLog($"Ok, AppFolder Virtual Path: {appFolderVirtualPath}", appFolderVirtualPath);
        }

        internal static string GetAppFolder<T>(HttpRequestMessage request, ILog log, Func<string, T, T> wrapLog)
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
                    log.Add("no folder found in url, will auto-detect");
                    var block = Eav.Factory.StaticBuild<DnnGetBlock>().GetCmsBlock(request, log);
                    appFolder = block?.App?.Folder;
                }

                log.Add($"App Folder: {appFolder}");
            }
            catch (Exception getBlockException)
            {
                const string msg = errPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + errSuffix;
                throw ReportToLogAndThrow<T>(request, HttpStatusCode.BadRequest, getBlockException, msg, wrapLog);
            }

            if (string.IsNullOrWhiteSpace(appFolder))
            {
                const string msg = errPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                                   "and tried app-detection using url-params/headers pageid/moduleid. " + errSuffix;
                throw ReportToLogAndThrow<T>(request, HttpStatusCode.BadRequest, new Exception(msg), msg, wrapLog);
            }

            return appFolder;
        }

        internal static HttpResponseException ReportToLogAndThrow<T>(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg, Func<string, T, T> wrapLog)
        {
            var helpText = ErrorHelp.HelpText(e);
            var exception = new Exception(msg + helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            wrapLog("error", default);
            return new HttpResponseException(request.CreateErrorResponse(code, exception.Message, e));
        }
    }
}