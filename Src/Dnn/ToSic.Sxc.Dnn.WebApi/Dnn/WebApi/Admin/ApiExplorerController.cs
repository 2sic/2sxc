using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using DotNetNuke.Web.Api;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [AllowAnonymous]
    public class ApiExplorerController : DnnApiControllerWithFixes
    {
        protected override string HistoryLogName => "Api.Explorer";

        public HttpResponseMessage GetAppApi(string controllerPath)
        {
            try
            {
                var assembly = BuildManager.GetCompiledAssembly(controllerPath);

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

                return responseMessage;
            }
            catch (Exception exc)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
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
