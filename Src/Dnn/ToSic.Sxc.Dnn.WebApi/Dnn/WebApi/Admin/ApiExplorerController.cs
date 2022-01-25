using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.IO;
using System.Net.Http;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Sxc.WebApi.Plumbing;

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

            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (DnnResponseMaker)GetService<ResponseMaker>() ;
            responseMaker.Init(this);
            
            var backend = GetService<ApiExplorerBackend>();

            if (backend.PreCheckAndCleanPath(ref path, out var error)) return error;

            try
            {
                var controllerVirtualPath = Path.Combine(AppFolderUtilities.GetAppFolderVirtualPath(GetService<IServiceProvider>(), Request, GetService<ISite>(), Log), path);
                Log.Add($"Controller Virtual Path: {controllerVirtualPath}");

                if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
                {
                    var msg = $"Error: can't find controller file: {controllerVirtualPath}";
                    return wrapLog(msg, responseMaker.InternalServerError(msg));
                }

                var assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);

                return wrapLog(null, backend.AnalyzeClassAndCreateDto(path, assembly));
                //var controllerName = path.Substring(path.LastIndexOf('\\') + 1);
                //controllerName = controllerName.Substring(0, controllerName.IndexOf('.'));
                //var controller = assembly.DefinedTypes.FirstOrDefault(a => controllerName.Equals(a.Name, StringComparison.InvariantCultureIgnoreCase));
                //if (controller == null)
                //{
                //    var msg = $"Error: can't find controller class: {controllerName} in file {Path.GetFileNameWithoutExtension(path)}. This can happen if the controller class does not have the same name as the file.";
                //    return responseMaker.InternalServerError(msg);
                //}

                //var controllerDto = backend.BuildApiControllerDto(controller);

                //var responseMessage = responseMaker.Json(controllerDto); // Request.CreateResponse(HttpStatusCode.OK);
                //// responseMessage.Content = new StringContent(controllerDto.ToJson(), Encoding.UTF8, "application/json");
                //return wrapLog("ok", responseMessage);
            }
            catch (Exception exc)
            {
                return wrapLog($"Error: {exc.Message}.", responseMaker.InternalServerError(exc));
            }
        }

        //private static bool PreCheckAndCleanPath(ref string path, Func<string, HttpResponseMessage, HttpResponseMessage> wrapLog, DnnResponseMaker responseMaker,
        //    out HttpResponseMessage inspect)
        //{
        //    if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
        //    {
        //        var msg = $"Error: bad parameter {path}";
        //        {
        //            inspect = wrapLog(msg, responseMaker.InternalServerError(msg));
        //            return true;
        //        }
        //    }

        //    // Ensure make windows path slashes to make later work easier
        //    path = path.Backslash();
        //    return false;
        //}

        //private ApiControllerDto BuildApiControllerDto(Type controller)
        //{
        //    var wrapLog = Log.Call<ApiControllerDto>();
        //    var controllerSecurity = Inspector.GetSecurity(controller);
        //    var controllerDto = new ApiControllerDto
        //    {
        //        controller = controller.Name,
        //        actions = controller.GetMethods()
        //            .Where(methodInfo => methodInfo.IsPublic
        //                                 && !methodInfo.IsSpecialName
        //                                 && Inspector.GetHttpVerbs(methodInfo).Count > 0)
        //            .Select(methodInfo =>
        //            {
        //                var methodSecurity = Inspector.GetSecurity(methodInfo);
        //                var mergedSecurity = ApiExplorerBackend.MergeSecurity(controllerSecurity, methodSecurity);
        //                return new ApiActionDto
        //                {
        //                    name = methodInfo.Name,
        //                    verbs = Inspector.GetHttpVerbs(methodInfo).Select(m => m.ToUpperInvariant()),
        //                    parameters = methodInfo.GetParameters().Select(p => new ApiActionParamDto
        //                    {
        //                        name = p.Name,
        //                        type = ApiExplorerJs.JsTypeName(p.ParameterType),
        //                        defaultValue = p.DefaultValue,
        //                        isOptional = p.IsOptional,
        //                        isBody = Inspector.IsBody(p),
        //                    }).ToArray(),
        //                    security = methodSecurity,
        //                    mergedSecurity = mergedSecurity,
        //                    returns = ApiExplorerJs.JsTypeName(methodInfo.ReturnType),
        //                };
        //            }),
        //        security = controllerSecurity
        //    };
        //    return wrapLog(null, controllerDto);
        //}


        #region Inspect parameters / attributes

        //private static bool IsBody(ParameterInfo paramInfo)
        //{
        //    return paramInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(FromBodyAttribute));
        //}

        //private static List<string> GetHttpVerbs(MethodInfo methodInfo)
        //{
        //    var httpMethods = new List<string>();

        //    var getAtt = methodInfo.GetCustomAttribute<HttpGetAttribute>();
        //    if (getAtt != null) httpMethods.Add(getAtt.HttpMethods[0].Method);

        //    var postAtt = methodInfo.GetCustomAttribute<HttpPostAttribute>();
        //    if (postAtt != null) httpMethods.Add(postAtt.HttpMethods[0].Method);

        //    var putAtt = methodInfo.GetCustomAttribute<HttpPutAttribute>();
        //    if (putAtt != null) httpMethods.Add(putAtt.HttpMethods[0].Method);

        //    var deleteAtt = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
        //    if (deleteAtt != null) httpMethods.Add(deleteAtt.HttpMethods[0].Method);

        //    var acceptVerbsAtt = methodInfo.GetCustomAttribute<AcceptVerbsAttribute>();
        //    if (acceptVerbsAtt != null) httpMethods.AddRange(acceptVerbsAtt.HttpMethods.Select(m => m.Method));

        //    return httpMethods;
        //}



        //private static ApiSecurityDto GetSecurity(MemberInfo member)
        //{
        //    var dnnAuthList = member.GetCustomAttributes<DnnModuleAuthorizeAttribute>().ToList();

        //    return new ApiSecurityDto
        //    {
        //        ignoreSecurity = member.GetCustomAttribute<AllowAnonymousAttribute>() != null,
        //        allowAnonymous = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Anonymous),
        //        requireVerificationToken = member.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() != null,
        //        superUser = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Host),
        //        admin = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Admin),
        //        edit = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Edit),
        //        view = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.View),
        //        // if it has any dnn authorize attributes or supported-modules it needs the context
        //        requireContext = dnnAuthList.Any() || member.GetCustomAttribute<SupportedModulesAttribute>() != null,
        //    };
        //}
        //private static ApiSecurityDto MergeSecurity(ApiSecurityDto contSec, ApiSecurityDto methSec)
        //{
        //    var ignoreSecurity = contSec.ignoreSecurity || methSec.ignoreSecurity;
        //    var allowAnonymous = contSec.allowAnonymous || methSec.allowAnonymous;
        //    var view = contSec.view || methSec.view;
        //    var edit = contSec.edit || methSec.edit;
        //    var admin = contSec.admin || methSec.admin;
        //    var superUser = contSec.superUser || methSec.superUser;
        //    var requireContext =  contSec.requireContext || methSec.requireContext;

        //    return new ApiSecurityDto
        //    {
        //        ignoreSecurity = ignoreSecurity,
        //        allowAnonymous = ignoreSecurity || allowAnonymous && !view && !edit && !admin && !superUser,
        //        view = ignoreSecurity || (allowAnonymous || view) && !edit && !admin && !superUser,
        //        edit = ignoreSecurity || (allowAnonymous || view || edit) && !admin && !superUser,
        //        admin = ignoreSecurity || (allowAnonymous || view || edit || admin) && !superUser,
        //        superUser = ignoreSecurity || allowAnonymous || view || edit || admin || superUser,
        //        requireContext = !ignoreSecurity && requireContext,
        //        requireVerificationToken = !ignoreSecurity && (contSec.requireVerificationToken || methSec.requireVerificationToken),
        //    };
        //}

        #endregion
    }


}


