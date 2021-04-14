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
using DotNetNuke.Security;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApiRouting;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ApiExplorerController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.Explorer";

        private const string ErrPrefix = "Api Explorer Controller Finder Error: ";
        private const string ErrSuffix = "Check event-log, code and inner exception. ";

        [HttpGet]
        public HttpResponseMessage Inspect(string path)
        {
            // @STV: most classes have a Log, you should not really create a new one because it will be missing in the insights
            //var log = new Log("Api.Explorer", null, Request?.RequestUri?.AbsoluteUri);
            
            var wrapLog = Log.Call<HttpResponseMessage>();

            Log.Add($"Controller Path from appRoot: {path}");

            if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
            {
                var msg = $"Error: bad parameter {path}";
                return wrapLog(msg, Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg));
            }

            // Ensure make windows path slashes to make later work easier
            path = path.Backslash();
            

            try
            {
                var controllerVirtualPath = Path.Combine(GetAppFolderRelative(), path);
                Log.Add($"Controller Virtual Path: {controllerVirtualPath}");

                if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
                {
                    var msg = $"Error: can't find controller {controllerVirtualPath}";
                    return wrapLog(msg, Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg));
                }

                var assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);

                // @STV - Had to optimize to only get the type which has the name of the controller-class file, not all classes
                // also don't just get our ApiControllers - any valid API controller would do
                //var apiControllers = assembly.DefinedTypes.Where(a =>
                //    a.BaseType == typeof(ApiController)
                //    || a.BaseType == typeof(DnnApiController));
                
                var controllerName = path.Substring(path.LastIndexOf('\\') + 1);
                controllerName = controllerName.Substring(0, controllerName.IndexOf('.'));
                var controller = assembly.DefinedTypes.FirstOrDefault(a => controllerName.Equals(a.Name, StringComparison.InvariantCultureIgnoreCase));

                var controllerDto = controller == null ? null : new
                {
                    controller = controller.Name,
                    actions = controller.GetMethods().Where(methodInfo => methodInfo.IsPublic && !methodInfo.IsSpecialName && GetHttpVerbs(methodInfo).Count > 0).Select(methodInfo => new
                    {
                        name = methodInfo.Name,
                        verbs = GetHttpVerbs(methodInfo).Select(m => m.ToUpperInvariant()),
                        parameters = methodInfo.GetParameters().Select(p => new
                        {
                            name = p.Name,
                            type = JsTypeName(p.ParameterType),
                            defaultValue = p.DefaultValue,
                            isOptional = p.IsOptional,
                        }),
                        returns = JsTypeName(methodInfo.ReturnType),
                    })
                };
                var json = controllerDto.ToJson();

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

            // @STV - better use pattern matching, shorter, simpler (Resharper) - before was more like this:
            //var acceptVerbsAttribute = methodInfo.GetCustomAttributes(typeof(AcceptVerbsAttribute)).FirstOrDefault() as AcceptVerbsAttribute;
            //if (acceptVerbsAttribute != null) httpMethods.AddRange(acceptVerbsAttribute.HttpMethods.Select(m => m.Method));

            if (methodInfo.GetCustomAttributes(typeof(HttpGetAttribute)).FirstOrDefault() is HttpGetAttribute getAttribute) 
                httpMethods.Add(getAttribute.HttpMethods[0].Method);

            if (methodInfo.GetCustomAttributes(typeof(HttpPostAttribute)).FirstOrDefault() is HttpPostAttribute postAttribute) 
                httpMethods.Add(postAttribute.HttpMethods[0].Method);

            if (methodInfo.GetCustomAttributes(typeof(HttpPutAttribute)).FirstOrDefault() is HttpPutAttribute putAttribute)
                httpMethods.Add(putAttribute.HttpMethods[0].Method);

            if (methodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute)).FirstOrDefault() is HttpDeleteAttribute deleteAttribute)
                httpMethods.Add(deleteAttribute.HttpMethods[0].Method);

            if (methodInfo.GetCustomAttributes(typeof(AcceptVerbsAttribute)).FirstOrDefault() is AcceptVerbsAttribute acceptVerbsAttribute)
                httpMethods.AddRange(acceptVerbsAttribute.HttpMethods.Select(m => m.Method));

            return httpMethods;
        }

        /// <summary>
        /// Give common type names a simple naming and only return the original for more complex types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string JsTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var mainName = type.Name;
                if (mainName.Contains("`")) mainName = mainName.Substring(0, mainName.IndexOf('`'));
                var parts = type.GenericTypeArguments.Select(t => JsTypeName(t));
                return $"{mainName}<{string.Join(", ", parts)}>";
            }

            if (type.IsArray) return JsTypeName(type.GetElementType()) + "[]";
            
            if (type == typeof(string)) return "string";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long int";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime)) return "datetime as string";
            if (type == typeof(Guid)) return "guid as string";
            // in case we don't know let's just return the hardcore name
            return type.FullName;
        }

        // @STV - this feels like very duplicate code 
        private string GetAppFolderRelative()
        {
            //var log = new Log("Api.Explorer.AppFolder", null, Request?.RequestUri?.AbsoluteUri);

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
