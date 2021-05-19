using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Sxc.WebApi.Plumbing;
#if NETFRAMEWORK
using PlatformResponseType = System.Net.Http.HttpResponseMessage;
#else
using PlatformResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.WebApi.ApiExplorer
{
    public class ApiExplorerBackend: WebApiBackendBase<ApiExplorerBackend>
    {
        public IApiInspector Inspector { get; }
        public ResponseMaker ResponseMaker { get; }

        public ApiExplorerBackend(IServiceProvider sp, IApiInspector inspector, ResponseMaker responseMaker): base(sp, "Bck.ApiExp")
        {
            Inspector = inspector;
            ResponseMaker = responseMaker;
        }

        public bool PreCheckAndCleanPath(ref string path, out PlatformResponseType error)
        {
            var wrapLog = Log.Call<bool>();

            Log.Add($"Controller Path from appRoot: {path}");

            if (string.IsNullOrWhiteSpace(path) || path.Contains(".."))
            {
                var msg = $"Error: bad parameter {path}";
                {
                    error = ResponseMaker.InternalServerError(msg);
                    return wrapLog(msg, true);
                }
            }

            // Ensure make windows path slashes to make later work easier
            path = path.Backslash();
            error = null;
            return false;
        }

        public PlatformResponseType AnalyzeClassAndCreateDto(string path, Assembly assembly)
        {
            var wrapLog = Log.Call<PlatformResponseType>();
            var controllerName = path.Substring(path.LastIndexOf('\\') + 1);
            controllerName = controllerName.Substring(0, controllerName.IndexOf('.'));
            var controller =
                assembly.DefinedTypes.FirstOrDefault(a =>
                    controllerName.Equals(a.Name, StringComparison.InvariantCultureIgnoreCase));
            if (controller == null)
            {
                var msg =
                    $"Error: can't find controller class: {controllerName} in file {Path.GetFileNameWithoutExtension(path)}. " +
                    $"This can happen if the controller class does not have the same name as the file.";
                    return wrapLog("error", ResponseMaker.InternalServerError(msg));
            }

            var controllerDto = BuildApiControllerDto(controller);

            var responseMessage = ResponseMaker.Json(controllerDto);
            return wrapLog("ok", responseMessage);

        }



        internal ApiControllerDto BuildApiControllerDto(Type controller)
        {
            var wrapLog = Log.Call<ApiControllerDto>();
            var controllerSecurity = Inspector.GetSecurity(controller);
            var controllerDto = new ApiControllerDto
            {
                controller = controller.Name,
                actions = controller.GetMethods()
                    .Where(methodInfo => methodInfo.IsPublic
                                         && !methodInfo.IsSpecialName
                                         && Inspector.GetHttpVerbs(methodInfo).Count > 0)
                    .Select(methodInfo =>
                    {
                        var methodSecurity = Inspector.GetSecurity(methodInfo);
                        var mergedSecurity = MergeSecurity(controllerSecurity, methodSecurity);
                        return new ApiActionDto
                        {
                            name = methodInfo.Name,
                            verbs = Inspector.GetHttpVerbs(methodInfo).Select(m => m.ToUpperInvariant()),
                            parameters = methodInfo.GetParameters().Select(p => new ApiActionParamDto
                            {
                                name = p.Name,
                                type = ApiExplorerJs.JsTypeName(p.ParameterType),
                                defaultValue = p.DefaultValue,
                                isOptional = p.IsOptional,
                                isBody = Inspector.IsBody(p),
                            }).ToArray(),
                            security = methodSecurity,
                            mergedSecurity = mergedSecurity,
                            returns = ApiExplorerJs.JsTypeName(methodInfo.ReturnType),
                        };
                    }),
                security = controllerSecurity
            };
            return wrapLog(null, controllerDto);
        }

        public ApiSecurityDto MergeSecurity(ApiSecurityDto contSec, ApiSecurityDto methSec)
        {
            var wrapLog = Log.Call<ApiSecurityDto>();
            var ignoreSecurity = contSec.ignoreSecurity || methSec.ignoreSecurity;
            var allowAnonymous = contSec.allowAnonymous || methSec.allowAnonymous;
            var view = contSec.view || methSec.view;
            var edit = contSec.edit || methSec.edit;
            var admin = contSec.admin || methSec.admin;
            var superUser = contSec.superUser || methSec.superUser;
            var requireContext = contSec.requireContext || methSec.requireContext;
            // AntiForgeryToken attributes on method prevails over attributes on class (last attribute wins)
            var requireVerificationToken =
                (methSec._validateAntiForgeryToken || methSec._autoValidateAntiforgeryToken ||
                 methSec._ignoreAntiforgeryToken)
                    ? methSec.requireVerificationToken
                    : contSec.requireVerificationToken;

            var result = new ApiSecurityDto
            {
                ignoreSecurity = ignoreSecurity,
                allowAnonymous = ignoreSecurity || allowAnonymous && !view && !edit && !admin && !superUser,
                view = ignoreSecurity || (allowAnonymous || view) && !edit && !admin && !superUser,
                edit = ignoreSecurity || (allowAnonymous || view || edit) && !admin && !superUser,
                admin = ignoreSecurity || (allowAnonymous || view || edit || admin) && !superUser,
                superUser = ignoreSecurity || allowAnonymous || view || edit || admin || superUser,
                requireContext = !ignoreSecurity && requireContext,
                requireVerificationToken = !ignoreSecurity && requireVerificationToken,
            };
            return wrapLog(null, result);
        }

    }
}
