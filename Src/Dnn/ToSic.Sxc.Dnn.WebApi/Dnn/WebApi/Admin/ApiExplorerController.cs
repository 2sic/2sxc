using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
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
using ToSic.Eav.Helpers;
using ToSic.Sxc.WebApi.ApiExplorer;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ApiExplorerController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.Explorer";

        [HttpGet]
        public HttpResponseMessage Inspect(string path)
        {
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
                var controllerVirtualPath = Path.Combine(AppFolderUtilities.GetAppFolderVirtualPath(Request, Log), path);
                Log.Add($"Controller Virtual Path: {controllerVirtualPath}");

                if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
                {
                    var msg = $"Error: can't find controller file: {controllerVirtualPath}";
                    return wrapLog(msg, Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg));
                }

                var assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);

                var controllerName = path.Substring(path.LastIndexOf('\\') + 1);
                controllerName = controllerName.Substring(0, controllerName.IndexOf('.'));
                var controller = assembly.DefinedTypes.FirstOrDefault(a => controllerName.Equals(a.Name, StringComparison.InvariantCultureIgnoreCase));
                if (controller == null)
                {
                    var msg = $"Error: can't find controller class: {controllerName} in file {Path.GetFileNameWithoutExtension(path)}. This can happen if the controller class does not have the same name as the file.";
                    return wrapLog(msg, Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg));
                }

                var controllerSecurity = GetSecurity(controller);
                var controllerDto = new ApiControllerDto
                {
                    controller = controller.Name,
                    actions = controller.GetMethods()
                        .Where(methodInfo => methodInfo.IsPublic
                                             && !methodInfo.IsSpecialName
                                             && GetHttpVerbs(methodInfo).Count > 0)
                        .Select(methodInfo =>
                        {
                            var methodSecurity = GetSecurity(methodInfo);
                            var mergedSecurity = MergeSecurity(controllerSecurity, methodSecurity);
                            return new ApiActionDto
                            {
                                name = methodInfo.Name,
                                verbs = GetHttpVerbs(methodInfo).Select(m => m.ToUpperInvariant()),
                                parameters = methodInfo.GetParameters().Select(p => new ApiActionParamDto
                                {
                                    name = p.Name,
                                    type = ApiExplorerJs.JsTypeName(p.ParameterType),
                                    defaultValue = p.DefaultValue,
                                    isOptional = p.IsOptional,
                                    isBody = IsBody(p),
                                }).ToArray(),
                                security = methodSecurity,
                                mergedSecurity = mergedSecurity,
                                returns = ApiExplorerJs.JsTypeName(methodInfo.ReturnType),
                            };
                        }),
                    security = controllerSecurity
                };

                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(controllerDto.ToJson(), Encoding.UTF8, "application/json");
                return wrapLog("ok", responseMessage);
            }
            catch (Exception exc)
            {
                return wrapLog($"Error: {exc.Message}.", Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc));
            }
        }





        #region Inspect parameters / attributes

        private static bool IsBody(ParameterInfo paramInfo)
        {
            return paramInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(FromBodyAttribute));
        }

        private static List<string> GetHttpVerbs(MethodInfo methodInfo)
        {
            var httpMethods = new List<string>();

            // @STV TODO: Pls use this method in all the code - it's shorter and less duplicate/error prone
            // Also applies to GetSecurity below
            var getAtt = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            if (getAtt != null) httpMethods.Add(getAtt.HttpMethods[0].Method);
            // old
            //if (methodInfo.GetCustomAttributes(typeof(HttpGetAttribute)).FirstOrDefault() is HttpGetAttribute getAttribute)
            //    httpMethods.Add(getAttribute.HttpMethods[0].Method);

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

        private static ApiSecurityDto GetSecurity(MemberInfo member)
        {
            var dnnAuthList = member.GetCustomAttributes<DnnModuleAuthorizeAttribute>().ToList();
            
            return new ApiSecurityDto
            {
                ignoreSecurity = member.GetCustomAttribute<AllowAnonymousAttribute>() != null,
                allowAnonymous = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Anonymous),
                requireVerificationToken = member.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() != null,
                superUser = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Host),
                admin = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Admin),
                edit = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Edit),
                view = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.View),
                // if it has any dnn authorize attributes or supported-modules it needs the context
                requireContext = dnnAuthList.Any() || member.GetCustomAttribute<SupportedModulesAttribute>() != null,
            };
        }
        private static ApiSecurityDto MergeSecurity(ApiSecurityDto contSec, ApiSecurityDto methSec)
        {
            // TODO: @STV - if the method requires admin, and the controller has dnn-anonymous or requires edit, are both required, or just the method?
            var ignoreSecurity = contSec.ignoreSecurity || contSec.ignoreSecurity;
            var view = !ignoreSecurity && (contSec.view || methSec.view);
            var edit = !ignoreSecurity && (view || contSec.edit || methSec.edit);
            var admin = !ignoreSecurity && (edit || contSec.admin || methSec.admin);
            var superUser = !ignoreSecurity && (admin || contSec.superUser || methSec.superUser);

            // TODO: @STV - does it need context, if security is ignored?
            var requireContext = !ignoreSecurity && (contSec.requireContext || methSec.requireContext);


            return new ApiSecurityDto
            {
                ignoreSecurity = ignoreSecurity,
                allowAnonymous = ignoreSecurity || contSec.allowAnonymous || methSec.allowAnonymous,
                requireVerificationToken = !ignoreSecurity && (contSec.requireVerificationToken || methSec.requireVerificationToken),
                view = view,
                edit = edit,
                admin = admin,
                superUser = superUser,
                requireContext = requireContext,
            };
        }

        private static ApiSecurityDto GetSecurityOld(TypeInfo controllerInfo, MethodInfo methodInfo)
        {
            var allowAnonymous =
                (controllerInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).FirstOrDefault() is AllowAnonymousAttribute)
                || (methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).FirstOrDefault() is AllowAnonymousAttribute);

            var dnnAnonymous =
                (controllerInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att1 && att1.AccessLevel == SecurityAccessLevel.Anonymous)
                || (methodInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att2 && att2.AccessLevel == SecurityAccessLevel.Anonymous);

            var requireVerificationToken =
                (controllerInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute)).FirstOrDefault() is ValidateAntiForgeryTokenAttribute)
                || (methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute)).FirstOrDefault() is ValidateAntiForgeryTokenAttribute);

            var superUser =
                (controllerInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att3 && att3.AccessLevel == SecurityAccessLevel.Host)
                 || (methodInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att4 && att4.AccessLevel == SecurityAccessLevel.Host);

            var admin =
                (controllerInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att5 && att5.AccessLevel == SecurityAccessLevel.Admin)
                 || (methodInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute att6 && att6.AccessLevel == SecurityAccessLevel.Admin);

            var dnnModuleAuthorize =
                (controllerInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute)
                || (methodInfo.GetCustomAttributes(typeof(DnnModuleAuthorizeAttribute)).FirstOrDefault() is DnnModuleAuthorizeAttribute);

            var supportedModulesAttribute =
                (controllerInfo.GetCustomAttributes(typeof(SupportedModulesAttribute)).FirstOrDefault() is SupportedModulesAttribute)
                || (methodInfo.GetCustomAttributes(typeof(SupportedModulesAttribute)).FirstOrDefault() is SupportedModulesAttribute);


            return new ApiSecurityDto
            {
                allowAnonymous = (allowAnonymous || dnnAnonymous && !superUser && !admin),
                requireVerificationToken = requireVerificationToken,
                superUser = superUser && !allowAnonymous,
                admin = admin && !allowAnonymous,
                requireContext = dnnModuleAuthorize && !allowAnonymous || supportedModulesAttribute,
            };
        }
        #endregion


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

        //        var responseMessage = request.CreateResponse(HttpStatusCode.OK);

        //        responseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

        //        return responseMessage;
        //    }
        //    catch (Exception exc)
        //    {
        //        return request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
        //    }
        //}
    }


}


