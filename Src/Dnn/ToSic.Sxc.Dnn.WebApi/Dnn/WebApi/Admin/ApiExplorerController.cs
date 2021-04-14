using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApiRouting;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [AllowAnonymous]
    public class ApiExplorerController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.Explorer";

        private const string ErrPrefix = "Api Explorer Controller Finder Error: ";
        private const string ErrSuffix = "Check event-log, code and inner exception. ";

        public HttpResponseMessage GetAppApi(string controllerPath)
        {
            Log.Add($"Api.Explorer path: {Request?.RequestUri?.AbsoluteUri}");
            var wrapLog = Log.Call<HttpResponseMessage>();

            Log.Add($"Controller Path from appRoot: {controllerPath}");

            try
            {
                var controllerVirtualPath = Path.Combine(GetAppFolderRelative(), controllerPath);
                Log.Add($"Controller Virtual Path: {controllerVirtualPath}");

                if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
                {
                    return wrapLog($"Error: can't find controller {controllerVirtualPath}", Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error: can't find controller."));
                }

                var assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);

                var apiControllers = assembly.DefinedTypes.Where(a =>
                    a.BaseType == typeof(ApiController)
                    || a.BaseType == typeof(DnnApiController));

                var json = apiControllers.Select(controller => new
                {
                    controller = controller.Name,
                    methods = controller.GetMethods().Where(methodInfo => methodInfo.IsPublic && !methodInfo.IsSpecialName && GetHttpVerbs(methodInfo).Count > 0).Select(methodInfo => new
                    {
                        action = methodInfo.Name,
                        httpMethods = GetHttpVerbs(methodInfo),
                        parametres = methodInfo.GetParameters().Select(p => new
                        {
                            name = p.Name,
                            type = p.ParameterType.FullName,
                            defaultValue = p.DefaultValue,
                            isOptional = p.IsOptional
                        }),
                        returnType = methodInfo.ReturnType.FullName,
                    })
                }).ToJson();

                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

                responseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return wrapLog("ok", responseMessage);
            }
            catch (Exception exc)
            {
                return wrapLog($"Error: {exc.Message}.", Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc));
            }
        }

        private static List<string> GetHttpVerbs(MethodInfo methodInfo)
        {
            var httpMethods = new List<string>();

            var getAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute)).FirstOrDefault() as HttpGetAttribute;
            if (getAttribute != null) httpMethods.Add(getAttribute.HttpMethods[0].Method.ToUpperInvariant());

            var postAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute)).FirstOrDefault() as HttpPostAttribute;
            if (postAttribute != null) httpMethods.Add(postAttribute.HttpMethods[0].Method.ToUpperInvariant());

            var putAttribute = methodInfo.GetCustomAttributes(typeof(HttpPutAttribute)).FirstOrDefault() as HttpPutAttribute;
            if (putAttribute != null) httpMethods.Add(putAttribute.HttpMethods[0].Method.ToUpperInvariant());

            var deleteAttribute = methodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute)).FirstOrDefault() as HttpDeleteAttribute;
            if (deleteAttribute != null) httpMethods.Add(deleteAttribute.HttpMethods[0].Method.ToUpperInvariant());

            var acceptVerbsAttribute = methodInfo.GetCustomAttributes(typeof(AcceptVerbsAttribute)).FirstOrDefault() as AcceptVerbsAttribute;
            if (acceptVerbsAttribute != null) httpMethods.AddRange(acceptVerbsAttribute.HttpMethods.Select(m => m.Method.ToUpperInvariant()));

            return httpMethods;
        }

        private string GetAppFolderRelative()
        {
            var wrapLog = Log.Call<string>();

            var routeData = Request.GetRouteData();

            // Figure out the Path, or show error for that.
            string appFolder = null;
            try
            {
                appFolder = Route.AppPathOrNull(routeData);

                // only check for app folder if we don't have a context
                if (appFolder == null)
                {
                    Log.Add("no folder found in url, will auto-detect");
                    var block = Eav.Factory.StaticBuild<DnnGetBlock>().GetCmsBlock(Request, Log);
                    appFolder = block?.App?.Folder;
                }

                Log.Add($"App Folder: {appFolder}");
            }
            catch (Exception getBlockException)
            {
                const string msg = ErrPrefix + "Trying to find app name, unexpected error - possibly bad/invalid headers. " + ErrSuffix;
                throw ReportToLogAndThrow(Request, HttpStatusCode.BadRequest, getBlockException, msg, wrapLog);
            }

            if (string.IsNullOrWhiteSpace(appFolder))
            {
                const string msg = ErrPrefix + "App name is unknown - tried to check name in url (.../app/[app-name]/...) " +
                                   "and tried app-detection using url-params/headers pageid/moduleid. " + ErrSuffix;
                throw ReportToLogAndThrow(Request, HttpStatusCode.BadRequest, new Exception(msg), msg, wrapLog);
            }

            var tenant = Eav.Factory.StaticBuild<DnnSite>();
            var appFolderRelative = Path.Combine(tenant.AppsRootRelative, appFolder);
            appFolderRelative = appFolderRelative.Replace("\\", @"/");

            return wrapLog($"Ok, App Folder Relative: {appFolderRelative}", appFolderRelative);
        }

        private static HttpResponseException ReportToLogAndThrow(HttpRequestMessage request, HttpStatusCode code, Exception e, string msg, Func<string, string, string> wrapLog)
        {
            var helpText = ErrorHelp.HelpText(e);
            var exception = new Exception(msg + helpText, e);
            DotNetNuke.Services.Exceptions.Exceptions.LogException(exception);
            wrapLog("error", null);
            return new HttpResponseException(request.CreateErrorResponse(code, exception.Message, e));
        }

        //public HttpResponseMessage Get(string relativePath = "")
        //{
        //    try
        //    {
        //        var apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();

        //        var apis = apiExplorer.ApiDescriptions.Where(api => api.RelativePath.Contains(relativePath));

        //        var json = apis.Select(api => new
        //        {
        //            id = api.ID,
        //            relativePath = api.RelativePath,
        //            httpMethod = api.HttpMethod,
        //            controller = api.ActionDescriptor.ControllerDescriptor.ControllerName,
        //            action = api.ActionDescriptor.ActionName,
        //            parametres = api.ParameterDescriptions.Select(p => new
        //            {
        //                name = p.Name,
        //                source = p.Source.ToString(),
        //                type = p.ParameterDescriptor.ParameterType.FullName,
        //                defaultValue = p.ParameterDescriptor.DefaultValue,
        //                isOptional = p.ParameterDescriptor.IsOptional
        //            })
        //        }).ToJson();

        //        var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

        //        responseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //        return responseMessage;
        //    }
        //    catch (Exception exc)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
        //    }
        //}
    }
}
